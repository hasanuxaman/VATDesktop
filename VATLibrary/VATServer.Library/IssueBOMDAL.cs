using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;

using System.IO;
using Excel;
using VATServer.Ordinary;
using System.Reflection;
using VATServer.Interface;


namespace VATServer.Library
{
    public class IssueBOMDAL : IIssueBOM
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        CommonDAL _cDal = new CommonDAL();

        #endregion

        public DataTable SearchIssueDetailDTNew(string IssueNo, string databaseName, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("IssueSearchDetail");

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
IssueDetailBOMs.IssueNo, 
IssueDetailBOMs.IssueLineNo,
IssueDetailBOMs.ItemNo, 
isnull(IssueDetailBOMs.Quantity,0)Quantity ,
isnull(IssueDetailBOMs.CostPrice,0)CostPrice,
isnull(IssueDetailBOMs.NBRPrice,0)NBRPrice,
isnull(IssueDetailBOMs.UOM,'N/A')UOM ,
isnull(IssueDetailBOMs.VATRate,0)VATRate,
isnull(IssueDetailBOMs.VATAmount,0)VATAmount,
isnull(IssueDetailBOMs.SubTotal,0)SubTotal,
isnull(IssueDetailBOMs.Comments,'N/A')Comments,
isnull(Products.ProductName,'N/A')ProductName,
isnull(isnull(Products.OpeningBalance,0)+
isnull(Products.QuantityInHand,0),0) as Stock,
isnull(IssueDetailBOMs.SD,0)SD,
isnull(IssueDetailBOMs.SDAmount,0)SDAmount,
isnull(Products.ProductCode,'N/A')ProductCode,
isnull(IssueDetailBOMs.UOMQty,isnull(IssueDetailBOMs.Quantity,0))UOMQty,
isnull(IssueDetailBOMs.UOMn,IssueDetailBOMs.UOM)UOMn,
isnull(IssueDetailBOMs.UOMc,1)UOMc,
isnull(IssueDetailBOMs.UOMPrice,isnull(IssueDetailBOMs.CostPrice,0))UOMPrice,
isnull(IssueDetailBOMs.UOMWastage,isnull(IssueDetailBOMs.Wastage,0))UOMWastage,
isnull(IssueDetailBOMs.BOMId,0)	BOMId,
isnull(IssueDetailBOMs.FinishItemNo,'0')FinishItemNo,
isnull(fp.ProductCode,'N/A')FinishProductCode,
isnull(fp.ProductName,'N/A')FinishProductName

                            FROM         dbo.IssueDetailBOMs  left outer join
                            Products on IssueDetailBOMs.ItemNo=Products.ItemNo LEFT OUTER JOIN
                            Products fp on IssueDetailBOMs.FinishItemNo=fp.ItemNo 
                            
                               
                            WHERE 
                            (IssueNo = @IssueNo ) 
                            ";

                #endregion

                #region SQL Command

                SqlCommand objCommIssueDetail = new SqlCommand();
                objCommIssueDetail.Connection = currConn;

                objCommIssueDetail.CommandText = sqlText;
                objCommIssueDetail.CommandType = CommandType.Text;

                #endregion

                #region Parameter

                if (!objCommIssueDetail.Parameters.Contains("@IssueNo"))
                { objCommIssueDetail.Parameters.AddWithValue("@IssueNo", IssueNo); }
                else { objCommIssueDetail.Parameters["@IssueNo"].Value = IssueNo; }

                #endregion Parameter

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "SearchIssueDetailDTNew", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "SearchIssueDetailDTNew", ex.ToString() + "\n" + sqlText);
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

