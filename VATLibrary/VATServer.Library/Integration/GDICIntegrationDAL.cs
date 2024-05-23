using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.OleDb;
//using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using System.IO;
using Excel;
using System.Globalization;
using VATServer.Interface;
using Newtonsoft.Json;

namespace VATServer.Library.Integration
{
    public class GDICIntegrationDAL
    {

        #region GDIC - Green Delta Insurance Company

        private string[] sqlResults;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        CommonDAL _cDAL = new CommonDAL();
        SaleDAL _SaleDAL = new SaleDAL();

        public DataTable GetSource_SaleData_MasterXX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {
                CommonDAL commonDal = new CommonDAL();

                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                string TableName = "vat_sym_data"; // need to change

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                if (MonthlyMiddlewarTable)
                {
                    TableName = "vat_sym_data_fullmonth";
                }

                sqlText = @"


--Update vat_sym_data set VATRate = 15 where Type = 'VAT' and IsLeader = 'N'
--Update vat_sym_data set VATRate = 0 where (Type = 'Export' or Type = 'NonVAT') and IsLeader = 'N'

Update " + TableName + @" set VATRate = 15 where Type = 'VAT' and IsLeader = 'N'
Update " + TableName + @" set VATRate = 0 where (Type = 'Export' or Type = 'NonVAT') and IsLeader = 'N'




SELECT
0							                                                        Selected
,InvoiceNo																			InvoiceNo
,ID																			        ID
,Post                                                                               Post
,''																					IsPrint
,ReferenceNo                                                                        ReferenceNo
,BranchCode																	        BranchCode
,CustomerName																	    CustomerName
,CustomerCode																	    CustomerCode
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										    Invoice_Date_Time
,DeliveryAddress																    DeliveryAddress
, ''																				VehicleNo
, ''																				VehicleType
,SUM((1*UnitPrice)+(1*UnitPrice)*(VATRate/100))										TotalValue
,SUM(1)																				TotalQuantity
,SUM((1*UnitPrice)*(VATRate/100)) 													TotalVATAmount
, IsProcessed																		Processed

FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'

----------------------------------------!Care on GROUP BY---------------------------------------- 
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
InvoiceNo
,ID
,BranchCode
,CustomerName
,CustomerCode
,InvoiceDateTime
,DeliveryAddress
----,VehicleNo
----,VehicleType
,IsProcessed
,Post
----,IsPrint
,ReferenceNo
";

                #endregion

                sqlText += " ORDER BY InvoiceDateTime";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

            return dtSalesMaster;

        }

