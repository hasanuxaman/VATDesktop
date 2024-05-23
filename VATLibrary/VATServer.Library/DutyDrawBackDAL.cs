using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VATViewModel.DTOs;
using System.Data.SqlClient;
using System.Data;
using VATServer.Interface;
using VATServer.Ordinary;

namespace VATServer.Library
{
    public class DutyDrawBackDAL : IDutyDrawBack
   {
       #region variables
       private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
       private const string FieldDelimeter = DBConstant.FieldDelimeter;
       #endregion
        
      public DataTable SearchDDBackHeader(string DDBackNo, string DDBackFromDate, string DDBackToDate
          , string DDBackSaleFromDate, string DDBackSaleToDate,string SalesInvoicNo,string CustomerName
          , string FinishGood, string Post, string BranchId = "0", string TransactionType = "DDB", SysDBInfoVMTemp connVM = null)
      {
          #region Variables

          SqlConnection currConn = null;
          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("SearchDDBackHeade");

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

              sqlText = string.Format(@"SELECT 
       d.DDBackNo
      ,convert (varchar,d.DDBackDate,120)DDBackDate
      ,d.SalesInvoiceNo
      ,convert (varchar,d.SalesDate,120)SalesDate,
isnull(d.CustormerID,0)CustormerID,
isnull(cm.CustomerName,0)CustomerName,
isnull(d.CurrencyId,0)CurrencyId,
isnull(c.CurrencyCode,0)CurrencyCode,
isnull(d.ExpCurrency,0)ExpCurrency,
isnull(d.BDTCurrency,0)BDTCurrency,
isnull(d.FgItemNo,0)FgItemNo,
isnull(ps.ProductName,0)ProductName,
isnull(d.TotalClaimCD,0)TotalClaimCD,
isnull(d.TotalClaimRD,0)TotalClaimRD,
isnull(d.TotalClaimSD,0)TotalClaimSD,
isnull(d.TotalDDBack,0)TotalDDBack,
isnull(d.TotalClaimVAT,0)TotalClaimVAT,
isnull(d.TotalClaimCnFAmount,0)TotalClaimCnFAmount,
isnull(d.TotalClaimInsuranceAmount,0)TotalClaimInsuranceAmount,
isnull(d.TotalClaimTVBAmount,0)TotalClaimTVBAmount,
isnull(d.TotalClaimTVAAmount,0)TotalClaimTVAAmount,
isnull(d.TotalClaimATVAmount,0)TotalClaimATVAmount,
isnull(d.TotalClaimOthersAmount,0)TotalClaimOthersAmount,
isnull(d.ApprovedSD,0)ApprovedSD,
isnull(d.TotalSDAmount,0)TotalSDAmount,
isnull(d.Comments,'N/A')Comments,
isnull(d.TransactionType,'DDB')TransactionType
      ,d.CreatedBy
      ,d.CreatedOn
      ,d.LastModifiedBy
      ,d.LastModifiedOn
      ,d.Post
      ,isnull(d.BranchId,0) BranchId
  FROM [dbo].[DutyDrawBackHeader] d LEFT OUTER JOIN Currencies c on c.CurrencyID=d.CurrencyId
left outer join Customers cm on cm.CustomerID=d.CustormerID
left outer join Products ps on ps.ItemNo=d.FgItemNo

 WHERE
                            (d.DDBackNo  LIKE '%' +  @DDBackNo   + '%' OR @DDBackNo IS NULL)
                            AND (d.DDBackDate>= @DDBackFromDate OR @DDBackFromDate IS NULL)
                            AND (d.DDBackDate <dateadd(d,1, @DDBackToDate) OR @DDBackToDate IS NULL)
                            AND (d.SalesDate>= @DDBackSaleFromDate OR @DDBackSaleFromDate IS NULL)
                            AND (d.SalesDate <dateadd(d,1, @DDBackSaleToDate) OR @DDBackSaleToDate IS NULL)
                            AND (d.SalesInvoiceNo  LIKE '%' +  @SalesInvoicNo   + '%' OR @SalesInvoicNo IS NULL) 
                            AND (d.CustormerID  LIKE '%' +  @CustomerName   + '%' OR @CustomerName IS NULL)
                            AND (d.FgItemNo  LIKE '%' +  @FinishGood   + '%' OR @FinishGood IS NULL)
                            AND (d.Post  LIKE '%' +  @Post   + '%' OR @Post IS NULL)
                            AND (d.BranchId  LIKE '%' +  @BranchId   + '%' OR @BranchId IS NULL)
                            AND (isnull(d.TransactionType,'DDB')  LIKE '%' +  @TransactionType   + '%' OR @TransactionType IS NULL)
                           -- AND (isnull(d.TransactionType,'DDB')='DDB')
                            ");



              #endregion

              #region SQL Command

              SqlCommand objCommIssueHeader = new SqlCommand();
              objCommIssueHeader.Connection = currConn;

              objCommIssueHeader.CommandText = sqlText;
              objCommIssueHeader.CommandType = CommandType.Text;

              #endregion

              #region Parameter

              if (!objCommIssueHeader.Parameters.Contains("@TransactionType"))
              { objCommIssueHeader.Parameters.AddWithValue("@TransactionType", TransactionType); }
              else { objCommIssueHeader.Parameters["@TransactionType"].Value = TransactionType; }

              if (!objCommIssueHeader.Parameters.Contains("@BranchId"))
              { objCommIssueHeader.Parameters.AddWithValue("@BranchId", BranchId); }
              else { objCommIssueHeader.Parameters["@BranchId"].Value = BranchId; }

              if (!objCommIssueHeader.Parameters.Contains("@Post"))
              { objCommIssueHeader.Parameters.AddWithValue("@Post", Post); }
              else { objCommIssueHeader.Parameters["@Post"].Value = Post; }

              if (!objCommIssueHeader.Parameters.Contains("@DDBackNo"))
              { objCommIssueHeader.Parameters.AddWithValue("@DDBackNo", DDBackNo); }
              else { objCommIssueHeader.Parameters["@DDBackNo"].Value = DDBackNo; }

              if (DDBackFromDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackFromDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackFromDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackFromDate", DDBackFromDate); }
                  else { objCommIssueHeader.Parameters["@DDBackFromDate"].Value = DDBackFromDate; }
              }
              if (DDBackToDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackToDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackToDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackToDate", DDBackToDate); }
                  else { objCommIssueHeader.Parameters["@DDBackToDate"].Value = DDBackToDate; }
              }

              if (DDBackSaleFromDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleFromDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleFromDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleFromDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleFromDate", DDBackFromDate); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleFromDate"].Value = DDBackFromDate; }
              }
              if (DDBackSaleToDate == "")
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleToDate", System.DBNull.Value); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleToDate"].Value = System.DBNull.Value; }
              }
              else
              {
                  if (!objCommIssueHeader.Parameters.Contains("@DDBackSaleToDate"))
                  { objCommIssueHeader.Parameters.AddWithValue("@DDBackSaleToDate", DDBackToDate); }
                  else { objCommIssueHeader.Parameters["@DDBackSaleToDate"].Value = DDBackToDate; }
              }

              if (!objCommIssueHeader.Parameters.Contains("@SalesInvoicNo"))
              { objCommIssueHeader.Parameters.AddWithValue("@SalesInvoicNo", SalesInvoicNo); }
              else { objCommIssueHeader.Parameters["@SalesInvoicNo"].Value = SalesInvoicNo; }

              // Common Filed
              if (!objCommIssueHeader.Parameters.Contains("@CustomerName"))
              { objCommIssueHeader.Parameters.AddWithValue("@CustomerName", CustomerName); }
              else { objCommIssueHeader.Parameters["@CustomerName"].Value = CustomerName; }


              // Common Filed
              if (!objCommIssueHeader.Parameters.Contains("@FinishGood"))
              { objCommIssueHeader.Parameters.AddWithValue("@FinishGood", FinishGood); }
              else { objCommIssueHeader.Parameters["@FinishGood"].Value = FinishGood; }
              

              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueHeader);
              dataAdapter.Fill(dataTable);
          }
          #endregion

          #region Catch & Finally
          catch (SqlException sqlex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
              //////throw sqlex;

              FileLogger.Log("DutyDrawBackDAL", "SearchDDBackHeader", sqlex.ToString() + "\n" + sqlText);

              throw new ArgumentNullException("", sqlex.Message.ToString());

          }
          catch (Exception ex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "SearchDDBackHeader", ex.ToString() + "\n" + sqlText);

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

      public DataTable SearchddBackDetails(string DDBackNo, string oldSaleID, SysDBInfoVMTemp connVM = null)
      {
          #region Variables

          SqlConnection currConn = null;
          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("ddBackDetails");

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
SELECT d.[DDBackNo]
      ,convert (varchar,d.[DDBackDate],120)[DDBackDate]
      ,d.[DDLineNo]
      ,d.[PurchaseInvoiceNo]
      ,convert (varchar,d.[PurchaseDate],120)[PurchaseDate]      
      ,d.[ItemNo]
      ,pR.[ProductCode] pitemcode
	  ,pR.[ProductName] pitemname
	  ,d.[FgItemNo]
	  ,pF.[ProductCode] fitemcode
	  ,pF.[ProductName] fitemname
      ,d.[BillOfEntry]
      ,d.[PurchaseUom]
      ,d.[PurchaseQuantity]
      ,d.[UnitPrice]
      ,d.[AV]
      ,d.[CD]
      ,d.[RD]
      ,d.[SD]
      ,d.[VAT]
      ,d.[CnF]
      ,d.[Insurance]
      ,d.[TVB]
      ,d.[TVA]
      ,d.[ATV]
      ,d.[Others]
      ,d.[UseQuantity]
      ,d.[ClaimCD]
      ,d.[ClaimRD]
      ,d.[ClaimSD]
      ,d.[ClaimVAT]
      ,d.[ClaimCnF]
      ,d.[ClaimInsurance]
      ,d.[ClaimTVB]
      ,d.[ClaimTVA]
      ,d.[ClaimATV]
      ,d.[ClaimOthers]
      ,d.[SubTotalDDB]
      ,d.[UOMc]
      ,d.[UOMn]
      ,d.[UOMCD]
      ,d.[UOMRD]
      ,d.[UOMSD]
      ,d.[UOMVAT]
      ,d.[UOMCnF]
      ,d.[UOMInsurance]
      ,d.[UOMTVB]
      ,d.[UOMTVA]
      ,d.[UOMATV]
      ,d.[UOMOthers]
      ,d.[UOMSubTotalDDB]
      ,d.[Post]
      ,d.[CreatedBy]
      ,d.[CreatedOn]
      ,d.[LastModifiedBy]
      ,d.[LastModifiedOn]
    ,  isnull(d.TransactionType,'DDB')TransactionType
,isnull(nullif(d.SalesInvoiceNo,''),'-')SalesInvoiceNo
,isnull(nullif(d.PurchasetransactionType,''),'-')PurchasetransactionType
,isnull(d.FGQty,0)FGQty

  FROM [dbo].[DutyDrawBackDetails] d

left outer join 
                        Products pR on pR.ItemNo=d.ItemNo left outer 
					 join Products pF on pF.ItemNo=d.FgItemNo

					 WHERE 
                         DDBackNo = @DDBackNo


";

              #endregion

              #region SQL Command

              SqlCommand objCommIssueDetail = new SqlCommand();
              objCommIssueDetail.Connection = currConn;

              objCommIssueDetail.CommandText = sqlText;
              objCommIssueDetail.CommandType = CommandType.Text;

              #endregion

              #region Parameter

              if (!objCommIssueDetail.Parameters.Contains("@DDBackNo"))
              { objCommIssueDetail.Parameters.AddWithValue("@DDBackNo", DDBackNo); }
              else { objCommIssueDetail.Parameters["@DDBackNo"].Value = DDBackNo; }

              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
              dataAdapter.Fill(dataTable);
          }
          #endregion

          #region Catch & Finally
          catch (SqlException sqlex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
              //////throw sqlex;

              FileLogger.Log("DutyDrawBackDAL", "SearchddBackDetails", sqlex.ToString() + "\n" + sqlText);

              throw new ArgumentNullException("", sqlex.Message.ToString());

          }
          catch (Exception ex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "SearchddBackDetails", ex.ToString() + "\n" + sqlText);

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

      public DataTable SearchddbSaleInvoices(string DDBackNo, SysDBInfoVMTemp connVM = null)
      {
          #region Variables

          SqlConnection currConn = null;
          string sqlText = "";
          DataTable dataTable = new DataTable("ddbSaleInvoices");

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
SELECT *

  FROM DutyDrawBackSaleInvoices
					 WHERE 
                         DDBackNo = @DDBackNo


";

              #endregion

              #region SQL Command

              SqlCommand objCommIssueDetail = new SqlCommand();
              objCommIssueDetail.Connection = currConn;

              objCommIssueDetail.CommandText = sqlText;
              objCommIssueDetail.CommandType = CommandType.Text;

              #endregion

              #region Parameter

              if (!objCommIssueDetail.Parameters.Contains("@DDBackNo"))
              { objCommIssueDetail.Parameters.AddWithValue("@DDBackNo", DDBackNo); }
              else { objCommIssueDetail.Parameters["@DDBackNo"].Value = DDBackNo; }

              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
              dataAdapter.Fill(dataTable);
          }
          #endregion

          #region Catch & Finally
          catch (SqlException sqlex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
              //////throw sqlex;

              FileLogger.Log("DutyDrawBackDAL", "SearchddbSaleInvoices", sqlex.ToString() + "\n" + sqlText);

              throw new ArgumentNullException("", sqlex.Message.ToString());

          }
          catch (Exception ex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "SearchddbSaleInvoices", ex.ToString() + "\n" + sqlText);

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

      //currConn to VcurrConn 25-Aug-2020
      public DataTable Purchase_DDBQty(string PurchaseInvoiceNo, string PurItemNo, SqlConnection VcurrConn, SqlTransaction Vtransaction, SysDBInfoVMTemp connVM = null)
      {
          #region Variables

          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataTable = new DataTable("ddBackDetails");

          #endregion

          #region Try
          try
          {
              #region open connection and transaction

              VcurrConn = _dbsqlConnection.GetConnection(connVM);
              if (VcurrConn.State != ConnectionState.Open)
              {
                  VcurrConn.Open();
              }

              #endregion open connection and transaction

              #region SQL Statement


sqlText = "  ";
sqlText += " DECLARE @PurchaseQty DECIMAL(25,9); ";
sqlText += " DECLARE @TotalDDBQty DECIMAL(25,9); ";
sqlText += " SELECT @PurchaseQty=isnull(uomQty,0)  FROM PurchaseInvoiceDetails  ";
sqlText += " WHERE  ";
sqlText += " PurchaseInvoiceNo=@PurchaseInvoiceNo ";
sqlText += " and ItemNo=@PurItemNo ";
sqlText += " SELECT @TotalDDBQty=isnull(sum(isnull(UseQuantity,0)* isnull(uomc,1)),0) ";
sqlText += " FROM DutyDrawBackDetails   ";
sqlText += " WHERE  ";
sqlText += " PurchaseInvoiceNo=@PurchaseInvoiceNo ";
sqlText += " and ItemNo=@PurItemNo ";
sqlText += " SELECT @PurchaseQty PurchaseQty,@TotalDDBQty TotalDDBQty ";

              #endregion

              #region SQL Command

              SqlCommand objCommIssueDetail = new SqlCommand();
              objCommIssueDetail.Connection = VcurrConn;

              SqlParameter parameter = new SqlParameter("@PurchaseInvoiceNo", SqlDbType.VarChar, 20);
              parameter.Value = PurchaseInvoiceNo;
              objCommIssueDetail.Parameters.Add(parameter);
              parameter = new SqlParameter("@PurItemNo", SqlDbType.VarChar, 20);
              parameter.Value = PurItemNo;
              objCommIssueDetail.Parameters.Add(parameter);

              objCommIssueDetail.CommandText = sqlText;
              objCommIssueDetail.CommandType = CommandType.Text;

              #endregion

             

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommIssueDetail);
              dataAdapter.Fill(dataTable);
          }
          #endregion

          #region Catch & Finally
          catch (SqlException sqlex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
              //////throw sqlex;

              FileLogger.Log("DutyDrawBackDAL", "Purchase_DDBQty", sqlex.ToString() + "\n" + sqlText);

              throw new ArgumentNullException("", sqlex.Message.ToString());

          }
          catch (Exception ex)
          {
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "Purchase_DDBQty", ex.ToString() + "\n" + sqlText);

              throw new ArgumentNullException("", ex.Message.ToString());

          }
          finally
          {
              if (VcurrConn != null)
              {
                  if (VcurrConn.State == ConnectionState.Open)
                  {
                      VcurrConn.Close();
                  }
              }
          }

          #endregion

          return dataTable;
      }
      public DataTable VAT7_1(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null)
      {
          #region Variables

          SqlConnection currConn = null;
          int transResult = 0;
          int countId = 0;
          string sqlText = "";
          DataTable dataSet = new DataTable("ReportVATDDB");
          //DataTable dataTable = new DataTable("ReportVAT16");

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

              #region Backup



              //top = "Go";

              #endregion Backup

              #region Backup

              string top;


              sqlText = @"
 SELECT 'Purchase'RType
, sh.PurchaseInvoiceNo,sh.BENumber,sh.InvoiceDateTime ,pf.ProductName ProductDescription,pf.HSCodeNo,sd.Quantity,sd.UOM 
FROM DutyDrawBackDetails dd
left outer join Products pf on pf.ItemNo=dd.FgItemNo
left outer join DutyDrawBackHeader dh on dh.DDBackNo=dd.DDBackNo
left outer join PurchaseInvoiceDetails sd on sd.ItemNo=dd.ItemNo
left outer join PurchaseInvoiceHeaders sh on sd.PurchaseInvoiceNo=sh.PurchaseInvoiceNo
where dd.DDBackNo ='DDB-001/0002/1119' and sd.PurchaseInvoiceNo in(select PurchaseInvoiceNo from PurchaseInvoiceDetails where PurchaseInvoiceNo in(
select PurchaseInvoiceNo from  DutyDrawBackDetails where DDBackNo='DDB-001/0002/1119'))

union all
SELECT 'Sale'RType, sh.SalesInvoiceNo,sh.EXPFormNo,sh.EXPFormDate,pr.ProductName ProductDescription,pr.HSCodeNo,sd.Quantity,sd.UOM 
FROM DutyDrawBackDetails dd
left outer join Products pr on pr.ItemNo=dd.ItemNo 
left outer join DutyDrawBackHeader dh on dh.DDBackNo=dd.DDBackNo
left outer join SalesInvoiceDetails sd on sd.ItemNo=dd.FgItemNo
left outer join SalesInvoiceHeaders sh on sd.SalesInvoiceNo=sh.SalesInvoiceNo  ";
              sqlText += @" where dd.DDBackNo =@ddbackno and sd.SalesInvoiceNo in(' @salesInvoice ')  ";


              top = "Go";

              #endregion Backup

              #endregion

              #region SQL Command

              SqlCommand objCommVAT16 = new SqlCommand();
              objCommVAT16.Connection = currConn;

              objCommVAT16.CommandText = sqlText;
              objCommVAT16.CommandType = CommandType.Text;

              #endregion

              #region Parameter

              objCommVAT16.CommandText = sqlText;
              objCommVAT16.CommandType = CommandType.Text;

              if (!objCommVAT16.Parameters.Contains("@ddbackno"))
              {
                  objCommVAT16.Parameters.AddWithValue("@ddbackno", ddbackno);
              }
              else
              {
                  objCommVAT16.Parameters["@ddbackno"].Value = ddbackno;
              }
              if (!objCommVAT16.Parameters.Contains("@salesInvoice"))
              {
                  objCommVAT16.Parameters.AddWithValue("@salesInvoice", salesInvoice);
              }
              else
              {
                  objCommVAT16.Parameters["@salesInvoice"].Value = salesInvoice;
              }


              #endregion Parameter

              SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT16);
              dataAdapter.Fill(dataSet);

          }
          #endregion

          #region Catch & Finally

          catch (SqlException sqlex)
          {
              FileLogger.Log("DutyDrawBackDAL", "VAT7_1", sqlex.ToString() + "\n" + sqlText);

              throw sqlex;
          }
          catch (Exception ex)
          {
              FileLogger.Log("DutyDrawBackDAL", "VAT7_1", ex.ToString() + "\n" + sqlText);

              throw ex;
          }
          finally
          {

              if (currConn.State == ConnectionState.Open)
              {
                  currConn.Close();
              }

          }

          #endregion

          return dataSet;
      }

      #region need to parameterize
      public string[] DutyDrawBacknsert(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null)
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

          string newID = "";
          string PostStatus = "";

          int IDExist = 0;


          #endregion Initializ

          #region try

          try
          {
              #region Validation for Header


              if (Master == null)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgNoDataToSave);
              }
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, "Please Check Issue Data and Time");

              }


              #endregion Validation for Header

              #region open connection and transaction

              currConn = _dbsqlConnection.GetConnection(connVM);
              if (currConn.State != ConnectionState.Open)
              {
                  currConn.Open();
              }
              transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameInsert);

              #endregion open connection and transaction

              #region Fiscal Year Check

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
              if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
              {

                  #region YearLock
                  sqlText = "";

                  sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                 " where @transactionYearCheck between PeriodStart and PeriodEnd";

                  DataTable dataTable = new DataTable("ProductDataT");
                  SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                  cmdIdExist.Transaction = transaction;

                  SqlParameter parameter = new SqlParameter("@transactionYearCheck", SqlDbType.VarChar, 20);
                  parameter.Value = transactionYearCheck;
                  cmdIdExist.Parameters.Add(parameter);

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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@DDBackNo ";
              SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn);
              cmdExistTran.Transaction = transaction;
              SqlParameter param = new SqlParameter("@DDBackNo", SqlDbType.VarChar, 30);
              param.Value = Master.DDBackNo;
              cmdExistTran.Parameters.Add(param);
              IDExist = (int)cmdExistTran.ExecuteScalar();

              if (IDExist > 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
              }

              #endregion Find Transaction Exist

              #region  ID Create For Other

              CommonDAL commonDal = new CommonDAL();
              if (Master.TransactionType.Trim().ToLower() == "ddb")
              {

                  newID = commonDal.TransactionCode("DDB", "DDB", "DutyDrawBackHeader", "DDBackNo",
                                               "DDBackDate", Master.DDBackDate, Master.BranchId.ToString(), currConn, transaction);
              }
              else if (Master.TransactionType.Trim().ToLower() == "vdb")
              {
                  newID = commonDal.TransactionCode("VDB", "VDB", "DutyDrawBackHeader", "DDBackNo",
                                              "DDBackDate", Master.DDBackDate, Master.BranchId.ToString(), currConn, transaction);
              }
              else if (Master.TransactionType.Trim().ToLower() == "sdb")
              {
                  newID = commonDal.TransactionCode("SDB", "SDB", "DutyDrawBackHeader", "DDBackNo",
                                             "DDBackDate", Master.DDBackDate, Master.BranchId.ToString(), currConn, transaction);

              }

              Master.DDBackNo = newID;

              #endregion Purchase ID Create For Other

              #region ID generated completed,Insert new Information in Header


              sqlText = "";
              sqlText += " insert into DutyDrawBackHeader(";
              sqlText += " DDBackNo,";
              sqlText += " DDBackDate,";
              sqlText += " SalesInvoiceNo,";
              sqlText += " SalesDate,";
              sqlText += " CustormerID,";
              sqlText += " CurrencyId,";
              sqlText += " ExpCurrency,";
              sqlText += " BDTCurrency,";
              sqlText += " FgItemNo,";
              sqlText += " TotalClaimCD,";
              sqlText += " TotalClaimRD,";
              sqlText += " TotalClaimSD,";
              sqlText += " TotalDDBack,";
              sqlText += " TotalClaimVAT,";
              sqlText += " TotalClaimCnFAmount,";
              sqlText += " TotalClaimInsuranceAmount,";
              sqlText += " TotalClaimTVBAmount,";
              sqlText += " TotalClaimTVAAmount,";
              sqlText += " TotalClaimATVAmount,";
              sqlText += " TotalClaimOthersAmount,";
              sqlText += " ApprovedSD,";
              sqlText += " TotalSDAmount,";
              sqlText += " Comments,";
              sqlText += " CreatedBy,";
              sqlText += " CreatedOn,";
              sqlText += " LastModifiedBy,";
              sqlText += " LastModifiedOn,";
              sqlText += " BranchId,";
              sqlText += " TransactionType,";

              sqlText += " Post";
              sqlText += " )";

              sqlText += " values";
              sqlText += " (";
              sqlText += "@newID,";
              sqlText += "@MasterDDBackDate,";
              sqlText += "@MasterSalesInvoiceNo,";
              sqlText += "@MasterSalesDate,";
              sqlText += "@MasterCustormerID,";
              sqlText += "@MasterCurrencyId,";
              sqlText += "@MasterExpCurrency,";
              sqlText += "@MasterBDTCurrency,";
              sqlText += "@MasterFgItemNo,";
              sqlText += "@MasterTotalClaimCD,";
              sqlText += "@MasterTotalClaimRD,";
              sqlText += "@MasterTotalClaimSD,";
              sqlText += "@MasterTotalDDBack,";
              sqlText += "@MasterTotalClaimSD,";
              sqlText += "@MasterTotalClaimVAT,";
              sqlText += "@MasterTotalClaimCnFAmount,";
              sqlText += "@MasterTotalClaimInsuranceAmount,";
              sqlText += "@MasterTotalClaimTVBAmount,";
              sqlText += "@MasterTotalClaimTVAAmount,";
              sqlText += "@MasterTotalClaimOthersAmount,";
              sqlText += "@ApprovedSD,";
              sqlText += "@TotalSDAmount,";
              sqlText += "@MasterComments,";
              sqlText += "@MasterCreatedBy,";
              sqlText += "@MasterCreatedOn,";
              sqlText += "@MasterLastModifiedBy,";
              sqlText += "@MasterLastModifiedOn,";
              sqlText += "@MasterBranchId,";
              sqlText += "@TransactionType,";
              sqlText += "@MasterPost";
              sqlText += ")";


              SqlCommand cmdInsert = new SqlCommand(sqlText, currConn);
              cmdInsert.Transaction = transaction;
              cmdInsert.Parameters.AddWithValueAndNullHandle("@newID", newID);

              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterDDBackDate", Master.DDBackDate);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterSalesDate", Master.SalesDate);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCustormerID", Master.CustormerID);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCurrencyId", Master.CurrencyId);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterExpCurrency", Master.ExpCurrency);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBDTCurrency", Master.BDTCurrency);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterFgItemNo", Master.FgItemNo);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimCD", Master.TotalClaimCD);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimRD", Master.TotalClaimRD);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimSD", Master.TotalClaimSD);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalDDBack", Master.TotalDDBack);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimVAT", Master.TotalClaimVAT);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimCnFAmount", Master.TotalClaimCnFAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimInsuranceAmount", Master.TotalClaimInsuranceAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimTVBAmount", Master.TotalClaimTVBAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimTVAAmount", Master.TotalClaimTVAAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimOthersAmount", Master.TotalClaimOthersAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@ApprovedSD", Master.ApprovedSD);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterBranchId", Master.BranchId);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
              cmdInsert.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);

              transResult = (int)cmdInsert.ExecuteNonQuery();
              if (transResult <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
              }


              #endregion ID generated completed,Insert new Information in Header

              #region if Transection not Other Insert Issue /Receive



              #endregion if Transection not Other Insert Issue /Receive

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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo=@newID ";
                  sqlText += " AND ItemNo=@ItemItemNo";
                  sqlText += " AND FgItemNo=@ItemFgItemNo";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;
                  cmdFindId.Parameters.AddWithValue("@newID", newID);
                  cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindId.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist > 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                  }

                  #endregion Find Transaction Exist

                  #region Insert only DetailTable

                  sqlText = "";
                  sqlText += " insert into DutyDrawBackDetails(";
                  sqlText += " DDBackNo,";
                  sqlText += " DDBackDate,";
                  sqlText += " DDLineNo,";
                  sqlText += " PurchaseInvoiceNo,";
                  sqlText += " PurchaseDate,";
                  sqlText += " FgItemNo,";
                  sqlText += " FGQty,";
                  sqlText += " ItemNo,";
                  sqlText += " BillOfEntry,";
                  sqlText += " PurchaseUom,";
                  sqlText += " PurchaseQuantity,";
                  sqlText += " UnitPrice,";
                  sqlText += " AV,";
                  sqlText += " CD,";
                  sqlText += " RD,";
                  sqlText += " SD,";
                  sqlText += " VAT,";
                  sqlText += " CnF,";
                  sqlText += " Insurance,";
                  sqlText += " TVB,";
                  sqlText += " TVA,";
                  sqlText += " ATV,";
                  sqlText += " Others,";
                  sqlText += " UseQuantity,";
                  sqlText += " ClaimCD,";
                  sqlText += " ClaimRD,";
                  sqlText += " ClaimSD,";
                  sqlText += " ClaimVAT,";
                  sqlText += " ClaimCnF,";
                  sqlText += " ClaimInsurance,";
                  sqlText += " ClaimTVB,";
                  sqlText += " ClaimTVA,";
                  sqlText += " ClaimATV,";
                  sqlText += " ClaimOthers,";
                  sqlText += " SubTotalDDB,";
                  sqlText += " UOMc,";
                  sqlText += " UOMn,";
                  sqlText += " UOMCD,";
                  sqlText += " UOMRD,";
                  sqlText += " UOMSD,";
                  sqlText += " UOMVAT,";
                  sqlText += " UOMCnF,";
                  sqlText += " UOMInsurance,";
                  sqlText += " UOMTVB,";
                  sqlText += " UOMTVA,";
                  sqlText += " UOMATV,";
                  sqlText += " UOMOthers,";
                  sqlText += " UOMSubTotalDDB,";
                  sqlText += " Post,";
                  sqlText += " CreatedBy,";
                  sqlText += " CreatedOn,";
                  sqlText += " LastModifiedBy,";
                  sqlText += " LastModifiedOn,";
                  sqlText += " PurchasetransactionType,";
                  sqlText += " BranchId,";
                  sqlText += " TransactionType,";
                  sqlText += " SalesInvoiceNo";
                  sqlText += " )";

                  sqlText += " values(	";
                  //sqlText += "'" + Master.Id + "',";

                  sqlText += "'" + newID + "',";
                  sqlText += "@ItemDDBackDate,";
                  sqlText += "@ItemDDLineNo,";
                  sqlText += "@ItemPurchaseInvoiceNo,";
                  sqlText += "@ItemPurchaseDate,";
                  sqlText += "@ItemFgItemNo,";
                  sqlText += "@ItemFGQty,";
                  sqlText += "@ItemItemNo,";
                  sqlText += "@ItemBillOfEntry,";
                  sqlText += "@ItemPurchaseUom,";
                  sqlText += "@ItemPurchaseQuantity,";
                  sqlText += "@ItemUnitPrice,";
                  sqlText += "@ItemAV,";
                  sqlText += "@ItemCD,";
                  sqlText += "@ItemRD,";
                  sqlText += "@ItemSD,";
                  sqlText += "@ItemVAT,";
                  sqlText += "@ItemCnF,";
                  sqlText += "@ItemInsurance,";
                  sqlText += "@ItemTVB,";
                  sqlText += "@ItemTVA,";
                  sqlText += "@ItemATV,";
                  sqlText += "@ItemOthers,";
                  sqlText += "@ItemUseQuantity,";
                  sqlText += "@ItemClaimCD,";
                  sqlText += "@ItemClaimRD,";
                  sqlText += "@ItemClaimSD,";
                  sqlText += "@ItemClaimVAT,";
                  sqlText += "@ItemClaimCnF,";
                  sqlText += "@ItemClaimInsurance,";
                  sqlText += "@ItemClaimTVB,";
                  sqlText += "@ItemClaimTVA,";
                  sqlText += "@ItemClaimATV,";
                  sqlText += "@ItemClaimOthers,";
                  sqlText += "@ItemSubTotalDDB,";
                  sqlText += "@ItemUOMc,";
                  sqlText += "@ItemUOMn,";
                  sqlText += "@ItemUOMCD,";
                  sqlText += "@ItemUOMRD,";
                  sqlText += "@ItemUOMSD,";
                  sqlText += "@ItemUOMVAT,";
                  sqlText += "@ItemUOMCnF,";
                  sqlText += "@ItemUOMInsurance,";
                  sqlText += "@ItemUOMTVB,";
                  sqlText += "@ItemUOMTVA,";
                  sqlText += "@ItemUOMATV,";
                  sqlText += "@ItemUOMOthers,";
                  sqlText += "@ItemUOMSubTotalDDB,";
                  sqlText += "@ItemPost,";
                  sqlText += "@ItemCreatedBy,";
                  sqlText += "@ItemCreatedOn,";
                  sqlText += "@ItemLastModifiedBy,";
                  sqlText += "@ItemLastModifiedOn,";
                  sqlText += "@ItemPTransType,";
                  sqlText += "@BranchId,";
                  sqlText += "@TransactionType,";
                  sqlText += "@ItemSalesInvoiceNo";
                  sqlText += ")	";


                  SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                  cmdInsDetail.Transaction = transaction;

                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDBackDate", Item.DDBackDate);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDLineNo", Item.DDLineNo);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseDate", OrdinaryVATDesktop.DateToDate(Item.PurchaseDate));
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFgItemNo", Item.FgItemNo);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFGQty", Item.FGQty);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillOfEntry", Item.BillOfEntry);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseUom", Item.PurchaseUom);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseQuantity", Item.PurchaseQuantity);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUnitPrice", Item.UnitPrice);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemAV", Item.AV);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCD", Item.CD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRD", Item.RD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVAT", Item.VAT);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCnF", Item.CnF);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemInsurance", Item.Insurance);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVB", Item.TVB);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVA", Item.TVA);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemATV", Item.ATV);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemOthers", Item.Others);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUseQuantity", Item.UseQuantity);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCD", Item.ClaimCD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimRD", Item.ClaimRD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimSD", Item.ClaimSD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimVAT", Item.ClaimVAT);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCnF", Item.ClaimCnF);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimInsurance", Item.ClaimInsurance);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVB", Item.ClaimTVB);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVA", Item.ClaimTVA);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimATV", Item.ClaimATV);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimOthers", Item.ClaimOthers);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotalDDB", Item.SubTotalDDB);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCD", Item.UOMCD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMRD", Item.UOMRD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSD", Item.UOMSD);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMVAT", Item.UOMVAT);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCnF", Item.UOMCnF);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMInsurance", Item.UOMInsurance);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVB", Item.UOMTVB);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVA", Item.UOMTVA);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMATV", Item.UOMATV);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMOthers", Item.UOMOthers);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSubTotalDDB", Item.UOMSubTotalDDB);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", Item.CreatedOn);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPTransType", Item.PTransType);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                  cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                  transResult = (int)cmdInsDetail.ExecuteNonQuery();

                  if (transResult <= 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                  }
                  #endregion Insert only DetailTable
              }
              foreach (var Item in ddbSaleInvoices.ToList())
              {
                  #region Find Transaction Exist

                  sqlText = "";
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackSaleInvoices WHERE DDBackNo=@newID ";
                  sqlText += " AND SalesInvoiceNo=@ItemSalesInvoiceNo";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;
                  cmdFindId.Parameters.AddWithValue("@newID", newID);
                  cmdFindId.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist > 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgFindExistID);
                  }

                  #endregion Find Transaction Exist

                  #region Insert only DetailTable

                  sqlText = "";
                  sqlText += " insert into DutyDrawBackSaleInvoices(";
                  sqlText += " DDBackNo,";
                  sqlText += " SL,";
                  sqlText += " SalesDate,";
                  sqlText += " BranchId,";
                  sqlText += " SalesInvoiceNo";
                  sqlText += " )";

                  sqlText += " values(	";
                  //sqlText += "'" + Master.Id + "',";

                  sqlText += "@newID,";
                  sqlText += "@ItemSL,";
                  sqlText += "@ItemSalesDate,";
                  sqlText += "@BranchId,";
                  sqlText += "@ItemSalesInvoiceNo";
                  sqlText += ")	";


                  SqlCommand cmdInssaleInv = new SqlCommand(sqlText, currConn);
                  cmdInssaleInv.Transaction = transaction;
                  cmdInssaleInv.Parameters.AddWithValue("@newID", newID);
                  cmdInssaleInv.Parameters.AddWithValue("@ItemSL", Item.SL);
                  cmdInssaleInv.Parameters.AddWithValue("@ItemSalesDate", OrdinaryVATDesktop.DateToDate(Item.SalesDate));
                  cmdInssaleInv.Parameters.AddWithValue("@BranchId", Item.BranchId);
                  cmdInssaleInv.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                  transResult = (int)cmdInssaleInv.ExecuteNonQuery();

                  if (transResult <= 0)
                  {
                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgSaveNotSuccessfully);
                  }
                  #endregion Insert only DetailTable
              }

              #endregion Insert Detail Table
              #endregion Insert into Details(Insert complete in Header)

              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct  Post from dbo.DutyDrawBackHeader WHERE DDBackNo=@newID";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;
              cmdIPS.Parameters.AddWithValue("@newID", newID);
              PostStatus = (string)cmdIPS.ExecuteScalar();
              if (string.IsNullOrEmpty(PostStatus))
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameInsert, MessageVM.issueMsgUnableCreatID);
              }


              #endregion Prefetch


              #region Update PeriodId

              sqlText = "";
              sqlText += @"