            return dataTable;
        }

        public string[] ImportData(DataTable dtIssueM, DataTable dtIssueD, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";

            IssueMasterVM IssueMasterVM;
            List<IssueDetailVM> IssueDetailVMs = new List<IssueDetailVM>();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            #region try
            try
            {
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    currConn.Open();
                }


                #region RowCount
                int MRowCount = 0;
                int MRow = dtIssueM.Rows.Count;
                for (int i = 0; i < dtIssueM.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueM.Rows[i]["ID"].ToString()))
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
                    string importID = dtIssueM.Rows[i]["ID"].ToString();
                    if (!string.IsNullOrEmpty(importID))
                    {
                        DataRow[] DetailRaws = dtIssueD.Select("ID='" + importID + "'");
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
                    string id = dtIssueM.Rows[i]["ID"].ToString();
                    DataRow[] tt = dtIssueM.Select("ID='" + id + "'");
                    if (tt.Length > 1)
                    {
                        throw new ArgumentNullException("you have duplicate master id " + id + " in import file.");
                    }

                }

                #endregion Double ID in Master


                CommonImportDAL cImport = new CommonImportDAL();

                #region checking from database is exist the information(NULL Check)

                #region Master

                for (int j = 0; j < MRowCount; j++)
                {
                    #region Checking Date is null or different formate

                    bool IsIssueDate;
                    IsIssueDate = cImport.CheckDate(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    if (IsIssueDate != true)
                    {
                        throw new ArgumentNullException("Please insert correct date format 'DD/MMM/YY' such as 31/Jan/13 in Issue_Date field.");
                    }
                    #endregion Checking Date is null or different formate

                    #region Checking Y/N value
                    bool post;
                    post = cImport.CheckYN(dtIssueM.Rows[j]["Post"].ToString().Trim());
                    if (post != true)
                    {
                        throw new ArgumentNullException("Please insert Y/N in Post field.");
                    }
                    #endregion Checking Y/N value

                    #region Check Return issue id
                    string ReturnId = string.Empty;
                    ReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Return issue id
                }

                #endregion Master

                #region Details

                #region Row count for details table
                int DRowCount = 0;
                for (int i = 0; i < dtIssueD.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dtIssueD.Rows[i]["ID"].ToString()))
                    {
                        DRowCount++;
                    }

                }

                #endregion Row count for details table

                for (int i = 0; i < DRowCount; i++)
                {
                    string ItemNo = string.Empty;
                    string UOMn = string.Empty;

                    #region FindItemId
                    if (string.IsNullOrEmpty(dtIssueD.Rows[i]["Item_Code"].ToString().Trim()))
                    {
                        throw new ArgumentNullException("Please insert value in 'Item_Code' field.");
                    }
                    ItemNo = cImport.FindItemId(dtIssueD.Rows[i]["Item_Name"].ToString().Trim()
                                                , dtIssueD.Rows[i]["Item_Code"].ToString().Trim(), currConn, transaction);

                    #endregion FindItemId

                    #region FindUOMn

                    UOMn = cImport.FindUOMn(ItemNo, currConn, transaction);

                    #endregion FindUOMn

                    #region FindUOMn
                    if (DatabaseInfoVM.DatabaseName.ToString() != "Sanofi_DB")
                    {
                        cImport.FindUOMc(UOMn, dtIssueD.Rows[i]["UOM"].ToString().Trim(), currConn, transaction);
                    }
                    #endregion FindUOMn

                    #region Numeric value check
                    bool IsQuantity = cImport.CheckNumericBool(dtIssueD.Rows[i]["Quantity"].ToString().Trim());
                    if (IsQuantity != true)
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
                decimal TotalAmount;
                for (int j = 0; j < MRowCount; j++)
                {
                    TotalAmount = 0;

                    #region Master Issue

                    string importID = dtIssueM.Rows[j]["ID"].ToString().Trim();
                    DateTime issueDateTime = Convert.ToDateTime(dtIssueM.Rows[j]["Issue_DateTime"].ToString().Trim());
                    #region CheckNull
                    string serialNo = cImport.ChecKNullValue(dtIssueM.Rows[j]["Reference_No"].ToString().Trim());
                    string comments = cImport.ChecKNullValue(dtIssueM.Rows[j]["Comments"].ToString().Trim());
                    #endregion CheckNull

                    #region Check Return issue id
                    string issueReturnId = cImport.CheckIssueReturnID(dtIssueM.Rows[j]["Return_Id"].ToString().Trim(), currConn, transaction);
                    #endregion Check Return receive id
                    string post = dtIssueM.Rows[j]["Post"].ToString().Trim();
                    string createdBy = dtIssueM.Rows[j]["Created_By"].ToString().Trim();
                    string lastModifiedBy = dtIssueM.Rows[j]["LastModified_By"].ToString().Trim();
                    string transactionType = dtIssueM.Rows[j]["Transection_Type"].ToString().Trim();

                    IssueMasterVM = new IssueMasterVM();

                    IssueMasterVM.IssueDateTime =
                        issueDateTime.ToString("yyyy-MM-dd") +
                                           DateTime.Now.ToString(" HH:mm:ss");
                    IssueMasterVM.TotalVATAmount = 0;
                    IssueMasterVM.TotalAmount = Convert.ToDecimal(0);
                    IssueMasterVM.SerialNo = serialNo.Replace(" ", "");
                    IssueMasterVM.Comments = comments;
                    IssueMasterVM.CreatedBy = createdBy;
                    IssueMasterVM.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    IssueMasterVM.LastModifiedBy = lastModifiedBy;
                    IssueMasterVM.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    IssueMasterVM.ReturnId = issueReturnId;
                    IssueMasterVM.transactionType = transactionType;
                    IssueMasterVM.Post = post;
                    IssueMasterVM.ImportId = importID;
                    DataRow[] DetailRaws; //= new DataRow[];//

                    #region MAtch

                    if (!string.IsNullOrEmpty(importID))
                    {
                        DetailRaws = dtIssueD.Select("ID='" + importID + "'");
                    }
                    else
                    {
                        DetailRaws = null;
                    }

                    #endregion MAtch

                    #endregion Master Issue

                    #region Details Issue

                    int counter = 1;
                    IssueDetailVMs = new List<IssueDetailVM>();
                    // Juwel 13/10/2015
                    DataTable dtDistinctItem = DetailRaws.CopyToDataTable().DefaultView.ToTable(true, "Item_Code", "Item_Name");

                    DataTable dtIssueDetail = DetailRaws.CopyToDataTable();

                    foreach (DataRow item in dtDistinctItem.Rows)
                    {
                        DataTable dtRepeatedItems = dtIssueDetail.Select("[Item_Code] ='" + item["Item_Code"].ToString() + "'").CopyToDataTable();

                        string itemCode = item["Item_Code"].ToString().Trim();
                        string itemName = item["Item_Name"].ToString().Trim();
                        string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);
                        decimal quantity = 0;
                        decimal avgPrice;
                        CommonDAL cmnDal = new CommonDAL();
                        DataTable priceData = cImport.FindAvgPriceImport(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction);
                        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());
                        if (quan > 0)
                        {
                            avgPrice = cmnDal.FormatingDecimal((amount / quan).ToString());
                        }
                        else
                        {
                            avgPrice = 0;
                        }

                        string uOM = "";
                        string uOMn = "";
                        string uOMc = "";


                        IssueDetailVM detail = new IssueDetailVM();

                        foreach (DataRow row in dtRepeatedItems.Rows)
                        {
                            if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                            {
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOM = uOMn;
                                uOMc = "1";
                            }
                            else
                            {
                                uOM = row["UOM"].ToString().Trim();
                                uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                                uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                            }
                            quantity = quantity + Convert.ToDecimal(row["Quantity"].ToString().Trim());
                        }

                        detail.ItemNo = itemNo;
                        detail.IssueLineNo = counter.ToString();
                        detail.Quantity = Convert.ToDecimal(quantity);
                        detail.NBRPrice = 0;
                        detail.VATRate = 0;
                        detail.VATAmount = 0;
                        detail.UOM = uOM;
                        detail.SD = 0;
                        detail.SDAmount = 0;
                        detail.CommentsD = "NA";
                        detail.IssueDateTimeD =
                            issueDateTime.ToString("yyyy-MM-dd") +
                                               DateTime.Now.ToString(" HH:mm:ss");
                        detail.BOMDate = "1900-01-01";
                        detail.FinishItemNo = "0";
                        detail.UOMn = uOMn;
                        detail.UOMc = Convert.ToDecimal(uOMc);
                        detail.Wastage = 0;

                        detail.CostPrice = Convert.ToDecimal(avgPrice);

                        detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        TotalAmount = TotalAmount + Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                        detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) / Convert.ToDecimal(uOMc));
                        detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) / Convert.ToDecimal(uOMc));

                        IssueDetailVMs.Add(detail);
                        counter++;
                    }
                    #region Previous code by ruba apu 13/10/2015
                    //foreach (DataRow row in DetailRaws)
                    //{
                    //    //string itemCode = row["Item_Code"].ToString().Trim();
                    //    //string itemName = row["Item_Name"].ToString().Trim();

                    //    //string itemNo = cImport.FindItemId(itemName, itemCode, currConn, transaction);

                    //    //string quantity = row["Quantity"].ToString().Trim();
                    //    //string uOM ="";
                    //    //string uOMn="";
                    //    //string uOMc = "";
                    //    //if (DatabaseInfoVM.DatabaseName.ToString() == "Sanofi_DB")
                    //    //{
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOM = uOMn;
                    //    //    uOMc = "1";
                    //    //}
                    //    //else
                    //    //{
                    //    //    uOM = row["UOM"].ToString().Trim();
                    //    //    uOMn = cImport.FindUOMn(itemNo, currConn, transaction);
                    //    //    uOMc = cImport.FindUOMc(uOMn, uOM, currConn, transaction);
                    //    //}

                    //    //IssueDetailVM detail = new IssueDetailVM();
                    //    detail.ItemNo = itemNo;
                    //    detail.Quantity = Convert.ToDecimal(quantity);
                    //    detail.NBRPrice = 0;
                    //    detail.VATRate = 0;
                    //    detail.VATAmount = 0;
                    //    detail.UOM = uOM;
                    //    detail.SD = 0;
                    //    detail.SDAmount = 0;
                    //    detail.CommentsD = "NA";
                    //    detail.IssueDateTimeD =
                    //        issueDateTime.ToString("yyyy-MM-dd") +
                    //                           DateTime.Now.ToString(" HH:mm:ss");
                    //    detail.BOMDate = "1900-01-01";
                    //    detail.FinishItemNo = "0";
                    //    detail.UOMn = uOMn;
                    //    detail.UOMc = Convert.ToDecimal(uOMc);
                    //    detail.Wastage = 0;

                    //    CommonDAL cmnDal = new CommonDAL();
                    //    decimal avgPrice;

                    //        DataTable priceData = cImport.FindAvgPriceImport(itemNo, issueDateTime.ToString("yyyy-MM-dd") + DateTime.Now.ToString(" HH:mm:ss"), currConn, transaction);
                    //        decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
                    //        decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

                    //        if (quan > 0)
                    //        {
                    //            avgPrice = cmnDal.FormatingDecimal((amount/quan).ToString());
                    //        }
                    //        else
                    //        {
                    //            avgPrice = 0;
                    //        }

                    //    //detail.CostPrice = cmnDal.FormatingDecimal(avgPrice);
                    //    detail.CostPrice = Convert.ToDecimal(avgPrice);

                    //    detail.SubTotal = Convert.ToDecimal(Convert.ToDecimal(avgPrice) * Convert.ToDecimal(quantity));
                    //    detail.UOMQty = Convert.ToDecimal(Convert.ToDecimal(quantity) / Convert.ToDecimal(uOMc));
                    //    detail.UOMPrice = Convert.ToDecimal(Convert.ToDecimal(avgPrice) / Convert.ToDecimal(uOMc));

                    //    IssueDetailVMs.Add(detail);
                    //    counter++;
                    //} // detail
                    #endregion previous code

                    #endregion Details Issue
                    IssueMasterVM.TotalAmount = Convert.ToDecimal(TotalAmount);

                    string[] sqlResults = IssueInsert(IssueMasterVM, IssueDetailVMs, transaction, currConn);
                    retResults[0] = sqlResults[0];
                }
                if (retResults[0] == "Success")
                {
                    transaction.Commit();
                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                    retResults[2] = "" + "1";
                    retResults[3] = "" + "N";
                    #endregion SuccessResult
                }
            }
            #endregion try
            #region catch & final
            catch (SqlException sqlex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "ImportData", sqlex.ToString());
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + "" + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "ImportData", ex.ToString());
                throw new ArgumentNullException("", ex.Message.ToString());

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
        public List<IssueMasterVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            //SqlConnection currConn = null;
            //SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueMasterVM> VMs = new List<IssueMasterVM>();
            IssueMasterVM vm;
            #endregion
            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, null, false);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new IssueMasterVM();
                    vm.Id = dr["Id"].ToString();
                    vm.IssueNo = dr["IssueNo"].ToString();
                    vm.IssueDateTime = OrdinaryVATDesktop.DateTimeToDate(dr["IssueDateTime"].ToString());
                    vm.TotalVATAmount = Convert.ToDecimal(dr["TotalVATAmount"]);
                    vm.TotalAmount = Convert.ToDecimal(dr["TotalAmount"]);
                    vm.SerialNo = dr["SerialNo"].ToString();
                    vm.Comments = dr["Comments"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.transactionType = dr["TransactionType"].ToString();
                    vm.ShiftId = dr["ShiftId"].ToString();
                    vm.Post = dr["Post"].ToString();
                    ////reading newly added fields
                    vm.ReturnId = dr["IssueReturnId"].ToString();
                    vm.ImportId = dr["ImportIDExcel"].ToString();

                    VMs.Add(vm);
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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);
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

        public DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
                #region SqlText

                sqlText = @"
SELECT
 Id
,IssueNo
,IssueDateTime
,ISNULL(TotalVATAmount,0) TotalVATAmount 
,ISNULL(TotalAmount,0) TotalAmount 
,SerialNo
,Comments
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
,ReceiveNo
,TransactionType
,IssueReturnId
,Post
,ShiftId
,ImportIDExcel