        public DataTable GetSaleDataX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

SELECT
ID																			                                ID
,Post                                                                                                       Post
,BranchCode																	                                Branch_Code
,LTRIM(RTRIM(CustomerGroup))														                        CustomerGroup
,LTRIM(RTRIM(CustomerName))															                        Customer_Name
,LTRIM(RTRIM(CustomerCode))															                        Customer_Code
,DeliveryAddress																                            Delivery_Address
,''																					                        Vehicle_No
,'' 																				                        VehicleType
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										                            Invoice_Date_Time
,ReferenceNo  																                                Reference_No  
,''																					                        Comments
,'New'																				                        Sale_Type
,'' 																				                        Previous_Invoice_No
,'N'																				                        Is_Print
,'0'																				                        Tender_Id
,LCNumber																		                            LC_Number
,CurrencyCode																	                            Currency_Code
,LTRIM(RTRIM(ProductCode))															                        Item_Code
,LTRIM(RTRIM(ProductName))															                        Item_Name
,UOM																			                            UOM
,1																					                        Quantity
,UnitPrice																	                                NBR_Price
,VATRate																		                            VAT_Rate
,SDRate																		                                SD_Rate
,(1*UnitPrice)+(1*UnitPrice)*(VATRate/100)											                        TotalValue
,(1*UnitPrice)																		                        SubTotal
,(1*UnitPrice)*(VATRate/100) 														                        VAT_Amount
,'N'																				                        Non_Stock
,0																					                        Trading_MarkUp
,REPLACE(Type, ' ', '')																                        Type
,DiscountAmount																                                Discount_Amount
,PromotionalQuantity															                            Promotional_Quantity
,'VAT 4.3'																			                        VAT_Name
, case when TransactionType = 'Local' then 'Other' else TransactionType end                                 TransactionType
,'1900-Jan-01'																		                        PreviousInvoiceDateTime
,'0'																				                        PreviousNBRPrice
,'0'																				                        PreviousQuantity
,'0'																				                        PreviousUOM
,'0'																				                        PreviousSubTotal
,'0'																				                        PreviousSD
,'0'																				                        PreviousSDAmount
,'0'																				                        PreviousVATRate
,'0'																				                        PreviousVATAmount
,'NA'																				                        ReasonOfReturn
, IsLeader																			                        IsLeader
, case when ISNULL(IsLeader, 'NA') = 'Y' then ISNULL(LeaderAmount,0) else 0	end				                LeaderAmount
, case when ISNULL(IsLeader, 'NA') = 'Y' then ((1*LeaderAmount)*(VATRate/100)) else 0 end	                LeaderVATAmount
, case when ISNULL(IsLeader, 'NA') = 'N' then ISNULL(LeaderAmount,0) 
when ISNULL(IsLeader, 'NA') = 'Y' then (1*UnitPrice) - ISNULL(LeaderAmount,0) else 0 end	                NonLeaderAmount
, case when ISNULL(IsLeader, 'NA') = 'N' then (1*LeaderAmount)*(VATRate/100)
when ISNULL(IsLeader, 'NA') = 'Y' then ((1*UnitPrice) - ISNULL(LeaderAmount,0))*(VATRate/100) else 0 end    NonLeaderVATAmount


FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }



                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }




                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;


            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        public DataTable GetSaleDataXX(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                CommonDAL commonDal = new CommonDAL();
                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }


                if (MonthlyMiddlewarTable)
                {
                    TableName = "vat_sym_data_fullmonth";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"


SELECT
ID																			                                ID
,Post                                                                                                       Post
,BranchCode																	                                Branch_Code
,LTRIM(RTRIM(CustomerGroup))														                        CustomerGroup
,LTRIM(RTRIM(CustomerName))															                        Customer_Name
,LTRIM(RTRIM(CustomerCode))															                        Customer_Code
,DeliveryAddress																                            Delivery_Address
,isnull(CustomerAddress,'-')																                CustomerAddress
,(case when len(isnull(bin,'')) >=9 then bin else ISNULL(NationalID,'-') end)								CustomerBIN
,''																					                        Vehicle_No
,'' 																				                        VehicleType
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										                            Invoice_Date_Time
,ReferenceNo  																                                Reference_No  
,''																					                        Comments
,(case when TransactionType = 'Credit' then TransactionType else 'New' end)									Sale_Type
,'' 																				                        Previous_Invoice_No
,'N'																				                        Is_Print
,'0'																				                        Tender_Id
,LCNumber																		                            LC_Number
,CurrencyCode																	                            Currency_Code
,LTRIM(RTRIM(ProductCode))															                        Item_Code
,LTRIM(RTRIM(ProductName))															                        Item_Name
,UOM																			                            UOM
,1																					                        Quantity
,UnitPrice																	                                NBR_Price
,(case when type != 'Export' and type !='NonVAT' then 15 else 0 end)										VAT_Rate
,SDRate																		                                SD_Rate
,(1*UnitPrice)+(1*UnitPrice)*(VATRate/100.00)											                        TotalValue
,(1*UnitPrice)																		                        SubTotal
,VATAmount 														                                            VAT_Amount
,'N'																				                        Non_Stock
,0																					                        Trading_MarkUp
,(case when type='otherrate' then 'VAT' else Type end)														  Type
,DiscountAmount																                                Discount_Amount
,PromotionalQuantity															                            Promotional_Quantity
,'VAT 4.3'																			                        VAT_Name
, (
case 
when Type ='export' and TransactionType!= 'credit'
	then 'ExportServiceNS'
when (type = 'NonVAT' or Type='OtherRate' or Type = 'VAT') and TransactionType!= 'credit'
	then 'ServiceNS'
else
TransactionType
end
)                                 TransactionType
,'1900-Jan-01'																		                        PreviousInvoiceDateTime
,'0'																				                        PreviousNBRPrice
,'0'																				                        PreviousQuantity
,'0'																				                        PreviousUOM
,'0'																				                        PreviousSubTotal
,'0'																				                        PreviousSD
,'0'																				                        PreviousSDAmount
,'0'																				                        PreviousVATRate
,'0'																				                        PreviousVATAmount
,'NA'																				                        ReasonOfReturn
, (case when ISNULL(IsLeader,'NA')='NA' then 'Y' else IsLeader end)											IsLeader
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' 
	then ISNULL(UnitPrice,0)
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice 
	then GDICLeader
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then UnitPrice 
else 0	
end)																										LeaderAmount
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' and Type='VAT'
	then Cast(((1*UnitPrice)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA') = 'Y' and Type='VAT' and GDICLeader != UnitPrice
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
else 0 
end)																										LeaderVATAmount


, (
case 
when ISNULL(IsLeader, 'NA') = 'N' 
	then ISNULL(UnitPrice,0) 
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice
	then Cast(UnitPrice - GDICLeader as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and Type='vat' and GDICLeader = UnitPrice
	then Cast(((VATAmount*100)/15.00) - GDICLeader as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y'  and GDICLeader = UnitPrice
	then Cast(UnitPrice - GDICLeader as decimal(25,9))
else 0 
end	)																										NonLeaderAmount         
, (
case 
when ISNULL(IsLeader, 'NA') = 'N' and Type='VAT' 
	then Cast((1*ISNULL(UnitPrice,0))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice and Type='VAT'
	then Cast((1*(UnitPrice - GDICLeader))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice and Type='VAT'
	then Cast((1*(((VATAmount*100.00)/15) - GDICLeader))*(15/100.00) as decimal(25,9))
else 0 
end	)																									NonLeaderVATAmount

FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }



                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }




                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;


            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        public DataTable GetSource_SaleData_Master(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                #region Old
                
                ////////CommonDAL commonDal = new CommonDAL();

                ////////bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                ////////#region open connection and transaction

                ////////currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo); // need to change

                ////////if (currConn.State != ConnectionState.Open)
                ////////{
                ////////    currConn.Open();
                ////////}

                ////////#endregion open connection and transaction

                ////////string TableName = "vat_sym_data"; // need to change

                ////////if (paramVM.DataSourceType == "Excel")
                ////////{
                ////////    TableName = "VAT_Source_Sales";
                ////////}

                ////////if (MonthlyMiddlewarTable)
                ////////{
                ////////    TableName = "vat_sym_data_fullmonth";
                ////////}

                #endregion

                DataTable dt = GetSaleSymData(paramVM, connVM);

                CommonDAL commonDal = new CommonDAL();
               
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                string deleteData = @"       
delete GDICSalesTempData                          
";
                SqlCommand cmdDelete = new SqlCommand(deleteData, currConn, transaction);
                cmdDelete.ExecuteNonQuery();

                commonDal.BulkInsert("GDICSalesTempData", dt, currConn, transaction);

                #region SQLText
                
                sqlText = @"

Update GDICSalesTempData set Type = 'VAT' where Type = 'NonLeader'
Update GDICSalesTempData set VATRate = 15 where Type = 'VAT' and IsLeader = 'N'
Update GDICSalesTempData set VATRate = 0 where (Type = 'Export' or Type = 'NonVAT') and IsLeader = 'N'

SELECT
0							                                                        Selected
,InvoiceNo																			InvoiceNo
,ID																			        ID
,Post                                                                               Post
,''																					IsPrint
,ReferenceNo                                                                        ReferenceNo
,BranchCode																	        BranchCode
,CustomerName																	    CustomerName
,CustomerCode																	    CustomerCode
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										    Invoice_Date_Time
,DeliveryAddress																    DeliveryAddress
, ''																				VehicleNo
, ''																				VehicleType
,SUM((1*UnitPrice)+(1*UnitPrice)*(VATRate/100))										TotalValue
,SUM(1)																				TotalQuantity
,SUM((1*UnitPrice)*(VATRate/100)) 													TotalVATAmount
, IsProcessed																		Processed

FROM GDICSalesTempData
WHERE 1=1
------AND TransactionType != 'Credit'

----------------------------------------!Care on GROUP BY---------------------------------------- 
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
InvoiceNo
,ID
,BranchCode
,CustomerName
,CustomerCode
,InvoiceDateTime
,DeliveryAddress
----,VehicleNo
----,VehicleType
,IsProcessed
,Post
----,IsPrint
,ReferenceNo
";

                #endregion

                sqlText += " ORDER BY InvoiceDateTime";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

                if (transaction != null)
                {
                    transaction.Commit();
                }


            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

            return dtSalesMaster;

        }

        public DataTable GetSaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                DataTable dt = GetSaleSymData(paramVM, connVM);

                CommonDAL commonDal = new CommonDAL();
                ////bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                ////string TableName = "vat_sym_data"; // need to change GDIC

                ////if (paramVM.DataSourceType == "Excel")
                ////{
                ////    TableName = "vat_sym_data";
                ////}


                ////if (MonthlyMiddlewarTable)
                ////{
                ////    TableName = "vat_sym_data_fullmonth";
                ////}

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                string deleteData = @"       
delete GDICSalesTempData                          
";
                SqlCommand cmdDelete = new SqlCommand(deleteData, currConn, transaction);
                cmdDelete.ExecuteNonQuery();

                commonDal.BulkInsert("GDICSalesTempData", dt, currConn, transaction);

                #region SQLText

                sqlText = @"
Update GDICSalesTempData set Type = 'VAT' where Type = 'NonLeader'
Update GDICSalesTempData set VATRate = 15 where Type = 'VAT' and IsLeader = 'N'
Update GDICSalesTempData set VATRate = 0 where (Type = 'Export' or Type = 'NonVAT') and IsLeader = 'N'

SELECT
ID																			                                ID
,Post                                                                                                       Post
,BranchCode																	                                Branch_Code
,LTRIM(RTRIM(CustomerGroup))														                        CustomerGroup
,LTRIM(RTRIM(CustomerName))															                        Customer_Name
,LTRIM(RTRIM(CustomerCode))															                        Customer_Code
,DeliveryAddress																                            Delivery_Address
,isnull(CustomerAddress,'-')																                CustomerAddress
,(case when len(isnull(bin,'')) >=9 then bin else ISNULL(NationalID,'-') end)								CustomerBIN
,''																					                        Vehicle_No
,'' 																				                        VehicleType
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										                            Invoice_Date_Time
,ReferenceNo  																                                Reference_No  
,''																					                        Comments
,(case when TransactionType = 'Credit' then TransactionType else 'New' end)									Sale_Type
,'' 																				                        Previous_Invoice_No
,'N'																				                        Is_Print
,'0'																				                        Tender_Id
,LCNumber																		                            LC_Number
,CurrencyCode																	                            Currency_Code
,LTRIM(RTRIM(ProductCode))															                        Item_Code
,LTRIM(RTRIM(ProductName))															                        Item_Name
,UOM																			                            UOM
,1																					                        Quantity
,UnitPrice																	                                NBR_Price
,(case when type != 'Export' and type !='NonVAT' then 15 else 0 end)										VAT_Rate
,SDRate																		                                SD_Rate
,(1*UnitPrice)+(1*UnitPrice)*(VATRate/100.00)											                        TotalValue
,(1*UnitPrice)																		                        SubTotal
,VATAmount 														                                            VAT_Amount
,'N'																				                        Non_Stock
,0																					                        Trading_MarkUp
,(case when type='otherrate' then 'VAT' else Type end)														  Type
,DiscountAmount																                                Discount_Amount
,PromotionalQuantity															                            Promotional_Quantity
,'VAT 4.3'																			                        VAT_Name
, (
case 
when Type ='export' and TransactionType!= 'credit'
	then 'ExportServiceNS'
when (type = 'NonVAT' or Type='OtherRate' or Type = 'VAT') and TransactionType!= 'credit'
	then 'ServiceNS'
else
TransactionType
end
)                                 TransactionType
,'1900-Jan-01'																		                        PreviousInvoiceDateTime
,'0'																				                        PreviousNBRPrice
,'0'																				                        PreviousQuantity
,'0'																				                        PreviousUOM
,'0'																				                        PreviousSubTotal
,'0'																				                        PreviousSD
,'0'																				                        PreviousSDAmount
,'0'																				                        PreviousVATRate
,'0'																				                        PreviousVATAmount
,'NA'																				                        ReasonOfReturn
, (case when ISNULL(IsLeader,'NA')='NA' then 'Y' else IsLeader end)											IsLeader
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' 
	then ISNULL(UnitPrice,0)
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice 
	then GDICLeader
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then UnitPrice 
else 0	
end)																										LeaderAmount
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' and Type='VAT'
	then Cast(((1*UnitPrice)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA') = 'Y' and Type='VAT' and GDICLeader != UnitPrice
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
else 0 
end)																										LeaderVATAmount


, (
case 
when ISNULL(IsLeader, 'NA') = 'N' 
	then ISNULL(UnitPrice,0) 
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice
	then Cast(UnitPrice - GDICLeader as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and Type='vat' and GDICLeader = UnitPrice
	then Cast(((VATAmount*100)/15.00) - GDICLeader as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y'  and GDICLeader = UnitPrice
	then Cast(UnitPrice - GDICLeader as decimal(25,9))
else 0 
end	)																										NonLeaderAmount         
, (
case 
when ISNULL(IsLeader, 'NA') = 'N' and Type='VAT' 
	then Cast((1*ISNULL(UnitPrice,0))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice and Type='VAT'
	then Cast((1*(UnitPrice - GDICLeader))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice and Type='VAT'
	then Cast((1*(((VATAmount*100.00)/15) - GDICLeader))*(15/100.00) as decimal(25,9))
else 0 
end	)																									NonLeaderVATAmount
,ISNULL(IsLeader,'NA') Option1

FROM GDICSalesTempData
WHERE 1=1
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                if (transaction != null)
                {
                    transaction.Commit();
                }

                return dtSales;

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        public DataTable GetSaleData_TypeValidation(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {
                CommonDAL commonDal = new CommonDAL();

                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

SELECT
*
FROM GDICSalesTempData
WHERE 1=1
AND ((Type='VAT' and VATRate=0) or ((Type = 'NonVAT' or Type ='Export') and VATRate != 0) or Type ='OtherRate')

";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;

            }

            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        #region Backup Methods

        public DataTable GetSaleData_TypeValidation_Backup(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {
                CommonDAL commonDal = new CommonDAL();

                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }
                if (MonthlyMiddlewarTable)
                {
                    TableName = "vat_sym_data_fullmonth";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

SELECT
*
FROM " + TableName +
@"
WHERE 1=1
AND ((Type='VAT' and VATRate=0) or ((Type = 'NonVAT' or Type ='Export') and VATRate != 0) or Type ='OtherRate')
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                //if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                //{
                //    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                //}
                //if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                //{
                //    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                //}


                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        public DataTable GetSource_SaleData_Master_Backup(IntegrationParam paramVM)
        {
            #region Initializ
            string sqlText = "";
            DataTable dtSalesMaster = new DataTable();

            SqlConnection currConn = null;
            #endregion

            try
            {

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText
                string TableName = "GDIC_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                sqlText = @"

SELECT
0							                                                        Selected
,InvoiceNo																			InvoiceNo
,ID																			        ID
,Post                                                                               Post
,IsPrint                                                                            IsPrint
,ReferenceNo                                                                        ReferenceNo
,BranchCode																	        BranchCode
,CustomerName																	    CustomerName
,CustomerCode																	    CustomerCode
,cast(InvoiceDateTime as varchar(20)) 										        InvoiceDateTime
,DeliveryAddress																    DeliveryAddress
, VehicleNo 																		VehicleNo
, VehicleType 																		VehicleType
,SUM((Quantity*UnitPrice)+(Quantity*UnitPrice)*(VATRate/100))	                    TotalValue
,SUM(Quantity)																		TotalQuantity
,SUM((Quantity*UnitPrice)*(VATRate/100)) 									        TotalVATAmount
, IsProcessed																		Processed

FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'

----------------------------------------!Care on GROUP BY---------------------------------------- 
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @SalesInvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #region Group By

                sqlText += @"

----------------------------------------GROUP BY---------------------------------------- 
GROUP BY 
InvoiceNo
,ID
,BranchCode
,CustomerName
,CustomerCode
,InvoiceDateTime
,DeliveryAddress
,VehicleNo
,VehicleType
,IsProcessed
,Post
,IsPrint
,ReferenceNo


";

                #endregion

                sqlText += " ORDER BY InvoiceDateTime";

                #endregion

                #region SQLExecution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSalesMaster);

                #endregion

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

            return dtSalesMaster;

        }

        public DataTable GetSaleData_Backup(IntegrationParam paramVM)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                string TableName = "GDIC_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection();
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"


SELECT
ID																			        ID
,Post                                                                               Post
,BranchCode																	        Branch_Code
,LTRIM(RTRIM(CustomerGroup))														CustomerGroup
,LTRIM(RTRIM(CustomerName))															Customer_Name
,LTRIM(RTRIM(CustomerCode))															Customer_Code
,DeliveryAddress																    Delivery_Address
,VehicleNo 																			Vehicle_No
,VehicleType 																		VehicleType
,cast(InvoiceDateTime as varchar(20)) 										        Invoice_Date_Time
,cast(InvoiceDateTime as varchar(20))										        Delivery_Date_Time
,ReferenceNo  																        Reference_No  
,Comments																		    Comments
,'New'																				Sale_Type
,'' 																				Previous_Invoice_No
,'N'																				Is_Print
,'0'																				Tender_Id
,LCNumber																		    LC_Number
,CurrencyCode																	    Currency_Code
,LTRIM(RTRIM(ProductCode))															Item_Code
,LTRIM(RTRIM(ProductName))															Item_Name
,UOM																			    UOM
,Quantity																		    Quantity
,UnitPrice																	        NBR_Price
,VATRate																		    VAT_Rate
,SDRate																		        SD_Rate
,(Quantity*UnitPrice)+(Quantity*UnitPrice)*(VATRate/100)	                        TotalValue
,(Quantity*UnitPrice)													            SubTotal
,(Quantity*UnitPrice)*(VATRate/100) 									            VAT_Amount
,'N'																				Non_Stock
,0																					Trading_MarkUp
,REPLACE(Type, ' ', '')																Type
,DiscountAmount																        Discount_Amount
,PromotionalQuantity															    Promotional_Quantity
,'VAT 4.3'																			VAT_Name
, case when TransactionType = 'Local' then 'Other' else TransactionType end         TransactionType
,PreviousInvoiceDateTime													        PreviousInvoiceDateTime
,PreviousNBRPrice															        PreviousNBRPrice
,PreviousQuantity															        PreviousQuantity
,PreviousUOM																        PreviousUOM
,(PreviousQuantity*PreviousNBRPrice)										        PreviousSubTotal
,PreviousSD																	        PreviousSD
,PreviousSDAmount															        PreviousSDAmount
,PreviousVATRate															        PreviousVATRate
,(PreviousQuantity*PreviousNBRPrice)*(PreviousVATRate/100)					        PreviousVATAmount
,ReasonOfReturn																        ReasonOfReturn
,IsLeader
,LeaderAmount

FROM " + TableName +
@"
WHERE 1=1
------AND TransactionType != 'Credit'
------AND IsProcessed='N'
";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }


                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }


                ////if (!string.IsNullOrWhiteSpace(paramVM.Processed) && paramVM.Processed.ToLower() == "ALL".ToLower())
                ////{
                ////    sqlText = sqlText.Replace("IsProcessed='N'", "1=1");
                ////}

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }



                ////if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                ////{
                ////    cmd.Parameters.AddWithValue("@SalesInvoiceNo", paramVM.RefNo);
                ////}



                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;


            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        #endregion

        public ResultVM SaveSale_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {


                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    paramVM.SetSteps(1);
                    rVM = SaveSale(paramVM);
                    paramVM.callBack();
                }
                else
                {
                    DateTime FromDate = Convert.ToDateTime(paramVM.FromDate);
                    DateTime ToDate = Convert.ToDateTime(paramVM.ToDate);
                    Double Days = (ToDate - FromDate).TotalDays + 1;

                    DateTime CurrentDate = FromDate;

                    paramVM.SetSteps(Convert.ToInt32(Days));

                    int rowCount = 0;

                    for (int i = 0; i < Days; i++)
                    {
                        paramVM.IDs = null;
                        paramVM.FromDate = CurrentDate.ToString("yyyy-MMM-dd");
                        paramVM.ToDate = CurrentDate.ToString("yyyy-MMM-dd");
                        rVM = SaveSale(paramVM, "", connVM);

                        rowCount = rowCount + rVM.RowCount;

                        CurrentDate = CurrentDate.AddDays(1);
                        paramVM.callBack();

                    }

                    rVM.RowCount = rowCount;
                }

                #region Save Data

                //////rVM = SaveSale(paramVM);

                #endregion

            }
            catch (Exception ex)
            {
                rVM = new ResultVM
                {
                    Message = ex.Message
                };
                throw ex;
            }
            finally
            {


            }
            return rVM;
        }

        public ResultVM SaveSale(IntegrationParam paramVM, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                #region Get Data from Source DB

                DataTable dtSales = new DataTable();
                dtSales = GetSaleData(paramVM, connVM);

                if (dtSales == null || dtSales.Rows.Count == 0)
                {
                    rVM.Message = "Transactions Already Integrated or Not Exist in Source!";
                    return rVM;
                }

                #endregion

                #region Table Validation

                TableValidation(dtSales, paramVM);

                #endregion

                #region Save Data

                sqlResults = _SaleDAL.SaveAndProcess(dtSales, () => { }, paramVM.DefaultBranchId, null, null, paramVM, null, null, UserId);

                #endregion

                #region Update Source Data

                if (paramVM.IDs == null || paramVM.IDs.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                    {
                        paramVM.IDs = new List<string>();
                        paramVM.IDs.Add(paramVM.RefNo);
                    }
                }


                if (paramVM == null || paramVM.IDs == null || paramVM.IDs.Count == 0)
                {
                    paramVM.IDs = new List<string>();
                    DataTable dtSelected = new DataTable();
                    dtSelected = dtSales.Copy();

                    DataView dv = new DataView(dtSelected);
                    dtSelected = dv.ToTable("Selected", true, "ID");

                    if (dtSelected != null && dtSelected.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtSelected.Rows)
                        {
                            paramVM.IDs.Add(dr["ID"].ToString());
                        }
                    }

                }

                if (paramVM != null && paramVM.IDs.Count >= 0)
                {

                    rVM = UpdateSource_SaleData(paramVM);

                }


                #endregion

                rVM.Status = sqlResults[0];
                rVM.Message = "Data Saved Successfully";
                rVM.RowCount = dtSales.Rows.Count;

                paramVM = new IntegrationParam();

                #region Comments Sep-12-2020

                //////if (paramVM != null && !string.IsNullOrWhiteSpace(paramVM.RefNo))
                //////{
                //////    SaleMasterVM varSaleMasterVM = new SaleMasterVM();
                //////    varSaleMasterVM = _SaleDAL.SelectAllList(0, new[] { "sih.ImportIDExcel" }, new[] { paramVM.RefNo }, null, null, null, null).FirstOrDefault();

                //////    rVM.InvoiceNo = varSaleMasterVM.SalesInvoiceNo;
                //////}

                #endregion

            }
            catch (Exception ex)
            {
                rVM = new ResultVM();
                rVM.Message = ex.Message;
                throw ex;
            }
            finally { }
            return rVM;
        }

        public ResultVM SaveCredit_Pre(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();
            try
            {

                //////DataTable dtCredit = new DataTable();
                //////dtCredit = GetSaleData(paramVM);

                //////if (dtCredit == null || dtCredit.Rows.Count == 0)
                //////{
                //////    rVM.Message = "This Transaction Already Integrated or Not Exist in Source!";
                //////    return rVM;
                //////}

                rVM = SaveSale(paramVM);

            }
            catch (Exception ex)
            {
                rVM.Message = ex.Message;

            }
            finally
            {


            }
            return rVM;
        }

        private void TableValidation(DataTable dtSales, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {

            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MMM-dd") };

            if (!dtSales.Columns.Contains("Branch_Code"))
            {
                var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
                dtSales.Columns.Add(columnName);
            }

            if (!dtSales.Columns.Contains("SL"))
            {
                dtSales.Columns.Add(SL);
            }
            if (!dtSales.Columns.Contains("CreatedBy"))
            {
                dtSales.Columns.Add(CreatedBy);
            }
            if (!dtSales.Columns.Contains("CreatedOn"))
            {
                dtSales.Columns.Add(CreatedOn);
            }
            if (!dtSales.Columns.Contains("ReturnId"))
            {
                dtSales.Columns.Add(ReturnId);
            }

            if (!dtSales.Columns.Contains("TransactionType"))
            {
                dtSales.Columns.Add(TransactionType);
            }
            if (!dtSales.Columns.Contains("BOMId"))
            {
                dtSales.Columns.Add(BOMId);
            }
        }

        public ResultVM UpdateSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            #endregion variable

            try
            {
                CommonDAL commonDal = new CommonDAL();

                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                #region Open Connection and Transaction

                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                    currConn.Open();
                    transaction = currConn.BeginTransaction();
                }

                #endregion


                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }

                if (MonthlyMiddlewarTable)
                {
                    TableName = "vat_sym_data_fullmonth";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE @TableName SET 
  IsProcessed = 'Y'
--, InvoiceNo=sih.SalesInvoiceNo, Post=sih.Post   
FROM @TableName sSal 
--INNER JOIN SalesInvoiceHeaders sih on sSal.ID=sih.ImportIDExcel
WHERE 1=1
";
                sqlText = sqlText.Replace("@TableName", TableName);
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.ID IN('" + IDs + "')";

                }

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = "Success";
                rVM.Message = "Source Data Updated Successfully!";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public ResultVM PostSource_SaleData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region variable
            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
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


                SaleMasterVM varSaleMasterVM = new SaleMasterVM();
                varSaleMasterVM.IDs = paramVM.IDs;

                rVM = _SaleDAL.Multiple_SalePost(varSaleMasterVM, transaction, currConn, connVM);

                string TableName = "GDIC_VAT_Source_DB.dbo.Sales";

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "VAT_Source_Sales";
                }

                string sqlText = @"";
                sqlText += @" 
UPDATE " + TableName +
@" SET 
Post = 'Y'
FROM " + TableName + " sSal" +
@"
INNER JOIN SalesInvoiceHeaders sih on sSal.InvoiceNo=sih.SalesInvoiceNo
WHERE 1=1
";
                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND sSal.InvoiceNo IN('" + IDs + "')";

                }


                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                int rows = cmd.ExecuteNonQuery();

                transaction.Commit();

                rVM.Status = rows > 0 ? "Success" : "Fail";

            }
            #region Catch and finally

            catch (Exception ex)
            {
                rVM.Message = ex.Message;

                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                if (currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            #endregion
            return rVM;
        }

        public DataTable GetSaleSymData(IntegrationParam paramVM, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            #endregion

            try
            {

                CommonDAL commonDal = new CommonDAL();
                bool MonthlyMiddlewarTable = commonDal.settingValue("Integration", "MonthlyMiddlewarTable", connVM, null, null) == "Y";

                string TableName = "vat_sym_data"; // need to change GDIC

                if (paramVM.DataSourceType == "Excel")
                {
                    TableName = "vat_sym_data";
                }

                if (MonthlyMiddlewarTable)
                {
                    TableName = "vat_sym_data_fullmonth";
                }

                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(paramVM.dtConnectionInfo);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQLText

                sqlText = @"

SELECT 
ID
,BranchCode
,CustomerGroup
,CustomerCode
,CustomerName
,DeliveryAddress
,InvoiceDateTime
,ReferenceNo
,Comments
,ProductCode
,ProductName
,UOM
,Quantity
,UnitPrice
,VATRate
,SDRate
,DiscountAmount
,PromotionalQuantity
,Post
,LCNumber
,CurrencyCode
,LineComments
,Type
,TransactionType
,IsLeader
,IsProcessed
,CompanyCode
,VehicleNo
,VehicleType
,PreviousInvoiceNo
,PreviousInvoiceDateTime
,PreviousNBRPrice
,PreviousQuantity
,PreviousUOM
,PreviousSubTotal
,PreviousSD
,PreviousSDAmount
,PreviousVATAmount
,PreviousVATRate
,ReasonOfReturn
,InvoiceNo
,VATAmount
,GDICLeader
,GDICNonLeader
,CustomerAddress
,NationalID
,bin 
FROM " + TableName +
@"
WHERE 1=1

";

                #region Filtering

                if (paramVM.TransactionType == "Credit")
                {
                    sqlText = sqlText + @" AND TransactionType = 'Credit' ";
                }
                else
                {
                    sqlText = sqlText + @" AND TransactionType != 'Credit' ";
                }

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsProcessed,'N')=@Processed";
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(Post,'N')=@PostStatus";
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    sqlText = sqlText + @" AND ISNULL(IsPrint,'N')=@PrintStatus";
                }

                string IDs = "";
                if (paramVM.IDs != null && paramVM.IDs.Count > 0)
                {
                    IDs = string.Join("','", paramVM.IDs);

                    sqlText = sqlText + @" AND ID IN('" + IDs + "')";

                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    sqlText = sqlText + @" AND ID = @InvoiceNo";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime >= @fromDate ";
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    sqlText = sqlText + @" AND InvoiceDateTime < dateadd(d,1,@toDate)";
                }
                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    sqlText = sqlText + @" AND BranchCode = @BranchCode";
                }

                #endregion

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn);

                #region Add Parameter Values

                if (paramVM.Processed == "Y" || paramVM.Processed == "N")
                {
                    cmd.Parameters.AddWithValue("@Processed", paramVM.Processed);
                }
                if (paramVM.PostStatus == "Y" || paramVM.PostStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PostStatus", paramVM.PostStatus);
                }
                if (paramVM.PrintStatus == "Y" || paramVM.PrintStatus == "N")
                {
                    cmd.Parameters.AddWithValue("@PrintStatus", paramVM.PrintStatus);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.RefNo))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", paramVM.RefNo);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    cmd.Parameters.AddWithValue("@fromDate", paramVM.FromDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    cmd.Parameters.AddWithValue("@toDate", paramVM.ToDate);
                }

                if (!string.IsNullOrWhiteSpace(paramVM.BranchCode))
                {
                    cmd.Parameters.AddWithValue("@BranchCode", paramVM.BranchCode);
                }

                #endregion

                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                return dtSales;

            }

            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        #endregion

        #region Statement Report

        public DataSet StatementReport(string PeriodId, int BranchId, string FromDate, string ToDate, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Declarations

            ResultVM rVM = new ResultVM();

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            DataTable data = new DataTable();
            DataSet dataSet = new DataSet();

            #endregion

            try
            {

                BranchProfileDAL dal = new BranchProfileDAL();
                BranchProfileVM branchProfileVm = new BranchProfileVM();

                if (BranchId != 0)
                {
                    branchProfileVm = dal.SelectAllList(BranchId.ToString(), null, null, null, null, connVM)
                        .FirstOrDefault();
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
                    transaction = currConn.BeginTransaction("");
                }
                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();
                string companyCode = commonDal.settingValue("CompanyCode", "Code", connVM, currConn, transaction);

                #region SQL Query

                #region 9.1 Data Query

                sqlText = @"

select b.BranchCode,b.BranchLegalName BranchName,sih.ImportIDExcel ID,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'1' NoteNo
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where 1=1 
and sid.Type in('Export')
and sid.TransactionType in('ServiceNS','Service','Export','ExportServiceNS','ExportTender','ExportPackage'
,'ExportTrading','ExportTradingTender','ExportService')
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

union all

select b.BranchCode,b.BranchLegalName,sih.ImportIDExcel,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'3' Note
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where 1=1 
and sid.TransactionType in('ServiceNS','Service','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading' ,'TradingTender','InternalIssue','Service'
,'TollSale')
AND isnull(sid.SourcePaidQuantity,0)=0
and ISNULL(sid.IsLeader,'NA') IN ('NA','Y')
and sid.Type in('NonVAT')
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

union all

select b.BranchCode,b.BranchLegalName,sih.ImportIDExcel,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'4' Note
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where 1=1 
and sid.Type in('VAT')
and sid.TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
AND isnull(sid.SourcePaidQuantity,0) = 0
and ISNULL(sid.IsLeader,'NA') IN ('NA','Y')
and sid.ProductType is null
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

union all

select b.BranchCode,b.BranchLegalName,sih.ImportIDExcel,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'4' Note
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where 1=1 
and sid.Type in('VAT')
and sid.TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service','TollSale')
AND isnull(sid.SourcePaidQuantity,0) = 0
and ISNULL(sid.IsLeader,'NA') ='N'
and sid.ProductType is null
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

union all

select b.BranchCode,b.BranchLegalName,sih.ImportIDExcel,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'27' NoteNo
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where  1=1   
and sid.Type in('VAT')
and sid.TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and ISNULL(sid.IsLeader,'NA') = 'Y'
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

union all

select b.BranchCode,b.BranchLegalName,sih.ImportIDExcel,sid.SalesInvoiceNo,sid.InvoiceDateTime,pro.ProductCode,pro.ProductName,sid.SubTotal,sid.VATAmount,sid.LeaderAmount
,sid.LeaderVATAmount,sid.NonLeaderAmount,sid.NonLeaderVATAmount,sid.Type,sid.TransactionType,ISNULL(sid.IsLeader,'NA')IsLeader
,'32' NoteNo
from SalesInvoiceDetails sid
left outer join SalesInvoiceHeaders sih on sid.SalesInvoiceNo=sih.SalesInvoiceNo
left outer join Products pro on sid.ItemNo=pro.ItemNo
left outer join BranchProfiles b on sid.BranchID=b.BranchID
where  1=1   
and sid.Type in('VAT')
and sid.TransactionType in('ServiceNS','Service','TollFinishIssue','Other','RawSale','PackageSale','Wastage', 'SaleWastage', 'CommercialImporter'
,'ServiceNS','Tender','Trading','ExportTrading','TradingTender','InternalIssue','Service')
and ISNULL(sid.IsLeader,'NA') = 'N'
and sid.PeriodID=@PeriodID
and sid.BranchId=@BranchId

";
                #endregion

                #region Daily Monthly

                string sqlDailyMonthly = @"

select BranchCode,ID,InvoiceDateTime,ProductCode,ProductName,UnitPrice,VATAmount,GDICLeader,GDICNonLeader
,VATRate,Type,TransactionType,isnull(IsProcessed,'N')IsProcessed
from SymData.dbo.vat_sym_data
where 1=1 
and InvoiceDateTime>=@FromDate
and InvoiceDateTime<=@ToDate
@conditionText
--and BranchCode=@BranchCode

select BranchCode,ID,InvoiceDateTime,ProductCode,ProductName,UnitPrice,VATAmount,GDICLeader,GDICNonLeader
,VATRate,Type,TransactionType,isnull(IsProcessed,'N')IsProcessed
from SymData.dbo.vat_sym_data_fullmonth
where 1=1 
and InvoiceDateTime>=@FromDate
and InvoiceDateTime<=@ToDate
@conditionText
--and BranchCode=@BranchCode

";

                #endregion

                #region Daily Monthly Data Deff

                string sqlDailyMonthlyDataDeff = @"

create table #Temp
(
[SL] [int] IDENTITY(1,1) NOT NULL,   
DailyID Varchar(50),
DailyBranchCode Varchar(50),    
DailyInvoiceDateTime datetime, 
DailyProductCode Varchar(50), 
DailyProductName Varchar(50), 
DailyUnitPrice decimal(25, 9), 
DailyVATAmount decimal(25, 9),

MonthlyID Varchar(50),
MonthlyBranchCode Varchar(50),    
MonthlyInvoiceDateTime datetime, 
MonthlyProductCode Varchar(50), 
MonthlyProductName Varchar(50), 
MonthlyUnitPrice decimal(25, 9), 
MonthlyVATAmount decimal(25, 9),
)


insert into #Temp (DailyID,DailyBranchCode,DailyInvoiceDateTime,DailyProductCode,DailyProductName,DailyUnitPrice,DailyVATAmount
,MonthlyID,MonthlyBranchCode,MonthlyInvoiceDateTime,MonthlyProductCode,MonthlyProductName,MonthlyUnitPrice,MonthlyVATAmount)

SELECT d.ID DailyID,d.BranchCode DailyBranchCode,d.InvoiceDateTime DailyInvoiceDateTime,d.ProductCode DailyProductCode
,d.ProductName DailyProductName,d.UnitPrice DailyUnitPrice,d.VATAmount DailyVATAmount

,m.ID MonthlyID,m.BranchCode MonthlyBranchCode,m.InvoiceDateTime MonthlyInvoiceDateTime, m.ProductCode MonthlyProductCode
,m.ProductName MonthlyProductName,m.UnitPrice MonthlyUnitPrice,m.VATAmount MonthlyVATAmount

FROM SymData.dbo.vat_sym_data d
FULL OUTER JOIN SymData.dbo.vat_sym_data_fullmonth m
ON d.ID=m.ID and d.ProductCode=m.ProductCode

where 1=1
and d.InvoiceDateTime>=@FromDate
and d.InvoiceDateTime<=@ToDate
and m.InvoiceDateTime>=@FromDate
and m.InvoiceDateTime<=@ToDate
@conditionText
--and d.BranchCode=@BranchCode
ORDER BY d.ID;

select MonthlyID ID,MonthlyBranchCode BranchCode,MonthlyInvoiceDateTime InvoiceDateTime,MonthlyProductCode ProductCode,MonthlyProductName ProductName
,0.00 DailyUnitPrice,0.00 DailyVATAmount
,MonthlyUnitPrice,MonthlyVATAmount
from #Temp where DailyID is null

union all

select DailyID ID,DailyBranchCode BranchCode,DailyInvoiceDateTime InvoiceDateTime,DailyProductCode ProductCode ,DailyProductName ProductName
,DailyUnitPrice ,DailyVATAmount 
,0.00 MonthlyUnitPrice,0.00 MonthlyVATAmount
from #Temp where MonthlyID is null

union all

select DailyID ID,DailyBranchCode BranchCode,DailyInvoiceDateTime InvoiceDateTime,DailyProductCode ProductCode ,DailyProductName ProductName
,DailyUnitPrice, MonthlyUnitPrice
,DailyVATAmount, MonthlyVATAmount
from #Temp
where 1=1 
and DailyID is not null
and MonthlyID is not null
and ((DailyUnitPrice-MonthlyUnitPrice)!=0 or (DailyVATAmount-MonthlyVATAmount)!=0 )

drop table #Temp 


";

                #endregion

                #endregion

                #region Sql Condition Apply

                string conditionText = "";

                if (BranchId != 0)
                {

                    conditionText = " and BranchCode=@BranchCode";

                    sqlDailyMonthly = sqlDailyMonthly.Replace("@conditionText", conditionText);

                    conditionText = "";

                    conditionText = " and d.BranchCode=@BranchCode";
                    sqlDailyMonthlyDataDeff = sqlDailyMonthlyDataDeff.Replace("@conditionText", conditionText);


                }
                else
                {
                    sqlDailyMonthly = sqlDailyMonthly.Replace("@conditionText", "");

                    sqlDailyMonthlyDataDeff = sqlDailyMonthlyDataDeff.Replace("@conditionText", "");

                }
                ////string sqlQueary = sqlDailyMonthly + "  " + sqlDailyMonthlyDataDeff;


                if (BranchId == 0)
                {
                    sqlText = sqlText.Replace("=@BranchId", ">@BranchId");

                }

                sqlText = sqlText + "  " + sqlDailyMonthly + "  " + sqlDailyMonthlyDataDeff;

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Sql Parameters

                cmd.Parameters.AddWithValue("@PeriodID", PeriodId);
                cmd.Parameters.AddWithValue("@BranchId", BranchId);

                cmd.Parameters.AddWithValue("@FromDate", FromDate);
                cmd.Parameters.AddWithValue("@ToDate", ToDate);

                if (BranchId != 0)
                {

                    cmd.Parameters.AddWithValue("@BranchCode", branchProfileVm.BranchCode);

                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dataSet);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #region Catch & Finally

            catch (Exception ex)
            {
                throw;

            }

            finally
            {
                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #endregion

            return dataSet;
        }

        #endregion

        #region API Integration

        public string[] SaveSaleAPI(DataTable dt, SysDBInfoVMTemp connVM = null)
        {
            ResultVM rVM = new ResultVM();

            string[] retResult = new string[6];

            retResult[0] = "Fail";
            retResult[1] = "Fail";

            try
            {
                string UniqueKey = Guid.NewGuid().ToString();

                var columnName = new DataColumn("UniqueKey") { DefaultValue = UniqueKey };
                dt.Columns.Add(columnName);

                IntegrationParam paramVM = new IntegrationParam();

                #region Get Data from Source DB

                DataTable dtSales = new DataTable();
                dtSales = GetSaleDataAPI(dt, connVM, UniqueKey);

                if (dtSales == null || dtSales.Rows.Count == 0)
                {
                    retResult[1] = "Transactions Already Integrated or Not Exist in Source!";
                    return retResult;
                }

                #endregion

                #region Table Validation

                DataTableValidation(dtSales);

                #endregion

                #region Save Data

                sqlResults = _SaleDAL.SaveAndProcess(dtSales, () => { }, paramVM.DefaultBranchId, null, connVM, paramVM, null, null);

                #endregion

                retResult[0] = sqlResults[0];
                retResult[1] = "Data Saved Successfully";
                //rVM.RowCount = dtSales.Rows.Count;

                //paramVM = new IntegrationParam();

            }
            catch (Exception ex)
            {
                retResult[0] = "Fail";
                retResult[1] = ex.Message;
                throw ex;
            }
            finally { }
            return retResult;
        }

        public DataTable GetSaleDataAPI(DataTable dt, SysDBInfoVMTemp connVM = null, string UniqueKey = "")
        {
            #region Initializ
            string sqlText = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            try
            {
                
                string TableName = "SalesTempData_API";

                CommonDAL commonDal = new CommonDAL();

                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction("");

                #endregion open connection and transaction

                commonDal.BulkInsert("SalesInvoice_Audit", dt, currConn, transaction, 10000, null, connVM);
                commonDal.BulkInsert("SalesTempData_API", dt, currConn, transaction, 10000, null, connVM);

                #region SQLText

                sqlText = @"


SELECT
ID																			                                ID
,Post                                                                                                       Post
,BranchCode																	                                Branch_Code
,LTRIM(RTRIM(CustomerGroup))														                        CustomerGroup
,LTRIM(RTRIM(CustomerName))															                        Customer_Name
,LTRIM(RTRIM(CustomerCode))															                        Customer_Code
,DeliveryAddress																                            Delivery_Address
,isnull(CustomerAddress,'-')																                CustomerAddress
,(case when len(isnull(bin,'')) >=9 then bin else ISNULL(NationalID,'-') end)								CustomerBIN
,''																					                        Vehicle_No
,'' 																				                        VehicleType
,Format(InvoiceDateTime, 'yyyy-MMM-dd') 										                            Invoice_Date_Time
,ReferenceNo  																                                Reference_No  
,''																					                        Comments
,(case when TransactionType = 'Credit' then TransactionType else 'New' end)									Sale_Type
,'' 																				                        Previous_Invoice_No
,'N'																				                        Is_Print
,'0'																				                        Tender_Id
,LCNumber																		                            LC_Number
,CurrencyCode																	                            Currency_Code
,LTRIM(RTRIM(ProductCode))															                        Item_Code
,LTRIM(RTRIM(ProductName))															                        Item_Name
,UOM																			                            UOM
,1																					                        Quantity
,UnitPrice																	                                NBR_Price
,(case when type != 'Export' and type !='NonVAT' then 15 else 0 end)										VAT_Rate
,SDRate																		                                SD_Rate
,(1*UnitPrice)+(1*UnitPrice)*(VATRate/100.00)											                        TotalValue
,(1*UnitPrice)																		                        SubTotal
,VATAmount 														                                            VAT_Amount
,'N'																				                        Non_Stock
,0																					                        Trading_MarkUp
,(case when type='otherrate' then 'VAT' else Type end)														  Type
,DiscountAmount																                                Discount_Amount
,PromotionalQuantity															                            Promotional_Quantity
,'VAT 4.3'																			                        VAT_Name
, (
case 
when Type ='export' and TransactionType!= 'credit'
	then 'ExportServiceNS'
when (type = 'NonVAT' or Type='OtherRate' or Type = 'VAT') and TransactionType!= 'credit'
	then 'ServiceNS'
else
TransactionType
end
)                                 TransactionType
,'1900-Jan-01'																		                        PreviousInvoiceDateTime
,'0'																				                        PreviousNBRPrice
,'0'																				                        PreviousQuantity
,'0'																				                        PreviousUOM
,'0'																				                        PreviousSubTotal
,'0'																				                        PreviousSD
,'0'																				                        PreviousSDAmount
,'0'																				                        PreviousVATRate
,'0'																				                        PreviousVATAmount
,'NA'																				                        ReasonOfReturn
, (case when ISNULL(IsLeader,'NA')='NA' then 'Y' else IsLeader end)											IsLeader
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' 
	then ISNULL(UnitPrice,0)
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice 
	then GDICLeader
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then UnitPrice 
else 0	
end)																										LeaderAmount
, (
case 
when ISNULL(IsLeader, 'NA') = 'NA' and Type='VAT'
	then Cast(((1*UnitPrice)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA') = 'Y' and Type='VAT' and GDICLeader != UnitPrice
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice 
	then Cast(((1*GDICLeader)*(15/100.00)) as decimal(25,9))
else 0 
end)																										LeaderVATAmount


, (
case 
when ISNULL(IsLeader, 'NA') = 'N' 
	then ISNULL(UnitPrice,0) 
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice
	then Cast(UnitPrice - GDICLeader as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice
	then Cast(((VATAmount*100)/15.00) - GDICLeader as decimal(25,9))
else 0 
end	)																										NonLeaderAmount         
, (
case 
when ISNULL(IsLeader, 'NA') = 'N' and Type='VAT' 
	then Cast((1*ISNULL(UnitPrice,0))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader != UnitPrice and Type='VAT'
	then Cast((1*(UnitPrice - GDICLeader))*(15/100.00)as decimal(25,9))
when ISNULL(IsLeader, 'NA')='Y' and GDICLeader = UnitPrice and Type='VAT'
	then Cast((1*(((VATAmount*100.00)/15) - GDICLeader))*(15/100.00) as decimal(25,9))
else 0 
end	)																									NonLeaderVATAmount

FROM " + TableName +
@"
WHERE 1=1
and UniqueKey=@UniqueKey 
------AND TransactionType != 'Credit'
------AND IsProcessed='N'

delete SalesTempData_API where 1=1 and UniqueKey=@UniqueKey 

";

                #endregion

                #region SQL Execution

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@UniqueKey", UniqueKey);
                DataTable dtSales = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dtSales);

                #endregion

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

                return dtSales;

            }
            #region Catch and Finall
            catch (Exception ex)
            {

                throw ex;
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

        private void DataTableValidation(DataTable dtSales)
        {

            var SL = new DataColumn("SL") { DefaultValue = "" };
            var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = "API" };
            var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
            var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
            ////var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
            var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString("yyyy-MMM-dd") };

            ////if (!dtSales.Columns.Contains("Branch_Code"))
            ////{
            ////    var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
            ////    dtSales.Columns.Add(columnName);
            ////}

            if (!dtSales.Columns.Contains("SL"))
            {
                dtSales.Columns.Add(SL);
            }
            if (!dtSales.Columns.Contains("CreatedBy"))
            {
                dtSales.Columns.Add(CreatedBy);
            }
            if (!dtSales.Columns.Contains("CreatedOn"))
            {
                dtSales.Columns.Add(CreatedOn);
            }
            if (!dtSales.Columns.Contains("ReturnId"))
            {
                dtSales.Columns.Add(ReturnId);
            }

            ////if (!dtSales.Columns.Contains("TransactionType"))
            ////{
            ////    dtSales.Columns.Add(TransactionType);
            ////}
            if (!dtSales.Columns.Contains("BOMId"))
            {
                dtSales.Columns.Add(BOMId);
            }
        }


        #endregion

    }
}