UPDATE DutyDrawBackHeader 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DDBackDate)) +  CONVERT(VARCHAR(4),YEAR(DDBackDate)),6)
WHERE DDBackNo = @DDBackNo


UPDATE DutyDrawBackDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DDBackDate)) +  CONVERT(VARCHAR(4),YEAR(DDBackDate)),6)
WHERE DDBackNo = @DDBackNo

";

              SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
              cmdUpdate.Parameters.AddWithValue("@DDBackNo", Master.DDBackNo);

              transResult = cmdUpdate.ExecuteNonQuery();

              #endregion


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
              retResults[1] = MessageVM.issueMsgSaveSuccessfully;
              retResults[2] = "" + newID;
              retResults[3] = "" + PostStatus;
              #endregion SuccessResult

          }
          #endregion Try

          #region Catch and Finall
          //catch (SqlException sqlex)
          //{
          //    transaction.Rollback();
          //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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
              transaction.Rollback();
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "DutyDrawBacknsert", ex.ToString() + "\n" + sqlText);

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
      public string[] DutyDrawBackUpdate(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null)
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
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
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
              transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

              #endregion open connection and transaction

              #region Fiscal Year Check

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
              if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
              {

                  #region YearLock
                  sqlText = "";

                  sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                 " where @transactionYearCheck between PeriodStart and PeriodEnd";

                  DataTable dataTable = new DataTable("ProductDataT");
                  SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                  cmdIdExist.Transaction = transaction;

                  SqlParameter parameter = new SqlParameter("@transactionYearCheck", SqlDbType.VarChar, 20);
                  parameter.Value = transactionYearCheck;
                  cmdIdExist.Parameters.Add(parameter);

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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
              cmdFindIdUpd.Transaction = transaction;
              cmdFindIdUpd.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

              if (IDExist <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
              }

              #endregion Find ID for Update

              #region ID check completed,update Information in Header

              #region update Header
              sqlText = "";
              sqlText += " update DutyDrawBackHeader set  ";
              sqlText += " DDBackDate               = @MasterDDBackDate,";
              sqlText += " SalesInvoiceNo           = @MasterSalesInvoiceNo,";
              sqlText += " SalesDate                = @MasterSalesDate,";
              sqlText += " CustormerID              = @MasterCustormerID,";
              sqlText += " CurrencyId               = @MasterCurrencyId,";
              sqlText += " ExpCurrency              = @MasterExpCurrency,";
              sqlText += " BDTCurrency              = @MasterBDTCurrency,";
              sqlText += " FgItemNo                 = @MasterFgItemNo,";
              sqlText += " TotalClaimCD             = @MasterTotalClaimCD,";
              sqlText += " TotalClaimRD             = @MasterTotalClaimRD,";
              sqlText += " TotalClaimSD             = @MasterTotalClaimSD,";
              sqlText += " TotalDDBack              = @MasterTotalDDBack,";
              sqlText += " TotalClaimVAT            = @MasterTotalClaimVAT,";
              sqlText += " TotalClaimCnFAmount      = @MasterTotalClaimCnFAmount,";
              sqlText += " TotalClaimInsuranceAmount= @MasterTotalClaimInsuranceAmount,";
              sqlText += " TotalClaimTVBAmount      = @MasterTotalClaimTVBAmount,";
              sqlText += " TotalClaimTVAAmount      = @MasterTotalClaimTVAAmount,";
              sqlText += " TotalClaimATVAmount      = @MasterTotalClaimATVAmount,";
              sqlText += " TotalClaimOthersAmount   = @MasterTotalClaimOthersAmount,";
              sqlText += " ApprovedSD               = @ApprovedSD,";
              sqlText += " TotalSDAmount            = @TotalSDAmount,";
              sqlText += " BranchId                 = @BranchId,";
              sqlText += " Comments                 = @MasterComments, ";
              sqlText += " CreatedBy                = @MasterCreatedBy, ";
              sqlText += " CreatedOn                = @MasterCreatedOn, ";
              sqlText += " LastModifiedBy           = @MasterLastModifiedBy ,";
              sqlText += " LastModifiedOn           = @MasterLastModifiedOn, ";
              sqlText += " TransactionType           = @TransactionType, ";
              sqlText += " Post                     = @MasterPost ";
              sqlText += " where  DDBackNo          = @MasterDDBackNo ";


              SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
              cmdUpdate.Transaction = transaction;

              cmdUpdate.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDDBackDate", Master.DDBackDate);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSalesInvoiceNo", Master.SalesInvoiceNo);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterSalesDate", Master.SalesDate);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterCustormerID", Master.CustormerID);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterCurrencyId", Master.CurrencyId);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterExpCurrency", Master.ExpCurrency);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterBDTCurrency", Master.BDTCurrency);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterFgItemNo", Master.FgItemNo);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimCD", Master.TotalClaimCD);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimRD", Master.TotalClaimRD);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimSD", Master.TotalClaimSD);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalDDBack", Master.TotalDDBack);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimVAT", Master.TotalClaimVAT);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimCnFAmount", Master.TotalClaimCnFAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimInsuranceAmount ", Master.TotalClaimInsuranceAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimTVBAmount", Master.TotalClaimTVBAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimTVAAmount", Master.TotalClaimTVAAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimATVAmount", Master.TotalClaimATVAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterTotalClaimOthersAmount", Master.TotalClaimOthersAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@ApprovedSD", Master.ApprovedSD);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@TotalSDAmount", Master.TotalSDAmount);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@BranchId", Master.BranchId);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterComments", Master.Comments);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterCreatedBy", Master.CreatedBy);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterCreatedOn", Master.CreatedOn);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);

              transResult = (int)cmdUpdate.ExecuteNonQuery();
              if (transResult <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
              }
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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo=@MasterDDBackNo ";
                  sqlText += " AND ItemNo=@ItemItemNo";
                  sqlText += " AND FgItemNo=@ItemFgItemNo";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;

                  cmdFindId.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
                  cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindId.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist <= 0)
                  {
                      // Insert
                      #region Insert only DetailTable

                      sqlText = "";
                      sqlText += " insert into DutyDrawBackDetails(";
                      sqlText += " DDBackNo,";
                      sqlText += " DDBackDate,";
                      sqlText += " DDLineNo,";
                      sqlText += " PurchaseInvoiceNo,";
                      sqlText += " PurchaseDate,";
                      sqlText += " FgItemNo,";
                      sqlText += " ItemNo,";
                      sqlText += " BillOfEntry,";
                      sqlText += " PurchaseUom,";
                      sqlText += " PurchaseQuantity,";
                      sqlText += " UnitPrice,";
                      sqlText += " AV,";
                      sqlText += " CD,";
                      sqlText += " RD,";
                      sqlText += " SD,";
                      sqlText += " VAT,";
                      sqlText += " CnF,";
                      sqlText += " Insurance,";
                      sqlText += " TVB,";
                      sqlText += " TVA,";
                      sqlText += " ATV,";
                      sqlText += " Others,";
                      sqlText += " UseQuantity,";
                      sqlText += " ClaimCD,";
                      sqlText += " ClaimRD,";
                      sqlText += " ClaimSD,";
                      sqlText += " ClaimVAT,";
                      sqlText += " ClaimCnF,";
                      sqlText += " ClaimInsurance,";
                      sqlText += " ClaimTVB,";
                      sqlText += " ClaimTVA,";
                      sqlText += " ClaimATV,";
                      sqlText += " ClaimOthers,";
                      sqlText += " SubTotalDDB,";
                      sqlText += " UOMc,";
                      sqlText += " UOMn,";
                      sqlText += " UOMCD,";
                      sqlText += " UOMRD,";
                      sqlText += " UOMSD,";
                      sqlText += " UOMVAT,";
                      sqlText += " UOMCnF,";
                      sqlText += " UOMInsurance,";
                      sqlText += " UOMTVB,";
                      sqlText += " UOMTVA,";
                      sqlText += " UOMATV,";
                      sqlText += " UOMOthers,";
                      sqlText += " UOMSubTotalDDB,";
                      sqlText += " BranchId,";
                      sqlText += " Post,";
                      sqlText += " CreatedBy,";
                      sqlText += " CreatedOn,";
                      sqlText += " LastModifiedBy,";
                      sqlText += " SalesInvoiceNo,";
                      sqlText += " FGQty,";
                      sqlText += " PurchasetransactionType,";
                      sqlText += " TransactionType,";
                      sqlText += " LastModifiedOn";
                      sqlText += " )";

                      sqlText += " values(	";
                      //sqlText += "'" + Master.Id + "',";

                      sqlText += "@MasterDDBackNo,";
                      sqlText += "@ItemDDBackDate,";
                      sqlText += "@ItemDDLineNo,";
                      sqlText += "@ItemPurchaseInvoiceNo,";
                      sqlText += "@ItemPurchaseDate,";
                      sqlText += "@ItemFgItemNo,";
                      sqlText += "@ItemItemNo,";
                      sqlText += "@ItemBillOfEntry,";
                      sqlText += "@ItemPurchaseUom,";
                      sqlText += "@ItemPurchaseQuantity,";
                      sqlText += "@ItemUnitPrice,";
                      sqlText += "@ItemAV,";
                      sqlText += "@ItemCD,";
                      sqlText += "@ItemRD,";
                      sqlText += "@ItemSD,";
                      sqlText += "@ItemVAT,";
                      sqlText += "@ItemCnF,";
                      sqlText += "@ItemInsurance,";
                      sqlText += "@ItemTVB,";
                      sqlText += "@ItemTVA,";
                      sqlText += "@ItemATV,";
                      sqlText += "@ItemOthers,";
                      sqlText += "@ItemUseQuantity,";
                      sqlText += "@ItemClaimCD,";
                      sqlText += "@ItemClaimRD,";
                      sqlText += "@ItemClaimSD,";
                      sqlText += "@ItemClaimVAT,";
                      sqlText += "@ItemClaimCnF,";
                      sqlText += "@ItemClaimInsurance,";
                      sqlText += "@ItemClaimTVB,";
                      sqlText += "@ItemClaimTVA,";
                      sqlText += "@ItemClaimATV,";
                      sqlText += "@ItemClaimOthers,";
                      sqlText += "@ItemSubTotalDDB,";
                      sqlText += "@ItemUOMc,";
                      sqlText += "@ItemUOMn,";
                      sqlText += "@ItemUOMCD,";
                      sqlText += "@ItemUOMRD,";
                      sqlText += "@ItemUOMSD,";
                      sqlText += "@ItemUOMVAT,";
                      sqlText += "@ItemUOMCnF,";
                      sqlText += "@ItemUOMInsurance,";
                      sqlText += "@ItemUOMTVB,";
                      sqlText += "@ItemUOMTVA,";
                      sqlText += "@ItemUOMATV,";
                      sqlText += "@ItemUOMOthers,";
                      sqlText += "@ItemUOMSubTotalDDB,";
                      sqlText += "@BranchId,";
                      sqlText += "@ItemPost,";
                      sqlText += "@ItemCreatedBy,";
                      sqlText += "@ItemCreatedOn,";
                      sqlText += "@ItemLastModifiedBy,";
                      sqlText += "@ItemSalesInvoiceNo,";
                      sqlText += "@ItemFGQty,";
                      sqlText += "@ItemPTransType,";
                      sqlText += "@TransactionType,";
                      sqlText += "@ItemLastModifiedOn";
                      sqlText += ")	";




                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDBackDate", Item.DDBackDate);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDLineNo", Item.DDLineNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseDate", Item.PurchaseDate);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFgItemNo", Item.FgItemNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillOfEntry", Item.BillOfEntry);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseUom", Item.PurchaseUom);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseQuantity", Item.PurchaseQuantity);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUnitPrice", Item.UnitPrice);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemAV", Item.AV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCD", Item.CD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRD", Item.RD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVAT", Item.VAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCnF", Item.CnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemInsurance", Item.Insurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVB", Item.TVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVA", Item.TVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemATV", Item.ATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemOthers", Item.Others);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUseQuantity", Item.UseQuantity);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCD", Item.ClaimCD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimRD", Item.ClaimRD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimSD", Item.ClaimSD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimVAT", Item.ClaimVAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCnF", Item.ClaimCnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimInsurance", Item.ClaimInsurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVB", Item.ClaimTVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVA", Item.ClaimTVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimATV", Item.ClaimATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimOthers", Item.ClaimOthers);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotalDDB", Item.SubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCD", Item.UOMCD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMRD", Item.UOMRD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSD", Item.UOMSD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMVAT", Item.UOMVAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCnF", Item.UOMCnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMInsurance", Item.UOMInsurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVB", Item.UOMTVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVA", Item.UOMTVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMATV", Item.UOMATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMOthers", Item.UOMOthers);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSubTotalDDB", Item.UOMSubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", Item.CreatedOn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFGQty", Item.FGQty);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPTransType", Item.PTransType);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Insert only DetailTable

                  }
                  else
                  {
                      //Update

                      #region Update only DetailTable

                      sqlText = "";
                      sqlText += " update DutyDrawBackDetails set ";

                      sqlText += " DDBackDate       =@ItemDDBackDate,";
                      sqlText += " DDLineNo         =@ItemDDLineNo,";
                      sqlText += " PurchaseInvoiceNo=@ItemPurchaseInvoiceNo,";
                      sqlText += " PurchaseDate     =@ItemPurchaseDate,";
                      sqlText += " FgItemNo         =@ItemFgItemNo,";
                      sqlText += " ItemNo           =@ItemItemNo,";
                      sqlText += " BillOfEntry      =@ItemBillOfEntry,";
                      sqlText += " PurchaseUom      =@ItemPurchaseUom,";
                      sqlText += " PurchaseQuantity =@ItemPurchaseQuantity,";
                      sqlText += " UnitPrice        =@ItemUnitPrice,";
                      sqlText += " AV               =@ItemAV,";
                      sqlText += " CD               =@ItemCD,";
                      sqlText += " RD               =@ItemRD,";
                      sqlText += " SD               =@ItemSD,";
                      sqlText += " VAT              =@ItemVAT,";
                      sqlText += " CnF              =@ItemCnF,";
                      sqlText += " Insurance        =@ItemInsurance,";
                      sqlText += " TVB              =@ItemTVB,";
                      sqlText += " TVA              =@ItemTVA,";
                      sqlText += " ATV              =@ItemATV,";
                      sqlText += " Others           =@ItemOthers,";
                      sqlText += " UseQuantity      =@ItemUseQuantity,";
                      sqlText += " ClaimCD          =@ItemClaimCD,";
                      sqlText += " ClaimRD          =@ItemClaimRD,";
                      sqlText += " ClaimSD          =@ItemClaimSD,";
                      sqlText += " ClaimVAT         =@ItemClaimVAT,";
                      sqlText += " ClaimCnF         =@ItemClaimCnF,";
                      sqlText += " ClaimInsurance   =@ItemClaimInsurance,";
                      sqlText += " ClaimTVB         =@ItemClaimTVB,";
                      sqlText += " ClaimTVA         =@ItemClaimTVA,";
                      sqlText += " ClaimATV         =@ItemClaimATV,";
                      sqlText += " ClaimOthers      =@ItemClaimOthers,";
                      sqlText += " SubTotalDDB      =@ItemSubTotalDDB,";
                      sqlText += " UOMc             =@ItemUOMc,";
                      sqlText += " UOMn             =@ItemUOMn,";
                      sqlText += " UOMCD            =@ItemUOMCD,";
                      sqlText += " UOMRD            =@ItemUOMRD,";
                      sqlText += " UOMSD            =@ItemUOMSD,";
                      sqlText += " UOMVAT           =@ItemUOMVAT,";
                      sqlText += " UOMCnF           =@ItemUOMCnF,";
                      sqlText += " UOMInsurance     =@ItemUOMInsurance,";
                      sqlText += " UOMTVB           =@ItemUOMTVB,";
                      sqlText += " UOMTVA           =@ItemUOMTVA,";
                      sqlText += " UOMATV           =@ItemUOMATV,";
                      sqlText += " UOMOthers        =@ItemUOMOthers,";
                      sqlText += " UOMSubTotalDDB   =@ItemUOMSubTotalDDB,";
                      sqlText += " BranchId         =@BranchId,";
                      sqlText += " Post             =@ItemPost,";
                      sqlText += " CreatedBy        =@ItemCreatedBy,";
                      sqlText += " CreatedOn        =@ItemCreatedOn,";
                      sqlText += " LastModifiedBy   =@ItemLastModifiedBy,";
                      sqlText += " LastModifiedOn   =@ItemLastModifiedOn,";
                      sqlText += " SalesInvoiceNo   =@ItemSalesInvoiceNo,";
                      sqlText += " TransactionType   =@TransactionType,";
                      sqlText += " FGQty            =@ItemFGQty";
                      sqlText += " where  DDBackNo  =@MasterDDBackNo ";
                      sqlText += " and ItemNo       =@ItemItemNo";
                      sqlText += " AND FgItemNo     =@ItemFgItemNo";



                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDBackDate", Item.DDBackDate);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@TransactionType", Master.TransactionType);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemDDLineNo", Item.DDLineNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseInvoiceNo", Item.PurchaseInvoiceNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseDate", OrdinaryVATDesktop.DateToDate(Item.PurchaseDate));
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFgItemNo", Item.FgItemNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemBillOfEntry", Item.BillOfEntry);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseUom", Item.PurchaseUom);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPurchaseQuantity", Item.PurchaseQuantity);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUnitPrice", Item.UnitPrice);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemAV", Item.AV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCD", Item.CD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemRD", Item.RD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSD", Item.SD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemVAT", Item.VAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCnF", Item.CnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemInsurance", Item.Insurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVB", Item.TVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemTVA", Item.TVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemATV", Item.ATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemOthers", Item.Others);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUseQuantity", Item.UseQuantity);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCD", Item.ClaimCD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimRD", Item.ClaimRD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimSD", Item.ClaimSD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimVAT", Item.ClaimVAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimCnF", Item.ClaimCnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimInsurance", Item.ClaimInsurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVB", Item.ClaimTVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimTVA", Item.ClaimTVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimATV", Item.ClaimATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemClaimOthers", Item.ClaimOthers);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSubTotalDDB", Item.SubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMc", Item.UOMc);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMn", Item.UOMn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCD", Item.UOMCD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMRD", Item.UOMRD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSD", Item.UOMSD);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMVAT", Item.UOMVAT);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMCnF", Item.UOMCnF);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMInsurance", Item.UOMInsurance);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVB", Item.UOMTVB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMTVA", Item.UOMTVA);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMATV", Item.UOMATV);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMOthers", Item.UOMOthers);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemUOMSubTotalDDB", Item.UOMSubTotalDDB);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", Item.CreatedOn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFGQty", Item.FGQty);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Update only DetailTable

                  }

                  #endregion Find Transaction Mode Insert or Update
              }
              foreach (var Item in ddbSaleInvoices.ToList())
              {
                  #region Find Transaction Mode Insert or Update

                  sqlText = "";
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackSaleInvoices WHERE DDBackNo=@MasterDDBackNo ";
                  sqlText += " AND SalesInvoiceNo=@ItemSalesInvoiceNo";
                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;

                  cmdFindId.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
                  cmdFindId.Parameters.AddWithValue("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist <= 0)
                  {
                      // Insert
                      #region Insert only DetailTable

                      sqlText = "";
                      sqlText += " insert into DutyDrawBackSaleInvoices(";
                      sqlText += " DDBackNo,";
                      sqlText += " SL,";
                      sqlText += " SalesDate,";
                      sqlText += " SalesInvoiceNo";
                      sqlText += " )";

                      sqlText += " values(	";
                      sqlText += "@MasterDDBackNo,";
                      sqlText += "@ItemSL,";
                      sqlText += "@ItemSalesDate,";
                      sqlText += "@ItemSalesInvoiceNo";
                      sqlText += ")	";
                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSL", Item.SL);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesDate", Item.SalesDate);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Insert only DetailTable
                  }
                  else
                  {
                      //Update

                      #region Update only DetailTable

                      sqlText = "";
                      sqlText += " update DutyDrawBackSaleInvoices set ";

                      sqlText += " SL=@ItemSL,";
                      sqlText += " SalesDate=@ItemSalesDate";
                      //sqlText += " SalesInvoiceNo=@ItemSalesInvoiceNo";
                      sqlText += " where  DDBackNo =@MasterDDBackNo ";
                      sqlText += " and SalesInvoiceNo=@ItemSalesInvoiceNo";

                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSL", Item.SL);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesDate", OrdinaryVATDesktop.DateToDate(Item.SalesDate));
                      //cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemSalesInvoiceNo", Item.SalesInvoiceNo);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Update only DetailTable

                  }

                  #endregion Find Transaction Mode Insert or Update
              }

              #endregion Update Detail Table

              #endregion  Update into Details(Update complete in Header)

              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct Post from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;
              cmdIPS.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
              PostStatus = (string)cmdIPS.ExecuteScalar();
              if (string.IsNullOrEmpty(PostStatus))
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
              }


              #endregion Prefetch

              #region Update PeriodId

              sqlText = "";
              sqlText += @"