FROM IssueHeaderBOMs  
WHERE  1=1
";


                if (Id > 0)
                {
                    sqlText += @" and Id=@Id";
                }

                string cField = "";
                if (conditionFields != null && conditionValues != null && conditionFields.Length == conditionValues.Length)
                {
                    for (int i = 0; i < conditionFields.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(conditionFields[i]) || string.IsNullOrWhiteSpace(conditionValues[i]) || conditionValues[i] == "0")
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
                    if (!string.IsNullOrWhiteSpace(likeVM.IssueNo))
                    {
                        sqlText += " AND IssueNo like @IssueNo";
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
                        if (string.IsNullOrWhiteSpace(conditionFields[j]) || string.IsNullOrWhiteSpace(conditionValues[j]) || conditionValues[j] == "0")
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
                    if (!string.IsNullOrWhiteSpace(likeVM.IssueNo))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@IssueNo", "%" + likeVM.IssueNo + "%");
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
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectAll", ex.ToString() + "\n" + sqlText);
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

        public List<IssueDetailVM> SelectIssueDetail(string issueNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<IssueDetailVM> VMs = new List<IssueDetailVM>();
            IssueDetailVM vm;
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
 iss.Id
,iss.IssueNo
,iss.IssueLineNo
,iss.ItemNo
,ISNULL(iss.Quantity,0) Quantity  
,ISNULL(iss.NBRPrice,0) NBRPrice  
,ISNULL(iss.CostPrice,0) CostPrice  
,iss.UOM
,ISNULL(iss.VATRate,0) VATRate  
,ISNULL(iss.VATAmount,0) VATAmount  
,ISNULL(iss.SubTotal,0) SubTotal  
,iss.Comments
,iss.CreatedBy
,iss.CreatedOn
,iss.LastModifiedBy
,iss.LastModifiedOn
,iss.ReceiveNo
,iss.IssueDateTime
,ISNULL(iss.SD,0) SD  
,ISNULL(iss.SDAmount,0) SDAmount  
,ISNULL(iss.Wastage,0) Wastage  
,iss.BOMDate
,iss.FinishItemNo
,iss.Post
,iss.TransactionType
,iss.IssueReturnId
,ISNULL(iss.DiscountAmount,0) DiscountAmount  
,ISNULL(iss.DiscountedNBRPrice,0) DiscountedNBRPrice  
,iss.UOMQty
,ISNULL(iss.UOMPrice,0) UOMPrice  
,ISNULL(iss.UOMc,0) UOMc  
,iss.UOMn
,iss.BOMId
,ISNULL(iss.UOMWastage,0) UOMWastage  
,iss.IsProcess
,p.ProductCode
,p.ProductName
FROM IssueDetailBOMs iss left outer join Products p on iss.ItemNo=p.ItemNo
WHERE  1=1

";
                if (issueNo != null)
                {
                    sqlText += "AND iss.IssueNo=@issueNo";
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

                if (issueNo != null)
                {
                    objComm.Parameters.AddWithValue("@issueNo", issueNo);
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
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {

                    vm = new IssueDetailVM();
                    vm.Id = dr["Id"].ToString();
                    vm.IssueNo = dr["IssueNo"].ToString();
                    vm.ItemNo = dr["ItemNo"].ToString();
                    vm.Quantity = Convert.ToDecimal(dr["Quantity"].ToString());
                    vm.NBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                    vm.CostPrice = Convert.ToDecimal(dr["CostPrice"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.SubTotal = Convert.ToDecimal(dr["SubTotal"].ToString());
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.Wastage = Convert.ToDecimal(dr["Wastage"].ToString());
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.Post = dr["Post"].ToString();
                    vm.UOMQty = Convert.ToDecimal(dr["UOMQty"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.ReceiveNo = dr["ReceiveNo"].ToString();
                    vm.IssueDateTime = dr["IssueDateTime"].ToString();
                    vm.transactionType = dr["TransactionType"].ToString();
                    vm.IssueReturnId = dr["IssueReturnId"].ToString();
                    vm.DiscountAmount = Convert.ToDecimal(dr["DiscountAmount"].ToString());
                    vm.DiscountedNBRPrice = Convert.ToDecimal(dr["DiscountedNBRPrice"].ToString());
                    vm.BOMId =  Convert.ToInt32(dr["BOMId"]);
                    vm.IsProcess = dr["IsProcess"].ToString();
                    vm.UOMWastage = Convert.ToDecimal(dr["UOMWastage"].ToString());
                    vm.ItemName = dr["ProductName"].ToString();
                    vm.ProductCode = dr["ProductCode"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    ////reading newly added fields
                    vm.IssueLineNo = dr["IssueLineNo"].ToString();
                    vm.CommentsD = dr["Comments"].ToString();
                    vm.BOMDate = dr["BOMDate"].ToString();

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
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectIssueDetail", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());

                FileLogger.Log("IssueBOMDAL", "SelectIssueDetail", ex.ToString() + "\n" + sqlText);
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

        public string[] ImportExcelFile(IssueMasterVM paramVM, SysDBInfoVMTemp connVM = null)
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
                //DataTable dt = new DataTable();
                #region Excel Reader

                string FileName = paramVM.File.FileName;
                string Fullpath = AppDomain.CurrentDomain.BaseDirectory + "Files\\Export\\" + FileName;
                System.IO.File.Delete(Fullpath);
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
                System.IO.File.Delete(Fullpath);
                #endregion

                DataTable dtIssueM = new DataTable();
                dtIssueM = ds.Tables["IssueM"];

                DataTable dtIssueD = new DataTable();
                dtIssueD = ds.Tables["IssueD"];


                dtIssueM.Columns.Add("Transection_Type");
                dtIssueM.Columns.Add("Created_By");
                dtIssueM.Columns.Add("LastModified_By");
                foreach (DataRow row in dtIssueM.Rows)
                {
                    row["Transection_Type"] = paramVM.transactionType;
                    row["Created_By"] = paramVM.CreatedBy;
                    row["LastModified_By"] = paramVM.CreatedBy;

                }


                //dt = ds.Tables[0].Select("empCode <>''").CopyToDataTable();

                #region Data Insert
                //PurchaseDAL puchaseDal = new PurchaseDAL();
                retResults = ImportData(dtIssueM, dtIssueD);
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

                FileLogger.Log("IssueBOMDAL", "ImportExcelFile", ex.ToString() + "\n" + sqlText);

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

        private void SetDefaultValue(IssueMasterVM vm, SysDBInfoVMTemp connVM = null)
        {
            if (string.IsNullOrWhiteSpace(vm.Comments))
            {
                vm.Comments = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.SerialNo))
            {
                vm.SerialNo = "-";
            }
            if (string.IsNullOrWhiteSpace(vm.ReceiveNo))
            {
                vm.ReceiveNo = "-";
            }
        }
        #endregion

        #region plain methods
        public string[] IssueInsertToMaster(IssueMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

            int IDExist = 0;


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
                if (Convert.ToDateTime(Master.IssueDateTime) < Convert.ToDateTime(firstDate))
                {
                    retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion
                #region  Insert new Information in Header


                sqlText = "";
                sqlText += " insert into IssueHeaderBOMs(";
                sqlText += " IssueNo,";
                sqlText += " IssueDateTime,";
                sqlText += " TotalVATAmount,";
                sqlText += " TotalAmount,";
                sqlText += " SerialNo,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " ReceiveNo,";
                sqlText += " transactionType,";
                sqlText += " IssueReturnId,";
                sqlText += " ImportIDExcel,";
                sqlText += " ShiftId,";

                sqlText += " Post";
                sqlText += " )";

                sqlText += " values";
                sqlText += " (";
                sqlText += " @MasterIssueNo,";
                sqlText += " @MasterIssueDateTime,";
                sqlText += " @MasterTotalVATAmount,";
                sqlText += " @MasterTotalAmount,";
                sqlText += " @MasterSerialNo,";
                sqlText += " @MasterComments,";
                sqlText += " @MasterCreatedBy,";
                sqlText += " @MasterCreatedOn,";
                sqlText += " @MasterLastModifiedBy,";
                sqlText += " @MasterLastModifiedOn,";
                sqlText += " @MasterReceiveNo,";
                sqlText += " @MastertransactionType,";
                sqlText += " @MasterReturnId,";
                sqlText += " @MasterImportId,";
                sqlText += " @MasterShiftId,";
                sqlText += " @MasterPost";
                sqlText += ")  SELECT SCOPE_IDENTITY()";


                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                cmdInsert.Transaction = transaction;

                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", Master.IssueNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterIssueDateTime", Master.IssueDateTime);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalAmount", Master.TotalAmount);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSerialNo", Master.SerialNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReceiveNo", Master.ReceiveNo);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MastertransactionType", Master.transactionType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterReturnId", Master.ReturnId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterImportId", Master.ImportId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterShiftId", Master.ShiftId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[4] = transResult.ToString();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
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
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + Master.IssueNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueInsertToMaster", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueInsertToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] IssueInsertToDetails(IssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " insert into IssueDetailBOMs(";
                sqlText += " IssueNo,";
                sqlText += " IssueLineNo,";
                sqlText += " ItemNo,";
                sqlText += " Quantity,";
                sqlText += " NBRPrice,";
                sqlText += " CostPrice,";
                sqlText += " UOM,";
                sqlText += " VATRate,";
                sqlText += " VATAmount,";
                sqlText += " SubTotal,";
                sqlText += " Comments,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " ReceiveNo,";
                sqlText += " IssueDateTime,";
                sqlText += " SD,";
                sqlText += " SDAmount,";
                sqlText += " Wastage,";
                sqlText += " BOMDate,";
                sqlText += " FinishItemNo,";
                sqlText += " transactionType,";
                sqlText += " IssueReturnId,";
                sqlText += " UOMQty,";
                sqlText += " UOMPrice,";
                sqlText += " UOMc,";
                sqlText += " UOMn,";
                sqlText += " Post";
                sqlText += " )";

                sqlText += " values(	";

                sqlText += "@DetailIssueNo,";
                sqlText += "@DetailIssueLineNo,";
                sqlText += "@DetailItemNo,";
                sqlText += "@DetailQuantity,";
                sqlText += " 0,";
                sqlText += "@DetailCostPrice,";
                sqlText += "@DetailUOM,";
                sqlText += "@DetailVATRate,";
                sqlText += "@DetailVATAmount,";
                sqlText += "@DetailSubTotal,";
                sqlText += "@DetailCommentsD,";
                sqlText += "@DetailCreatedBy,";
                sqlText += "@DetailCreatedOn,";
                sqlText += "@DetailLastModifiedBy,";
                sqlText += "@DetailLastModifiedOn,";
                sqlText += "@DetailReceiveNoD,";
                sqlText += "@DetailIssueDateTimeD,";
                sqlText += " 0,	";
                sqlText += " 0,";
                sqlText += "@DetailWastage,";
                sqlText += "@DetailBOMDate,";
                sqlText += "@DetailFinishItemNo,";
                sqlText += "@DetailtransactionType,";
                sqlText += "@DetailReturnId,";
                sqlText += "@DetailUOMQty,";
                sqlText += "@DetailUOMPrice,";
                sqlText += "@DetailUOMc,";
                sqlText += "@DetailUOMn,";
                sqlText += "@DetailPost";
                sqlText += ")	";


                SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                cmdInsDetail.Transaction = transaction;

                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueNo", Detail.IssueNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueLineNo", Detail.IssueLineNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailItemNo", Detail.ItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailQuantity", Detail.Quantity);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCostPrice", Detail.CostPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOM", Detail.UOM);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailVATRate", Detail.VATRate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailVATAmount", Detail.VATAmount);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailSubTotal", Detail.SubTotal);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCommentsD", Detail.CommentsD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCreatedBy", Detail.CreatedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailCreatedOn", Detail.CreatedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailLastModifiedBy", Detail.LastModifiedBy);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailLastModifiedOn", Detail.LastModifiedOn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailReceiveNoD", Detail.ReceiveNoD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailIssueDateTimeD", Detail.IssueDateTimeD);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailWastage", Detail.Wastage);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailBOMDate", Detail.BOMDate);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailFinishItemNo", Detail.FinishItemNo);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailtransactionType", Detail.transactionType);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailReturnId", Detail.ReturnId);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMQty", Detail.UOMQty);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMPrice", Detail.UOMPrice);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMc", Detail.UOMc);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailUOMn", Detail.UOMn);
                cmdInsDetail.Parameters.AddWithValueAndNullHandle("@DetailPost", Detail.Post);

                transResult = (int)cmdInsDetail.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }
                #endregion Insert only DetailTable

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
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + Detail.IssueNo;
                retResults[3] = "" + PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueInsertToDetails", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueInsertToDetails", ex.ToString() + "\n" + sqlText);
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

        public string[] IssueUpdateToMaster(IssueMasterVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            int nextId = 0;

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
                if (Convert.ToDateTime(Master.IssueDateTime) < Convert.ToDateTime(firstDate))
                {
                    retResults[1] = "No Entry Allowed Before " + firstDate + "!";
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, retResults[1]);
                }

                #endregion
                #region update
                sqlText = "";

                sqlText += " update IssueHeaderBOMs set  ";

                sqlText += "  IssueDateTime  =@MasterIssueDateTime";
                sqlText += " ,TotalVATAmount =@MasterTotalVATAmount";
                sqlText += " ,TotalAmount    =@MasterTotalAmount";
                sqlText += " ,SerialNo       =@MasterSerialNo";
                sqlText += " ,Comments       =@MasterComments";
                sqlText += " ,LastModifiedBy =@MasterLastModifiedBy ";
                sqlText += " ,LastModifiedOn =@MasterLastModifiedOn";
                sqlText += " ,ReceiveNo      =@MasterReceiveNo";
                sqlText += " ,transactionType=@MastertransactionType";
                sqlText += " ,IssueReturnId  =@MasterReturnId ";
                sqlText += " ,ShiftId       =@MasterShiftId ";
                sqlText += "  where  IssueNo =@MasterIssueNo ";


                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValue("@MasterIssueDateTime", Master.IssueDateTime);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalVATAmount", Master.TotalVATAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterTotalAmount", Master.TotalAmount);
                cmdUpdate.Parameters.AddWithValue("@MasterSerialNo", Master.SerialNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterComments", Master.Comments ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedBy", Master.LastModifiedBy ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterLastModifiedOn", Master.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValue("@MasterReceiveNo", Master.ReceiveNo ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MastertransactionType", Master.transactionType ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterReturnId", Master.ReturnId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterShiftId", Master.ShiftId ?? Convert.DBNull);
                cmdUpdate.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo ?? Convert.DBNull);

                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header


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
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdateToMaster", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdateToMaster", ex.ToString() + "\n" + sqlText);
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

        public string[] IssueUpdateToDetails(IssueDetailVM Detail, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region update
                sqlText = "";

                sqlText += " update IssueDetailBOMs set  ";

                sqlText += "  IssueNo            =@IssueNo";
                sqlText += " ,IssueLineNo        =@IssueLineNo";
                sqlText += " ,ItemNo             =@ItemNo";
                sqlText += " ,Quantity           =@Quantity";
                sqlText += " ,NBRPrice           =@NBRPrice";
                sqlText += " ,CostPrice          =@CostPrice";
                sqlText += " ,UOM                =@UOM";
                sqlText += " ,VATRate            =@VATRate";
                sqlText += " ,VATAmount          =@VATAmount";
                sqlText += " ,SubTotal           =@SubTotal";
                sqlText += " ,CreatedBy          =@CreatedBy";
                sqlText += " ,CreatedOn          =@CreatedOn";
                sqlText += " ,LastModifiedBy     =@LastModifiedBy";
                sqlText += " ,LastModifiedOn     =@LastModifiedOn";
                sqlText += " ,ReceiveNo          =@ReceiveNo";
                sqlText += " ,IssueDateTime      =@IssueDateTime";
                sqlText += " ,SD                 =@SD";
                sqlText += " ,SDAmount           =@SDAmount";
                sqlText += " ,Wastage            =@Wastage";
                sqlText += " ,BOMDate            =@BOMDate";
                sqlText += " ,FinishItemNo       =@FinishItemNo";
                sqlText += " ,Post               =@Post";
                sqlText += " ,TransactionType    =@TransactionType";
                sqlText += " ,IssueReturnId      =@IssueReturnId";
                sqlText += " ,DiscountAmount     =@DiscountAmount";
                sqlText += " ,DiscountedNBRPrice =@DiscountedNBRPrice";
                sqlText += " ,UOMQty             =@UOMQty";
                sqlText += " ,UOMPrice           =@UOMPrice";
                sqlText += " ,UOMc               =@UOMc";
                sqlText += " ,UOMn               =@UOMn";
                sqlText += " ,BOMId              =@BOMId";
                sqlText += " ,UOMWastage         =@UOMWastage";
                sqlText += " ,IsProcess          =@IsProcess";
                sqlText += " where             Id=@Id ";

                SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                cmdUpdate.Transaction = transaction;

                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueNo", Detail.IssueNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueLineNo", Detail.IssueLineNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ItemNo", Detail.ItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Quantity", Detail.Quantity);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@NBRPrice", Detail.NBRPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CostPrice", Detail.CostPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOM", Detail.UOM);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATRate", Detail.VATRate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATAmount", Detail.VATAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SubTotal", Detail.SubTotal);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedBy", Detail.CreatedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@CreatedOn", Detail.CreatedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedBy", Detail.LastModifiedBy);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@LastModifiedOn", Detail.LastModifiedOn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@ReceiveNo", Detail.ReceiveNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueDateTime", Detail.IssueDateTime);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SD", Detail.SD);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@SDAmount", Detail.SDAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Wastage", Detail.Wastage);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMDate", Detail.BOMDate);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@FinishItemNo", Detail.FinishItemNo);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Post", Detail.Post);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Detail.transactionType);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IssueReturnId", Detail.IssueReturnId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DiscountAmount", Detail.DiscountAmount);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@DiscountedNBRPrice", Detail.DiscountedNBRPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMQty", Detail.UOMQty);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMPrice", Detail.UOMPrice);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMc", Detail.UOMc);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMn", Detail.UOMn);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMId", Detail.BOMId);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@UOMWastage", Detail.UOMWastage);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@IsProcess", Detail.IsProcess);
                cmdUpdate.Parameters.AddWithValueAndNullHandle("@Id", Detail.Id);


                transResult = (int)cmdUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }
                #endregion update Header

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
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Detail.Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdateToDetails", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdateToDetails", ex.ToString() + "\n" + sqlText);
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

        public string[] IssueAllPost(PostVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                sqlText += " update IssueHeaderBOMs set  ";
                sqlText += " LastModifiedBy             = @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn             = @MasterLastModifiedOn,";
                sqlText += " Post                       = @MasterPost";
                sqlText += " where  IssueNo   = @MasterPurchaseInvoiceNo ";

                sqlText += " update IssueDetailBOMs set ";
                sqlText += " LastModifiedBy= @MasterLastModifiedBy,";
                sqlText += " LastModifiedOn= @MasterLastModifiedOn,";
                sqlText += " Post=@MasterPost ";
                sqlText += " where  IssueNo =@MasterPurchaseInvoiceNo ";


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
            catch (SqlException sqlex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueAllPost", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IssueBOMDAL", "IssueAllPost", ex.ToString() + "\n" + sqlText);

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

        #region need to parameterize

        //currConn to VcurrConn 25-Aug-2020
        public string[] IssueInsert(IssueMasterVM Master, List<IssueDetailVM> Details, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            var Id = "";
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

            ////SqlConnection vcurrConn = VcurrConn;

            ////if (vcurrConn == null)
            ////{
            ////    VcurrConn = null;
            ////    Vtransaction = null;
            ////}

            int transResult = 0;
            string sqlText = "";

            string newID = "";
            string PostStatus = "";

            int IDExist = 0;


            #endregion Initializ

            #region Try
            try
            {
                #region Validation for Header


                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

                }


                #endregion Validation for Header

                #region open connection and transaction
                ////if (vcurrConn == null)
                ////{
                ////    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                ////    if (VcurrConn.State != ConnectionState.Open)
                ////    {
                ////        VcurrConn.Open();
                ////    }

                ////    Vtransaction = VcurrConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);
                ////}

                #endregion open connection and transaction

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
                    transaction = currConn.BeginTransaction(MessageVM.depMsgMethodNameInsert);
                }

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaderBOMs WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
                cmdExistTran.Transaction = transaction;
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@MasterIssueNo", Master.IssueNo);
                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                }

                #endregion Find Transaction Exist

                #region Purchase ID Create
                if (string.IsNullOrEmpty(Master.transactionType)) //start
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.msgTransactionNotDeclared);
                }
                #region Purchase ID Create For Other

                CommonDAL commonDal = new CommonDAL();

                if (Master.transactionType == "Other")
                {
                    newID = commonDal.TransactionCode("Issue", "Other", "IssueHeaderBOMs", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction);


                }
                if (Master.transactionType == "IssueReturn")
                {
                    newID = commonDal.TransactionCode("Issue", "IssueReturn", "IssueHeaderBOMs", "IssueNo",
                                              "IssueDateTime", Master.IssueDateTime, Master.BranchId.ToString(), currConn, transaction);


                }
                #endregion Purchase ID Create For Other



                #endregion Purchase ID Create Not Complete

                #region ID generated completed,Insert new Information in Header

                IssueMasterVM imVM = new IssueMasterVM();
                imVM.IssueNo = newID;
                imVM.IssueDateTime = Master.IssueDateTime;
                imVM.TotalVATAmount = Master.TotalVATAmount;
                imVM.TotalAmount = Master.TotalAmount;
                imVM.SerialNo = Master.SerialNo;
                imVM.Comments = Master.Comments;
                imVM.CreatedBy = Master.CreatedBy;
                imVM.CreatedOn = Master.CreatedOn;
                imVM.LastModifiedBy = Master.LastModifiedBy;
                imVM.LastModifiedOn = Master.LastModifiedOn;
                imVM.ReceiveNo = Master.ReceiveNo;
                imVM.transactionType = Master.transactionType;
                imVM.ReturnId = Master.ReturnId;
                imVM.ImportId = Master.ImportId;
                imVM.Post = Master.Post;

                retResults = IssueInsertToMaster(imVM, currConn, transaction);
                Id = retResults[4];
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                }

                #region Comments
                
                //sqlText = "";
                //sqlText += " insert into IssueHeaderBOMs(";
                //sqlText += " IssueNo                                ,";
                //sqlText += " IssueDateTime                                ,";
                //sqlText += " TotalVATAmount                                ,";
                //sqlText += " TotalAmount                                ,";
                //sqlText += " SerialNo                                ,";
                //sqlText += " Comments                                ,";
                //sqlText += " CreatedBy                                ,";
                //sqlText += " CreatedOn                                ,";
                //sqlText += " LastModifiedBy                                ,";
                //sqlText += " LastModifiedOn                                ,";
                //sqlText += " ReceiveNo                                ,";
                //sqlText += " transactionType                                ,";
                //sqlText += " IssueReturnId                                ,";
                //sqlText += " ImportIDExcel                                ,";
                //sqlText += " Post                   ";
                //sqlText += " )";

                //sqlText += " values";
                //sqlText += " (";
                //sqlText += "'" + newID + "',";
                //sqlText += " '" + Master.IssueDateTime + "',";
                //sqlText += " '" + Master.TotalVATAmount + "',";
                //sqlText += " '" + Master.TotalAmount + "',";
                //sqlText += " '" + Master.SerialNo + "',";
                //sqlText += " '" + Master.Comments + "',";
                //sqlText += " '" + Master.CreatedBy + "',";
                //sqlText += " '" + Master.CreatedOn + "',";
                //sqlText += " '" + Master.LastModifiedBy + "',";
                //sqlText += " '" + Master.LastModifiedOn + "',";
                //sqlText += " '" + Master.ReceiveNo + "',";
                //sqlText += " '" + Master.transactionType + "',";
                //sqlText += " '" + Master.ReturnId + "',";
                //sqlText += " '" + Master.ImportId + "',";
                //sqlText += "'" + Master.Post + "'";
                //sqlText += ")";


                //SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
                //cmdInsert.Transaction = transaction;
                //transResult = (int)cmdInsert.ExecuteNonQuery();
                //if (transResult <= 0)
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                //}
                                    #endregion Comments


                #endregion ID generated completed,Insert new Information in Header

                #region Insert into Details(Insert complete in Header)
                #region Validation for Detail

                if (Details.Count() < 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
                }


                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetailBOMs WHERE IssueNo='" + newID + "' ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                    }

                    #endregion Find Transaction Exist

                    #region Insert only DetailTable
                    IssueDetailVM iDetVM = new IssueDetailVM();
                    iDetVM.IssueNo = newID;
                    iDetVM.IssueLineNo = Item.IssueLineNo;
                    iDetVM.ItemNo = Item.ItemNo;
                    iDetVM.Quantity = Item.Quantity;
                    iDetVM.NBRPrice = 0;
                    iDetVM.CostPrice = Item.CostPrice;
                    iDetVM.UOM = Item.UOM;
                    iDetVM.VATRate = Item.VATRate;
                    iDetVM.VATAmount = Item.VATAmount;
                    iDetVM.SubTotal = Item.SubTotal;
                    iDetVM.CommentsD = Item.CommentsD;
                    iDetVM.CreatedBy = Master.CreatedBy;
                    iDetVM.CreatedOn = Master.CreatedOn;
                    iDetVM.LastModifiedBy = Master.LastModifiedBy;
                    iDetVM.LastModifiedOn = Master.LastModifiedOn;
                    iDetVM.ReceiveNo = Item.ReceiveNoD;
                    iDetVM.IssueDateTime = Item.IssueDateTimeD;
                    iDetVM.SD = 0;
                    iDetVM.SDAmount = 0;
                    iDetVM.Wastage = Item.Wastage;
                    iDetVM.BOMDate = Item.BOMDate;
                    iDetVM.FinishItemNo = Item.FinishItemNo;
                    iDetVM.transactionType = Master.transactionType;
                    iDetVM.IssueReturnId = Master.ReturnId;
                    iDetVM.UOMQty = Item.UOMQty;
                    iDetVM.UOMPrice = Item.UOMPrice;
                    iDetVM.UOMc = Item.UOMc;
                    iDetVM.UOMn = Item.UOMn;
                    iDetVM.Post = Master.Post;

                    retResults = IssueInsertToDetails(iDetVM, currConn, transaction);
                    if (retResults[0] != "Success")
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    }

                                    #region Comments
                    //sqlText = "";
                    //sqlText += " insert into IssueDetailBOMs(";
                    //sqlText += " IssueNo                                ,";
                    //sqlText += " IssueLineNo                                ,";
                    //sqlText += " ItemNo                                ,";
                    //sqlText += " Quantity                                ,";
                    //sqlText += " NBRPrice                                ,";
                    //sqlText += " CostPrice                                ,";
                    //sqlText += " UOM                                ,";
                    //sqlText += " VATRate                                ,";
                    //sqlText += " VATAmount                                ,";
                    //sqlText += " SubTotal                                ,";
                    //sqlText += " Comments                                ,";
                    //sqlText += " CreatedBy                                ,";
                    //sqlText += " CreatedOn                                ,";
                    //sqlText += " LastModifiedBy                                ,";
                    //sqlText += " LastModifiedOn                                ,";
                    //sqlText += " ReceiveNo                                ,";
                    //sqlText += " IssueDateTime                                ,";
                    //sqlText += " SD                                ,";
                    //sqlText += " SDAmount                                ,";
                    //sqlText += " Wastage                                ,";
                    //sqlText += " BOMDate                                ,";
                    //sqlText += " FinishItemNo                                ,";
                    //sqlText += " transactionType                                ,";
                    //sqlText += " IssueReturnId                                ,";
                    //sqlText += " UOMQty                                ,";
                    //sqlText += " UOMPrice                                ,";
                    //sqlText += " UOMc                                ,";
                    //sqlText += " UOMn                                ,";
                    //sqlText += " Post                   ";
                    //sqlText += " )";

                    //sqlText += " values(	";
                    ////sqlText += "'" + Master.Id + "',";

                    //sqlText += "'" + newID                                    + "',";
                    //sqlText += "'" + Item.IssueLineNo                                    + "',";
                    //sqlText += "'" + Item.ItemNo                                    + "',";
                    //sqlText += "'" + Item.Quantity                                    + "',";
                    //sqlText += " 0,";
                    //sqlText += "'" + Item.CostPrice                                    + "',";
                    //sqlText += "'" + Item.UOM                                    + "',";
                    //sqlText += "'" + Item.VATRate                                    + "',";
                    //sqlText += "'" + Item.VATAmount                                    + "',";
                    //sqlText += "'" + Item.SubTotal                                    + "',";
                    //sqlText += "'" + Item.CommentsD                                    + "',";
                    //sqlText += "'" + Master.CreatedBy                                    + "',";
                    //sqlText += "'" + Master.CreatedOn                                    + "',";
                    //sqlText += "'" + Master.LastModifiedBy                                    + "',";
                    //sqlText += "'" + Master.LastModifiedOn                                    + "',";
                    //sqlText += "'" + Item.ReceiveNoD                                    + "',";
                    //sqlText += "'" + Item.IssueDateTimeD                                    + "',";
                    //sqlText += "0,";
                    //sqlText += "0,";
                    //sqlText += "'" + Item.Wastage                                    + "',";
                    //sqlText += "'" + Item.BOMDate                                    + "',";
                    //sqlText += "'" + Item.FinishItemNo                                    + "',";
                    //sqlText += "'" + Master.transactionType                                    + "',";
                    //sqlText += "'" + Master.ReturnId                                    + "',";
                    //sqlText += "" +  Item.UOMQty                                    + ",";
                    //sqlText += "" +  Item.UOMPrice                                    + ",";
                    //sqlText += "" +  Item.UOMc                                    + ",";
                    //sqlText += "'" + Item.UOMn                                    + "',";
                    //sqlText += "'" + Master.Post                                    + "'";
                    //sqlText += ")	";


                    //SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    //cmdInsDetail.Transaction = transaction;
                    //transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    //if (transResult <= 0)
                    //{
                    //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                    //}
                                    #endregion Comments
                    #endregion Insert only DetailTable


                }


                #endregion Insert Detail Table
                #endregion Insert into Details(Insert complete in Header)

                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct  Post from dbo.IssueHeaderBOMs WHERE IssueNo='" + newID + "'";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch

                #region Commit

                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        //if (transResult > 0)
                        //{
                            transaction.Commit();
                        //}
                    }
                }

                #endregion Commit

                #region SuccessResult
                retResults = new string[5];
                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSaveSuccessfully;
                retResults[2] = "" + newID;
                retResults[3] = "" + PostStatus;
                retResults[4] =Id;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                if (currConn == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueInsert", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                if (currConn == null)
                {

                    transaction.Rollback();
                }

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueInsert", ex.ToString() + "\n" + sqlText);
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
        
        public string[] IssueUpdate(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null)
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

            #endregion Initializ
            #region Try
            try
            {
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgNoDataToUpdate);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, "Please Check Invoice Data and Time");

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("IssueDetailBOMs", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype

                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaderBOMs WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
                }

                #endregion Find ID for Update



                #region ID check completed,update Information in Header

                #region update Header
                string[] cFields = new string[] { "IssueNo" };
                string[] cVals = new string[] { Master.IssueNo };

                IssueMasterVM imVM = SelectAllList(0, cFields, cVals, currConn, transaction,null,null).FirstOrDefault();

                imVM.IssueDateTime          =Master.IssueDateTime  ;
                imVM.TotalVATAmount         =Master.TotalVATAmount ;
                imVM.TotalAmount            =Master.TotalAmount    ;
                imVM.SerialNo               =Master.SerialNo       ;
                imVM.Comments               =Master.Comments       ;
                imVM.LastModifiedBy         =Master.LastModifiedBy ;
                imVM.LastModifiedOn         =Master.LastModifiedOn ;
                imVM.ReceiveNo              =Master.ReceiveNo      ;
                imVM.transactionType        =Master.transactionType;
                imVM.ReturnId               =Master.ReturnId       ;
                imVM.Post                   =Master.Post           ;
                sqlText = "";
                retResults = IssueUpdateToMaster(imVM, currConn, transaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                }

                                    #region Comments
                //sqlText += " update IssueHeaderBOMs set  ";

                //sqlText += " IssueDateTime          = '"+ Master.IssueDateTime                                    + "' ,";
                //sqlText += " TotalVATAmount         ='" + Master.TotalVATAmount                                    + "' ,";
                //sqlText += " TotalAmount            ='" + Master.TotalAmount                                    + "' ,";
                //sqlText += " SerialNo               ='" + Master.SerialNo                                    + "' ,";
                //sqlText += " Comments               ='" + Master.Comments                                    + "' ,";
                //sqlText += " LastModifiedBy         ='" + Master.LastModifiedBy                                    + "' ,";
                //sqlText += " LastModifiedOn         ='" + Master.LastModifiedOn                                    + "' ,";
                //sqlText += " ReceiveNo              ='" + Master.ReceiveNo                                    + "' ,";
                //sqlText += " transactionType        ='" + Master.transactionType                                    + "' ,";
                //sqlText += " IssueReturnId          ='" + Master.ReturnId                                    + "' ,";
                //sqlText += " Post                   = '"+ Master.Post                                    + "' ";
                //sqlText += " where  IssueNo= '" + Master.IssueNo + "' ";


                //SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                //cmdUpdate.Transaction = transaction;
                //transResult = (int)cmdUpdate.ExecuteNonQuery();
                //if (transResult <= 0)
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                //}
                                    #endregion Comments
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

                foreach (var Item in Details.ToList())
                {
                    #region Find Transaction Mode Insert or Update

                    sqlText = "";
                    sqlText += "select COUNT(IssueNo) from IssueDetailBOMs WHERE IssueNo=@MasterIssueNo  ";
                    sqlText += " AND ItemNo=@ItemItemNo ";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        // Insert
                        #region Insert only DetailTable
                        IssueDetailVM iDetVM = new IssueDetailVM();
                        iDetVM.IssueNo                  =Master.IssueNo           ;       
                        iDetVM.IssueLineNo              =Item.IssueLineNo         ;
                        iDetVM.ItemNo                   =Item.ItemNo              ;
                        iDetVM.Quantity                 =Item.Quantity            ;
                        iDetVM.NBRPrice                 =0;
                        iDetVM.CostPrice                =Item.CostPrice           ;
                        iDetVM.UOM                      =Item.UOM                 ;
                        iDetVM.VATRate                  =Item.VATRate             ;
                        iDetVM.VATAmount                =Item.VATAmount           ;
                        iDetVM.SubTotal                 =Item.SubTotal            ;
                        iDetVM.CommentsD                 =Item.CommentsD           ;
                        iDetVM.CreatedBy                =Master.CreatedBy         ;
                        iDetVM.CreatedOn                =Master.CreatedOn         ;
                        iDetVM.LastModifiedBy           =Master.LastModifiedBy    ;
                        iDetVM.LastModifiedOn           =Master.LastModifiedOn    ;
                        iDetVM.ReceiveNo                =Item.ReceiveNoD          ;
                        iDetVM.IssueDateTime            =Item.IssueDateTimeD      ;
                        iDetVM.SD                       =0;
                        iDetVM.SDAmount                 =0;
                        iDetVM.Wastage                  =Item.Wastage             ;
                        iDetVM.BOMDate                  =Item.BOMDate             ;
                        iDetVM.FinishItemNo             =Item.FinishItemNo        ;
                        iDetVM.transactionType          =Master.transactionType   ;
                        iDetVM.IssueReturnId            =Master.ReturnId          ;
                        iDetVM.UOMQty                   =Item.UOMQty              ;
                        iDetVM.UOMPrice                 =Item.UOMPrice            ;
                        iDetVM.UOMc                     =Item.UOMc                ;
                        iDetVM.UOMn                     =Item.UOMn                ;
                        iDetVM.Post                     =Master.Post              ;

                        retResults = IssueInsertToDetails(iDetVM, currConn, transaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }

                                    #region Comments
                        ////sqlText = "";
                        ////sqlText += " insert into IssueDetailBOMs(";

                        ////sqlText += " IssueNo                                ,";
                        ////sqlText += " IssueLineNo                                ,";
                        ////sqlText += " ItemNo                                ,";
                        ////sqlText += " Quantity                                ,";
                        ////sqlText += " NBRPrice                                ,";
                        ////sqlText += " CostPrice                                ,";
                        ////sqlText += " UOM                                ,";
                        ////sqlText += " VATRate                                ,";
                        ////sqlText += " VATAmount                                ,";
                        ////sqlText += " SubTotal                                ,";
                        ////sqlText += " Comments                                ,";
                        ////sqlText += " CreatedBy                                ,";
                        ////sqlText += " CreatedOn                                ,";
                        ////sqlText += " LastModifiedBy                                ,";
                        ////sqlText += " LastModifiedOn                                ,";
                        ////sqlText += " ReceiveNo                                ,";
                        ////sqlText += " IssueDateTime                                ,";
                        ////sqlText += " SD                                ,";
                        ////sqlText += " SDAmount                                ,";
                        ////sqlText += " Wastage                                ,";
                        ////sqlText += " BOMDate                                ,";
                        ////sqlText += " FinishItemNo                                ,";
                        ////sqlText += " transactionType                                ,";
                        ////sqlText += " IssueReturnId                                ,";
                        ////sqlText += " UOMQty                                ,";
                        ////sqlText += " UOMPrice                                ,";
                        ////sqlText += " UOMc                                ,";
                        ////sqlText += " UOMn                                ,";
                        ////sqlText += " Post               ";
                        ////sqlText += " )";
                        ////sqlText += " values(	";
                        //////sqlText += "'" + Master.Id + "',";

                        ////sqlText += "'" + Master.IssueNo                                    + "',";
                        ////sqlText += "'" + Item.IssueLineNo                                    + "',";
                        ////sqlText += "'" + Item.ItemNo                                    + "',";
                        ////sqlText += "'" + Item.Quantity                                    + "',";
                        ////sqlText += " 0,";
                        ////sqlText += "'" + Item.CostPrice                                    + "',";
                        ////sqlText += "'" + Item.UOM                                    + "',";
                        ////sqlText += "'" + Item.VATRate                                    + "',";
                        ////sqlText += "'" + Item.VATAmount                                    + "',";
                        ////sqlText += "'" + Item.SubTotal                                    + "',";
                        ////sqlText += "'" + Item.CommentsD                                    + "',";
                        ////sqlText += "'" + Master.CreatedBy                                    + "',";
                        ////sqlText += "'" + Master.CreatedOn                                    + "',";
                        ////sqlText += "'" + Master.LastModifiedBy                                    + "',";
                        ////sqlText += "'" + Master.LastModifiedOn                                    + "',";
                        ////sqlText += "'" + Item.ReceiveNoD                                    + "',";
                        ////sqlText += "'" + Item.IssueDateTimeD                                    + "',";
                        ////sqlText += "0,";
                        ////sqlText += "0,";
                        ////sqlText += "'" + Item.Wastage                                    + "',";
                        ////sqlText += "'" + Item.BOMDate                                    + "',";
                        ////sqlText += "'" + Item.FinishItemNo                                    + "',";
                        ////sqlText += "'" + Master.transactionType                                    + "',";
                        ////sqlText += "'" + Master.ReturnId                                    + "',";
                        ////sqlText += "" +  Item.UOMQty                                    + ",";
                        ////sqlText += "" +  Item.UOMPrice                                    + ",";
                        ////sqlText += "" +  Item.UOMc                                    + ",";
                        ////sqlText += "'" + Item.UOMn                                    + "',";
                        ////sqlText += "'" + Master.Post                                    + "'";
                        ////sqlText += ")	";


                        ////SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        ////cmdInsDetail.Transaction = transaction;
                        ////transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        ////if (transResult <= 0)
                        ////{
                        ////    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        ////}
                                    #endregion Comments

                        #endregion Insert only DetailTable
                       
                    }
                    else
                    {
                        //Update

                        #region Update only DetailTable
                        if (!string.IsNullOrEmpty(Item.FinishItemNo))
                        {
                            if (Item.FinishItemNo != "N/A" && Item.FinishItemNo != "0")
                            {
                                cFields = new string[] { "iss.IssueNo", "iss.ItemNo", "iss.FinishItemNo" };
                                cVals = new string[] { Master.IssueNo, Item.ItemNo, Item.FinishItemNo };
                            }
                        }
                        else
                        {
                            cFields = new string[] { "iss.IssueNo", "iss.ItemNo" };
                            cVals = new string[] { Master.IssueNo, Item.ItemNo };
                        }
                        IssueDetailVM iDetVM = SelectIssueDetail(null, cFields, cVals, currConn, transaction).FirstOrDefault();

                        iDetVM.IssueLineNo          =Item.IssueLineNo             ;     
                        iDetVM.Quantity             =Item.Quantity                ;
                        iDetVM.CostPrice            =Item.CostPrice               ;
                        iDetVM.UOM                  =Item.UOM                     ;
                        iDetVM.SubTotal             =Item.SubTotal                ;
                        iDetVM.CommentsD            = Item.CommentsD;
                        iDetVM.LastModifiedBy       =Master.LastModifiedBy        ;
                        iDetVM.LastModifiedOn       =Master.LastModifiedOn        ;
                        iDetVM.ReceiveNo            =Item.ReceiveNoD              ;
                        iDetVM.IssueDateTime        =Item.IssueDateTimeD          ;
                        iDetVM.Wastage              =Item.Wastage                 ;
                        iDetVM.BOMDate              =Item.BOMDate                 ;
                        iDetVM.transactionType      =Master.transactionType       ;
                        iDetVM.IssueReturnId        =Master.ReturnId              ;
                        iDetVM.UOMQty               =Item.UOMQty                  ;
                        iDetVM.UOMPrice             =Item.UOMPrice                ;
                        iDetVM.UOMc                 =Item.UOMc                    ;
                        iDetVM.UOMn                 = Item.UOMn                   ;
                        iDetVM.Post                 =Master.Post                  ;

                        retResults = IssueUpdateToDetails(iDetVM, currConn, transaction);
                        if (retResults[0] != "Success")
                        {
                            throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        }

                                    #region Comments
                        //sqlText = "";
                        //sqlText += " update IssueDetailBOMs set ";


                        //sqlText += " IssueLineNo            ='" + Item.IssueLineNo                                    + "',";
                        //sqlText += " Quantity               ='" + Item.Quantity                                    + "',";
                        //sqlText += " CostPrice              ='" + Item.CostPrice                                    + "',";
                        //sqlText += " UOM                    ='" + Item.UOM                                    + "',";
                        //sqlText += " SubTotal               ='" + Item.SubTotal                                    + "',";
                        //sqlText += " Comments               ='" + Item.CommentsD                                    + "',";
                        //sqlText += " LastModifiedBy         ='" + Master.LastModifiedBy                                    + "',";
                        //sqlText += " LastModifiedOn         ='" + Master.LastModifiedOn                                    + "',";
                        //sqlText += " ReceiveNo              ='" + Item.ReceiveNoD                                    + "',";
                        //sqlText += " IssueDateTime          ='" + Item.IssueDateTimeD                                    + "',";
                        //sqlText += " Wastage                ='" + Item.Wastage                                    + "',";
                        //sqlText += " BOMDate                ='" + Item.BOMDate                                    + "',";
                        //sqlText += " transactionType        ='" + Master.transactionType                                    + "',";
                        //sqlText += " IssueReturnId          ='" + Master.ReturnId                                    + "',";
                        //sqlText += " UOMQty                 = " + Item.UOMQty                                    + ",";
                        //sqlText += " UOMPrice               = " + Item.UOMPrice                                    + ",";
                        //sqlText += " UOMc                   = " + Item.UOMc                                    + ",";
                        //sqlText += " UOMn                   = '" + Item.UOMn                                    + "',";
                        //sqlText += " Post                   ='" + Master.Post                                    + "'";
                        //sqlText += " where  IssueNo ='" + Master.IssueNo + "' ";
                        //sqlText += " and ItemNo='" + Item.ItemNo + "'";
                        //if (!string.IsNullOrEmpty(Item.FinishItemNo))
                        //{
                        //    if (Item.FinishItemNo != "N/A" && Item.FinishItemNo != "0")
                        //    {
                        //        sqlText += " and FinishItemNo='" + Item.FinishItemNo + "'";
                        //    }
                        //}


                        //SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        //cmdInsDetail.Transaction = transaction;
                        //transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        //if (transResult <= 0)
                        //{
                        //    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                        //}
                                    #endregion Comments
                        #endregion Update only DetailTable

                    }

                    #endregion Find Transaction Mode Insert or Update
                }//foreach (var Item in Details.ToList())

                #region Remove row
                sqlText = "";
                sqlText += " SELECT  distinct ItemNo";
                sqlText += " from IssueDetailBOMs WHERE IssueNo=@MasterIssueNo";

                DataTable dt = new DataTable("Previous");
                SqlCommand cmdRIFB = new SqlCommand(sqlText, currConn);
                cmdRIFB.Transaction = transaction;
                cmdRIFB.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                SqlDataAdapter dta = new SqlDataAdapter(cmdRIFB);
                dta.Fill(dt);
                foreach (DataRow pItem in dt.Rows)
                {
                    var p = pItem["ItemNo"].ToString();

                    //var tt= Details.Find(x => x.ItemNo == p);
                    var tt = Details.Count(x => x.ItemNo.Trim() == p.Trim());
                    if (tt == 0)
                    {
                        sqlText = "";
                        sqlText += " delete FROM IssueDetailBOMs ";
                        sqlText += " WHERE IssueNo=@MasterIssueNo ";
                        sqlText += " AND ItemNo='" + p + "'";
                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                        transResult = (int)cmdInsDetail.ExecuteNonQuery();
                    }

                }
                #endregion Remove row


                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaderBOMs WHERE IssueNo=@MasterIssueNo";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    //if (transResult > 0)
                    //{
                        transaction.Commit();
                    //}

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
                retResults[2] = Master.IssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try
            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdate", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssueUpdate", ex.ToString() + "\n" + sqlText);
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public string[] IssuePost(IssueMasterVM Master, List<IssueDetailVM> Details, SysDBInfoVMTemp connVM = null, string UserId = "")
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
                string vNegStockAllow = string.Empty;
                CommonDAL commonDal = new CommonDAL();
                vNegStockAllow = commonDal.settings("Sale", "NegStockAllow");
                if (string.IsNullOrEmpty(vNegStockAllow))
                {
                    throw new ArgumentNullException(MessageVM.msgSettingValueNotSave, "Sale");
                }
                bool NegStockAllow = Convert.ToBoolean(vNegStockAllow == "Y" ? true : false);
                #region Validation for Header

                if (Master == null)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                }
                else if (Convert.ToDateTime(Master.IssueDateTime) < DateTime.MinValue || Convert.ToDateTime(Master.IssueDateTime) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgCheckDatePost);

                }



                #endregion Validation for Header
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #region Add BOMId

                //commonDal.TableFieldAdd("IssueDetailBOMs", "BOMId", "varchar(20)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMQty", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMPrice", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMc", "decimal(25, 9)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMn", "varchar(50)", currConn); //tablename,fieldName, datatype
                //commonDal.TableFieldAdd("IssueDetailBOMs", "UOMWastage", "decimal(25, 9)", currConn); //tablename,fieldName, datatype


                #endregion Add BOMId
                transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNamePost);

                #endregion open connection and transaction
                #region Fiscal Year Check

                string transactionDate = Master.IssueDateTime;
                string transactionYearCheck = Convert.ToDateTime(Master.IssueDateTime).ToString("yyyy-MM-dd");
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
                sqlText = sqlText + "select COUNT(IssueNo) from IssueHeaderBOMs WHERE IssueNo=@MasterIssueNo  ";
                SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
                cmdFindIdUpd.Transaction = transaction;
                cmdFindIdUpd.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);

                int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

                if (IDExist <= 0)
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgUnableFindExistIDPost);
                }

                #endregion Find ID for Update

                #region ID check completed,update Information in Header
                PostVM pvm = new PostVM();
                pvm.LastModifiedBy = Master.LastModifiedBy;
                pvm.LastModifiedOn = Master.LastModifiedOn;
                pvm.Post = Master.Post;
                pvm.Code = Master.IssueNo;
                retResults = IssueAllPost(pvm, currConn, transaction);
                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                }

                #region update Header
                //sqlText = "";

                //sqlText += " update IssueHeaderBOMs set  ";
                //sqlText += " LastModifiedBy='" + Master.LastModifiedBy + "' ,";
                //sqlText += " LastModifiedOn='" + Master.LastModifiedOn + "' ,";
                //sqlText += " Post= '" + Master.Post + "' ";
                //sqlText += " where  IssueNo= '" + Master.IssueNo + "' ";


                //SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                //cmdUpdate.Transaction = transaction;
                //transResult = (int)cmdUpdate.ExecuteNonQuery();
                //if (transResult <= 0)
                //{
                //    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                //}
                #endregion update Header



                #endregion ID check completed,update Information in Header

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
                    sqlText += "select COUNT(IssueNo) from IssueDetailBOMs WHERE IssueNo=@MasterIssueNo ";
                    sqlText += " AND ItemNo=@ItemItemNo";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                    cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                    IDExist = (int)cmdFindId.ExecuteScalar();

                    if (IDExist <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgNoDataToPost);
                    }
                    else
                    {
                        #region Update only DetailTable

                        //sqlText = "";
                        //sqlText += " update IssueDetailBOMs set ";
                        //sqlText += " Post='" + Master.Post + "'";
                        //sqlText += " where  IssueNo ='" + Master.IssueNo + "' ";
                        //sqlText += " and ItemNo='" + Item.ItemNo + "'";

                        //SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        //cmdInsDetail.Transaction = transaction;
                        //transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        //if (transResult <= 0)
                        //{
                        //    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSuccessfully);
                        //}
                        #endregion Update only DetailTable

                        #region Update Item Qty
                        
                            #region Find Quantity From Products
                            ProductDAL productDal = new ProductDAL();
                            //decimal oldStock = productDal.StockInHand(Item.ItemNo, Master.IssueDateTime, currConn, transaction);
                            decimal oldStock = Convert.ToDecimal(productDal.AvgPriceNew(Item.ItemNo, Master.IssueDateTime,
                                                              currConn, transaction, true,true,true,true,null,UserId).Rows[0]["Quantity"].ToString());


                            #endregion Find Quantity From Products

                            #region Find Quantity From Transaction

                            sqlText = "";
                            sqlText += "select isnull(Quantity ,0) from IssueDetailBOMs ";
                            sqlText += " WHERE ItemNo=@ItemItemNo and IssueNo= @MasterIssueNo";
                            SqlCommand cmdTranQty = new SqlCommand(sqlText, currConn);
                            cmdTranQty.Transaction = transaction;
                            cmdTranQty.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                            cmdTranQty.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                            decimal TranQty = (decimal)cmdTranQty.ExecuteScalar();

                            #endregion Find Quantity From Transaction

                            #region Qty  check and Update
                            if (NegStockAllow == false)
                            {
                                if (TranQty > (oldStock + TranQty))
                                {
                                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost,
                                                                    MessageVM.issueMsgStockNotAvailablePost);
                                }
                            }


                            #endregion Qty  check and Update
                         

                        #endregion Qty  check and Update
                    }
                    #endregion Find Transaction Mode Insert or Update
                }

                #endregion Update Detail Table

                #endregion  Update into Details(Update complete in Header)


                #region return Current ID and Post Status

                sqlText = "";
                sqlText = sqlText + "select distinct Post from IssueHeaderBOMs WHERE IssueNo=@MasterIssueNo ";
                SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
                cmdIPS.Transaction = transaction;
                cmdIPS.Parameters.AddWithValue("@MasterIssueNo", Master.IssueNo);
                PostStatus = (string)cmdIPS.ExecuteScalar();
                if (string.IsNullOrEmpty(PostStatus))
                {
                    throw new ArgumentNullException(MessageVM.issueMsgMethodNamePost, MessageVM.issueMsgPostNotSelect);
                }


                #endregion Prefetch
                #region Commit

                if (transaction != null)
                {
                    //if (transResult > 0)
                    //{
                        transaction.Commit();
                    //}

                }

                #endregion Commit
                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.issueMsgSuccessfullyPost;
                retResults[2] = Master.IssueNo;
                retResults[3] = PostStatus;
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (SqlException sqlex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "IssuePost", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "IssuePost", ex.ToString() + "\n" + sqlText);
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result

        }
        public decimal ReturnIssueQty(string issueReturnId, string itemNo, SysDBInfoVMTemp connVM = null)
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

                sqlText = "select Sum(isnull(IssueDetailBOMs.Quantity,0)) from IssueDetailBOMs ";
                sqlText += "where ItemNo =@itemNo and IssueReturnId =@issueReturnId ";
                sqlText += "group by ItemNo";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Parameters.AddWithValue("@itemNo", itemNo);
                cmd.Parameters.AddWithValue("@issueReturnId", issueReturnId);
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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("IssueBOMDAL", "ReturnIssueQty", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("IssueBOMDAL", "ReturnIssueQty", ex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", ex.Message.ToString());

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

        #endregion
    }
}
