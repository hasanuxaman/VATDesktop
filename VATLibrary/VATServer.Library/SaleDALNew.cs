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

// TransactionType no probs 
namespace VATServer.Library
{
    public class SaleDALNew 
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDAL = new CommonDAL();
        ProductDAL _ProductDAL = new ProductDAL();

        #endregion

        #region Navigation

        public NavigationVM Sale_Navigation(NavigationVM vm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
------declare @Id as int = 61731;
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
select top 1 sih.Id, sih.SalesInvoiceNo from SalesInvoiceHeaders sih
where 1=1 
and sih.SalesInvoiceNo=@InvoiceNo

";
                    #endregion
                }
                else if (vm.Id == 0 || vm.ButtonName == "First")
                {

                    #region First Item

                    sqlText = sqlText + @"
--------------------------------------------------First--------------------------------------------------
---------------------------------------------------------------------------------------------------------
select top 1 sih.Id, sih.SalesInvoiceNo from SalesInvoiceHeaders sih
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
select top 1 sih.Id, sih.SalesInvoiceNo from SalesInvoiceHeaders sih
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
select top 1 sih.Id, sih.SalesInvoiceNo from SalesInvoiceHeaders sih
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
select top 1 sih.Id, sih.SalesInvoiceNo from SalesInvoiceHeaders sih
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
                    vm.InvoiceNo = dt.Rows[0]["SalesInvoiceNo"].ToString();
                }
                else
                {
                    if (vm.ButtonName == "Previous" || vm.ButtonName == "Current")
                    {
                        vm.ButtonName = "First";
                        vm = Sale_Navigation(vm, currConn, transaction);

                    }
                    else if (vm.ButtonName == "Next")
                    {
                        vm.ButtonName = "Last";
                        vm = Sale_Navigation(vm, currConn, transaction);

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
                throw ex;
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

        #region Web Methods

        public List<SaleMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, SysDBInfoVMTemp connVM = null, string transactionType = null)
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleMasterVM> VMs = new List<SaleMasterVM>();
            SaleMasterVM vm;
            #endregion
            try
            {
                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false, transactionType);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new SaleMasterVM();
                    vm.Id = dr["Id"].ToString();

                    vm.ShiftId = dr["ShiftId"].ToString();
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.DeliveryAddress1 = dr["DeliveryAddress1"].ToString();
                    vm.DeliveryAddress2 = dr["DeliveryAddress2"].ToString();
                    vm.DeliveryAddress3 = dr["DeliveryAddress3"].ToString();
                    vm.InvoiceDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                    vm.DeliveryDate = OrdinaryVATDesktop.DateTimeToDate(dr["DeliveryDate"].ToString());

                    vm.TotalSubtotal = Convert.ToDecimal(dr["TotalSubtotal"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.VDSAmount = Convert.ToDecimal(dr["VDSAmount"].ToString());

                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.SaleType = dr["SaleType"].ToString();
                    vm.PreviousSalesInvoiceNo = dr["PreviousSalesInvoiceNo"].ToString();
                    vm.Trading = dr["Trading"].ToString();
                    vm.IsPrint = dr["IsPrint"].ToString();
                    vm.TenderId = dr["TenderId"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.LCNumber = dr["LCNumber"].ToString();
                    vm.CurrencyID = dr["CurrencyID"].ToString();
                    vm.CurrencyRateFromBDT = Convert.ToDecimal(dr["CurrencyRateFromBDT"].ToString());
                    vm.CompInvoiceNo = dr["CompInvoiceNo"].ToString();
                    vm.LCBank = dr["LCBank"].ToString();
                    vm.LCDate = dr["LCDate"].ToString();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    vm.CustomerGroup = dr["CustomerGroupName"].ToString();
                    vm.CurrencyCode = dr["CurrencyCode"].ToString();
                    vm.ValueOnly = dr["ValueOnly"].ToString();
                    ////reading newly added fields
                    vm.ReturnId = dr["SaleReturnId"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.GetVDSCertificate = dr["GetVDSCertificate"].ToString();
                    vm.VDSCertificateDate = dr["VDSCertificateDate"].ToString();
                    vm.ImportIDExcel = dr["ImportIDExcel"].ToString();
                    vm.AlReadyPrint = dr["AlReadyPrint"].ToString();
                    vm.IsGatePass = dr["IsGatePass"].ToString();
                    vm.PINo = dr["PINo"].ToString();
                    vm.PIDate = dr["PIDate"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.EXPFormNo = dr["EXPFormNo"].ToString();
                    vm.EXPFormDate = dr["EXPFormDate"].ToString();
                    vm.IsDeemedExport = dr["IsDeemedExport"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"].ToString());
                    vm.SignatoryName = dr["SignatoryName"].ToString();
                    vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                    vm.HPSTotalAmount = Convert.ToDecimal(dr["HPSTotalAmount"].ToString());


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

        public List<SaleMasterVM> SelectAllTop1(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, SysDBInfoVMTemp connVM = null, string[] IdsForIn = null)
        {
            #region Variables
            ////SqlConnection currConn = null;
            ////SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleMasterVM> VMs = new List<SaleMasterVM>();
            SaleMasterVM vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution


                DataTable dt = SelectAllTop1(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new SaleMasterVM();
                    vm.Id = dr["Id"].ToString();

                    vm.ShiftId = dr["ShiftId"].ToString();
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.CustomerName = dr["CustomerName"].ToString();
                    vm.DeliveryAddress1 = dr["DeliveryAddress1"].ToString();
                    vm.DeliveryAddress2 = dr["DeliveryAddress2"].ToString();
                    vm.DeliveryAddress3 = dr["DeliveryAddress3"].ToString();
                    vm.InvoiceDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["InvoiceDateTime"].ToString());
                    vm.DeliveryDate = OrdinaryVATDesktop.DateTimeToDate(dr["DeliveryDate"].ToString());

                    vm.TotalSubtotal = Convert.ToDecimal(dr["TotalSubtotal"].ToString());
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"].ToString());
                    vm.VDSAmount = Convert.ToDecimal(dr["VDSAmount"].ToString());

                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.SaleType = dr["SaleType"].ToString();
                    vm.PreviousSalesInvoiceNo = dr["PreviousSalesInvoiceNo"].ToString();
                    vm.Trading = dr["Trading"].ToString();
                    vm.IsPrint = dr["IsPrint"].ToString();
                    vm.TenderId = dr["TenderId"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.LCNumber = dr["LCNumber"].ToString();
                    vm.CurrencyID = dr["CurrencyID"].ToString();
                    vm.CurrencyRateFromBDT = Convert.ToDecimal(dr["CurrencyRateFromBDT"].ToString());
                    vm.CompInvoiceNo = dr["CompInvoiceNo"].ToString();
                    vm.LCBank = dr["LCBank"].ToString();
                    vm.LCDate = dr["LCDate"].ToString();
                    vm.VehicleID = dr["VehicleID"].ToString();
                    vm.VehicleType = dr["VehicleType"].ToString();
                    vm.VehicleNo = dr["VehicleNo"].ToString();
                    vm.CustomerGroup = dr["CustomerGroupName"].ToString();
                    vm.CurrencyCode = dr["CurrencyCode"].ToString();
                    vm.ValueOnly = dr["ValueOnly"].ToString();
                    ////reading newly added fields
                    vm.ReturnId = dr["SaleReturnId"].ToString();
                    vm.IsVDS = dr["IsVDS"].ToString();
                    vm.GetVDSCertificate = dr["GetVDSCertificate"].ToString();
                    vm.VDSCertificateDate = dr["VDSCertificateDate"].ToString();
                    vm.ImportIDExcel = dr["ImportIDExcel"].ToString();
                    vm.AlReadyPrint = dr["AlReadyPrint"].ToString();
                    vm.IsGatePass = dr["IsGatePass"].ToString();
                    vm.PINo = dr["PINo"].ToString();
                    vm.PIDate = dr["PIDate"].ToString();
                    vm.EXPFormNo = dr["EXPFormNo"].ToString();
                    vm.EXPFormDate = dr["EXPFormDate"].ToString();
                    vm.IsDeemedExport = dr["IsDeemedExport"].ToString();
                    vm.BranchId = Convert.ToInt32(dr["BranchId"].ToString());
                    vm.DeductionAmount = Convert.ToDecimal(dr["DeductionAmount"].ToString());
                    vm.SignatoryName = dr["SignatoryName"].ToString();
                    vm.SignatoryDesig = dr["SignatoryDesig"].ToString();
                    vm.HPSTotalAmount = Convert.ToDecimal(dr["HPSTotalAmount"].ToString());


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


        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null
            , SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SaleMasterVM likeVM = null, bool Dt = false, string transactionType = null, string MultipleSearch = "N", SysDBInfoVMTemp connVM = null)
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

sih.SalesInvoiceNo 
,cg.CustomerGroupName
,c.CustomerName
,sih.InvoiceDateTime InvoiceDateTime
,sih.DeliveryDate
,sih.DeliveryAddress1
,isnull(sih.TotalAmount,0) TotalAmount 
,isnull(sih.TotalVATAmount,0) TotalVATAmount 
,isnull(sih.TotalAmount,0)-isnull(sih.TotalVATAmount,0) TotalSubtotal 
,sih.SaleType
,sih.TransactionType
,isnull(sih.AlReadyPrint,0) AlReadyPrint
,sih.DeliveryChallanNo
,cr.CurrencyCode
,sih.DeliveryAddress2
,sih.DeliveryAddress3
,isnull(sih.HPSTotalAmount,0) HPSTotalAmount 
,sih.ValueOnly
,sih.SerialNo
,sih.Comments
,sih.CreatedBy
,sih.CreatedOn
,sih.LastModifiedBy
,sih.LastModifiedOn
,sih.PreviousSalesInvoiceNo
,isnull(sih.Trading,0) Trading 
,sih.IsPrint
,sih.Post
,sih.LCNumber
,isnull(sih.CurrencyRateFromBDT,0) CurrencyRateFromBDT 
,sih.IsVDS
,sih.GetVDSCertificate
,sih.VDSCertificateDate
,sih.IsGatePass
,sih.CompInvoiceNo
,sih.LCBank
,sih.Is6_3TollCompleted
,isnull(NULLIF(sih.LCDate,''),'1900/01/01')LCDate
,isnull(sih.VDSAmount,0) VDSAmount 
,v.VehicleType
,v.VehicleNo
 ,isnull(sih.DeliveryChallanNo,'N')DeliveryChallan
,isnull(sih.PINo,'N/A')PINo
,isnull(sih.PIDate,'1900/01/01')PIDate
,isnull(sih.EXPFormNo ,'N/A')EXPFormNo
,sih.EXPFormDate
,isnull(sih.IsDeemedExport ,'N')IsDeemedExport
,isnull(sih.SignatoryName ,'N/A')SignatoryName
,isnull(sih.SignatoryDesig ,'N/A')SignatoryDesig
,isnull(sih.DeductionAmount,0) DeductionAmount

,sih.ImportIDExcel
,SaleType,isnull(sih.PreviousSalesInvoiceNo,SalesInvoiceNo) PID
,sih.VehicleID
,isnull(sih.ShiftId,1) ShiftId
,sih.TenderId
,sih.CurrencyID
,isnull(sih.BranchId,0) BranchId
,sih.CustomerID
,sih.SaleReturnId
,isnull(sih.ImportIDExcel,'')ImportID
,sih.Id
,sih.FileName
,sih.FiscalYear
,sih.IsCurrencyConvCompleted

FROM SalesInvoiceHeaders sih 

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleID = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1
";
                #endregion SqlText

                sqlTextCount += @" select count(sih.SalesInvoiceNo)RecordCount

FROM SalesInvoiceHeaders sih 

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleID = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1";

                if (Id > 0)
                {
                    sqlTextParameter += @" and sih.Id=@Id";
                }
                if (MultipleSearch == "Y")
                {
                    if (BureauInfoVM.IsBureau == true)
                    {
                        sqlTextParameter += @" and sih.IsPrint='N'";

                    }
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
                //if (likeVM != null)
                //{
                //    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                //    {
                //        sqlText += " AND sih.SalesInvoiceNo like @SalesInvoiceNo";
                //    }
                //    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                //    {
                //        sqlText += " AND sih.SerialNo like @SerialNo";
                //    }
                //}
                #endregion SqlText

                sqlTextOrderBy += " order by sih.InvoiceDateTime desc, sih.SalesInvoiceNo desc";

                #region SqlExecution
                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;
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

        public List<SaleDetailVm> SelectSaleDetail(string saleInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleDetailVm> VMs = new List<SaleDetailVm>();
            SaleDetailVm vm;
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
sd.Id
,ISNULL(bom.ReferenceNo,'NA') BOMReferenceNo
,ISNULL(sd.BOMId,0)BOMId
,sd.SalesInvoiceNo
,sd.InvoiceLineNo
,sd.ItemNo
,ISNULL(sd.Quantity,0) Quantity
,ISNULL(sd.SalesPrice,0) SalesPrice  
,ISNULL(sd.NBRPrice,0) NBRPrice  
,ISNULL(sd.AVGPrice,0) AVGPrice  
,sd.UOM
,sd.VATRate
,sd.VATAmount
,ISNULL(sd.SubTotal,0) SubTotal  
,sd.Comments
,sd.CreatedBy
,sd.CreatedOn
,sd.LastModifiedBy
,sd.LastModifiedOn
,ISNULL(sd.SD,0) SD  
,ISNULL(sd.SDAmount,0) SDAmount  
,ISNULL(sd.VDSAmount,0) VDSAmount  
,sd.SaleType
,sd.PreviousSalesInvoiceNo
,sd.Trading
,sd.InvoiceDateTime
,sd.NonStock
,sd.TradingMarkUp
,sd.Type
,sd.BENumber
,sd.Post
,sd.UOMQty
,ISNULL(sd.UOMPrice,0) UOMPrice  
,ISNULL(sd.UOMc,0) UOMc  
,sd.UOMn
,ISNULL(sd.DollerValue,0) DollerValue  
,ISNULL(sd.CurrencyValue,0) CurrencyValue  
,sd.TransactionType
,sd.VATName
,sd.SaleReturnId
,ISNULL(sd.DiscountAmount,0) DiscountAmount  
,ISNULL(sd.DiscountedNBRPrice,0) DiscountedNBRPrice  
,ISNULL(sd.PromotionalQuantity,0) PromotionalQuantity  
,sd.FinishItemNo
,ISNULL(sd.CConversionDate,'19900101') CConversionDate
,sd.ReturnTransactionType
,ISNULL(sd.Weight,0) Weight  
,sd.ValueOnly
,p.ProductName
,p.ProductCode
,sd.TotalValue
,sd.WareHouseRent
,sd.WareHouseVAT
,ISNULL(sd.ATVRate,0) ATVRate
,ISNULL(sd.ATVablePrice,0) ATVablePrice
,ISNULL(sd.ATVAmount,0) ATVAmount
,ISNULL(sd.TradeVATRate,0) TradeVATRate
,ISNULL(sd.TradeVATAmount,0) TradeVATAmount
,ISNULL(sd.TradeVATableValue,0) TradeVATableValue
,ISNULL(sd.IsFixedVAT,'N') IsFixedVAT
,ISNULL(sd.FixedVATAmount,0) FixedVATAmount
,ISNULL(sd.ProductDescription,'N/A') ProductDescription
,sd.IsCommercialImporter
,sd.IsSample
,isnull(sd.HPSAmount,0)HPSAmount
,isnull(sd.HPSRate,0)HPSRate
,ISNULL(sd.CDNVATAmount,0) CDNVATAmount
,ISNULL(sd.CDNSDAmount,0) CDNSDAmount
,ISNULL(sd.CDNSubtotal,0) CDNSubtotal

FROM SalesInvoiceDetails sd
LEFT OUTER JOIN Products p ON p.ItemNo = sd.ItemNo
LEFT OUTER JOIN BOMs bom ON ISNULL(sd.BOMId,0) = bom.BOMId
WHERE  1=1

";

                if (saleInvoiceNo != null)
                {
                    sqlText += "AND sd.SalesInvoiceNo=@SalesInvoiceNo";
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

                if (saleInvoiceNo != null)
                {
                    objComm.Parameters.AddWithValue("@SalesInvoiceNo", saleInvoiceNo);
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
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SaleDetailVm();
                    vm.Id = Convert.ToString(dr["Id"]);
                    vm.BOMId = Convert.ToInt32(dr["BOMId"]);
                    vm.BOMReferenceNo = Convert.ToString(dr["BOMReferenceNo"]);

                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.InvoiceLineNo = dr["InvoiceLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.VDSAmountD = Convert.ToDecimal(dr["VDSAmount"].ToString());

                    vm.TradingD = dr["Trading"].ToString();
                    vm.NonStockD = dr["NonStock"].ToString();
                    vm.SaleTypeD = dr["SaleType"].ToString();
                    vm.CommentsD = dr["Comments"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.Type = dr["Type"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.DollerValue = Convert.ToDecimal(dr["DollerValue"].ToString());
                    vm.CurrencyValue = Convert.ToDecimal(dr["CurrencyValue"].ToString());
                    vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                    vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                    vm.PromotionalQuantity = Convert.ToDecimal(dr["PromotionalQuantity"].ToString());
                    vm.CConversionDate = OrdinaryVATDesktop.DateTimeToDate(dr["CConversionDate"].ToString());
                    vm.ReturnTransactionType = dr["ReturnTransactionType"].ToString();
                    vm.Weight = dr["Weight"].ToString();
                    vm.ValueOnly = dr["ValueOnly"].ToString() == "" ? "N" : dr["ValueOnly"].ToString();
                    vm.VatName = dr["VATName"].ToString();
                    vm.Total = vm.SubTotal + vm.VATAmount;
                    //vm.BDTValue = vm.Total;
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    //new fields
                    vm.TotalValue = Convert.ToDecimal(dr["TotalValue"].ToString() == "" ? "0" : dr["TotalValue"].ToString());
                    vm.WareHouseRent = Convert.ToDecimal(dr["WareHouseRent"].ToString() == "" ? "0" : dr["WareHouseRent"].ToString());
                    vm.WareHouseVAT = Convert.ToDecimal(dr["WareHouseVAT"].ToString() == "" ? "0" : dr["WareHouseVAT"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString() == "" ? "0" : dr["ATVRate"].ToString());
                    vm.ATVablePrice = Convert.ToDecimal(dr["ATVablePrice"].ToString() == "" ? "0" : dr["ATVablePrice"].ToString());
                    vm.ATVAmount = Convert.ToDecimal(dr["ATVAmount"].ToString() == "" ? "0" : dr["ATVAmount"].ToString());
                    vm.IsCommercialImporter = dr["IsCommercialImporter"].ToString() == "" ? "N" : dr["IsCommercialImporter"].ToString();

                    //newly added fields
                    vm.AvgRate = Convert.ToDecimal(dr["AVGPrice"].ToString());
                    vm.PreviousSalesInvoiceNoD = dr["PreviousSalesInvoiceNo"].ToString();
                    vm.InvoiceDateTime = dr["InvoiceDateTime"].ToString();
                    vm.BENumber = dr["BENumber"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.ReturnId = dr["SaleReturnId"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.TradeVATRate = Convert.ToDecimal(dr["TradeVATRate"].ToString());
                    vm.TradeVATAmount = Convert.ToDecimal(dr["TradeVATAmount"].ToString());
                    vm.TradeVATableValue = Convert.ToDecimal(dr["TradeVATableValue"].ToString());
                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.IsSample = dr["IsSample"].ToString();
                    vm.HPSAmount = Convert.ToDecimal(dr["HPSAmount"].ToString());
                    vm.HPSRate = Convert.ToDecimal(dr["HPSRate"].ToString());

                    vm.CDNVATAmount = Convert.ToDecimal(dr["CDNVATAmount"].ToString());
                    vm.CDNSDAmount = Convert.ToDecimal(dr["CDNSDAmount"].ToString());
                    vm.CDNSubtotal = Convert.ToDecimal(dr["CDNSubtotal"].ToString());



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



        public List<SaleDetailVm> SearchSaleDetailDTNewList(string SalesInvoiceNo, string InvoiceDate, bool SearchPreviousForCNDN = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SaleDetailVm> VMs = new List<SaleDetailVm>();
            SaleDetailVm vm;
            #endregion
            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SearchSaleDetailDTNew(SalesInvoiceNo);

                foreach (DataRow dr in dt.Rows)
                {

                    vm = new SaleDetailVm();

                    vm.SourcePaidQuantity = Convert.ToDecimal(dr["SourcePaidQuantity"].ToString());
                    vm.SourcePaidVATAmount = Convert.ToDecimal(dr["SourcePaidVATAmount"].ToString());
                    vm.NBRPriceInclusiveVAT = Convert.ToDecimal(dr["NBRPriceInclusiveVAT"].ToString());

                    vm.BOMId = Convert.ToInt32(dr["BOMId"]);
                    vm.BOMReferenceNo = Convert.ToString(dr["BOMReferenceNo"]);
                    vm.SalesInvoiceNo = dr["SalesInvoiceNo"].ToString();
                    vm.InvoiceLineNo = dr["InvoiceLineNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.SalesPrice = Convert.ToDecimal(dr["SalesPrice"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.VDSAmountD = Convert.ToDecimal(dr["VDSAmount"].ToString());

                    vm.TradingD = dr["Trading"].ToString();
                    vm.NonStockD = dr["NonStock"].ToString();
                    //vm.SaleTypeD = dr["SaleType"].ToString();
                    vm.CommentsD = dr["Comments"].ToString();
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.Type = dr["Type"].ToString();
                    //vm.Post = dr["Post"].ToString();
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.DollerValue = Convert.ToDecimal(dr["DollerValue"].ToString());
                    vm.CurrencyValue = Convert.ToDecimal(dr["CurrencyValue"].ToString());
                    vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                    vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                    vm.PromotionalQuantity = Convert.ToDecimal(dr["PromotionalQuantity"].ToString());
                    vm.CConversionDate = OrdinaryVATDesktop.DateTimeToDate(dr["CConversionDate"].ToString());
                    vm.ReturnTransactionType = dr["ReturnTransactionType"].ToString();
                    vm.Weight = dr["Weight"].ToString();
                    vm.ValueOnly = dr["ValueOnly"].ToString() == "" ? "N" : dr["ValueOnly"].ToString();
                    vm.VatName = dr["VATName"].ToString();
                    vm.Total = vm.SubTotal + vm.VATAmount;
                    ////////vm.BDTValue = vm.Total;
                    vm.ProductName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = dr["CreatedOn"].ToString();
                    //vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    //////new fields
                    vm.TotalValue = Convert.ToDecimal(dr["TotalValue"].ToString() == "" ? "0" : dr["TotalValue"].ToString());
                    vm.WareHouseRent = Convert.ToDecimal(dr["WareHouseRent"].ToString() == "" ? "0" : dr["WareHouseRent"].ToString());
                    vm.WareHouseVAT = Convert.ToDecimal(dr["WareHouseVAT"].ToString() == "" ? "0" : dr["WareHouseVAT"].ToString());
                    vm.ATVRate = Convert.ToDecimal(dr["ATVRate"].ToString() == "" ? "0" : dr["ATVRate"].ToString());
                    vm.ATVablePrice = Convert.ToDecimal(dr["ATVablePrice"].ToString() == "" ? "0" : dr["ATVablePrice"].ToString());
                    vm.ATVAmount = Convert.ToDecimal(dr["ATVAmount"].ToString() == "" ? "0" : dr["ATVAmount"].ToString());
                    vm.IsCommercialImporter = dr["IsCommercialImporter"].ToString() == "" ? "N" : dr["IsCommercialImporter"].ToString();

                    //////newly added fields

                    vm.TradeVATRate = Convert.ToDecimal(dr["TradeVATRate"].ToString());
                    vm.TradeVATAmount = Convert.ToDecimal(dr["TradeVATAmount"].ToString());
                    vm.TradeVATableValue = Convert.ToDecimal(dr["TradeVATableValue"].ToString());
                    vm.IsFixedVAT = dr["IsFixedVAT"].ToString();
                    vm.FixedVATAmount = Convert.ToDecimal(dr["FixedVATAmount"].ToString());
                    vm.ProductDescription = dr["ProductDescription"].ToString();
                    vm.HPSAmount = Convert.ToDecimal(dr["HPSAmount"].ToString());
                    vm.HPSRate = Convert.ToDecimal(dr["HPSRate"].ToString());

                    vm.CDNVATAmount = Convert.ToDecimal(dr["CDNVATAmount"].ToString());
                    vm.CDNSDAmount = Convert.ToDecimal(dr["CDNSDAmount"].ToString());
                    vm.CDNSubtotal = Convert.ToDecimal(dr["CDNSubtotal"].ToString());

                    vm.PreviousSalesInvoiceNoD = SearchPreviousForCNDN == true ? dr["SalesInvoiceNo"].ToString() : dr["PreviousSalesInvoiceNo"].ToString();
                    vm.PreviousInvoiceDateTime = SearchPreviousForCNDN == true ? InvoiceDate : dr["PreviousInvoiceDateTime"].ToString();
                    vm.PreviousNBRPrice = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["NBRPrice"].ToString()) : Convert.ToDecimal(dr["PreviousNBRPrice"].ToString());
                    vm.PreviousQuantity = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["Quantity"].ToString()) : Convert.ToDecimal(dr["PreviousQuantity"].ToString());
                    vm.PreviousUOM = SearchPreviousForCNDN == true ? dr["UOM"].ToString() : dr["PreviousUOM"].ToString();
                    vm.PreviousSubTotal = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["SubTotal"].ToString()) : Convert.ToDecimal(dr["PreviousSubTotal"].ToString());
                    vm.PreviousVATRate = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["VATRate"].ToString()) : Convert.ToDecimal(dr["PreviousVATRate"].ToString());
                    vm.PreviousVATAmount = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["VATAmount"].ToString()) : Convert.ToDecimal(dr["PreviousVATAmount"].ToString());
                    vm.PreviousSD = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["SD"].ToString()) : Convert.ToDecimal(dr["PreviousSD"].ToString());
                    vm.PreviousSDAmount = SearchPreviousForCNDN == true ? Convert.ToDecimal(dr["SDAmount"].ToString()) : Convert.ToDecimal(dr["PreviousSDAmount"].ToString());
                    vm.ReasonOfReturn = dr["ReasonOfReturn"].ToString();

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






        #region Excel Import Web Methods

        public string[] ImportExcelIntegrationFile(SaleMasterVM paramVM, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
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

                DataTable dtSetting = new DataTable();
                CommonDAL commonDal = new CommonDAL();

                if (settingVM.SettingsDTUser == null)
                {
                    dtSetting = commonDal.SettingDataAll(null, null);
                }

                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();

                //dt = ds.Tables[0];
                reader.Close();
                stream.Close();

                File.Delete(Fullpath);
                #endregion

                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();

                dtSaleM = ds.Tables["SaleM"];

                DataTable dtSalesData = dtSaleM.Copy();


                #region Column Mapping

                dtSaleM.Columns["Branch_Code"].ColumnName = "BranchCode";
                dtSaleM.Columns["Customer_Code"].ColumnName = "CustomerCode";
                dtSaleM.Columns["Customer_Name"].ColumnName = "CustomerName";
                dtSaleM.Columns["Reference_No"].ColumnName = "ReferenceNo";
                dtSaleM.Columns["Item_Code"].ColumnName = "ProductCode";
                dtSaleM.Columns["Item_Name"].ColumnName = "ProductName";
                dtSaleM.Columns["NBR_Price"].ColumnName = "UnitPrice";
                dtSaleM.Columns["VAT_Rate"].ColumnName = "VATRate";
                dtSaleM.Columns["SD_Rate"].ColumnName = "SDRate";
                dtSaleM.Columns["Discount_Amount"].ColumnName = "DiscountAmount";
                dtSaleM.Columns["Promotional_Quantity"].ColumnName = "PromotionalQuantity";
                dtSaleM.Columns["LC_Number"].ColumnName = "LCNumber";
                dtSaleM.Columns["Currency_Code"].ColumnName = "CurrencyCode";
                dtSaleM.Columns["CommentsD"].ColumnName = "LineComments";
                dtSaleM.Columns["Previous_Invoice_No"].ColumnName = "PreviousInvoiceNo";
                dtSaleM.Columns["Delivery_Address"].ColumnName = "DeliveryAddress";
                dtSaleM.Columns["Delivery_Date_Time"].ColumnName = "DeliveryDateTime";
                dtSaleM.Columns["SubTotal"].ColumnName = "Subtotal";


                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Weight");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "CustomerGroup");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Vehicle_No");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "VehicleType");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "CustomerGroup");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Sale_Type");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Is_Print");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Tender_Id");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "SDAmount");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "VAT_Amount");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Non_Stock");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "Trading_MarkUp");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "VAT_Name");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ProductDescription");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpDescription");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpQuantity");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpGrossWeight");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpNetWeight");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpNumberFrom");
                OrdinaryVATDesktop.DtDeleteColumn(dtSaleM, "ExpNumberTo");

                dtSaleM.Columns.Add(new DataColumn() { ColumnName = "CompanyCode", DefaultValue = "Excel" });
                dtSaleM.Columns.Add(new DataColumn() { ColumnName = "IsProcessed", DefaultValue = "N" });


                #region Merge Date Time

                if (dtSaleM.Columns.Contains("Invoice_Date") && dtSaleM.Columns.Contains("Invoice_Time"))
                {
                    if (!dtSaleM.Columns.Contains("InvoiceDateTime"))
                    {
                        dtSaleM.Columns.Add("InvoiceDateTime");
                    }

                    foreach (DataRow dtSaleMRow in dtSaleM.Rows)
                    {
                        dtSaleMRow["InvoiceDateTime"] = OrdinaryVATDesktop.DateToDate_YMD(dtSaleMRow["Invoice_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dtSaleMRow["Invoice_Time"].ToString());
                    }

                    dtSaleM.Columns.Remove("Invoice_Date");
                    dtSaleM.Columns.Remove("Invoice_Time");
                }
                else
                {
                    OrdinaryVATDesktop.DataTable_DateFormat(dtSaleM, "Invoice_Date_Time");
                    dtSaleM.Columns["Invoice_Date_Time"].ColumnName = "InvoiceDateTime";

                }

                #endregion


                #endregion
                IntegrationParam param = new IntegrationParam() { TransactionType = paramVM.TransactionType, BranchCode = paramVM.BranchCode, DefaultBranchCode = paramVM.BranchCode };

                TableValidation(dtSaleM, param, false);


                #region Delete Processed Data

                string deleteText = @"delete from VAT_Source_Sales ";

                SqlCommand cmd = new SqlCommand(deleteText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion

                retResults = commonDal.BulkInsert("VAT_Source_Sales", dtSaleM, currConn, transaction);


                #region Group by Customer BCL
                string code = commonDal.settingValue("CompanyCode", "Code");
                if (code == "BCL")
                {
                    string updateText = @"--select distinct Customercode from VAT_Source_Sales

create table #Refs
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	U_ID varchar(600),
	RefNo varchar(600),
)

create table #FinalConcat
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	U_ID varchar(6000),
	RefNo varchar(6000),
)
create table #InvoiceDate
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	InvoiceDate varchar(600),

)

Insert Into #Refs (Customer_code,U_ID, RefNo)
select distinct Customercode, ID,ReferenceNo from VAT_Source_Sales

Insert Into #FinalConcat (Customer_code,RefNo,U_ID)
SELECT DISTINCT temp2.Customer_code, 
    SUBSTRING(
        (
            SELECT '~'+temp1.RefNo  AS [text()]
            FROM #Refs temp1
            WHERE temp1.Customer_code = temp2.Customer_code 
            ORDER BY temp1.Customer_code
            FOR XML PATH ('')
        ), 2, 6000) [Refs],    
		SUBSTRING(
        (
            SELECT '~'+temp1.U_ID  AS [text()]
            FROM #Refs temp1
            WHERE temp1.Customer_code = temp2.Customer_code 
            ORDER BY temp1.Customer_code
            FOR XML PATH ('')
        ), 2, 6000) IDs
FROM #Refs temp2


insert into #InvoiceDate(Customer_code, InvoiceDate)
select  CustomerCode, max(InvoiceDateTime) from VAT_Source_Sales
group by CustomerCode


--select * from #InvoiceDate



update VAT_Source_Sales set ID = #FinalConcat.U_ID, ReferenceNo=#FinalConcat.RefNo from #FinalConcat 
where #FinalConcat.Customer_code  = VAT_Source_Sales.CustomerCode

update VAT_Source_Sales set InvoiceDateTime = #InvoiceDate.InvoiceDate from #InvoiceDate 
where #InvoiceDate.Customer_code  = VAT_Source_Sales.CustomerCode

drop table #Refs
drop table #FinalConcat
drop table #InvoiceDate";


                    cmd.CommandText = updateText;

                    cmd.ExecuteNonQuery();
                }

                #endregion

                retResults[0] = "Success";//Success or Fail
                retResults[1] = "Success";// Success or Fail Message

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
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString();
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



        public string[] ImportExcelIntegrationDataTable(DataTable saleData, SaleMasterVM paramVM, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null,
            SqlTransaction Vtransaction = null)
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
            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
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

                #region Settings

                DataTable dtSetting = new DataTable();
                CommonDAL commonDal = new CommonDAL();

                if (settingVM.SettingsDTUser == null)
                {
                    dtSetting = commonDal.SettingDataAll(null, null);
                }
                #endregion

                #region More Decalartions

                DataSet ds = new DataSet();
                DataTable dtSaleM = new DataTable();

                #endregion


                dtSaleM = saleData;

                DataTable dtSalesData = dtSaleM.Copy();

                #region Table Validation

                TableValidation_VAT_Source_Sales(dtSaleM, paramVM);

                IntegrationParam param = new IntegrationParam() { TransactionType = paramVM.TransactionType, BranchCode = paramVM.BranchCode, DefaultBranchCode = paramVM.BranchCode };
                TableValidation(dtSaleM, param, false);

                #endregion

                #region Delete Processed Data

                string deleteText = @"delete from VAT_Source_Sales ";

                SqlCommand cmd = new SqlCommand(deleteText, currConn, transaction);
                transResult = cmd.ExecuteNonQuery();

                #endregion

                #region Debug

                //if (true)
                //{

                //}
                string names = "";

                foreach (DataColumn column in dtSaleM.Columns)
                {
                    names += column.ColumnName + ",";
                }
                #endregion

                #region Bulk Insert to VAT_Source_Sales

                retResults = commonDal.BulkInsert("VAT_Source_Sales", dtSaleM, currConn, transaction);

                #endregion

                #region Customization
                string code = commonDal.settingValue("CompanyCode", "Code", null, currConn, transaction);

                #region BCL - Beximco Communications Ltd.

                if (code == "BCL")
                {
                    string updateText = @"
------select distinct Customercode from VAT_Source_Sales

create table #Refs
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	U_ID varchar(600),
	RefNo varchar(600),
)

create table #FinalConcat
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	U_ID varchar(6000),
	RefNo varchar(6000),
)
create table #InvoiceDate
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	InvoiceDate varchar(600),

)

Insert Into #Refs (Customer_code,U_ID, RefNo)
select distinct Customercode, ID,ReferenceNo from VAT_Source_Sales

Insert Into #FinalConcat (Customer_code,RefNo,U_ID)
SELECT DISTINCT temp2.Customer_code, 
    SUBSTRING(
        (
            SELECT '~'+temp1.RefNo  AS [text()]
            FROM #Refs temp1
            WHERE temp1.Customer_code = temp2.Customer_code 
            ORDER BY temp1.Customer_code
            FOR XML PATH ('')
        ), 2, 6000) [Refs],    
		SUBSTRING(
        (
            SELECT '~'+temp1.U_ID  AS [text()]
            FROM #Refs temp1
            WHERE temp1.Customer_code = temp2.Customer_code 
            ORDER BY temp1.Customer_code
            FOR XML PATH ('')
        ), 2, 6000) IDs
FROM #Refs temp2


insert into #InvoiceDate(Customer_code, InvoiceDate)
select  CustomerCode, max(InvoiceDateTime) from VAT_Source_Sales
group by CustomerCode


------select * from #InvoiceDate



update VAT_Source_Sales set ID = #FinalConcat.U_ID, ReferenceNo=#FinalConcat.RefNo from #FinalConcat 
where #FinalConcat.Customer_code  = VAT_Source_Sales.CustomerCode

update VAT_Source_Sales set InvoiceDateTime = #InvoiceDate.InvoiceDate from #InvoiceDate 
where #InvoiceDate.Customer_code  = VAT_Source_Sales.CustomerCode

drop table #Refs
drop table #FinalConcat
drop table #InvoiceDate";


                    cmd.CommandText = updateText;

                    transResult = cmd.ExecuteNonQuery();
                }
                #endregion

                #endregion

                retResults[0] = "Success";//Success or Fail
                retResults[1] = "Success";// Success or Fail Message

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
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[4] = ex.Message.ToString();
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


        public string[] ImportExcelFile(SaleMasterVM paramVM, SysDBInfoVMTemp connVM = null)
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
                DataTable dtSetting = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    dtSetting = new CommonDAL().SettingDataAll(null, null);
                }

                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();

                //dt = ds.Tables[0];
                reader.Close();
                stream.Close();

                File.Delete(Fullpath);
                #endregion

                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();

                dtSaleM = ds.Tables["SaleM"];

                DataTable dtSalesData = dtSaleM.Copy();
                IntegrationParam param = new IntegrationParam() { TransactionType = paramVM.TransactionType, BranchCode = paramVM.BranchCode, DefaultBranchCode = paramVM.BranchCode };
                TableValidation(dtSalesData, param);

                if (paramVM.TransactionType.ToLower() == "other" || paramVM.TransactionType.ToLower() == "servicens")
                {
                    dtSalesData.Columns.Add(new DataColumn() { ColumnName = "token", DefaultValue = paramVM.Token });

                    retResults = SaveTempAndProcess(dtSalesData, () => { }, paramVM.BranchId, "", null, paramVM.Token);

                }
                else
                {
                    retResults = SaveAndProcess_WithOutBulk(dtSalesData, () => { }, paramVM.BranchId, null);
                }


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
                retResults[4] = ex.Message.ToString(); //catch ex
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


        public string[] SaveToTemp(DataTable dtSaleM, int BranchId, string transactionType, string token, SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null)
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
                DataTable dtSalesData = dtSaleM.Copy();

                TableValidation(dtSalesData, new IntegrationParam() { TransactionType = transactionType });


                if (transactionType.ToLower() == "other" || transactionType.ToLower() == "servicens")
                {
                    dtSalesData.Columns.Add(new DataColumn() { ColumnName = "token", DefaultValue = token });

                    retResults = SaveTempAndProcess(dtSalesData, () => { }, BranchId, "", null, token, paramVM);

                }
                else
                {
                    retResults = SaveAndProcess_WithOutBulk(dtSalesData, () => { }, BranchId, null);
                }


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
                retResults[4] = ex.Message.ToString(); //catch ex
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



        public string[] SaveSaleACI(DataTable dtSaleM, int BranchId, string transactionType, string token, SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null, string UserId = "")
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
                DataTable dtSalesData = dtSaleM.Copy();

                TableValidation(dtSalesData, new IntegrationParam() { TransactionType = transactionType, BranchCode = paramVM.BranchCode });


                //if (transactionType.ToLower() == "other" || transactionType.ToLower() == "servicens")
                //{


                //}
                //else
                //{
                //    retResults = SaveAndProcess_WithOutBulk(dtSalesData, () => { }, BranchId, null);
                //}

                dtSalesData.Columns.Add(new DataColumn() { ColumnName = "token", DefaultValue = token });

                retResults = SaveAndProcess(dtSalesData, () => { }, BranchId, String.Empty, null, paramVM,null,null,UserId);

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
                //retResults[4] = ex.Message.ToString(); //catch ex
                //return retResults;

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





        public string[] ImportExcelFileFull(SaleMasterVM paramVM, SysDBInfoVMTemp connVM = null, string UserId = "")
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
                DataTable dtSetting = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    dtSetting = new CommonDAL().SettingDataAll(null, null);
                }

                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();

                //dt = ds.Tables[0];
                reader.Close();
                stream.Close();

                File.Delete(Fullpath);
                #endregion

                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();

                dtSaleM = ds.Tables["SaleM"];

                DataTable dtSalesData = dtSaleM.Copy();
                IntegrationParam param = new IntegrationParam() { TransactionType = paramVM.TransactionType, BranchCode = paramVM.BranchCode, DefaultBranchCode = paramVM.BranchCode };
                TableValidation(dtSalesData, param);

                if (paramVM.TransactionType.ToLower() == "other" || paramVM.TransactionType.ToLower() == "servicens")
                {
                    dtSalesData.Columns.Add(new DataColumn() { ColumnName = "token", DefaultValue = paramVM.Token });

                    retResults = SaveAndProcess(dtSalesData, () => { }, paramVM.BranchId,"",null,null,null,null,UserId);

                }
                else
                {
                    retResults = SaveAndProcess_WithOutBulk(dtSalesData, () => { }, paramVM.BranchId, null);
                }


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
                retResults[4] = ex.Message.ToString(); //catch ex
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


        private void TableValidation_VAT_Source_Sales(DataTable dtSaleM, SaleMasterVM saleMasterVM, SysDBInfoVMTemp connVM = null)
        {
            #region Rename Columns

            if (dtSaleM.Columns.Contains("Branch_Code")) { dtSaleM.Columns["Branch_Code"].ColumnName = "BranchCode"; }
            if (dtSaleM.Columns.Contains("Customer_Code")) { dtSaleM.Columns["Customer_Code"].ColumnName = "CustomerCode"; }
            if (dtSaleM.Columns.Contains("Customer_Name")) { dtSaleM.Columns["Customer_Name"].ColumnName = "CustomerName"; }
            if (dtSaleM.Columns.Contains("Reference_No")) { dtSaleM.Columns["Reference_No"].ColumnName = "ReferenceNo"; }
            if (dtSaleM.Columns.Contains("Item_Code")) { dtSaleM.Columns["Item_Code"].ColumnName = "ProductCode"; }
            if (dtSaleM.Columns.Contains("Item_Name")) { dtSaleM.Columns["Item_Name"].ColumnName = "ProductName"; }
            if (dtSaleM.Columns.Contains("NBR_Price")) { dtSaleM.Columns["NBR_Price"].ColumnName = "UnitPrice"; }
            if (dtSaleM.Columns.Contains("VAT_Rate")) { dtSaleM.Columns["VAT_Rate"].ColumnName = "VATRate"; }
            if (dtSaleM.Columns.Contains("SD_Rate")) { dtSaleM.Columns["SD_Rate"].ColumnName = "SDRate"; }
            if (dtSaleM.Columns.Contains("Discount_Amount")) { dtSaleM.Columns["Discount_Amount"].ColumnName = "DiscountAmount"; }
            if (dtSaleM.Columns.Contains("Promotional_Quantity")) { dtSaleM.Columns["Promotional_Quantity"].ColumnName = "PromotionalQuantity"; }
            if (dtSaleM.Columns.Contains("LC_Number")) { dtSaleM.Columns["LC_Number"].ColumnName = "LCNumber"; }
            if (dtSaleM.Columns.Contains("Currency_Code")) { dtSaleM.Columns["Currency_Code"].ColumnName = "CurrencyCode"; }
            if (dtSaleM.Columns.Contains("CommentsD")) { dtSaleM.Columns["CommentsD"].ColumnName = "LineComments"; }
            if (dtSaleM.Columns.Contains("Previous_Invoice_No")) { dtSaleM.Columns["Previous_Invoice_No"].ColumnName = "PreviousInvoiceNo"; }
            if (dtSaleM.Columns.Contains("Delivery_Address")) { dtSaleM.Columns["Delivery_Address"].ColumnName = "DeliveryAddress"; }
            if (dtSaleM.Columns.Contains("SubTotal")) { dtSaleM.Columns["SubTotal"].ColumnName = "Subtotal"; }
            if (dtSaleM.Columns.Contains("Delivery_Date_Time")) { dtSaleM.Columns["Delivery_Date_Time"].ColumnName = "DeliveryDateTime"; }

            #endregion

            #region Remove Columns

            List<string> deleteColumnList = new List<string>();

            deleteColumnList.Add("Weight");
            deleteColumnList.Add("CustomerGroup");
            deleteColumnList.Add("Vehicle_No");
            deleteColumnList.Add("VehicleType");
            deleteColumnList.Add("CustomerGroup");
            deleteColumnList.Add("Sale_Type");
            deleteColumnList.Add("Is_Print");
            deleteColumnList.Add("Tender_Id");
            //deleteColumnList.Add("SDAmount");
            //deleteColumnList.Add("VAT_Amount");
            deleteColumnList.Add("Non_Stock");
            deleteColumnList.Add("Trading_MarkUp");
            deleteColumnList.Add("VAT_Name");
            deleteColumnList.Add("ProductDescription");
            deleteColumnList.Add("ExpDescription");
            deleteColumnList.Add("ExpQuantity");
            deleteColumnList.Add("ExpGrossWeight");
            deleteColumnList.Add("ExpNetWeight");
            deleteColumnList.Add("ExpNumberFrom");
            deleteColumnList.Add("ExpNumberTo");

            OrdinaryVATDesktop.DtDeleteColumns(dtSaleM, deleteColumnList);

            #endregion

            #region Merge Date Time

            if (dtSaleM.Columns.Contains("Invoice_Date") && dtSaleM.Columns.Contains("Invoice_Time"))
            {
                if (!dtSaleM.Columns.Contains("InvoiceDateTime"))
                {
                    dtSaleM.Columns.Add("InvoiceDateTime");
                }

                foreach (DataRow dtSaleMRow in dtSaleM.Rows)
                {
                    dtSaleMRow["InvoiceDateTime"] = OrdinaryVATDesktop.DateToDate_YMD(dtSaleMRow["Invoice_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dtSaleMRow["Invoice_Time"].ToString());
                }

                dtSaleM.Columns.Remove("Invoice_Date");
                dtSaleM.Columns.Remove("Invoice_Time");
            }
            else
            {
                OrdinaryVATDesktop.DataTable_DateFormat(dtSaleM, "Invoice_Date_Time");
                dtSaleM.Columns["Invoice_Date_Time"].ColumnName = "InvoiceDateTime";

            }

            #endregion

            #region Add Coulmns

            if (!dtSaleM.Columns.Contains("CompanyCode"))
            {
                dtSaleM.Columns.Add(new DataColumn() { ColumnName = "CompanyCode", DefaultValue = "Excel" });
            }

            if (!dtSaleM.Columns.Contains("IsProcessed"))
            {
                dtSaleM.Columns.Add(new DataColumn() { ColumnName = "IsProcessed", DefaultValue = "N" });
            }

            #endregion

        }


        public void TableValidation(DataTable dtSaleM, IntegrationParam saleMasterVM, bool audit = true, SysDBInfoVMTemp connVM = null)
        {

            #region Add Columns


            if (!dtSaleM.Columns.Contains("Branch_Code") && !string.IsNullOrEmpty(saleMasterVM.BranchCode) && !dtSaleM.Columns.Contains("BranchCode"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = saleMasterVM.BranchCode };
                dtSaleM.Columns.Add(columnName);
            }

            if (!dtSaleM.Columns.Contains("TransactionType"))
            {
                var columnName = new DataColumn("TransactionType") { DefaultValue = saleMasterVM.TransactionType };
                dtSaleM.Columns.Add(columnName);
            }

            #region Audit Columns

            if (audit)
            {
                if (!dtSaleM.Columns.Contains("SL"))
                {
                    var columnName = new DataColumn("SL") { DefaultValue = "" };
                    dtSaleM.Columns.Add(columnName);
                }

                if (!dtSaleM.Columns.Contains("CreatedBy"))
                {
                    var columnName = new DataColumn("CreatedBy") { DefaultValue = "" };
                    dtSaleM.Columns.Add(columnName);
                }

                if (!dtSaleM.Columns.Contains("ReturnId"))
                {
                    var columnName = new DataColumn("ReturnId") { DefaultValue = 0 };
                    dtSaleM.Columns.Add(columnName);
                }

                if (!dtSaleM.Columns.Contains("BOMId"))
                {
                    var columnName = new DataColumn("BOMId") { DefaultValue = 0 };
                    dtSaleM.Columns.Add(columnName);
                }



                if (!dtSaleM.Columns.Contains("CreatedOn"))
                {
                    var columnName = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };
                    dtSaleM.Columns.Add(columnName);
                }

            }
            #endregion


            #endregion

        }

        public string[] ImportExcelFile_Backup(SaleMasterVM paramVM, SysDBInfoVMTemp connVM = null)
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
                DataTable dtSetting = new DataTable();

                if (settingVM.SettingsDTUser == null)
                {
                    dtSetting = new CommonDAL().SettingDataAll(null, null);
                }

                DataSet ds = new DataSet();
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                File.Delete(Fullpath);
                if (paramVM.File != null && paramVM.File.ContentLength > 0)
                {
                    paramVM.File.SaveAs(Fullpath);
                }


                FileStream stream = File.Open(Fullpath, FileMode.Open, FileAccess.Read);
                IExcelDataReader reader = null;
                if (FileName.EndsWith(".xls"))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (FileName.EndsWith(".xlsx"))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }
                reader.IsFirstRowAsColumnNames = true;
                ds = reader.AsDataSet();

                //dt = ds.Tables[0];
                reader.Close();
                File.Delete(Fullpath);
                #endregion

                DataTable dtSaleM = new DataTable();
                DataTable dtSaleD = new DataTable();

                dtSaleM = ds.Tables["SaleM"];
                string SingleSaleImport = "";
                SingleSaleImport = _cDAL.settingsDesktop("SingleFileImport", "SaleImport", dtSetting);

                if (SingleSaleImport.ToLower() == "y")
                {
                    DataView view = new DataView(dtSaleM);

                    dtSaleM = new DataTable();
                    dtSaleM = view.ToTable(true, "ID", "Customer_Name", "Customer_Code", "Delivery_Address", "Vehicle_No",
                        "Invoice_Date_Time", "Delivery_Date_Time", "Reference_No", "Comments", "Sale_Type", "Previous_Invoice_No", "Is_Print", "Tender_Id",
                        "Post", "LC_Number", "Currency_Code");

                    dtSaleD = new DataTable();
                    dtSaleD = view.ToTable(true, "ID", "Item_Code", "Item_Name", "Quantity", "NBR_Price", "UOM", "VAT_Rate", "VAT_Amount",
                        "SD_Rate", "Non_Stock", "Trading_MarkUp", "Type", "Discount_Amount", "Promotional_Quantity", "VAT_Name", "Total_Price", "CommentsD");
                }
                else
                {
                    dtSaleD = ds.Tables["SaleD"];
                }

                DataTable dtSaleE = new DataTable();
                dtSaleE = ds.Tables["SaleE"];


                dtSaleM.Columns.Add("Transection_Type");
                dtSaleM.Columns.Add("Created_By");
                dtSaleM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtSaleM.Rows)
                {
                    row["Transection_Type"] = paramVM.TransactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.LastModifiedBy;

                }
                dtSaleD.Columns.Add("TotalValue");
                dtSaleD.Columns.Add("WareHouseRent");
                dtSaleD.Columns.Add("WareHouseVAT");
                dtSaleD.Columns.Add("ATVRate");
                dtSaleD.Columns.Add("ATVablePrice");
                dtSaleD.Columns.Add("ATVAmount");
                dtSaleD.Columns.Add("IsCommercialImporter");

                for (int i = 0; i < dtSaleD.Rows.Count; i++)
                {
                    dtSaleD.Rows[i]["TotalValue"] = "0";
                    dtSaleD.Rows[i]["WareHouseRent"] = "0";
                    dtSaleD.Rows[i]["WareHouseVAT"] = "0";
                    dtSaleD.Rows[i]["ATVRate"] = "0";
                    dtSaleD.Rows[i]["ATVablePrice"] = "0";
                    dtSaleD.Rows[i]["ATVAmount"] = "0";
                    dtSaleD.Rows[i]["IsCommercialImporter"] = "N";

                    if (false)
                    {
                    }
                }

                #region Data Insert
                retResults = ImportData(dtSaleM, dtSaleD, dtSaleE, false, paramVM.BranchId);
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
                retResults[4] = ex.Message.ToString(); //catch ex
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

        #endregion

        #endregion

        #region plain methods

        public string[] SalesInsertToMaster(SaleMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, DataTable settingsDt = null)
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

                string firstDate = "01-July-2019";
                bool CN_DN_PreviousInvocie = commonDal.settingsCache("Sale", "CN_DN_PreviousInvocie", settingsDt) == "Y";

                if (Master.TransactionType == "Credit")
                {
                    if (CN_DN_PreviousInvocie)
                    {
                        if (Convert.ToDateTime(Master.DeliveryDate) < Convert.ToDateTime(firstDate))
                        {
                            retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }
                    }
                }
                else
                {

                    if (Convert.ToDateTime(Master.InvoiceDateTime) < Convert.ToDateTime(firstDate))
                    {
                        retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }
                }
                #endregion

                #region Insert

                sqlText = "";
                sqlText += " insert into SalesInvoiceHeaders";
                sqlText += " (";
                sqlText += " SalesInvoiceNo,";
                sqlText += " CustomerID,";
                sqlText += " DeliveryAddress1,";
                sqlText += " DeliveryAddress2,";
                sqlText += " DeliveryAddress3,";
                sqlText += " VehicleID,";
                sqlText += " InvoiceDateTime,";
                sqlText += " DeliveryDate,";

                sqlText += " TotalAmount,";
                sqlText += " TotalVATAmount,";
                sqlText += " VDSAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " SaleType,";
                sqlText += " PreviousSalesInvoiceNo,";
                sqlText += " Trading,";
                sqlText += " IsPrint,";
                sqlText += " TenderId,";
                sqlText += " TransactionType,";
                sqlText += " Post,";
                sqlText += " LCNumber,";
                sqlText += " CurrencyID,";
                sqlText += " CurrencyRateFromBDT,";
                sqlText += " SaleReturnId,";
                sqlText += " IsVDS,";
                sqlText += " GetVDSCertificate,";
                sqlText += " VDSCertificateDate,";
                sqlText += " ImportIDExcel,";
                sqlText += " AlReadyPrint,";
                sqlText += " LCBank,";
                sqlText += " LCDate,";
                sqlText += " DeliveryChallanNo,";
                sqlText += " IsGatePass,";
                sqlText += " CompInvoiceNo,";
                sqlText += " ShiftId,";
                sqlText += " ValueOnly,";
                sqlText += " PINo,";
                sqlText += " PIDate,";
                sqlText += " EXPFormNo,";
                sqlText += " EXPFormDate,";
                sqlText += " BranchId,";
                sqlText += " DeductionAmount,";
                sqlText += " SignatoryName,";
                sqlText += " SignatoryDesig,";
                sqlText += " IsDeemedExport,";
                sqlText += " SaleInvoiceNumber,";
                sqlText += " FiscalYear,";
                sqlText += " AppVersion,";

                #region HPS
                sqlText += " HPSTotalAmount";
                #endregion

                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += "@SalesInvoiceNo,";
                sqlText += "@CustomerID,";
                sqlText += "@DeliveryAddress1,";
                sqlText += "@DeliveryAddress2,";
                sqlText += "@DeliveryAddress3,";
                sqlText += "@VehicleID,";
                sqlText += "@InvoiceDateTime,";
                sqlText += "@DeliveryDate,";

                sqlText += "@TotalAmount,";
                sqlText += "@TotalVATAmount,";
                sqlText += "@VDSAmount,";

                sqlText += "@SerialNo,";
                sqlText += "@Comments,";
                sqlText += "@CreatedBy,";
                sqlText += "@CreatedOn,";
                sqlText += "@LastModifiedBy,";
                sqlText += "@LastModifiedOn,";
                sqlText += "@SaleType,";
                sqlText += "@PreviousSalesInvoiceNo,";
                sqlText += "@Trading,";
                sqlText += "@IsPrint,";
                sqlText += "@TenderId,";
                sqlText += "@TransactionType,";
                sqlText += "@Post,";
                sqlText += "@LCNumber,";
                sqlText += "@CurrencyID,";
                sqlText += "@CurrencyRateFromBDT,";
                sqlText += "@SaleReturnId,";
                sqlText += "@IsVDS,";
                sqlText += "@GetVDSCertificate,";
                sqlText += "@VDSCertificateDate,";
                sqlText += "@ImportIDExcel,";
                sqlText += "@AlReadyPrint,";
                sqlText += "@LCBank,";
                sqlText += "@LCDate,";
                sqlText += "@DeliveryChallanNo,";
                sqlText += "@IsGatePass,";
                sqlText += "@CompInvoiceNo,";
                sqlText += "@ShiftId,";
                sqlText += "@ValueOnly,";
                sqlText += "@PINo,";
                sqlText += "@PIDate,";
                sqlText += "@EXPFormNo,";
                sqlText += "@EXPFormDate,";
                sqlText += "@BranchId,";
                sqlText += "@DeductionAmount,";
                sqlText += "@SignatoryName,";
                sqlText += "@SignatoryDesig,";

                sqlText += "@IsDeemedExport,";
                sqlText += "@SaleInvoiceNumber,";
                sqlText += "@FiscalYear,";
                sqlText += "@AppVersion,";

                #region HPS
                sqlText += "@HPSTotalAmount";
                #endregion

                sqlText += ")  SELECT SCOPE_IDENTITY()";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Master.SalesInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress1", Master.DeliveryAddress1);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress2", Master.DeliveryAddress2);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress3", Master.DeliveryAddress3);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleID", Master.VehicleID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(Master.InvoiceDateTime));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryDate", OrdinaryVATDesktop.DateToDate(Master.DeliveryDate));

                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VDSAmount", Master.VDSAmount);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleType", Master.SaleType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Trading", Master.Trading);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsPrint", Master.IsPrint);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TenderId", Master.TenderId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LCNumber", Master.LCNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsVDS", Master.IsVDS == null ? "N" : Master.IsVDS);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@GetVDSCertificate", Master.GetVDSCertificate == null ? "N" : Master.GetVDSCertificate);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@VDSCertificateDate", OrdinaryVATDesktop.DateToDate(Master.VDSCertificateDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportIDExcel", Master.ImportIDExcel);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AlReadyPrint", Master.AlReadyPrint == null ? "0" : Master.AlReadyPrint);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LCBank", Master.LCBank);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@LCDate", OrdinaryVATDesktop.DateToDate(Master.LCDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryChallanNo", Master.DeliveryChallanNo == null ? "-" : Master.DeliveryChallanNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsGatePass", Master.IsGatePass == null ? "N" : Master.IsGatePass);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@CompInvoiceNo", Master.CompInvoiceNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@ValueOnly", Master.ValueOnly);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PINo", Master.PINo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PIDate", OrdinaryVATDesktop.DateToDate(Master.PIDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@EXPFormNo", Master.EXPFormNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@EXPFormDate", OrdinaryVATDesktop.DateToDate(Master.EXPFormDate));
                cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DeductionAmount", Master.DeductionAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@IsDeemedExport", Master.IsDeemedExport);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FiscalYear", Master.FiscalYear);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleInvoiceNumber", Master.SaleInvoiceNumber);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@AppVersion", Master.AppVersion);

                #region
                cmdInsert.Parameters.AddWithValueAndNullHandle("@HPSTotalAmount", Master.HPSTotalAmount);
                #endregion

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }
                #region Update  Receive header for trip
                if (!string.IsNullOrEmpty(Master.SerialNo) && (Master.SerialNo) != "-")
                {

                    sqlText = @"
                       update ReceiveHeaders set IsTripComplete = 'Y'
                         where ReferenceNo=@SerialNo";
                    SqlCommand cmdRecieveUpdate = new SqlCommand(sqlText, currConn);
                    cmdRecieveUpdate.Transaction = transaction;
                    cmdRecieveUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);

                    transResult = cmdRecieveUpdate.ExecuteNonQuery();
                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException("Uneble to update Receive Header");
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

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] SalesInsertToDetail(SaleDetailVm Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " insert into SalesInvoiceDetails(";

                sqlText += " SalesInvoiceNo";
                sqlText += " ,SourcePaidQuantity";
                sqlText += " ,SourcePaidVATAmount";
                sqlText += " ,NBRPriceInclusiveVAT";



                sqlText += " ,BOMId";
                sqlText += " ,InvoiceLineNo";
                sqlText += " ,ItemNo";
                sqlText += " ,Quantity";
                sqlText += " ,SalesPrice";
                sqlText += " ,NBRPrice";
                sqlText += " ,AVGPrice";
                sqlText += " ,UOM";
                sqlText += " ,VATRate";
                sqlText += " ,VATAmount";
                sqlText += " ,SubTotal";
                sqlText += " ,Comments";
                sqlText += " ,CreatedBy";
                sqlText += " ,CreatedOn";
                sqlText += " ,LastModifiedBy";
                sqlText += " ,LastModifiedOn";
                sqlText += " ,SD";
                sqlText += " ,SDAmount";
                sqlText += " ,VDSAmount";
                sqlText += " ,SaleType";
                sqlText += " ,PreviousSalesInvoiceNo";
                sqlText += " ,Trading";
                sqlText += " ,InvoiceDateTime";
                sqlText += " ,NonStock";
                sqlText += " ,TradingMarkUp";
                sqlText += " ,Type";
                sqlText += " ,BENumber";
                sqlText += " ,Post";
                sqlText += " ,UOMQty";
                sqlText += " ,UOMPrice";
                sqlText += " ,UOMc";
                sqlText += " ,UOMn";
                sqlText += " ,DollerValue";
                sqlText += " ,CurrencyValue";
                sqlText += " ,TransactionType";
                sqlText += " ,VATName";
                sqlText += " ,SaleReturnId";
                sqlText += " ,DiscountAmount";
                sqlText += " ,DiscountedNBRPrice";
                sqlText += " ,PromotionalQuantity";
                sqlText += " ,FinishItemNo";
                sqlText += " ,ValueOnly";
                sqlText += " ,CConversionDate";
                sqlText += " ,ReturnTransactionType";
                sqlText += " ,Weight";
                sqlText += " ,TotalValue";
                sqlText += " ,WareHouseRent";
                sqlText += " ,WareHouseVAT";
                sqlText += " ,ATVRate";
                sqlText += " ,ATVablePrice";
                sqlText += " ,ATVAmount";
                sqlText += " ,IsCommercialImporter";
                sqlText += " ,TradeVATRate";
                sqlText += " ,TradeVATAmount";
                sqlText += " ,BranchId";
                sqlText += " ,TradeVATableValue";
                sqlText += " ,CDNVATAmount";
                sqlText += " ,CDNSDAmount";
                sqlText += " ,CDNSubtotal";
                sqlText += " ,ProductDescription";
                sqlText += " ,IsFixedVAT";
                sqlText += " ,FixedVATAmount";
                sqlText += " ,HPSRate";
                sqlText += " ,HPSAmount";
                sqlText += " ,PreviousInvoiceDateTime";
                sqlText += " ,PreviousNBRPrice";
                sqlText += " ,PreviousQuantity";
                sqlText += " ,PreviousUOM";
                sqlText += " ,PreviousSubTotal";
                sqlText += " ,PreviousVATAmount";
                sqlText += " ,PreviousVATRate";
                sqlText += " ,PreviousSD";
                sqlText += " ,PreviousSDAmount";
                sqlText += " ,ReasonOfReturn";
                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@SalesInvoiceNo";
                sqlText += ",@SourcePaidQuantity";
                sqlText += ",@SourcePaidVATAmount";
                sqlText += ",@NBRPriceInclusiveVAT";

                sqlText += ",@BOMId";
                sqlText += ",@InvoiceLineNo";
                sqlText += ",@ItemNo";
                sqlText += ",@Quantity";
                sqlText += ",@SalesPrice";
                sqlText += ",@NBRPrice";
                sqlText += ",@AVGPrice";
                sqlText += ",@UOM";
                sqlText += ",@VATRate";
                sqlText += ",@VATAmount";
                sqlText += ",@SubTotal";
                sqlText += ",@Comments";
                sqlText += ",@CreatedBy";
                sqlText += ",@CreatedOn";
                sqlText += ",@LastModifiedBy";
                sqlText += ",@LastModifiedOn";
                sqlText += ",@SD";
                sqlText += ",@SDAmount";
                sqlText += ",@VDSAmount";
                sqlText += ",@SaleType";
                sqlText += ",@PreviousSalesInvoiceNo";
                sqlText += ",@Trading";
                sqlText += ",@InvoiceDateTime";
                sqlText += ",@NonStock";
                sqlText += ",@TradingMarkUp";
                sqlText += ",@Type";
                sqlText += ",@BENumber";
                sqlText += ",@Post";
                sqlText += ",@UOMQty";
                sqlText += ",@UOMPrice";
                sqlText += ",@UOMc";
                sqlText += ",@UOMn";
                sqlText += ",@DollerValue";
                sqlText += ",@CurrencyValue";
                sqlText += ",@TransactionType";
                sqlText += ",@VATName";
                sqlText += ",@SaleReturnId";
                sqlText += ",@DiscountAmount";
                sqlText += ",@DiscountedNBRPrice";
                sqlText += ",@PromotionalQuantity";
                sqlText += ",@FinishItemNo";
                sqlText += ",@ValueOnly";
                sqlText += ",@CConversionDate";
                sqlText += ",@ReturnTransactionType";
                sqlText += ",@Weight";
                sqlText += ",@TotalValue";
                sqlText += ",@WareHouseRent";
                sqlText += ",@WareHouseVAT";
                sqlText += ",@ATVRate";
                sqlText += ",@ATVablePrice";
                sqlText += ",@ATVAmount";
                sqlText += ",@IsCommercialImporter";
                sqlText += ",@TradeVATRate";
                sqlText += ",@TradeVATAmount";
                sqlText += ",@BranchId";
                sqlText += ",@TradeVATableValue";
                sqlText += ",@CDNVATAmount";
                sqlText += ",@CDNSDAmount";
                sqlText += ",@CDNSubtotal";
                sqlText += ",@ProductDescription";
                sqlText += ",@IsFixedVAT";
                sqlText += ",@FixedVATAmount";
                sqlText += ",@HPSRate";
                sqlText += ",@HPSAmount";
                sqlText += ",@PreviousInvoiceDateTime";
                sqlText += ",@PreviousNBRPrice";
                sqlText += ",@PreviousQuantity";
                sqlText += ",@PreviousUOM";
                sqlText += ",@PreviousSubTotal";
                sqlText += ",@PreviousVATAmount";
                sqlText += ",@PreviousVATRate";
                sqlText += ",@PreviousSD";
                sqlText += ",@PreviousSDAmount";
                sqlText += ",@ReasonOfReturn";
                sqlText += ")	";



                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Detail.SalesInvoiceNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SourcePaidQuantity", Detail.SourcePaidQuantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SourcePaidVATAmount", Detail.SourcePaidVATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@NBRPriceInclusiveVAT", Detail.NBRPriceInclusiveVAT);


                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@InvoiceLineNo", Detail.InvoiceLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Quantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SalesPrice", Detail.SalesPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@NBRPrice", Detail.NBRPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@AVGPrice", Detail.AvgRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Comments", Detail.CommentsD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CreatedOn", OrdinaryVATDesktop.DateToDate(Detail.CreatedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Detail.LastModifiedOn));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SD", Detail.SD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VDSAmount", Detail.VDSAmountD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SaleType", Detail.SaleTypeD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousSalesInvoiceNo", Detail.PreviousSalesInvoiceNoD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Trading", Detail.TradingD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(Detail.InvoiceDateTime));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@NonStock", Detail.NonStockD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TradingMarkUp", Detail.TradingMarkUp);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Type", Detail.Type);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BENumber", Detail.BENumber == null ? "-" : Detail.BENumber);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DollerValue", Detail.DollerValue);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CurrencyValue", Detail.CurrencyValue);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.TransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATName", Detail.VatName);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@SaleReturnId", Detail.ReturnId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DiscountAmount", Detail.DiscountAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DiscountedNBRPrice", Detail.DiscountedNBRPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PromotionalQuantity", Detail.PromotionalQuantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FinishItemNo", Detail.FinishItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ValueOnly", Detail.ValueOnly);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CConversionDate", OrdinaryVATDesktop.DateToDate(Detail.CConversionDate));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ReturnTransactionType", Detail.ReturnTransactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@Weight", Detail.Weight);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TotalValue", Detail.TotalValue);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@WareHouseRent", Detail.WareHouseRent);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@WareHouseVAT", Detail.WareHouseVAT);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ATVRate", Detail.ATVRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ATVablePrice", Detail.ATVablePrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ATVAmount", Detail.ATVAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IsCommercialImporter", Detail.IsCommercialImporter);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TradeVATRate", Detail.TradeVATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TradeVATAmount", Detail.TradeVATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Detail.BranchId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TradeVATableValue", Detail.TradeVATableValue);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CDNVATAmount", Detail.CDNVATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CDNSDAmount", Detail.CDNSDAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@CDNSubtotal", Detail.CDNSubtotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ProductDescription", Detail.ProductDescription);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@IsFixedVAT", Detail.IsFixedVAT);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@FixedVATAmount", Detail.FixedVATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@HPSRate", Detail.HPSRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@HPSAmount", Detail.HPSAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousInvoiceDateTime", OrdinaryVATDesktop.DateToDate(Detail.PreviousInvoiceDateTime));
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousNBRPrice", Detail.PreviousNBRPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousQuantity", Detail.PreviousQuantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousUOM", Detail.PreviousUOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousSubTotal", Detail.PreviousSubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousVATAmount", Detail.PreviousVATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousVATRate", Detail.PreviousVATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousSD", Detail.PreviousSD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@PreviousSDAmount", Detail.PreviousSDAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ReasonOfReturn", Detail.ReasonOfReturn);




                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }
                #endregion Insert only DetailTable

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
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Detail.SalesInvoiceNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            //catch (SqlException sqlex)
            //{
            //    retResults = new string[5];
            //    retResults[0] = "Fail";
            //    retResults[1] = sqlex.Message;
            //    retResults[2] = "0";
            //    retResults[3] = "N";
            //    retResults[4] = "0";
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

                throw new ArgumentNullException("", ex.Message.ToString());
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

        public string[] SalesUpdateToMaster(SaleMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region Entry Date Check

                string firstDate = "01-July-2019";

                if (Master.TransactionType == "Credit")
                {
                    if (Convert.ToDateTime(Master.DeliveryDate) < Convert.ToDateTime(firstDate))
                    {
                        retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }
                }
                else
                {
                    if (Convert.ToDateTime(Master.InvoiceDateTime) < Convert.ToDateTime(firstDate))
                    {
                        retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }
                }

                #endregion

                #region Update

                sqlText = "";
                sqlText += " update SalesInvoiceHeaders set  ";

                sqlText += " SalesInvoiceNo=@SalesInvoiceNo";
                sqlText += " ,CustomerID=@CustomerID";
                sqlText += " ,DeliveryAddress1=@DeliveryAddress1";
                sqlText += " ,DeliveryAddress2=@DeliveryAddress2";
                sqlText += " ,DeliveryAddress3=@DeliveryAddress3";
                sqlText += " ,VehicleID=@VehicleID";
                sqlText += " ,InvoiceDateTime=@InvoiceDateTime";
                sqlText += " ,DeliveryDate=@DeliveryDate";

                sqlText += " ,TotalAmount=@TotalAmount";
                sqlText += " ,TotalVATAmount=@TotalVATAmount";
                sqlText += " ,VDSAmount=@VDSAmount";

                sqlText += " ,SerialNo=@SerialNo";
                sqlText += " ,Comments=@Comments";
                sqlText += " ,LastModifiedBy=@LastModifiedBy";
                sqlText += " ,LastModifiedOn=@LastModifiedOn";
                sqlText += " ,SaleType=@SaleType";
                sqlText += " ,PreviousSalesInvoiceNo=@PreviousSalesInvoiceNo";
                sqlText += " ,Trading=@Trading";
                sqlText += " ,IsPrint=@IsPrint";
                sqlText += " ,TenderId=@TenderId";
                sqlText += " ,TransactionType=@TransactionType";
                sqlText += " ,Post=@Post";
                sqlText += " ,LCNumber=@LCNumber";
                sqlText += " ,CurrencyID=@CurrencyID";
                sqlText += " ,CurrencyRateFromBDT=@CurrencyRateFromBDT";
                sqlText += " ,SaleReturnId=@SaleReturnId";

                sqlText += " ,LCBank=@LCBank";
                sqlText += " ,LCDate=@LCDate";
                sqlText += " ,DeliveryChallanNo=@DeliveryChallanNo";
                sqlText += " ,IsGatePass=@IsGatePass";
                sqlText += " ,CompInvoiceNo=@CompInvoiceNo";
                sqlText += " ,ShiftId=@ShiftId";
                sqlText += " ,ValueOnly=@ValueOnly";
                sqlText += " ,PINo=@PINo";
                sqlText += " ,PIDate=@PIDate";
                sqlText += " ,EXPFormNo=@EXPFormNo";
                sqlText += " ,EXPFormDate=@EXPFormDate";

                sqlText += " ,IsVDS=@IsVDS";
                sqlText += " ,GetVDSCertificate=@GetVDSCertificate";
                sqlText += " ,VDSCertificateDate=@VDSCertificateDate";
                sqlText += " ,ImportIDExcel=@ImportIDExcel";
                sqlText += " ,AlReadyPrint=@AlReadyPrint";
                sqlText += " ,IsDeemedExport=@IsDeemedExport";
                sqlText += " ,BranchId=@BranchId";
                sqlText += " ,DeductionAmount=@DeductionAmount";
                sqlText += " ,SignatoryName=@SignatoryName";
                sqlText += " ,SignatoryDesig=@SignatoryDesig";

                #region
                sqlText += " ,HPSTotalAmount=@HPSTotalAmount";
                #endregion

                sqlText += " where                Id=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo", Master.SalesInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", Master.CustomerID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryAddress1", Master.DeliveryAddress1);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryAddress2", Master.DeliveryAddress2);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryAddress3", Master.DeliveryAddress3);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VehicleID", Master.VehicleID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(Master.InvoiceDateTime));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryDate", OrdinaryVATDesktop.DateToDate(Master.DeliveryDate));

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VDSAmount", Master.VDSAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SerialNo", Master.SerialNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Master.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SaleType", Master.SaleType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PreviousSalesInvoiceNo", Master.PreviousSalesInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Trading", Master.Trading);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsPrint", Master.IsPrint);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TenderId", Master.TenderId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Master.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LCNumber", Master.LCNumber);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SaleReturnId", Master.ReturnId);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LCBank", Master.LCBank);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LCDate", OrdinaryVATDesktop.DateToDate(Master.LCDate));

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CompInvoiceNo", Master.CompInvoiceNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ShiftId", Master.ShiftId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ValueOnly", Master.ValueOnly);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PINo", Master.PINo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@PIDate", OrdinaryVATDesktop.DateToDate(Master.PIDate));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@EXPFormNo", Master.EXPFormNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@EXPFormDate", OrdinaryVATDesktop.DateToDate(Master.EXPFormDate));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ImportIDExcel", Master.ImportIDExcel);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsVDS", Master.IsVDS);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@GetVDSCertificate", Master.GetVDSCertificate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VDSCertificateDate", OrdinaryVATDesktop.DateToDate(Master.VDSCertificateDate));
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@AlReadyPrint", Master.AlReadyPrint);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeliveryChallanNo", Master.DeliveryChallanNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsGatePass", Master.IsGatePass);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsDeemedExport", Master.IsDeemedExport);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DeductionAmount", Master.DeductionAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);

                #region
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@HPSTotalAmount", Master.HPSTotalAmount);
                #endregion

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.Id);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
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
                retResults[2] = "" + Master.Id;
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

        public string[] SalesUpdateComment(SaleMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update SalesInvoiceHeaders set  ";

                //sqlText += " CurrencyID=@CurrencyID";
                sqlText += " Comments=@Comments";

                //sqlText += " ,CurrencyRateFromBDT=@CurrencyRateFromBDT";
                //sqlText += " ,SaleReturnId=@SaleReturnId";


                sqlText += " where   SalesInvoiceNo=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;



                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);

                //cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                //cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                //cmdUpdate.Parameters.AddWithValueAndNullHandle("@SaleReturnId", Master.ReturnId);


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.SalesInvoiceNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
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

        public string[] SalesCurrencyValueUpdate(SaleMasterVM Master, List<SaleDetailVm> details, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update SalesInvoiceHeaders set  ";

                sqlText += " CurrencyID=@CurrencyID";
                //sqlText += " Comments=@Comments";

                sqlText += " ,CurrencyRateFromBDT=@CurrencyRateFromBDT";
                sqlText += " ,SaleReturnId=@SaleReturnId";
                sqlText += " ,IsCurrencyConvCompleted=@IsCurrencyConvCompleted";

                sqlText += " where   SalesInvoiceNo=@Id";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;



                //cmdUpdate.Parameters.AddWithValueAndNullHandle("@Comments", Master.Comments);

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyID", Master.CurrencyID);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT", Master.CurrencyRateFromBDT);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SaleReturnId", Master.ReturnId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsCurrencyConvCompleted", "Y");


                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Master.SalesInvoiceNo);

                transResult = cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }


                #endregion ID generated completed,Insert new Information in Header


                string updateDetails =
                    @"update 
SalesInvoiceDetails set 

CurrencyValue = @CurrencyValue,
 
DollerValue = @DollerValue, 

SaleReturnId = @SaleReturnId

where SalesInvoiceNo = @Id and ItemNo = @ItemNo
";

                cmdUpdate.CommandText = updateDetails;
                cmdUpdate.Parameters.AddWithValue("@CurrencyValue", "");
                cmdUpdate.Parameters.AddWithValue("@DollerValue", "");
                cmdUpdate.Parameters.AddWithValue("@ItemNo", "");
                foreach (SaleDetailVm detailVm in details)
                {
                    cmdUpdate.Parameters["@CurrencyValue"].Value = detailVm.CurrencyValue;
                    cmdUpdate.Parameters["@DollerValue"].Value = detailVm.DollerValue;
                    cmdUpdate.Parameters["@ItemNo"].Value = detailVm.ItemNo;

                    cmdUpdate.ExecuteNonQuery();

                    //cmdUpdate.Parameters.Remove("@CurrencyValue");
                    //cmdUpdate.Parameters.Remove("@DollerValue");
                    //cmdUpdate.Parameters.Remove("@ItemNo");
                }




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

        public string[] SaleAllPost(PostVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update SalesInvoiceHeaders set  ";
                sqlText += " LastModifiedBy             = @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn             = @MasterLastModifiedOn,";
                sqlText += " Post                       = @MasterPost";
                sqlText += " where  SalesInvoiceNo   = @MasterPurchaseInvoiceNo ";

                sqlText += " update SalesInvoiceDetails set ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  SalesInvoiceNo =@MasterPurchaseInvoiceNo ";


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
            return retResults;
            #endregion Result

        }

        #endregion

        #region Basic Method

        public string[] UpdatePrintNew(string InvoiceNo, int PrintCopy, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "0";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            int NewPrintCopy = 0;
            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(InvoiceNo))
                {
                    throw new ArgumentNullException("UpdatePrintNew", "Please select one invoice no.");
                }

                #endregion Validation

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                transaction = currConn.BeginTransaction("UpdatePrintNew");

                #endregion open connection and transaction

                //sqlText = @"SELECT isnull(AlReadyPrint,0)AlReadyPrint FROM SalesInvoiceHeaders WHERE SalesInvoiceNo =@InvoiceNo";

                //InvoiceNo = InvoiceNo.Replace("'","");

                //DataTable dataTable = new DataTable("SalesInvoiceHeaders");
                //SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                //cmdIdExist.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                //cmdIdExist.Transaction = transaction;

                //transResult = (int)cmdIdExist.ExecuteScalar();

                //if (transResult <= 0)
                //{
                //    //PrintCopy = 0;
                //}
                //else
                //{
                //NewPrintCopy = transResult + PrintCopy;
                //}

                #region Update Print
                if (PrintCopy > 0)
                {


                    sqlText = @"update SalesInvoiceHeaders set IsPrint='Y',AlReadyPrint=isnull(AlReadyPrint,0)+@PrintCopy where SalesInvoiceNo IN(@InvoiceNo)";

                    if (!InvoiceNo.Contains("'"))
                    {
                        InvoiceNo = "'" + InvoiceNo + "'";
                    }

                    sqlText = sqlText.Replace("@InvoiceNo", InvoiceNo);

                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);

                    cmdUpdate.CommandText = sqlText;
                    cmdUpdate.Transaction = transaction;
                    cmdUpdate.Parameters.AddWithValue("@PrintCopy", PrintCopy);
                    ////cmdUpdate.Parameters.AddWithValue("@InvoiceNo", InvoiceNo);
                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                }
                #region Commit

                if (transaction != null)
                {

                    transaction.Commit();
                    retResults[0] = "Success";
                    retResults[1] = "Sales Invoice Print Successfully Update.";
                    retResults[2] = "" + InvoiceNo;
                    retResults[3] = "" + NewPrintCopy.ToString();
                }
                else
                {
                    retResults[0] = "Fail";
                    retResults[1] = "Unexpected error to update print.";
                    retResults[2] = "" + InvoiceNo;
                    retResults[3] = "" + NewPrintCopy.ToString();

                }

                #endregion Commit

                #endregion Update Print

            }
            #region catch

            //catch (SqlException sqlex)
            //{
            //    if (transaction != null)
            //        transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //}
            catch (Exception ex)
            {
                retResults = new string[5];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                retResults[4] = "0";
                if (transaction != null)
                    transaction.Rollback();
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

            return retResults;
        }

        //currConn to VcurrConn 25-Aug-2020
        public string[] SalesInsert(SaleMasterVM MasterVM, List<SaleDetailVm> DetailVMs, List<SaleExportVM> ExportDetails
            , List<TrackingVM> Trackings, SqlTransaction Vtransaction, SqlConnection VcurrConn, int branchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            bool PriceDeclarationTradingProduct = false;




            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";

            string newIDCreate = "";
            string PostStatus = "";

            int IDExist = 0;
            string vehicleId = "0";
            string vProductType = "";


            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(MasterVM.InvoiceDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

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
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(MasterVM.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(MasterVM.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header

                CommonDAL commonDal = new CommonDAL();

                #region Load Settings

                DataTable settings = new DataTable();
                if (settingVM.SettingsDTUser != null && settingVM.SettingsDTUser.Rows.Count > 0)
                {
                    settings = settingVM.SettingsDTUser;
                }
                else
                {
                    settings = new CommonDAL().SettingDataAll(null, null);
                }

                var tt = commonDal.settingsCache("Sale", "TotalPrice", settings);
                bool IsTotalPrice = Convert.ToBoolean(commonDal.settingsCache("Sale", "TotalPrice", settings).ToString().ToLower() == "y" ? true : false);


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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }



                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = MasterVM.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(MasterVM.InvoiceDateTime).ToString("yyyy-MM-dd");
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

                #region Find Transaction Exist

                sqlText = "";
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
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

                #region Other


                if (MasterVM.TransactionType == "Other")
                {
                    newIDCreate = commonDal.TransactionCode("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                              "InvoiceDateTime", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

                    //New Method

                    var resultCode = commonDal.GetCurrentCode("Sale", "Other", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn,
                        transaction);

                    var newIdara = resultCode.Split('~');

                    latestId = newIdara[0];
                    fiscalYear = newIdara[1];

                }

                #endregion

                #region Export

                else if (
                    MasterVM.TransactionType == "Export"
                )
                {
                    newIDCreate = commonDal.TransactionCode("Sale", "Export", "SalesInvoiceHeaders", "SalesInvoiceNo",
                                             "InvoiceDateTime", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);


                    var resultCode = commonDal.GetCurrentCode("Sale", "Export", MasterVM.InvoiceDateTime, MasterVM.BranchId.ToString(), currConn, transaction);

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

                #region checkId and FiscalYear

                sqlText = @"select count(SaleInvoiceNumber) from SalesInvoiceHeaders 
where SaleInvoiceNumber = @SaleInvoiceNumber and FiscalYear = @FiscalYear and TransactionType = @TransactionType and BranchId = @BranchId";

                var sqlCmd = new SqlCommand(sqlText, currConn, transaction);

                // latestId = (Convert.ToInt32(latestId) - 1).ToString();

                sqlCmd.Parameters.AddWithValue("@SaleInvoiceNumber", latestId);
                sqlCmd.Parameters.AddWithValue("@FiscalYear", fiscalYear);
                sqlCmd.Parameters.AddWithValue("@TransactionType", MasterVM.TransactionType);
                sqlCmd.Parameters.AddWithValue("@BranchId", MasterVM.BranchId);

                var count = (int)sqlCmd.ExecuteScalar();

                if (count > 0)
                {
                    FileLogger.Log("SaleDAL", "Insert", "SaleInvoiceNumber " + newIDCreate + " Already Exists");
                    throw new Exception("Sales Id " + newIDCreate + " Already Exists");
                }

                #endregion

                #region Check Existing Id

                sqlText = "select count(SalesInvoiceNo) from SalesInvoiceHeaders where SalesInvoiceNo = @SalesInvoiceNo";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@SalesInvoiceNo", newIDCreate);

                var count1 = (int)cmd.ExecuteScalar();

                if (count1 > 0)
                {
                    FileLogger.Log("SaleDAL", "Insert", "Sales Id " + newIDCreate + " Already Exists");
                    throw new Exception("Sales Id " + newIDCreate + " Already Exists");
                }

                #endregion

                MasterVM.SalesInvoiceNo = newIDCreate;

                #endregion Purchase ID Create Not Complete

                #region VehicleId
                if (string.IsNullOrEmpty(MasterVM.VehicleID) || Convert.ToDecimal(MasterVM.VehicleID) <= 0)
                {
                    string vehicleID = "0";
                    sqlText = "";
                    sqlText = sqlText + "select VehicleID from Vehicles WHERE VehicleNo=@MasterVehicleNo ";

                    SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleId.Transaction = transaction;
                    cmdExistVehicleId.Parameters.AddWithValue("@MasterVehicleNo", MasterVM.VehicleNo);
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
                        if (MasterVM.vehicleSaveInDB == true)
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
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleType", MasterVM.VehicleType ?? "-");
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterVehicleNo", MasterVM.VehicleNo);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedBy", MasterVM.CreatedBy);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(MasterVM.CreatedOn));
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedBy", MasterVM.LastModifiedBy);
                        cmdExistVehicleIns.Parameters.AddWithValue("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(MasterVM.LastModifiedOn));
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

                    MasterVM.VehicleID = vehicleId;

                }



                #endregion VehicleId

                #region ID generated completed,Insert new Information in Header

                var vTotalAmount = commonDal.decimal259(MasterVM.TotalAmount);


                MasterVM.TotalSubtotal = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalSubtotal));
                MasterVM.TotalAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalAmount));
                MasterVM.TotalVATAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.TotalVATAmount));
                MasterVM.VDSAmount = Convert.ToDecimal(commonDal.decimal259(MasterVM.VDSAmount));
                MasterVM.CurrencyRateFromBDT = Convert.ToDecimal(commonDal.decimal259(MasterVM.CurrencyRateFromBDT));
                MasterVM.SaleInvoiceNumber = latestId;
                MasterVM.FiscalYear = fiscalYear;

                retResults = SalesInsertToMaster(MasterVM, currConn, transaction, null, settings);

                Id = retResults[4];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (DetailVMs.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }

                #endregion Validation for Detail

                #region Set BOMId

                SetBOMId(MasterVM, DetailVMs, currConn, transaction);

                #endregion


                retResults = SalesInsert2(MasterVM, DetailVMs, ExportDetails, Trackings, transaction, currConn, settings,UserId);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }
                #endregion Insert into Details(Insert complete in Header)

                


                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE SalesInvoiceHeaders 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo


UPDATE SalesInvoiceDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@SalesInvoiceNo", MasterVM.SalesInvoiceNo);

                transResult = cmdUpdate.ExecuteNonQuery();

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


        //currConn to VcurrConn 25-Aug-2020
        public string[] SalesInsert2(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings
            , SqlTransaction Vtransaction, SqlConnection VcurrConn, DataTable settings = null, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";
            //bool PriceDeclarationTradingProduct = false;
            ProductDAL productDal = new ProductDAL();
            IssueDAL issDal = new IssueDAL();
            ReceiveDAL recDal = new ReceiveDAL();
            VehicleDAL vDAL = new VehicleDAL();

            int transResult = 0;
            string sqlText = "";


            int IDExist = 0;
            string vehicleId = "0";
            string vProductType = "";


            #endregion Initializ

            #region Try
            try
            {
                CommonDAL commonDal = new CommonDAL();
                int IssuePlaceQty = Convert.ToInt32(commonDal.settingsCache("Issue", "Quantity", settings));
                int IssuePlaceAmt = Convert.ToInt32(commonDal.settingsCache("Issue", "Amount", settings));
                int RPlaceQty = Convert.ToInt32(commonDal.settingsCache("Receive", "Quantity", settings));
                int RPlaceAmt = Convert.ToInt32(commonDal.settingsCache("Receive", "Amount", settings));
                var tt = commonDal.settingsCache("Sale", "TotalPrice", settings);
                bool IsTotalPrice = Convert.ToBoolean(commonDal.settingsCache("Sale", "TotalPrice", settings).ToString().ToLower() == "y" ? true : false);
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, "Please Check Invoice Data and Time");

                }

                #endregion Validation for Header

                #region Fiscal Year Check

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");


                #endregion Fiscal Year CHECK

                #region if Transection not Other Insert Issue /Receive

                #region Sale For InternalIssue or TollIssue or Trading

                if (Master.TransactionType == "Service"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "InternalIssue"
                    )
                {

                    #region Insert to Issue Header
                    IssueMasterVM imVM = new IssueMasterVM();
                    imVM.ShiftId = Master.ShiftId;
                    imVM.IssueNo = Master.SalesInvoiceNo;
                    imVM.IssueDateTime = Master.InvoiceDateTime;
                    imVM.TotalVATAmount = 0;
                    imVM.TotalAmount = 0;
                    imVM.SerialNo = Master.SalesInvoiceNo;
                    imVM.Comments = Master.Comments;
                    imVM.CreatedBy = Master.CreatedBy;
                    imVM.CreatedOn = Master.CreatedOn;
                    imVM.LastModifiedBy = Master.LastModifiedBy;
                    imVM.LastModifiedOn = Master.LastModifiedOn;
                    imVM.ReceiveNo = Master.SalesInvoiceNo;
                    imVM.transactionType = Master.TransactionType;
                    imVM.ReturnId = Master.ReturnId;
                    imVM.Post = Master.Post;
                    imVM.BranchId = Master.BranchId;
                    //imVM.SaleInvoiceNumber = Master.SaleInvoiceNumber;
                    //imVM.FiscalYear = Master.FiscalYear;

                    retResults = issDal.IssueInsertToMaster(imVM, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #region Comments


                    #endregion

                    #endregion Insert to Issue Header

                    #region Insert to Receive
                    ReceiveMasterVM rmVM = new ReceiveMasterVM();
                    rmVM.ShiftId = Master.ShiftId;
                    rmVM.ReceiveNo = Master.SalesInvoiceNo;
                    rmVM.ReceiveDateTime = Master.InvoiceDateTime;
                    rmVM.TotalAmount = 0;
                    rmVM.TotalVATAmount = 0;
                    rmVM.SerialNo = Master.SalesInvoiceNo;
                    rmVM.Comments = Master.Comments;
                    rmVM.CreatedBy = Master.CreatedBy;
                    rmVM.CreatedOn = Master.CreatedOn;
                    rmVM.LastModifiedBy = Master.LastModifiedBy;
                    rmVM.LastModifiedOn = Master.LastModifiedOn;
                    rmVM.transactionType = Master.TransactionType;
                    rmVM.ReturnId = Master.ReturnId;
                    rmVM.Post = Master.Post;
                    rmVM.BranchId = Master.BranchId;

                    retResults = recDal.ReceiveInsertToMaster(rmVM, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }


                    #region Comments



                    #endregion

                    #endregion Insert to Receive Header

                }
                else if (Master.TransactionType == "Trading"
              || Master.TransactionType == "TradingTender"
              || Master.TransactionType == "ExportTrading"
              || Master.TransactionType == "ExportTradingTender"
                    )
                {
                    #region Insert to Issue Header
                    IssueMasterVM imVM = new IssueMasterVM();
                    imVM.ShiftId = Master.ShiftId;
                    imVM.IssueNo = Master.SalesInvoiceNo;
                    imVM.IssueDateTime = Master.InvoiceDateTime;
                    imVM.TotalVATAmount = 0;
                    imVM.TotalAmount = 0;
                    imVM.SerialNo = Master.SalesInvoiceNo;
                    imVM.Comments = Master.Comments;
                    imVM.CreatedBy = Master.CreatedBy;
                    imVM.CreatedOn = Master.CreatedOn;
                    imVM.LastModifiedBy = Master.LastModifiedBy;
                    imVM.LastModifiedOn = Master.LastModifiedOn;
                    imVM.ReceiveNo = Master.SalesInvoiceNo;
                    imVM.transactionType = Master.TransactionType;
                    imVM.ReturnId = Master.ReturnId;
                    imVM.Post = Master.Post;
                    imVM.BranchId = Master.BranchId;

                    retResults = issDal.IssueInsertToMaster(imVM, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #region Comments


                    #endregion

                    #endregion Insert to Issue Header

                }

                #endregion Purchase ID Create For IssueReturn

                #region TollIssue
                else if (Master.TransactionType == "TollIssue")
                {
                    #region Insert to Issue Header
                    IssueMasterVM imVM = new IssueMasterVM();
                    imVM.ShiftId = Master.ShiftId;
                    imVM.IssueNo = Master.SalesInvoiceNo;
                    imVM.IssueDateTime = Master.InvoiceDateTime;
                    imVM.TotalVATAmount = 0;
                    imVM.TotalAmount = 0;
                    imVM.SerialNo = Master.SalesInvoiceNo;
                    imVM.Comments = Master.Comments;
                    imVM.CreatedBy = Master.CreatedBy;
                    imVM.CreatedOn = Master.CreatedOn;
                    imVM.LastModifiedBy = Master.LastModifiedBy;
                    imVM.LastModifiedOn = Master.LastModifiedOn;
                    imVM.ReceiveNo = Master.SalesInvoiceNo;
                    imVM.transactionType = Master.TransactionType;
                    imVM.ReturnId = Master.ReturnId;
                    imVM.Post = Master.Post;
                    imVM.BranchId = Master.BranchId;

                    retResults = issDal.IssueInsertToMaster(imVM, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #endregion Insert to Issue Header

                }

                #endregion TollIssue

                #region Sale for Wastage
                if (Master.TransactionType == "Wastage" || Master.TransactionType == "SaleWastage")
                {
                    #region Insert to Receive
                    ReceiveMasterVM rmVM = new ReceiveMasterVM();
                    rmVM.ShiftId = Master.ShiftId;
                    rmVM.ReceiveNo = Master.SalesInvoiceNo;
                    rmVM.ReceiveDateTime = Master.InvoiceDateTime;
                    rmVM.TotalAmount = 0;
                    rmVM.TotalVATAmount = 0;
                    rmVM.SerialNo = Master.SalesInvoiceNo;
                    rmVM.Comments = Master.Comments;
                    rmVM.CreatedBy = Master.CreatedBy;
                    rmVM.CreatedOn = Master.CreatedOn;
                    rmVM.LastModifiedBy = Master.LastModifiedBy;
                    rmVM.LastModifiedOn = Master.LastModifiedOn;
                    rmVM.transactionType = Master.TransactionType;
                    rmVM.ReturnId = Master.ReturnId;
                    rmVM.Post = Master.Post;
                    rmVM.BranchId = Master.BranchId;
                    retResults = recDal.ReceiveInsertToMaster(rmVM, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #region Comments

                    #endregion

                    #endregion Insert to Receive Header
                }
                #endregion Sale for Wastage
                //bool TradingWithSale = Convert.ToBoolean(new CommonDAL().settings("Trading", "TradingWithSale"));
                bool TradingWithSale = Convert.ToBoolean(new CommonDAL().settingsCache("Trading", "TradingWithSale", settings).ToString().ToLower() == "y" ? true : false);

                if (TradingWithSale == true)
                {
                    foreach (var Item in Details.ToList())
                    {
                        string ItemType = productDal.GetProductTypeByItemNo(Item.ItemNo, VcurrConn, Vtransaction);
                        if (ItemType.ToString().ToLower() == "trading")
                        {
                            vProductType = "trading";
                        }
                    }
                    #region vProductType == "trading"
                    if (vProductType.ToLower() == "trading")
                    {
                        #region Insert to Receive Issue Headers
                        ReceiveMasterVM rmVM = new ReceiveMasterVM();

                        rmVM.ReceiveNo = Master.SalesInvoiceNo;
                        rmVM.ReceiveDateTime = Master.InvoiceDateTime;
                        rmVM.TotalAmount = 0;
                        rmVM.TotalVATAmount = 0;
                        rmVM.SerialNo = Master.SalesInvoiceNo;
                        rmVM.Comments = Master.Comments;
                        rmVM.CreatedBy = Master.CreatedBy;
                        rmVM.CreatedOn = Master.CreatedOn;
                        rmVM.LastModifiedBy = Master.LastModifiedBy;
                        rmVM.LastModifiedOn = Master.LastModifiedOn;
                        rmVM.transactionType = "TradingAuto";
                        rmVM.ReturnId = Master.ReturnId;
                        rmVM.Post = Master.Post;
                        rmVM.BranchId = Master.BranchId;

                        retResults = recDal.ReceiveInsertToMaster(rmVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, retResults[1]);

                        }
                        IssueMasterVM ivm = new IssueMasterVM();
                        ivm.IssueNo = Master.SalesInvoiceNo;
                        ivm.IssueDateTime = Master.InvoiceDateTime;
                        ivm.TotalVATAmount = 0;
                        ivm.TotalAmount = 0;
                        ivm.SerialNo = Master.SalesInvoiceNo;
                        ivm.Comments = Master.Comments;
                        ivm.CreatedBy = Master.CreatedBy;
                        ivm.CreatedOn = Master.CreatedOn;
                        ivm.LastModifiedBy = Master.LastModifiedBy;
                        ivm.LastModifiedOn = Master.LastModifiedOn;
                        ivm.ReceiveNo = Master.SalesInvoiceNo;
                        ivm.transactionType = "TradingAuto";
                        ivm.ReturnId = Master.ReturnId;
                        ivm.Post = Master.Post;
                        ivm.BranchId = Master.BranchId;

                        retResults = issDal.IssueInsertToMaster(ivm, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, retResults[1]);
                        }


                        #endregion Insert to Receive Issue Headers
                    }
                    #endregion vProductType == "trading"

                } //if (TradingWithSale == false)




                #endregion if Transection not Other Insert Issue /Receive

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                List<SaleDetailVm> detailVms = new List<SaleDetailVm>();

                foreach (var Item in Details.ToList())
                {

                    #region Find AVG Rate

                    decimal AvgRate = 0;
                    decimal amount = 0;
                    decimal quantity = 0;

                    DataTable priceData = null;
                    if (Item.ReturnTransactionType == "InternalIssue" || Item.ReturnTransactionType == "Trading" || Item.ReturnTransactionType == "Service")
                    {
                        priceData = productDal.AvgPriceForInternalSales(Item.ItemNo, Master.InvoiceDateTime, VcurrConn, Vtransaction, false);
                        amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                    }
                    else
                    {
                        if (Master.TransactionType == "Service"
                        || Master.TransactionType == "ExportService"
                        || Master.TransactionType == "Trading"
                        || Master.TransactionType == "ExportTradingTender"
                        || Master.TransactionType == "TradingTender"
                        || Master.TransactionType == "ExportTrading"
                        || Master.TransactionType == "TollIssue"
                        || Master.TransactionType == "TollIssue"
                        || Master.TransactionType == "InternalIssue")
                        {
                            priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, VcurrConn, Vtransaction, true, true, true, true, null,UserId);
                            amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                            quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        }
                        else if (Master.TransactionType == "RawSale"
                        || Master.TransactionType == "RawCredit"
                      )
                        {
                            priceData = productDal.AvgPriceNew(Item.ItemNo, Master.InvoiceDateTime, VcurrConn, Vtransaction, true, true, false, true, null);
                            amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                            quantity = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        }

                    }

                    if (quantity > 0)
                    {
                        AvgRate = amount / quantity;
                    }
                    else
                    {
                        AvgRate = 0;
                    }

                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);

                    #endregion Find AVG Rate

                    #region Insert Issue and Receive if Transaction is not Other

                    #region Transaction is Wastage
                    if (Master.TransactionType == "Wastage" || Master.TransactionType == "SaleWastage")
                    {
                        #region Insert to Receive

                        decimal subTotalRecive = Item.NBRPrice * Item.Quantity;

                        ReceiveDetailVM rdVM = new ReceiveDetailVM();

                        rdVM.BOMId = Item.BOMId;
                        rdVM.ReceiveNo = Master.SalesInvoiceNo;
                        rdVM.ReceiveLineNo = Item.InvoiceLineNo;
                        rdVM.ItemNo = Item.ItemNo;
                        rdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        rdVM.CostPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.NBRPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.UOM = Item.UOM;
                        rdVM.VATRate = 0;
                        rdVM.VATAmount = 0;
                        if (IsTotalPrice)
                        {
                            rdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            rdVM.SubTotal = Convert.ToDecimal((subTotalRecive));
                        }

                        rdVM.CommentsD = Item.CommentsD;
                        rdVM.CreatedBy = Master.CreatedBy;
                        rdVM.CreatedOn = Master.CreatedOn;
                        rdVM.LastModifiedBy = Master.LastModifiedBy;
                        rdVM.LastModifiedOn = Master.LastModifiedOn;
                        rdVM.SD = 0;
                        rdVM.SDAmount = 0;
                        rdVM.ReceiveDateTime = Master.InvoiceDateTime;
                        rdVM.transactionType = Master.TransactionType;
                        rdVM.ReturnId = Master.ReturnId;
                        rdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        rdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        rdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        rdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        rdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        rdVM.UOMn = Item.UOMn;
                        rdVM.Post = Master.Post;
                        rdVM.BranchId = Master.BranchId;

                        retResults = recDal.ReceiveInsertToDetail(rdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }



                        #endregion Insert to Receive
                        #region Update Receive

                        sqlText = "";


                        sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                        sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                        sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + Master.SalesInvoiceNo + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateReceive.Transaction = Vtransaction;
                        int UpdateReceive = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (UpdateReceive <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive
                    }

                    #endregion Transaction is Wastage

                    #region  if (TradingWithSale == false)
                    if (TradingWithSale == true)
                    {
                        string ItemType = productDal.GetProductTypeByItemNo(Item.ItemNo, VcurrConn, Vtransaction);

                        if (ItemType.ToLower() == "trading")
                        {
                            DataTable bomDt = new DataTable();
                            bomDt = new BOMDAL().SelectAll(Item.BOMId.ToString(), null, null, VcurrConn, Vtransaction, null, true);
                            //////decimal declarePrice = Convert.ToDecimal(bomDt.Rows[0]["NBRPrice"]);
                            string VATName = "VAT 4.3";
                            string EffectDate = DateTime.Now.ToString();

                            if (bomDt != null && bomDt.Rows.Count > 0)
                            {
                                VATName = bomDt.Rows[0]["VATName"].ToString();
                                EffectDate = bomDt.Rows[0]["EffectDate"].ToString();
                            }

                            string UOM = productDal.GetProductTypeByItemNo(Item.ItemNo, VcurrConn, Vtransaction);

                            #region Insert to Receive  17 in
                            ReceiveDetailVM rdetVM = new ReceiveDetailVM();
                            rdetVM.ReceiveNo = Master.SalesInvoiceNo;
                            rdetVM.ReceiveLineNo = Item.InvoiceLineNo;
                            rdetVM.ItemNo = Item.ItemNo;
                            rdetVM.Quantity = Item.Quantity;
                            rdetVM.CostPrice = Item.NBRPrice;
                            rdetVM.NBRPrice = Item.NBRPrice;
                            rdetVM.UOM = UOM;
                            rdetVM.VATRate = 0;
                            rdetVM.VATAmount = 0;
                            rdetVM.SubTotal = Item.NBRPrice * Item.UOMQty;
                            rdetVM.CommentsD = Item.CommentsD;
                            rdetVM.CreatedBy = Master.CreatedBy;
                            rdetVM.CreatedOn = Master.CreatedOn;
                            rdetVM.LastModifiedBy = Master.LastModifiedBy;
                            rdetVM.LastModifiedOn = Master.LastModifiedOn;
                            rdetVM.SD = 0;
                            rdetVM.SDAmount = 0;
                            rdetVM.ReceiveDateTime = Master.InvoiceDateTime;
                            rdetVM.transactionType = "TradingAuto";
                            rdetVM.ReturnId = Master.ReturnId;
                            rdetVM.VatName = VATName;
                            rdetVM.UOMQty = Item.UOMQty;
                            rdetVM.UOMPrice = Item.UOMPrice;
                            rdetVM.UOMc = Item.UOMc;
                            rdetVM.UOMn = Item.UOMn;
                            rdetVM.Post = Master.Post;
                            rdetVM.BranchId = Master.BranchId;

                            retResults = recDal.ReceiveInsertToDetail(rdetVM, VcurrConn, Vtransaction);
                            if (retResults[0] != "Success")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, retResults[1]);
                            }



                            #endregion Insert to Receive

                            #region Update Receive

                            sqlText = "";

                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                            sqlText += " where ReceiveHeaders.ReceiveNo='" + Master.SalesInvoiceNo + "' ";

                            SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, VcurrConn);
                            cmdUpdateReceive.Transaction = Vtransaction;
                            cmdUpdateReceive.ExecuteNonQuery();
                            transResult = 1;


                            #endregion Update Receive

                            #region Issue
                            DataTable avgPrice = productDal.AvgPriceNew(Item.ItemNo.Trim(),
                                                              Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MMM-dd") +
                                                             DateTime.Now.ToString(" HH:mm:00"), VcurrConn, Vtransaction, true, true, false, false, null,UserId);
                            decimal Amount = Convert.ToDecimal(avgPrice.Rows[0]["Amount"].ToString());
                            decimal Quantity = Convert.ToDecimal(avgPrice.Rows[0]["Quantity"].ToString());
                            decimal costPrice = 0;
                            if (Quantity > 0)
                            {
                                costPrice = Amount / Quantity;
                            }

                            IssueDetailVM IdVM = new IssueDetailVM();
                            IdVM.IssueNo = Master.SalesInvoiceNo;
                            IdVM.IssueLineNo = Item.InvoiceLineNo;
                            IdVM.ItemNo = Item.ItemNo;
                            IdVM.Quantity = Item.Quantity;
                            IdVM.NBRPrice = costPrice;
                            IdVM.CostPrice = costPrice;
                            IdVM.UOM = UOM;
                            IdVM.VATRate = 0;
                            IdVM.VATAmount = 0;
                            IdVM.SubTotal = Item.Quantity * costPrice;

                            IdVM.CommentsD = Item.CommentsD;
                            IdVM.CreatedBy = Master.CreatedBy;
                            IdVM.CreatedOn = Master.CreatedOn;
                            IdVM.LastModifiedBy = Master.LastModifiedBy;
                            IdVM.LastModifiedOn = Master.LastModifiedOn;
                            IdVM.ReceiveNo = Master.SalesInvoiceNo;
                            IdVM.IssueDateTime = Master.InvoiceDateTime;
                            IdVM.SD = 0;
                            IdVM.SDAmount = 0;
                            IdVM.Wastage = 0;
                            IdVM.BOMDate = Convert.ToDateTime(EffectDate).ToString("MM/dd/yyyy");
                            IdVM.FinishItemNo = Item.ItemNo;
                            IdVM.transactionType = "TradingAuto";
                            IdVM.IssueReturnId = Master.ReturnId;
                            IdVM.UOMQty = Item.UOMQty;
                            IdVM.UOMPrice = costPrice * Item.UOMc;
                            IdVM.UOMc = Item.UOMc;
                            IdVM.UOMn = Item.UOMn;
                            IdVM.UOMWastage = 0;
                            IdVM.BOMId = Item.BOMId;
                            IdVM.Post = Master.Post;
                            IdVM.BranchId = Master.BranchId;

                            retResults = issDal.IssueInsertToDetails(IdVM, VcurrConn, Vtransaction);
                            if (retResults[0] != "Success")
                            {
                                throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, retResults[1]);
                            }
                            #endregion Issue



                            #region Update Issue

                            sqlText = "";
                            sqlText += " update IssueHeaders set ";
                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                            sqlText += " where (IssueHeaders.IssueNo= '" + Master.SalesInvoiceNo + "')";

                            SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
                            cmdUpdateIssue.Transaction = Vtransaction;
                            cmdUpdateIssue.ExecuteNonQuery();
                            transResult = 1;

                            #endregion
                        } //if (ItemType.ToLower() == "trading" && Item.BOMId > 0)
                    }
                    #endregion  if (TradingWithSale == false)

                    #region Service

                    if (Master.TransactionType == "Service"
                        || Master.TransactionType == "ExportService"
                        )
                    {
                        #region Insert to Issue

                        decimal subTotatlIssue = AvgRate * Item.Quantity;
                        IssueDetailVM issdVM = new IssueDetailVM();

                        issdVM.BOMId = Item.BOMId;
                        issdVM.IssueNo = Master.SalesInvoiceNo;
                        issdVM.IssueLineNo = Item.InvoiceLineNo;
                        issdVM.ItemNo = Item.ItemNo;
                        issdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        issdVM.NBRPrice = 0;
                        issdVM.CostPrice = Convert.ToDecimal((AvgRate));
                        issdVM.UOM = Item.UOM;
                        issdVM.VATRate = 0;
                        issdVM.VATAmount = 0;
                        if (IsTotalPrice)
                        {
                            issdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            issdVM.SubTotal = Convert.ToDecimal((subTotatlIssue));
                        }
                        issdVM.CommentsD = Item.CommentsD;
                        issdVM.CreatedBy = Master.CreatedBy;
                        issdVM.CreatedOn = Master.CreatedOn;
                        issdVM.LastModifiedBy = Master.LastModifiedBy;
                        issdVM.LastModifiedOn = Master.LastModifiedOn;
                        issdVM.ReceiveNo = Master.SalesInvoiceNo;
                        issdVM.IssueDateTime = Master.InvoiceDateTime;
                        issdVM.SD = 0;
                        issdVM.SDAmount = 0;
                        issdVM.Wastage = 0;
                        issdVM.BOMDate = "1900/01/01";
                        issdVM.FinishItemNo = "0";
                        issdVM.transactionType = Master.TransactionType;
                        issdVM.IssueReturnId = Master.ReturnId;
                        issdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        issdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        issdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        issdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        issdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        issdVM.UOMn = (Item.UOMn);
                        issdVM.Post = Master.Post;
                        issdVM.BranchId = Master.BranchId;
                        retResults = issDal.IssueInsertToDetails(issdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }


                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + Master.SalesInvoiceNo + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateIssue.Transaction = Vtransaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        decimal subTotalRecive = Item.NBRPrice * Item.Quantity;
                        ReceiveDetailVM rdVM = new ReceiveDetailVM();

                        rdVM.BOMId = Item.BOMId;
                        rdVM.ReceiveNo = Master.SalesInvoiceNo;
                        rdVM.ReceiveLineNo = Item.InvoiceLineNo;
                        rdVM.ItemNo = Item.ItemNo;
                        rdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        rdVM.CostPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.NBRPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.UOM = Item.UOM;
                        rdVM.VATRate = 0;
                        rdVM.VATAmount = 0;
                        if (IsTotalPrice)
                        {
                            rdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            rdVM.SubTotal = Convert.ToDecimal((subTotalRecive));

                        }
                        rdVM.CommentsD = Item.CommentsD;
                        rdVM.CreatedBy = Master.CreatedBy;
                        rdVM.CreatedOn = Master.CreatedOn;
                        rdVM.LastModifiedBy = Master.LastModifiedBy;
                        rdVM.LastModifiedOn = Master.LastModifiedOn;
                        rdVM.SD = 0;
                        rdVM.SDAmount = 0;
                        rdVM.ReceiveDateTime = Master.InvoiceDateTime;
                        rdVM.transactionType = Master.TransactionType;
                        rdVM.ReturnId = Master.ReturnId;
                        rdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        rdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        rdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        rdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        rdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        rdVM.UOMn = Item.UOMn;
                        rdVM.Post = Master.Post;
                        rdVM.BranchId = Master.BranchId;
                        retResults = recDal.ReceiveInsertToDetail(rdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }



                        #endregion Insert to Receive
                        #region Update Receive

                        sqlText = "";


                        sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                        sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                        sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + Master.SalesInvoiceNo + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateReceive.Transaction = Vtransaction;
                        int UpdateReceive = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (UpdateReceive <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive


                    }
                    #endregion Service

                    #region Transaction is Trading

                    else if (Master.TransactionType == "Trading"
                   || Master.TransactionType == "ExportTradingTender"
                   || Master.TransactionType == "TradingTender"
                   || Master.TransactionType == "ExportTrading"
                   )
                    {
                        #region Insert to Issue

                        decimal subTotatlIssue = AvgRate * Item.Quantity;
                        IssueDetailVM issdVM = new IssueDetailVM();

                        issdVM.BOMId = Item.BOMId;
                        issdVM.IssueNo = Master.SalesInvoiceNo;
                        issdVM.IssueLineNo = Item.InvoiceLineNo;
                        issdVM.ItemNo = Item.ItemNo;
                        issdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        issdVM.NBRPrice = 0;
                        issdVM.CostPrice = Convert.ToDecimal((AvgRate));
                        issdVM.UOM = Item.UOM;
                        issdVM.VATRate = 0;
                        issdVM.VATAmount = 0;

                        if (IsTotalPrice)
                        {
                            issdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            issdVM.SubTotal = Convert.ToDecimal((subTotatlIssue));
                        }
                        issdVM.CommentsD = Item.CommentsD;
                        issdVM.CreatedBy = Master.CreatedBy;
                        issdVM.CreatedOn = Master.CreatedOn;
                        issdVM.LastModifiedBy = Master.LastModifiedBy;
                        issdVM.LastModifiedOn = Master.LastModifiedOn;
                        issdVM.ReceiveNo = Master.SalesInvoiceNo;
                        issdVM.IssueDateTime = Master.InvoiceDateTime;
                        issdVM.SD = 0;
                        issdVM.SDAmount = 0;
                        issdVM.Wastage = 0;
                        issdVM.BOMDate = "1900/01/01";
                        issdVM.FinishItemNo = "0";
                        issdVM.transactionType = Master.TransactionType;
                        issdVM.IssueReturnId = Master.ReturnId;
                        issdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        issdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        issdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        issdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        issdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        issdVM.UOMn = (Item.UOMn);
                        issdVM.Post = Master.Post;
                        issdVM.BranchId = Master.BranchId;
                        retResults = issDal.IssueInsertToDetails(issdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }


                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + Master.SalesInvoiceNo + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateIssue.Transaction = Vtransaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                    }


                    #endregion Transaction is Trading

                    #region Transaction is TollIssue

                    if (Master.TransactionType == "TollIssue")
                    {
                        #region Issue Settings

                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);

                        decimal subTotA = AvgRate * Item.Quantity;

                        subTotA = FormatingNumeric(subTotA, IssuePlaceAmt);

                        #endregion Issue Settings

                        #region Insert to Issue

                        IssueDetailVM issdVM = new IssueDetailVM();
                        issdVM.IssueNo = Master.SalesInvoiceNo;
                        issdVM.IssueLineNo = Item.InvoiceLineNo;
                        issdVM.ItemNo = Item.ItemNo;
                        issdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        issdVM.NBRPrice = 0;
                        issdVM.CostPrice = Convert.ToDecimal((AvgRate));
                        issdVM.UOM = Item.UOM;
                        issdVM.VATRate = 0;
                        issdVM.VATAmount = 0;

                        if (IsTotalPrice)
                        {
                            issdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            issdVM.SubTotal = subTotA;
                        }

                        issdVM.CommentsD = Item.CommentsD;
                        issdVM.CreatedBy = Master.CreatedBy;
                        issdVM.CreatedOn = Master.CreatedOn;
                        issdVM.LastModifiedBy = Master.LastModifiedBy;
                        issdVM.LastModifiedOn = Master.LastModifiedOn;
                        issdVM.ReceiveNo = Master.SalesInvoiceNo;
                        issdVM.IssueDateTime = Master.InvoiceDateTime;
                        issdVM.SD = 0;
                        issdVM.SDAmount = 0;
                        issdVM.Wastage = 0;
                        issdVM.BOMDate = "1900/01/01";
                        issdVM.FinishItemNo = "0";
                        issdVM.transactionType = Master.TransactionType;
                        issdVM.IssueReturnId = Master.ReturnId;
                        issdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        issdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        issdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        issdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        issdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        issdVM.UOMn = Item.UOMn;
                        issdVM.Post = Master.Post;
                        issdVM.BranchId = Master.BranchId;

                        retResults = issDal.IssueInsertToDetails(issdVM, VcurrConn, Vtransaction);

                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }



                        #endregion Insert to Issue

                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + Master.SalesInvoiceNo + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateIssue.Transaction = Vtransaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                    }

                    #endregion Transaction is TollIssue

                    #region Transaction is InternalIssue

                    if (Master.TransactionType == "InternalIssue")
                    {
                        //ProductDAL productDal = new ProductDAL();

                        decimal NBRPrice = productDal.GetLastNBRPriceFromBOM_VatName(Item.ItemNo, "VAT 4.3 (Internal Issue)", Master.InvoiceDateTime, VcurrConn, Vtransaction);
                        #region Insert to Issue

                        #region Issue Settings
                        AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);
                        decimal subTotA = AvgRate * Item.Quantity;
                        subTotA = FormatingNumeric(subTotA, IssuePlaceAmt);

                        #endregion Issue Settings

                        IssueDetailVM issdVM = new IssueDetailVM();

                        issdVM.BOMId = Item.BOMId;
                        issdVM.IssueNo = Master.SalesInvoiceNo;
                        issdVM.IssueLineNo = Item.InvoiceLineNo;
                        issdVM.ItemNo = Item.ItemNo;
                        issdVM.Quantity = Item.Quantity;
                        issdVM.NBRPrice = 0;
                        issdVM.CostPrice = AvgRate;
                        issdVM.UOM = Item.UOM;
                        issdVM.VATRate = 0;
                        issdVM.VATAmount = 0;
                        if (IsTotalPrice)
                        {
                            issdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            issdVM.SubTotal = subTotA;
                        }
                        issdVM.CommentsD = Item.CommentsD;
                        issdVM.CreatedBy = Master.CreatedBy;
                        issdVM.CreatedOn = Master.CreatedOn;
                        issdVM.LastModifiedBy = Master.LastModifiedBy;
                        issdVM.LastModifiedOn = Master.LastModifiedOn;
                        issdVM.ReceiveNo = Master.SalesInvoiceNo;
                        issdVM.IssueDateTime = Master.InvoiceDateTime;
                        issdVM.SD = 0;
                        issdVM.SDAmount = 0;
                        issdVM.Wastage = 0;
                        issdVM.BOMDate = "1900/01/01";
                        issdVM.FinishItemNo = "0";
                        issdVM.transactionType = Master.TransactionType;
                        issdVM.IssueReturnId = Master.ReturnId;
                        issdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        issdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        issdVM.UOMQty = Item.UOMQty;
                        issdVM.UOMPrice = AvgRate;
                        issdVM.UOMc = Item.UOMc;
                        issdVM.UOMn = Item.UOMn;
                        issdVM.Post = Master.Post;
                        issdVM.BranchId = Master.BranchId;

                        retResults = issDal.IssueInsertToDetails(issdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }


                        #endregion Insert to Issue
                        #region Update Issue

                        sqlText = "";
                        sqlText += " update IssueHeaders set ";
                        sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                        sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                        sqlText += " where (IssueHeaders.IssueNo= '" + Master.SalesInvoiceNo + "')";

                        SqlCommand cmdUpdateIssue = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateIssue.Transaction = Vtransaction;
                        transResult = (int)cmdUpdateIssue.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveIssue);
                        }
                        #endregion Update Issue

                        #region Insert to Receive

                        #region Issue Settings


                        NBRPrice = FormatingNumeric(NBRPrice, RPlaceQty);


                        #endregion Issue Settings

                        decimal subTot = Item.NBRPrice * Item.Quantity;
                        subTot = FormatingNumeric(subTot, RPlaceAmt);

                        //var t = (NBRPrice + "*" + Item.Quantity);

                        ReceiveDetailVM rdVM = new ReceiveDetailVM();

                        rdVM.BOMId = Item.BOMId;
                        rdVM.ReceiveNo = Master.SalesInvoiceNo;
                        rdVM.ReceiveLineNo = Item.InvoiceLineNo;
                        rdVM.ItemNo = Item.ItemNo;
                        rdVM.Quantity = Convert.ToDecimal((Item.Quantity));
                        rdVM.CostPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.NBRPrice = Convert.ToDecimal((Item.NBRPrice));
                        rdVM.UOM = Item.UOM;
                        rdVM.VATRate = 0;
                        rdVM.VATAmount = 0;
                        if (IsTotalPrice)
                        {
                            rdVM.SubTotal = Convert.ToDecimal((Item.SubTotal));
                        }
                        else
                        {
                            rdVM.SubTotal = subTot;
                        }
                        rdVM.CommentsD = Item.CommentsD;
                        rdVM.CreatedBy = Master.CreatedBy;
                        rdVM.CreatedOn = Master.CreatedOn;
                        rdVM.LastModifiedBy = Master.LastModifiedBy;
                        rdVM.LastModifiedOn = Master.LastModifiedOn;
                        rdVM.SD = 0;
                        rdVM.SDAmount = 0;
                        rdVM.ReceiveDateTime = Master.InvoiceDateTime;
                        rdVM.transactionType = Master.TransactionType;
                        rdVM.ReturnId = Master.ReturnId;
                        rdVM.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                        rdVM.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                        rdVM.UOMQty = Convert.ToDecimal((Item.UOMQty));
                        rdVM.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                        rdVM.UOMc = Convert.ToDecimal((Item.UOMc));
                        rdVM.UOMn = Item.UOMn;
                        rdVM.Post = Master.Post;
                        rdVM.BranchId = Master.BranchId;
                        retResults = recDal.ReceiveInsertToDetail(rdVM, VcurrConn, Vtransaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                        }


                        #endregion Insert to Receive
                        #region Update Receive

                        sqlText = "";


                        sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                        sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                        sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                        sqlText += " where ReceiveHeaders.ReceiveNo='" + Master.SalesInvoiceNo + "' ";


                        SqlCommand cmdUpdateReceive = new SqlCommand(sqlText, VcurrConn);
                        cmdUpdateReceive.Transaction = Vtransaction;
                        transResult = (int)cmdUpdateReceive.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableToSaveReceive);
                        }
                        #endregion Update Receive
                    }

                    #endregion Transaction is InternalIssue


                    #endregion Insert Issue and Receive if Transaction is not Other

                    #region Insert only DetailTable

                    AvgRate = FormatingNumeric(AvgRate, IssuePlaceAmt);

                    Item.SalesInvoiceNo = Master.SalesInvoiceNo;
                    Item.Quantity = Convert.ToDecimal((Item.Quantity));
                    Item.PromotionalQuantity = Item.PromotionalQuantity;
                    Item.SalesPrice = Convert.ToDecimal((Item.SalesPrice));
                    Item.NBRPrice = Convert.ToDecimal((Item.NBRPrice));
                    Item.AvgRate = Convert.ToDecimal((AvgRate));
                    Item.VATRate = Convert.ToDecimal((Item.VATRate));
                    Item.VATAmount = Convert.ToDecimal((Item.VATAmount));
                    Item.SubTotal = Convert.ToDecimal((Item.SubTotal));
                    Item.CreatedBy = Master.CreatedBy;
                    Item.CreatedOn = Master.CreatedOn;
                    Item.LastModifiedBy = Master.LastModifiedBy;
                    Item.LastModifiedOn = Master.LastModifiedOn;
                    Item.SD = Convert.ToDecimal((Item.SD));
                    Item.SDAmount = Convert.ToDecimal((Item.SDAmount));
                    Item.VDSAmountD = Convert.ToDecimal((Item.VDSAmountD));

                    Item.InvoiceDateTime = Master.InvoiceDateTime;
                    Item.TransactionType = Master.TransactionType;
                    Item.ReturnId = Master.ReturnId;
                    Item.Post = Master.Post;
                    Item.UOMQty = Convert.ToDecimal((Item.UOMQty));
                    Item.UOMc = Convert.ToDecimal((Item.UOMc));
                    Item.DiscountAmount = Convert.ToDecimal((Item.DiscountAmount));
                    Item.DiscountedNBRPrice = Convert.ToDecimal((Item.DiscountedNBRPrice));
                    Item.DollerValue = Convert.ToDecimal((Item.DollerValue));
                    Item.CurrencyValue = Convert.ToDecimal((Item.CurrencyValue));
                    Item.UOMPrice = Convert.ToDecimal((Item.UOMPrice));
                    Item.BranchId = Master.BranchId;
                    Item.ProductDescription = Item.ProductDescription;
                    Item.IsFixedVAT = Item.IsFixedVAT;
                    Item.FixedVATAmount = Convert.ToDecimal((Item.FixedVATAmount));

                    #region New Field DN/CN
                    Item.PreviousInvoiceDateTime = (Item.PreviousInvoiceDateTime);
                    Item.PreviousNBRPrice = Convert.ToDecimal((Item.PreviousNBRPrice));
                    Item.PreviousQuantity = Convert.ToDecimal((Item.PreviousQuantity));
                    Item.PreviousUOM = (Item.PreviousUOM);
                    Item.PreviousSubTotal = Convert.ToDecimal((Item.PreviousSubTotal));
                    Item.PreviousVATAmount = Convert.ToDecimal((Item.PreviousVATAmount));
                    Item.PreviousVATRate = Convert.ToDecimal((Item.PreviousVATRate));
                    Item.PreviousSD = Convert.ToDecimal((Item.PreviousSD));
                    Item.PreviousSDAmount = Convert.ToDecimal((Item.PreviousSDAmount));
                    Item.ReasonOfReturn = (Item.ReasonOfReturn);

                    #endregion

                    #region
                    Item.HPSRate = Item.HPSRate;
                    #endregion

                    detailVms.Add(Item);

                    retResults = SalesInsertToDetail(Item, VcurrConn, Vtransaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                    }

                    #endregion Insert only DetailTable
                }



                #region Bulk Insert

                //string listJson = JsonConvert.SerializeObject(detailVms);
                //DataTable detailsTable = JsonConvert.DeserializeObject<DataTable>(listJson);

                //detailsTable.Columns["VatName"].ColumnName = "VATName";
                //detailsTable.Columns["NonStockD"].ColumnName = "NonStock";
                //detailsTable.Columns["TradingD"].ColumnName = "Trading";
                //detailsTable.Columns["PreviousSalesInvoiceNoD"].ColumnName = "PreviousSalesInvoiceNo";
                //detailsTable.Columns["SaleTypeD"].ColumnName = "SaleType";
                //detailsTable.Columns["VDSAmountD"].ColumnName = "VDSAmount";
                //detailsTable.Columns["CommentsD"].ColumnName = "Comments";
                //detailsTable.Columns["ReturnId"].ColumnName = "SaleReturnId"; //SaleReturnId
                //detailsTable.Columns["AvgRate"].ColumnName = "AVGPrice"; //SaleReturnId

                //detailsTable.Columns.Remove("BOMReferenceNo");
                //detailsTable.Columns.Remove("ProductCode");
                //detailsTable.Columns.Remove("ProductName");
                //detailsTable.Columns.Remove("BDTValue");
                //detailsTable.Columns.Remove("Total");

                ////  SearchSaleDetailDTNew("")

                //retResults = commonDal.BulkInsert("SalesInvoiceDetails", detailsTable, currConn, transaction);

                //if (retResults[0] != "Success")
                //{
                //    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                //}


                #endregion


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region Insert into Export
                if (Master.TransactionType == "Export"
                    || Master.TransactionType == "ExportTender"
                    || Master.TransactionType == "ExportTrading"
                    || Master.TransactionType == "ExportServiceNS"
                    || Master.TransactionType == "ExportService"
                    || Master.TransactionType == "ExportTradingTender"
                    || Master.TransactionType == "ExportPackage"

                    )
                {
                    #region Validation for Export

                    if (ExportDetails.Count() < 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgNoDataToSave);
                    }

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "delete from SalesInvoiceHeadersExport where SalesInvoiceNo='" + Master.SalesInvoiceNo + "'";


                    SqlCommand cmdFindId1 = new SqlCommand(sqlText, VcurrConn);
                    cmdFindId1.Transaction = Vtransaction;
                    cmdFindId1.ExecuteNonQuery();

                    #endregion Find Transaction Exist

                    #endregion Validation for Export

                    foreach (var ItemExport in ExportDetails.ToList())
                    {
                        #region Insert only SalesInvoiceHeadersExport



                        ////NeedChange
                        sqlText = "";
                        sqlText += " insert into SalesInvoiceHeadersExport(";
                        sqlText += " SalesInvoiceNo,";
                        sqlText += " SaleLineNo,";
                        sqlText += " RefNo,";
                        sqlText += " Description,";
                        sqlText += " Quantity,";
                        sqlText += " GrossWeight,";
                        sqlText += " NetWeight,";
                        sqlText += " NumberFrom,";
                        sqlText += " NumberTo,";
                        sqlText += " Comments,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "'" + Master.SalesInvoiceNo + "',";
                        sqlText += "@ItemExportSaleLineNo ,";
                        sqlText += "@ItemExportRefNo ,";
                        sqlText += "@ItemExportDescription ,";
                        sqlText += "@ItemExportQuantityE ,";
                        sqlText += "@ItemExportGrossWeight ,";
                        sqlText += "@ItemExportNetWeight ,";
                        sqlText += "@ItemExportNumberFrom ,";
                        sqlText += "@ItemExportNumberTo ,";
                        sqlText += "@ItemExportCommentsE ,";
                        sqlText += "@MasterCreatedBy ,";
                        sqlText += "@MasterCreatedOn ,";
                        sqlText += "@MasterLastModifiedBy ,";
                        sqlText += "@MasterLastModifiedOn";

                        //sqlText += "'" + Master.@Post + "'";
                        sqlText += ")	";


                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, VcurrConn);
                        cmdInsDetail.Transaction = Vtransaction;

                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportSaleLineNo", ItemExport.SaleLineNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportRefNo", ItemExport.RefNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportDescription", ItemExport.Description);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportQuantityE", ItemExport.QuantityE);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportGrossWeight", ItemExport.GrossWeight);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportNetWeight", ItemExport.NetWeight);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportNumberFrom", ItemExport.NumberFrom);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportNumberTo", ItemExport.NumberTo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemExportCommentsE", ItemExport.CommentsE);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert,
                                                            MessageVM.saleMsgSaveNotSuccessfully);
                        }



                        #endregion Insert only DetailTable
                    }
                }

                #endregion Insert into Export

                #region TrackingWithSale
                bool TrackingWithSale = Convert.ToBoolean(commonDal.settingsCache("Purchase", "TrackingWithSale", settings) == "Y" ? true : false);
                bool TrackingWithSaleFIFO = Convert.ToBoolean(commonDal.settingsCache("Purchase", "TrackingWithSaleFIFO", settings) == "Y" ? true : false);
                int NumberOfItems = Convert.ToInt32(commonDal.settingsCache("Sale", "NumberOfItems", settings));
                if (TrackingWithSale)
                {



                    foreach (var Item in Details.ToList())
                    {
                        DataTable tracDt = new DataTable();
                        sqlText = "";
                        sqlText += @" select top " + Convert.ToInt32(Item.Quantity) + " * from PurchaseSaleTrackings";
                        sqlText += @" where IsSold=0";
                        sqlText += @" and ItemNo='" + Item.ItemNo + "'";
                        if (TrackingWithSaleFIFO)
                        {
                            sqlText += @" order by id asc ";
                        }
                        else
                        {
                            sqlText += @" order by id desc ";
                        }
                        SqlCommand cmdRIFB1 = new SqlCommand(sqlText, VcurrConn);
                        cmdRIFB1.Transaction = Vtransaction;
                        SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                        reportDataAdapt1.Fill(tracDt);
                        if (Item.Quantity > tracDt.Rows.Count)
                        {
                            throw new ArgumentNullException("Stock not available", "Stock not available");
                        }
                        foreach (DataRow itemTrac in tracDt.Rows)
                        {
                            sqlText = "";
                            sqlText += " update PurchaseSaleTrackings set  ";
                            sqlText += " IsSold= '1' ,";
                            sqlText += " TradeVATableValue=@TradeVATableValue,";// '" + Item.TradeVATableValue / Item.Quantity + "' ,";
                            sqlText += " TradeVATAmount=@TradeVATAmount,";// '" + Item.TradeVATAmount / Item.Quantity + "' ,";
                            sqlText += " TradeVATRate= @ItemTradeVATRate,";
                            sqlText += " TotalValue=@TotalValue ,";

                            sqlText += " SalesInvoiceNo= '" + Master.SalesInvoiceNo + "' ,";
                            sqlText += " SaleInvoiceDateTime=@MasterInvoiceDateTime ";
                            sqlText += " where  Id= '" + itemTrac["Id"] + "' ";

                            SqlCommand cmdUpdate = new SqlCommand(sqlText, VcurrConn);
                            cmdUpdate.Transaction = Vtransaction;

                            cmdUpdate.Parameters.AddWithValue("@MasterInvoiceDateTime", Master.InvoiceDateTime);
                            cmdUpdate.Parameters.AddWithValue("@TradeVATableValue", Item.TradeVATableValue / Item.Quantity);
                            cmdUpdate.Parameters.AddWithValue("@TradeVATAmount", Item.TradeVATAmount / Item.Quantity);
                            cmdUpdate.Parameters.AddWithValue("@ItemTradeVATRate", Item.TradeVATRate);
                            cmdUpdate.Parameters.AddWithValue("@TotalValue", Convert.ToDecimal((Item.TradeVATableValue / Item.Quantity) + (Item.TradeVATAmount / Item.Quantity)));
                            transResult = (int)cmdUpdate.ExecuteNonQuery();
                            if (transResult <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                            }
                        }
                    }
                    ReportDSDAL rds = new ReportDSDAL();
                    DataSet ds = new DataSet();
                    ds = rds.VAT11ReportCommercialImporterNew(Master.SalesInvoiceNo, "N", "N", VcurrConn, Vtransaction);
                    if (ds.Tables[0].Rows.Count > NumberOfItems)
                    {
                        //throw new ArgumentNullException("Number of Items in a Invoice exist the Page Limit", "Number of Items in a Invoice exist the Page Limit");
                    }

                }
                #endregion TrackingWithSale

                #region Commit


                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Master.SalesInvoiceNo;
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

            }
            finally
            {


            }
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }

        public string[] SalesUpdate(SaleMasterVM Master, List<SaleDetailVm> Details, List<SaleExportVM> ExportDetails, List<TrackingVM> Trackings, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            bool PriceDeclarationTradingProduct = false;
            string vProductType = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string PostStatus = "";

            string vehicleId = "0";
            ProductDAL productDal = new ProductDAL();
            ReceiveDAL rDAL = new ReceiveDAL();
            IssueDAL issDAL = new IssueDAL();


            #endregion Initializ

            #region Try
            try
            {

                #region Find Month Lock

                string PeriodName = Convert.ToDateTime(Master.InvoiceDateTime).ToString("MMMM-yyyy");
                string[] vValues = { PeriodName };
                string[] vFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

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
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.InvoiceDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.InvoiceDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameUpdate);

                CommonDAL commonDal = new CommonDAL();
                int IssuePlaceQty = Convert.ToInt32(commonDal.settings("Issue", "Quantity"));
                int IssuePlaceAmt = Convert.ToInt32(commonDal.settings("Issue", "Amount"));
                int ReceivePlaceAmt = Convert.ToInt32(commonDal.settings("Receive", "Amount"));


                #endregion open connection and transaction

                #region Current Status

                #region Post Status

                string currentPostStatus = "N";

                sqlText = "";
                sqlText = @"
----------declare @InvoiceNo as varchar(100)='INV-0001/0719'

select top 1 SalesInvoiceNo, Post from SalesInvoiceDetails
where 1=1 
and SalesInvoiceNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.SalesInvoiceNo);

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
----------declare @InvoiceNo as varchar(100)='INV-0001/0719'

select ItemNo, SalesInvoiceNo from SalesInvoiceDetails
where 1=1 
and SalesInvoiceNo=@InvoiceNo

";

                    cmd = new SqlCommand(sqlText, currConn, transaction);
                    cmd.Parameters.AddWithValue("@InvoiceNo", Master.SalesInvoiceNo);

                    da = new SqlDataAdapter(cmd);
                    da.Fill(dtCurrentItems);

                }
                #endregion

                #endregion

                //PriceDeclarationTradingProduct = Convert.ToBoolean(commonDal.settingValue("PriceDeclaration", "TradingProduct") == "Y" ? true : false);

                #region Fiscal Year Check

                string transactionDate = Master.InvoiceDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MM-dd");
                bool IsTotalPrice = Convert.ToBoolean(commonDal.settings("Sale", "TotalPrice").ToString().ToLower() == "y" ? true : false);
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
                sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUnableFindExistID);
                }

                #endregion Find ID for Update

                #region VehicleId
                sqlText = "";
                sqlText = sqlText + "select COUNT(VehicleNo) from Vehicles WHERE VehicleNo=@MasterVehicleNo ";
                SqlCommand cmdExistVehicleId = new SqlCommand(sqlText, currConn);
                cmdExistVehicleId.Transaction = transaction;
                cmdExistVehicleId.Parameters.AddWithValue("@MasterVehicleNo", Master.VehicleNo);
                IDExist = (int)cmdExistVehicleId.ExecuteScalar();

                if (IDExist <= 0)
                {

                    sqlText = "";
                    sqlText +=
                        " INSERT INTO Vehicles (VehicleID,	VehicleType,	VehicleNo,	Description,	Comments,	ActiveStatus,CreatedBy,	CreatedOn,	LastModifiedBy,	LastModifiedOn)";
                    sqlText += " select MAX(isnull(VehicleID,0)+1) ,";
                    sqlText += " @MasterVehicleType,	";
                    sqlText += " @MasterVehicleNo,";
                    sqlText += " 'NA',";
                    sqlText += " 'NA',	";
                    if (Master.vehicleSaveInDB == true)
                    {
                        sqlText += " 'Y',";

                    }
                    else
                    {
                        sqlText += " 'N',";
                    }
                    sqlText += "@MasterCreatedBy,";
                    sqlText += "@MasterCreatedOn,";
                    sqlText += "@MasterLastModifiedBy,";
                    sqlText += "@MasterLastModifiedOn ";
                    sqlText += " from Vehicles;";

                    SqlCommand cmdExistVehicleIns = new SqlCommand(sqlText, currConn);
                    cmdExistVehicleIns.Transaction = transaction;

                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleType", Master.VehicleType);
                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);
                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", OrdinaryVATDesktop.DateToDate(Master.CreatedOn));
                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdExistVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));

                    transResult = (int)cmdExistVehicleIns.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                    }
                }
                else
                {
                    sqlText = "";
                    sqlText += " Update Vehicles SET VehicleType= @MasterVehicleType ";
                    sqlText += " where VehicleNo= @MasterVehicleNo";

                    SqlCommand cmdUpdateVehicleIns = new SqlCommand(sqlText, currConn);
                    cmdUpdateVehicleIns.Transaction = transaction;
                    cmdUpdateVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleType", Master.VehicleType);
                    cmdUpdateVehicleIns.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);

                    transResult = (int)cmdUpdateVehicleIns.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                    }
                }

                sqlText = "";
                sqlText += "select	VehicleID FROM Vehicles where VehicleNo= @MasterVehicleNo; ";

                SqlCommand cmdPrefetch = new SqlCommand(sqlText, currConn);
                cmdPrefetch.Transaction = transaction;
                cmdPrefetch.Parameters.AddWithValueAndNullHandle("@MasterVehicleNo", Master.VehicleNo);

                vehicleId = Convert.ToString(cmdPrefetch.ExecuteScalar());
                if (string.IsNullOrEmpty(vehicleId))
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgUnableCreatID);
                }


                #endregion VehicleId

                #region ID check completed,update Information in Header

                #region update Header
                string[] cFields = new string[] { "sih.SalesInvoiceNo" };
                string[] cVals = new string[] { Master.SalesInvoiceNo };
                SaleMasterVM smVm = SelectAllList(0, cFields, cVals, currConn, transaction, null, null).FirstOrDefault();

                if (smVm.Post == "Y")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.ThisTransactionWasPosted);
                }
                
                smVm.DeductionAmount = Master.DeductionAmount;

                smVm.ShiftId = Master.ShiftId;
                smVm.CustomerID = Master.CustomerID;
                smVm.DeliveryAddress1 = Master.DeliveryAddress1;
                smVm.DeliveryAddress2 = Master.DeliveryAddress2;
                smVm.DeliveryAddress3 = Master.DeliveryAddress3;
                smVm.VehicleID = vehicleId;
                smVm.InvoiceDateTime = Master.InvoiceDateTime;
                smVm.TotalAmount = Master.TotalAmount;
                smVm.TotalVATAmount = Master.TotalVATAmount;
                smVm.VDSAmount = Master.VDSAmount;

                smVm.SerialNo = Master.SerialNo;
                smVm.Comments = Master.Comments;
                smVm.DeliveryDate = Master.DeliveryDate;
                smVm.LastModifiedBy = Master.LastModifiedBy;
                smVm.LastModifiedOn = Master.LastModifiedOn;
                smVm.SaleType = Master.SaleType;
                smVm.PreviousSalesInvoiceNo = Master.PreviousSalesInvoiceNo;
                smVm.Trading = Master.Trading;
                smVm.IsPrint = Master.IsPrint;
                smVm.TenderId = Master.TenderId;
                smVm.TransactionType = Master.TransactionType;
                smVm.ReturnId = Master.ReturnId;
                smVm.CurrencyID = Master.CurrencyID;
                smVm.CurrencyRateFromBDT = Master.CurrencyRateFromBDT;
                smVm.ImportIDExcel = Master.ImportIDExcel;
                smVm.LCDate = Master.LCDate;
                smVm.LCBank = Master.LCBank;
                smVm.LCNumber = Master.LCNumber;
                smVm.PINo = Master.PINo;
                smVm.PIDate = Master.PIDate;
                smVm.EXPFormNo = Master.EXPFormNo;
                smVm.EXPFormDate = Master.EXPFormDate;
                smVm.Post = Master.Post;
                smVm.IsDeemedExport = Master.IsDeemedExport;
                smVm.BranchId = Master.BranchId;
                smVm.DeductionAmount = Master.DeductionAmount;
                smVm.SignatoryName = Master.SignatoryName;
                smVm.SignatoryDesig = Master.SignatoryDesig;
                smVm.HPSTotalAmount = Master.HPSTotalAmount;

                retResults = SalesUpdateToMaster(smVm, currConn, transaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, retResults[1]);
                }

                #endregion update Header



                #endregion ID check completed,update Information in Header

                #region Update into Details(Update complete in Header)

                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgNoDataToUpdate);
                }

                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM ReceiveDetails WHERE ReceiveNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM ReceiveHeaders WHERE ReceiveNo=@MasterSalesInvoiceNo ";

                sqlText += @" delete FROM IssueDetails WHERE IssueNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM IssueHeaders WHERE IssueNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM IssueDetailBOMs WHERE IssueNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM IssueHeaderBOMs WHERE IssueNo=@MasterSalesInvoiceNo ";

                sqlText += @" delete FROM PurchaseInvoiceDuties WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM PurchaseInvoiceDetails WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM PurchaseInvoiceHeaders WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM PurchaseSaleTrackings WHERE PurchaseInvoiceNo=@MasterSalesInvoiceNo ";


                sqlText += @" delete FROM SalesInvoiceHeadersExport WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                sqlText += @" delete FROM SalesInvoiceDetails WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";


                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                #region Set BOMId


                SetBOMId(Master, Details, currConn, transaction);

                #endregion

                Master.CreatedBy = smVm.CreatedBy;
                Master.CreatedOn = smVm.CreatedOn;

                retResults = SalesInsert2(Master, Details, ExportDetails, Trackings, transaction, currConn,null,UserId);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, retResults[1]);
                }

                #endregion Validation for Detail

                #endregion  Update into Details(Update complete in Header)


                #region Update PeriodId

                sqlText = "";
                sqlText += @"

UPDATE SalesInvoiceHeaders 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo


UPDATE SalesInvoiceDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
WHERE SalesInvoiceNo = @SalesInvoiceNo

";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
                cmdUpdate.Parameters.AddWithValue("@SalesInvoiceNo", Master.SalesInvoiceNo);

                transResult = cmdUpdate.ExecuteNonQuery();

                #endregion

                #region Update Product Stock

                if (currentPostStatus == "Y" && dtCurrentItems != null && dtCurrentItems.Rows.Count > 0)
                {

                    ResultVM rVM = new ResultVM();

                    ParameterVM paramVM = new ParameterVM();
                    paramVM.BranchId = Master.BranchId;
                    paramVM.InvoiceNo = Master.SalesInvoiceNo;
                    paramVM.dt = dtCurrentItems;

                    paramVM.IDs = new List<string>();

                    foreach (DataRow dr in paramVM.dt.Rows)
                    {
                        paramVM.IDs.Add(dr["ItemNo"].ToString());
                    }

                    if (paramVM.IDs.Count > 0)
                    {
                        rVM = _ProductDAL.Product_Stock_Update(paramVM, currConn, transaction,connVM,UserId);
                    }

                }

                #endregion

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                    ////if (transResult > 0)
                    ////{

                    ////}

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = Master.SalesInvoiceNo;
                retResults[3] = "N";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //////catch (SqlException sqlex)
            //////{
            //////    transaction.Rollback();
            //////    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //////    //throw sqlex;
            //////}
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
                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        //currConn to VcurrConn 25-Aug-2020
        public string[] SalesPost(SaleMasterVM Master, List<SaleDetailVm> Details, List<TrackingVM> Trackings
            , SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null, bool BulkPost = false)
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
            int transResult = 0;
            string sqlText = "";
            ReceiveDAL recDal = new ReceiveDAL();
            IssueDAL issDal = new IssueDAL();




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

select top 1 SalesInvoiceNo, TransactionType, Post from SalesInvoiceDetails
where 1=1 
and SalesInvoiceNo=@InvoiceNo

";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", Master.SalesInvoiceNo);

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
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, vFields, vValues).FirstOrDefault();

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
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                string postWithDate = commonDal.settings("Integration", "PostWithDateTime");
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
                    ////if (vcurrConn == null)
                    ////{
                    ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                    ////    if (VcurrConn.State != ConnectionState.Open)
                    ////    {
                    ////        VcurrConn.Open();
                    ////    }
                    ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.saleMsgMethodNamePost);

                    ////}

                    #region comment before 28 Oct 2020

                    //////currConn = _dbsqlConnection.GetConnection(connVM);
                    //////if (currConn.State != ConnectionState.Open)
                    //////{
                    //////    currConn.Open();
                    //////}

                    //////transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNamePost);

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
                    sqlText = sqlText + "select COUNT(SalesInvoiceNo) from SalesInvoiceHeaders WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                    cmdFindIdUpd.Transaction = transaction;

                    cmdFindIdUpd.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);

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

                    Details = SelectSaleDetail(Master.SalesInvoiceNo, null, null, currConn, transaction);

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
                    else
                    {
                        DataTable dtAvgPrice = new DataTable();
                        foreach (var Item in Details)
                        {
                            StockQuantity = 0;
                            dtAvgPrice = productDal.AvgPriceNew(Item.ItemNo.Trim(),
                                                                 Convert.ToDateTime(Master.InvoiceDateTime).ToString("yyyy-MMM-dd") +
                                                                DateTime.Now.ToString(" HH:mm:00"), currConn, transaction, true, true, true, false, null);
                            StockQuantity = Convert.ToDecimal(dtAvgPrice.Rows[0]["Quantity"].ToString());

                            if (Item.UOMQty > StockQuantity)
                            {
                                throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, "(" + Item.ProductCode + ") " + Item.ProductName + Environment.NewLine + MessageVM.saleMsgStockNotAvailablePost);

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
                    sqlText += @" Update  SalesInvoiceHeaders        set  SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig, Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                    sqlText += @" Update  SalesInvoiceDetails        set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";


                    if (postWithDate == "Y")
                    {
                        sqlText += @" Update  SalesInvoiceHeaders        set  InvoiceDateTime = @InvoiceDateTime   WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                        sqlText += @" Update  SalesInvoiceDetails        set  InvoiceDateTime = @InvoiceDateTime    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";


                    }

                    SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                    cmdDeleteDetail.Transaction = transaction;
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(Master.LastModifiedOn));
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SignatoryDesig", Master.SignatoryDesig);
                    cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@SignatoryName", Master.SignatoryName);

                    if (postWithDate == "Y")
                    {
                        string dateTime = commonDal.ServerDateWithTime();
                        cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", dateTime);
                    }

                    transResult = cmdDeleteDetail.ExecuteNonQuery();


                    #endregion

                    #region Tracking
                    if (Trackings != null && Trackings.Count > 0)
                    {
                        for (int i = 0; i < Trackings.Count; i++)
                        {
                            if (Trackings[i].SaleInvoiceNo == Master.SalesInvoiceNo)
                            {
                                sqlText = "";
                                //NeedChange
                                sqlText += " update Trackings set  ";
                                sqlText += " LastModifiedBy =@MasterLastModifiedBy, ";
                                sqlText += " LastModifiedOn =@MasterLastModifiedOn, ";
                                sqlText += " SalePost = @MasterPost";
                                sqlText += " where  SaleInvoiceNo =@MasterSalesInvoiceNo  ";
                                sqlText += " and  Heading1 =@Heading1"; //'" + Trackings[i].Heading1 + "' ";

                                SqlCommand cmdUpdateTracking = new SqlCommand(sqlText, currConn);
                                cmdUpdateTracking.Transaction = transaction;
                                cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy);
                                cmdUpdateTracking.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                                cmdUpdateTracking.Parameters.AddWithValue("@MasterPost", Master.Post);
                                cmdUpdateTracking.Parameters.AddWithValue("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
                                cmdUpdateTracking.Parameters.AddWithValue("@Heading1", Trackings[i].Heading1);
                                transResult = (int)cmdUpdateTracking.ExecuteNonQuery();
                                if (transResult <= 0)
                                {
                                    throw new ArgumentNullException(MessageVM.saleMsgMethodNamePost, MessageVM.saleMsgPostNotSuccessfully);
                                }

                            }

                        }
                    }


                    #endregion

                }


                #region Sale_Product_IN_OUT

                if (TransactionType.ToLower() != "ServiceNS")
                {
                    ResultVM rVM = new ResultVM();

                    ParameterVM paramVM = new ParameterVM();
                    paramVM.InvoiceNo = Master.SalesInvoiceNo;

                    rVM = Sale_Product_IN_OUT(paramVM, currConn, transaction);
                }

                #endregion


                #region Update ACI Middleware DB

                string code = commonDal.settings("CompanyCode", "Code", currConn, transaction);

                if (OrdinaryVATDesktop.IsACICompany(code))
                {
                    sqlText = @"update ACIData.dbo.SaleInvoices set Post = 'Y'
where InvoiceNo = @InvoiceNo";

                    SqlCommand cmdACI = new SqlCommand(sqlText, currConn, transaction);

                    cmdACI.Parameters.AddWithValue("@InvoiceNo", Master.SalesInvoiceNo);

                    cmdACI.ExecuteNonQuery();
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

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                ////throw ex;
            }

            finally
            {

                #region comment 28 Oct 2020

                ////if (vcurrConn == null)
                ////{
                ////    if (VcurrConn != null)
                ////    {
                ////        if (VcurrConn.State == ConnectionState.Open)
                ////        {
                ////            VcurrConn.Close();
                ////        }
                ////    }
                ////}

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


        public ResultVM Sale_Product_IN_OUT(ParameterVM paramVM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
----------declare @InvoiceNo as varchar(100)='INV-0001/0719'

select SalesInvoiceNo, TransactionType, BranchId, Post from SalesInvoiceHeaders
where 1=1 
and SalesInvoiceNo=@InvoiceNo

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
----------declare @InvoiceNo as varchar(100)='INV-0001/0719'
declare @MultiplicationFactor as int=1


select 
@MultiplicationFactor = case 
when TransactionType in
(
'Credit','RawCredit'
) then  1 
when TransactionType in
(
'Other'
,'RawSale'
,'Debit','PackageSale','Wastage','SaleWastage','CommercialImporter','ServiceNS','Export','ExportServiceNS'
,'ExportTender','Tender','ExportPackage','Trading','ExportTrading','TradingTender','ExportTradingTender'
,'InternalIssue','Service','ExportService'
) then  -1 
end

from SalesInvoiceDetails
where 1=1 
and SalesInvoiceNo=@InvoiceNo


select ItemNo, SalesInvoiceNo, TransactionType, UOMQty * (@MultiplicationFactor) as Quantity, Post from SalesInvoiceDetails
where 1=1 
and SalesInvoiceNo=@InvoiceNo

";
                #endregion


                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.InvoiceNo);

                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                paramVM.dt = dt;

                rVM = _ProductDAL.Product_IN_OUT(paramVM, currConn, transaction);

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




        //currConn to VcurrConn 25-Aug-2020
        private void SetBOMId(SaleMasterVM MasterVM, List<SaleDetailVm> DetailVMs, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            string CustomerWiseBOM = new CommonDAL().settings("Sale", "CustomerWiseBOM");
            ProductDAL _ProductDAL = new ProductDAL();

            string CustomerID = MasterVM.CustomerID;

            string InvoiceDateTime = Convert.ToDateTime(MasterVM.InvoiceDateTime).ToString("yyyy-MMM-dd");

            if (CustomerWiseBOM == "N")
            {
                CustomerID = "0";
            }

            foreach (SaleDetailVm dVM in DetailVMs)
            {
                dVM.BOMId = _ProductDAL.GetBOMId_ByReferenceNo(dVM.ItemNo, dVM.VatName, InvoiceDateTime, CustomerID, dVM.BOMReferenceNo, VcurrConn, Vtransaction);
            }


        }


        private int UpdatIssueAvgPrice(string invoiceNo, string itemNo, decimal avgPrice, SysDBInfoVMTemp connVM = null)
        {
            #region Variables


            int transResult = 0;
            string sqlText = "";
            decimal retResults = 0;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
            try
            {
                currConn = _dbsqlConnection.GetConnectionNoTimeOut();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Last UseQuantity

                sqlText = "  ";
                sqlText += " SELECT SubTotal FROM IssueDetails ";
                sqlText += " where  IssueNo =@invoiceNo ";
                sqlText += " and ItemNo = @itemNo";
                SqlCommand cmdGetLastNBRPriceFromBOM = new SqlCommand(sqlText, currConn);
                cmdGetLastNBRPriceFromBOM.Transaction = transaction;
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@invoiceNo", invoiceNo);
                cmdGetLastNBRPriceFromBOM.Parameters.AddWithValue("@itemNo", itemNo);
                if (cmdGetLastNBRPriceFromBOM.ExecuteScalar() == null)
                {

                }
                else
                {
                    retResults = (decimal)cmdGetLastNBRPriceFromBOM.ExecuteScalar();
                }
                #endregion Last UseQuantity
                CommonDAL commonDal = new CommonDAL();
                var tt = retResults;
                var tt1 = avgPrice;
                var tt2 = commonDal.decimal259(avgPrice);


                var tt4 = "";
                #region Update only DetailTable

                sqlText = "";

                sqlText += " update IssueDetails set ";
                sqlText += " UOMPrice=@avgPrice,";//'" + avgPrice + "' ,";
                sqlText += " SubTotal=@avgPrice'" + " * UOMQty ,";
                sqlText += " NBRPrice=@avgPrice'" + " * UOMc ,";
                sqlText += " CostPrice=@avgPrice'" + " * UOMc ,";
                sqlText += " IsProcess='Y'";

                sqlText += " where  IssueNo =@invoiceNo ";
                sqlText += " and ItemNo =@itemNo";

                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@avgPrice", avgPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@invoiceNo", invoiceNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("Update Avg Price", "Update Avg Price error");

                }
                #endregion Update only DetailTable
            }


            #endregion

            #region Catch & Finally

            //catch (SqlException sqlex)
            //{
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {

                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            return transResult;

            #endregion

        }

        public decimal ReturnSaleQty(string saleReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
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

                sqlText = "select Sum(isnull(SalesInvoiceDetails.Quantity,0)) from SalesInvoiceDetails ";
                sqlText += " where ItemNo = @itemNo and SaleReturnId = @saleReturnId";
                sqlText += " group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);
                cmd.Parameters.AddWithValue("@saleReturnId", saleReturnId);

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
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //throw sqlex;
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public string[] CurrencyInfo(string salesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "";//currency major
            retResults[2] = "";//currency minor
            retResults[3] = "";//currency symbol


            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";

            #endregion Initializ

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameUpdate);

                #endregion open connection and transaction

                #region currency

                sqlText = "";
                sqlText +=
                    " select c.CurrencyMajor,c.CurrencyMinor,c.CurrencySymbol from Currencies c,SalesInvoiceHeaders s ";
                sqlText += " where c.CurrencyId = s.CurrencyID ";
                sqlText += " and s.SalesInvoiceNo =@salesInvoiceNo ";
                sqlText += " group by c.CurrencyMajor,c.CurrencyMinor,c.CurrencySymbol ";

                DataTable dt = new DataTable("CurrencyData");
                SqlCommand cmdCurrency = new SqlCommand(sqlText, currConn);
                cmdCurrency.Transaction = transaction;
                //if (!salesInvoiceNo.Contains("'"))
                //{
                //    salesInvoiceNo = "'" + salesInvoiceNo + "'";
                //}

                cmdCurrency.Parameters.AddWithValue("@salesInvoiceNo", salesInvoiceNo);

                SqlDataAdapter adpt = new SqlDataAdapter(cmdCurrency);
                adpt.Fill(dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    retResults[0] = "Success";
                    retResults[1] = dt.Rows[0]["CurrencyMajor"].ToString(); //currency major
                    retResults[2] = dt.Rows[0]["CurrencyMinor"].ToString(); //currency minor
                    retResults[3] = dt.Rows[0]["CurrencySymbol"].ToString(); //currency symbol

                }

                #endregion currency

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit
            }
            #region Catch and Finall

            catch (SqlException sqlex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string GetCategoryName(string itemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string retResults = "";
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

                #region Return CategoryName

                sqlText = "  ";

                sqlText = " select pc.CategoryName from Products p inner join ProductCategories pc on p.CategoryID = pc.CategoryID ";
                sqlText += " where ItemNo = @itemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);
                if (cmd.ExecuteScalar() == null)
                {
                    retResults = "";
                }
                else
                {
                    retResults = (string)cmd.ExecuteScalar();
                }

                #endregion Return CategoryName

            }

            #endregion try

            #region Catch and Finall
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

            #region Results

            return retResults;

            #endregion
        }

        #endregion

        #region Integration Methods

        public ResultVM Multiple_SalePost(SaleMasterVM paramVM, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
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

                #region Post
                CommonDAL commonDal = new CommonDAL();

                UserInformationDAL dal = new UserInformationDAL();

                UserInformationVM user = dal.SelectAll(Convert.ToInt32(UserInfoVM.UserId)).FirstOrDefault();

                string vNegStockAllow = string.Empty;
                string postWithDate = commonDal.settings("Integration", "PostWithDateTime");
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {

                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.ID IN('" + IDs + "')";


                    sqlText = "";
                    sqlText += @" Update  ReceiveDetails             set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE ReceiveNo IN('" + IDs + "')";
                    sqlText += @" Update  ReceiveHeaders             set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE ReceiveNo IN('" + IDs + "')";
                    sqlText += @" Update  IssueDetails               set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE IssueNo IN('" + IDs + "')";
                    sqlText += @" Update  IssueHeaders               set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE IssueNo IN('" + IDs + "')";
                    sqlText += @" Update  IssueDetailBOMs            set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE IssueNo IN('" + IDs + "')";
                    sqlText += @" Update  IssueHeaderBOMs            set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE IssueNo IN('" + IDs + "')";
                    sqlText += @" Update  PurchaseInvoiceDuties      set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE PurchaseInvoiceNo IN('" + IDs + "')";
                    sqlText += @" Update  PurchaseInvoiceDetails     set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE PurchaseInvoiceNo IN('" + IDs + "')";
                    sqlText += @" Update  PurchaseInvoiceHeaders     set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE PurchaseInvoiceNo IN('" + IDs + "')";
                    sqlText += @" Update  PurchaseTDSs               set  Post ='Y'    WHERE PurchaseInvoiceNo IN('" + IDs + "')";
                    sqlText += @" Update  SalesInvoiceHeaders        set  SignatoryName=@SignatoryName,SignatoryDesig=@SignatoryDesig, Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE SalesInvoiceNo IN('" + IDs + "')";
                    sqlText += @" Update  SalesInvoiceDetails        set  Post ='Y',LastModifiedBy =@LastModifiedBy,LastModifiedOn =@LastModifiedOn    WHERE SalesInvoiceNo IN('" + IDs + "')";

                    if (postWithDate == "Y")
                    {
                        sqlText += @" Update  SalesInvoiceHeaders        set  InvoiceDateTime = @InvoiceDateTime   WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";
                        sqlText += @" Update  SalesInvoiceDetails        set  InvoiceDateTime = @InvoiceDateTime    WHERE SalesInvoiceNo=@MasterSalesInvoiceNo ";


                    }


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


                    if (postWithDate == "Y")
                    {
                        string dateTime = commonDal.ServerDateWithTime();
                        cmdPost.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime", dateTime);
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

        public string[] ImportBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null, bool IsDiscount = false, string userId = null, string token = null, IntegrationParam paramVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region SQL Text
                //SalesTempData.BranchId = Customers.BranchId and
                //SalesTempData.BranchId = Customers.BranchId and

                string sqlText = "";

                string defaultValueSql = @"       
update SalesTempData set ItemNo = 0, CustomerID = 0;
update SalesTempData set VehicleType = '-' where VehicleType is null;      
--update SalesTempData set CustomerBIN = '-' where len(CustomerBIN) = 0 or CustomerBIN is null                                
";

                string updateItemCustomerId = @"
update SalesTempData set ItemNo=Products.ItemNo from Products where Products.ProductCode=SalesTempData.Item_Code  and  SalesTempData.ItemNo = '0';
update SalesTempData set ItemNo=Products.ItemNo from Products where Products.productName =SalesTempData.Item_Name and  SalesTempData.ItemNo = '0';




";

                string getdefaultData = @"
select * from SalesTempData where ItemNo = 0 or CustomerID = 0 or ((GroupId is null or GroupId = 0) and CustomerGroup is not null)";

                string getCustomers = @"
select distinct BranchId, CustomerID, Customer_Name, Customer_Code, CustomerGroup, CustomerBIN, Delivery_Address 
from SalesTempData where CustomerID = 0 or CustomerID is null";

                string getItemNo = @"
select distinct BranchId, ItemNo, Item_Code, Item_Name, UOM, VAT_Rate, NBR_Price 
from SalesTempData where ItemNo = 0";

                string getGroups = @"
select distinct BranchId, GroupId, CustomerGroup from SalesTempData where (GroupId is null or GroupId = 0) and CustomerGroup is not null";


                string getVehicleData = @"select distinct Vehicle_No,VehicleType from SalesTempData where VehicleID = 0 or VehicleID is null;";

                string branchAndCurrency = @"
update SalesTempData set BranchId=BranchProfiles.BranchID 
from BranchProfiles where BranchProfiles.BranchCode=SalesTempData.Branch_Code or BranchProfiles.IntegrationCode = SalesTempData.Branch_Code;
update SalesTempData set CurrencyId=Currencies.CurrencyId from Currencies where Currencies.CurrencyCode=SalesTempData.Currency_Code;


update SalesTempData set Discount_Amount = 0 where Discount_Amount is null
update SalesTempData set DiscountedNBRPrice = NBR_Price 

--UPDATE SalesTempData SET SubTotal = (NBR_Price*Quantity)  WHERE SubTotal IS NULL OR SubTotal <= 0
update SalesTempData set Delivery_Date_Time = Invoice_Date_Time where Delivery_Date_Time is null

";
                string customerGroup = @"
update SalesTempData set GroupId=CustomerGroups.CustomerGroupID from CustomerGroups where CustomerGroups.CustomerGroupName=SalesTempData.CustomerGroup;
";
                string vehicleNo = @"
update SalesTempData set VehicleID=(select top 1 Vehicles.VehicleID 
from Vehicles where Vehicles.VehicleNo=SalesTempData.Vehicle_No and Vehicles.VehicleType = SalesTempData.VehicleType); 
update SalesTempData set VAT_Name='VAT 4.3' where VAT_Name like '%vat 1%';
";
                // var setIsProcessed = @"update SalesTempData set IsProcessed = 1 where ItemNo != 0 and CustomerID != 0";

                string bomUpdate = @" update SalesTempData set BOMId = boms.BOMId from boms where 
 boms.FinishItemNo = SalesTempData.ItemNo and boms.VATName = SalesTempData.VAT_Name 
 and boms.EffectDate <= cast(SalesTempData.Invoice_Date_Time as datetime) --cast(SalesTempData.Invoice_Date_Time as datetime);";

                string uomUpdate = @"

update SalesTempData set UOMn = Products.UOM from Products 
where SalesTempData.ItemNo = Products.ItemNo;

update SalesTempData set SalesTempData.UOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = SalesTempData.UOMn 
and UOMs.UOMTo = SalesTempData.UOM and (SalesTempData.UOMc = 0 or SalesTempData.UOMc is null)

update SalesTempData set UOMc = (case when UOM = UOMn then 1.00 else UOMc end)
where UOMc = 0 or UOMc is null

update SalesTempData set NBR_Price = isnull(UOMc,0) * UOMPrice;

update SalesTempData set UOMQty = UOMc * Quantity where UOMQty = 0 or UOMQty is null ;

--------update SalesTempData set SDAmount = ((SubTotal*SD_Rate)/100);

update SalesTempData set CurrencyRateFromBDT = CurrencyConversion.CurrencyRate from CurrencyConversion where 
SalesTempData.CurrencyId = CurrencyConversion.CurrencyCodeFrom and 260 = CurrencyConversion.CurrencyCodeTo



update SalesTempData set ReturnId = CurrencyConversion.CurrencyRate from CurrencyConversion where 
249 = CurrencyConversion.CurrencyCodeFrom and 260 = CurrencyConversion.CurrencyCodeTo

update SalesTempData set ShiftId = (select top 1 Shifts.Id from Shifts where cast(SalesTempData.Invoice_Date_Time as time) between Shifts.ShiftStart and Shifts.ShiftEnd);

update SalesTempData set IsCommercialImporter = Settings.SettingValue from Settings where SettingGroup = 'CommercialImporter' and SettingName = 'CommercialImporter';

update SalesTempData set 
[Type] = case
when [Type] = 'vat' or [Type] = 'vat system'
then 'VAT'
when [Type] = 'non-vat system'
then 'NonVAT'
else
[Type]
end


update SalesTempData set Delivery_Address = Customers.Address1 
from Customers 
where SalesTempData.CustomerID = Customers.CustomerID and 
(SalesTempData.Delivery_Address = '-' or  SalesTempData.Delivery_Address is null)


update SalesTempData set PreviousImportedExcelId = Previous_Invoice_No where TransactionType = 'credit' or TransactionType = 'debit'
update SalesTempData set Previous_Invoice_No = SalesInvoiceHeaders.SalesInvoiceNo from SalesInvoiceHeaders 
where SalesInvoiceHeaders.ImportIDExcel = SalesTempData.PreviousImportedExcelId and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousInvoiceDateTime = SalesInvoiceHeaders.InvoiceDateTime from SalesInvoiceHeaders 
where SalesInvoiceHeaders.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesTempData.PreviousInvoiceDateTime is null
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousNBRPrice = SalesInvoiceDetails.NBRPrice from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousNBRPrice is null or SalesTempData.PreviousNBRPrice = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousQuantity = SalesInvoiceDetails.Quantity from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousQuantity is null or SalesTempData.PreviousQuantity = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousSubTotal = SalesInvoiceDetails.SubTotal from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousSubTotal is null or SalesTempData.PreviousSubTotal = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousSD = SalesInvoiceDetails.SD from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousSD is null or SalesTempData.PreviousSD = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')


update SalesTempData set PreviousSDAmount = SalesInvoiceDetails.SDAmount from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousSDAmount is null or SalesTempData.PreviousSDAmount = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')


update SalesTempData set PreviousVATRate = SalesInvoiceDetails.VATRate from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousVATRate is null or SalesTempData.PreviousVATRate = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')


update SalesTempData set PreviousVATAmount = SalesInvoiceDetails.VATAmount from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousVATAmount is null or SalesTempData.PreviousVATAmount = 0)
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')

update SalesTempData set PreviousUOM = SalesInvoiceDetails.VATAmount from SalesInvoiceDetails 
where SalesInvoiceDetails.SalesInvoiceNo = SalesTempData.PreviousImportedExcelId and SalesInvoiceDetails.ItemNo = SalesTempData.ItemNo
and (SalesTempData.PreviousUOM is null or SalesTempData.PreviousUOM = '-')
and (SalesTempData.TransactionType = 'credit'or SalesTempData.TransactionType = 'debit')




";

                string deleteDuplicate = @"delete from SalesTempData where ID in (
select SalesTempData.ID from 
SalesTempData join SalesInvoiceHeaders on SalesTempData.ID = SalesInvoiceHeaders.ImportIDExcel
)";




                string selectDuplicate = @"
select distinct SalesTempData.ID from 
SalesTempData join SalesInvoiceHeaders on SalesTempData.ID = SalesInvoiceHeaders.ImportIDExcel";

                string updateCustomerBIN = @"
create table #customerTemp(
SL int identity(1,1), 
Customer_Name varchar(200), 
CustomerID varchar(50),
BIN varchar(50),
)

insert into #customerTemp(Customer_Name,CustomerId, BIN)
select distinct Customer_Name,CustomerID,CustomerBIN
from SalesTempData where len(CustomerBIN) > 3


update Customers Set Customers.VATRegistrationNo = #customerTemp.BIN from #customerTemp 
where Customers.CustomerID = #customerTemp.CustomerID 
and (Customers.VATRegistrationNo is null or len(Customers.VATRegistrationNo) =0 or Customers.VATRegistrationNo = '-')

drop table #customerTemp";

                string uomDefault = @"update SalesTempData set UOMc = 1
                where UOMc = 0 or UOMc is null

update SalesTempData set NBR_Price = UOMc * UOMPrice;

update SalesTempData set UOMQty = UOMc * Quantity where UOMQty = 0 or UOMQty is null ;
";
                string discountAmount = @"update SalesTempData set UOMPrice = (DiscountedNBRPrice - Discount_Amount)";
                string uomPrice = @"update SalesTempData set UOMPrice = NBR_Price";


                string deleteTemp = @"delete from SalesTempData ";


                if (userId != null)
                {
                    deleteTemp += " where UserId = " + userId;
                }

                if (token != null)
                {
                    deleteTemp += " where token = '" + token + "'";
                }

                // " DBCC CHECKIDENT ('SalesTempData', RESEED, 0);  " +

                //deleteTemp += defaultValueSql;



                #endregion

                #region Discount Amount Condition Check
                CommonDAL commonDal = new CommonDAL();

                string code = commonDal.settingValue("CompanyCode", "Code", null, currConn, transaction);

                if (code != "BATA")
                {
                    branchAndCurrency += discountAmount;
                }
                else
                {
                    branchAndCurrency += uomPrice;
                }

                #endregion

                #region Clear SalesTempData and Bulk Insert

                var cmd = new SqlCommand(deleteTemp, currConn, transaction);
                cmd.CommandTimeout = 200;
                if (deleteFlag)
                {
                    transResult = cmd.ExecuteNonQuery();
                }


                retResults = commonDal.BulkInsert("SalesTempData", salesData, currConn, transaction, 0, rowsCopiedCallBack);

                #endregion



                #region Update InvoiceDateTime
                if (paramVM != null && paramVM.IsEntryDate == true && !string.IsNullOrWhiteSpace(paramVM.InvoiceDateTime))
                {
                    string InvoiceDateTime = "";
                    string CurrentTime = "";
                    if (!string.IsNullOrEmpty(paramVM.InvoiceTime))
                    {
                        CurrentTime = paramVM.InvoiceTime;
                    }
                    else
                    {
                        CurrentTime = DateTime.Now.ToString(" HH:mm:ss");
                    }

                    InvoiceDateTime = paramVM.InvoiceDateTime + CurrentTime;

                    sqlText = "";
                    sqlText = @"  update SalesTempData set Invoice_Date_Time = @InvoiceDateTime";
                    cmd.CommandText = sqlText;
                    cmd.Parameters.AddWithValue("@InvoiceDateTime", OrdinaryVATDesktop.DateToDate(InvoiceDateTime));

                    transResult = cmd.ExecuteNonQuery();

                }

                if (paramVM != null && !string.IsNullOrWhiteSpace(paramVM.InvoiceTime))
                {

                    sqlText = "";
                    sqlText =
                        @" update SalesTempData set Invoice_Date_Time = FORMAT(cast(Invoice_Date_Time as datetime),'yyyy-MM-dd')+' '+'" +
                        paramVM.InvoiceTime + "'";

                    cmd.CommandText = sqlText;
                    transResult = cmd.ExecuteNonQuery();

                }
                #endregion


                #region Update Period Id


                sqlText = "";
                sqlText = @"  
update  SalesTempData                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(Invoice_Date_Time)) +  CONVERT(VARCHAR(4),YEAR(Invoice_Date_Time)),6)
where PeriodId is null or PeriodId = ''
";
                cmd.CommandText = sqlText;

                transResult = cmd.ExecuteNonQuery();


                #endregion


                #region Duplicate Continue Check

                string salesDuplicate = commonDal.settingValue("Import", "SaleDuplicateInsert", null, currConn, transaction);

                if (code.ToLower() == "cp" || code == "ACIGDJ" || OrdinaryVATDesktop.IsACICompany(code))
                {
                    if (salesDuplicate.ToLower() == "y")
                    {
                        cmd.CommandText = deleteDuplicate;

                        int flag = cmd.ExecuteNonQuery();
                    }
                    else
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
                }


                if (OrdinaryVATDesktop.IsACICompany(code))
                {
                    string productConfirmCheck = @"select top 1 ProductCode from 
Products p join (select distinct Item_Code from SalesTempData)stp
on p.ProductCode = stp.Item_Code 
where isnull(P.IsConfirmed,'Y') = 'N'";

                    SqlCommand cmdConfirmCheck = new SqlCommand(productConfirmCheck, vConnection, transaction);
                    SqlDataAdapter confrmAdapter = new SqlDataAdapter(cmdConfirmCheck);
                    DataTable confrmtable = new DataTable();

                    confrmAdapter.Fill(confrmtable);

                    if (confrmtable.Rows.Count > 0)
                    {
                        throw new Exception(confrmtable.Rows[0][0] + " Product Code Is Not Yet Confirmed");
                    }

                }


                #region Delete Duplicate Data with Period Id



                if (code == "BCL" || code == "GDIC")
                {

                    sqlText = "";
                    sqlText = @"  
delete from SalesTempData  
where 1=1
and ID in (select ImportIDExcel from SalesInvoiceHeaders)
and PeriodId in (select PeriodID from SalesInvoiceHeaders)
and (PeriodId is not null and PeriodId <> '')
";
                    cmd.CommandText = sqlText;

                    transResult = cmd.ExecuteNonQuery();
                }

                #endregion
                #endregion

                #region FiscalYearCheck

                cmd.CommandText = @"
 select distinct cast(Temp.invoice_date as varchar(200))invoice_date
 from (
select cast(invoice_date_time as date)invoice_date 
from SalesTempData) as Temp";

                SqlDataAdapter fAdapter = new SqlDataAdapter(cmd);
                DataTable ftable = new DataTable();

                fAdapter.Fill(ftable);

                string fiscalYearText = @"select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                        " where 1=1 and isnull(PeriodLock,'Y') = 'Y' and isnull(GLLock,'Y') = 'Y'";

                int rowsCount = ftable.Rows.Count;


                if (rowsCount > 0)
                {
                    fiscalYearText += "  and (";

                    for (var index = 0; index < rowsCount; index++)
                    {
                        DataRow row = ftable.Rows[index];
                        fiscalYearText += "  ( '" + row["invoice_date"] + "'  between PeriodStart and PeriodEnd  )";

                        if (index != (rowsCount - 1))
                        {
                            fiscalYearText += " or ";
                        }
                    }


                    fiscalYearText += ")";

                }



                cmd.CommandText = fiscalYearText;
                ftable = new DataTable();
                fAdapter.SelectCommand = cmd;

                fAdapter.Fill(ftable);

                if (ftable.Rows.Count > 0)
                {
                    throw new Exception("Fiscal Year Is Locked");
                }


                #endregion

                #region Update Existing ItemNo, CustomerId

                //string CompanyCode = commonDal.settingValue("CompanyCode", "Code");


                if (code == "KCCL")
                {
                    updateItemCustomerId += @" 
update SalesTempData set ItemNo = ProductDetails.ItemNo 
from ProductDetails where SalesTempData.Item_Name = ProductDetails.ProductName and SalesTempData.ItemNo = '0' ";
                }
                else if (code != "DHL" && !OrdinaryVATDesktop.IsDHLCrat)
                {
                    updateItemCustomerId +=
                        @"
update SalesTempData set CustomerID=(select top 1 Customers.CustomerID 
from Customers where Customers.CustomerCode =SalesTempData.Customer_Code ) where   ( SalesTempData.CustomerID = '0' or SalesTempData.CustomerID is null)

update SalesTempData set CustomerID=(select top 1 Customers.CustomerID 
from Customers where Customers.CustomerName =SalesTempData.Customer_Name  ) where   ( SalesTempData.CustomerID = '0' or SalesTempData.CustomerID is null)

";
                }
                else if (code == "DHL" && !OrdinaryVATDesktop.IsDHLCrat)
                {
                    updateItemCustomerId +=
                        @"
update SalesTempData set CustomerID=Customers.CustomerID from Customers 
where Customers.CustomerName =SalesTempData.Customer_Name and  SalesTempData.CustomerID = '0'
and Customers.BranchId != 2;";
                }
                else if (OrdinaryVATDesktop.IsDHLCrat)
                {
                    updateItemCustomerId += @" 
update SalesTempData  set CustomerID=Customers.CustomerID from Customers 
where Customers.CustomerName =SalesTempData.Customer_Name 
and SalesTempData.Delivery_Address = Customers.Address1 
and  SalesTempData.CustomerID = '0';";
                }
                else
                {
                    updateItemCustomerId += @"

update SalesTempData set CustomerID=(select top 1 Customers.CustomerID 
from Customers where Customers.CustomerCode =SalesTempData.Customer_Code ) where   ( SalesTempData.CustomerID = '0' or SalesTempData.CustomerID is null)

update SalesTempData set CustomerID=(select top 1 Customers.CustomerID 
from Customers where Customers.CustomerName =SalesTempData.Customer_Name  ) where   ( SalesTempData.CustomerID = '0' or SalesTempData.CustomerID is null)

";
                }

                #region Sql Command

                cmd.CommandText = branchAndCurrency;

                var updateItemResult = cmd.ExecuteNonQuery();

                cmd.CommandText = defaultValueSql + updateItemCustomerId + " " + customerGroup + " " + vehicleNo;

                cmd.ExecuteNonQuery();


                cmd.CommandText = getCustomers;

                var defaultData = new DataTable();
                DataTable dtCustomerGroup = new DataTable();
                DataTable dtCustomer = new DataTable();
                DataTable dtItem = new DataTable();

                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dtCustomer);

                cmd.CommandText = getItemNo;
                dataAdapter.Fill(dtItem);

                cmd.CommandText = getGroups;
                dataAdapter.Fill(dtCustomerGroup);

                #endregion

                #endregion

                #region Insert New Customer and Products

                var commonImportDal = new CommonImportDAL();

                string productSave = commonDal.settingValue("AutoSave", "SaleProduct", null, currConn, transaction);
                string customerSave = commonDal.settingValue("AutoSave", "SaleCustomers", null, currConn, transaction);

                #region Customer insert check flag

                bool codeCheck = false;
                bool nameCheck = false;
                bool addrsCheck = false;

                if (code == "DHL" && !OrdinaryVATDesktop.IsDHLCrat)
                {
                    codeCheck = true;
                }
                else if (OrdinaryVATDesktop.IsDHLCrat)
                {
                    codeCheck = true;
                    nameCheck = true;
                    addrsCheck = true;
                }

                #endregion



                DataTable dtTemp = new DataTable();

                ////// branch 2 2 2
                //////dtTemp = defaultData;
                //////DataView dv = new DataView(dtCustomer);

                #region Customer Group

                //{
                //    dtTemp = new DataTable();
                //    dtTemp = dv.ToTable(true, "BranchId", "GroupId", "CustomerGroup");
                //    DataRow[] drr = dtTemp.Select("GroupId IN ('','0')");
                //    if (drr != null && drr.Count() > 0)
                //    {
                //        dtCustomerGroup = drr.CopyToDataTable();

                //    }
                //}


                foreach (DataRow dr in dtCustomerGroup.Rows)
                {
                    int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                    if (string.IsNullOrEmpty(dr["GroupId"].ToString()) || dr["GroupId"].ToString() == "0")
                    {
                        if (customerSave == "Y")
                        {
                            if (!string.IsNullOrEmpty(dr["CustomerGroup"].ToString()))
                            {
                                CustomerGroupDAL groupDal = new CustomerGroupDAL();

                                CustomerGroupVM groupVM = new CustomerGroupVM();

                                groupVM.CustomerGroupName = dr["CustomerGroup"].ToString();
                                groupVM.CustomerGroupDescription = "-";
                                groupVM.ActiveStatus = "Y";
                                groupVM.GroupType = "LOCAL";
                                groupVM.Comments = "-";
                                groupVM.Info1 = "-";
                                groupVM.Info2 = "-";
                                groupVM.Info3 = "-";
                                groupVM.Info4 = "-";
                                groupVM.Info5 = "-";
                                groupVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                string[] results = groupDal.InsertToCustomerGroupNew(groupVM, null, currConn, transaction);
                            }
                        }
                    }
                }
                #endregion

                #region Customer
                //{
                //    dtTemp = new DataTable();
                //    dtTemp = dv.ToTable(true, "BranchId", "CustomerID", "Customer_Name", "Customer_Code", "CustomerGroup", "CustomerBIN", "Delivery_Address");
                //    DataRow[] drr = dtTemp.Select("CustomerID  IN ('','0')");

                //    if (drr != null && drr.Count() > 0)
                //    {
                //        dtCustomer = drr.CopyToDataTable();
                //    }
                //}


                foreach (DataRow dr in dtCustomer.Rows)
                {
                    int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                    if (dr["CustomerID"].ToString().Trim() == "0" || string.IsNullOrEmpty(dr["CustomerID"].ToString().Trim()))
                    {
                        if (customerSave == "Y")
                        {
                            string customerName = dr["Customer_Name"].ToString().Trim();
                            string customerCode = dr["Customer_Code"].ToString().Trim();
                            string group = dr["CustomerGroup"].ToString().Trim();
                            string CustomerBIN = string.IsNullOrEmpty(dr["CustomerBIN"].ToString().Trim()) ? "-" : dr["CustomerBIN"].ToString().Trim();
                            string Delivery_Address = string.IsNullOrEmpty(dr["Delivery_Address"].ToString().Trim()) ? "-" : dr["Delivery_Address"].ToString().Trim();

                            commonImportDal.FindCustomerId(customerName, customerCode, currConn, transaction, true, group, branchIdRow, null, CustomerBIN, nameCheck, Delivery_Address, codeCheck, addrsCheck);
                        }
                        else
                        {
                            throw new Exception("Customer Not Found-" + dr["Customer_Code"].ToString() + dr["Customer_Name"].ToString().Trim());
                        }

                    }
                }
                #endregion

                #region Item/Product
                //{
                //    dtTemp = new DataTable();2222
                //    dtTemp = dv.ToTable(true, "BranchId", "ItemNo", "Item_Code", "Item_Name", "UOM", "VAT_Rate", "NBR_Price");
                //    DataRow[] drr = dtTemp.Select("ItemNo IN ('','0')");

                //    if (drr != null && drr.Count() > 0)
                //    {
                //        dtItem = drr.CopyToDataTable();
                //    }
                //}

                foreach (DataRow dr in dtItem.Rows)
                {
                    int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                    if (dr["ItemNo"].ToString().Trim() == "0" || string.IsNullOrEmpty(dr["ItemNo"].ToString().Trim()))
                    {

                        if (productSave == "Y")
                        {
                            string itemCode = dr["Item_Code"].ToString().Trim();
                            string itemName = dr["Item_Name"].ToString().Trim();
                            string uom = dr["UOM"].ToString().Trim();
                            decimal varRate = !string.IsNullOrEmpty(dr["VAT_Rate"].ToString())
                                ? Convert.ToDecimal(dr["VAT_Rate"].ToString())
                                : 0;

                            decimal nbrPrice = !string.IsNullOrEmpty(dr["NBR_Price"].ToString())
                                ? Convert.ToDecimal(dr["NBR_Price"].ToString())
                                : 0;

                            commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, uom, branchIdRow, varRate, nbrPrice);
                        }
                        else
                        {
                            throw new Exception("Product Not Found-" + dr["Item_Code"].ToString() + dr["Item_Name"].ToString().Trim());
                        }

                    }

                }
                #endregion

                #endregion

                #region Insert Vehicle

                defaultData = new DataTable();

                cmd.CommandText = getVehicleData;

                dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(defaultData);

                VehicleDAL dal = new VehicleDAL();


                foreach (DataRow row in defaultData.Rows)
                {
                    VehicleVM vm = new VehicleVM();
                    //    &&
                    //(row["VehicleType"] != null && row["VehicleType"].ToString().Trim() != "-" &&
                    // !string.IsNullOrEmpty(row["VehicleType"].ToString().Trim()))

                    if ((row["Vehicle_No"] != null && row["Vehicle_No"].ToString().Trim() != "-" &&
                         !string.IsNullOrEmpty(row["Vehicle_No"].ToString().Trim())))
                    {
                        vm.VehicleNo = row["Vehicle_No"].ToString();
                        vm.ActiveStatus = "Y";
                        vm.VehicleType = "-";
                        vm.Code = vm.VehicleNo;
                        vm.VehicleType = row["VehicleType"] == null || string.IsNullOrEmpty(row["VehicleType"].ToString()) ? "-" : row["VehicleType"].ToString();
                        vm.CreatedBy = "Excel";
                        string[] result = dal.InsertToVehicle(vm, null, currConn, transaction);

                        //if (result[0].ToLower() != "success")
                        //{
                        //    throw new Exception("Vehicle Cant be Inserted " + row["Vehicle_No"]);
                        //}
                    }
                }



                #endregion

                #region Update Again for New Entry (BranchId, ItemNo, CustomerGroup, BOM, UOM, VehicleNo)
                cmd.Parameters.AddWithValue("@branchId", branchId);
                string branchText =
                    @"
update SalesTempData set BranchId = @branchId where BranchId is null or BranchId = 0 ;

update SalesTempData Set ProductDescription = Item_Name 
where ProductDescription is null or ProductDescription = '0' or ProductDescription = 'NA'";

                string uomc = commonDal.settingValue("Integration", "UOM", null, currConn, transaction);

                if (uomc.ToLower() == "y")
                {
                    uomUpdate += "  " + uomDefault;
                }


                string updateLastStep = branchText + updateItemCustomerId + " " + customerGroup + " " + bomUpdate + " " + uomUpdate + " " + vehicleNo;
                cmd.CommandText = updateLastStep;

                if (OrdinaryVATDesktop.IsDHLCrat)
                {
                    cmd.CommandText += updateCustomerBIN;
                }

                int reUpdate = cmd.ExecuteNonQuery();

                #endregion

                #region Commit

                if (vTransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }

                retResults[0] = "Success";
                retResults[2] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();

                #endregion
            }

            #region Catch and finally

            catch (Exception ex)
            {
                if (vTransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }
                //transaction.Commit();
                FileLogger.Log("SaleDAL124", "SaveAndProcess", ex.Message + " \n" + ex.StackTrace);

                throw ex;
            }
            finally
            {
                if (vConnection == null)
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

        public string[] ProcessBigData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }
                #endregion

                #region SQL Text

                var defaultValueSql = @"update SalesTempData set ItemNo = 0, CustomerID = 0;";

                var updateItemCustomerId = @"
update SalesTempData set ItemNo=Products.ItemNo from Products where Products.ProductCode=SalesTempData.Item_Code;
update SalesTempData set ItemNo=Products.ItemNo from Products where Products.productName =SalesTempData.Item_Name;

update SalesTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerCode =SalesTempData.Customer_Code;
update SalesTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerName =SalesTempData.Customer_Name;";

                var getdefaultData = @"select * from SalesTempData where ItemNo = 0 or CustomerID = 0 or GroupId is null or GroupId = 0;";
                var branchAndCurrency = @"
update SalesTempData set BranchId=BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode=SalesTempData.Branch_Code;
update SalesTempData set CurrencyId=Currencies.CurrencyId from Currencies where Currencies.CurrencyCode=SalesTempData.Currency_Code;
";
                var customerGroup = @"
update SalesTempData set GroupId=CustomerGroups.CustomerGroupID from CustomerGroups where CustomerGroups.CustomerGroupName=SalesTempData.CustomerGroup;
";
                var vehicleNo = @"
update SalesTempData set VehicleID=Vehicles.VehicleID from Vehicles where Vehicles.VehicleNo=SalesTempData.Vehicle_No; 
update SalesTempData set VAT_Name='VAT 4.3' where VAT_Name like '%vat 1%';
";
                // var setIsProcessed = @"update SalesTempData set IsProcessed = 1 where ItemNo != 0 and CustomerID != 0";

                var bomUpdate = @" update SalesTempData set BOMId = boms.BOMId from boms where 
 boms.FinishItemNo = SalesTempData.ItemNo and boms.VATName = SalesTempData.VAT_Name 
 and boms.EffectDate <= cast(SalesTempData.Invoice_Date_Time as datetime);";

                var uomUpdate = @"

update SalesTempData set UOMn = Products.UOM from Products 
where SalesTempData.ItemNo = Products.ItemNo;

update SalesTempData set SalesTempData.UOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = SalesTempData.UOMn and UOMs.UOMTo = SalesTempData.UOM;

 update SalesTempData set UOMc = (case when UOM = UOMn then 1.00 else UOMc end);

update SalesTempData set UOMPrice = UOMc * NBR_Price;

update SalesTempData set UOMQty = UOMc * Quantity ;

update SalesTempData set SDAmount = ((SubTotal*SD_Rate)/100);

update SalesTempData set CurrencyRateFromBDT = CurrencyConversion.CurrencyRate from CurrencyConversion where 
SalesTempData.CurrencyId = CurrencyConversion.CurrencyCodeFrom and 260 = CurrencyConversion.CurrencyCodeTo

update SalesTempData set ShiftId = (select top 1 Shifts.Id from Shifts where cast(SalesTempData.Invoice_Date_Time as time) between Shifts.ShiftStart and Shifts.ShiftEnd);

update SalesTempData set IsCommercialImporter = Settings.SettingValue from Settings where SettingGroup = 'CommercialImporter' and SettingName = 'CommercialImporter';
update SalesTempData set BranchId = @branchId where BranchId is null or BranchId = 0 ;


";


                #endregion

                var cmd = new SqlCommand("", currConn, transaction);

                #region Sql Command

                cmd.CommandText = branchAndCurrency + " " + updateItemCustomerId + " " + customerGroup + " " + vehicleNo;
                var updateItemResult = cmd.ExecuteNonQuery();

                cmd.CommandText = getdefaultData;
                var defaultData = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(defaultData);



                #endregion

                #region Insert Customer and Products

                var commonImportDal = new CommonImportDAL();

                if (defaultData != null && defaultData.Rows.Count > 0)
                {


                    DataTable dtTemp = new DataTable();
                    DataTable dtCustomerGroup = new DataTable();
                    DataTable dtCustomer = new DataTable();
                    DataTable dtItem = new DataTable();

                    dtTemp = defaultData;
                    DataView dv = new DataView(dtTemp);

                    dtCustomerGroup = dv.ToTable(true, "BranchId", "GroupId", "CustomerGroup");
                    dtCustomer = dv.ToTable(true, "BranchId", "CustomerID", "Customer_Name", "Customer_Code", "CustomerGroup");
                    dtItem = dv.ToTable(true, "BranchId", "ItemNo", "Item_Code", "Item_Name", "UOM", "VAT_Rate");


                    #region Customer Group

                    foreach (DataRow dr in dtCustomerGroup.Rows)
                    {
                        int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                        if (string.IsNullOrEmpty(dr["GroupId"].ToString()) || dr["GroupId"].ToString() == "0")
                        {

                            if (!string.IsNullOrEmpty(dr["CustomerGroup"].ToString()))
                            {
                                CustomerGroupDAL groupDal = new CustomerGroupDAL();

                                CustomerGroupVM groupVM = new CustomerGroupVM();

                                groupVM.CustomerGroupName = dr["CustomerGroup"].ToString();
                                groupVM.CustomerGroupDescription = "-";
                                groupVM.ActiveStatus = "Y";
                                groupVM.GroupType = "LOCAL";
                                groupVM.Comments = "-";
                                groupVM.Info1 = "-";
                                groupVM.Info2 = "-";
                                groupVM.Info3 = "-";
                                groupVM.Info4 = "-";
                                groupVM.Info5 = "-";
                                groupVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                string[] results = groupDal.InsertToCustomerGroupNew(groupVM);
                            }
                        }
                    }
                    #endregion

                    #region Customer
                    foreach (DataRow dr in dtCustomer.Rows)
                    {
                        int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                        if (dr["CustomerID"].ToString().Trim() == "0")
                        {
                            string customerName = dr["Customer_Name"].ToString().Trim();
                            string customerCode = dr["Customer_Code"].ToString().Trim();
                            string group = dr["CustomerGroup"].ToString().Trim();

                            commonImportDal.FindCustomerId(customerName, customerCode, currConn, transaction, true, group, branchIdRow);
                        }
                    }
                    #endregion

                    #region Item/Product
                    foreach (DataRow dr in dtItem.Rows)
                    {
                        int branchIdRow = !string.IsNullOrEmpty(dr["BranchId"].ToString()) ? Convert.ToInt32(dr["BranchId"].ToString()) : branchId;

                        if (dr["ItemNo"].ToString().Trim() == "0")
                        {
                            var itemCode = dr["Item_Code"].ToString().Trim();
                            var itemName = dr["Item_Name"].ToString().Trim();
                            var uom = dr["UOM"].ToString().Trim();
                            var varRate = !string.IsNullOrEmpty(dr["VAT_Rate"].ToString())
                                ? Convert.ToDecimal(dr["VAT_Rate"].ToString())
                                : 0;

                            commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, uom, branchIdRow, varRate);
                        }

                    }
                    #endregion

                }


                #region Comments Jun-18-2020

                ////foreach (DataRow row in defaultData.Rows)
                ////{
                ////    var branchIdRow = !string.IsNullOrEmpty(row["BranchId"].ToString()) ? Convert.ToInt32(row["BranchId"].ToString()) : branchId;

                ////    if (string.IsNullOrEmpty(row["GroupId"].ToString()) || row["GroupId"].ToString() == "0")
                ////    {

                ////        if (!string.IsNullOrEmpty(row["CustomerGroup"].ToString()))
                ////        {
                ////            var groupDal = new CustomerGroupDAL();

                ////            var groupVM = new CustomerGroupVM();

                ////            groupVM.CustomerGroupName = row["CustomerGroup"].ToString();
                ////            groupVM.CustomerGroupDescription = "-";
                ////            groupVM.ActiveStatus = "Y";
                ////            groupVM.GroupType = "LOCAL";
                ////            groupVM.Comments = "-";
                ////            groupVM.Info1 = "-";
                ////            groupVM.Info2 = "-";
                ////            groupVM.Info3 = "-";
                ////            groupVM.Info4 = "-";
                ////            groupVM.Info5 = "-";
                ////            groupVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                ////            var results = groupDal.InsertToCustomerGroupNew(groupVM);
                ////        }
                ////    }


                ////    if (row["CustomerID"].ToString().Trim() == "0")
                ////    {
                ////        var customerName = row["Customer_Name"].ToString().Trim();
                ////        var customerCode = row["Customer_Code"].ToString().Trim();
                ////        var group = row["CustomerGroup"].ToString().Trim();

                ////        commonImportDal.FindCustomerId(customerName, customerCode, currConn, transaction, true, group, branchIdRow);
                ////    }

                ////    if (row["ItemNo"].ToString().Trim() == "0")
                ////    {
                ////        var itemCode = row["Item_Code"].ToString().Trim();
                ////        var itemName = row["Item_Name"].ToString().Trim();
                ////        var uom = row["UOM"].ToString().Trim();
                ////        var varRate = !string.IsNullOrEmpty(row["VAT_Rate"].ToString())
                ////            ? Convert.ToDecimal(row["VAT_Rate"].ToString())
                ////            : 0;

                ////        commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, uom, branchIdRow, varRate);
                ////    }


                ////}

                #endregion


                #endregion

                #region Reupdate
                cmd.Parameters.AddWithValue("@branchId", branchId);
                cmd.CommandText = updateItemCustomerId + " " + customerGroup + " " + bomUpdate + " " + uomUpdate;
                var reUpdate = cmd.ExecuteNonQuery();

                #endregion

                retResults[0] = "success";

                if (retResults[0].ToLower() == "success" && updateItemResult > 0 && vTransaction == null)
                {
                    transaction.Commit();
                }

                retResults[0] = retResults[0].ToLower() == "success" && (updateItemResult > 0 || reUpdate > 0) ? "success" : "fail";

                retResults[2] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();
            }

            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open && vConnection == null)
                {
                    currConn.Close();
                }
            }

            #endregion

            return retResults;
        }

        public string[] SaveAndProcess(DataTable data, Action callBack = null, int branchId = 1, string app = "", SysDBInfoVMTemp connVM = null, IntegrationParam paramVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string UserId = "")
        {
            #region Initializations

            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            CommonDAL commonDal = new CommonDAL();
            int transResult = 0;
            #endregion

            try
            {
                #region Connection and Transaction

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    connection.Open();
                }
                else
                {
                    connection = vConnection;
                }
                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }
                else
                {
                    transaction = vTransaction;
                }

                #endregion

                #region Merge Date Time

                if (data.Columns.Contains("Invoice_Date") && data.Columns.Contains("Invoice_Time"))
                {
                    if (!data.Columns.Contains("Invoice_Date_Time"))
                    {
                        data.Columns.Add("Invoice_Date_Time");
                    }

                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["Invoice_Date_Time"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Invoice_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Invoice_Time"].ToString());
                    }

                    data.Columns.Remove("Invoice_Date");
                    data.Columns.Remove("Invoice_Time");
                }
                else
                {
                    OrdinaryVATDesktop.DataTable_DateFormat(data, "Invoice_Date_Time");
                    if (data.Columns.Contains("Delivery_Date_Time"))
                    {
                        OrdinaryVATDesktop.DataTable_DateFormat(data, "Delivery_Date_Time");
                    }
                }

                #endregion

                #region ImportBigData

                result = ImportBigData(data, true, connection, transaction, null, branchId, null, false, null, null, paramVM);

                #endregion

                #region Check UOMc

                var uomcText = @"select top 1 * from SalesTempData  where UOMc = 0 or UOMc  is null";
                SqlCommand uomCmd = new SqlCommand(uomcText, connection, transaction);
                SqlDataAdapter uomAdapter = new SqlDataAdapter(uomCmd);
                DataTable uomcTable = new DataTable();

                uomAdapter.Fill(uomcTable);

                if (uomcTable.Rows.Count > 0)
                {
                    throw new Exception("UOM Conversion not found for " + uomcTable.Rows[0]["Item_Code"] + "-" +
                                        uomcTable.Rows[0]["Item_Name"]);
                }

                #endregion

                #region Sale Type Check

                string saleType = @"select distinct top 1  ID 
                        from SalesTempData where Sale_Type not in  ('credit','new','debit')";
                SqlCommand saleTypeCmd = new SqlCommand(saleType, connection, transaction);
                SqlDataAdapter saleTypedapter = new SqlDataAdapter(saleTypeCmd);
                DataTable saleTypeTable = new DataTable();

                saleTypedapter.Fill(saleTypeTable);


                if (saleTypeTable.Rows.Count > 0)
                {
                    throw new Exception("Invalid Sale Type for ID " + saleTypeTable.Rows[0]["ID"]);
                }

                #endregion

                #region If ImportBigData Success

                if (result[0].ToLower() == "success")
                {
                    callBack();

                    #region Save InvoiceId SaleTemp

                    result = SaveInvoiceIdSaleTemp(0, connection, transaction);

                    #endregion

                    #region Comments on Jul-09-2020

                    ////// insert into SalesTempData2
                    ////// find distinct split amount
                    ////// delete from SalesTempData
                    ////// create #temp table 
                    ////// start loop
                    ////// insert top distinct portion to #temp
                    ////// insert to saleTempdata of top portion
                    ////// update saletempdata2 isLoaded
                    ////// delete from #temp
                    ////// delete from #SaleTempData
                    ////// Other proccess
                    //////end loop
                    ////// drop #temp
                    ////// delete from SalesTempdata1 adn reset identity
                    //////sqlText = "delete from SalesTempData; DBCC CHECKIDENT ('SalesTempData', RESEED, 0);";
                    //////for (int i = 0; i < 10; i++)
                    //////{
                    //////}

                    //////tempCommand.CommandText = getTopData;
                    //////int topData = tempCommand.ExecuteNonQuery();

                    //////tempCommand.CommandText = inserToSaleTemp1;
                    //////tempCommand.ExecuteNonQuery();

                    //////tempCommand.CommandText = updateTemp2;
                    //////tempCommand.ExecuteNonQuery();

                    #endregion

                    callBack();

                    #region Code Generation For Sale

                    string sqlText = "";
                    sqlText = @"select * from SalesTempData order by Id ";
                    SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);
                    cmd.CommandTimeout = 500;

                    DataTable codeGen = new DataTable();

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(codeGen);

                    CodeGenerationForSale(codeGen, connection, transaction);

                    #endregion

                    CommonDAL commonDAl = new CommonDAL();

                    #region Delete and Bulk Insert to SalesTempData

                    sqlText = "delete from SalesTempData; DBCC CHECKIDENT ('SalesTempData', RESEED, 0);";

                    cmd.CommandText = sqlText;
                    transResult = cmd.ExecuteNonQuery();

                    result = commonDal.BulkInsert("SalesTempData", codeGen, connection, transaction);
                    codeGen.Clear();

                    #endregion

                    bool TradingWithSale = new CommonDAL().settings("Trading", "TradingWithSale", connection, transaction).ToString().ToLower() == "y";
                    //transaction.Commit();
                    #region If Bulk Insert to SalesTempData Table Success
                    if (result[0].ToLower() == "success")
                    {
                        #region GetMasterData and ImportSalesBigData

                        var masters = GetMasterData(connection, transaction, app);

                        //debug

                        //var dt = masters.Select("SalesInvoiceNo = 'INV-0820/1019'").CopyToDataTable();


                        result = ImportSalesBigData(masters, true, connection, transaction);

                        #endregion

                        callBack();

                        #region If ImportSalesBigData Success
                        if (result[0].ToLower() == "success")
                        {
                            #region GetDetailsData and BulkInsert to SalesInvoiceDetails

                            masters.Clear();
                            var details = GetDetailsData(connection, transaction);

                            #region Code Debug

                            //////if (true)
                            //////{

                            //////    details.Rows[0]["InvoiceDateTime"] = "2020-Jul-01";
                            //////    details.Rows[0]["CreatedOn"] = "2020-Jul-01";
                            //////    details.Rows[0]["CConversionDate"] = "2020-Jul-01";
                            //////    details.Rows[0]["PreviousInvoiceDateTime"] = "2020-Jul-01";

                            //////}
                            ///

                            string columnNames = "";

                            foreach (DataColumn cl in details.Columns)
                            {
                                columnNames += cl.ColumnName + ",";
                            }

                            #endregion

                            result = commonDal.BulkInsert("SalesInvoiceDetails", details, connection, transaction);

                            #endregion

                            #region If BulkInsert to SalesInvoiceDetails Table Success

                            if (result[0].ToLower() == "success")
                            {
                                callBack();

                                #region Update SalesInvoiceDetails ProductDescription

                                details.Clear();
                                sqlText = "";
                                sqlText += " update SalesInvoiceDetails set ProductDescription=Comments where ProductDescription in('0','','-')";

                                SqlCommand cmdUpdate = new SqlCommand(sqlText, connection);
                                cmdUpdate.Transaction = transaction;

                                transResult = cmdUpdate.ExecuteNonQuery();

                                #endregion

                                #region TradingWithSale

                                if (TradingWithSale)
                                {
                                    #region Get Receive & Issue Master Data

                                    var receiveMasters = GetReceiveMasterData(connection, transaction, app);
                                    var issueMaster = GetIssueMasterData(connection, transaction, app);

                                    #endregion

                                    #region BulkInsert to Receive

                                    result = commonDal.BulkInsert("ReceiveHeaders", receiveMasters, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var receiveDetails = GetReceiveDetailsData(connection, transaction);
                                        string ids = "";

                                        foreach (DataRow row in receiveDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            row["VATName"] = bomDt.Rows[0]["VATName"];

                                            ids += "'" + row["ReceiveNo"] + "',";
                                        }


                                        result = commonDal.BulkInsert("ReceiveDetails", receiveDetails, connection, transaction);
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                            sqlText += " where ReceiveHeaders.ReceiveNo in ( " + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            transResult = cmd.ExecuteNonQuery();
                                        }
                                    }

                                    #endregion

                                    #region BulkInsert to Issue

                                    result = commonDal.BulkInsert("IssueHeaders", issueMaster, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var IssueDetails = GetIssueDetailsData(connection, transaction);
                                        string ids = "";

                                        ProductDAL productDal = new ProductDAL();

                                        foreach (DataRow row in IssueDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            DataTable avgPrice = productDal.AvgPriceNew(row["ItemNo"].ToString().Trim(),
                                                Convert.ToDateTime(row["IssueDateTime"]).ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:00"), connection, transaction, true, true, false, false, null,UserId);

                                            decimal Amount = Convert.ToDecimal(avgPrice.Rows[0]["Amount"].ToString());
                                            decimal Quantity = Convert.ToDecimal(avgPrice.Rows[0]["Quantity"].ToString());
                                            decimal costPrice = 0;
                                            if (Quantity > 0)
                                            {
                                                costPrice = Amount / Quantity;
                                            }

                                            row["NBRPrice"] = costPrice;
                                            row["CostPrice"] = costPrice;
                                            row["SubTotal"] = costPrice * Convert.ToDecimal(row["Quantity"]);
                                            row["UOMPrice"] = costPrice * Convert.ToDecimal(row["UOMc"]);

                                            row["BOMDate"] = Convert.ToDateTime(bomDt.Rows[0]["EffectDate"]).ToString("MM/dd/yyyy");

                                            ids += "'" + row["IssueNo"] + "',";

                                        }

                                        result = commonDal.BulkInsert("IssueDetails", IssueDetails, connection, transaction);
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText = "";
                                            sqlText += " update IssueHeaders set ";
                                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                            sqlText += " where IssueHeaders.IssueNo in (" + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            transResult = cmd.ExecuteNonQuery();
                                        }
                                    }
                                    #endregion

                                }

                                #endregion

                                #region Comments on Jul-09-2020 / Stock Check

                                //cmd.CommandText = "select Distinct SalesInvoiceNo,Invoice_Date_Time from SalesTempData where Post = 'Y'";
                                //uomcTable = new DataTable();
                                //adapter.SelectCommand = cmd;

                                //adapter.Fill(uomcTable);


                                //foreach (DataRow row in uomcTable.Rows)
                                //{
                                //    SalesPost(new SaleMasterVM()
                                //        {
                                //            SalesInvoiceNo = row["SalesInvoiceNo"].ToString(),
                                //            InvoiceDateTime = row["Invoice_Date_Time"].ToString()
                                //        }, new List<SaleDetailVM>(), new List<TrackingVM>(), transaction, connection,
                                //        null,
                                //        true);
                                //}

                                #endregion

                            }
                            else
                            {
                                throw new Exception("Details Import Failed");
                            }
                            #endregion

                            #region Update Sales PeriodId

                            cmd.CommandText = @"update  SalesInvoiceHeaders                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = ''

update  SalesInvoiceDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = '' 

update SalesInvoiceHeaders set FiscalYear = FiscalYear.CurrentYear
from FiscalYear where SalesInvoiceHeaders.PeriodID = FiscalYear.PeriodID
and SalesInvoiceHeaders.FiscalYear is null

";


                            transResult = cmd.ExecuteNonQuery();

                            #endregion

                            #region Comments on Jul-09-2020

                            //commonDal.Update_PeriodId(connection, transaction);

                            #endregion

                        }
                        else
                        {
                            throw new Exception("Master Import Failed");
                        }
                        #endregion

                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }

                    #endregion

                    #region Comments on Jul-09-2020

                    //tempCommand.CommandText = deletehashTemp + " " + deleteSaleTemp;
                    //tempCommand.ExecuteNonQuery();
                    //tempCommand.CommandText = drophashTemp;
                    //tempCommand.ExecuteNonQuery();

                    #endregion

                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }
                #endregion

                #region Commit

                if (result[0].ToLower() == "success" && vTransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #region Catch and Finally

            catch (Exception e)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open && vConnection == null)
                {
                    connection.Close();

                }

            }
            #endregion

            return result;
        }


        /*
         * This method inserts to SalesInvoiceHeaderMaster temp table, which is used for
         * preview and post for big data set
         */
        public ResultVM BulkInsertMasterTemp(DataTable masterData, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            ResultVM resultVm = new ResultVM();
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


                #region Delete Master Temp

                string deleteSqlText = @"delete from SalesInvoiceHeaderMasterTemp";
                SqlCommand cmd = new SqlCommand(deleteSqlText, currConn, transaction);
                cmd.ExecuteNonQuery();

                #endregion


                CommonDAL commonDal = new CommonDAL();

                string[] result = commonDal.BulkInsert("SalesInvoiceHeaderMasterTemp", masterData, currConn, transaction);


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

                resultVm.Status = "Success";
                resultVm.Message = MessageVM.PurchasemsgSaveSuccessfully;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (Exception ex)
            {

                resultVm.Status = "Fail";
                resultVm.Exception = ex.Message;

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
            return resultVm;
            #endregion Result
        }



        #endregion

        #region Method 01

        public DataTable SearchSaleExportDTNew(string SalesInvoiceNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {

            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("SearchExport");

            #endregion

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT SaleLineNo,
                                Description,
                                Quantity,
                                GrossWeight, NetWeight,
                                NumberFrom, NumberTo,
                                Comments
                                FROM SalesInvoiceHeadersExport
                                WHERE (SalesInvoiceNo = @SalesInvoiceNo)";

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                if (!objCommSaleDetail.Parameters.Contains("@SalesInvoiceNo"))
                { objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceNo", SalesInvoiceNo); }
                else { objCommSaleDetail.Parameters["@SalesInvoiceNo"].Value = SalesInvoiceNo; }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
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

        public DataTable SearchSalesHeaderDTNew(string SalesInvoiceNo, string ImportIDExcel = "", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesHeader");

            #endregion

            #region Try
            try
            {

                string[] cValues = { SalesInvoiceNo };
                string[] cFields = { "sih.SalesInvoiceNo" };
                if (!string.IsNullOrWhiteSpace(ImportIDExcel))
                {
                    cValues = new[] { ImportIDExcel };
                    cFields = new[] { "sih.ImportIDExcel" };
                }


                dataTable = SelectAll(0, cFields, cValues, null, null, null, true);

            }
            #endregion

            #region Catch & Finally

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
            finally
            {

            }

            #endregion

            return dataTable;

        }

        public DataTable SearchSalesHeaderDTNewMultipleExport(string SalesInvoiceNo, string CustomerName, string CustomerGroupName, string VehicleType, string VehicleNo, string SerialNo, string InvoiceDateFrom, string InvoiceDateTo, string SaleType, string Trading, string IsPrint, string transactionType, string Post, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesHeader");

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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountAmount", "decimal(25, 9)", currConn);
                //commonDal.TableFieldAdd("SalesInvoiceDetails", "DiscountedNBRPrice", "decimal(25, 9)", currConn);
                #endregion open connection and transaction

                #region SQL Statement



                sqlText = "";
                //sqlText += " insert into IssueHeaders(";
                sqlText += " SELECT ";
                sqlText += "isnull(SalesInvoiceHeaders.ShiftId,1)ShiftId, ";
                sqlText += " SalesInvoiceHeaders.SalesInvoiceNo, ";
                sqlText += " SalesInvoiceHeaders.CustomerID,";
                sqlText += " isnull(Customers.CustomerName,'N/A')CustomerName,";
                sqlText += " isnull(CustomerGroups.CustomerGroupName,'N/A')CustomerGroupName,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress1,'N/A')DeliveryAddress1,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress2,'N/A')DeliveryAddress2,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryAddress3,'N/A')DeliveryAddress3,";
                sqlText += "    isnull(Customers.VATRegistrationNo,'0')  VATRegistrationNo,";
                sqlText += " isnull(SalesInvoiceHeaders.LCNumber,'N/A')LCNumber,";
                sqlText += " isnull(SalesInvoiceHeaders.LCDate,'1900/01/01')LCDate,";
                sqlText += " isnull(SalesInvoiceHeaders.LCBank,'N/A')LCBank,";
                sqlText += " isnull(SalesInvoiceHeaders.SignatoryName,'N/A')SignatoryName,";
                sqlText += " isnull(SalesInvoiceHeaders.SignatoryDesig,'N/A')SignatoryDesig,";

                sqlText += " SalesInvoiceHeaders.VehicleID,";
                sqlText += " isnull(Vehicles.VehicleType,'N/A')VehicleType,";
                sqlText += " isnull(Vehicles.VehicleNo,'N/A')VehicleNo,";
                sqlText += " isnull(SalesInvoiceHeaders.TotalAmount,0)TotalAmount,";
                sqlText += " isnull(SalesInvoiceHeaders.TotalVATAmount,0)TotalVATAmount,";
                sqlText += " isnull(SalesInvoiceHeaders.SerialNo,'N/A')SerialNo,";
                sqlText += " convert (varchar,SalesInvoiceHeaders.InvoiceDateTime,120)InvoiceDate,";
                sqlText += " convert (varchar,SalesInvoiceHeaders.DeliveryDate,120)DeliveryDate,";
                sqlText += " isnull(SalesInvoiceHeaders.Comments ,'N/A')Comments,";
                sqlText += " SaleType,isnull(PreviousSalesInvoiceNo,SalesInvoiceHeaders.SalesInvoiceNo) PID,";
                sqlText += " Trading,IsPrint,";
                sqlText += " SalesInvoiceHeaders.TenderId,";
                sqlText += " isnull(SalesInvoiceHeaders.transactionType,'NA')transactionType,";
                sqlText += " isnull(SalesInvoiceHeaders.CurrencyID,'260')CurrencyID,";
                sqlText += " isnull(cur.CurrencyCode,'BDT')CurrencyCode,";
                sqlText += " isnull(SalesInvoiceHeaders.CurrencyRateFromBDT,1)CurrencyRateFromBDT,";
                sqlText += " isnull(SalesInvoiceHeaders.AlReadyPrint,0)AlReadyPrint,";
                sqlText += " isnull(SalesInvoiceHeaders.SaleReturnId,0)SaleReturnId,";
                sqlText += " isnull(SalesInvoiceHeaders.DeliveryChallanNo,'N')DeliveryChallan,";
                sqlText += " isnull(SalesInvoiceHeaders.IsGatePass,'N')IsGatePass,";
                sqlText += " isnull(SalesInvoiceHeaders.Post,'N')Post,";
                sqlText += " isnull(SalesInvoiceHeaders.ImportIDExcel,'')ImportID";
                sqlText += " FROM  SalesInvoiceHeaders LEFT OUTER JOIN";
                sqlText += " Customers ON  SalesInvoiceHeaders.CustomerID =  Customers.CustomerID LEFT OUTER JOIN";
                sqlText += " CustomerGroups ON  Customers.CustomerGroupID =  CustomerGroups.CustomerGroupID LEFT OUTER JOIN";
                sqlText += " Vehicles ON  SalesInvoiceHeaders.VehicleID =  Vehicles.VehicleID  LEFT OUTER JOIN ";
                sqlText += " Currencies cur ON isnull(SalesInvoiceHeaders.CurrencyID,'260')=cur.CurrencyId";
                sqlText += " WHERE ";
                sqlText += " (SalesInvoiceHeaders.SalesInvoiceNo LIKE '%' +'" + SalesInvoiceNo + "' + '%' OR SalesInvoiceHeaders.SalesInvoiceNo IS NULL)  ";
                //sqlText += " AND (SalesInvoiceHeaders.CustomerID LIKE '%' + '" + CustomerID + "' + '%' OR '" + CustomerID + "' IS NULL) ";
                sqlText += " AND (Customers.CustomerName LIKE '%' + '" + CustomerName + "' + '%' OR Customers.CustomerName IS NULL)  ";
                //sqlText += " AND (Customers.CustomerGroupID LIKE '%' +'" + CustomerGroupID + "' + '%' OR '" + CustomerGroupID + "' IS NULL) ";
                sqlText += " AND (CustomerGroups.CustomerGroupName LIKE '%' + '" + CustomerGroupName + "' + '%' OR customerGroups.CustomerGroupName IS NULL)  ";
                //sqlText += " AND (SalesInvoiceHeaders.VehicleID LIKE '%' + '" + VehicleID + "' + '%' OR '" + VehicleID + "' IS NULL)  ";
                //sqlText += " AND (Vehicles.VehicleType LIKE '%' + '" + VehicleType + "' + '%' OR '" + VehicleType + "' IS NULL)  ";
                sqlText += " AND (Vehicles.VehicleNo LIKE '%' + '" + VehicleNo + "' + '%' OR Vehicles.VehicleNo IS NULL)  ";
                sqlText += " AND (SalesInvoiceHeaders.SerialNo LIKE '%' + '" + SerialNo + "' + '%' OR SalesInvoiceHeaders.SerialNo IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.InvoiceDateTime>= '" + InvoiceDateFrom + "' ) ";
                sqlText += " AND (SalesInvoiceHeaders.InvoiceDateTime<dateadd(d,1, '" + InvoiceDateTo + "') ) ";
                sqlText += " AND (SalesInvoiceHeaders.SaleType LIKE '%' + '" + SaleType + "' + '%' OR SalesInvoiceHeaders.SaleType IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.Trading LIKE '%' + '" + Trading + "' + '%' OR SalesInvoiceHeaders.Trading IS NULL) ";
                sqlText += " AND (SalesInvoiceHeaders.IsPrint LIKE '%' + '" + IsPrint + "' + '%' OR SalesInvoiceHeaders.IsPrint IS NULL) ";
                if (transactionType != "All")
                {
                    if (transactionType == "ExportServiceNS")
                    {
                        sqlText += " AND (SalesInvoiceHeaders.transactionType in('ExportServiceNS','ExportServiceNSCredit')) ";

                    }
                    else
                    {
                        sqlText += " AND (SalesInvoiceHeaders.transactionType in('" + transactionType + "')) ";
                    }
                }
                sqlText += " AND (SalesInvoiceHeaders.Post LIKE '%' + '" + Post + "' + '%' OR SalesInvoiceHeaders.Post IS NULL) ";

                sqlText += " and SalesInvoiceHeaders.SalesInvoiceNo not in( select SalesInvoiceNo from SaleExportInvoices)";

                sqlText += " order by InvoiceDateTime desc";

                #endregion

                #region SQL Command

                SqlCommand objCommSalesHeader = new SqlCommand();
                objCommSalesHeader.Connection = currConn;

                objCommSalesHeader.CommandText = sqlText;
                objCommSalesHeader.CommandType = CommandType.Text;

                #endregion



                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSalesHeader);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

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

        public DataTable SearchSaleDetailDTNew(string SalesInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesDetail");

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
sd.SalesInvoiceNo
,ISNULL(sd.SourcePaidQuantity,0)  SourcePaidQuantity
,ISNULL(sd.SourcePaidVATAmount,0) SourcePaidVATAmount
,ISNULL(sd.NBRPriceInclusiveVAT,0) NBRPriceInclusiveVAT
,ISNULL(bom.ReferenceNo,'NA') BOMReferenceNo
,ISNULL(sd.BOMId,0) BOMId
,sd.InvoiceLineNo
,sd.ItemNo
,isnull(sd.Quantity,0)Quantity                           
,isnull(sd.PromotionalQuantity,0)PromotionalQuantity
,isnull(isnull(sd.Quantity,0)-isnull(sd.PromotionalQuantity,0),0)SaleQuantity                                                     
,isnull(sd.SalesPrice,0)SalesPrice
,isnull(sd.NBRPrice,0)NBRPrice
,isnull(sd.UOM,'N/A')UOM
,isnull(sd.VATRate,0)VATRate
,isnull(sd.VATAmount,0)VATAmount
,isnull(nullif(sd.ValueOnly,''),'N')ValueOnly
,isnull(sd.SubTotal,0)SubTotal
,isnull(sd.Comments,'N/A')Comments
,isnull(Products.ProductName,'N/A')ProductName
,isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock
,isnull(sd.SD,0)SD
,isnull(sd.SDAmount,0)SDAmount
,ISNULL(sd.VDSAmount,0) VDSAmount  
,isnull(SaleType,'VAT')
,isnull(sd.PreviousSalesInvoiceNo,sd.SalesInvoiceNo)PreviousSalesInvoiceNo
,isnull(sd.Trading,'N')Trading
,isnull(sd.NonStock,'N')NonStock
,isnull(sd.tradingMarkup,0)tradingMarkup
,isnull(sd.Type,'Type')Type
,isnull(Products.ProductCode,'N/A')ProductCode
,isnull(sd.UOMQty,0)UOMQty
,isnull(sd.UOMn,sd.UOM)UOMn
,isnull(sd.UOMc,0)UOMc
,isnull(sd.UOMPrice,0)UOMPrice
,isnull(nullif(sd.VATName,''),'NA')VATName
,isnull(sd.DiscountAmount,0)DiscountAmount
,case when isnull(sd.DiscountedNBRPrice,0) >0 then isnull(sd.DiscountedNBRPrice,0) else isnull(sd.NBRPrice,0) end DiscountedNBRPrice
,isnull(sd.CurrencyValue,sd.SubTotal)CurrencyValue
,isnull(sd.DollerValue,0)DollerValue
,isnull(sd.ReturnTransactionType,'')ReturnTransactionType
,isnull(NULLIF(sd.Weight,'0'),'0')Weight
,isnull(sd.WareHouseRent,0)WareHouseRent
,isnull(sd.WareHouseVAT,0)WareHouseVAT
,isnull(sd.ATVRate,0)ATVRate
,isnull(sd.ATVablePrice,0)ATVablePrice
,isnull(sd.ATVAmount,0)ATVAmount
,isnull(sd.TotalValue,0)TotalValue
,isnull(sd.TradeVATRate,0)TradeVATRate
,isnull(sd.TradeVATableValue,0)TradeVATableValue
,isnull(sd.TradeVATAmount,0)TradeVATAmount
,isnull(sd.IsCommercialImporter,'N')IsCommercialImporter
,convert (varchar,isnull(sd.CConversionDate,'01/01/1900'),120) CConversionDate
,isnull(sd.CDNVATAmount ,0)CDNVATAmount
,isnull(sd.CDNSDAmount ,0)CDNSDAmount
,isnull(sd.CDNSubtotal ,0)CDNSubtotal
,isnull(sd.ProductDescription ,'N/A')ProductDescription
,isnull(sd.IsFixedVAT ,'N')IsFixedVAT
,isnull(sd.FixedVATAmount ,'0')FixedVATAmount
,isnull(sd.PreviousNBRPrice ,'0')PreviousNBRPrice
,isnull(sd.PreviousQuantity ,'0')PreviousQuantity
,isnull(sd.PreviousUOM ,'0')PreviousUOM
,isnull(sd.PreviousSubTotal ,'0')PreviousSubTotal
,isnull(sd.PreviousVATRate ,'0')PreviousVATRate
,isnull(sd.PreviousVATAmount ,'0')PreviousVATAmount
,isnull(sd.PreviousSD ,'0')PreviousSD
,isnull(sd.PreviousSDAmount ,'0')PreviousSDAmount
,convert (varchar,isnull(sd.PreviousInvoiceDateTime,'01/01/2020'),120) PreviousInvoiceDateTime
,isnull(sd.ReasonOfReturn ,'N/A')ReasonOfReturn

,isnull(sd.HPSRate ,'0')HPSRate
,isnull(sd.HPSAmount ,'0')HPSAmount
                            

FROM  SalesInvoiceDetails sd 
LEFT OUTER JOIN Products ON sd.ItemNo = Products.ItemNo  
LEFT OUTER JOIN BOMs bom ON ISNULL(sd.BOMId,0) = bom.BOMId             
WHERE 
(sd.SalesInvoiceNo = @SalesInvoiceNo) 
--order by InvoiceLineNo
                            ";



                string VAT6_3OrderByProduct = new CommonDAL().settings("Reports", "VAT6_3OrderByProduct");


                if (VAT6_3OrderByProduct.ToLower() == "y")
                {
                    sqlText += @"  order by Products.ProductCode";
                }
                else
                {
                    sqlText += @"  order by InvoiceLineNo";

                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;

                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter
                if (!objCommSaleDetail.Parameters.Contains("@SalesInvoiceNo"))
                { objCommSaleDetail.Parameters.AddWithValue("@SalesInvoiceNo", SalesInvoiceNo); }
                else { objCommSaleDetail.Parameters["@SalesInvoiceNo"].Value = SalesInvoiceNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

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

        public DataTable SearchSaleDetailTemp(string SalesInvoiceNo, string userId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("SearchSalesDetail");

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

                //transaction = currConn.BeginTransaction();
                #endregion open connection and transaction

                #region SQL Statement

                sqlText = @"
                           SELECT    
ISNULL(sd.BOMId,0) BOMId
,sd.SalesInvoiceNo
,row_number() OVER (ORDER BY SL)  InvoiceLineNo
,sd.ItemNo
,isnull(sd.Quantity,0)Quantity                           
,isnull(sd.Promotional_Quantity,0)PromotionalQuantity
,isnull(isnull(sd.Quantity,0)-isnull(sd.Promotional_Quantity,0),0)SaleQuantity                                                     
,isnull(sd.SalesPrice,0)SalesPrice
,isnull(sd.NBR_Price,0)NBRPrice
,isnull(sd.UOM,'N/A')UOM
,isnull(sd.VAT_Rate,0)VATRate
,isnull(sd.VAT_Amount,0)VATAmount
,isnull(nullif(sd.ValueOnly,''),'N')ValueOnly
,isnull(sd.SubTotal,0)SubTotal
,isnull(sd.Comments,'N/A')Comments
,isnull(Products.ProductName,'N/A')ProductName
,isnull(isnull(Products.OpeningBalance,0)+isnull(Products.QuantityInHand,0),0) as Stock
,isnull(sd.SD_Rate,0)SD
,isnull(sd.SDAmount,0)SDAmount
,ISNULL(sd.VDSAmount,0) VDSAmount  
,isnull(sd.Sale_Type,'VAT') SaleType
,isnull(sd.Previous_Invoice_No,sd.SalesInvoiceNo)PreviousSalesInvoiceNo
,isnull(sd.Trading,'N')Trading
,isnull(sd.Non_Stock,'N')NonStock
,isnull(sd.Trading_MarkUp,0)tradingMarkup
,isnull(sd.Type,'Type')Type
,isnull(Products.ProductCode,'N/A')ProductCode
,isnull(sd.UOMQty,0)UOMQty
,isnull(sd.UOMn,sd.UOM)UOMn
,isnull(sd.UOMc,0)UOMc
,isnull(sd.UOMPrice,0)UOMPrice
,isnull(nullif(sd.VAT_Name,''),'NA')VATName
,isnull(sd.Discount_Amount,0)DiscountAmount
,case when isnull(sd.Discount_Amount,0) >0 then isnull(sd.Discount_Amount,0) else isnull(sd.NBR_Price,0) end DiscountedNBRPrice
,isnull(sd.CurrencyValue,sd.SubTotal)CurrencyValue
,isnull(sd.DollerValue,0)DollerValue
,'' ReturnTransactionType
,isnull(NULLIF(sd.Weight,'0'),'0')Weight
,isnull(sd.WareHouseRent,0)WareHouseRent
,isnull(sd.WareHouseVAT,0)WareHouseVAT
,isnull(sd.ATVRate,0)ATVRate
,isnull(sd.ATVablePrice,0)ATVablePrice
,isnull(sd.ATVAmount,0)ATVAmount
,isnull(sd.TotalValue,0)TotalValue
,isnull(sd.TradeVATRate,0)TradeVATRate
,isnull(sd.TradeVATableValue,0)TradeVATableValue
,isnull(sd.TradeVATAmount,0)TradeVATAmount
,isnull(sd.IsCommercialImporter,'N')IsCommercialImporter
,convert (varchar,isnull(sd.CConversionDate,'01/01/1900'),120) CConversionDate
,isnull(sd.CDNVATAmount ,0)CDNVATAmount
,isnull(sd.CDNSDAmount ,0)CDNSDAmount
,isnull(sd.CDNSubtotal ,0)CDNSubtotal
,isnull(sd.ProductDescription ,'N/A')ProductDescription
,isnull(sd.IsFixedVAT ,'N')IsFixedVAT
,isnull(sd.FixedVATAmount ,'0')FixedVATAmount

,0 HPSRate
,0 HPSAmount
                            

FROM  SalesTempData sd LEFT OUTER JOIN
Products ON sd.ItemNo = Products.ItemNo               
 
where ID = @Id and UserId = @userId 
--order by InvoiceLineNo
                            ";



                string VAT6_3OrderByProduct = new CommonDAL().settings("Reports", "VAT6_3OrderByProduct");


                if (VAT6_3OrderByProduct.ToLower() == "y")
                {
                    sqlText += @"  order by Products.ProductCode";
                }
                else
                {
                    sqlText += @"  order by InvoiceLineNo";

                }

                #endregion

                #region SQL Command

                SqlCommand objCommSaleDetail = new SqlCommand();
                objCommSaleDetail.Connection = currConn;
                //objCommSaleDetail.Transaction = transaction;

                objCommSaleDetail.CommandText = sqlText;
                objCommSaleDetail.CommandType = CommandType.Text;

                #endregion.

                #region Parameter

                objCommSaleDetail.Parameters.AddWithValue("@Id", SalesInvoiceNo);
                objCommSaleDetail.Parameters.AddWithValue("@userId", userId);

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (Exception ex)
            {
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

            #endregion

            return dataTable;

        }

        public int LoadIssueItems(SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region Variables
            int transResult = 0;

            string sqlText = "";
            DataTable dtIssue = new DataTable("IssueDetails");
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion

            #region Try
            try
            {
                currConn = _dbsqlConnection.GetConnectionNoTimeOut();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region SQL Statement

                sqlText = @"
                            SELECT    
                            IssueNo,
                            ItemNo,
                            IssueDateTime
                            FROM  IssueDetails  
                            where isnull(IsProcess,'N') <>'Y'
                            
                            order by IssueDateTime    
 
                           
                            ";
                #endregion

                #region SQL Command

                SqlCommand objsql = new SqlCommand();
                objsql.Transaction = transaction;

                objsql.Connection = currConn;
                objsql.CommandText = sqlText;
                objsql.CommandType = CommandType.Text;
                SqlDataAdapter dataAdapter1 = new SqlDataAdapter(objsql);
                dataAdapter1.Fill(dtIssue);

                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
                #endregion
                if (dtIssue == null)
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNamePost,
                                                    MessageVM.receiveMsgNoDataToPost);
                }

                else
                {
                    ProductDAL sale = new ProductDAL();
                    string vIssueItem = "";
                    string vIssueNo = "";
                    string vIssueDate;
                    foreach (DataRow IssueItem in dtIssue.Rows)
                    {
                        vIssueDate = Convert.ToDateTime(IssueItem["IssueDateTime"]).ToString("yyyy-MM-dd");
                        vIssueItem = IssueItem["ItemNo"].ToString();
                        vIssueNo = IssueItem["IssueNo"].ToString();
                        DataTable dt = sale.AvgPriceNew(vIssueItem, vIssueDate, null, null, false,true,true,true,null,UserId);
                        decimal Quantity = Convert.ToDecimal(dt.Rows[0]["Quantity"]);
                        decimal Amount = Convert.ToDecimal(dt.Rows[0]["Amount"]);
                        decimal AvgRate = 0;
                        if (Quantity == 0)
                        {
                            AvgRate = 0;
                        }
                        else
                        {
                            AvgRate = Amount / Quantity;
                        }
                        if (AvgRate <= 0)
                        {
                            AvgRate = 0;
                        }
                        UpdatIssueAvgPrice(vIssueNo, vIssueItem, AvgRate);

                    }
                }
            }
            #endregion

            #region Catch & Finally

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

            return transResult;

        }

        public string[] ImportData(DataTable dtSaleM, DataTable dtSaleD, DataTable dtSaleE, bool CommercialImporter = false, int branchId = 0, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region variable
            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SaleMasterVM saleMaster = new SaleMasterVM();
            List<SaleDetailVm> saleDetails = new List<SaleDetailVm>();
            List<SaleExportVM> saleExport = new List<SaleExportVM>();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string VinvoiceNo = "";
            string VitemCode = "";
            string commentsD = "NA";
            decimal VAT_Amount = 0;

            //string cTotalValue = "0";
            //string cWareHouseRent = "0";
            //string cWareHouseVAT = "0";
            //string cATVRate = "0";
            //string cATVablePrice = "0";
            //string cATVAmount = "0";
            //string cIsCommercialImporter = "0";

            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.StatisticsEnabled = true;
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Checking Das ta");
                }
                CommonImportDAL cImport = new CommonImportDAL();
                CommonDAL commonDal = new CommonDAL();
                ProductDAL productDal = new ProductDAL();

                #region RowCount
                int MRowCount = 0;
                int MRow = dtSaleM.Rows.Count;
                DataTable varDt = new DataTable();
                varDt.TableName = "NewSaleM";
                varDt = dtSaleM.Copy();
                var DatabaseName = commonDal.settings("DatabaseName", "DatabaseName");
                var saleExistContinue = commonDal.settings("Import", "SaleExistContinue");
                MRowCount = dtSaleM.Rows.Count;
                for (int i = 0; i < MRow; i++)
                {
                    string importID = varDt.Rows[i]["ID"].ToString().Trim();
                    var exist = cImport.CheckSaleImportIdExist(importID, currConn, transaction);
                    if (exist.ToLower() == "exist")
                    {

                        if (saleExistContinue.ToLower() == "n")
                        {
                            string msg = "Import Id " + importID.ToString() + "Already Exist in Database";
                            throw new ArgumentNullException(msg);
                        }
                        else
                        {
                            //DataRow[] drm = null;//a datarow array
                            //DataRow[] drd = null;//a datarow array
                            //DataRow[] dre = null;//a datarow array
                            //if (dtSaleM != null && dtSaleM.Rows.Count > 0)
                            //{
                            //    drm = dtSaleM.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in drm)
                            //    {
                            //        dtSaleM.Rows.Remove(row);
                            //    }
                            //}
                            //if (dtSaleD != null && dtSaleD.Rows.Count > 0)
                            //{
                            //    drd = dtSaleD.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in drd)
                            //    {
                            //        dtSaleD.Rows.Remove(row);
                            //    }
                            //}
                            //if (dtSaleE != null && dtSaleE.Rows.Count > 0)
                            //{
                            //    dre = dtSaleE.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in dre)
                            //    {
                            //        dtSaleE.Rows.Remove(row);
                            //    }
                            //}
                            //UpdateIsProcessed(1, importID);
                            retResults[0] = "success";
                            retResults[1] = "Import id already exists";
                            return retResults;

                            //loop throw the array and deete those rows from datatable

                            //loop throw the array and deete those rows from datatable
                        }

                    }
                    else
                    {
                        var tt = importID;
                    }

                }
                if (dtSaleM.Rows.Count <= 0)
                {
                    retResults[0] = "Information";
                    retResults[1] = "You do Not Have Data to Import";
                    throw new ArgumentNullException(retResults[1]);

                }
                //for (int i = 0; i < dtSaleM.Rows.Count; i++)
                //{
                //    string importID = dtSaleM.Rows[i]["ID"].ToString();
                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        MRowCount++;
                //    }

                //}

                #endregion RowCount

                #region ID in Master or Detail table

                //// in details check
                //for (int i = 0; i < MRowCount; i++)
                //{
                //    string importID = dtSaleM.Rows[i]["ID"].ToString();

                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        DataRow[] DetailRaws = dtSaleD.Select("ID='" + importID + "'");
                //        if (DetailRaws.Length == 0)
                //        {
                //            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                //        }

                //    }

                //}
                //// in master check
                //for (int i = 0; i < dtSaleD.Rows.Count; i++)
                //{
                //    string importID = dtSaleD.Rows[i]["ID"].ToString();

                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        DataRow[] DetailRaws = dtSaleM.Select("ID='" + importID + "'");
                //        if (DetailRaws.Length == 0)
                //        {
                //            throw new ArgumentNullException("The ID " + importID + " is not available in master table");
                //        }

                //    }

                //}

                #endregion

                #region Double ID in Master

                //for (int i = 0; i < MRowCount; i++)
                //{
                //    string id = dtSaleM.Rows[i]["ID"].ToString();
                //    DataRow[] tt = dtSaleM.Select("ID='" + id + "'");
                //    if (tt.Length > 1)
                //    {
                //        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                //    }

                //}

                #endregion Double ID in Master


                #region Read from settings
                string vPriceDeclaration = commonDal.settings("Sale", "PriceDeclarationForImport");
                bool IsPriceDeclaration = Convert.ToBoolean(vPriceDeclaration == "Y" ? true : false);
                string vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                bool isNegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                string DefaultProductCategory1 = commonDal.settings("AutoSave", "DefaultProductCategory");
                string DefaultCustomerGroup = commonDal.settings("AutoSave", "DefaultCustomerGroup");
                string DefaultProductCategoryId = "";
                string DefaultCustomerGroupId = "";

                #endregion

                string conversionDate = DateTime.Now.ToString("yyyy-MM-dd");

                #region Find Master Column for Sanofi

                bool IsCompInvoiceNo = false;
                for (int i = 0; i < dtSaleM.Columns.Count; i++)
                {
                    if (dtSaleM.Columns[i].ColumnName.ToString() == "Comp_Invoice_No")
                    {
                        IsCompInvoiceNo = true;
                    }
                }
                ////Find details column for CP
                bool IsColWeight = false;
                for (int i = 0; i < dtSaleD.Columns.Count; i++)
                {
                    if (dtSaleD.Columns[i].ColumnName.ToString() == "Weight")
                    {
                        IsColWeight = true;
                    }
                }

                #endregion
                #region checking from database is exist the information(NULL Check)
                #region Master
                //string CurrencyId = string.Empty;
                //string USDCurrencyId = string.Empty;

                for (int i = 0; i < MRowCount; i++)
                {

                    //var ImportId = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    //CurrencyId = string.Empty;
                    //USDCurrencyId = string.Empty;
                    #region Master
                    #region FindCustomerId
                    //cImport.FindCustomerId(dtSaleM.Rows[i]["Customer_Name"].ToString().Trim(), dtSaleM.Rows[i]["Customer_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindCustomerId

                    #region FindCurrencyId
                    //CurrencyId = cImport.FindCurrencyId(dtSaleM.Rows[i]["Currency_Code"].ToString().Trim(), currConn, transaction);
                    //USDCurrencyId = cImport.FindCurrencyId("USD", currConn, transaction);
                    //cImport.FindCurrencyRateFromBDT(CurrencyId, currConn, transaction);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId,conversionDate, currConn, transaction);

                    #endregion FindCurrencyId

                    #region FindTenderId
                    //cImport.FindTenderId(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion FindTenderId

                    #region Checking Date is null or different formate
                    bool IsInvoiceDate;
                    IsInvoiceDate = cImport.CheckDate(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim());
                    if (IsInvoiceDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date_Time field.");
                    }
                    bool IsDeliveryDate;
                    IsDeliveryDate = cImport.CheckDate(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim());
                    if (IsDeliveryDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Delivery_Date_Time field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    bool isPrint;
                    post = cImport.CheckYN(dtSaleM.Rows[i]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    isPrint = cImport.CheckYN(dtSaleM.Rows[i]["Is_Print"].ToString().Trim());
                    if (isPrint != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Is_Print field.");
                    }
                    #endregion Checking Y/N value

                    #region Check previous invoice id
                    //string PreInvoiceId = string.Empty;
                    //string TenderId = string.Empty;

                    //PreInvoiceId = cImport.CheckPreInvoiceID(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    //TenderId = cImport.CheckTenderID(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice id

                    #region Check LC Number
                    if (dtSaleM.Rows[i]["Transection_Type"].ToString().Trim() == "Export")
                    {

                        string LCNumber = string.Empty;
                        DataRow[] ExportRaws = dtSaleE.Select("ID='" + dtSaleM.Rows[i]["ID"].ToString().Trim() + "'");
                        if (ExportRaws.Length > 0)
                        {
                            LCNumber = dtSaleM.Rows[i]["LC_Number"].ToString().Trim();
                            if (string.IsNullOrEmpty(LCNumber) || LCNumber == "0")
                            {
                                throw new ArgumentNullException("Please insert value in LC_Number field.");
                            }
                        }
                    }
                    #endregion  Check LC Number

                    #endregion Master

                }
                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = dtSaleD.Rows.Count;
                //for (int i = 0; i < dtSaleD.Rows.Count; i++)
                //{
                //    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["ID"].ToString()))
                //    {
                //        DRowCount++;
                //    }

                //}
                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    VitemCode = string.Empty;
                    string UOMn = string.Empty;
                    string UOMc = string.Empty;
                    bool IsQuantity, IsNbrPrice, IsTrading, IsSDRate, IsVatRate, IsDiscount, IsPromoQuantity;


                    #region FindItemId
                    if (string.IsNullOrEmpty(dtSaleD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    bool ItemAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct") == "Y" ? true : false);
                    // db call
                    VitemCode = dtSaleD.Rows[i]["ItemNo"].ToString().Trim();
                    //cImport.FindItemId(dtSaleD.Rows[i]["Item_Name"].ToString().Trim()
                    //         , dtSaleD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction,
                    //         ItemAutoSave, dtSaleD.Rows[i]["UOM"].ToString().Trim());

                    #endregion FindItemId
                    //db call
                    #region FindUOMn
                    UOMn = cImport.FindUOMn(VitemCode, currConn, transaction);
                    #endregion FindUOMn
                    #region FindUOMc
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        UOMc = cImport.FindUOMc(UOMn, dtSaleD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMc

                    #region FindLastNBRPrice

                    DataRow[] vmaster; //= new DataRow[];//

                    string nbrPrice = string.Empty;
                    var transactionDate = "";
                    vmaster = dtSaleM.Select("ID='" + dtSaleD.Rows[i]["ID"].ToString().Trim() + "'");
                    foreach (DataRow row in vmaster)
                    {
                        var tt = Convert.ToDateTime(row["Invoice_Date_Time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss").Trim();
                        transactionDate = tt;
                    }
                    if (IsPriceDeclaration == true)
                    {
                        nbrPrice = cImport.FindLastNBRPrice(VitemCode, dtSaleD.Rows[i]["VAT_Name"].ToString().Trim(),
                            transactionDate, currConn, transaction);
                        if (Convert.ToDecimal(nbrPrice) == 0)
                        {

                            if (vmaster[0]["Transection_Type"].ToString() != "ExportService"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "Service"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                               )
                            {
                                throw new ArgumentNullException("Price declaration of item('" +
                                                dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "') not find in database");
                            }
                        }
                    }

                    #endregion FindLastNBRPrice
                    #region VATName
                    cImport.FindVatName(dtSaleD.Rows[i]["VAT_Name"].ToString().Trim());
                    #endregion VATName
                    #region Numeric value check

                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["Quantity"].ToString().Trim())
                        && !string.IsNullOrEmpty(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()))
                    {
                        if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) == 0
                            && Convert.ToDecimal(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()) == 0)
                        {
                            throw new ArgumentNullException("Please insert quantity value in ID: " + dtSaleD.Rows[i]["ID"].ToString().Trim() + ", '" +
                                      dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "'('" + dtSaleD.Rows[i]["Item_Code"].ToString().Trim() + "').");
                        }
                    }


                    IsQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["Total_Price"].ToString().Trim());
                    }
                    else
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["NBR_Price"].ToString().Trim());
                    }
                    if (IsNbrPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in NBR_Price field.");
                    }
                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim()))
                    {
                        IsVatRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim());
                        if (IsVatRate != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in VAT_Rate field.");
                        }
                    }

                    IsSDRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["SD_Rate"].ToString().Trim());
                    if (IsSDRate != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in SD_Rate field.");
                    }
                    IsTrading = cImport.CheckNumericBool(dtSaleD.Rows[i]["Trading_MarkUp"].ToString().Trim());
                    if (IsTrading != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Trading_MarkUp field.");
                    }
                    IsDiscount = cImport.CheckNumericBool(dtSaleD.Rows[i]["Discount_Amount"].ToString().Trim());
                    if (IsDiscount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Discount_Amount field.");
                    }
                    IsPromoQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim());
                    if (IsPromoQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Promotional_Quantity field.");
                    }
                    #endregion Numeric value check

                    #region Checking Y/N value
                    bool NonStock;
                    NonStock = cImport.CheckYN(dtSaleD.Rows[i]["Non_Stock"].ToString().Trim());
                    if (NonStock != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Non_Stock field.");
                    }
                    #endregion Checking Y/N value


                    #region Check Stock

                    string quantityInHand = productDal.AvgPriceNew(VitemCode, transactionDate, currConn, transaction, false,true,true,true,null,UserId).Rows[0]["Quantity"].ToString();

                    string tenderStock = "0";//"0,0.0000");

                    decimal minValue = 0;
                    if (Convert.ToDecimal(quantityInHand) < Convert.ToDecimal(tenderStock))
                    {
                        minValue = Convert.ToDecimal(quantityInHand);
                    }
                    else
                    {
                        minValue = Convert.ToDecimal(tenderStock);

                    }
                    if (vmaster[0]["Transection_Type"].ToString() == "Tender" || vmaster[0]["Transection_Type"].ToString() == "TradingTender")
                    {
                        if (Convert.ToDecimal(quantityInHand) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(minValue))
                        {
                            throw new ArgumentNullException("Stock Not available for " + VitemCode);
                        }
                    }
                    else if (vmaster[0]["Transection_Type"].ToString() != "Credit"
                                && vmaster[0]["Transection_Type"].ToString() != "VAT11GaGa"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS")
                    {
                        if (isNegStockAllow == false)
                        {
                            if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(quantityInHand))
                            {
                                throw new ArgumentNullException("Stock Not available for " + VitemCode);
                            }
                        }
                    }


                    #endregion Check Stock


                }


                #endregion Details
                #region Export
                if (dtSaleM.Rows.Count > 0 && (dtSaleM.Rows[0]["Transection_Type"].ToString() == "Export"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTrading"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportServiceNS"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportService"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTradingTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportPackage"))
                {

                    #region Row count for export details table
                    int ERowCount = 0;
                    for (int i = 0; i < dtSaleE.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtSaleE.Rows[i]["ID"].ToString()))
                        {
                            ERowCount++;
                        }

                    }
                    #endregion Row count for export details table

                    for (int e = 0; e < ERowCount; e++)
                    {
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Description"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Description field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Quantity"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Quantity field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["GrossWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in GrossWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NetWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NetWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberFrom"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberFrom field.");
                        }
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberTo"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberTo field.");
                        }

                    }
                }

                #endregion Export

                #endregion checking from database is exist the information(NULL Check)


                if (currConn.State == ConnectionState.Open)
                {
                    transaction.Commit();
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data");
                }


                for (int i = 0; i < MRowCount; i++)
                {
                    #region Process model
                    #region Master Sale

                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    string customerName = dtSaleM.Rows[i]["Customer_Name"].ToString().Trim();
                    string customerCode = dtSaleM.Rows[i]["Customer_Code"].ToString().Trim();
                    // Remove DB Process
                    #region FindCustomerId
                    bool CustomerAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleCustomers") == "Y" ? true : false);
                    string customerId = dtSaleM.Rows[i]["CustomerID"].ToString().Trim();//cImport.FindCustomerId(customerName, customerCode, currConn, transaction, CustomerAutoSave);

                    #endregion FindCustomerId

                    #region FindBranchId
                    // Remove DB Process

                    string branchCode = dtSaleM.Rows[i]["Branch_Code"].ToString().Trim();
                    string BranchId = dtSaleM.Rows[i]["BranchId"].ToString().Trim(); //cImport.FindBranchId(branchCode, currConn, transaction);
                    if (!string.IsNullOrWhiteSpace(BranchId))
                    {
                        branchId = Convert.ToInt32(BranchId);
                    }

                    #endregion

                    string deliveryAddress = dtSaleM.Rows[i]["Delivery_Address"].ToString().Trim();
                    string vehicleNo = dtSaleM.Rows[i]["Vehicle_No"].ToString().Trim();
                    //var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    //var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("MM/dd/yyyy HH:mm:ss");
                    var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd");
                    var deliveryDateTime =
                        Convert.ToDateTime(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    #region CheckNull
                    string referenceNo = cImport.ChecKNullValue(dtSaleM.Rows[i]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtSaleM.Rows[i]["Comments"].ToString().Trim());
                    #endregion CheckNull
                    string saleType = dtSaleM.Rows[i]["Sale_Type"].ToString().Trim();
                    #region Check previous invoice no.
                    // Remove DB Process

                    string previousInvoiceNo = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice no.
                    string isPrint = dtSaleM.Rows[i]["Is_Print"].ToString().Trim();
                    #region Check Tender id
                    // Remove DB Process
                    string tenderId = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Tender id
                    string post = dtSaleM.Rows[i]["Post"].ToString().Trim();

                    #region CheckNull
                    string lCNumber = cImport.ChecKNullValue(dtSaleM.Rows[i]["LC_Number"].ToString().Trim());
                    #endregion CheckNull
                    string currencyCode = dtSaleM.Rows[i]["Currency_Code"].ToString().Trim();
                    string createdBy = dtSaleM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtSaleM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtSaleM.Rows[i]["Transection_Type"].ToString().Trim();
                    string compInvoiceNo = null;
                    if (IsCompInvoiceNo == true)
                    {
                        compInvoiceNo = dtSaleM.Rows[i]["Comp_Invoice_No"].ToString().Trim();
                    }



                    #region Master

                    saleMaster = new SaleMasterVM();
                    saleMaster.CustomerID = customerId;
                    saleMaster.DeliveryAddress1 = deliveryAddress;
                    saleMaster.VehicleNo = vehicleNo;
                    saleMaster.InvoiceDateTime = Convert.ToDateTime(invoiceDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.SerialNo = referenceNo;
                    saleMaster.Comments = comments;
                    saleMaster.CreatedBy = createdBy;
                    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.LastModifiedBy = lastModifiedBy;
                    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.SaleType = saleType;
                    saleMaster.PreviousSalesInvoiceNo = previousInvoiceNo;
                    saleMaster.Trading = "N";
                    saleMaster.IsPrint = isPrint;
                    saleMaster.TenderId = tenderId;
                    saleMaster.TransactionType = transactionType;
                    saleMaster.DeliveryDate =
                       Convert.ToDateTime(deliveryDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.Post = post; //Post


                    // Remove DB Process
                    var currencyid = dtSaleM.Rows[i]["CurrencyID"].ToString().Trim();//cImport.FindCurrencyId(currencyCode, currConn, transaction);
                    saleMaster.CurrencyID = currencyid; //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, currConn, transaction));
                    // remove DB 
                    saleMaster.ReturnId = cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction);
                    // return ID is used for doller rate
                    saleMaster.LCNumber = lCNumber;
                    saleMaster.ImportIDExcel = VinvoiceNo;

                    saleMaster.TotalAmount = Convert.ToDecimal("0.00");
                    saleMaster.TotalVATAmount = Convert.ToDecimal("0.00");
                    saleMaster.CompInvoiceNo = compInvoiceNo;
                    saleMaster.BranchId = branchId;

                    #endregion Master


                    #endregion Master Sale

                    #region Match

                    DataRow[] DetailsRaws; //= new DataRow[];//
                    if (!string.IsNullOrEmpty(VinvoiceNo))
                    {
                        DetailsRaws = dtSaleD.Select("ID='" + VinvoiceNo + "'");
                    }
                    else
                    {
                        DetailsRaws = null;
                    }



                    #endregion Match

                    #region Details Sale

                    //int totalCounter = 1;
                    int lineCounter = 1;
                    decimal totalAmount = 0;
                    decimal totalVatAmount = 0;


                    saleDetails = new List<SaleDetailVm>();
                    #region Juwel 15/10/2015

                    DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name", "Non_Stock", "Type", "VAT_Name", "ItemNo");
                    //DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code","Non_Stock", "Type", "VAT_Name");
                    DataTable dtSalesDetail = DetailsRaws.CopyToDataTable();

                    //DataTable dtDistinctItem = dtSaleD.Select("ID='" + VinvoiceNo + "'").CopyToDataTable();


                    string nBRPrice = "", uOM = "", uOMn = "", uOMc = "";

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        decimal cTotalValue = 0, cWareHouseRent = 0, cWareHouseVAT = 0, cATVRate = 0
                            , cATVablePrice = 0
                            , cATVAmount = 0
                            , Total_Price = 0;
                        decimal NBR_Price = 0, totalQuantity = 0, totalPrice = 0, vatRate = 0, sdRate = 0, tradingMarkup = 0, discountAmount = 0, promotionalQuantity = 0;
                        string cIsCommercialImporter = "N";


                        decimal LastNBRPrice = 0;
                        string saleDID = VinvoiceNo;//row["ID"].ToString().Trim();
                        VitemCode = item["Item_Code"].ToString().Trim();


                        // db call
                        //string itemName = item["Item_Name"].ToString().Trim();
                        bool ItemAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct") == "Y" ? true : false);
                        // db call
                        string itemNo = item["ItemNo"].ToString().Trim();//cImport.FindItemId(item["Item_Name"].ToString().Trim(), VitemCode, currConn, transaction, ItemAutoSave);
                        string vATName = item["VAT_Name"].ToString().Trim();
                        string nonStock = item["Non_Stock"].ToString().Trim();
                        string type = item["Type"].ToString().Trim();
                        //string commentsD = item["CommentsD"].ToString().Trim();
                        string weight = "";

                        DataTable dtRepeatedItems = dtSalesDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            //cTotalValue = row["TotalValue"].ToString().Trim();
                            //cWareHouseRent = row["WareHouseRent"].ToString().Trim();
                            //cWareHouseVAT = row["WareHouseVAT"].ToString().Trim();
                            //cATVRate = row["ATVRate"].ToString().Trim();
                            //cATVablePrice = row["ATVablePrice"].ToString().Trim();
                            //cATVAmount = row["ATVAmount"].ToString().Trim();
                            //cIsCommercialImporter = row["IsCommercialImporter"].ToString().Trim();

                            Total_Price = Convert.ToDecimal(row["Total_Price"].ToString().Trim() == "" ? "0" : row["TotalValue"].ToString().Trim());
                            //VAT_Amount = Convert.ToDecimal(row["VAT_Amount"].ToString().Trim() == "" ? "0" : row["VAT_Amount"].ToString().Trim());
                            totalQuantity = totalQuantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            commentsD = row["CommentsD"].ToString().Trim();
                            //totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());/////Juwel
                            if (type.ToLower() != "non-vat system")
                            {
                                vatRate = vatRate + Convert.ToDecimal(row["VAT_Rate"].ToString().Trim());
                                sdRate = sdRate + Convert.ToDecimal(row["SD_Rate"].ToString().Trim());
                                tradingMarkup = tradingMarkup + Convert.ToDecimal(row["Trading_MarkUp"].ToString().Trim());
                            }
                            //else
                            //{
                            //    vatRate =vatRate+ 0;
                            //    sdRate =sdRate+ 0;
                            //    tradingMarkup = tradingMarkup + 0;
                            //}

                            discountAmount = discountAmount + Convert.ToDecimal(row["Discount_Amount"].ToString().Trim());
                            promotionalQuantity = promotionalQuantity + Convert.ToDecimal(row["Promotional_Quantity"].ToString().Trim());
                            if (IsColWeight == true)
                            {
                                weight = row["Weight"].ToString().Trim();
                            }

                            if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                            {
                                NBR_Price = NBR_Price + Convert.ToDecimal(row["NBR_Price"].ToString().Trim());
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                                totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim() == "" ? "0" : row["Total_Price"].ToString().Trim());

                                cTotalValue = cTotalValue + Convert.ToDecimal(row["TotalValue"].ToString().Trim() == "" ? "0" : row["TotalValue"].ToString().Trim());
                                cWareHouseRent = cWareHouseRent + Convert.ToDecimal(row["WareHouseRent"].ToString().Trim() == "" ? "0" : row["WareHouseRent"].ToString().Trim());
                                cWareHouseVAT = cWareHouseVAT + Convert.ToDecimal(row["WareHouseVAT"].ToString().Trim() == "" ? "0" : row["WareHouseVAT"].ToString().Trim());
                                cATVRate = Convert.ToDecimal(row["ATVRate"].ToString().Trim() == "" ? "0" : row["ATVRate"].ToString().Trim());
                                cATVablePrice = cATVablePrice + Convert.ToDecimal(row["ATVablePrice"].ToString().Trim() == "" ? "0" : row["ATVablePrice"].ToString().Trim());
                                cATVAmount = cATVAmount + Convert.ToDecimal(row["ATVAmount"].ToString().Trim() == "" ? "0" : row["ATVAmount"].ToString().Trim());
                                cIsCommercialImporter = row["IsCommercialImporter"].ToString().Trim() == "" ? "N" : row["IsCommercialImporter"].ToString().Trim();

                            }


                        }

                        #region For Sanofi
                        if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                        {
                            // string totalPrice = row["Total_Price"].ToString().Trim();
                            nBRPrice = (Convert.ToDecimal(totalPrice) / Convert.ToDecimal(totalQuantity)).ToString();
                            LastNBRPrice = Convert.ToDecimal(nBRPrice);


                            uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                            uOM = uOMn;
                            uOMc = "1";
                        }
                        else
                        {
                            nBRPrice = NBR_Price.ToString();

                            if (transactionType == "ExportService"
                                 && transactionType == "ExportServiceNS"
                               && transactionType == "Service"
                               && transactionType == "ServiceNS")
                            {
                                if (nBRPrice == "0")
                                {
                                    LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                }
                                else
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                            else
                            {
                                LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                if (IsPriceDeclaration == false)
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                        }
                        #endregion

                        string vATRate = Convert.ToString(vatRate / dtRepeatedItems.Rows.Count);
                        if (string.IsNullOrEmpty(vATRate))
                        {
                            vATRate = "0";
                        }
                        string sDRate = Convert.ToString(sdRate / dtRepeatedItems.Rows.Count);
                        string tradingMarkUp = tradingMarkup.ToString();
                        if (type.ToLower() == "vat" || type.ToLower() == "vat system")
                        {
                            type = "VAT";

                        }
                        else
                        {
                            type = "Non VAT";
                            vATName = " ";
                        }

                        SaleDetailVm detail = new SaleDetailVm();
                        detail.InvoiceLineNo = lineCounter.ToString();
                        detail.ItemNo = itemNo;
                        decimal vQuantity = Convert.ToDecimal(Convert.ToDecimal(totalQuantity) + Convert.ToDecimal(promotionalQuantity));
                        detail.Quantity = vQuantity;
                        detail.PromotionalQuantity = Convert.ToDecimal(promotionalQuantity);
                        detail.VATRate = Convert.ToDecimal(vATRate);
                        detail.VATAmount = VAT_Amount == 0 ? Convert.ToDecimal(vQuantity) * Convert.ToDecimal(vATRate) : VAT_Amount;
                        detail.SD = Convert.ToDecimal(sDRate);
                        detail.CommentsD = commentsD;
                        detail.SaleTypeD = saleType;
                        detail.PreviousSalesInvoiceNoD = previousInvoiceNo;
                        detail.TradingD = "N";
                        detail.NonStockD = nonStock;
                        detail.Type = type;
                        detail.CConversionDate = invoiceDateTime;
                        detail.VatName = vATName;
                        detail.Weight = weight;
                        detail.TradingMarkUp = Convert.ToDecimal(tradingMarkUp);


                        if (CommercialImporter)
                        {

                            detail.TotalValue = Convert.ToDecimal(cTotalValue);
                            detail.WareHouseRent = Convert.ToDecimal(cWareHouseRent);
                            detail.WareHouseVAT = Convert.ToDecimal(cWareHouseVAT);
                            detail.ATVRate = Convert.ToDecimal(cATVRate);
                            detail.ATVablePrice = Convert.ToDecimal(cATVablePrice);
                            detail.ATVAmount = Convert.ToDecimal(cATVAmount);
                            detail.IsCommercialImporter = cIsCommercialImporter;


                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            LastNBRPrice = LastNBRPrice;// Total_Price / vQuantity;
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);

                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;

                            decimal subTotal = nbrPrice;
                            detail.NBRPrice = nbrPrice / vQuantity;
                            detail.SalesPrice = detail.NBRPrice;

                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(detail.NBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);


                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }
                        else
                        {
                            detail.TotalValue = Convert.ToDecimal(cTotalValue);
                            detail.WareHouseRent = Convert.ToDecimal(cWareHouseRent);
                            detail.WareHouseVAT = Convert.ToDecimal(cWareHouseVAT);
                            detail.ATVRate = Convert.ToDecimal(cATVRate);
                            detail.ATVablePrice = Convert.ToDecimal(cATVablePrice);
                            detail.ATVAmount = Convert.ToDecimal(cATVAmount);
                            detail.IsCommercialImporter = cIsCommercialImporter;

                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);
                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;
                            detail.UOM = uOM;

                            detail.SalesPrice = nbrPrice;
                            detail.NBRPrice = nbrPrice;
                            decimal subTotal = vQuantity * nbrPrice;
                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);
                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }
                        detail.BranchId = branchId;

                        saleDetails.Add(detail);


                        lineCounter++;
                    }
                    #endregion

                    #endregion Details Sale

                    #region Details Export

                    int eCounter = 1;

                    if (transactionType == "Export"
                    || transactionType == "ExportTender"
                    || transactionType == "ExportTrading"
                    || transactionType == "ExportServiceNS"
                    || transactionType == "ExportService"
                    || transactionType == "ExportTradingTender"
                    || transactionType == "ExportPackage")
                    {
                        DataRow[] ExportRaws; //= new DataRow[];//
                        if (!string.IsNullOrEmpty(VinvoiceNo))
                        {
                            ExportRaws = dtSaleE.Select("ID='" + VinvoiceNo + "'");
                        }
                        else
                        {

                            ExportRaws = null;
                            if (ExportRaws == null)
                            {
                                throw new ArgumentNullException("For Export sale must filup the SaleE file");
                            }

                        }

                        saleExport = new List<SaleExportVM>();
                        foreach (DataRow row in ExportRaws)
                        {
                            string saleEID = row["ID"].ToString().Trim();
                            string description = row["Description"].ToString().Trim();
                            string quantityE = row["Quantity"].ToString().Trim();
                            string grossWeight = row["GrossWeight"].ToString().Trim();

                            string netWeight = row["NetWeight"].ToString().Trim();
                            string numberFrom = row["NumberFrom"].ToString().Trim();
                            string numberTo = row["NumberTo"].ToString().Trim();
                            //string portFrom = row["PortFrom"].ToString().Trim();

                            //string portTo = row["PortTo"].ToString().Trim();


                            SaleExportVM expDetail = new SaleExportVM();
                            expDetail.SaleLineNo = eCounter.ToString();
                            expDetail.Description = description.ToString();
                            expDetail.QuantityE = quantityE.ToString();
                            expDetail.GrossWeight = grossWeight.ToString();
                            expDetail.NetWeight = netWeight.ToString();
                            expDetail.NumberFrom = numberFrom.ToString();
                            expDetail.NumberTo = numberTo.ToString();
                            expDetail.CommentsE = "NA";
                            expDetail.RefNo = "NA";
                            expDetail.BranchId = branchId;
                            saleExport.Add(expDetail);

                            eCounter++;

                        } // details


                    }
                    #endregion Details Export

                    #endregion Process model


                    if (CommercialImporter)
                    {
                        #region CommercialImporter

                        bool NeedSave = false;
                        bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                        bool TrackingWithSaleFIFO = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSaleFIFO") == "Y" ? true : false);
                        int NumberOfItems = Convert.ToInt32(commonDal.settingValue("Sale", "NumberOfItems"));

                        string sqlText = "";
                        string dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                        string user = createdBy + dtime;
                        IsertPurchaseSaleTrackingTemps(user, currConn, transaction);
                        decimal qty = 0;
                        decimal Saleqty = 0;

                        foreach (var Item in saleDetails.ToList())
                        {


                            qty = Item.Quantity;
                            for (int t = 0; t < qty; t++)
                            {

                                DataTable tracDt = new DataTable();
                                sqlText = "";
                                sqlText += @" select top 1 * from PurchaseSaleTrackingTemps";
                                sqlText += @" where IsSold=0";
                                sqlText += @" and ItemNo=@ItemItemNo";
                                sqlText += @" and LoginUser='" + user + "'";
                                if (TrackingWithSaleFIFO)
                                {
                                    sqlText += @" order by id asc ";
                                }
                                else
                                {
                                    sqlText += @" order by id desc ";
                                }
                                SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                                cmdRIFB1.Transaction = transaction;
                                cmdRIFB1.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                                SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                                reportDataAdapt1.Fill(tracDt);

                                foreach (DataRow itemTrac in tracDt.Rows)
                                {
                                    sqlText = "";
                                    sqlText += " update PurchaseSaleTrackingTemps set  ";
                                    sqlText += " IsSold= '1' ";
                                    sqlText += " ,SalesInvoiceNo= '" + dtime + "' ";
                                    //sqlText += " ,SaleInvoiceDateTime= '" + Master.InvoiceDateTime + "' ";
                                    sqlText += " where  Id= '" + itemTrac["Id"] + "' ";

                                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                                    cmdUpdate.Transaction = transaction;
                                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                                    }
                                }
                                ReportDSDAL rds = new ReportDSDAL();
                                DataSet ds = new DataSet();
                                ds = rds.VAT11ReportCommercialImporterNewTemp(dtime, "N", "N", currConn, transaction);
                                int rowcnt = ds.Tables[0].Rows.Count;
                                if (rowcnt >= NumberOfItems)
                                {
                                    Saleqty = Saleqty + 1;
                                    var obj = saleDetails.FirstOrDefault(x => x.ItemNo == Item.ItemNo);
                                    if (obj != null) obj.Quantity = Saleqty;

                                    var sqlResults = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn,branchId,null,UserId);
                                    Thread.Sleep(1000);
                                    retResults[0] = sqlResults[0];
                                    Saleqty = 0;
                                    dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                                    NeedSave = false;

                                }
                                else
                                {

                                    Saleqty = Saleqty + 1;
                                    var obj = saleDetails.FirstOrDefault(x => x.ItemNo == Item.ItemNo);
                                    if (obj != null) obj.Quantity = Saleqty;
                                    NeedSave = true;


                                    //saleDetails
                                }

                            }




                        }
                        DeletePurchaseSaleTrackingTemps(user, currConn, transaction);

                        if (NeedSave)
                        {
                            var sqlResults1 = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn,branchId,connVM,UserId);
                            Thread.Sleep(1000);
                            retResults[0] = sqlResults1[0];
                            Saleqty = 0;
                            dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                        }

                        #endregion CommercialImporter

                    }
                    else
                    {
                        var sqlResults = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn,branchId,connVM,UserId);
                        retResults[0] = sqlResults[0];
                    }


                }// master

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    retResults[4] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();
                    #endregion SuccessResult
                }
                //SAVE_DOWORK_SUCCESS = true;
            }
            #endregion try
            #region catch & final
            //catch (SqlException sqlex)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            //catch (ArgumentNullException aeg)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            //    //throw ex;
            //}
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString() + FieldDelimeter + "Invoice:" + VinvoiceNo + FieldDelimeter + "Item:" + VitemCode);//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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

        public decimal FormatingNumeric(decimal value, int DecPlace, SysDBInfoVMTemp connVM = null)
        {
            object outPutValue = 0;
            string decPointLen = "";
            try
            {

                for (int i = 0; i < DecPlace; i++)
                {
                    decPointLen = decPointLen + "0";
                }
                if (value < 1000)
                {
                    var a = "0." + decPointLen + "";
                    outPutValue = value.ToString(a);
                }
                else
                {
                    var a = "0,0." + decPointLen + "";
                    outPutValue = value.ToString(a);

                }


            }
            #region Catch
            catch (Exception ex)
            {
                //string exMessage = ex.Message;
                //if (ex.InnerException != null)
                //{
                //    exMessage = exMessage + Environment.NewLine + ex.InnerException.Message + Environment.NewLine +
                //                ex.StackTrace;

                //}
                //MessageBox.Show(ex.Message, "Program", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //FileLogger.Log("Program", "FormatTextBoxQty4", exMessage);
            }
            #endregion Catch

            return Convert.ToDecimal(outPutValue);
        }

        public void SetDeliveryChallanNo(string saleInvoiceNo, string challanDate, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
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
                    transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region SetDCNo
                CommonDAL commonDal = new CommonDAL();
                string newID = commonDal.TransactionCode("Sale", "Delivery", "SalesInvoiceHeaders", "DeliveryChallanNo",
                                                    "DeliveryDate", challanDate, "1", currConn, transaction);

                #endregion
                #region Update into table

                sqlText = "  ";
                sqlText = " Update SalesInvoiceHeaders Set DeliveryChallanNo= '";
                sqlText += newID + "' where SalesInvoiceNo = @saleInvoiceNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo);
                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }

                if (transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Update into table

            }

            #endregion try

            #region Catch and Finall
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
        }

        public void SetGatePass(string saleInvoiceNo, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            SqlConnection currConn = null;

            int transResult = 0;
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

                #region Update into table

                sqlText = "  ";
                sqlText = " Update SalesInvoiceHeaders Set IsGatePass= 'Y'";
                sqlText += " where SalesInvoiceNo = @saleInvoiceNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@saleInvoiceNo", saleInvoiceNo);
                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion Update into table

            }

            #endregion try

            #region Catch and Finall
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
        }
        //currConn to VcurrConn 25-Aug-2020
        public void IsertPurchaseSaleTrackingTemps(string LoginUser, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";

            int transResult = 0;
            #endregion

            #region Try

            try
            {


                #region Update into table

                sqlText = "  ";
                sqlText = @" 
delete from PurchaseSaleTrackingTemps where LoginUser=@LoginUser

insert into PurchaseSaleTrackingTemps(
Id
,LoginUser
,PurchaseInvoiceNo
,PurchaseInvoiceDateTime
,ReceiveDate
,ItemNo
,BENumber
,SalesInvoiceNo
,SaleInvoiceDateTime
,IsSold
,CustomHouse)

select 
Id
,@LoginUser
,PurchaseInvoiceNo
,PurchaseInvoiceDateTime
,ReceiveDate
,ItemNo
,BENumber
,SalesInvoiceNo
,SaleInvoiceDateTime
,IsSold
,CustomHouse

from  PurchaseSaleTrackings
  where IsSold=0
";

                SqlCommand cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);
                cmd.Parameters.AddWithValue("@LoginUser", LoginUser);

                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion Update into table

            }

            #endregion try

            #region Catch and Finall
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
            finally
            {
                if (VcurrConn != null)
                {
                    if (VcurrConn.State == ConnectionState.Open)
                    {
                        //currConn.Close();

                    }
                }
            }

            #endregion
        }
        //currConn to VcurrConn 25-Aug-2020
        public void DeletePurchaseSaleTrackingTemps(string LoginUser, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";

            int transResult = 0;
            #endregion

            #region Try

            try
            {


                #region Update into table

                sqlText = "  ";
                sqlText = @" 
delete from PurchaseSaleTrackingTemps where LoginUser=@LoginUser
 
";

                SqlCommand cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);
                cmd.Parameters.AddWithValue("@LoginUser", LoginUser);

                transResult = (int)cmd.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }


                #endregion Update into table

            }

            #endregion try

            #region Catch and Finall
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
            finally
            {
                if (VcurrConn != null)
                {
                    if (VcurrConn.State == ConnectionState.Open)
                    {
                        //currConn.Close();

                    }
                }
            }

            #endregion
        }


        #endregion

        #region Method 02

        public DataTable GetUnProcessedTempData(SysDBInfoVMTemp connVM = null)
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
            try
            {

                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sql = @"select * from SalesTempData where IsProcessed = 0";

                #endregion


                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);
                var dataAdapter = new SqlDataAdapter(cmd);
                var unprocessedData = new DataTable();
                dataAdapter.Fill(unprocessedData);

                #endregion

                transaction.Commit();

                return unprocessedData;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public int GetUnProcessedCount(SysDBInfoVMTemp connVM = null)
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
            try
            {

                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sql = @"select count(distinct(Id)) from SalesTempData where IsProcessed = 0";

                #endregion


                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);
                var rowCount = (int)cmd.ExecuteScalar();
                #endregion

                transaction.Commit();

                return rowCount;

            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public string[] ProccessTempData(SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sql = @"select top(1) Id from SalesTempData where IsProcessed = 0";
                var getTempById = @"select * from SalesTempData where Id = @Id";
                var updateProcessed = @"update SalesTempData set IsProcessed = 1 Where Id = @Id";

                #endregion

                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);
                var topId = (string)cmd.ExecuteScalar();

                cmd.CommandText = getTempById;
                cmd.Parameters.AddWithValue("@Id", topId);

                var tempDataById = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(tempDataById);


                cmd.CommandText = updateProcessed;
                // cmd.Parameters.AddWithValue("@Id", topId);
                cmd.ExecuteNonQuery();

                #endregion


                transaction.Commit();
                retResults[0] = "success";

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public string[] GetTopUnProcessed(SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sql = @"select top(1) Id from SalesTempData where IsProcessed = 0 and BranchId is not null";

                #endregion

                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);
                var topId = (string)cmd.ExecuteScalar();

                #endregion


                transaction.Commit();
                retResults[0] = "success";
                retResults[1] = "success";
                retResults[2] = topId;

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable GetById(string id, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var getTempById = @"select * from SalesTempData where Id = @Id";


                #endregion

                #region Sql Command

                var cmd = new SqlCommand(getTempById, currConn, transaction);
                cmd.CommandText = getTempById;
                cmd.Parameters.AddWithValue("@Id", id);

                var tempDataById = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(tempDataById);

                #endregion


                transaction.Commit();
                // retResults[0] = "success";

                return tempDataById;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public string[] UpdateIsProcessed(int flag, string Id, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var updateIsProcessed = @"update SalesTempData set IsProcessed = @flag Where Id = @Id";

                #endregion

                #region Sql Command

                var cmd = new SqlCommand(updateIsProcessed, currConn, transaction);

                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@flag", flag);

                var row = cmd.ExecuteNonQuery();
                #endregion


                transaction.Commit();
                retResults[0] = "success";
                retResults[1] = "success";
                retResults[2] = row.ToString();

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public int GetUnProcessedCount_tempTable(string tempTable, SysDBInfoVMTemp connVM = null)
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
            try
            {

                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sql = @"select count(distinct(Id)) from " + tempTable + " where IsProcessed = 0";

                #endregion


                #region Sql Command

                var cmd = new SqlCommand(sql, currConn, transaction);

                var rowCount = (int)cmd.ExecuteScalar();
                #endregion

                transaction.Commit();

                return rowCount;

            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable SaleTempNoBranch(SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var getTempByBranch = @"select * from SalesTempData where BranchId is null";


                #endregion

                #region Sql Command

                var cmd = new SqlCommand(getTempByBranch, currConn, transaction);
                cmd.CommandText = getTempByBranch;

                var noBranchData = new DataTable();
                var dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(noBranchData);

                #endregion


                transaction.Commit();

                return noBranchData;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable GetMasterData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, string app = "", SysDBInfoVMTemp connVM = null, string userId = null, bool orderBy = false, string token = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;


            try
            {

                #region Connection and Transaction

                if (vConnection != null)
                {
                    connection = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();

                }

                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }

                #endregion


                var sqlText = @"
SELECT distinct 
SalesInvoiceNo
,[Delivery_Address] DeliveryAddress1
,[Invoice_Date_Time] InvoiceDateTime
,[Delivery_Date_Time] DeliveryDate
,[CustomerID] CustomerID
, Sale_Type SaleType
, Previous_Invoice_No PreviousSalesInvoiceNo
, Tender_Id TenderId
, LC_Number LCNumber
,[BranchId] BranchId
,[VehicleID] VehicleID,
TotalAmount,
TotalVATAmount,
--BomId,
ReturnId SaleReturnId,
TransactionType,
Comments,
CreatedBy,
CreatedOn,
Post,
CurrencyID,
Id ImportIDExcel ,
Id  ,
Is_Print IsPrint
,Reference_No  SerialNo
,DeliveryAddress2
,DeliveryAddress3
,Trading 
,IsVDS 
,GetVDSCertificate 
,VDSCertificateDate 
,AlReadyPrint 
,DeliveryChallanNo 
,IsGatePass 
,CompInvoiceNo 
,LCBank 
,LCDate 
,PINo 
,PIDate 
,EXPFormNo 
,IsDeemedExport 
,VDSAmount 
,Is6_3TollCompleted 
,ValueOnly 
,CurrencyRateFromBDT 
,ShiftId 
,FileName
,EXPFormDate
,DeductionAmount
,CNDNRef
,BranchRef
,DataSource
,IsCurrencyConvCompleted
FROM SalesTempData   
";


                if (userId != null)
                {
                    sqlText += " where UserId = " + userId;
                }
                if (token != null)
                {
                    sqlText += " where token = '" + token + "'";
                }

                if (orderBy)
                {
                    sqlText += " order by Id";
                }

                var cmd = new SqlCommand(sqlText, connection, transaction);
                cmd.CommandTimeout = 500;
                var table = new DataTable();

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);



                table.Columns.Add(new DataColumn("AppVersion") { DefaultValue = app });


                if (vTransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                return table;


            }
            catch (Exception ex)
            {
                if (vTransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", ex.Message.ToString());
            }
            finally
            {
                if (vConnection == null && connection != null)
                {
                    connection.Close();
                }
            }

        }

        public DataTable GetReceiveMasterData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, string app = "", SysDBInfoVMTemp connVM = null, string token = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;


            try
            {
                if (vConnection != null)
                {
                    connection = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();

                }

                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }


                var sqlText = @"
SELECT distinct SalesTempData.SalesInvoiceNo ReceiveNo

      ,SalesTempData.[Invoice_Date_Time] ReceiveDateTime


      ,SalesTempData.[BranchId] BranchId
	  ,SalesTempData.TotalAmount

	  ,SalesTempData.ReturnId ReceiveReturnId ,
	 'TradingAuto' TransactionType,
      SalesTempData.Comments,
	  SalesTempData.CreatedBy,
	  SalesTempData.CreatedOn,
      SalesTempData.Post,
      SalesTempData.SalesInvoiceNo  SerialNo

	    FROM  SalesTempData left outer join Products on SalesTempData.ItemNo = Products.ItemNo 
	   left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID
	   where ProductCategories.IsRaw = 'Trading' ";


                if (token != null)
                {
                    sqlText += " and token = '" + token + "'";
                }



                var cmd = new SqlCommand(sqlText, connection, transaction);

                var table = new DataTable();

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);



                table.Columns.Add(new DataColumn("AppVersion") { DefaultValue = app });


                if (transaction != null && vTransaction == null)
                {
                    transaction.Commit();
                }

                return table;


            }
            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (connection != null && vConnection == null)
                {
                    connection.Close();
                }
            }

        }

        public DataTable GetIssueMasterData(SqlConnection vConnection = null, SqlTransaction vTransaction = null, string app = "", SysDBInfoVMTemp connVM = null, string token = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;


            try
            {
                if (vConnection != null)
                {
                    connection = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();

                }

                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }


                var sqlText = @"
SELECT distinct 
	   SalesTempData.SalesInvoiceNo IssueNo

      ,SalesTempData.[Invoice_Date_Time] IssueDateTime


      ,SalesTempData.[BranchId] BranchId
	  ,SalesTempData.TotalAmount,

	  SalesTempData.ReturnId IssueReturnId,
	  'TradingAuto' TransactionType,
      SalesTempData.Comments,
	  SalesTempData.CreatedBy,
	  SalesTempData.CreatedOn,
      SalesTempData.Post,
      SalesTempData.SalesInvoiceNo  SerialNo

	   FROM  SalesTempData left outer join Products on SalesTempData.ItemNo = Products.ItemNo 
	   left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID
	   where ProductCategories.IsRaw = 'Trading'  ";

                if (token != null)
                {
                    sqlText += " and token = '" + token + "'";
                }



                var cmd = new SqlCommand(sqlText, connection, transaction);

                var table = new DataTable();

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);



                table.Columns.Add(new DataColumn("AppVersion") { DefaultValue = app });


                if (transaction != null && vTransaction == null)
                {
                    transaction.Commit();
                }

                return table;


            }
            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (connection != null && vConnection == null)
                {
                    connection.Close();
                }
            }

        }

        public DataTable GetMasterDataOtherDb(SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;


            try
            {
                if (vConnection != null)
                {
                    connection = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();

                }

                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }


                var sqlText = @"SELECT distinct ID SalesInvoiceNo
      ,[Delivery_Address] DeliveryAddress1
      ,[Invoice_Date_Time] InvoiceDateTime
      ,[Delivery_Date_Time] DeliveryDate
    
      ,[CustomerID] CustomerID
       , Sale_Type SaleType
       , Previous_Invoice_No PreviousSalesInvoiceNo
       , Tender_Id TenderId
       , LC_Number LCNumber

      ,[BranchId] BranchId
      ,[VehicleID] VehicleID,
	  TotalAmount,
	  TotalVATAmount,
	  --BomId,
	  ReturnId SaleReturnId,
	  TransactionType,
      Comments,
	  CreatedBy,
	  CreatedOn,
      Post,
CurrencyID,
Id ImportIDExcel ,
Is_Print IsPrint
      ,Reference_No  SerialNo
        ,DeliveryAddress2
        ,DeliveryAddress3
        ,Trading 
        ,IsVDS 
        ,GetVDSCertificate 
        ,VDSCertificateDate 
        ,AlReadyPrint 
        ,DeliveryChallanNo 
        ,IsGatePass 
        ,CompInvoiceNo 
        ,LCBank 
        ,LCDate 
        ,PINo 
        ,PIDate 
        ,EXPFormNo 
        ,IsDeemedExport 
        ,VDSAmount 
        ,Is6_3TollCompleted 
        ,ValueOnly 
        ,CurrencyRateFromBDT 
        ,ShiftId 

      ,EXPFormDate
      ,DeductionAmount
	   FROM SalesTempData";

                var cmd = new SqlCommand(sqlText, connection, transaction);

                var table = new DataTable();

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                if (transaction != null && vTransaction == null)
                {
                    transaction.Commit();
                }

                return table;


            }
            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (connection != null && vConnection == null)
                {
                    connection.Close();
                }
            }

        }


        public string[] SaveAndProcessIntegration(DataTable data, Action callBack = null, int branchId = 1, string app = "", SysDBInfoVMTemp connVM = null, string userId = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection(connVM);
                connection.Open();
                transaction = connection.BeginTransaction();

                result = ImportBigData(data, true, connection, transaction, null, branchId, null, false, userId);

                string uomcText = @"select top 1 * from SalesTempData  where UOMc = 0 or UOMc  is null";
                SqlCommand uomCmd = new SqlCommand(uomcText, connection, transaction);
                SqlDataAdapter uomAdapter = new SqlDataAdapter(uomCmd);
                DataTable uomcTable = new DataTable();

                uomAdapter.Fill(uomcTable);

                if (uomcTable.Rows.Count > 0)
                {
                    throw new Exception("UOM Conversion not found for " + uomcTable.Rows[0]["Item_Code"] + "-" +
                                        uomcTable.Rows[0]["Item_Name"]);
                }

                if (result[0].ToLower() == "success")
                {
                    callBack();

                    result = SaveInvoiceIdSaleTemp(0, connection, transaction);

                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }


        public string[] SaveTempAndProcess(DataTable data, Action callBack = null, int branchId = 1, string app = "", SysDBInfoVMTemp connVM = null, string token = null, IntegrationParam paramVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection(connVM);
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();

                #region Merge Date Time

                if (data.Columns.Contains("Invoice_Date") && data.Columns.Contains("Invoice_Time"))
                {
                    if (!data.Columns.Contains("Invoice_Date_Time"))
                    {
                        data.Columns.Add("Invoice_Date_Time");
                    }

                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["Invoice_Date_Time"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Invoice_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Invoice_Time"].ToString());
                    }

                    data.Columns.Remove("Invoice_Date");
                    data.Columns.Remove("Invoice_Time");
                }
                else
                {
                    OrdinaryVATDesktop.DataTable_DateFormat(data, "Invoice_Date_Time");
                    if (data.Columns.Contains("Delivery_Date_Time"))
                    {
                        OrdinaryVATDesktop.DataTable_DateFormat(data, "Delivery_Date_Time");
                    }
                }

                #endregion


                result = ImportBigData(data, true, connection, transaction, null, branchId, null, false, null, token, paramVM);

                var uomcText = @"select top 1 * from SalesTempData  where (UOMc = 0 or UOMc  is null) and token='" +
                               token + "'";
                var uomCmd = new SqlCommand(uomcText, connection, transaction);
                var uomAdapter = new SqlDataAdapter(uomCmd);
                var uomcTable = new DataTable();

                uomAdapter.Fill(uomcTable);


                if (uomcTable.Rows.Count > 0)
                {
                    throw new Exception("UOM Conversion not found for " + uomcTable.Rows[0]["Item_Code"] + "-" +
                                        uomcTable.Rows[0]["Item_Name"]);
                }


                if (result[0].ToLower() == "success")
                {
                    callBack();

                    result = SaveInvoiceIdSaleTemp(0, connection, transaction);

                    string sqlText = "";

                    sqlText = @"Select * from SalesTempData where token = @token order by Id ";
                    SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);

                    cmd.Parameters.AddWithValue("@token", token);

                    //////cmd.CommandText = sqlText;

                    var codeGen = new DataTable();

                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(codeGen);

                    CodeGenerationForSale(codeGen, connection, transaction);

                    var commonDAl = new CommonDAL();

                    sqlText = "delete from SalesTempData where token = @token1; ";

                    cmd.CommandText = sqlText;
                    cmd.Parameters.AddWithValue("@token1", token);
                    cmd.ExecuteNonQuery();

                    result = commonDal.BulkInsert("SalesTempData", codeGen, connection, transaction);
                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }

                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }


        public string[] SaveToTransactionTables(Action callBack = null, int branchId = 1, string app = "", SysDBInfoVMTemp connVM = null, string token = null, string UserId = "")
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection(connVM);
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();
                string sqlText = "";
                if (true)
                {
                    SqlCommand cmd = new SqlCommand(sqlText, connection, transaction);

                    bool TradingWithSale = new CommonDAL().settings("Trading", "TradingWithSale", connection, transaction).ToString().ToLower() == "y";
                    result[0] = "success";

                    if (result[0].ToLower() == "success")
                    {
                        callBack();
                        var masters = GetMasterData(connection, transaction, app, null, null, false, token);


                        if (masters.Rows.Count == 0)
                            throw new Exception("Data Not found");


                        result = ImportSalesBigData(masters, true, connection, transaction);

                        if (result[0].ToLower() == "success")
                        {
                            callBack();

                            var details = GetDetailsData(connection, transaction, null, null, token);

                            result = commonDal.BulkInsert("SalesInvoiceDetails", details, connection, transaction);
                            if (result[0].ToLower() == "success")
                            {
                                sqlText = "";
                                sqlText += " update SalesInvoiceDetails set ProductDescription=Comments where ProductDescription in('0','','-')";

                                SqlCommand cmdUpdate = new SqlCommand(sqlText, connection);
                                cmdUpdate.Transaction = transaction;


                                cmdUpdate.ExecuteNonQuery();


                                #region TradingWithSale

                                if (TradingWithSale)
                                {
                                    var receiveMasters =
                                        GetReceiveMasterData(connection, transaction, app, null, token);
                                    var issueMaster = GetIssueMasterData(connection, transaction, app, null, token);


                                    result = commonDal.BulkInsert("ReceiveHeaders", receiveMasters, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var receiveDetails = GetReceiveDetailsData(connection, transaction, null, token);
                                        string ids = "";

                                        foreach (DataRow row in receiveDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            row["VATName"] = bomDt.Rows[0]["VATName"];

                                            ids += "'" + row["ReceiveNo"] + "',";
                                        }


                                        result = commonDal.BulkInsert("ReceiveDetails", receiveDetails, connection, transaction);
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                            sqlText += " where ReceiveHeaders.ReceiveNo in ( " + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            cmd.ExecuteNonQuery();
                                        }
                                    }

                                    result = commonDal.BulkInsert("IssueHeaders", issueMaster, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var IssueDetails = GetIssueDetailsData(connection, transaction, null, token);
                                        string ids = "";

                                        ProductDAL productDal = new ProductDAL();

                                        foreach (DataRow row in IssueDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            DataTable avgPrice = productDal.AvgPriceNew(row["ItemNo"].ToString().Trim(),
                                                Convert.ToDateTime(row["IssueDateTime"]).ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:00"), connection, transaction, true, true, false, false, null,UserId);

                                            decimal Amount = Convert.ToDecimal(avgPrice.Rows[0]["Amount"].ToString());
                                            decimal Quantity = Convert.ToDecimal(avgPrice.Rows[0]["Quantity"].ToString());
                                            decimal costPrice = 0;
                                            if (Quantity > 0)
                                            {
                                                costPrice = Amount / Quantity;
                                            }

                                            row["NBRPrice"] = costPrice;
                                            row["CostPrice"] = costPrice;
                                            row["SubTotal"] = costPrice * Convert.ToDecimal(row["Quantity"]);
                                            row["UOMPrice"] = costPrice * Convert.ToDecimal(row["UOMc"]);

                                            row["BOMDate"] = Convert.ToDateTime(bomDt.Rows[0]["EffectDate"]).ToString("MM/dd/yyyy");

                                            ids += "'" + row["IssueNo"] + "',";

                                        }

                                        result = commonDal.BulkInsert("IssueDetails", IssueDetails, connection, transaction);
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText = "";
                                            sqlText += " update IssueHeaders set ";
                                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                            sqlText += " where IssueHeaders.IssueNo in (" + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                #endregion


                            }
                            else
                            {
                                throw new Exception("Details Import Failed");
                            }


                            cmd.CommandText = @"update  SalesInvoiceHeaders                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = ''

update  SalesInvoiceDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = '' ";


                            cmd.ExecuteNonQuery();

                            //commonDal.Update_PeriodId(connection, transaction);
                            callBack();
                        }
                        else
                        {
                            throw new Exception("Master Import Failed");
                        }
                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }


                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }




        #endregion

        #region Method 03

        public string[] SaveAndProcessITS(DataTable data, Action callBack = null, int branchId = 1, string app = "", string transactionType = "Other", string createdBy = "d", SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;

            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection(connVM);
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();

                #region SqlText

                var deleteTemp = @"delete from SaleTempCSV; ";

                deleteTemp += " DBCC CHECKIDENT ('SaleTempCSV', RESEED, 0);  ";

                var deleteTemp2 = @"delete from SalesTempDataIntertek; ";

                deleteTemp2 += " DBCC CHECKIDENT ('SalesTempDataIntertek', RESEED, 0);  ";

                string sqlText = @"
select 
[Transaction Reference] ID,
[Transaction Reference] Reference_No,
'-'  Delivery_Address,
[Transaction Date] Invoice_Date_Time,
[Transaction Date] Delivery_Date_Time,
[LOCATION  Analysis Code] Branch_Code,
'BDT' Currency_Code,
'-' Item_Name,
[SEGMENT Analysis Code] Item_Code,
1 Quantity,
[Base Amount] NBR_Price,
'Unit' UOM,
15 VAT_Rate,
0 SD_Rate,
'Y' Non_Stock,
[VALUE ADDED TAX  Analysis Code] AnalysisCode6,
'VAT 4.3' VAT_Name,
'NEW' Sale_Type,
'-' Comments,
[Transaction Reference] CommentsD,
'N' Post,
[DEBTORS/ CREDITORS  Analysis Code] Customer_Code,
'-' Customer_Name,
'N' Is_Print,
'0' Tender_Id,
'0' LC_Number,
0 SubTotal
, 0 Trading_MarkUp
from SaleTempCSV
";
                string getTempData = @" SELECT [SL]
      ,[ID]
      ,[SalesInvoiceNo]
      ,[Customer_Name]
      ,[Customer_Code]
      ,[Delivery_Address]
      ,[Vehicle_No]
      ,format(Invoice_Date_Time,'yyyy-MM-dd HH:mm:ss')Invoice_Date_Time
      ,format(Delivery_Date_Time,'yyyy-MM-dd HH:mm:ss')Delivery_Date_Time
      ,[Reference_No]
      ,[Comments]
      ,[Sale_Type]
      ,[Previous_Invoice_No]
      ,[Is_Print]
      ,[Tender_Id]
      ,[Post]
      ,[LC_Number]
      ,[Currency_Code]
      ,[Item_Code]
      ,[Item_Name]
      ,[Quantity]
      ,[NBR_Price]
      ,[UOM]
      ,[VAT_Rate]
      ,[SD_Rate]
      ,[Non_Stock]
      ,[Trading_MarkUp]
      ,[Type]
      ,[Discount_Amount]
      ,[Promotional_Quantity]
      ,[VAT_Name]
      ,[SubTotal]
      ,[IsVATComplete]
      ,[ItemNo]
      ,[CustomerID]
      ,[IsProcessed]
      ,[Branch_Code]
      ,[ExpDescription]
      ,[ExpQuantity]
      ,[ExpGrossWeight]
      ,[ExpNetWeight]
      ,[ExpNumberFrom]
      ,[ExpNumberTo]
      , CommentsD
      ,[BranchId]
      ,[CurrencyId]
      ,[CustomerGroup]
      ,[VAT_Amount]
      ,[GroupId]
      ,[VehicleID]
      ,[TotalAmount]
      ,[TotalVATAmount]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[BOMId]
      ,[TransactionType]
      ,[ReturnId]
      ,[UOMPrice]
      ,[UOMc]
      ,[UOMn]
      ,[UOMQty]
      ,[SDAmount]
      ,DiscountedNBRPrice
      ,AnalysisCode6 OtherRef
  FROM SalesTempDataIntertek

";
                string updateTable2 = @"

--update SalesTempDataIntertek set Customer_Code=SaleTempCSV.AccountCode
--from     SaleTempCSV
--where DebitCredit ='D'  and SalesTempDataIntertek.ID=SaleTempCSV.TransactionReference

update SalesTempDataIntertek set NBR_Price = NBR_Price * -1 where NBR_Price < 0

update SalesTempDataIntertek set Type= case when SalesTempDataIntertek.AnalysisCode6='VAT1500' then 'VAT' when SalesTempDataIntertek.AnalysisCode6='VATEXM0' then 'NonVAT' else '-' end 


update SalesTempDataIntertek 
set SalesTempDataIntertek.Type = 
case 
when SalesTempDataIntertek.AnalysisCode6 = 'VAT0000' then 'Export' 
when SalesTempDataIntertek.AnalysisCode6 = 'VATLC00' then 'DeemExport' 
end
where SalesTempDataIntertek.Type = '-'


--update SalesTempDataIntertek set SalesTempDataIntertek.Type = case when CustomerGroups.GroupType='Export' then 'Export' when CustomerGroups.GroupType='Local' then 'DeemExport' else '.' end  
--from SalesTempDataIntertek left outer join Customers
--on SalesTempDataIntertek.Customer_Code = Customers.CustomerCode 
--left outer join CustomerGroups on Customers.CustomerGroupID = CustomerGroups.CustomerGroupID
--where SalesTempDataIntertek.Type = '-'





update SalesTempDataIntertek set Type='t' where Type not in ('Export','DeemExport') 

update SalesTempDataIntertek set SalesTempDataIntertek.Delivery_Address = case when Customers.Address1 is not null then Customers.Address1 else '' end
from SalesTempDataIntertek left outer join Customers
on SalesTempDataIntertek.Customer_Code = Customers.CustomerCode 


update SalesTempDataIntertek set TransactionType = @ttype
update SalesTempDataIntertek set CreatedBy = @crby

update SalesTempDataIntertek set Sale_Type = 'credit' where TransactionType = 'credit'
update SalesTempDataIntertek set Sale_Type = 'debit' where TransactionType = 'debit'

create table #temp
(
	Id int Identity(1,1),
	Customer varchar(200),
	Total int,
	Rate decimal(25,9),
	CustomerId int
)


Insert Into #temp (Customer,Total)
select  Customer_Code, Sum(SubTotal) Total from SalesTempDataIntertek group by Customer_Code,ID

update #temp set CustomerId =Customers.CustomerID from Customers where Customers.CustomerCode = #temp.Customer

update #temp set Rate = 0

update #temp set Rate = CustomerDiscounts.Rate from CustomerDiscounts 
where #temp.CustomerId = CustomerDiscounts.CustomerID 
and #temp.Total >= CustomerDiscounts.MinValue and #temp.Total <= CustomerDiscounts.MaxValue

-- 
update SalesTempDataIntertek set CommentsD = Reference_No


create table #Refs
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	RefNo varchar(600),
	Type varchar(100)
)
Insert Into #Refs (Customer_code, RefNo,Type)
select distinct Customer_code, ID,Type from SalesTempDataIntertek

create table #updtRefs
(
	Id int Identity(1,1),
	Customer_code varchar(100),
	RefNo varchar(6000),
	Type varchar(100)
)

insert into #updtRefs (Customer_code, RefNo,Type)
SELECT DISTINCT temp2.Customer_code, 
    SUBSTRING(
        (
            SELECT ','+temp1.RefNo  AS [text()]
            FROM #Refs temp1
            WHERE temp1.Customer_code = temp2.Customer_code and temp1.Type = temp2.Type
            ORDER BY temp1.Customer_code
            FOR XML PATH ('')
        ), 2, 6000) [Refs],Type
FROM #Refs temp2


update SalesTempDataIntertek set Discount_Amount =(SalesTempDataIntertek.SubTotal*(#temp.Rate/100)) --, ID = #temp.Id, Reference_No = #temp.Id
from #temp where #temp.Customer = SalesTempDataIntertek.Customer_code
 
update SalesTempDataIntertek set  ID = #updtRefs.RefNo, Reference_No = #updtRefs.RefNo
from #updtRefs where #updtRefs.Customer_code = SalesTempDataIntertek.Customer_code and #updtRefs.Type = SalesTempDataIntertek.Type



update SalesTempDataIntertek Set TransactionType = 'Export' where Type = 'Export' or Type= 'DeemExport'
update SalesTempDataIntertek set Type= case when SalesTempDataIntertek.AnalysisCode6='VAT1500' then 'VAT' when SalesTempDataIntertek.AnalysisCode6='VATEXM0' then 'NonVAT' else Type end 
update SalesTempDataIntertek set VAT_Rate = 0 where Type != 'VAT'


--select * from #temp

--update SalesTempDataIntertek set DiscountedNBRPrice = NBR_Price 
--update SalesTempDataIntertek set NBR_Price = (NBR_Price - Discount_Amount)
--update SalesTempDataIntertek set SubTotal = (NBR_Price*1)


drop table #updtRefs
drop table #Refs
drop table #temp

--update SalesTempDataIntertek set  SDAmount=(SubTotal*SD_Rate)/100
--where SDAmount<=0 or SDAmount is null

 --update SalesTempDataIntertek set  VAT_Amount=((SubTotal)+SDAmount)*VAT_Rate/100
 --where VAT_Amount<=0 or VAT_Amount is null


";
                string checkCustomerCode = @"
select distinct customer_code from SalesTempDataIntertek
except
select distinct customercode from Customers";


                #endregion






                DataTable table = new DataTable();
                SqlCommand cmd = new SqlCommand(deleteTemp, connection, transaction);
                cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("SaleTempCSV", data, connection, transaction);



                cmd.CommandText = deleteTemp2;

                cmd.ExecuteNonQuery();

                cmd.CommandText = sqlText;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);



                result = commonDal.BulkInsert("SalesTempDataIntertek", table, connection, transaction);


                //------------------------------------
                //if (result[0].ToLower() == "success")
                //{
                //    transaction.Commit();
                //}


                //return result;
                //-----------------------------------



                #region Check customer
                SqlCommand cmdCus = new SqlCommand(checkCustomerCode, connection, transaction);
                DataTable dtCustomers = new DataTable();

                SqlDataAdapter adapterCus = new SqlDataAdapter(cmdCus);

                adapterCus.Fill(dtCustomers);





                if (dtCustomers.Rows.Count > 0)
                {
                    throw new Exception("Customer not found for code - " + dtCustomers.Rows[0]["customer_code"]);
                }

                #endregion


                //------------------------------------
                //if (result[0].ToLower() == "success")
                //{
                //    transaction.Commit();
                //}


                //return result;
                //-----------------------------------



                cmd.CommandText = updateTable2;
                cmd.CommandTimeout = 100;
                cmd.Parameters.AddWithValue("@ttype", transactionType);
                cmd.Parameters.AddWithValue("@crby", createdBy);

                cmd.ExecuteNonQuery();



                #region delete duplicate

                string deleteDuplicate = @"
update  SalesTempDataIntertek                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(Invoice_Date_Time)) +  CONVERT(VARCHAR(4),YEAR(Invoice_Date_Time)),6)
where PeriodId is null or PeriodId = ''

delete from SalesTempDataIntertek where ID in (
                select vr.ID from SalesTempDataIntertek vr inner join SalesInvoiceHeaders rh
                on vr.ID = rh.ImportIDExcel and vr.PeriodId = rh.PeriodId)";

                cmd.CommandText = deleteDuplicate;
                cmd.ExecuteNonQuery();
                #endregion




                cmd.CommandText = getTempData;

                adapter.SelectCommand = cmd;

                table = new DataTable();

                adapter.Fill(table);

                if (table.Rows.Count == 0)
                {
                    return result;
                }


                #region Next Step

                result = ImportBigData(table, true, connection, transaction, null, branchId, null, true);

                CheckUOMConversion(connection, transaction);

                if (result[0].ToLower() == "success")
                {
                    callBack();

                    result = CodegenAndSave(connection, transaction, commonDal);

                    if (result[0].ToLower() == "success")
                    {
                        callBack();
                        var masters = GetMasterData(connection, transaction, app);


                        //transaction.Commit();

                        result = ImportSalesBigData(masters, true, connection, transaction);

                        if (result[0].ToLower() == "success")
                        {
                            callBack();
                            var details = GetDetailsData(connection, transaction);
                            result = commonDal.BulkInsert("SalesInvoiceDetails", details, connection, transaction);
                            if (result[0].ToLower() == "success")
                            {
                                sqlText = "";
                                sqlText += " update SalesInvoiceDetails set ProductDescription=Comments where ProductDescription in('0','','-')";

                                SqlCommand cmdUpdate = new SqlCommand(sqlText, connection);
                                cmdUpdate.CommandTimeout = 100;
                                cmdUpdate.Transaction = transaction;

                                //adding parameter

                                cmdUpdate.ExecuteNonQuery();


                                #region TradingWithSale
                                bool TradingWithSale = new CommonDAL().settings("Trading", "TradingWithSale").ToString().ToLower() == "y";

                                if (TradingWithSale)
                                {
                                    var receiveMasters = GetReceiveMasterData(connection, transaction, app);
                                    var issueMaster = GetIssueMasterData(connection, transaction, app);


                                    result = commonDal.BulkInsert("ReceiveHeaders", receiveMasters, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var receiveDetails = GetReceiveDetailsData(connection, transaction);
                                        string ids = "";

                                        foreach (DataRow row in receiveDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            row["VATName"] = bomDt.Rows[0]["VATName"];

                                            ids += "'" + row["ReceiveNo"] + "',";
                                        }


                                        result = commonDal.BulkInsert("ReceiveDetails", receiveDetails, connection, transaction);
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText += "  update ReceiveHeaders set TotalAmount=  ";
                                            sqlText += " (select sum(Quantity*CostPrice) from ReceiveDetails ";
                                            sqlText += " where ReceiveDetails.ReceiveNo =ReceiveHeaders.ReceiveNo) ";
                                            sqlText += " where ReceiveHeaders.ReceiveNo in ( " + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            cmd.ExecuteNonQuery();
                                        }

                                    }

                                    result = commonDal.BulkInsert("IssueHeaders", issueMaster, connection, transaction);

                                    if (result[0].ToLower() == "success")
                                    {
                                        var IssueDetails = GetIssueDetailsData(connection, transaction);
                                        string ids = "";

                                        ProductDAL productDal = new ProductDAL();

                                        foreach (DataRow row in IssueDetails.Rows)
                                        {
                                            DataTable bomDt = new BOMDAL().SelectAll(row["BOMId"].ToString(), null, null, connection, transaction, null, true);

                                            DataTable avgPrice = productDal.AvgPriceNew(row["ItemNo"].ToString().Trim(),
                                                Convert.ToDateTime(row["IssueDateTime"]).ToString("yyyy-MMM-dd") +
                                                DateTime.Now.ToString(" HH:mm:00"), connection, transaction, true, true, false, false, null,UserId);

                                            decimal Amount = Convert.ToDecimal(avgPrice.Rows[0]["Amount"].ToString());
                                            decimal Quantity = Convert.ToDecimal(avgPrice.Rows[0]["Quantity"].ToString());
                                            decimal costPrice = 0;
                                            if (Quantity > 0)
                                            {
                                                costPrice = Amount / Quantity;
                                            }

                                            row["NBRPrice"] = costPrice;
                                            row["CostPrice"] = costPrice;
                                            row["SubTotal"] = costPrice * Convert.ToDecimal(row["Quantity"]);
                                            row["UOMPrice"] = costPrice * Convert.ToDecimal(row["UOMc"]);

                                            row["BOMDate"] = Convert.ToDateTime(bomDt.Rows[0]["EffectDate"]).ToString("MM/dd/yyyy");

                                            ids += "'" + row["IssueNo"] + "',";

                                        }

                                        result = commonDal.BulkInsert("IssueDetails", IssueDetails, connection, transaction);

                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            sqlText = "";
                                            sqlText += " update IssueHeaders set ";
                                            sqlText += " TotalAmount= (select sum(Quantity*CostPrice) from IssueDetails";
                                            sqlText += "  where IssueDetails.IssueNo =IssueHeaders.IssueNo)";
                                            sqlText += " where IssueHeaders.IssueNo in (" + ids.TrimEnd(',') + ")";

                                            cmd.CommandText = sqlText;

                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                #endregion


                                #region Update Sales PeriodId

                                cmd.CommandText = @"update  SalesInvoiceHeaders                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = ''

update  SalesInvoiceDetails                             
set PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(InvoiceDateTime)) +  CONVERT(VARCHAR(4),YEAR(InvoiceDateTime)),6)
where PeriodId is null or PeriodId = '' ";


                                int transResult = cmd.ExecuteNonQuery();

                                #endregion


                            }
                            else
                            {
                                throw new Exception("Details Import Failed");
                            }

                            callBack();
                        }
                        else
                        {
                            throw new Exception("Master Import Failed");
                        }
                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }


                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                #endregion

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                //transaction.Commit();


                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }

        public string[] SaveAndProcess_WithOutBulk(DataTable data, Action callBack = null, int branchId = 1, string app = "", SysDBInfoVMTemp connVM = null, SqlConnection vConnection = null, SqlTransaction vTransaction = null, string UserId = "")
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                #region Connection and Transaction

                if (vConnection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    connection.Open();
                }
                else
                {
                    connection = vConnection;
                }
                if (vTransaction == null)
                {
                    transaction = connection.BeginTransaction();
                }
                else
                {
                    transaction = vTransaction;
                }

                #endregion
                var commonDal = new CommonDAL();

                #region Merge Date Time

                if (data.Columns.Contains("Invoice_Date") && data.Columns.Contains("Invoice_Time"))
                {
                    if (!data.Columns.Contains("Invoice_Date_Time"))
                    {
                        data.Columns.Add("Invoice_Date_Time");
                    }

                    foreach (DataRow dataRow in data.Rows)
                    {
                        dataRow["Invoice_Date_Time"] = OrdinaryVATDesktop.DateToDate_YMD(dataRow["Invoice_Date"].ToString()) + " " + OrdinaryVATDesktop.DateToTime_HMS(dataRow["Invoice_Time"].ToString());
                    }

                    data.Columns.Remove("Invoice_Date");
                    data.Columns.Remove("Invoice_Time");
                }
                else
                {
                    OrdinaryVATDesktop.DataTable_DateFormat(data, "Invoice_Date_Time");
                    if (data.Columns.Contains("Delivery_Date_Time"))
                    {
                        OrdinaryVATDesktop.DataTable_DateFormat(data, "Delivery_Date_Time");
                    }
                }

                #endregion


                result = ImportBigData(data, true, connection, transaction, null, branchId);

                var uomcText = @"select top 1 * from SalesTempData  where UOMc = 0 or UOMc  is null";
                var uomCmd = new SqlCommand(uomcText, connection, transaction);
                var uomAdapter = new SqlDataAdapter(uomCmd);
                var uomcTable = new DataTable();

                uomAdapter.Fill(uomcTable);

                if (uomcTable.Rows.Count > 0)
                {
                    throw new Exception("UOM Conversion not found for " + uomcTable.Rows[0]["Item_Code"] + "-" +
                                        uomcTable.Rows[0]["Item_Name"]);
                }


                if (result[0].ToLower() == "success")
                {
                    callBack();

                    result = SaveInvoiceIdSaleTemp(0, connection, transaction);


                    if (result[0].ToLower() == "success")
                    {
                        callBack();

                        DataTable masters = GetMasterData(connection, transaction, app, null, null, true);
                        callBack();

                        DataTable details = GetDetailsDataWithId(connection, transaction);

                        List<SaleMasterVM> masterVMS = OrdinaryVATDesktop.GetListFromDataTable<List<SaleMasterVM>>(masters);
                        List<SaleDetailVm> detailVMs = OrdinaryVATDesktop.GetListFromDataTable<List<SaleDetailVm>>(details);

                        foreach (SaleMasterVM saleMasterVm in masterVMS)
                        {
                            saleMasterVm.VehicleNo =
                                (string.IsNullOrEmpty(saleMasterVm.VehicleID) ||
                                 Convert.ToDecimal(saleMasterVm.VehicleID) <= 0)
                                    ? "-"
                                    : saleMasterVm.VehicleNo;

                            List<SaleDetailVm> filterDetails =
                                detailVMs.Where(x => x.Id == saleMasterVm.ImportIDExcel).ToList();

                            saleMasterVm.LastModifiedBy = "";
                            saleMasterVm.LastModifiedBy = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            result = SalesInsert(saleMasterVm, filterDetails, new List<SaleExportVM>(), new List<TrackingVM>(),
                                transaction, connection, branchId,connVM,UserId);

                            if (result[0].ToLower() == "fail")
                            {
                                throw new Exception("sale Insert failed " + result[1]);
                            }
                        }

                        callBack();

                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }


                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    if (vTransaction == null && transaction != null)
                    {
                        transaction.Commit();
                    }
                }


                return result;
            }
            #region Catch and Finally

            catch (Exception e)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";

                FileLogger.Log("SaleDAL124", "SaveAndProcess", e.Message + " \n" + e.StackTrace);
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open && vConnection == null)
                {
                    connection.Close();

                }

            }
            #endregion
        }
        //currConn to VcurrConn 25-Aug-2020
        private string[] CodegenAndSave(SqlConnection VcurrConn, SqlTransaction Vtransaction, CommonDAL commonDal)
        {
            string[] result;
            string sqlText = "";
            var cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);
            cmd.CommandTimeout = 100;
            result = SaveInvoiceIdSaleTemp(0, VcurrConn, Vtransaction);

            sqlText =
                @"Select * from SalesTempData order by Id";

            cmd.CommandText = sqlText;
            var codeGen = new DataTable();

            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(codeGen);

            CodeGenerationForSale(codeGen, VcurrConn, Vtransaction);
            var oldId = "";
            var newId = "";
            var invoiceId = "";
            var commonDAl = new CommonDAL();


            sqlText = "delete from SalesTempData; DBCC CHECKIDENT ('SalesTempData', RESEED, 0);";

            cmd.CommandText = sqlText;
            cmd.ExecuteNonQuery();

            result = commonDal.BulkInsert("SalesTempData", codeGen, VcurrConn, Vtransaction);
            return result;
        }
        //currConn to VcurrConn 25-Aug-2020
        private void CheckUOMConversion(SqlConnection VcurrConn, SqlTransaction Vtransaction)
        {
            var uomcText = @"select top 1 * from SalesTempData  where UOMc = 0 or UOMc  is null";
            var uomCmd = new SqlCommand(uomcText, VcurrConn, Vtransaction);
            var uomAdapter = new SqlDataAdapter(uomCmd);
            var uomcTable = new DataTable();

            uomAdapter.Fill(uomcTable);


            if (uomcTable.Rows.Count > 0)
            {
                throw new Exception("UOM Conversion not found for " + uomcTable.Rows[0]["Item_Code"] + "-" +
                                    uomcTable.Rows[0]["Item_Name"]);
            }
        }

        public string[] SaveAndProcessTempData(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();

                result = ProcessBigData(connection, transaction, null, branchId);

                if (result[0].ToLower() == "success")
                {
                    callBack();


                    //                    var sqlText = @"select top 1 ID from SalesTempData 
                    //                                    where (SD_Rate > 0 and SDAmount <= 0) 
                    //                                    or  (VAT_Rate > 0 and VAT_Amount <=0)";

                    string sqlText = "";
                    var cmd = new SqlCommand(sqlText, connection, transaction);

                    //                    var val = cmd.ExecuteScalar();

                    //                    if(val != null)
                    //                    {
                    //                        throw new Exception("Please Provide Vat amount and SD Amount of "+val.ToString());
                    //                    }

                    //var invoiceId = GetFisrtInvoiceId(connection, transaction);

                    //var id = Convert.ToInt32(invoiceId[2].Split('-')[1].Split('/')[0]);

                    result = SaveInvoiceIdSaleTemp(0, connection, transaction);


                    sqlText =
                     @"Select * from SalesTempData order by Id";

                    cmd.CommandText = sqlText;
                    var codeGen = new DataTable();

                    var adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(codeGen);

                    //codeGen = CodeGenerationForSale(codeGen, connection, transaction);
                    CodeGenerationForSale(codeGen, connection, transaction);
                    var oldId = "";
                    var newId = "";
                    var invoiceId = "";
                    var commonDAl = new CommonDAL();

                    //foreach (DataRow row in codeGen.Rows)
                    //{
                    //    newId = row["ID"].ToString();

                    //    if (newId == oldId)
                    //    {
                    //        row["SalesInvoiceNo"] = invoiceId;
                    //    }
                    //    else
                    //    {
                    //        invoiceId = commonDAl.CodeGeneration("Sale", "Other", row["Invoice_Date_Time"].ToString(),
                    //            row["BranchId"].ToString(), connection, transaction);

                    //        row["SalesInvoiceNo"] = invoiceId;
                    //        oldId = newId;
                    //    }
                    //}


                    sqlText = "delete from SalesTempData";

                    cmd.CommandText = sqlText;
                    cmd.ExecuteNonQuery();

                    result = commonDal.BulkInsert("SalesTempData", codeGen, connection, transaction);


                    if (result[0].ToLower() == "success")
                    {
                        callBack();
                        var masters = GetMasterData(connection, transaction);

                        result = ImportSalesBigData(masters, true, connection, transaction);

                        if (result[0].ToLower() == "success")
                        {
                            callBack();
                            var details = GetDetailsData(connection, transaction);


                            result = commonDal.BulkInsert("SalesInvoiceDetails", details, connection, transaction);

                            if (result[0].ToLower() != "success")
                            {
                                throw new Exception("Details Import Failed");
                            }

                            callBack();
                        }
                        else
                        {
                            throw new Exception("Master Import Failed");
                        }
                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }


                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }

        public string[] SaveAndProcessOtherDb(DataTable data, Action callBack = null, int branchId = 1, SysDBInfoVMTemp connVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };
            try
            {
                connection = _dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();

                result = ImportBigData(data, true, connection, transaction, null, branchId);

                if (result[0].ToLower() == "success")
                {
                    //callBack();

                    //var invoiceId = GetFisrtInvoiceId(connection, transaction);

                    //var id = Convert.ToInt32(invoiceId[2].Split('-')[1].Split('/')[0]);

                    //result = SaveInvoiceIdSaleTemp(id, connection, transaction);


                    if (result[0].ToLower() == "success")
                    {
                        callBack();
                        var masters = GetMasterDataOtherDb(connection, transaction);

                        result = ImportSalesBigData(masters, true, connection, transaction);

                        if (result[0].ToLower() == "success")
                        {
                            callBack();
                            var details = GetDetailsDataOtherDb(connection, transaction);


                            result = commonDal.BulkInsert("SalesInvoiceDetails", details, connection, transaction);

                            if (result[0].ToLower() != "success")
                            {
                                throw new Exception("Details Import Failed");
                            }

                            callBack();
                        }
                        else
                        {
                            throw new Exception("Master Import Failed");
                        }
                    }
                    else
                    {
                        throw new Exception("Sale Temp Invoice update Failed");
                    }


                }
                else
                {
                    throw new Exception("Sale Temp Import Failed");
                }

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();

                }

                result[0] = "fail";
                throw e;
            }
            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
        }

        public DataTable GetDetailsData(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string userId = null, string token = null)
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

                #region SQL Text

                sqlText = @"
SELECT distinct 
[SalesInvoiceNo]										AS SalesInvoiceNo
,[Invoice_Date_Time] 									AS InvoiceDateTime
,row_number() OVER (ORDER BY SalesInvoiceNo) 			AS InvoiceLineNo
,[Sale_Type] 											AS SaleType
,[Previous_Invoice_No] 									AS PreviousSalesInvoiceNo
,[Post]													AS Post
,sum([Quantity]) 										AS Quantity
,[NBR_Price] 											AS NBRPrice
,[UOM]													AS UOM
,[VAT_Rate] 											AS VATRate
,[SD_Rate] 												AS SD
,[Non_Stock] 											AS NonStock
,[Trading_MarkUp] 										AS TradingMarkUp
,[VAT_Name] 											AS VATName
,sum([SubTotal]) 										AS SubTotal
,[ItemNo]												AS ItemNo
,[CommentsD] 											AS Comments
,[BranchId]												AS BranchId
,sum([VAT_Amount]) 										AS VATAmount
,[CreatedBy]											AS CreatedBy
,[CreatedOn]											AS CreatedOn
,[BOMId]												AS BOMId
,[TransactionType]										AS TransactionType
,[ReturnId] 											AS SaleReturnId
,[UOMPrice]												AS UOMPrice
,[UOMc]													AS UOMc
,[UOMn]													AS UOMn
,sum([UOMQty]) 											AS UOMQty
,sum([SDAmount])  										AS SDAmount
,[Type]													AS Type
,sum(Discount_Amount)  									AS DiscountAmount
,sum(Promotional_Quantity)  							AS PromotionalQuantity
,[Weight]												AS Weight
,WareHouseRent											AS WareHouseRent
,WareHouseVAT											AS WareHouseVAT
,ATVRate												AS ATVRate
,ATVablePrice											AS ATVablePrice
,sum(ATVAmount)											AS ATVAmount
,TradeVATRate											AS TradeVATRate
,sum(TradeVATAmount)									AS TradeVATAmount
,sum(VDSAmountD	)										AS VDSAmount
,sum(CDNVATAmount)										AS CDNVATAmount
,sum(CDNSDAmount)										AS CDNSDAmount
,sum(CDNSubtotal)										AS CDNSubtotal
,BENumber												AS BENumber
,TradingD 												AS Trading
,CConversionDate 										AS CConversionDate
,SalesPrice 											AS SalesPrice
,TradeVATableValue 										AS TradeVATableValue
,sum(TotalValue) 										AS TotalValue
,IsCommercialImporter 									AS IsCommercialImporter
,sum(DollerValue) 										AS DollerValue
,sum(CurrencyValue)  									AS CurrencyValue
,ValueOnly  											AS ValueOnly
,DiscountedNBRPrice  									AS DiscountedNBRPrice
,ExtraCharge 											AS ExtraCharges
,OtherRef 												AS OtherRef
,PreviousNBRPrice										AS PreviousNBRPrice
,sum(PreviousQuantity)									AS PreviousQuantity
,sum(PreviousSubTotal)									AS PreviousSubTotal
,PreviousSD	 											AS PreviousSD
,sum(PreviousSDAmount)									AS PreviousSDAmount
,PreviousVATRate										AS PreviousVATRate
,sum(PreviousVATAmount)									AS PreviousVATAmount
,PreviousUOM	  										AS PreviousUOM
,PreviousInvoiceDateTime								AS PreviousInvoiceDateTime
,ReasonOfReturn											AS ReasonOfReturn
,sum(SourcePaidVATAmount)								AS SourcePaidVATAmount
,sum(SourcePaidQuantity)								AS SourcePaidQuantity
,sum(NBRPriceInclusiveVAT)								AS NBRPriceInclusiveVAT
,IsLeader								                AS IsLeader
,sum(LeaderAmount)								        AS LeaderAmount
,sum(LeaderVATAmount)								    AS LeaderVATAmount
,sum(NonLeaderAmount)								    AS NonLeaderAmount
,sum(NonLeaderVATAmount)								AS NonLeaderVATAmount
,sum(LineTotal)								AS LineTotal

FROM [SalesTempData]   

 
  "; //InvoiceLineNo



                if (userId != null)
                {
                    sqlText += " where UserId = " + userId;
                }

                if (token != null)
                {
                    sqlText += " where token = '" + token + "'";
                }

                #region Group By


                sqlText += @"  
GROUP BY 
ItemNo
,[SalesInvoiceNo]
,[Invoice_Date_Time] 
      ,[Sale_Type] 
      ,[Previous_Invoice_No] 
      ,[Post]
      ,[NBR_Price] 
      ,[UOM]
      ,[VAT_Rate] 
      ,[SD_Rate] 
      ,[Non_Stock] 
      ,[Trading_MarkUp] 
      ,[VAT_Name] 
      ,[ItemNo]
      ,[CommentsD] 
      ,[BranchId]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[BOMId]
      ,[TransactionType]
      ,[ReturnId] 
      ,[UOMPrice]
      ,[UOMc]
      ,[UOMn]
      ,[Type]
       ,[Weight]
       ,WareHouseRent
       ,WareHouseVAT
       ,ATVRate
       ,ATVablePrice
       ,TradeVATRate
       ,BENumber
       ,TradingD 
       ,CConversionDate 
       ,SalesPrice 
       ,TradeVATableValue 
       ,IsCommercialImporter 
      ,ExtraCharge
       ,ValueOnly  
       ,DiscountedNBRPrice 
       ,OtherRef 
	   ,PreviousNBRPrice	
	   ,PreviousSD		
	   ,PreviousVATRate	
	   ,PreviousUOM	
       ,PreviousInvoiceDateTime
       ,ReasonOfReturn
       ,NBRPriceInclusiveVAT
       ,IsLeader
       ,LineTotal

";
                #endregion

                #endregion

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.CommandTimeout = 500;

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                //var count = dt.Rows.Count;
                //for (int i = 1; i <= count; i++)
                //{
                //    dt.Rows[i - 1]["InvoiceLineNo"] = i;
                //}
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

            return dt;

            #endregion
        }

        public DataTable GetDetailsDataWithId(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string userId = null)
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


                sqlText = @"SELECT distinct [ID]
      ,[Invoice_Date_Time] InvoiceDateTime
    ,row_number() OVER (ORDER BY ID) InvoiceLineNo

      ,[Sale_Type] SaleTypeD
      ,[Previous_Invoice_No] PreviousSalesInvoiceNo

      ,[Post]

      ,sum([Quantity]) Quantity
      ,[NBR_Price] NBRPrice
      ,[UOM]
      ,[VAT_Rate] VATRate
      ,[SD_Rate] SD
      ,[Non_Stock] NonStock
      ,[Trading_MarkUp] TradingMarkUp

      ,[VAT_Name] VATName
      ,sum([SubTotal]) SubTotal

      ,[ItemNo]

      ,[CommentsD] Comments
      ,[BranchId]


      ,sum([VAT_Amount]) VATAmount

      ,[CreatedBy]
      ,[CreatedOn]
      ,[BOMId]
      ,[TransactionType]
      ,[ReturnId] SaleReturnId
      ,[UOMPrice]
      ,[UOMc]
      ,[UOMn]
      ,sum([UOMQty]) UOMQty
      ,sum([SDAmount])  SDAmount
      ,[Type]
	  ,sum(Discount_Amount)  DiscountAmount
	  ,sum(Promotional_Quantity)  PromotionalQuantity
       ,[Weight]
       ,WareHouseRent
       ,WareHouseVAT
       ,ATVRate
       ,ATVablePrice
       ,ATVAmount
       ,TradeVATRate
       ,TradeVATAmount
       ,VDSAmountD VDSAmount
       ,CDNVATAmount
       ,CDNSDAmount
       ,CDNSubtotal
       ,BENumber
       ,TradingD Trading
       ,CConversionDate 
       ,SalesPrice 
       ,TradeVATableValue 
       ,sum(TotalValue) TotalValue
       ,IsCommercialImporter 
       ,sum(DollerValue) DollerValue
       ,sum(CurrencyValue)  CurrencyValue
       ,ValueOnly  
       ,DiscountedNBRPrice  
,ProductDescription
  FROM [SalesTempData]   

 
  "; //InvoiceLineNo



                if (userId != null)
                {
                    sqlText += " where UserId = " + userId;
                }

                sqlText += @"  group by ItemNo,
[ID]
      ,[Invoice_Date_Time] 


      ,[Sale_Type] 
      ,[Previous_Invoice_No] 

      ,[Post]


      ,[NBR_Price] 
      ,[UOM]
      ,[VAT_Rate] 
      ,[SD_Rate] 
      ,[Non_Stock] 
      ,[Trading_MarkUp] 

      ,[VAT_Name] 
 

      ,[ItemNo]

      ,[CommentsD] 
      ,[BranchId]


 
      ,[CreatedBy]
      ,[CreatedOn]
      ,[BOMId]
      ,[TransactionType]
      ,[ReturnId] 
      ,[UOMPrice]
      ,[UOMc]
      ,[UOMn]


      ,[Type]

       ,[Weight]
       ,WareHouseRent
       ,WareHouseVAT
       ,ATVRate
       ,ATVablePrice
       ,ATVAmount
       ,TradeVATRate
       ,TradeVATAmount
       ,VDSAmountD 
       ,CDNVATAmount
       ,CDNSDAmount
       ,CDNSubtotal
       ,BENumber
       ,TradingD 
       ,CConversionDate 
       ,SalesPrice 
       ,TradeVATableValue 

       ,IsCommercialImporter 

 
       ,ValueOnly  
       ,DiscountedNBRPrice
,ProductDescription";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                //var count = dt.Rows.Count;
                //for (int i = 1; i <= count; i++)
                //{
                //    dt.Rows[i - 1]["InvoiceLineNo"] = i;
                //}
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

            return dt;

            #endregion
        }

        public DataTable GetIssueDetailsData(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string token = null)
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


                sqlText = @"
	   SELECT 
	   SalesTempData.[SalesInvoiceNo] IssueNo

      ,SalesTempData.[Invoice_Date_Time] IssueDateTime



      ,SalesTempData.[Post]
	  ,'TradingAuto' TransactionType

      ,SalesTempData.[Quantity]
	  , 0 NBRPrice
      , 0 CostPrice
	  , BOMs.EffectDate BOMDate

      ,SalesTempData.[UOM]

    , SalesTempData.[SalesInvoiceNo] ReceiveNo
	  ,SalesTempData.ReturnId IssueReturnId
      ,SalesTempData.[ItemNo] FinishItemNo
      ,SalesTempData.[ItemNo] 

      ,SalesTempData.BranchId
	  ,0 SubTotal

      ,SalesTempData.[CreatedBy]
      ,SalesTempData.[CreatedOn]
      ,SalesTempData.[UOMPrice]
      ,SalesTempData.[UOMc]
      ,SalesTempData.[UOMn]
      ,SalesTempData.[UOMQty]
      ,SalesTempData.BOMId
      , 0 SD
      , 0 SDAmount
      ,  0 Wastage
	  , 0 VATRate
	  , 0 VATAmount
  FROM SalesTempData left outer join Products on SalesTempData.ItemNo = Products.ItemNo 
	   left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID
	   left outer join BOMs  on SalesTempData.BOMId = BOMs.BOMId
	   where ProductCategories.IsRaw = 'Trading'  ";


                if (token != null)
                {
                    sqlText += " and token = '" + token + "'";
                }



                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


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

            return dt;

            #endregion
        }

        public DataTable GetReceiveDetailsData(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string token = null)
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


                sqlText = @"
	  SELECT 
	   SalesTempData.[SalesInvoiceNo] ReceiveNo

      ,SalesTempData.[Invoice_Date_Time] ReceiveDateTime



      ,SalesTempData.[Post]
	  ,'TradingAuto' TransactionType

      ,SalesTempData.[Quantity]
      ,SalesTempData.[NBR_Price] NBRPrice
      ,SalesTempData.[NBR_Price] CostPrice
	  ,'' VATName
      ,ProductCategories.IsRaw UOM


      ,SalesTempData.[ItemNo]

      ,SalesTempData.BranchId
	  ,(SalesTempData.[NBR_Price] * SalesTempData.[UOMQty]) SubTotal

      ,SalesTempData.[CreatedBy]
      ,SalesTempData.[CreatedOn]
      ,SalesTempData.[UOMPrice]
      ,SalesTempData.[UOMc]
      ,SalesTempData.[UOMn]
      ,SalesTempData.[UOMQty]
      ,SalesTempData.BOMId,
	     0 SD,
	  0 SDAmount,
	  0 VATRate,
	  0 VATAmount
  FROM SalesTempData left outer join Products on SalesTempData.ItemNo = Products.ItemNo 
	   left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID
	   where ProductCategories.IsRaw = 'Trading'  ";



                if (token != null)
                {
                    sqlText += " and token = '" + token + "'";
                }

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                var count = dt.Rows.Count;
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

            return dt;

            #endregion
        }

        public DataTable GetDetailsDataOtherDb(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                sqlText = @"SELECT ID SalesInvoiceNo
	 ,0	[InvoiceLineNo]
      ,[Invoice_Date_Time] InvoiceDateTime


      ,[Sale_Type] SaleType
      ,[Previous_Invoice_No] PreviousSalesInvoiceNo

      ,[Post]

      ,[Quantity]
      ,[NBR_Price] NBRPrice
      ,[UOM]
      ,[VAT_Rate] VATRate
      ,[SD_Rate] SD
      ,[Non_Stock] NonStock
      ,[Trading_MarkUp] TradingMarkUp

      ,[VAT_Name] VATName
      ,[SubTotal]

      ,[ItemNo]

      ,[CommentsD] Comments
      ,[BranchId]


      ,[VAT_Amount] VATAmount

      ,[CreatedBy]
      ,[CreatedOn]
      ,[BOMId]
      ,[TransactionType]
      ,[ReturnId] SaleReturnId
      ,[UOMPrice]
      ,[UOMc]
      ,[UOMn]
      ,[UOMQty]
      ,[SDAmount]
      ,[Type]
	  ,Discount_Amount  DiscountAmount
	  ,Promotional_Quantity  PromotionalQuantity
       ,[Weight]
       ,WareHouseRent
       ,WareHouseVAT
       ,ATVRate
       ,ATVablePrice
       ,ATVAmount
       ,TradeVATRate
       ,TradeVATAmount
       ,VDSAmountD VDSAmount
       ,CDNVATAmount
       ,CDNSDAmount
       ,CDNSubtotal
       ,BENumber
       ,TradingD Trading
       ,CConversionDate 
       ,SalesPrice 
       ,TradeVATableValue 
       ,TotalValue 
       ,IsCommercialImporter 
       ,DollerValue 
       ,CurrencyValue  
       ,ValueOnly  
       ,DiscountedNBRPrice  
       ,ProductDescription  
  FROM [SalesTempData]";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dt);


                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                var count = dt.Rows.Count;
                for (int i = 1; i <= count; i++)
                {
                    dt.Rows[i - 1]["InvoiceLineNo"] = i;
                }
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

            return dt;

            #endregion
        }

        public DataTable GetSaleExcelData(List<string> invoiceList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
SELECT --top 100000
      sih.[SalesInvoiceNo] ID
      ,bp.BranchCode Branch_Code
	  ,c.CustomerCode Customer_Code
      ,c.CustomerName Customer_Name
	  ,convert(varchar(50),sih.[InvoiceDateTime],111) Invoice_Date
      ,convert(varchar(50),sih.[InvoiceDateTime],108) Invoice_Time
      --,sih.InvoiceDateTime Invoice_Date_Time
	  ,sih.SerialNo Reference_No
	  ,p.ProductCode Item_Code
	  ,p.ProductName Item_Name
	  , sids.Quantity
	  ,sids.UOM
	  , (sids.UOMPrice+isnull(sids.DiscountAmount,0)) NBR_Price
	  --, sids.NBRPrice NBR_Price
      ,sids.Weight
	  ,sids.SubTotal SubTotal 	  	  
	  ,vh.VehicleNo Vehicle_No
	  ,vh.VehicleType	  	  
	  ,cg.CustomerGroupName CustomerGroup
      ,sih.[DeliveryAddress1] Delivery_Address
      ,sih.[Comments]
      ,sih.[SaleType] Sale_Type
      ,sih.[TransactionType] TransactionType

	   ,sih.[IsPrint] Is_Print
      ,sih.[TenderId] Tender_Id

      ,sih.[Post]
      ,sih.[LCNumber] LC_Number
      ,cc.CurrencyCode Currency_Code
      ,sids.Comments CommentsD

	  ,sids.SD SD_Rate
      ,sids.SDAmount
	  ,sids.VATRate VAT_Rate
      ,sids.VATAmount VAT_Amount

	  ,sids.NonStock Non_Stock	  
	  ,sids.TradingMarkUp Trading_MarkUp
	  ,sids.Type 
	  ,sids.DiscountAmount Discount_Amount
	  ,sids.PromotionalQuantity Promotional_Quantity 	  	  
	  ,sids.VATName VAT_Name 	  	  
	  ,sids.ProductDescription
	  ,sids.[PreviousSalesInvoiceNo] Previous_Invoice_No
      ,convert(varchar(50),sids.PreviousInvoiceDateTime,111)PreviousInvoiceDateTime
	  ,isnull(sids.PreviousNBRPrice	 ,0)PreviousNBRPrice
	  ,isnull(sids.PreviousQuantity	 ,0)PreviousQuantity
	  ,isnull(sids.PreviousSubTotal	 ,0)PreviousSubTotal
	  ,isnull(sids.PreviousSD		 ,0)PreviousSD
	  ,isnull(sids.PreviousSDAmount	 ,0)PreviousSDAmount
	  ,isnull(sids.PreviousVATRate	 ,0)PreviousVATRate
	  ,isnull(sids.PreviousVATAmount ,0)PreviousVATAmount
	  ,isnull(sids.PreviousUOM	     ,'-')PreviousUOM
	  ,isnull(sids.ReasonOfReturn	     ,'-')ReasonOfReturn
	  ,'-' ExpDescription 	  	  
	  ,'-' ExpQuantity
	  ,'-' ExpGrossWeight 	  	  
	  ,'-' ExpNetWeight
	  , '-'  ExpNumberFrom
	  , '-' ExpNumberTo
      --,cast(sih.[DeliveryDate] as varchar(100))   Delivery_Date_Time

  FROM SalesInvoiceHeaders sih left outer join  SalesInvoiceDetails sids
  on sih.SalesInvoiceNo = sids.SalesInvoiceNo left outer join BranchProfiles bp
  on sih.BranchId = bp.BranchID left outer join Customers c
  on sih.CustomerID = c.CustomerID left outer join CustomerGroups cg
  on c.CustomerGroupID = cg.CustomerGroupID left outer join Vehicles vh
  on sih.[VehicleID] = vh.[VehicleID] left outer join Products p
  on  sids.ItemNo = p.ItemNo left outer join Currencies cc
  on sih.[CurrencyID] = cc.[CurrencyID]  

  where sih.SalesInvoiceNo in ( ";

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

        public DataTable GetSaleJoin(string invoiceNo, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                sqlText = @"SELECT 
      sih.[SalesInvoiceNo] 
      ,bp.BranchCode 
	  ,cg.CustomerGroupName 
      ,c.CustomerName 
	  ,c.CustomerCode 
	  ,c.CustomerID 
      ,sih.[DeliveryAddress1] 
      ,sih.[DeliveryAddress2] 
      ,sih.[DeliveryAddress3] 
      ,cast(sih.[InvoiceDateTime] as varchar(100))  InvoiceDate
      ,cast(sih.[DeliveryDate] as varchar(100))   DeliveryDate
	 , sih.SerialNo 
      ,sih.[Comments]
      ,sih.[SaleType] 
      ,sih.[PreviousSalesInvoiceNo] 
	   ,sih.[IsPrint] 
      ,sih.[TenderId] 
      ,sih.[DeductionAmount] 
      ,sih.[ShiftId] 
      ,sih.[VehicleID] 
      ,sih.[TotalAmount]
      ,sih.[TotalVATAmount]
      ,sih.[Post]
      ,sih.[LCNumber] 
      ,sih.[LCBank] 
      ,sih.[PINo]
      ,sih.[SaleReturnId]
      ,sih.[PIDate]
      ,sih.[EXPFormNo]
      ,sih.[EXPFormDate]
      ,sih.[Id]
      ,sih.[IsDeemedExport]
      ,sih.[BranchId]
      ,sih.[VDSAmount]
      ,sih.[CurrencyID] 
      ,sih.[Trading] 
      ,sih.[CurrencyRateFromBDT] 
      ,sih.[ImportIDExcel] ImportID 
      ,sih.[AlReadyPrint]  
,isnull(sih.PreviousSalesInvoiceNo,sih.SalesInvoiceNo) PID
      ,cc.CurrencyCode 
      ,sids.Comments 
	  ,p.ProductCode 
	  ,p.ProductName 
	  , sids.Quantity
	  , sids.NBRPrice 
	  , sih.LCDate 
	  ,sids.UOM
	  ,sids.VATRate 
	  ,sids.SD 
	  ,sids.Post  
	  ,sids.NonStock 	  
	  ,sids.TradingMarkUp 
	  ,sids.Type 
	  ,sids.DiscountAmount 
	  ,sids.PromotionalQuantity  	  	  
	  ,sids.VATName  	  	  
	  ,sids.SubTotal  	  	  
      ,sids.[UOMPrice]
      ,sids.[UOMc]
      ,sids.[UOMn]
      ,sids.[UOMQty]
      ,sids.[SDAmount] 	  	  
	  ,vh.VehicleNo 	  	  
	  ,vh.VehicleType 	  	  

  FROM SalesInvoiceHeaders sih left outer join  SalesInvoiceDetails sids
  on sih.SalesInvoiceNo = sids.SalesInvoiceNo left outer join BranchProfiles bp
  on sih.BranchId = bp.BranchID left outer join Customers c
  on sih.CustomerID = c.CustomerID left outer join CustomerGroups cg
  on c.CustomerGroupID = cg.CustomerGroupID left outer join Vehicles vh
  on sih.[VehicleID] = vh.[VehicleID] left outer join Products p
  on  sids.ItemNo = p.ItemNo left outer join Currencies cc
  on sih.[CurrencyID] = cc.[CurrencyID] where  sih.[SalesInvoiceNo] = @invoiceNo ";



                var cmd = new SqlCommand(sqlText, currConn, transaction);

                cmd.Parameters.AddWithValue("@invoiceNo", invoiceNo);

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

        public string[] SaveSalesToKohinoor(DataTable table, DataTable db, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetDepoConnection(db);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @"";


                #endregion

                #region Sql Command

                var len = table.Rows.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "update SalesInvoiceHeaders set VatInvoiceNo = @VatInvoiceNo, IsVatCompleted = 'Y' where SalesInvoiceNo = @SalesInvoiceNo;";
                }

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                for (int i = 0; i < len; i++)
                {
                    cmd.Parameters.AddWithValue("@VatInvoiceNo" + i, table.Rows[i]["SalesInvoiceNo"]);
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo" + i, table.Rows[i]["ID"]);
                }


                var rows = cmd.ExecuteNonQuery();
                #endregion


                transaction.Commit();

                retResults[0] = rows > 0 ? "success" : "fail";

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public string[] SaveSalesToLink3(DataTable table, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {

                    currConn = new OracleConnection(@"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.10.5)(PORT=64819))(CONNECT_DATA=(SERVICE_NAME=PDB_HQDEVDB)));
uid=RFBL_ERP;pwd=RFBL_ERP");//_dbsqlConnection.GetConnectionLink3OLEDB();
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @"";


                #endregion

                #region Sql Command

                var len = table.Rows.Count;
                var t = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                for (int i = 0; i < len; i++)
                {
                    sqlText += "update DELIVERY_ORDER_MASTER set MUSHAK_NO = '" + table.Rows[i]["SalesInvoiceNo"] + "', MUSHAK_DATE = '" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "' where DO_ID = '" + table.Rows[i]["ID"] + "'";
                }

                var cmd = new OracleCommand(sqlText, currConn);
                cmd.Transaction = transaction;

                var rows = cmd.ExecuteNonQuery();
                #endregion


                transaction.Commit();

                retResults[0] = rows > 0 ? "success" : "fail";

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        #endregion

        #region Method 04

        public string[] PostSales(DataTable table, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            OracleConnection currConn = null;
            OracleTransaction transaction = null;
            int transResult = 0;
            #endregion variable

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {

                    currConn = new OracleConnection(@"SERVER=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.10.5)(PORT=64819))(CONNECT_DATA=(SERVICE_NAME=PDB_HQDEVDB)));
uid=RFBL_ERP;pwd=RFBL_ERP");//_dbsqlConnection.GetConnectionLink3OLEDB();
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @"";
                var rows = 0;

                #endregion

                #region Sql Command
                var cmd = new OracleCommand(sqlText, currConn);
                cmd.Transaction = transaction;

                var len = table.Rows.Count;
                var t = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                for (int i = 0; i < len; i++)
                {
                    sqlText = " update DELIVERY_ORDER_MASTER set IS_MUSHAK = 'Y', MUSHAK_DATE = '" + t + "'  where DO_ID = '" + table.Rows[i]["ID"] + "' ";
                    cmd.CommandText = sqlText;
                    rows = cmd.ExecuteNonQuery();
                }


                #endregion


                transaction.Commit();

                retResults[0] = rows > 0 ? "success" : "fail";

                return retResults;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable GetInvoiceNoFromTemp(SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @"select distinct ID, SalesInvoiceNo from SalesTempData";


                #endregion

                #region Sql Command

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                var table = new DataTable();

                adapter.Fill(table);

                #endregion


                transaction.Commit();

                return table;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }
        //currConn to VcurrConn 25-Aug-2020
        public DataTable CodeGenerationForSaleX(DataTable table, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string NewCode = "";
            string CodePreFix = "";
            string CodeGenerationFormat = "";
            string CodeGenerationMonthYearFormat = "";
            string BranchCode = "001";
            string BranchNumber = "";
            string CurrentYear = "2020";

            int CodeLength = 0;
            int nextNumber = 0;
            string sqlText = "";
            string CodeGroup = "Sale", CodeName = "Other", TransactionDate = "", BranchId = "1";
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataSet ds = new DataSet();
            string year = "";
            string OldId = "";
            #endregion

            #region Try

            try
            {
                var rowsCount = table.Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    var OldDate = TransactionDate;
                    var id = table.Rows[i]["Id"].ToString();

                    if (id == OldId)
                    {
                        table.Rows[i]["SalesInvoiceNo"] = NewCode;
                        continue;
                    }

                    OldId = id;
                    TransactionDate = Convert.ToDateTime(table.Rows[i]["Invoice_Date_Time"]).ToString("dd/MMM/yyyy");

                    #region SettingsExist

                    sqlText = "  ";

                    sqlText += " SELECT   top 1  SettingName FROM Settings";
                    sqlText += " WHERE     (SettingGroup ='CodeGenerationFormat') and   (SettingValue ='Y')  "; // one time

                    sqlText += " SELECT   top 1  SettingName FROM Settings";
                    sqlText += " WHERE     (SettingGroup ='CodeGenerationMonthYearFormat') and   (SettingValue ='Y')  "; // one time
                    sqlText += " SELECT   count(BranchCode) BranchNumber FROM BranchProfiles where IsArchive='0' and ActiveStatus='Y'"; // one time

                    sqlText += "  SELECT   * from  CodeGenerations where CurrentYear='2020' ";
                    sqlText += "  select CurrentYear from FiscalYear where '" + Convert.ToDateTime(TransactionDate).ToString("dd/MMM/yyyy") + "' between PeriodStart and PeriodEnd ";

                    var branch_Id = " SELECT   top 1  BranchCode FROM BranchProfiles";
                    branch_Id += " WHERE     (BranchID ='" + BranchId + "')   "; // could be multiple


                    SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                    cmdExist.Transaction = Vtransaction;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdExist);

                    if (string.IsNullOrEmpty(CodeGenerationFormat) ||
                        string.IsNullOrEmpty(CodeGenerationMonthYearFormat) || string.IsNullOrEmpty(BranchNumber) || TransactionDate != OldDate)
                    {

                        dataAdapter.Fill(ds);


                        if (ds.Tables[0] != null || ds.Tables[0].Rows.Count > 0)
                            CodeGenerationFormat = ds.Tables[0].Rows[0][0].ToString();

                        if (ds.Tables[1] != null || ds.Tables[1].Rows.Count > 0)
                            CodeGenerationMonthYearFormat = ds.Tables[1].Rows[0][0].ToString();

                        if (ds.Tables[2] != null || ds.Tables[2].Rows.Count > 0)
                            BranchNumber = ds.Tables[2].Rows[0][0].ToString();

                        if (ds.Tables[3] == null || ds.Tables[3].Rows.Count <= 0)
                        {
                            sqlText = "  ";
                            sqlText += "  update CodeGenerations set CurrentYear ='2020'  where CurrentYear <='2020'";

                            cmdExist = new SqlCommand(sqlText, VcurrConn);
                            cmdExist.Transaction = Vtransaction;
                            cmdExist.ExecuteNonQuery();
                        }
                        if (ds.Tables[4] != null || ds.Tables[4].Rows.Count > 0)
                            CurrentYear = ds.Tables[4].Rows[0][0].ToString();
                    }



                    //if (ds.Tables[2] != null || ds.Tables[2].Rows.Count > 0)
                    //    BranchCode = ds.Tables[2].Rows[0][0].ToString();



                    var currentBranchId = table.Rows[i]["BranchId"].ToString();

                    if (BranchId != currentBranchId)
                    {
                        cmdExist.CommandText = branch_Id;
                        BranchId = currentBranchId;
                        BranchCode = cmdExist.ExecuteScalar().ToString();
                    }



                    try
                    {
                        if (string.IsNullOrEmpty(CodePreFix))
                        {
                            sqlText = "  ";

                            sqlText += " SELECT     * FROM Codes";
                            sqlText += " WHERE     (CodeGroup =@CodeGroup) AND (CodeName = @CodeName)";

                            cmdExist = new SqlCommand(sqlText, VcurrConn);
                            cmdExist.Transaction = Vtransaction;


                            cmdExist.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                            cmdExist.Parameters.AddWithValue("@CodeName", CodeName);

                            dataAdapter = new SqlDataAdapter(cmdExist);
                            dataAdapter.Fill(dt1);
                            if (dt1 == null || dt1.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.CodeNotFound, "CodeNotFound");
                            }
                            else
                            {
                                CodePreFix = dt1.Rows[0]["prefix"].ToString();
                                CodeLength = Convert.ToInt32(dt1.Rows[0]["Lenth"]);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        FileLogger.Log("SaleDAL", "CodeGenerationForSale", e.Message + "\n" + e.StackTrace);
                    }



                    //var currentYear = Convert.ToDateTime(TransactionDate).ToString("yyyy");

                    if (CurrentYear != year || nextNumber == 1)
                    {
                        sqlText = "  ";
                        sqlText += " SELECT * FROM CodeGenerations ";
                        sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix";
                        year = CurrentYear;


                        cmdExist = new SqlCommand(sqlText, VcurrConn);
                        cmdExist.Transaction = Vtransaction;

                        cmdExist.Parameters.AddWithValue("@BranchId", BranchId);
                        cmdExist.Parameters.AddWithValue("@CurrentYear", year);
                        cmdExist.Parameters.AddWithValue("@Prefix", CodePreFix);


                        dataAdapter = new SqlDataAdapter(cmdExist);
                        dataAdapter.Fill(dt2);


                        if (dt2 == null || dt2.Rows.Count <= 0)
                        {
                            sqlText = "  ";
                            sqlText +=
                                " INSERT INTO CodeGenerations(	CurrentYear,BranchId,Prefix,LastId)";
                            sqlText += " VALUES(";
                            sqlText += " @CurrentYear,";
                            sqlText += " @BranchId,";
                            sqlText += " @Prefix,";
                            sqlText += " 1";
                            sqlText += " )";

                            SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                            cmdExist1.Transaction = Vtransaction;

                            cmdExist1.Parameters.AddWithValue("@BranchId", BranchId);
                            cmdExist1.Parameters.AddWithValue("@CurrentYear", year);
                            cmdExist1.Parameters.AddWithValue("@Prefix", CodePreFix);

                            object objfoundId1 = cmdExist1.ExecuteNonQuery();

                            nextNumber = 1;
                        }
                        else
                        {
                            nextNumber = Convert.ToInt32(dt2.Rows[0]["LastId"]) + 1;
                        }


                    }
                    else
                    {
                        nextNumber += 1;
                    }


                    var t1 = CodeGenerationFormat;
                    var t2 = CodeGenerationMonthYearFormat;
                    var t3 = BranchCode;
                    var t4 = BranchNumber;
                    CodeGenerationMonthYearFormat = CodeGenerationMonthYearFormat.Replace("Y", "y");
                    if (Convert.ToInt32(BranchNumber) <= 1)
                    {
                        CodeGenerationFormat = CodeGenerationFormat.Replace("B/", "");
                    }
                    CodeGenerationFormat = CodeGenerationFormat.Substring(0, CodeGenerationFormat.Length - 1);

                    var my = Convert.ToDateTime(TransactionDate).ToString(CodeGenerationMonthYearFormat);
                    var nextNumb = nextNumber.ToString().PadLeft(CodeLength, '0');
                    CodeGenerationFormat = CodeGenerationFormat.Replace("N", nextNumb);
                    CodeGenerationFormat = CodeGenerationFormat.Replace("Y", my);
                    CodeGenerationFormat = CodeGenerationFormat.Replace("B", BranchCode);

                    NewCode = CodePreFix + "-" + CodeGenerationFormat;

                    CodeGenerationFormat = t1;

                    table.Rows[i]["SalesInvoiceNo"] = NewCode;

                    #endregion Last Price
                }


                sqlText = "  ";
                sqlText += " update  CodeGenerations set LastId='" + nextNumber + "'";
                sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix";


                var cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);


                cmd.Parameters.AddWithValue("@BranchId", BranchId);
                cmd.Parameters.AddWithValue("@CurrentYear", year);
                cmd.Parameters.AddWithValue("@Prefix", CodePreFix);
                var rows = cmd.ExecuteNonQuery();



            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                FileLogger.Log("SaleDAL", "CodeGen", ex.Message);
                //throw ex;
            }

            finally
            {

            }


            #endregion

            #region Results

            return table;
            #endregion

        }
        //currConn to VcurrConn 25-Aug-2020
        public DataTable CodeGenerationForSale1(DataTable table, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string NewCode = "";
            string CodePreFix = "";
            string CodeGenerationFormat = "";
            string CodeGenerationMonthYearFormat = "";
            string BranchCode = "001";
            string BranchNumber = "";
            string CurrentYear = "2020";

            int CodeLength = 0;
            int nextNumber = 0;
            string sqlText = "";
            string CodeGroup = "Sale", CodeName = "Other", TransactionDate = "", BranchId = "1";
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            DataSet ds = new DataSet();
            string year = "";
            string OldId = "";
            #endregion

            #region Try

            try
            {
                var rowsCount = table.Rows.Count;
                for (int i = 0; i < rowsCount; i++)
                {
                    var OldDate = TransactionDate;
                    var id = table.Rows[i]["Id"].ToString();

                    if (id == OldId)
                    {
                        table.Rows[i]["SalesInvoiceNo"] = NewCode;
                        continue;
                    }

                    OldId = id;
                    TransactionDate = Convert.ToDateTime(table.Rows[i]["Invoice_Date_Time"]).ToString("dd/MMM/yyyy");

                    #region SettingsExist

                    sqlText = "  ";

                    sqlText += " SELECT   top 1  SettingName FROM Settings";
                    sqlText += " WHERE     (SettingGroup ='CodeGenerationFormat') and   (SettingValue ='Y')  ";

                    sqlText += " SELECT   top 1  SettingName FROM Settings";
                    sqlText += " WHERE     (SettingGroup ='CodeGenerationMonthYearFormat') and   (SettingValue ='Y')  ";

                    sqlText += " SELECT   top 1  BranchCode FROM BranchProfiles";
                    sqlText += " WHERE     (BranchID ='" + BranchId + "')   ";

                    sqlText += " SELECT   count(BranchCode) BranchNumber FROM BranchProfiles where IsArchive='0' and ActiveStatus='Y'";

                    sqlText += "  SELECT   * from  CodeGenerations where CurrentYear='2020' ";
                    sqlText += "  select CurrentYear from FiscalYear where '" + Convert.ToDateTime(TransactionDate).ToString("dd/MMM/yyyy") + "' between PeriodStart and PeriodEnd ";

                    SqlCommand cmdExist = new SqlCommand(sqlText, VcurrConn);
                    cmdExist.Transaction = Vtransaction;
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(cmdExist);
                    ds = new DataSet();
                    dataAdapter.Fill(ds);




                    if (ds.Tables[0] != null || ds.Tables[0].Rows.Count > 0)
                        CodeGenerationFormat = ds.Tables[0].Rows[0][0].ToString();

                    if (ds.Tables[1] != null || ds.Tables[1].Rows.Count > 0)
                        CodeGenerationMonthYearFormat = ds.Tables[1].Rows[0][0].ToString();
                    if (ds.Tables[2] != null || ds.Tables[2].Rows.Count > 0)
                        BranchCode = ds.Tables[2].Rows[0][0].ToString();

                    if (ds.Tables[3] != null || ds.Tables[3].Rows.Count > 0)
                        BranchNumber = ds.Tables[3].Rows[0][0].ToString();

                    if (ds.Tables[4] == null || ds.Tables[4].Rows.Count <= 0)
                    {
                        sqlText = "  ";
                        sqlText += "  update CodeGenerations set CurrentYear ='2020'  where CurrentYear <='2020'";

                        cmdExist = new SqlCommand(sqlText, VcurrConn);
                        cmdExist.Transaction = Vtransaction;
                        cmdExist.ExecuteNonQuery();
                    }
                    if (ds.Tables[5] != null || ds.Tables[5].Rows.Count > 0)
                        CurrentYear = ds.Tables[5].Rows[0][0].ToString();




                    //if (ds.Tables[2] != null || ds.Tables[2].Rows.Count > 0)
                    //    BranchCode = ds.Tables[2].Rows[0][0].ToString();



                    //var currentBranchId = table.Rows[i]["BranchId"].ToString();

                    //if (BranchId != currentBranchId)
                    //{
                    //    cmdExist.CommandText = branch_Id;
                    //    BranchId = currentBranchId;
                    //    BranchCode = cmdExist.ExecuteScalar().ToString();
                    //}



                    try
                    {
                        if (string.IsNullOrEmpty(CodePreFix))
                        {
                            sqlText = "  ";

                            sqlText += " SELECT     * FROM Codes";
                            sqlText += " WHERE     (CodeGroup =@CodeGroup) AND (CodeName = @CodeName)";

                            cmdExist = new SqlCommand(sqlText, VcurrConn);
                            cmdExist.Transaction = Vtransaction;


                            cmdExist.Parameters.AddWithValue("@CodeGroup", CodeGroup);
                            cmdExist.Parameters.AddWithValue("@CodeName", CodeName);
                            dt1 = new DataTable();

                            dataAdapter = new SqlDataAdapter(cmdExist);
                            dataAdapter.Fill(dt1);
                            if (dt1 == null || dt1.Rows.Count <= 0)
                            {
                                throw new ArgumentNullException(MessageVM.CodeNotFound, "CodeNotFound");
                            }
                            else
                            {
                                CodePreFix = dt1.Rows[0]["prefix"].ToString();
                                CodeLength = Convert.ToInt32(dt1.Rows[0]["Lenth"]);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        FileLogger.Log("SaleDAL", "CodeGenerationForSale", e.Message + "\n" + e.StackTrace);
                    }



                    //var currentYear = Convert.ToDateTime(TransactionDate).ToString("yyyy");

                    //if (CurrentYear != year || nextNumber == 1)
                    if (nextNumber == 0)
                    {
                        sqlText = "  ";
                        sqlText += " SELECT  top 1 * FROM CodeGenerations ";
                        sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix order by LastId Desc";
                        year = CurrentYear;


                        cmdExist = new SqlCommand(sqlText, VcurrConn);
                        cmdExist.Transaction = Vtransaction;

                        cmdExist.Parameters.AddWithValue("@BranchId", BranchId);
                        cmdExist.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                        cmdExist.Parameters.AddWithValue("@Prefix", CodePreFix);

                        dt2 = new DataTable();
                        dataAdapter = new SqlDataAdapter(cmdExist);
                        dataAdapter.Fill(dt2);


                        if (dt2 == null || dt2.Rows.Count <= 0)
                        {
                            sqlText = "  ";
                            sqlText +=
                                " INSERT INTO CodeGenerations(	CurrentYear,BranchId,Prefix,LastId)";
                            sqlText += " VALUES(";
                            sqlText += " @CurrentYear,";
                            sqlText += " @BranchId,";
                            sqlText += " @Prefix,";
                            sqlText += " 1";
                            sqlText += " )";

                            SqlCommand cmdExist1 = new SqlCommand(sqlText, VcurrConn);
                            cmdExist1.Transaction = Vtransaction;

                            cmdExist1.Parameters.AddWithValue("@BranchId", BranchId);
                            cmdExist1.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                            cmdExist1.Parameters.AddWithValue("@Prefix", CodePreFix);

                            object objfoundId1 = cmdExist1.ExecuteNonQuery();

                            nextNumber = 1;
                        }
                        else
                        {
                            var ttt = dt2.Rows[0]["LastId"];
                            nextNumber = Convert.ToInt32(dt2.Rows[0]["LastId"]) + 1;
                        }


                    }
                    else
                    {
                        nextNumber += 1;
                    }


                    var t1 = CodeGenerationFormat;
                    var t2 = CodeGenerationMonthYearFormat;
                    var t3 = BranchCode;
                    var t4 = BranchNumber;
                    CodeGenerationMonthYearFormat = CodeGenerationMonthYearFormat.Replace("Y", "y");
                    if (Convert.ToInt32(BranchNumber) <= 1)
                    {
                        CodeGenerationFormat = CodeGenerationFormat.Replace("B/", "");
                    }
                    CodeGenerationFormat = CodeGenerationFormat.Substring(0, CodeGenerationFormat.Length - 1);

                    var my = Convert.ToDateTime(TransactionDate).ToString(CodeGenerationMonthYearFormat);
                    var nextNumb = nextNumber.ToString().PadLeft(CodeLength, '0');
                    CodeGenerationFormat = CodeGenerationFormat.Replace("N", nextNumb);
                    CodeGenerationFormat = CodeGenerationFormat.Replace("Y", my);
                    CodeGenerationFormat = CodeGenerationFormat.Replace("B", BranchCode);

                    NewCode = CodePreFix + "-" + CodeGenerationFormat;

                    CodeGenerationFormat = t1;

                    table.Rows[i]["SalesInvoiceNo"] = NewCode;

                    #endregion Last Price
                }


                sqlText = "  ";
                sqlText += " update  CodeGenerations set LastId='" + nextNumber + "'";
                sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix";


                var cmd = new SqlCommand(sqlText, VcurrConn, Vtransaction);


                cmd.Parameters.AddWithValue("@BranchId", BranchId);
                cmd.Parameters.AddWithValue("@CurrentYear", CurrentYear);
                cmd.Parameters.AddWithValue("@Prefix", CodePreFix);
                var rows = cmd.ExecuteNonQuery();



            }

            #endregion try

            #region Catch and Finall

            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                FileLogger.Log("SaleDAL", "CodeGen", ex.Message);
                //throw ex;
            }

            finally
            {

            }


            #endregion

            #region Results

            return table;
            #endregion

        }
        //currConn to VcurrConn 25-Aug-2020
        public void CodeGenerationForSale(DataTable table, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                var oldId = "";
                var newId = "";
                var invoiceId = "";
                var commonDAl = new CommonDAL();

                if (table == null || table.Rows.Count == 0)
                {
                    return;
                }


                var branchId = table.Rows[0]["BranchId"].ToString();
                string transactionType = table.Rows[0]["TransactionType"].ToString();

                string codeGroup = "Sale";

                if (transactionType.ToLower() == "tollissue")
                {
                    codeGroup = "TollIssue";
                }

                var firstInvoice = commonDAl.CodeGeneration(codeGroup, transactionType, table.Rows[0]["Invoice_Date_Time"].ToString(),
                    branchId, VcurrConn, Vtransaction, true);
                //INV-1001/0720
                var dataArray = firstInvoice.Split('~');
                var length = Convert.ToInt32(dataArray[4]);
                var codePreFix = dataArray[6];
                var nextId = Convert.ToInt32(dataArray[5]);
                var currentYear = dataArray[8];


                for (var i = 0; i < table.Rows.Count; i++)
                {
                    DataRow row = table.Rows[i];

                    newId = row["ID"].ToString();
                    //1
                    if (newId == oldId && transactionType == table.Rows[i]["TransactionType"].ToString())
                    {
                        row["SalesInvoiceNo"] = invoiceId;
                    }
                    else
                    {//2
                        string transactionDate = table.Rows[i]["Invoice_Date_Time"].ToString();

                        if (branchId != table.Rows[i]["BranchId"].ToString() || transactionType != table.Rows[i]["TransactionType"].ToString())
                        {
                            UpdateCodeGen(VcurrConn, Vtransaction, nextId - 1, branchId, currentYear, codePreFix);

                            branchId = table.Rows[i]["BranchId"].ToString();
                            transactionType = table.Rows[i]["TransactionType"].ToString();

                            firstInvoice = commonDAl.CodeGeneration("Sale", transactionType, transactionDate,
                               branchId, VcurrConn, Vtransaction, true);

                            dataArray = firstInvoice.Split('~');
                            length = Convert.ToInt32(dataArray[4]);
                            codePreFix = dataArray[6];
                            nextId = Convert.ToInt32(dataArray[5]);
                            currentYear = dataArray[8];


                        }

                        invoiceId = commonDAl.CodeGeneration1(dataArray[0], dataArray[1], dataArray[2], dataArray[3],
                            length,
                            nextId, codePreFix, transactionDate);

                        row["SalesInvoiceNo"] = invoiceId;
                        oldId = newId;

                        nextId++;
                    }
                }

                UpdateCodeGen(VcurrConn, Vtransaction, nextId - 1, branchId, currentYear, codePreFix);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //currConn to VcurrConn 25-Aug-2020
        private void UpdateCodeGen(SqlConnection VcurrConn, SqlTransaction Vtransaction, int nextId, string branchId,
            string currentYear, string codePreFix, SysDBInfoVMTemp connVM = null)
        {
            var sqlText = "  ";
            sqlText += " update  CodeGenerations set LastId='" + nextId + "'";
            sqlText += " WHERE CurrentYear=@CurrentYear AND BranchId=@BranchId AND Prefix=@Prefix";


            var cmdExist = new SqlCommand(sqlText, VcurrConn);
            cmdExist.Transaction = Vtransaction;

            cmdExist.Parameters.AddWithValue("@BranchId", branchId);

            cmdExist.Parameters.AddWithValue("@CurrentYear", currentYear);
            cmdExist.Parameters.AddWithValue("@Prefix", codePreFix);
            cmdExist.ExecuteNonQuery();
        }

        public DataTable CheckInvoiceNoExist(List<string> ids, SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @"select  SalesInvoiceNo from SalesInvoiceHeaders where SalesInvoiceNo in (";

                var len = ids.Count;
                for (int i = 0; i < len; i++)
                {
                    sqlText += "@id" + i + ",";
                }

                sqlText = sqlText.TrimEnd(',');

                sqlText += ")";

                #endregion

                #region Sql Command

                var cmd = new SqlCommand(sqlText, currConn, transaction);


                for (int i = 0; i < len; i++)
                {
                    cmd.Parameters.AddWithValue("@id" + i, ids[i]);
                }

                var adapter = new SqlDataAdapter(cmd);

                var table = new DataTable();

                adapter.Fill(table);

                #endregion


                transaction.Commit();

                return table;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        public DataTable GetOldDbList(SysDBInfoVMTemp connVM = null)
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

            try
            {
                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnectionSys();
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Sql Text

                var sqlText = @" IF  EXISTS (SELECT * FROM sys.objects 
                 WHERE object_id = OBJECT_ID(N'OldDB'))
		begin
		Select DBName from OldDB
		end";

                #endregion

                #region Sql Command

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                var table = new DataTable();

                adapter.Fill(table);

                #endregion


                transaction.Commit();

                return table;
            }
            #region Catch and finally

            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
        }

        #endregion

        #region unused //// Commented before Jan-26-2020



        #endregion

        #region Demonstration

        public string[] SaveTempTest(DataTable table, SysDBInfoVMTemp connVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;
            try
            {
                connection = _dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                var columns = new List<string>();

                foreach (DataColumn column in table.Columns)
                {
                    columns.Add(column.ColumnName);
                }

                var sqlText = @"insert into SalesTempData (" + string.Join(",", columns) + ") values ";

                var len = table.Rows.Count;
                var colLen = columns.Count;

                for (int i = 0; i < len; i++)
                {
                    sqlText += "(";


                    for (int j = 0; j < colLen; j++)
                    {
                        sqlText += "@" + columns[j] + i;

                        if (j != (colLen - 1))
                        {
                            sqlText += ",";
                        }
                    }


                    sqlText += ")";

                    if (i != (len - 1))
                    {
                        sqlText += ",";
                    }
                }

                var cmd = new SqlCommand(sqlText, connection);
                cmd.Transaction = transaction;


                for (int i = 0; i < len; i++)
                {

                    for (int j = 0; j < colLen; j++)
                    {
                        cmd.Parameters.AddWithValue("@" + columns[j] + i, table.Rows[i][columns[j]]);
                    }

                }

                var rows = (int)cmd.ExecuteNonQuery();
                transaction.Commit();

                return rows == len ? new[] { "success" } : new[] { "fail" };

            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }

        public string[] ImportDataTest(DataTable dtSaleM, DataTable dtSaleD, DataTable dtSaleE, bool CommercialImporter = false, int branchId = 0, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[6];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            SaleMasterVM saleMaster = new SaleMasterVM();
            List<SaleDetailVm> saleDetails = new List<SaleDetailVm>();
            List<SaleExportVM> saleExport = new List<SaleExportVM>();
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string VinvoiceNo = "";
            string VitemCode = "";
            string commentsD = "NA";
            decimal VAT_Amount = 0;

            //string cTotalValue = "0";
            //string cWareHouseRent = "0";
            //string cWareHouseVAT = "0";
            //string cATVRate = "0";
            //string cATVablePrice = "0";
            //string cATVAmount = "0";
            //string cIsCommercialImporter = "0";

            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.StatisticsEnabled = true;
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Checking Das ta");
                }
                CommonImportDAL cImport = new CommonImportDAL();
                CommonDAL commonDal = new CommonDAL();
                ProductDAL productDal = new ProductDAL();

                #region RowCount
                int MRowCount = 0;
                int MRow = dtSaleM.Rows.Count;
                DataTable varDt = new DataTable();
                varDt.TableName = "NewSaleM";
                varDt = dtSaleM.Copy();
                var DatabaseName = commonDal.settings("DatabaseName", "DatabaseName");
                var saleExistContinue = commonDal.settings("Import", "SaleExistContinue");
                MRowCount = dtSaleM.Rows.Count;
                for (int i = 0; i < MRow; i++)
                {
                    string importID = varDt.Rows[i]["ID"].ToString().Trim();
                    var exist = cImport.CheckSaleImportIdExist(importID, currConn, transaction);
                    if (exist.ToLower() == "exist")
                    {

                        if (saleExistContinue.ToLower() == "n")
                        {
                            string msg = "Import Id " + importID.ToString() + "Already Exist in Database";
                            throw new ArgumentNullException(msg);
                        }
                        else
                        {
                            //DataRow[] drm = null;//a datarow array
                            //DataRow[] drd = null;//a datarow array
                            //DataRow[] dre = null;//a datarow array
                            //if (dtSaleM != null && dtSaleM.Rows.Count > 0)
                            //{
                            //    drm = dtSaleM.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in drm)
                            //    {
                            //        dtSaleM.Rows.Remove(row);
                            //    }
                            //}
                            //if (dtSaleD != null && dtSaleD.Rows.Count > 0)
                            //{
                            //    drd = dtSaleD.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in drd)
                            //    {
                            //        dtSaleD.Rows.Remove(row);
                            //    }
                            //}
                            //if (dtSaleE != null && dtSaleE.Rows.Count > 0)
                            //{
                            //    dre = dtSaleE.Select("ID =" + importID); //get the rows with matching condition in arrray
                            //    foreach (DataRow row in dre)
                            //    {
                            //        dtSaleE.Rows.Remove(row);
                            //    }
                            //}
                            //UpdateIsProcessed(1, importID);
                            retResults[0] = "success";
                            retResults[1] = "Import id already exists";
                            return retResults;

                            //loop throw the array and deete those rows from datatable

                            //loop throw the array and deete those rows from datatable
                        }

                    }
                    else
                    {
                        var tt = importID;
                    }

                }
                if (dtSaleM.Rows.Count <= 0)
                {
                    retResults[0] = "Information";
                    retResults[1] = "You do Not Have Data to Import";
                    throw new ArgumentNullException(retResults[1]);

                }
                //for (int i = 0; i < dtSaleM.Rows.Count; i++)
                //{
                //    string importID = dtSaleM.Rows[i]["ID"].ToString();
                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        MRowCount++;
                //    }

                //}

                #endregion RowCount

                #region ID in Master or Detail table

                //// in details check
                //for (int i = 0; i < MRowCount; i++)
                //{
                //    string importID = dtSaleM.Rows[i]["ID"].ToString();

                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        DataRow[] DetailRaws = dtSaleD.Select("ID='" + importID + "'");
                //        if (DetailRaws.Length == 0)
                //        {
                //            throw new ArgumentNullException("The ID " + importID + " is not available in detail table");
                //        }

                //    }

                //}
                //// in master check
                //for (int i = 0; i < dtSaleD.Rows.Count; i++)
                //{
                //    string importID = dtSaleD.Rows[i]["ID"].ToString();

                //    if (!string.IsNullOrEmpty(importID))
                //    {
                //        DataRow[] DetailRaws = dtSaleM.Select("ID='" + importID + "'");
                //        if (DetailRaws.Length == 0)
                //        {
                //            throw new ArgumentNullException("The ID " + importID + " is not available in master table");
                //        }

                //    }

                //}

                #endregion

                #region Double ID in Master

                //for (int i = 0; i < MRowCount; i++)
                //{
                //    string id = dtSaleM.Rows[i]["ID"].ToString();
                //    DataRow[] tt = dtSaleM.Select("ID='" + id + "'");
                //    if (tt.Length > 1)
                //    {
                //        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                //    }

                //}

                #endregion Double ID in Master


                #region Read from settings
                string vPriceDeclaration = commonDal.settings("Sale", "PriceDeclarationForImport");
                bool IsPriceDeclaration = Convert.ToBoolean(vPriceDeclaration == "Y" ? true : false);
                string vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                bool isNegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                string DefaultProductCategory1 = commonDal.settings("AutoSave", "DefaultProductCategory");
                string DefaultCustomerGroup = commonDal.settings("AutoSave", "DefaultCustomerGroup");
                string DefaultProductCategoryId = "";
                string DefaultCustomerGroupId = "";

                #endregion

                string conversionDate = DateTime.Now.ToString("yyyy-MM-dd");

                #region Find Master Column for Sanofi

                bool IsCompInvoiceNo = false;
                for (int i = 0; i < dtSaleM.Columns.Count; i++)
                {
                    if (dtSaleM.Columns[i].ColumnName.ToString() == "Comp_Invoice_No")
                    {
                        IsCompInvoiceNo = true;
                    }
                }
                ////Find details column for CP
                bool IsColWeight = false;
                for (int i = 0; i < dtSaleD.Columns.Count; i++)
                {
                    if (dtSaleD.Columns[i].ColumnName.ToString() == "Weight")
                    {
                        IsColWeight = true;
                    }
                }

                #endregion
                #region checking from database is exist the information(NULL Check)
                #region Master
                //string CurrencyId = string.Empty;
                //string USDCurrencyId = string.Empty;

                for (int i = 0; i < MRowCount; i++)
                {

                    //var ImportId = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    //CurrencyId = string.Empty;
                    //USDCurrencyId = string.Empty;
                    #region Master
                    #region FindCustomerId
                    //cImport.FindCustomerId(dtSaleM.Rows[i]["Customer_Name"].ToString().Trim(), dtSaleM.Rows[i]["Customer_Code"].ToString().Trim(), currConn, transaction);
                    #endregion FindCustomerId

                    #region FindCurrencyId
                    //CurrencyId = cImport.FindCurrencyId(dtSaleM.Rows[i]["Currency_Code"].ToString().Trim(), currConn, transaction);
                    //USDCurrencyId = cImport.FindCurrencyId("USD", currConn, transaction);
                    //cImport.FindCurrencyRateFromBDT(CurrencyId, currConn, transaction);
                    //cImport.FindCurrencyRateBDTtoUSD(USDCurrencyId,conversionDate, currConn, transaction);

                    #endregion FindCurrencyId

                    #region FindTenderId
                    //cImport.FindTenderId(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion FindTenderId

                    #region Checking Date is null or different formate
                    bool IsInvoiceDate;
                    IsInvoiceDate = cImport.CheckDate(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim());
                    if (IsInvoiceDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Invoice_Date_Time field.");
                    }
                    bool IsDeliveryDate;
                    IsDeliveryDate = cImport.CheckDate(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim());
                    if (IsDeliveryDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Delivery_Date_Time field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    bool isPrint;
                    post = cImport.CheckYN(dtSaleM.Rows[i]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    isPrint = cImport.CheckYN(dtSaleM.Rows[i]["Is_Print"].ToString().Trim());
                    if (isPrint != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Is_Print field.");
                    }
                    #endregion Checking Y/N value

                    #region Check previous invoice id
                    //string PreInvoiceId = string.Empty;
                    //string TenderId = string.Empty;

                    //PreInvoiceId = cImport.CheckPreInvoiceID(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    //TenderId = cImport.CheckTenderID(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice id

                    #region Check LC Number
                    if (dtSaleM.Rows[i]["Transection_Type"].ToString().Trim() == "Export")
                    {

                        string LCNumber = string.Empty;
                        DataRow[] ExportRaws = dtSaleE.Select("ID='" + dtSaleM.Rows[i]["ID"].ToString().Trim() + "'");
                        if (ExportRaws.Length > 0)
                        {
                            LCNumber = dtSaleM.Rows[i]["LC_Number"].ToString().Trim();
                            if (string.IsNullOrEmpty(LCNumber) || LCNumber == "0")
                            {
                                throw new ArgumentNullException("Please insert value in LC_Number field.");
                            }
                        }
                    }
                    #endregion  Check LC Number

                    #endregion Master

                }
                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = dtSaleD.Rows.Count;
                //for (int i = 0; i < dtSaleD.Rows.Count; i++)
                //{
                //    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["ID"].ToString()))
                //    {
                //        DRowCount++;
                //    }

                //}
                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    VitemCode = string.Empty;
                    string UOMn = string.Empty;
                    string UOMc = string.Empty;
                    bool IsQuantity, IsNbrPrice, IsTrading, IsSDRate, IsVatRate, IsDiscount, IsPromoQuantity;


                    #region FindItemId
                    if (string.IsNullOrEmpty(dtSaleD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    bool ItemAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct") == "Y" ? true : false);
                    // db call
                    VitemCode = dtSaleD.Rows[i]["ItemNo"].ToString().Trim();
                    //cImport.FindItemId(dtSaleD.Rows[i]["Item_Name"].ToString().Trim()
                    //         , dtSaleD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction,
                    //         ItemAutoSave, dtSaleD.Rows[i]["UOM"].ToString().Trim());

                    #endregion FindItemId
                    //db call
                    #region FindUOMn
                    UOMn = cImport.FindUOMn(VitemCode, currConn, transaction);
                    #endregion FindUOMn
                    #region FindUOMc
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        UOMc = cImport.FindUOMc(UOMn, dtSaleD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMc

                    #region FindLastNBRPrice

                    DataRow[] vmaster; //= new DataRow[];//

                    string nbrPrice = string.Empty;
                    var transactionDate = "";
                    vmaster = dtSaleM.Select("ID='" + dtSaleD.Rows[i]["ID"].ToString().Trim() + "'");
                    foreach (DataRow row in vmaster)
                    {
                        var tt = Convert.ToDateTime(row["Invoice_Date_Time"].ToString()).ToString("yyyy-MM-dd HH:mm:ss").Trim();
                        transactionDate = tt;
                    }
                    if (IsPriceDeclaration == true)
                    {
                        nbrPrice = cImport.FindLastNBRPrice(VitemCode, dtSaleD.Rows[i]["VAT_Name"].ToString().Trim(),
                            transactionDate, currConn, transaction);
                        if (Convert.ToDecimal(nbrPrice) == 0)
                        {

                            if (vmaster[0]["Transection_Type"].ToString() != "ExportService"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "Service"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                               )
                            {
                                throw new ArgumentNullException("Price declaration of item('" +
                                                dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "') not find in database");
                            }
                        }
                    }

                    #endregion FindLastNBRPrice
                    #region VATName
                    cImport.FindVatName(dtSaleD.Rows[i]["VAT_Name"].ToString().Trim());
                    #endregion VATName
                    #region Numeric value check

                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["Quantity"].ToString().Trim())
                        && !string.IsNullOrEmpty(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()))
                    {
                        if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) == 0
                            && Convert.ToDecimal(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim()) == 0)
                        {
                            throw new ArgumentNullException("Please insert quantity value in ID: " + dtSaleD.Rows[i]["ID"].ToString().Trim() + ", '" +
                                      dtSaleD.Rows[i]["Item_Name"].ToString().Trim() + "'('" + dtSaleD.Rows[i]["Item_Code"].ToString().Trim() + "').");
                        }
                    }


                    IsQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Quantity field.");
                    }
                    if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["Total_Price"].ToString().Trim());
                    }
                    else
                    {
                        IsNbrPrice = cImport.CheckNumericBool(dtSaleD.Rows[i]["NBR_Price"].ToString().Trim());
                    }
                    if (IsNbrPrice != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in NBR_Price field.");
                    }
                    if (!string.IsNullOrEmpty(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim()))
                    {
                        IsVatRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["VAT_Rate"].ToString().Trim());
                        if (IsVatRate != true)
                        {
                            throw new ArgumentNullException("Please insert decimal value in VAT_Rate field.");
                        }
                    }

                    IsSDRate = cImport.CheckNumericBool(dtSaleD.Rows[i]["SD_Rate"].ToString().Trim());
                    if (IsSDRate != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in SD_Rate field.");
                    }
                    IsTrading = cImport.CheckNumericBool(dtSaleD.Rows[i]["Trading_MarkUp"].ToString().Trim());
                    if (IsTrading != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Trading_MarkUp field.");
                    }
                    IsDiscount = cImport.CheckNumericBool(dtSaleD.Rows[i]["Discount_Amount"].ToString().Trim());
                    if (IsDiscount != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Discount_Amount field.");
                    }
                    IsPromoQuantity = cImport.CheckNumericBool(dtSaleD.Rows[i]["Promotional_Quantity"].ToString().Trim());
                    if (IsPromoQuantity != true)
                    {
                        throw new ArgumentNullException("Please insert decimal value in Promotional_Quantity field.");
                    }
                    #endregion Numeric value check

                    #region Checking Y/N value
                    bool NonStock;
                    NonStock = cImport.CheckYN(dtSaleD.Rows[i]["Non_Stock"].ToString().Trim());
                    if (NonStock != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Non_Stock field.");
                    }
                    #endregion Checking Y/N value


                    #region Check Stock

                    string quantityInHand = productDal.AvgPriceNew(VitemCode, transactionDate, currConn, transaction, false,true,true,true,null).Rows[0]["Quantity"].ToString();

                    string tenderStock = "0";//"0,0.0000");

                    decimal minValue = 0;
                    if (Convert.ToDecimal(quantityInHand) < Convert.ToDecimal(tenderStock))
                    {
                        minValue = Convert.ToDecimal(quantityInHand);
                    }
                    else
                    {
                        minValue = Convert.ToDecimal(tenderStock);

                    }
                    if (vmaster[0]["Transection_Type"].ToString() == "Tender" || vmaster[0]["Transection_Type"].ToString() == "TradingTender")
                    {
                        if (Convert.ToDecimal(quantityInHand) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(minValue))
                        {
                            throw new ArgumentNullException("Stock Not available for " + VitemCode);
                        }
                    }
                    else if (vmaster[0]["Transection_Type"].ToString() != "Credit"
                                && vmaster[0]["Transection_Type"].ToString() != "VAT11GaGa"
                                && vmaster[0]["Transection_Type"].ToString() != "ServiceNS"
                                && vmaster[0]["Transection_Type"].ToString() != "ExportServiceNS")
                    {
                        if (isNegStockAllow == false)
                        {
                            if (Convert.ToDecimal(dtSaleD.Rows[i]["Quantity"].ToString().Trim()) * Convert.ToDecimal(UOMc) > Convert.ToDecimal(quantityInHand))
                            {
                                throw new ArgumentNullException("Stock Not available for " + VitemCode);
                            }
                        }
                    }


                    #endregion Check Stock


                }


                #endregion Details
                #region Export
                if (dtSaleM.Rows.Count > 0 && (dtSaleM.Rows[0]["Transection_Type"].ToString() == "Export"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTrading"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportServiceNS"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportService"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportTradingTender"
                    || dtSaleM.Rows[0]["Transection_Type"].ToString() == "ExportPackage"))
                {

                    #region Row count for export details table
                    int ERowCount = 0;
                    for (int i = 0; i < dtSaleE.Rows.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(dtSaleE.Rows[i]["ID"].ToString()))
                        {
                            ERowCount++;
                        }

                    }
                    #endregion Row count for export details table

                    for (int e = 0; e < ERowCount; e++)
                    {
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Description"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Description field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["Quantity"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in Quantity field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["GrossWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in GrossWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NetWeight"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NetWeight field.");
                        }

                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberFrom"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberFrom field.");
                        }
                        if (string.IsNullOrEmpty(dtSaleE.Rows[e]["NumberTo"].ToString().Trim()))
                        {
                            throw new ArgumentNullException("Please insert value in NumberTo field.");
                        }

                    }
                }

                #endregion Export

                #endregion checking from database is exist the information(NULL Check)


                if (currConn.State == ConnectionState.Open)
                {
                    transaction.Commit();
                    currConn.Close();
                    currConn.Open();
                    transaction = currConn.BeginTransaction("Import Data");
                }


                for (int i = 0; i < MRowCount; i++)
                {
                    #region Process model
                    #region Master Sale

                    VinvoiceNo = dtSaleM.Rows[i]["ID"].ToString().Trim();
                    string customerName = dtSaleM.Rows[i]["Customer_Name"].ToString().Trim();
                    string customerCode = dtSaleM.Rows[i]["Customer_Code"].ToString().Trim();
                    // Remove DB Process
                    #region FindCustomerId
                    bool CustomerAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleCustomers") == "Y" ? true : false);
                    string customerId = dtSaleM.Rows[i]["CustomerID"].ToString().Trim();//cImport.FindCustomerId(customerName, customerCode, currConn, transaction, CustomerAutoSave);

                    #endregion FindCustomerId

                    #region FindBranchId
                    // Remove DB Process

                    string branchCode = dtSaleM.Rows[i]["Branch_Code"].ToString().Trim();
                    string BranchId = dtSaleM.Rows[i]["BranchId"].ToString().Trim(); //cImport.FindBranchId(branchCode, currConn, transaction);
                    if (!string.IsNullOrWhiteSpace(BranchId))
                    {
                        branchId = Convert.ToInt32(BranchId);
                    }

                    #endregion

                    string deliveryAddress = dtSaleM.Rows[i]["Delivery_Address"].ToString().Trim();
                    string vehicleNo = dtSaleM.Rows[i]["Vehicle_No"].ToString().Trim();
                    //var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    //var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("MM/dd/yyyy HH:mm:ss");
                    var invoiceDateTime = Convert.ToDateTime(dtSaleM.Rows[i]["Invoice_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd");
                    var deliveryDateTime =
                        Convert.ToDateTime(dtSaleM.Rows[i]["Delivery_Date_Time"].ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss");
                    #region CheckNull
                    string referenceNo = cImport.ChecKNullValue(dtSaleM.Rows[i]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtSaleM.Rows[i]["Comments"].ToString().Trim());
                    #endregion CheckNull
                    string saleType = dtSaleM.Rows[i]["Sale_Type"].ToString().Trim();
                    #region Check previous invoice no.
                    // Remove DB Process

                    string previousInvoiceNo = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Previous_Invoice_No"].ToString().Trim(), currConn, transaction);
                    #endregion Check previous invoice no.
                    string isPrint = dtSaleM.Rows[i]["Is_Print"].ToString().Trim();
                    #region Check Tender id
                    // Remove DB Process
                    string tenderId = cImport.CheckPrePurchaseNo(dtSaleM.Rows[i]["Tender_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Tender id
                    string post = dtSaleM.Rows[i]["Post"].ToString().Trim();

                    #region CheckNull
                    string lCNumber = cImport.ChecKNullValue(dtSaleM.Rows[i]["LC_Number"].ToString().Trim());
                    #endregion CheckNull
                    string currencyCode = dtSaleM.Rows[i]["Currency_Code"].ToString().Trim();
                    string createdBy = dtSaleM.Rows[i]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtSaleM.Rows[i]["LastModified_By"].ToString().Trim();
                    string transactionType = dtSaleM.Rows[i]["Transection_Type"].ToString().Trim();
                    string compInvoiceNo = null;
                    if (IsCompInvoiceNo == true)
                    {
                        compInvoiceNo = dtSaleM.Rows[i]["Comp_Invoice_No"].ToString().Trim();
                    }



                    #region Master

                    saleMaster = new SaleMasterVM();
                    saleMaster.CustomerID = customerId;
                    saleMaster.DeliveryAddress1 = deliveryAddress;
                    saleMaster.VehicleNo = vehicleNo;
                    saleMaster.InvoiceDateTime = Convert.ToDateTime(invoiceDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.SerialNo = referenceNo;
                    saleMaster.Comments = comments;
                    saleMaster.CreatedBy = createdBy;
                    saleMaster.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.LastModifiedBy = lastModifiedBy;
                    saleMaster.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    saleMaster.SaleType = saleType;
                    saleMaster.PreviousSalesInvoiceNo = previousInvoiceNo;
                    saleMaster.Trading = "N";
                    saleMaster.IsPrint = isPrint;
                    saleMaster.TenderId = tenderId;
                    saleMaster.TransactionType = transactionType;
                    saleMaster.DeliveryDate =
                       Convert.ToDateTime(deliveryDateTime).ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss");
                    saleMaster.Post = post; //Post


                    // Remove DB Process
                    var currencyid = dtSaleM.Rows[i]["CurrencyID"].ToString().Trim();//cImport.FindCurrencyId(currencyCode, currConn, transaction);
                    saleMaster.CurrencyID = currencyid; //Post
                    saleMaster.CurrencyRateFromBDT = Convert.ToDecimal(cImport.FindCurrencyRateFromBDT(currencyid, currConn, transaction));
                    // remove DB 
                    saleMaster.ReturnId = cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction);
                    // return ID is used for doller rate
                    saleMaster.LCNumber = lCNumber;
                    saleMaster.ImportIDExcel = VinvoiceNo;

                    saleMaster.TotalAmount = Convert.ToDecimal("0.00");
                    saleMaster.TotalVATAmount = Convert.ToDecimal("0.00");
                    saleMaster.CompInvoiceNo = compInvoiceNo;
                    saleMaster.BranchId = branchId;

                    #endregion Master


                    #endregion Master Sale

                    #region Match

                    DataRow[] DetailsRaws; //= new DataRow[];//
                    if (!string.IsNullOrEmpty(VinvoiceNo))
                    {
                        DetailsRaws = dtSaleD.Select("ID='" + VinvoiceNo + "'");
                    }
                    else
                    {
                        DetailsRaws = null;
                    }



                    #endregion Match

                    #region Details Sale

                    //int totalCounter = 1;
                    int lineCounter = 1;
                    decimal totalAmount = 0;
                    decimal totalVatAmount = 0;


                    saleDetails = new List<SaleDetailVm>();
                    #region Juwel 15/10/2015

                    DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name", "Non_Stock", "Type", "VAT_Name", "ItemNo");
                    //DataTable dtDistinctItem = DetailsRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code","Non_Stock", "Type", "VAT_Name");
                    DataTable dtSalesDetail = DetailsRaws.CopyToDataTable();

                    //DataTable dtDistinctItem = dtSaleD.Select("ID='" + VinvoiceNo + "'").CopyToDataTable();


                    string nBRPrice = "", uOM = "", uOMn = "", uOMc = "";

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        decimal cTotalValue = 0, cWareHouseRent = 0, cWareHouseVAT = 0, cATVRate = 0
                            , cATVablePrice = 0
                            , cATVAmount = 0
                            , Total_Price = 0;
                        decimal NBR_Price = 0, totalQuantity = 0, totalPrice = 0, vatRate = 0, sdRate = 0, tradingMarkup = 0, discountAmount = 0, promotionalQuantity = 0;
                        string cIsCommercialImporter = "N";


                        decimal LastNBRPrice = 0;
                        string saleDID = VinvoiceNo;//row["ID"].ToString().Trim();
                        VitemCode = item["Item_Code"].ToString().Trim();


                        // db call
                        //string itemName = item["Item_Name"].ToString().Trim();
                        bool ItemAutoSave = Convert.ToBoolean(commonDal.settingValue("AutoSave", "SaleProduct") == "Y" ? true : false);
                        // db call
                        string itemNo = item["ItemNo"].ToString().Trim();//cImport.FindItemId(item["Item_Name"].ToString().Trim(), VitemCode, currConn, transaction, ItemAutoSave);
                        string vATName = item["VAT_Name"].ToString().Trim();
                        string nonStock = item["Non_Stock"].ToString().Trim();
                        string type = item["Type"].ToString().Trim();
                        //string commentsD = item["CommentsD"].ToString().Trim();
                        string weight = "";

                        DataTable dtRepeatedItems = dtSalesDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            //cTotalValue = row["TotalValue"].ToString().Trim();
                            //cWareHouseRent = row["WareHouseRent"].ToString().Trim();
                            //cWareHouseVAT = row["WareHouseVAT"].ToString().Trim();
                            //cATVRate = row["ATVRate"].ToString().Trim();
                            //cATVablePrice = row["ATVablePrice"].ToString().Trim();
                            //cATVAmount = row["ATVAmount"].ToString().Trim();
                            //cIsCommercialImporter = row["IsCommercialImporter"].ToString().Trim();

                            Total_Price = Convert.ToDecimal(row["Total_Price"].ToString().Trim() == "" ? "0" : row["TotalValue"].ToString().Trim());
                            //VAT_Amount = Convert.ToDecimal(row["VAT_Amount"].ToString().Trim() == "" ? "0" : row["VAT_Amount"].ToString().Trim());
                            totalQuantity = totalQuantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                            commentsD = row["CommentsD"].ToString().Trim();
                            //totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim());/////Juwel
                            if (type.ToLower() != "non-vat system")
                            {
                                vatRate = vatRate + Convert.ToDecimal(row["VAT_Rate"].ToString().Trim());
                                sdRate = sdRate + Convert.ToDecimal(row["SD_Rate"].ToString().Trim());
                                tradingMarkup = tradingMarkup + Convert.ToDecimal(row["Trading_MarkUp"].ToString().Trim());
                            }
                            //else
                            //{
                            //    vatRate =vatRate+ 0;
                            //    sdRate =sdRate+ 0;
                            //    tradingMarkup = tradingMarkup + 0;
                            //}

                            discountAmount = discountAmount + Convert.ToDecimal(row["Discount_Amount"].ToString().Trim());
                            promotionalQuantity = promotionalQuantity + Convert.ToDecimal(row["Promotional_Quantity"].ToString().Trim());
                            if (IsColWeight == true)
                            {
                                weight = row["Weight"].ToString().Trim();
                            }

                            if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                            {
                                NBR_Price = NBR_Price + Convert.ToDecimal(row["NBR_Price"].ToString().Trim());
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                                totalPrice = totalPrice + Convert.ToDecimal(row["Total_Price"].ToString().Trim() == "" ? "0" : row["Total_Price"].ToString().Trim());

                                cTotalValue = cTotalValue + Convert.ToDecimal(row["TotalValue"].ToString().Trim() == "" ? "0" : row["TotalValue"].ToString().Trim());
                                cWareHouseRent = cWareHouseRent + Convert.ToDecimal(row["WareHouseRent"].ToString().Trim() == "" ? "0" : row["WareHouseRent"].ToString().Trim());
                                cWareHouseVAT = cWareHouseVAT + Convert.ToDecimal(row["WareHouseVAT"].ToString().Trim() == "" ? "0" : row["WareHouseVAT"].ToString().Trim());
                                cATVRate = Convert.ToDecimal(row["ATVRate"].ToString().Trim() == "" ? "0" : row["ATVRate"].ToString().Trim());
                                cATVablePrice = cATVablePrice + Convert.ToDecimal(row["ATVablePrice"].ToString().Trim() == "" ? "0" : row["ATVablePrice"].ToString().Trim());
                                cATVAmount = cATVAmount + Convert.ToDecimal(row["ATVAmount"].ToString().Trim() == "" ? "0" : row["ATVAmount"].ToString().Trim());
                                cIsCommercialImporter = row["IsCommercialImporter"].ToString().Trim() == "" ? "N" : row["IsCommercialImporter"].ToString().Trim();

                            }


                        }

                        #region For Sanofi
                        if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                        {
                            // string totalPrice = row["Total_Price"].ToString().Trim();
                            nBRPrice = (Convert.ToDecimal(totalPrice) / Convert.ToDecimal(totalQuantity)).ToString();
                            LastNBRPrice = Convert.ToDecimal(nBRPrice);


                            uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                            uOM = uOMn;
                            uOMc = "1";
                        }
                        else
                        {
                            nBRPrice = NBR_Price.ToString();

                            if (transactionType == "ExportService"
                                 && transactionType == "ExportServiceNS"
                               && transactionType == "Service"
                               && transactionType == "ServiceNS")
                            {
                                if (nBRPrice == "0")
                                {
                                    LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                }
                                else
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                            else
                            {
                                LastNBRPrice = cImport.FindLastNBRPriceFromBOM(itemNo, vATName,
                                                                            invoiceDateTime, currConn, transaction);
                                if (IsPriceDeclaration == false)
                                {
                                    LastNBRPrice = Convert.ToDecimal(nBRPrice);
                                }
                            }
                        }
                        #endregion

                        string vATRate = Convert.ToString(vatRate / dtRepeatedItems.Rows.Count);
                        if (string.IsNullOrEmpty(vATRate))
                        {
                            vATRate = "0";
                        }
                        string sDRate = Convert.ToString(sdRate / dtRepeatedItems.Rows.Count);
                        string tradingMarkUp = tradingMarkup.ToString();
                        if (type.ToLower() == "vat" || type.ToLower() == "vat system")
                        {
                            type = "VAT";

                        }
                        else
                        {
                            type = "Non VAT";
                            vATName = " ";
                        }

                        SaleDetailVm detail = new SaleDetailVm();
                        detail.InvoiceLineNo = lineCounter.ToString();
                        detail.ItemNo = itemNo;
                        decimal vQuantity = Convert.ToDecimal(Convert.ToDecimal(totalQuantity) + Convert.ToDecimal(promotionalQuantity));
                        detail.Quantity = vQuantity;
                        detail.PromotionalQuantity = Convert.ToDecimal(promotionalQuantity);
                        detail.VATRate = Convert.ToDecimal(vATRate);
                        detail.VATAmount = VAT_Amount == 0 ? Convert.ToDecimal(vQuantity) * Convert.ToDecimal(vATRate) : VAT_Amount;
                        detail.SD = Convert.ToDecimal(sDRate);
                        detail.CommentsD = commentsD;
                        detail.SaleTypeD = saleType;
                        detail.PreviousSalesInvoiceNoD = previousInvoiceNo;
                        detail.TradingD = "N";
                        detail.NonStockD = nonStock;
                        detail.Type = type;
                        detail.CConversionDate = invoiceDateTime;
                        detail.VatName = vATName;
                        detail.Weight = weight;
                        detail.TradingMarkUp = Convert.ToDecimal(tradingMarkUp);


                        if (CommercialImporter)
                        {

                            detail.TotalValue = Convert.ToDecimal(cTotalValue);
                            detail.WareHouseRent = Convert.ToDecimal(cWareHouseRent);
                            detail.WareHouseVAT = Convert.ToDecimal(cWareHouseVAT);
                            detail.ATVRate = Convert.ToDecimal(cATVRate);
                            detail.ATVablePrice = Convert.ToDecimal(cATVablePrice);
                            detail.ATVAmount = Convert.ToDecimal(cATVAmount);
                            detail.IsCommercialImporter = cIsCommercialImporter;


                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            LastNBRPrice = LastNBRPrice;// Total_Price / vQuantity;
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);

                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;

                            decimal subTotal = nbrPrice;
                            detail.NBRPrice = nbrPrice / vQuantity;
                            detail.SalesPrice = detail.NBRPrice;

                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(detail.NBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);


                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }
                        else
                        {
                            detail.TotalValue = Convert.ToDecimal(cTotalValue);
                            detail.WareHouseRent = Convert.ToDecimal(cWareHouseRent);
                            detail.WareHouseVAT = Convert.ToDecimal(cWareHouseVAT);
                            detail.ATVRate = Convert.ToDecimal(cATVRate);
                            detail.ATVablePrice = Convert.ToDecimal(cATVablePrice);
                            detail.ATVAmount = Convert.ToDecimal(cATVAmount);
                            detail.IsCommercialImporter = cIsCommercialImporter;

                            detail.DiscountAmount = Convert.ToDecimal(discountAmount);
                            decimal discountedNBRPrice = Convert.ToDecimal(Convert.ToDecimal(LastNBRPrice)
                                                                           * Convert.ToDecimal(uOMc))
                                                         - Convert.ToDecimal(discountAmount);
                            detail.DiscountedNBRPrice = Convert.ToDecimal(discountedNBRPrice);
                            decimal nbrPrice = Convert.ToDecimal(discountedNBRPrice) - Convert.ToDecimal(discountAmount);
                            detail.UOMn = uOMn;
                            detail.UOM = uOM;

                            detail.SalesPrice = nbrPrice;
                            detail.NBRPrice = nbrPrice;
                            decimal subTotal = vQuantity * nbrPrice;
                            detail.SubTotal = Convert.ToDecimal(subTotal);
                            decimal vATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.VATAmount = subTotal * Convert.ToDecimal(vATRate) / 100;
                            detail.SDAmount = subTotal * Convert.ToDecimal(sDRate) / 100;
                            detail.UOMQty = Convert.ToDecimal(vQuantity) * Convert.ToDecimal(uOMc);
                            detail.UOMc = Convert.ToDecimal(uOMc);
                            detail.UOMPrice = Convert.ToDecimal(LastNBRPrice);
                            detail.DollerValue = Convert.ToDecimal(subTotal) /
                                                 Convert.ToDecimal(
                                                     cImport.FindCurrencyRateBDTtoUSD(cImport.FindCurrencyId("USD", currConn, transaction), invoiceDateTime, currConn, transaction));
                            detail.CurrencyValue = Convert.ToDecimal(subTotal);
                            #region Total for Master
                            totalAmount = totalAmount + subTotal + vATAmount;
                            totalVatAmount = totalVatAmount + vATAmount;

                            saleMaster.TotalAmount = totalAmount;
                            saleMaster.TotalVATAmount = totalVatAmount;
                            #endregion Total for Master
                        }
                        detail.BranchId = branchId;

                        saleDetails.Add(detail);


                        lineCounter++;
                    }
                    #endregion

                    #endregion Details Sale

                    #region Details Export

                    int eCounter = 1;

                    if (transactionType == "Export"
                    || transactionType == "ExportTender"
                    || transactionType == "ExportTrading"
                    || transactionType == "ExportServiceNS"
                    || transactionType == "ExportService"
                    || transactionType == "ExportTradingTender"
                    || transactionType == "ExportPackage")
                    {
                        DataRow[] ExportRaws; //= new DataRow[];//
                        if (!string.IsNullOrEmpty(VinvoiceNo))
                        {
                            ExportRaws = dtSaleE.Select("ID='" + VinvoiceNo + "'");
                        }
                        else
                        {

                            ExportRaws = null;
                            if (ExportRaws == null)
                            {
                                throw new ArgumentNullException("For Export sale must filup the SaleE file");
                            }

                        }

                        saleExport = new List<SaleExportVM>();
                        foreach (DataRow row in ExportRaws)
                        {
                            string saleEID = row["ID"].ToString().Trim();
                            string description = row["Description"].ToString().Trim();
                            string quantityE = row["Quantity"].ToString().Trim();
                            string grossWeight = row["GrossWeight"].ToString().Trim();

                            string netWeight = row["NetWeight"].ToString().Trim();
                            string numberFrom = row["NumberFrom"].ToString().Trim();
                            string numberTo = row["NumberTo"].ToString().Trim();
                            //string portFrom = row["PortFrom"].ToString().Trim();

                            //string portTo = row["PortTo"].ToString().Trim();


                            SaleExportVM expDetail = new SaleExportVM();
                            expDetail.SaleLineNo = eCounter.ToString();
                            expDetail.Description = description.ToString();
                            expDetail.QuantityE = quantityE.ToString();
                            expDetail.GrossWeight = grossWeight.ToString();
                            expDetail.NetWeight = netWeight.ToString();
                            expDetail.NumberFrom = numberFrom.ToString();
                            expDetail.NumberTo = numberTo.ToString();
                            expDetail.CommentsE = "NA";
                            expDetail.RefNo = "NA";
                            expDetail.BranchId = branchId;
                            saleExport.Add(expDetail);

                            eCounter++;

                        } // details


                    }
                    #endregion Details Export

                    #endregion Process model


                    if (CommercialImporter)
                    {
                        #region CommercialImporter

                        bool NeedSave = false;
                        bool TrackingWithSale = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSale") == "Y" ? true : false);
                        bool TrackingWithSaleFIFO = Convert.ToBoolean(commonDal.settingValue("Purchase", "TrackingWithSaleFIFO") == "Y" ? true : false);
                        int NumberOfItems = Convert.ToInt32(commonDal.settingValue("Sale", "NumberOfItems"));

                        string sqlText = "";
                        string dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                        string user = createdBy + dtime;
                        IsertPurchaseSaleTrackingTemps(user, currConn, transaction);
                        decimal qty = 0;
                        decimal Saleqty = 0;

                        foreach (var Item in saleDetails.ToList())
                        {


                            qty = Item.Quantity;
                            for (int t = 0; t < qty; t++)
                            {

                                DataTable tracDt = new DataTable();
                                sqlText = "";
                                sqlText += @" select top 1 * from PurchaseSaleTrackingTemps";
                                sqlText += @" where IsSold=0";
                                sqlText += @" and ItemNo=@ItemItemNo";
                                sqlText += @" and LoginUser='" + user + "'";
                                if (TrackingWithSaleFIFO)
                                {
                                    sqlText += @" order by id asc ";
                                }
                                else
                                {
                                    sqlText += @" order by id desc ";
                                }
                                SqlCommand cmdRIFB1 = new SqlCommand(sqlText, currConn);
                                cmdRIFB1.Transaction = transaction;
                                cmdRIFB1.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                                SqlDataAdapter reportDataAdapt1 = new SqlDataAdapter(cmdRIFB1);
                                reportDataAdapt1.Fill(tracDt);

                                foreach (DataRow itemTrac in tracDt.Rows)
                                {
                                    sqlText = "";
                                    sqlText += " update PurchaseSaleTrackingTemps set  ";
                                    sqlText += " IsSold= '1' ";
                                    sqlText += " ,SalesInvoiceNo= '" + dtime + "' ";
                                    //sqlText += " ,SaleInvoiceDateTime= '" + Master.InvoiceDateTime + "' ";
                                    sqlText += " where  Id= '" + itemTrac["Id"] + "' ";

                                    SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                                    cmdUpdate.Transaction = transaction;
                                    transResult = (int)cmdUpdate.ExecuteNonQuery();
                                    if (transResult <= 0)
                                    {
                                        throw new ArgumentNullException(MessageVM.saleMsgMethodNameUpdate, MessageVM.saleMsgUpdateNotSuccessfully);
                                    }
                                }
                                ReportDSDAL rds = new ReportDSDAL();
                                DataSet ds = new DataSet();
                                ds = rds.VAT11ReportCommercialImporterNewTemp(dtime, "N", "N", currConn, transaction);
                                int rowcnt = ds.Tables[0].Rows.Count;
                                if (rowcnt >= NumberOfItems)
                                {
                                    Saleqty = Saleqty + 1;
                                    var obj = saleDetails.FirstOrDefault(x => x.ItemNo == Item.ItemNo);
                                    if (obj != null) obj.Quantity = Saleqty;

                                    var sqlResults = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn,branchId,null);
                                    Thread.Sleep(1000);
                                    retResults[0] = sqlResults[0];
                                    Saleqty = 0;
                                    dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                                    NeedSave = false;

                                }
                                else
                                {

                                    Saleqty = Saleqty + 1;
                                    var obj = saleDetails.FirstOrDefault(x => x.ItemNo == Item.ItemNo);
                                    if (obj != null) obj.Quantity = Saleqty;
                                    NeedSave = true;


                                    //saleDetails
                                }

                            }




                        }
                        DeletePurchaseSaleTrackingTemps(user, currConn, transaction);

                        if (NeedSave)
                        {
                            var sqlResults1 = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn);
                            Thread.Sleep(1000);
                            retResults[0] = sqlResults1[0];
                            Saleqty = 0;
                            dtime = DateTime.Now.ToString("yyyyMMMddHHmmss");
                        }

                        #endregion CommercialImporter

                    }
                    else
                    {
                        var sqlResults = SalesInsert(saleMaster, saleDetails, saleExport, null, transaction, currConn);
                        retResults[0] = sqlResults[0];
                    }


                }// master

                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    retResults[4] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();
                    #endregion SuccessResult
                }
                //SAVE_DOWORK_SUCCESS = true;
            }
            #endregion try
            #region catch & final
            //catch (SqlException sqlex)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            //catch (ArgumentNullException aeg)
            //{
            //    if (transaction != null)
            //    {
            //        transaction.Rollback();
            //    }
            //    throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + aeg.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            //    //throw ex;
            //}
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString() + FieldDelimeter + "Invoice:" + VinvoiceNo + FieldDelimeter + "Item:" + VitemCode);//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //throw ex;
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



        public string[] SaveMaster(List<SaleMasterVM> masters, SysDBInfoVMTemp connVM = null)
        {
            SqlTransaction transaction = null;
            SqlConnection connection = null;

            try
            {

                connection = _dbsqlConnection.GetConnection();
                connection.Open();
                transaction = connection.BeginTransaction();


                var sqlText = "";

                sqlText = "";
                sqlText += " insert into SalesInvoiceHeaders";
                sqlText += " (";
                sqlText += " SalesInvoiceNo,";
                sqlText += " CustomerID,";
                sqlText += " DeliveryAddress1,";
                sqlText += " DeliveryAddress2,";
                sqlText += " DeliveryAddress3,";
                sqlText += " VehicleID,";
                sqlText += " InvoiceDateTime,";
                sqlText += " DeliveryDate,";
                sqlText += " TotalAmount,";
                sqlText += " TotalVATAmount,";
                sqlText += " VDSAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " SaleType,";
                sqlText += " PreviousSalesInvoiceNo,";
                sqlText += " Trading,";
                sqlText += " IsPrint,";
                sqlText += " TenderId,";
                sqlText += " TransactionType,";
                sqlText += " Post,";
                sqlText += " LCNumber,";
                sqlText += " CurrencyID,";
                sqlText += " CurrencyRateFromBDT,";
                sqlText += " SaleReturnId,";
                sqlText += " IsVDS,";
                sqlText += " GetVDSCertificate,";
                sqlText += " VDSCertificateDate,";
                sqlText += " ImportIDExcel,";
                sqlText += " AlReadyPrint,";
                sqlText += " LCBank,";
                sqlText += " LCDate,";
                sqlText += " DeliveryChallanNo,";
                sqlText += " IsGatePass,";
                sqlText += " CompInvoiceNo,";
                sqlText += " ShiftId,";
                sqlText += " ValueOnly,";
                sqlText += " PINo,";
                sqlText += " PIDate,";
                sqlText += " EXPFormNo,";
                sqlText += " EXPFormDate,";
                sqlText += " BranchId,";
                sqlText += " DeductionAmount,";
                sqlText += " IsDeemedExport";

                sqlText += " )";


                sqlText += " values";
                var lenMasters = masters.Count;

                for (int i = 0; i < lenMasters; i++)
                {
                    sqlText += " (";
                    sqlText += "@SalesInvoiceNo" + i + ",";
                    sqlText += "@CustomerID" + i + ",";
                    sqlText += "@DeliveryAddress1" + i + ",";
                    sqlText += "@DeliveryAddress2" + i + ",";
                    sqlText += "@DeliveryAddress3" + i + ",";
                    sqlText += "@VehicleID" + i + ",";
                    sqlText += "@InvoiceDateTime" + i + ",";
                    sqlText += "@DeliveryDate" + i + ",";
                    sqlText += "@TotalAmount" + i + ",";
                    sqlText += "@TotalVATAmount" + i + ",";
                    sqlText += "@VDSAmount" + i + ",";

                    sqlText += "@SerialNo" + i + ",";
                    sqlText += "@Comments" + i + ",";
                    sqlText += "@CreatedBy" + i + ",";
                    sqlText += "@CreatedOn" + i + ",";
                    sqlText += "@LastModifiedBy" + i + ",";
                    sqlText += "@LastModifiedOn" + i + ",";
                    sqlText += "@SaleType" + i + ",";
                    sqlText += "@PreviousSalesInvoiceNo" + i + ",";
                    sqlText += "@Trading" + i + ",";
                    sqlText += "@IsPrint" + i + ",";
                    sqlText += "@TenderId" + i + ",";
                    sqlText += "@TransactionType" + i + ",";
                    sqlText += "@Post" + i + ",";
                    sqlText += "@LCNumber" + i + ",";
                    sqlText += "@CurrencyID" + i + ",";
                    sqlText += "@CurrencyRateFromBDT" + i + ",";
                    sqlText += "@SaleReturnId" + i + ",";
                    sqlText += "@IsVDS" + i + ",";
                    sqlText += "@GetVDSCertificate,";
                    sqlText += "@VDSCertificateDate" + i + ",";
                    sqlText += "@ImportIDExcel" + i + ",";
                    sqlText += "@AlReadyPrint" + i + ",";
                    sqlText += "@LCBank" + i + ",";
                    sqlText += "@LCDate" + i + ",";
                    sqlText += "@DeliveryChallanNo" + i + ",";
                    sqlText += "@IsGatePass" + i + ",";
                    sqlText += "@CompInvoiceNo" + i + ",";
                    sqlText += "@ShiftId" + i + ",";
                    sqlText += "@ValueOnly" + i + ",";
                    sqlText += "@PINo" + i + ",";
                    sqlText += "@PIDate" + i + ",";
                    sqlText += "@EXPFormNo" + i + ",";
                    sqlText += "@EXPFormDate" + i + ",";
                    sqlText += "@BranchId" + i + ",";
                    sqlText += "@DeductionAmount" + i + ",";

                    sqlText += "@IsDeemedExport" + i + "";

                    sqlText += ")";


                    if (i != (lenMasters - 1))
                    {
                        sqlText += ", ";
                    }
                }

                var cmdInsert = new SqlCommand(sqlText, connection);
                cmdInsert.Transaction = transaction;



                for (int i = 0; i < lenMasters; i++)
                {
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@SalesInvoiceNo" + i + "", masters[i].SalesInvoiceNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CustomerID" + i + "", masters[i].CustomerID);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress1" + i + "", masters[i].DeliveryAddress1);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress2" + i + "", masters[i].DeliveryAddress2);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryAddress3" + i + "", masters[i].DeliveryAddress3);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@VehicleID" + i + "", masters[i].VehicleID);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@InvoiceDateTime" + i + "", masters[i].InvoiceDateTime);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryDate" + i + "", masters[i].DeliveryDate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalAmount" + i + "", masters[i].TotalAmount);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalVATAmount" + i + "", masters[i].TotalVATAmount);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@VDSAmount" + i + "", masters[i].VDSAmount);

                    cmdInsert.Parameters.AddWithValueAndNullHandle("@SerialNo" + i + "", masters[i].SerialNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@Comments" + i + "", masters[i].Comments);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedBy" + i + "", masters[i].CreatedBy);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CreatedOn" + i + "", OrdinaryVATDesktop.DateToDate(masters[i].CreatedOn));
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedBy" + i + "", masters[i].LastModifiedBy);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", OrdinaryVATDesktop.DateToDate(masters[i].LastModifiedOn));
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleType" + i + "", masters[i].SaleType);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@PreviousSalesInvoiceNo" + i + "", masters[i].PreviousSalesInvoiceNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@Trading" + i + "", masters[i].Trading);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@IsPrint" + i + "", masters[i].IsPrint);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@TenderId" + i + "", masters[i].TenderId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType" + i + "", masters[i].TransactionType);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@Post" + i + "", masters[i].Post);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LCNumber" + i + "", masters[i].LCNumber);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyID" + i + "", masters[i].CurrencyID);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CurrencyRateFromBDT" + i + "", masters[i].CurrencyRateFromBDT);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@SaleReturnId" + i + "", masters[i].ReturnId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@IsVDS" + i + "", masters[i].IsVDS == null ? "N" : masters[i].IsVDS);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@GetVDSCertificate" + i + "", masters[i].GetVDSCertificate == null ? "N" : masters[i].GetVDSCertificate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@VDSCertificateDate" + i + "", masters[i].VDSCertificateDate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ImportIDExcel" + i + "", masters[i].ImportIDExcel);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@AlReadyPrint" + i + "", masters[i].AlReadyPrint == null ? "0" : masters[i].AlReadyPrint);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LCBank" + i + "", masters[i].LCBank);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@LCDate" + i + "", masters[i].LCDate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeliveryChallanNo" + i + "", masters[i].DeliveryChallanNo == null ? "-" : masters[i].DeliveryChallanNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@IsGatePass" + i + "", masters[i].IsGatePass == null ? "N" : masters[i].IsGatePass);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@CompInvoiceNo" + i + "", masters[i].CompInvoiceNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ShiftId" + i + "", masters[i].ShiftId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@ValueOnly" + i + "", masters[i].ValueOnly);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@PINo" + i + "", masters[i].PINo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@PIDate" + i + "", masters[i].PIDate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@EXPFormNo" + i + "", masters[i].EXPFormNo);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@EXPFormDate" + i + "", masters[i].EXPFormDate);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@BranchId" + i + "", masters[i].BranchId);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@DeductionAmount" + i + "", masters[i].DeductionAmount);
                    cmdInsert.Parameters.AddWithValueAndNullHandle("@IsDeemedExport" + i + "", masters[i].IsDeemedExport);
                }



                var rows = (int)cmdInsert.ExecuteNonQuery();
                transaction.Commit();

                return rows == lenMasters ? new[] { "success" } : new[] { "fail" };

            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }
        }




        #endregion

        #region Method 05

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

                transaction = currConn.BeginTransaction(MessageVM.saleMsgMethodNameUpdate);
                CommonDAL commonDal = new CommonDAL();
                bool TollReceiveWithIssue = Convert.ToBoolean(commonDal.settings("TollReceive", "WithIssue") == "Y" ? true : false);

                #endregion open connection and transaction
                for (int i = 0; i < Ids.Length - 1; i++)
                {
                    var salesInvoiceId = Convert.ToInt32(Ids[i]);

                    SaleMasterVM master = SelectAllList(salesInvoiceId, null, null, currConn, transaction, null, null).FirstOrDefault();

                    //List<SaleDetailVM> Details = SelectSaleDetail(master.SalesInvoiceNo, null, null, currConn, transaction);

                    UserInformationVM userinformation = new UserInformationDAL().SelectAll(0, new[] { "ui.UserID" },
                            new[] { UserId }).FirstOrDefault();
                    List<SaleDetailVm> Details = new List<SaleDetailVm>();
                    List<TrackingVM> Trakings = new List<TrackingVM>();
                    master.Post = "Y";
                    master.SignatoryName = userinformation.FullName;
                    master.SignatoryDesig = userinformation.Designation;
                    retResults = SalesPost(master, Details, Trakings, transaction, currConn);
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



        public void UpdateSalesTempData(SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            SqlConnection currConn = null;

            int transResult = 0;
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

                #region Update ItemNo
                sqlText = "  ";
                sqlText = @" Declare @hasData int;
Declare @itemNo varchar(20);
Declare @productName varchar(100);
Declare @productCode varchar(20);
set @hasData = (select count(ItemNo) from SalesTempData where ItemNo='0');

WHILE @hasData > 0
BEGIN
	set @productCode = (select top 1 Item_Code from SalesTempData where ItemNo='0');
	set @itemNo = (select ItemNo from Products where ProductCode= @productCode);
	if(@itemNo is not null)
	begin
		select @itemNo;
	end

	else
	begin
		set @productName = (select top 1 Item_Name from SalesTempData where ItemNo='0');
		set @itemNo = (select ItemNo from Products where ProductName= @productName);
		if(@itemNo is not null)
		begin
			select @itemNo;
		end
		else
		begin
			set @itemNo='-1';
		end
	end
	UPDATE TOP (1) SalesTempData set ItemNo=@itemNo where ItemNo='0';

	set @hasData = (select count(ItemNo) from SalesTempData where ItemNo='0');
END";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                transResult = (int)cmd.ExecuteNonQuery();
                #endregion

                #region Update ItemNo
                sqlText = "  ";
                sqlText = @" Declare @hasData int;
Declare @customerId varchar(20);
Declare @customerCode varchar(20);
Declare @customerName varchar(100);
set @hasData = (select count(CustomerID) from SalesTempData where CustomerID='0');

WHILE @hasData > 0
BEGIN
	set @customerCode = (select top 1 Customer_Code from SalesTempData where CustomerID='0');
	set @customerId = (select CustomerID from Customers where CustomerCode= @customerCode);
	if(@customerId is not null)
	begin
		select @customerId;
	end

	else
	begin
		set @customerName = (select top 1 Customer_Name from SalesTempData where CustomerID='0');
		set @customerId = (select CustomerID from Customers where CustomerName= @customerName);
		if(@customerId is not null)
		begin
			select @customerId;
		end
		else
		begin
			set @customerId='-1';
		end
	end
	UPDATE TOP (1) SalesTempData set CustomerID=@customerId where CustomerID='0';

	set @hasData = (select count(CustomerID) from SalesTempData where CustomerID='0');
END";

                cmd = new SqlCommand(sqlText, currConn);
                transResult = (int)cmd.ExecuteNonQuery();
                #endregion


            }

            #endregion try

            #region Catch and Finall
            catch (Exception ex)
            {
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

            #endregion
        }

        public string[] GetFisrtInvoiceId(SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string[] sqlResult = new string[5];
            sqlResult[0] = "Fail";

            try
            {
                #region open connection and transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction

                var sqlText = @"select top 1 Invoice_Date_Time from SalesTempData order by Invoice_Date_Time";

                var cmd = new SqlCommand(sqlText, currConn, transaction);
                var date = cmd.ExecuteScalar();


                var commonDal = new CommonDAL();

                var newIDCreate = commonDal.TransactionCode("Sale", "Other", "SalesInvoiceHeaders", "SalesInvoiceNo",
                    "InvoiceDateTime", date.ToString(), "1", currConn, transaction);

                if (vTransaction == null)
                {
                    transaction.Commit();

                }


                sqlResult[0] = "success";
                sqlResult[1] = "success";
                sqlResult[2] = newIDCreate;

                return sqlResult;
            }
            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open && vConnection == null)
                {
                    currConn.Close();
                }
            }

        }

        public string[] SaveInvoiceIdSaleTemp(int firstId, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string[] sqlResult = new string[5];
            sqlResult[0] = "Fail";

            try
            {
                #region open connection and transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }

                #endregion open connection and transaction


                string vatCalculation = @" 
update SalesTempData set   SubTotal= NBR_Price*Quantity
 where SubTotal<=0 or SubTotal is null

update SalesTempData set  SDAmount=(SubTotal*SD_Rate)/100
 where SDAmount<=0 or SDAmount is null

  update SalesTempData set  VAT_Amount=(SubTotal+SDAmount)*VAT_Rate/100
 where VAT_Amount<=0 or VAT_Amount is null";

                string ExcludingVATCal = @"

update SalesTempData 
set DiscountedNBRPrice = ((SubTotal+Discount_Amount)/Quantity) 

--update SalesTempData set SourcePaidVATAmount = (SubTotal+Discount_Amount)/Quantity*SourcePaidQuantity*VAT_Rate/(100+VAT_Rate)

update SalesTempData 
set SourcePaidVATAmount = (DiscountedNBRPrice*SourcePaidQuantity*VAT_Rate)/(100+VAT_Rate)

update SalesTempData set VAT_Amount = SourcePaidVATAmount+(((Quantity-SourcePaidQuantity)*NBR_Price*VAT_Rate)/(100+VAT_Rate))


--- ((((SubTotal+Discount_Amount)/Quantity)*VAT_Rate)/(100+VAT_Rate))

update SalesTempData set Discount_Amount = DiscountedNBRPrice-UOMPrice

update SalesTempData set NBRPriceInclusiveVAT = NBR_Price

update SalesTempData set NBR_Price = ((((NBR_Price*100)/(100+VAT_Rate))*100)/(100+SD_Rate))
update SalesTempData set SubTotal = NBR_Price*Quantity

--update SalesTempData set SubTotal = (SubTotal-((SubTotal*VAT_Rate)/(100+VAT_Rate)))

";


                #region Total Amount Cal

                string sqlText = @"declare @invoiceNo as varchar(10)
declare @count as int
DECLARE @intFlag INT
declare @Pre as varchar(10)
declare @Length as int
--declare @Id int

--set @Id = '+'

select @Pre=Prefix,@Length=Lenth from codes where CodeGroup='Sale' and CodeName='Other'

 create table #temp(sl int identity(1,1), Id varchar(100),SalesInvoiceNo varchar(100),InvoiceDate datetime, TotalAmount decimal(25,7),TotalVATAmount decimal(25,7) )
 
update SalesTempData set CDNVATAmount = VAT_Amount, CDNSDAmount = SDAmount, CDNSubtotal = SubTotal

update SalesTempData set  TotalValue=SubTotal+VAT_Amount+SDAmount
where TotalValue<=0 or TotalValue is null


--update SalesTempData set  CurrencyValue= SubTotal * CurrencyRateFromBDT
--update SalesTempData set DollerValue = (case when SalesTempData.CurrencyRateFromBDT > 1 then (SubTotal/returnId) else 0 end)

 insert into #temp(Id,InvoiceDate,TotalAmount,TotalVATAmount) 
 select  distinct Id,Invoice_Date_Time, sum(TotalValue),sum(VAT_Amount) from SalesTempData group by Id,Invoice_Date_Time
   
--SET @intFlag = 1

--   select @count=  count(sl) from #temp; 

 
--WHILE (@intFlag <= @count)
--BEGIN


--     update #temp set SalesInvoiceNo= @Pre+'-'+ REPLICATE('0',case when @Length-len(@Id)>0 then  @Length-len(@Id) else 0 end ) +convert(varchar(10),@Id)+'/'+ FORMAT(InvoiceDate, 'MMyy')   where sl =@intFlag

--	SET @Id=@Id+1

--    SET @intFlag = @intFlag + 1
--END


  --select * from #temp 
  update SalesTempData set SalesTempData.TotalAmount=#temp.TotalAmount
  ,SalesTempData.TotalVATAmount=#temp.TotalVATAmount
  from #temp where SalesTempData.Id=#temp.Id


  drop table #temp";




                #endregion

                SqlCommand cmd = new SqlCommand(vatCalculation, currConn, transaction);

                CommonDAL commonDal = new CommonDAL();

                #region VAT Calculation Switch

                string code = commonDal.settingValue("CompanyCode", "Code", null, currConn, transaction);
                string ExcludingVAT = commonDal.settingValue("Sale", "ExcludingVAT", null, currConn, transaction);
                string autoCurrencyConv = commonDal.settingValue("Sale", "AutoCurrencyConv", null, currConn, transaction);

                if (code == "BATA" && ExcludingVAT.ToLower() == "n")
                {
                    cmd.CommandText = ExcludingVATCal;
                }

                #endregion


                cmd.ExecuteNonQuery();

                string updateIsComplete =
                    @"update SalesTempData set IsCurrencyConvCompleted = '" + autoCurrencyConv + "'";


                if (autoCurrencyConv.ToLower() == "y")
                {
                    string currencyConv = @"     update SalesTempData set  CurrencyValue= SubTotal * CurrencyRateFromBDT
update SalesTempData set DollerValue = (case when SalesTempData.CurrencyRateFromBDT > 1 then (SubTotal/returnId) else 0 end)
";
                    sqlText += currencyConv;
                }



                cmd.CommandText = sqlText + "   " + updateIsComplete;

                cmd.Parameters.AddWithValue("@Id", firstId);

                int rows = cmd.ExecuteNonQuery();




                if (vTransaction == null && transaction != null)
                {
                    transaction.Commit();

                }

                sqlResult[0] = rows > 0 ? "success" : "fail";

                return sqlResult;
            }
            catch (Exception e)
            {
                if (vTransaction == null && transaction != null)
                {
                    transaction.Rollback();

                }

                return sqlResult;
            }
            finally
            {

                if (currConn.State == ConnectionState.Open && vConnection == null)
                {
                    currConn.Close();

                }

            }
        }

        public string[] ImportSalesBigData(DataTable salesData, bool deleteFlag = true, SqlConnection vConnection = null, SqlTransaction vTransaction = null, SqlRowsCopiedEventHandler rowsCopiedCallBack = null, SysDBInfoVMTemp connVM = null)
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

            try
            {


                #region Connection and Transaction

                if (vConnection != null)
                {
                    currConn = vConnection;
                }

                if (vTransaction != null)
                {
                    transaction = vTransaction;
                }

                if (vConnection == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();

                }

                if (vTransaction == null)
                {
                    transaction = currConn.BeginTransaction();
                }

                #endregion

                #region Comments Oct-04-2020

                //                var defaultValueSql = @"update SalesTempData set ItemNo = 0, CustomerID = 0;";

                //                var updateItemCustomerId = @"
                //update SalesTempData set ItemNo=Products.ItemNo from Products where Products.ProductCode=SalesTempData.Item_Code;
                //update SalesTempData set ItemNo=Products.ItemNo from Products where Products.productName =SalesTempData.Item_Name;
                //
                //update SalesTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerCode =SalesTempData.Customer_Code;
                //update SalesTempData set CustomerID=Customers.CustomerID from Customers where Customers.CustomerName =SalesTempData.Customer_Name;";

                //                var getdefaultData = @"select * from SalesTempData where ItemNo = 0 or CustomerID = 0;";
                //                var branchAndCurrency = @"
                //update SalesTempData set BranchId=BranchProfiles.BranchID from BranchProfiles where BranchProfiles.BranchCode=SalesTempData.Branch_Code;
                //update SalesTempData set CurrencyId=Currencies.CurrencyId from Currencies where Currencies.CurrencyCode=SalesTempData.Currency_Code;
                //";
                //                var customerGroup = @"
                //update SalesTempData set GroupId=CustomerGroups.CustomerGroupID from CustomerGroups where CustomerGroups.CustomerGroupName=SalesTempData.[Group];
                //";
                //                var vehicleNo = @"
                //update SalesTempData set VehicleID=Vehicles.VehicleID from Vehicles where Vehicles.VehicleNo=SalesTempData.Vehicle_No; 
                //";
                // var setIsProcessed = @"update SalesTempData set IsProcessed = 1 where ItemNo != 0 and CustomerID != 0";

                //var deleteTemp = @"delete from SalesTempData; ";

                //deleteTemp += " DBCC CHECKIDENT ('SalesTempData', RESEED, 0);";

                //#endregion
                //var cmd = new SqlCommand(deleteTemp, currConn, transaction);

                //if (deleteFlag)
                //{
                //    cmd.ExecuteNonQuery();
                //}

                #endregion


                var commonDal = new CommonDAL();

                retResults = commonDal.BulkInsert("SalesInvoiceHeaders", salesData, currConn, transaction, 0,
                    rowsCopiedCallBack);

                #region Comments Oct-04-2020


                //cmd.CommandText = branchAndCurrency + " " + updateItemCustomerId + " " + customerGroup + " " + vehicleNo;
                //var updateItemResult = cmd.ExecuteNonQuery();

                //cmd.CommandText = getdefaultData;
                //var defaultData = new DataTable();
                //var dataAdapter = new SqlDataAdapter(cmd);
                //dataAdapter.Fill(defaultData);



                //#endregion

                //#region Insert Customer and Products

                //var commonImportDal = new CommonImportDAL();

                //foreach (DataRow row in defaultData.Rows)
                //{
                //    if (row["CustomerID"].ToString().Trim() == "0")
                //    {
                //        var customerName = row["Customer_Name"].ToString().Trim();
                //        var customerCode = row["Customer_Code"].ToString().Trim();
                //        var group = row["Group"].ToString().Trim();

                //        commonImportDal.FindCustomerId(customerName, customerCode, currConn, transaction, true, group);
                //    }

                //    if (row["ItemNo"].ToString().Trim() == "0")
                //    {
                //        var itemCode = row["Item_Code"].ToString().Trim();
                //        var itemName = row["Item_Name"].ToString().Trim();
                //        var uom = row["UOM"].ToString().Trim();

                //        commonImportDal.FindItemId(itemName, itemCode, currConn, transaction, true, uom);
                //    }
                //}

                //#endregion

                //#region Reupdate

                //cmd.CommandText = updateItemCustomerId + " " + customerGroup;
                //var reUpdate = cmd.ExecuteNonQuery();

                #endregion


                if (vTransaction == null && transaction != null)
                {
                    transaction.Commit();
                }


                //if (retResults[0].ToLower() == "success" && updateItemResult > 0)
                //{
                //}

                //retResults[0] = retResults[0].ToLower() == "success" && (updateItemResult > 0 || reUpdate > 0) ? "success" : "fail";

                retResults[2] = currConn.RetrieveStatistics()["ExecutionTime"].ToString();
            }



            catch (Exception ex)
            {
                if (transaction != null && vTransaction == null)
                {
                    transaction.Rollback();
                }
                throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());
            }
            finally
            {
                if (currConn.State == ConnectionState.Open && vConnection == null)
                {
                    currConn.Close();
                }
            }



            return retResults;
        }

        public DataTable SelectAllTop1(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null, string[] IdsForIn = null)
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
SELECT TOP 1
 sih.Id
,sih.ValueOnly
,isnull(sih.ShiftId,1) ShiftId
,sih.SalesInvoiceNo 
,sih.CustomerID
,sih.DeliveryAddress1
,sih.DeliveryAddress2
,sih.DeliveryAddress3
,sih.VehicleID
,sih.InvoiceDateTime
,sih.DeliveryDate


,isnull(sih.TotalAmount,0) TotalAmount 
,isnull(sih.TotalVATAmount,0) TotalVATAmount 
,isnull(sih.VDSAmount,0) VDSAmount 

,isnull(sih.HPSTotalAmount,0) HPSTotalAmount 

,isnull(sih.TotalAmount,0)-isnull(sih.TotalVATAmount,0) TotalSubtotal 
,sih.SerialNo
,sih.Comments
,sih.CreatedBy
,sih.CreatedOn
,sih.LastModifiedBy
,sih.LastModifiedOn
,sih.SaleType
,sih.PreviousSalesInvoiceNo
,isnull(sih.Trading,0) Trading 
,sih.IsPrint
,sih.TenderId
,sih.TransactionType
,sih.Post
,sih.LCNumber
,sih.CurrencyID
,isnull(sih.CurrencyRateFromBDT,0) CurrencyRateFromBDT 
,sih.SaleReturnId
,sih.IsVDS
,sih.GetVDSCertificate
,sih.VDSCertificateDate
,sih.ImportIDExcel
,isnull(sih.AlReadyPrint,0) AlReadyPrint
,sih.DeliveryChallanNo
,sih.IsGatePass
,sih.CompInvoiceNo
,sih.LCBank
,sih.Is6_3TollCompleted
,isnull(NULLIF(sih.LCDate,''),'1900/01/01')LCDate
,sih.InvoiceDateTime InvoiceDate
,c.CustomerName
,v.VehicleType
,v.VehicleNo
,cr.CurrencyCode
,cg.CustomerGroupName
,SaleType,isnull(sih.PreviousSalesInvoiceNo,SalesInvoiceNo) PID
 ,isnull(sih.DeliveryChallanNo,'N')DeliveryChallan
,isnull(sih.ImportIDExcel,'')ImportID
,isnull(sih.PINo,'N/A')PINo
,isnull(sih.PIDate,'1900/01/01')PIDate
,isnull(sih.EXPFormNo ,'N/A')EXPFormNo
,sih.EXPFormDate
,isnull(sih.BranchId,0) BranchId
,isnull(sih.IsDeemedExport ,'N')IsDeemedExport
,isnull(sih.SignatoryName ,'N/A')SignatoryName
,isnull(sih.SignatoryDesig ,'N/A')SignatoryDesig
,isnull(sih.DeductionAmount,0) DeductionAmount

FROM SalesInvoiceHeaders sih 

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleID = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and sih.Id=@Id";
                }


                if (IdsForIn != null)
                {
                    sqlText += " and sih.ImportIDExcel in (";

                    foreach (string id in IdsForIn)
                    {
                        sqlText += " '" + id + "', ";
                    }

                    sqlText = sqlText.TrimEnd(',') + " )";
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                    {
                        sqlText += " AND sih.SalesInvoiceNo like @SalesInvoiceNo";
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        sqlText += " AND sih.SerialNo like @SerialNo";
                    }
                }
                #endregion SqlText
                sqlText += " order by sih.SalesInvoiceNo desc";

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
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SalesInvoiceNo", "%" + likeVM.SalesInvoiceNo + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
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

        public DataTable SelectAllHeaderTemp(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SaleMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
SELECT distinct
 sih.Id
,sih.ValueOnly
,isnull(sih.ShiftId,1) ShiftId
,sih.SalesInvoiceNo 
,sih.CustomerID
,sih.Delivery_Address DeliveryAddress1
,'' DeliveryAddress2
,'' DeliveryAddress3
,sih.VehicleID
,sih.Invoice_Date_Time  InvoiceDateTime
,sih.Delivery_Date_Time DeliveryDate


,isnull(sih.TotalAmount,0) TotalAmount 
,isnull(sih.TotalVATAmount,0) TotalVATAmount 
,isnull(sih.VDSAmount,0) VDSAmount 

,0 HPSTotalAmount 

,isnull(sih.TotalAmount,0)-isnull(sih.TotalVATAmount,0) TotalSubtotal 
,sih.Reference_No SerialNo
,sih.Comments
,sih.CreatedBy
,sih.CreatedOn
,sih.CreatedBy LastModifiedBy
,sih.CreatedOn LastModifiedOn
,sih.Sale_Type SaleType
,sih.Previous_Invoice_No PreviousSalesInvoiceNo
,isnull(sih.Trading,0) Trading 
,sih.Is_Print IsPrint
,sih.Tender_Id TenderId
,sih.TransactionType
,sih.Post
,sih.LC_Number LCNumber
,sih.CurrencyID
,isnull(sih.CurrencyRateFromBDT,0) CurrencyRateFromBDT 
,sih.ReturnId SaleReturnId
,sih.IsVDS
,sih.GetVDSCertificate
,sih.VDSCertificateDate
,sih.ID ImportIDExcel
,isnull(sih.AlReadyPrint,0) AlReadyPrint
,sih.DeliveryChallanNo
,sih.IsGatePass
,sih.CompInvoiceNo
,sih.LCBank
,sih.Is6_3TollCompleted
,isnull(NULLIF(sih.LCDate,''),'1900/01/01')LCDate
,sih.Invoice_Date_Time InvoiceDate
,c.CustomerName
,v.VehicleType
,v.VehicleNo
,cr.CurrencyCode
,cg.CustomerGroupName
,isnull(sih.Previous_Invoice_No,SalesInvoiceNo) PID
 ,isnull(sih.DeliveryChallanNo,'N')DeliveryChallan
,isnull(sih.ID,'')ImportID
,isnull(sih.PINo,'N/A')PINo
,isnull(sih.PIDate,'1900/01/01')PIDate
,isnull(sih.EXPFormNo ,'N/A')EXPFormNo
,sih.EXPFormDate
,isnull(sih.BranchId,0) BranchId
,isnull(sih.IsDeemedExport ,'N')IsDeemedExport

,isnull(sih.DeductionAmount,0) DeductionAmount

FROM SalesTempData sih 

left outer join Customers c on sih.CustomerID=c.CustomerID 
left outer join Vehicles v on sih.VehicleID = v.VehicleID
left outer join Currencies cr on sih.CurrencyID=cr.CurrencyId
left outer join CustomerGroups cg on c.CustomerGroupID=cg.CustomerGroupID
WHERE  1=1 
";


                if (Id > 0)
                {
                    sqlText += @" and sih.Id=@Id";
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
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                    {
                        sqlText += " AND sih.SalesInvoiceNo like @SalesInvoiceNo";
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        sqlText += " AND sih.SerialNo like @SerialNo";
                    }
                }
                #endregion SqlText
                sqlText += " order by sih.SalesInvoiceNo desc";

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
                if (likeVM != null)
                {
                    if (!string.IsNullOrEmpty(likeVM.SalesInvoiceNo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SalesInvoiceNo", "%" + likeVM.SalesInvoiceNo + "%");
                    }
                    if (!string.IsNullOrEmpty(likeVM.SerialNo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@SerialNo", "%" + likeVM.SerialNo + "%");
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



        #region Split Methods


        public string[] SaveSaleITS_Split(IntegrationParam param, string UserId = "", SysDBInfoVMTemp connVM = null)
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

                string deleteSource = @"delete from SaleTempCSV_Source";
                SqlCommand cmd = new SqlCommand(deleteSource, currConn, transaction);
                cmd.ExecuteNonQuery();

                string[] result = commonDal.BulkInsert("SaleTempCSV_Source", param.Data, currConn, transaction);

                #endregion


                string deleteDuplicate = @"
delete from SaleTempCSV_Source where ID in (
                select vr.ID from SaleTempCSV_Source vr inner join SalesInvoiceHeaders rh
                on vr.[Transaction Reference] = rh.ImportIDExcel)";

                cmd.CommandText = deleteDuplicate;
                cmd.ExecuteNonQuery();


                #region Loop counter

                string getLoopCount = @"select Ceiling(count(distinct [DEBTORS/ CREDITORS  Analysis Code])/10.00) from SaleTempCSV_Source";

                cmd.CommandText = getLoopCount;
                int counter = Convert.ToInt32(cmd.ExecuteScalar());

                #endregion

                param.SetSteps(counter);

                transaction.Commit();
                currConn.Close();

                DataTable sourceData = new DataTable();


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
select  distinct top 10 [DEBTORS/ CREDITORS  Analysis Code] 
from SaleTempCSV_Source
where isnull(IsProcessed,'N') = 'N'";

                    cmd.CommandText = insertIds;
                    cmd.ExecuteNonQuery();

                    string getData = @"SELECT [Id]
      ,[Journal No.]
      ,[Journal Line]
      ,[Journal Type]
      ,[Transaction Date]
      ,[Accounting Period]
      ,[Allocation Marker]
      ,[Transaction Reference]
      ,[Account Code]
      ,[Description]
      ,[Debit/Credit marker]
      ,[Base Amount]
      ,[Currency Code]
      ,[Transaction Amount]
      ,[Description1]
      ,[Journal Source]
      ,[BUSINESS UNIT Analysis Code]
      ,[LOCATION  Analysis Code]
      ,[SEGMENT Analysis Code]
      ,[ACTIVITY  Analysis Code]
      ,[TDS/ EMP/ BANK CHARGES  Analysis Code]
      ,[VALUE ADDED TAX  Analysis Code]
      ,[DEBTORS/ CREDITORS  Analysis Code]
      ,[COST CENTRE   Analysis Code]
      ,[BUSINESS SPECIFIC  Analysis Code]
      ,[Originator Operator Code]
      ,[Originated Date]
      ,[Modified By (After Posting)]
      ,[Modified Date (After Posting)]
      ,[Allocated By]
      ,[Allocation Date]
      ,[Analysis Code]
      ,[Analysis Code1]

  FROM SaleTempCSV_Source where [DEBTORS/ CREDITORS  Analysis Code] in (Select ID from #tempIds)";

                    cmd.CommandText = getData;
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(sourceData);

                    //sourceData.Columns.Remove("IsProcessed");

                    #endregion

                    retResults = SaveAndProcessITS(sourceData, () => { }, param.DefaultBranchId, "",
                        param.TransactionType, param.CurrentUser,null,UserId);


                    #region updateSourceTable

                    string updateSourceAndClearTemp = @"update SaleTempCSV_Source set IsProcessed = 'Y' where [DEBTORS/ CREDITORS  Analysis Code]  in (select ID from #tempIds);
                                            delete from #tempIds;
drop table #tempIds";

                    cmd.CommandText = updateSourceAndClearTemp;
                    cmd.ExecuteNonQuery();
                    param.callBack();
                    #endregion


                    transaction.Commit();
                    currConn.Close();
                    transaction.Dispose();
                    currConn.Dispose();

                    sourceData.Clear();
                }


                #region Drop Temp table

                //currConn = _dbsqlConnection.GetConnection();
                //currConn.Open();
                //transaction = currConn.BeginTransaction();
                //cmd.Connection = currConn;
                //cmd.Transaction = transaction;

                //string dropTemp = @"drop table #tempIds";
                //cmd.CommandText = dropTemp;
                //cmd.ExecuteNonQuery();


                //transaction.Commit();
                //currConn.Close();
                //transaction.Dispose();
                //currConn.Dispose();

                #endregion

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





        #endregion



    }
}