UPDATE DutyDrawBackHeader 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DDBackDate)) +  CONVERT(VARCHAR(4),YEAR(DDBackDate)),6)
WHERE DDBackNo = @DDBackNo


UPDATE DutyDrawBackDetails 
SET PeriodId=RIGHT('0'+CONVERT(VARCHAR(2),MONTH(DDBackDate)) +  CONVERT(VARCHAR(4),YEAR(DDBackDate)),6)
WHERE DDBackNo = @DDBackNo

";

              cmdUpdate = new SqlCommand(sqlText, currConn, transaction);
              cmdUpdate.Parameters.AddWithValue("@DDBackNo", Master.DDBackNo);

              transResult = cmdUpdate.ExecuteNonQuery();

              #endregion

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
              retResults[1] = MessageVM.issueMsgUpdateSuccessfully;
              retResults[2] = Master.DDBackNo;
              retResults[3] = PostStatus;
              #endregion SuccessResult

          }
          #endregion Try

          #region Catch and Finall
          //catch (SqlException sqlex)
          //{
          //    transaction.Rollback();
          //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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
              transaction.Rollback();
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;

              FileLogger.Log("DutyDrawBackDAL", "DutyDrawBackUpdate", ex.ToString() + "\n" + sqlText);

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
      public string[] DutyDrawBackPost(DDBHeaderVM Master, List<DDBDetailsVM> Details, SysDBInfoVMTemp connVM = null)
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
              else if (Convert.ToDateTime(Master.DDBackDate) < DateTime.MinValue || Convert.ToDateTime(Master.DDBackDate) > DateTime.MaxValue)
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
              transaction = currConn.BeginTransaction(MessageVM.issueMsgMethodNameUpdate);

              #endregion open connection and transaction
              #region Fiscal Year Check

              string transactionDate = Master.DDBackDate;
              string transactionYearCheck = Convert.ToDateTime(Master.DDBackDate).ToString("yyyy-MM-dd");
              if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue || Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
              {

                  #region YearLock
                  sqlText = "";

                  sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                                                 " where @transactionYearCheck between PeriodStart and PeriodEnd";

                  DataTable dataTable = new DataTable("ProductDataT");
                  SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                  cmdIdExist.Transaction = transaction;
                  SqlParameter parameter = new SqlParameter("@transactionYearCheck", SqlDbType.VarChar, 20);
                  parameter.Value = transactionYearCheck;
                  cmdIdExist.Parameters.Add(parameter);
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
              sqlText = sqlText + "select COUNT(DDBackNo) from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdFindIdUpd = new SqlCommand(sqlText, currConn);
              cmdFindIdUpd.Transaction = transaction;

              cmdFindIdUpd.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              int IDExist = (int)cmdFindIdUpd.ExecuteScalar();

              if (IDExist <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableFindExistID);
              }

              #endregion Find ID for Update



              #region ID check completed,update Information in Header

              #region update Header
              sqlText = "";
              sqlText += " update DutyDrawBackHeader set  ";
              sqlText += " LastModifiedBy= @MasterLastModifiedBy ,";
              sqlText += " LastModifiedOn= @MasterLastModifiedOn, ";
              sqlText += " Post= @MasterPost";

              sqlText += " where  DDBackNo= @MasterDDBackNo ";


              SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
              cmdUpdate.Transaction = transaction;

              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", Master.LastModifiedBy);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", Master.LastModifiedOn);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterPost", Master.Post);
              cmdUpdate.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);

              transResult = (int)cmdUpdate.ExecuteNonQuery();
              if (transResult <= 0)
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
              }
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
                  sqlText += "select COUNT(DDBackNo) from DutyDrawBackDetails WHERE DDBackNo=@MasterDDBackNo  ";
                  sqlText += " AND ItemNo=@ItemItemNo ";
                  sqlText += " AND FgItemNo=@ItemFgItemNo ";

                  SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                  cmdFindId.Transaction = transaction;

                  cmdFindId.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);
                  cmdFindId.Parameters.AddWithValue("@ItemItemNo", Item.ItemNo);
                  cmdFindId.Parameters.AddWithValue("@ItemFgItemNo", Item.FgItemNo);

                  IDExist = (int)cmdFindId.ExecuteScalar();

                  if (IDExist <= 0)
                  {

                      throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);

                  }
                  else
                  {
                      //Update

                      #region Update only DetailTable

                      sqlText = "";
                      sqlText += " update DutyDrawBackDetails set ";
                      sqlText += " Post=@ItemPost,";
                      sqlText += " LastModifiedBy=@ItemLastModifiedBy,";
                      sqlText += " LastModifiedOn=@ItemLastModifiedOn";
                      sqlText += " where  DDBackNo =@MasterDDBackNo  ";
                      sqlText += " and ItemNo=@ItemItemNo ";
                      sqlText += " AND FgItemNo=@ItemFgItemNo";




                      SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                      cmdInsDetail.Transaction = transaction;

                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", Item.LastModifiedOn);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MasterDDBackNo", Master.DDBackNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                      cmdInsDetail.Parameters.AddWithValueAndNullHandle("@ItemFgItemNo", Item.FgItemNo);

                      transResult = (int)cmdInsDetail.ExecuteNonQuery();

                      if (transResult <= 0)
                      {
                          throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUpdateNotSuccessfully);
                      }
                      #endregion Update only DetailTable
                      #region Update Issue and Receive if Transaction is not Other







                      #endregion Update Issue and Receive if Transaction is not Other
                  }

                  #endregion Find Transaction Mode Insert or Update
              }


              #endregion Update Detail Table

              #endregion  Update into Details(Update complete in Header)
              #region return Current ID and Post Status

              sqlText = "";
              sqlText = sqlText + "select distinct Post from DutyDrawBackHeader WHERE DDBackNo=@MasterDDBackNo ";
              SqlCommand cmdIPS = new SqlCommand(sqlText, currConn);
              cmdIPS.Transaction = transaction;

              cmdIPS.Parameters.AddWithValue("@MasterDDBackNo", Master.DDBackNo);

              PostStatus = (string)cmdIPS.ExecuteScalar();
              if (string.IsNullOrEmpty(PostStatus))
              {
                  throw new ArgumentNullException(MessageVM.issueMsgMethodNameUpdate, MessageVM.issueMsgUnableCreatID);
              }


              #endregion Prefetch
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
              retResults[1] = MessageVM.issueMsgSuccessfullyPost;
              retResults[2] = Master.DDBackNo;
              retResults[3] = PostStatus;
              #endregion SuccessResult

          }
          #endregion Try

          #region Catch and Finall
          //catch (SqlException sqlex)
          //{
          //    transaction.Rollback();
          //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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
              transaction.Rollback();
              ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());//, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
              //////throw ex;
              FileLogger.Log("DutyDrawBackDAL", "DutyDrawBackPost", ex.ToString() + "\n" + sqlText);

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
      #endregion
    }
}
