using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;


namespace VATServer.Library
{
    public class BOMDAL : IBOM
    {
        #region Global Variables

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        private const string FieldDelimeter = DBConstant.FieldDelimeter;


        #endregion

        public DataTable SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT   
B.FinishItemNo, 
B.FirstSupplyDate, 
P.ProductName, 
isnull(NULLIF(b.Comments, ''),'-')Comments,
b.VATName,
b.WholeSalePrice,
b.RawOHCost,
b.VATRate,b.SD,b.PacketPrice,b.NBRPrice,b.TradingMarkUp,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo
,isnull(b.CustomerID,0)CustomerID

FROM dbo.BOMs AS B inner JOIN
dbo.Products AS P ON B.FinishItemNo = P.ItemNo
left outer join Customers cus on isnull(b.CustomerID,0)=cus.CustomerID

 WHERE 
b.FinishItemNo=@finItem and b.EffectDate=@effectDate and b.VATName=@vatName

                                ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(b.CustomerId,0)='" + CustomerID + "' ";
                }
                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMMaster", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMMaster", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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


                sqlText = @"
--declare @finItem varchar(20);
--declare @vatName varchar(50);
--declare @effectDate datetime;
-- SET @finItem=62;
-- SET @vatName='vat 1'
-- SET @effectDate='2013-07-01'

SELECT   


stat,rawitemtype,BOMLineNo, 
RawItemNo, 
ProductName, 
UOM,
UseQuantity, 
WastageQuantity, 
Cost, 
RebateRate, 
ActiveStatus,UnitCost,
ProductCode ,HSCodeNo,
UOMPrice, 
UOMc, 
UOMn, 
UOMUQty, 
UOMWQty,isnull(PBOMId,0)PBOMId,PInvoiceNo
,isnull(TransactionType,'0')TransactionType
,isnull(CustomerID,0)CustomerID

from(

SELECT   
'A' as stat,rawitemtype,R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Raw' ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'B' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
 and RawItemType='Pack'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'C' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
 and RawItemType='Finish'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union 

SELECT  
'D' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName

 and RawItemType='Trading'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 


union
SELECT  
'E' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Overhead'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union
SELECT  
'F' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='WIP'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
union
SELECT  
'G' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
WHERE 
r.FinishItemNo=@finItem and r.EffectDate=@effectDate and r.VATName=@vatName
and RawItemType='Service'
 ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(r.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 
) as v1 
order by stat,BOMLineNo
";


                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@finItem"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@vatName"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMRaw", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMRaw", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchOH(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("OH");

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

                sqlText = @"SELECT 
                b.HeadName HeadNameOld,
                b.HeadID,
                p.ProductCode OHCode,p.ProductName HeadName,
                b.HeadAmount,
                b.OHLineNo,
                b.FinishItemNo,
                isnull(b.AdditionalCost,0)AdditionalCost,
                convert(varchar,b.EffectDate,120)EffectDate,
                isnull(b.RebatePercent,0)RebatePercent
                ,isnull(b.CustomerID,0)CustomerID
                FROM BOMCompanyOverhead b  LEFT OUTER JOIN
                products p ON b.HeadID=p.ItemNo               
                WHERE FinishItemNo=@finItem and EffectDate=@effectDate and VATName=@vatName ";
                if (CustomerID == "0" || string.IsNullOrEmpty(CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(b.CustomerId,0)='" + CustomerID + "' ";
                }
                sqlText += @" 

                order by OHLineNo";

                SqlCommand objCommCBOHead = new SqlCommand();
                objCommCBOHead.Connection = currConn;
                objCommCBOHead.CommandText = sqlText;
                objCommCBOHead.CommandType = CommandType.Text;

                #endregion

                #region param

                if (finItem == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@finItem"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@finItem", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@finItem"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@finItem"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@finItem", finItem);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@finItem"].Value = finItem;
                    }
                }

                if (vatName == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@vatName"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@vatName", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@vatName"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@vatName"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@vatName", vatName);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@vatName"].Value = vatName;
                    }
                }

                if (effectDate == "")
                {
                    if (!objCommCBOHead.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@effectDate", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@effectDate"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOHead.Parameters.Contains("@effectDate"))
                    {
                        objCommCBOHead.Parameters.AddWithValue("@effectDate", effectDate);
                    }
                    else
                    {
                        objCommCBOHead.Parameters["@effectDate"].Value = effectDate;
                    }
                }


                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOHead);
                dataAdapter.Fill(dataTable);


            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchOH", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchOH", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        #region New

        //
        public DataTable SearchBOMMasterNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                #endregion open connection and transaction

                #region sql statement

                sqlText = @"SELECT   
B.FinishItemNo, 
P.ProductName, 
isnull(NULLIF(b.Comments, ''),'-')Comments,
b.VATName,
b.WholeSalePrice,
b.RawOHCost,
b.VATRate,b.SD,b.PacketPrice,b.NBRPrice,b.TradingMarkUp,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo
,isnull(b.CustomerID,0)CustomerID

FROM dbo.BOMs AS B inner JOIN
dbo.Products AS P ON B.FinishItemNo = P.ItemNo
left outer join Customers cus on isnull(b.CustomerID,0)=cus.CustomerID

 WHERE 1=1 
and b.BOMId=@BOMId 

                                ";

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMMasterNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMMasterNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        //
        public DataTable SearchBOMRawNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM");

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


                sqlText = @"
--declare @finItem varchar(20);
--declare @vatName varchar(50);
--declare @effectDate datetime;
-- SET @finItem=62;
-- SET @vatName='vat 1'
-- SET @effectDate='2013-07-01'

SELECT   


stat,rawitemtype,BOMLineNo, 
RawItemNo, 
ProductName, 
UOM,
UseQuantity, 
WastageQuantity, 
Cost, 
RebateRate, 
ActiveStatus,UnitCost,
ProductCode ,HSCodeNo,
UOMPrice, 
UOMc, 
UOMn, 
UOMUQty, 
UOMWQty,isnull(PBOMId,0)PBOMId,PInvoiceNo
,isnull(TransactionType,'0')TransactionType
,isnull(CustomerID,0)CustomerID
,isnull(IssueOnProduction,'Y')IssueOnProduction

from(

SELECT   
'A' as stat,rawitemtype,R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
 
and RawItemType In('Raw','NonInventory') ";

                sqlText += @" 
union 

SELECT  
'B' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction

FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
 
 
 and RawItemType='Pack'
 ";

                sqlText += @" 
union 

SELECT  
'C' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo

 WHERE 1=1
 and r.BOMId=@BOMId 
 
 
 and RawItemType='Finish'
 ";

                sqlText += @" 
union 

SELECT  
'D' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  

 and RawItemType='Trading'
 ";

                sqlText += @" 


union
SELECT  
'E' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='Overhead'
 ";

                sqlText += @" 
union
SELECT  
'F' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='WIP'
 ";

                sqlText += @" 
union
SELECT  
'G' as stat, rawitemtype,
R.BOMLineNo, 
R.RawItemNo, 
P.ProductName, 
R.UOM,
isnull(R.UseQuantity,0)UseQuantity, 
isnull(R.WastageQuantity,0)WastageQuantity, 
isnull(R.Cost,0)Cost, 
isnull(R.RebateRate,0)RebateRate, 
'Y' ActiveStatus,UnitCost,
P.ProductCode ,isnull(p.HSCodeNo,'N/A')HSCodeNo,
isnull(R.UOMPrice,R.Cost)UOMPrice, 
isnull(R.UOMc,1)UOMc, 
isnull(R.UOMn,R.UOM)UOMn, 
isnull(R.UOMUQty,R.UseQuantity)UOMUQty, 
isnull(R.UOMWQty,R.WastageQuantity)UOMWQty,isnull(R.PBOMId,0)PBOMId,isnull(nullif(R.PInvoiceNo,'0'),'0')PInvoiceNo
,isnull(R.TransactionType,'0') TransactionType
,isnull(r.CustomerID,0)CustomerID
,isnull(r.IssueOnProduction,'Y')IssueOnProduction
FROM dbo.BOMRaws AS R inner JOIN
dbo.Products AS P ON R.RawItemNo = P.ItemNo
 WHERE 1=1
 and r.BOMId=@BOMId 
  
and RawItemType='Service'
 ";
                sqlText += @" 
) as v1 
order by stat,BOMLineNo
";


                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMRawNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchBOMRawNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable SearchOHNew(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            //int transResult = 0;
            //int countId = 0;
            string sqlText = "";

            DataTable dataTable = new DataTable("OH");

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

                sqlText = @"SELECT 
                b.HeadName HeadNameOld,
                b.HeadID,
                p.ProductCode OHCode,p.ProductName HeadName,
                b.HeadAmount,
                b.OHLineNo,
                b.FinishItemNo,
                isnull(b.AdditionalCost,0)AdditionalCost,
                convert(varchar,b.EffectDate,120)EffectDate,
                isnull(b.RebatePercent,0)RebatePercent
                ,isnull(b.CustomerID,0)CustomerID
                FROM BOMCompanyOverhead b  LEFT OUTER JOIN
                products p ON b.HeadID=p.ItemNo       

 WHERE 1=1
 and b.BOMId=@BOMId 
        
                ";

                sqlText += @"       order by OHLineNo";

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;
                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;

                #endregion

                #region param

                if (BOMId == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = System.DBNull.Value;
                    }

                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@BOMId"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@BOMId", BOMId);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@BOMId"].Value = BOMId;
                    }
                }



                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);


            }
            #endregion

            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchOHNew", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchOHNew", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] ServiceInsert(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            string VATName = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            int IDExist = 0;
            int BOMLineNo = 0;
            int nextBOMId = 0;
            string savedBOM = string.Empty;
            DataTable BOMDT = new DataTable("BOM");

            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                transaction = currConn.BeginTransaction(MessageVM.spMsgMethodNameInsert);

                #endregion open connection and transaction

                #region Insert into Details(Insert complete in Header)

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameInsert, MessageVM.spMsgNoDataToSave);
                }



                #endregion Validation for Detail

                #region Insert Detail Table

                foreach (var Item in Details.ToList())
                {


                    #region Fiscal Year Check

                    string transactionDate = Item.EffectDate;
                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region Insert BOM Table


                    decimal LastNBRPrice = 0;
                    decimal LastNBRWithSDAmount = 0;
                    decimal LastMarkupAmount = 0;
                    decimal LastSDAmount = 0;

                    #region Find Last Declared NBRPrice

                    var vFinishItemNo = Item.ItemNo;
                    var vEffectDate = Item.EffectDate;
                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRPrice,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName";

                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }

                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRWithSDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRWithSDAmount.Transaction = transaction;
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                    if (objLastNBRWithSDAmount != null)
                    {
                        LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                    }
                    sqlText = "";
                    sqlText += "select top 1 isnull(SDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                    if (objLastSDAmount != null)
                    {
                        LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                    }

                    sqlText = "";
                    sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastMarkupAmount.Transaction = transaction;
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                    if (objLastMarkupAmount != null)
                    {
                        LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                    }

                    #endregion Find Last Declared NBRPrice

                    ////////////////////

                    #region Find Transaction Exist

                    #region Find Transaction Exist

                    sqlText = "";
                    sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@vFinishItemNo  ";
                    sqlText += " AND EffectDate=@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                    cmdFindBOMId.Transaction = transaction;
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    IDExist = (int)cmdFindBOMId.ExecuteScalar();

                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        "Price declaration for this item already exist in same date.");
                    }

                    #endregion Find Transaction Exist

                    #endregion Find Transaction Exist

                    #region Generate BOMId

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";
                    SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                    cmdGenId.Transaction = transaction;
                    nextBOMId = (int)cmdGenId.ExecuteScalar();

                    if (nextBOMId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        "Sorry,Unable to generate BOMId.");
                    }

                    #endregion Generate BOMId

                    #region Insert only BOM


                    Item.LastNBRPrice = LastNBRPrice;
                    Item.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    Item.LastSDAmount = LastSDAmount;
                    Item.LastMarkupValue = LastMarkupAmount;


                    sqlText = "";
                    sqlText += " insert into BOMs(";
                    sqlText += " BOMId,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " VATRate,";
                    sqlText += " UOM,";
                    sqlText += " SD,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " Comments,";
                    sqlText += " ActiveStatus,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " RawTotal,";
                    sqlText += " PackingTotal,";
                    sqlText += " RebateTotal,";
                    sqlText += " AdditionalTotal,";
                    sqlText += " RebateAdditionTotal,";
                    sqlText += " NBRPrice,";
                    sqlText += " Packetprice,";
                    sqlText += " RawOHCost,";
                    sqlText += " LastNBRPrice,";
                    sqlText += " LastNBRWithSDAmount,";
                    sqlText += " TotalQuantity,";
                    sqlText += " SDAmount,";
                    sqlText += " VATAmount,";
                    sqlText += " WholeSalePrice,";
                    sqlText += " NBRWithSDAmount,";
                    sqlText += " MarkUpValue,";
                    sqlText += " LastMarkUpValue,";
                    sqlText += " LastSDAmount,";
                    sqlText += " LastAmount,";
                    sqlText += " FirstSupplyDate,";
                    sqlText += " BranchId,";
                    sqlText += " Post";

                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMId,";
                    sqlText += "@ItemItemNo,";
                    sqlText += "@ItemEffectDate,";
                    sqlText += "@ItemVATName,";
                    sqlText += "@ItemVATRate,";
                    sqlText += "@ItemUOM,";
                    sqlText += "@ItemSDRate,";
                    sqlText += "@ItemTradingMarkup,";
                    sqlText += "@ItemComments,";
                    sqlText += "@ItemActiveStatus,";
                    sqlText += "@ItemCreatedBy,";
                    sqlText += "@ItemCreatedOn,";
                    sqlText += "@ItemLastModifiedBy,";
                    sqlText += "@ItemLastModifiedOn,";
                    sqlText += "@ItemRawTotal,";
                    sqlText += "@ItemPackingTotal,";
                    sqlText += "@ItemRebateTotal,";
                    sqlText += "@ItemAdditionalTotal,";
                    sqlText += "@ItemRebateAdditionTotal,";
                    sqlText += "@ItemPNBRPrice,";
                    sqlText += "@ItemPPacketPrice,";
                    sqlText += "@ItemRawOHCost,";
                    sqlText += "@LastNBRPrice,";
                    sqlText += "@LastNBRWithSDAmount,";
                    sqlText += "@ItemTotalQuantity,";
                    sqlText += "@ItemSDAmount,";
                    sqlText += "@ItemVatAmount,";
                    sqlText += "@ItemWholeSalePrice,";
                    sqlText += "@ItemNBRWithSDAmount,";
                    sqlText += "@ItemMarkupValue,";
                    sqlText += "@ItemLastMarkupValue,";
                    sqlText += "@LastSDAmount,";
                    sqlText += "@ItemLastSDAmount,";
                    sqlText += "@FirstSupplyDate,";
                    sqlText += "@BranchId,";
                    sqlText += "@ItemPost";


                    sqlText += ")	";




                    SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                    cmdInsMaster.Transaction = transaction;
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", OrdinaryVATDesktop.DateToDate(Item.EffectDate));
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemSDRate", Item.SDRate);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemTradingMarkup", Item.TradingMarkup);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemActiveStatus", Item.ActiveStatus);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemCreatedBy", Item.CreatedBy ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemCreatedOn", OrdinaryVATDesktop.DateToDate(Item.CreatedOn));
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(Item.LastModifiedOn));
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRawTotal", Item.RawTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPackingTotal", Item.PackingTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRebateTotal", Item.RebateTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemAdditionalTotal", Item.AdditionalTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRebateAdditionTotal", Item.RebateAdditionTotal);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPNBRPrice", Item.PNBRPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPPacketPrice", Item.PPacketPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemRawOHCost", Item.RawOHCost);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastNBRPrice", LastNBRPrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastNBRWithSDAmount", LastNBRWithSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemTotalQuantity", Item.TotalQuantity);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemVatAmount", Item.VatAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemWholeSalePrice", Item.WholeSalePrice);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemNBRWithSDAmount", Item.NBRWithSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemMarkupValue", Item.MarkupValue);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastMarkupValue", Item.LastMarkupValue);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@LastSDAmount", LastSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemLastSDAmount", Item.LastSDAmount);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@FirstSupplyDate", OrdinaryVATDesktop.DateToDate(Item.FirstSupplyDate));
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@BranchId", Item.BranchId);
                    cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post);

                    transResult = (int)cmdInsMaster.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    #endregion Insert only BOM

                    savedBOM = savedBOM + nextBOMId + "','";
                }
                savedBOM = savedBOM.Substring(0, savedBOM.Length - 3);

                    #endregion Insert Detail Table

                #endregion Insert into Details(Insert complete in Header)

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
                retResults[1] = MessageVM.spMsgSaveSuccessfully;
                retResults[2] = savedBOM;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (Exception sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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

                FileLogger.Log("BOMDAL", "ServiceInsert", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public string[] ServiceUpdate(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
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

            string VATName = "";

            #endregion Initializ

            #region try

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

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameUpdate, MessageVM.spMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    var vFinishItemNo = Item.ItemNo;
                    var vEffectDate = Item.EffectDate;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region update BOM Table

                    if (Item == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgNoDataToSave);
                    }

                    #region Checking bom exist

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo ";
                    sqlText += " and effectdate=@ItemEffectDate ";
                    sqlText += " and VATName=@VATName";

                    SqlCommand cmdIsExist = new SqlCommand(sqlText, currConn);
                    cmdIsExist.Transaction = transaction;
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int isExist = (int)cmdIsExist.ExecuteScalar();

                    if (isExist <= 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Update not completed, Please save fitst of this item: '" +
                                                        Item.ItemNo + "'");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo ";
                    sqlText += " and effectdate>@ItemEffectDate ";
                    sqlText += " and VATName=@VATName ";
                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate", "Data already posted.You cannot update this data.");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo  ";
                    sqlText += " and effectdate>@ItemEffectDate ";
                    sqlText += " and VATName=@VATName ";

                    SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                    cmdOtherBom.Transaction = transaction;
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    int otherBom = (int)cmdOtherBom.ExecuteScalar();

                    if (otherBom > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                    }

                    #endregion Checking other BOM after this date

                    decimal LastNBRPrice = 0;
                    decimal LastNBRWithSDAmount = 0;
                    decimal LastMarkupAmount = 0;
                    decimal LastSDAmount = 0;

                    #region Find Last Declared NBRPrice


                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRPrice,0) from BOMs WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }

                    sqlText = "";
                    sqlText += "select top 1 isnull(NBRWithSDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRWithSDAmount.Transaction = transaction;
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                    if (objLastNBRWithSDAmount != null)
                    {
                        LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                    }
                    sqlText = "";
                    sqlText += "select top 1 isnull(SDAmount,0) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastSDAmount.Transaction = transaction;
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                    if (objLastSDAmount != null)
                    {
                        LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                    }

                    sqlText = "";
                    sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo  ";
                    sqlText += " AND EffectDate<@vEffectDate ";
                    sqlText += " AND VATName=@VATName ";
                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                    cmdFindLastMarkupAmount.Transaction = transaction;
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                    cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                    if (objLastMarkupAmount != null)
                    {
                        LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                    }


                    #endregion Find Last Declared NBRPrice

                    Item.LastNBRPrice = LastNBRPrice;
                    Item.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    Item.LastSDAmount = LastSDAmount;
                    Item.LastMarkupValue = LastMarkupAmount;

                    #region BOM Master Update

                    sqlText = "";

                    sqlText += " update BOMs set  ";

                    sqlText += " EffectDate             =@ItemEffectDate ,";
                    sqlText += " FirstSupplyDate        =@ItemFirstSupplyDate ,";
                    sqlText += " VATName                =@ItemVATName ,";
                    sqlText += " VATRate                =@ItemVATRate  ,";
                    sqlText += " UOM                    =@ItemUOM ,";
                    sqlText += " SD                     =@ItemSDRate  ,";
                    sqlText += " TradingMarkUp          =@ItemTradingMarkup  ,";
                    sqlText += " Comments               =@ItemComments ,";
                    sqlText += " ActiveStatus           =@ItemActiveStatus ,";
                    sqlText += " LastModifiedBy         =@ItemLastModifiedBy ,";
                    sqlText += " LastModifiedOn         =@ItemLastModifiedOn ,";
                    sqlText += " RawTotal               =@ItemRawTotal  ,";
                    sqlText += " PackingTotal           =@ItemPackingTotal  ,";
                    sqlText += " RebateTotal            =@ItemRebateTotal  ,";
                    sqlText += " AdditionalTotal        =@ItemAdditionalTotal  ,";
                    sqlText += " RebateAdditionTotal    =@ItemRebateAdditionTotal  ,";
                    sqlText += " NBRPrice               =@ItemPNBRPrice  ,";
                    sqlText += " PacketPrice            =@ItemPPacketPrice  ,";
                    sqlText += " RawOHCost              =@ItemRawOHCost  ,";
                    sqlText += " LastNBRPrice           =@LastNBRPrice  ,";
                    sqlText += " LastNBRWithSDAmount    =@LastNBRWithSDAmount  ,";
                    sqlText += " TotalQuantity          =@ItemTotalQuantity , ";
                    sqlText += " SDAmount               =@ItemSDAmount , ";
                    sqlText += " VATAmount              =@ItemVatAmount , ";
                    sqlText += " WholeSalePrice         =@ItemWholeSalePrice , ";
                    sqlText += " NBRWithSDAmount        =@ItemNBRWithSDAmount , ";
                    sqlText += " MarkUpValue            =@ItemMarkupValue , ";
                    sqlText += " LastMarkUpValue        =@ItemLastMarkupValue , ";
                    sqlText += " LastSDAmount           =@LastSDAmount , ";
                    sqlText += " LastAmount             =@ItemLastAmount , ";
                    sqlText += " Post                   =@ItemPost ";
                    sqlText += " where FinishItemNo     =@ItemItemNo  ";
                    sqlText += " and effectdate          =@ItemEffectDate";
                    sqlText += " and VATName             =@VATName";

                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;

                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", OrdinaryVATDesktop.DateToDate(Item.EffectDate));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemFirstSupplyDate", OrdinaryVATDesktop.DateToDate(Item.FirstSupplyDate));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATRate", Item.VATRate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemUOM", Item.UOM ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemSDRate", Item.SDRate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemTradingMarkup", Item.TradingMarkup);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemComments", Item.Comments ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemActiveStatus", Item.ActiveStatus ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(Item.LastModifiedOn));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRawTotal", Item.RawTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPackingTotal", Item.PackingTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRebateTotal", Item.RebateTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemAdditionalTotal", Item.AdditionalTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRebateAdditionTotal", Item.RebateAdditionTotal);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPNBRPrice", Item.PNBRPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPPacketPrice", Item.PPacketPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemRawOHCost", Item.RawOHCost);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastNBRPrice", LastNBRPrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastNBRWithSDAmount", LastNBRWithSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemTotalQuantity", Item.TotalQuantity);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemSDAmount", Item.SDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVatAmount", Item.VatAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemWholeSalePrice", Item.WholeSalePrice);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemNBRWithSDAmount", Item.NBRWithSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemMarkupValue", Item.MarkupValue);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastMarkupValue", Item.LastMarkupValue);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@LastSDAmount", LastSDAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastAmount", Item.LastAmount);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPost", Item.Post ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", OrdinaryVATDesktop.DateToDate(Item.EffectDate));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);

                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #region Remove row at BOMs

                    sqlText = "";
                    sqlText += " SELECT  distinct BOMId";
                    sqlText += " from BOMs where FinishItemNo =@ItemItemNo and effectdate=@ItemEffectDate and VATName=@VATName";

                    DataTable dt = new DataTable("Previous");
                    SqlCommand cmdService = new SqlCommand(sqlText, currConn);
                    cmdService.Transaction = transaction;
                    cmdService.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdService.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdService.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    SqlDataAdapter dta = new SqlDataAdapter(cmdService);
                    dta.Fill(dt);
                    foreach (DataRow pBomID in dt.Rows)
                    {
                        var p = pBomID["BOMId"].ToString();
                        var tt = Details.Count(x => x.BOMId.Trim() == p.Trim());
                        if (tt == 0)
                        {
                            sqlText = "";
                            sqlText += " delete FROM BOMs ";
                            sqlText += " WHERE BOMId =@p";
                            SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                            cmdInsDetail.Transaction = transaction;
                            cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);

                            transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        }

                    }

                    #endregion Remove row at BOMs

                    #endregion Update Detail Table

                    #endregion  Update into Details(Update complete in Header)
                }

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
                retResults[1] = MessageVM.spMsgUpdateSuccessfully;

                #endregion SuccessResult

            }

            #endregion

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "ServiceUpdate", ex.ToString() + "\n" + sqlText);

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

        public string[] ServicePost(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
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

            string VATName = "";

            #endregion Initializ

            #region try

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

                #region Validation for Detail

                if (Details.Count() <= 0)
                {
                    throw new ArgumentNullException(MessageVM.spMsgMethodNameUpdate, MessageVM.spMsgNoDataToUpdate);
                }


                #endregion Validation for Detail

                #region Update Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year

                    #region CheckVATName

                    VATName = Item.VATName;
                    if (string.IsNullOrEmpty(VATName))
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgVatNameNotFound);

                    }

                    #endregion CheckVATName

                    #region update BOM Table

                    if (Item == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgNoDataToSave);
                    }

                    #region Checking bom exist

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where BOMId =@ItemBOMId";

                    SqlCommand cmdIsExist = new SqlCommand(sqlText, currConn);
                    cmdIsExist.Transaction = transaction;
                    cmdIsExist.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);

                    int isExist = (int)cmdIsExist.ExecuteScalar();

                    if (isExist <= 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Post not completed, Please save fitst of this item: '" +
                                                        Item.ItemNo + "'");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where BOMId =@ItemBOMId";
                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemBOMId", Item.BOMId);

                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate", "Data already posted.You cannot update this data.");
                    }

                    #endregion Checking other BOM after this date

                    #region Checking other BOM after this date

                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where FinishItemNo =@ItemItemNo ";
                    sqlText += " and effectdate>@ItemEffectDate ";
                    sqlText += " and VATName=@VATName ";

                    SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                    cmdOtherBom.Transaction = transaction;
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                    int otherBom = (int)cmdOtherBom.ExecuteScalar();

                    if (otherBom > 0)
                    {
                        throw new ArgumentNullException("BOMUpdate",
                                                        "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                    }

                    #endregion Checking other BOM after this date

                    #region BOM Master Update

                    sqlText = "";

                    sqlText += " update BOMs set  ";
                    sqlText += " LastModifiedBy=@ItemLastModifiedBy,";
                    sqlText += " LastModifiedOn=@ItemLastModifiedOn ,";
                    sqlText += " Post=@ItemPost ";

                    sqlText += " where ";
                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate ";
                    sqlText += " AND VATName=@ItemVATName ";


                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedBy", Item.LastModifiedBy);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(DateTime.Now.ToString()));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemPost", "Y");
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", OrdinaryVATDesktop.DateToDate(Item.EffectDate));
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName);


                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion Update Detail Table

                    #endregion  Update into Details(Update complete in Header)

                }

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
                retResults[1] = "Successfully Posted";

                #endregion SuccessResult

            }

            #endregion

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //throw sqlex;
            //}
            catch (Exception ex)
            {
                transaction.Rollback();
                retResults[0] = "Fail";
                retResults[1] = ex.Message;

                FileLogger.Log("BOMDAL", "ServicePost", ex.ToString() + "\n" + sqlText);

                return retResults;

                //throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] ServiceDelete(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            string sqlText = "";

            #endregion Initializ

            #region try

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

                #region Delete Detail Table

                foreach (var Item in Details.ToList())
                {

                    #region Validation for Fiscal Year

                    #region Fiscal Year Check

                    string transactionYearCheck = Convert.ToDateTime(Item.EffectDate).ToString("yyyy-MM-dd");
                    if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                        Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                    {

                        #region YearLock

                        sqlText = "";

                        sqlText +=
                            "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                            " where '" + transactionYearCheck + "' between PeriodStart and PeriodEnd";

                        DataTable dataTable = new DataTable("ProductDataT");
                        SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                        cmdIdExist.Transaction = transaction;
                        SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                        reportDataAdapt.Fill(dataTable);

                        if (dataTable == null)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }

                        else if (dataTable.Rows.Count <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else
                        {
                            if (dataTable.Rows[0]["MLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
                            }
                            else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearisLock);
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
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }

                        else if (dtYearNotExist.Rows.Count < 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                        else
                        {
                            if (Convert.ToDateTime(transactionYearCheck) <
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                                ||
                                Convert.ToDateTime(transactionYearCheck) >
                                Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                            {
                                throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                                MessageVM.msgFiscalYearNotExist);
                            }
                        }

                        #endregion YearNotExist

                    }


                    #endregion Fiscal Year CHECK

                    #endregion Validation for Fiscal Year


                    sqlText = "";
                    sqlText = "select count(bomid) from boms ";
                    sqlText += " where ";

                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate";
                    sqlText += " AND VATName=@ItemVATName ";

                    sqlText += " and Post='Y'";

                    SqlCommand cmdIsPosted = new SqlCommand(sqlText, currConn);
                    cmdIsPosted.Transaction = transaction;
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdIsPosted.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName);


                    int isPosted = (int)cmdIsPosted.ExecuteScalar();

                    if (isPosted > 0)
                    {
                        throw new ArgumentNullException("BOMDelete", "Data already posted.You cannot delete this data.");
                    }

                    #region BOM Master Delete

                    sqlText = "";

                    sqlText += " Delete BOMs ";
                    sqlText += " where ";
                    sqlText += " FinishItemNo=@ItemItemNo ";
                    sqlText += " and EffectDate=@ItemEffectDate ";
                    sqlText += " AND VATName=@ItemVATName ";

                    SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                    cmdMasterUpdate.Transaction = transaction;
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemItemNo", Item.ItemNo ?? Convert.DBNull);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemEffectDate", Item.EffectDate);
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ItemVATName", Item.VATName ?? Convert.DBNull);


                    transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                        MessageVM.PurchasemsgUpdateNotSuccessfully);
                    }

                    #endregion BOM Master Delete

                }

                #endregion Delete Detail Table

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
                retResults[1] = "Successfully Deleted.";

                #endregion SuccessResult

            }
            #endregion

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                transaction.Rollback();

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BOMDAL", "ServiceDelete", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "ServiceDelete", ex.ToString() + "\n" + sqlText);

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


        #region SearchInputValues

        public DataTable SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post,
                                           string FinishItemNo, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";
            DataTable dataTable = new DataTable("BOM");

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

                sqlText =
                    @"
                        select distinct b.BOMId, b.FinishItemNo,p.ProductCode,p.ProductName,
                        b.EffectDate,b.VATName,b.NBRPrice,b.UOM
                        ,isnull(NULLIF(b.Comments, ''),'-')Comments
                        ,isnull(b.SD,0)SD,isnull(b.VATRate,0)VATRate,isnull(b.TradingMarkUp,0)TradingMarkUp,b.Post

                        from boms b 
						left outer join
                        products P on b.FinishItemNo =p.ItemNo 
						Inner join (
                                        select VATName,FinishItemNo, max(EffectDate) effDate from BOMS 

                                        group by VATName, FinishItemNo ) v
                                        on b.VATName = v.VATName
                                        and b.EffectDate = v.effDate
                                        and b.FinishItemNo=v.FinishItemNo
                                        and (b.FinishItemNo = @FinishItemNo or @FinishItemNo is null)";

                if (!string.IsNullOrEmpty(post))
                {
                    sqlText += "  AND (b.post =@post)";

                }
                sqlText += " order by b.FinishItemNo asc,EffectDate desc";

                #endregion

                #region SQL Command

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;

                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;
                objCommCBOM.Parameters.AddWithValue("@post", post);

                if (FinishItemNo == "")
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemNo"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@FinishItemNo", System.DBNull.Value);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemNo"].Value = System.DBNull.Value;
                    }
                }
                else
                {
                    if (!objCommCBOM.Parameters.Contains("@FinishItemNo"))
                    {
                        objCommCBOM.Parameters.AddWithValue("@FinishItemNo", FinishItemNo);
                    }
                    else
                    {
                        objCommCBOM.Parameters["@FinishItemNo"].Value = FinishItemNo;
                    }
                }

                //if (EffectDate == "")
                //{
                //    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                //    {
                //        objCommCBOM.Parameters.AddWithValue("@EffectDate", System.DBNull.Value);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@EffectDate"].Value = System.DBNull.Value;
                //    }
                //}
                //else
                //{
                //    if (!objCommCBOM.Parameters.Contains("@EffectDate"))
                //    {
                //        objCommCBOM.Parameters.AddWithValue("@EffectDate", EffectDate);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@EffectDate"].Value = EffectDate;
                //    }
                //}
                //if (VATName == "")
                //{
                //    if (!objCommCBOM.Parameters.Contains("@VATName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValue("@VATName", System.DBNull.Value);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@VATName"].Value = System.DBNull.Value;
                //    }
                //}
                //else
                //{
                //    if (!objCommCBOM.Parameters.Contains("@VATName"))
                //    {
                //        objCommCBOM.Parameters.AddWithValue("@VATName", VATName);
                //    }
                //    else
                //    {
                //        objCommCBOM.Parameters["@VATName"].Value = VATName;
                //    }
                //}

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchInputValues", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchInputValues", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }

        #endregion

        public DataTable SearchServicePrice(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("BOM");

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

                sqlText = "";
                sqlText +=
                    "           SELECT b.BOMId,b.FinishItemNo ItemNo,p.ProductCode PCode,p.ProductName ProductName,";
                sqlText += " b.UOM UOM,b.NBRPrice BasePrice,b.TradingMarkUp Other,b.MarkUpValue OtherAmount,";
                sqlText +=
                    " b.sd SDRate,b.SDAmount SDAmount,b.VATRate VATRate,b.VATAmount VATAmount,b.WholeSalePrice SalePrice,";
                sqlText += " b.Comments Comment,p.HSCodeNo HSCodeNo,b.EffectDate EffectDate";
                sqlText += " FROM BOMs b LEFT OUTER JOIN";
                sqlText += " products p ON b.FinishItemNo=p.ItemNo";
                sqlText += " WHERE b.BOMId in(@BOMId)";


                #endregion

                #region SQL Command

                SqlCommand objCommCBOM = new SqlCommand();
                objCommCBOM.Connection = currConn;

                objCommCBOM.CommandText = sqlText;
                objCommCBOM.CommandType = CommandType.Text;
                objCommCBOM.Parameters.AddWithValue("@BOMId", BOMId);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommCBOM);
                dataAdapter.Fill(dataTable);
            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SearchServicePrice", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SearchServicePrice", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        public DataTable CustomerByBomId(string BOMId, SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables


            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM Use Qty");

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

                sqlText = @"
                         select isnull(b.BOMCode,'0')BOMCode,b.CustomerID, isnull(c.CustomerName ,'NA')CustomerName
from BOMs b
left outer join Customers c on b.CustomerID=c.CustomerID
where BOMId=@BOMId";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!objComm.Parameters.Contains("@BOMId"))
                {
                    objComm.Parameters.AddWithValue("@BOMId", BOMId);
                }
                else
                {
                    objComm.Parameters["@BOMId"].Value = BOMId;
                }


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BOMDAL", "CustomerByBomId", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "CustomerByBomId", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }

        public DataTable UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Objects & Variables


            SqlConnection currConn = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("BOM Use Qty");

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

                sqlText = @"
                            SELECT b.effectdate, fp.ProductCode FCode,fp.ProductName FProName,Rp.ProductName RProName, 
isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity UseQuantity,
isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity WastageQuantity,
isnull(isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity +isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity,0) TotalUseQuantity
,isnull(isnull(Rp.QuantityInHand,0)+isnull(Rp.OpeningBalance,0),0) QuantityInHand
,isnull(isnull(Rp.QuantityInHand,0)+isnull(Rp.OpeningBalance,0),0)-
isnull(isnull(isnull(UOMUQty,b.UseQuantity),0)*@Quantity +isnull(isnull(UOMWQty,b.WastageQuantity),0)*@Quantity,0) Rest

from boms b inner join
(select distinct FinishItemNo, 
max(EffectDate)EffectDate from boms
group by FinishItemNo) M on
b.FinishItemNo=m.FinishItemNo and b.EffectDate<=m.EffectDate  LEFT OUTER JOIN
products RP ON b.RawItemNo=Rp.ItemNo LEFT OUTER JOIN
products FP ON b.finishItemNo =fp.ItemNo
WHERE b.FinishItemNo=@FinishItemNo AND 
b.EffectDate<=@EffectDate 
ORDER BY Rp.ProductName";

                SqlCommand objComm = new SqlCommand();
                objComm.Connection = currConn;
                objComm.CommandText = sqlText;
                objComm.CommandType = CommandType.Text;

                if (!objComm.Parameters.Contains("@FinishItemNo"))
                {
                    objComm.Parameters.AddWithValue("@FinishItemNo", FinishItemNo);
                }
                else
                {
                    objComm.Parameters["@FinishItemNo"].Value = FinishItemNo;
                }

                if (!objComm.Parameters.Contains("@EffectDate"))
                {
                    objComm.Parameters.AddWithValue("@EffectDate", EffectDate);
                }
                else
                {
                    objComm.Parameters["@EffectDate"].Value = EffectDate;
                }


                if (!objComm.Parameters.Contains("@Quantity"))
                {
                    objComm.Parameters.AddWithValue("@Quantity", Quantity);
                }
                else
                {
                    objComm.Parameters["@Quantity"].Value = Quantity;
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objComm);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #endregion
            #region catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "UseQuantityDT", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "UseQuantityDT", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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

        #region web methods

        public List<BOMNBRVM> SelectAllList(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMNBRVM> VMs = new List<BOMNBRVM>();
            BOMNBRVM vm;
            #endregion
            try
            {
                #region sql statement
                #region SqlExecution

                currConn = VcurrConn;
                transaction = Vtransaction;


                DataTable dt = SelectAll(BOMId, conditionFields, conditionValues, currConn, transaction, null, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new BOMNBRVM();

                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.BOMId = dr["BOMId"].ToString();
                        vm.ItemNo = dr["FinishItemNo"].ToString();
                        //vm.EffectDate = dr["EffectDate"].ToString();
                        vm.EffectDate = OrdinaryVATDesktop.DateTimeToDate(dr["EffectDate"].ToString());
                        vm.VATName = dr["VATName"].ToString();
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.SDRate = Convert.ToDecimal(dr["SD"].ToString());
                        vm.TradingMarkup = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RawTotal = Convert.ToDecimal(dr["RawTotal"].ToString());
                        vm.PackingTotal = Convert.ToDecimal(dr["PackingTotal"].ToString());
                        vm.RebateTotal = Convert.ToDecimal(dr["RebateTotal"].ToString());
                        vm.AdditionalTotal = Convert.ToDecimal(dr["AdditionalTotal"].ToString());
                        vm.RebateAdditionTotal = Convert.ToDecimal(dr["RebateAdditionTotal"].ToString());
                        vm.PNBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.PPacketPrice = Convert.ToDecimal(dr["PacketPrice"].ToString());
                        vm.RawOHCost = Convert.ToDecimal(dr["RawOHCost"].ToString());
                        vm.LastNBRPrice = Convert.ToDecimal(dr["LastNBRPrice"].ToString());
                        vm.LastNBRWithSDAmount = Convert.ToDecimal(dr["LastNBRWithSDAmount"].ToString());
                        vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                        vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                        vm.VatAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                        vm.WholeSalePrice = Convert.ToDecimal(dr["WholeSalePrice"].ToString());
                        vm.NBRWithSDAmount = Convert.ToDecimal(dr["NBRWithSDAmount"].ToString());
                        vm.MarkupValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                        vm.LastMarkupValue = Convert.ToDecimal(dr["LastMarkUpValue"].ToString());
                        vm.LastSDAmount = Convert.ToDecimal(dr["LastSDAmount"].ToString());
                        vm.LastAmount = Convert.ToDecimal(dr["LastAmount"].ToString());
                        vm.Post = dr["Post"].ToString();
                        vm.UOM = dr["UOM"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.FinishItemName = dr["ProductName"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.FinishItemCode = dr["ProductCode"].ToString();
                        vm.FinishCategory = dr["CategoryID"].ToString();
                        vm.FirstSupplyDate = dr["FirstSupplyDate"].ToString();
                        vm.AutoIssue = dr["AutoIssue"].ToString();
                        vm.MasterComments = dr["MasterComments"].ToString();
                        vm.SubmittedFilePath = dr["SubmittedFilePath"].ToString();
                        vm.SubmittedFileName = dr["SubmittedFileName"].ToString();
                        vm.ApprovedFilePath = dr["ApprovedFilePath"].ToString();
                        vm.ApprovedFileName = dr["ApprovedFileName"].ToString();


                        VMs.Add(vm);
                    }
                    catch (Exception e) { }
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
                FileLogger.Log("BOMDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        //
        public DataTable SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
                #region SQL Text
                if (count == "All")
                {
                    sqlText = @"SELECT";
                }
                else
                {
                    sqlText = @"SELECT top " + count + " ";
                }
                sqlText += @"

 pc.IsRaw ProductType
,bm.BOMId
,bm.FinishItemNo
,p.ProductName
,p.ProductCode
,c.CustomerName
,ISNULL(bm.ReferenceNo,'NA') ReferenceNo
,isnull(bm.FirstSupplyDate,'1900/01/01') FirstSupplyDate
,bm.EffectDate
,bm.VATName
,isnull(bm.VATRate,0)VATRate
,isnull(bm.SD,0)SD
,isnull(bm.TradingMarkUp,0)TradingMarkUp
,bm.Comments
,bm.ActiveStatus
,bm.CreatedBy
,bm.CreatedOn
,bm.LastModifiedBy
,bm.LastModifiedOn
,isnull(bm.RawTotal,0)RawTotal
,isnull(bm.PackingTotal,0)PackingTotal
,isnull(bm.RebateTotal,0)RebateTotal
,isnull(bm.AdditionalTotal,0)AdditionalTotal
,isnull(bm.RebateAdditionTotal,0)RebateAdditionTotal
,isnull(bm.NBRPrice,0)NBRPrice
,isnull(bm.PacketPrice,0)PacketPrice
,bm.RawOHCost
,isnull(bm.LastNBRPrice,0)LastNBRPrice
,isnull(bm.LastNBRWithSDAmount,0)LastNBRWithSDAmount
,isnull(bm.TotalQuantity,0)TotalQuantity
,isnull(bm.SDAmount,0)SDAmount
,isnull(bm.VATAmount,0)VATAmount
,isnull(bm.WholeSalePrice,0)WholeSalePrice
,isnull(bm.NBRWithSDAmount,0)NBRWithSDAmount
,isnull(bm.MarkUpValue,0)MarkUpValue
,isnull(bm.LastMarkUpValue,0)LastMarkUpValue
,isnull(bm.LastSDAmount,0)LastSDAmount
,isnull(bm.LastAmount,0)LastAmount
,bm.Post
,bm.UOM
,bm.CustomerID
,p.CategoryID
,p.HSCodeNo
, isnull(bm.UOMn,bm.UOM)UOMn
,isnull(bm.BranchId, 0) BranchId
,isnull(bm.AutoIssue, 'Y') AutoIssue
,isnull(bm.SubmittedFilePath, '') SubmittedFilePath
,isnull(bm.SubmittedFileName, '') SubmittedFileName
,isnull(bm.ApprovedFilePath, '') ApprovedFilePath
,isnull(bm.ApprovedFileName, '') ApprovedFileName

,bm.MasterComments

FROM BOMs  bm left outer join Products p 
on bm.FinishItemNo=p.ItemNo left outer join Customers c
on bm.CustomerID=c.CustomerID
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE  1=1 
";
                #endregion

                sqlTextCount += @" select count(bm.BOMId)RecordCount

FROM BOMs  bm left outer join Products p 
on bm.FinishItemNo=p.ItemNo left outer join Customers c
on bm.CustomerID=c.CustomerID
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE  1=1 
";

                if (BOMId != null && BOMId != "0")
                {
                    sqlTextParameter += @" and bm.BOMId=@BOMId";
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
                //    if (!string.IsNullOrEmpty(likeVM.FinishItemName))
                //    {
                //        sqlText += " AND p.ProductName like @ProductName ";
                //    }
                //}
                sqlTextOrderBy += " order by bm.EffectDate desc, pc.IsRaw, p.ProductName ";

                #endregion SqlText
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
                //    if (!string.IsNullOrEmpty(likeVM.FinishItemName))
                //    {
                //        da.SelectCommand.Parameters.AddWithValue("@ProductName", "%" + likeVM.FinishItemName + "%");
                //    }
                //}

                if (BOMId != null)
                {
                    da.SelectCommand.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                da.Fill(ds);

                #endregion SqlExecution

                ////if (Vtransaction == null && transaction != null)
                ////{
                ////    transaction.Commit();
                ////}
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
                FileLogger.Log("BOMDAL", "SelectAll", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                ////if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                ////{
                ////    currConn.Close();
                ////}
            }
            #endregion
            return dt;
        }



        public List<BOMItemVM> SelectAllItems(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMItemVM> VMs = new List<BOMItemVM>();
            BOMItemVM vm;
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
 braw.BOMRawId
,p.ProductName
,braw.BOMId
,braw.BOMLineNo
,braw.FinishItemNo
,braw.RawItemNo
,braw.RawItemType
,braw.EffectDate
,braw.VATName
,isnull(braw.UseQuantity,0) UseQuantity 
,isnull(braw.WastageQuantity,0) WastageQuantity 
,isnull(braw.Cost,0) Cost 
,braw.UOM
,isnull(braw.VATRate,0)VATRate 
,isnull(braw.VATAmount,0)VATAmount 
,isnull(braw.SD,0)SD 
,isnull(braw.SDAmount,0)SDAmount 
,isnull(braw.TradingMarkUp,0)TradingMarkUp 
,isnull(braw.RebateRate,0)RebateRate 
,isnull(braw.MarkUpValue,0)MarkUpValue 
,braw.CreatedBy
,braw.CreatedOn
,braw.LastModifiedBy
,braw.LastModifiedOn
,braw.UnitCost
,braw.UOMn
,isnull(braw.UOMc,0)UOMc 
,isnull(braw.UOMPrice,0)UOMPrice 
,isnull(braw.UOMUQty,0)UOMUQty 
,isnull(braw.UOMWQty,0)UOMWQty 
,isnull(braw.TotalQuantity,0)TotalQuantity 
,braw.Post
,braw.PBOMId
,braw.PInvoiceNo
,braw.TransactionType
,braw.CustomerID
,braw.IssueOnProduction
,p.ProductCode

from BOMRaws braw 
left outer join Products p on braw.RawItemNo=p.ItemNo where 1=1 ";

                if (BOMId != null)
                {
                    sqlText += @" and braw.BOMId=@BOMId";
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
                        sqlText += " AND braw." + conditionFields[i] + "=@" + cField;
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
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }

                if (BOMId != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BOMItemVM();

                    vm.BOMRawId = dr["BOMRawId"].ToString();
                    vm.BOMId = dr["BOMId"].ToString();
                    vm.BOMLineNo = dr["BOMLineNo"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.RawItemNo = dr["RawItemNo"].ToString();
                    vm.RawItemType = dr["RawItemType"].ToString();
                    vm.EffectDate = dr["EffectDate"].ToString();
                    vm.UseQuantity = Convert.ToDecimal(dr["UseQuantity"].ToString());
                    vm.WastageQuantity = Convert.ToDecimal(dr["WastageQuantity"].ToString());
                    vm.Cost = Convert.ToDecimal(dr["Cost"].ToString());
                    vm.UOM = dr["UOM"].ToString();
                    vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                    //vm.VATAmount = dr["VATAmount"].ToString();
                    vm.SD = Convert.ToDecimal(dr["SD"].ToString());
                    vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                    vm.TradingMarkUp = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                    vm.RebateRate = Convert.ToDecimal(dr["RebateRate"].ToString());
                    vm.MarkUpValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                    //vm.CreatedBy = dr["CreatedBy"].ToString();
                    //vm.CreatedOn = dr["CreatedOn"].ToString();
                    //vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    //vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.UnitCost = Convert.ToDecimal(dr["UnitCost"].ToString());
                    vm.UOMn = dr["UOMn"].ToString();
                    vm.UOMc = Convert.ToDecimal(dr["UOMc"].ToString());
                    vm.UOMPrice = Convert.ToDecimal(dr["UOMPrice"].ToString());
                    vm.UOMUQty = Convert.ToDecimal(dr["UOMUQty"].ToString());
                    vm.UOMWQty = Convert.ToDecimal(dr["UOMWQty"].ToString());
                    vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.PBOMId = dr["PBOMId"].ToString();
                    vm.PInvoiceNo = dr["PInvoiceNo"].ToString();
                    vm.TransactionType = dr["TransactionType"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.IssueOnProduction = dr["IssueOnProduction"].ToString();
                    vm.RawItemName = dr["ProductName"].ToString();
                    vm.RawItemCode = dr["ProductCode"].ToString();
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
                FileLogger.Log("BOMDAL", "SelectAllItems", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAllItems", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<BOMOHVM> SelectAllOverheads(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMOHVM> VMs = new List<BOMOHVM>();
            BOMOHVM vm;
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
 bco.BOMOverHeadId
,bco.BOMId
,bco.OHLineNo
,bco.HeadName HeadNameOld
,bco.FinishItemNo
,bco.EffectDate
,bco.VATName
,isnull(bco.HeadAmount,0) HeadAmount 
,bco.CreatedBy
,bco.CreatedOn
,bco.LastModifiedBy
,bco.LastModifiedOn
,bco.Info5
,isnull(bco.RebatePercent,0) RebatePercent 
,isnull(bco.RebateAmount,0) RebateAmount 
,isnull(bco.AdditionalCost,0) AdditionalCost 
,bco.Post
,bco.HeadID
,bco.CustomerID
,p.ProductCode
,p.ProductName HeadName
from BOMCompanyOverhead bco left outer join Products p on bco.HeadID=p.ItemNo 
where 1=1 
";

                if (BOMId != null)
                {
                    sqlText += @" and bco.BOMId=@BOMId";
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
                        objComm.Parameters.AddWithValueAndNullHandle("@" + cField, conditionValues[j]);
                    }
                }

                if (BOMId != null)
                {
                    objComm.Parameters.AddWithValueAndNullHandle("@BOMId", BOMId);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new BOMOHVM();

                    vm.BOMId = dr["BOMId"].ToString();
                    vm.OHLineNo = dr["OHLineNo"].ToString();
                    vm.HeadName = dr["HeadName"].ToString();
                    vm.FinishItemNo = dr["FinishItemNo"].ToString();
                    vm.EffectDate = dr["EffectDate"].ToString();
                    vm.HeadAmount = Convert.ToDecimal(dr["HeadAmount"].ToString());
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.RebatePercent = Convert.ToDecimal(dr["RebatePercent"].ToString());
                    vm.RebateAmount = Convert.ToDecimal(dr["RebateAmount"].ToString());
                    vm.AdditionalCost = Convert.ToDecimal(dr["AdditionalCost"].ToString());
                    vm.Post = dr["Post"].ToString();
                    vm.HeadID = dr["HeadID"].ToString();
                    vm.CustomerID = dr["CustomerID"].ToString();
                    vm.OHCode = dr["ProductCode"].ToString();

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
                FileLogger.Log("BOMDAL", "SelectAllOverheads", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAllOverheads", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public string[] BOMPreInsert(BOMNBRVM vm, SysDBInfoVMTemp connVM = null)
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
            string PostStatus = "";
            int IDExist = 0;
            //string VATName = "";
            int nextBOMId = 0;

            #endregion Initializ
            #region Try
            try
            {
                #region Ready Data to Insert

                int countRaw = 0;
                int countOh = 0;
                Decimal rawTotal = 0;
                Decimal packingTotal = 0;
                Decimal rebateTotal = 0;
                Decimal rebateAdditionalTotal = 0;
                Decimal additionalTotal = 0;
                Decimal vMarkupValue;
                Decimal vSDAmount;
                Decimal vVatAmount;

                #region Raw Material

                foreach (var rawItem in vm.Items)
                {
                    vMarkupValue = rawItem.Cost * vm.TradingMarkup / 100;
                    vSDAmount = (rawItem.Cost + vMarkupValue) * vm.SDRate / 100;
                    vVatAmount = (rawItem.Cost + vMarkupValue + vSDAmount) * vm.VATRate / 100;

                    rawItem.BOMLineNo = countRaw.ToString();
                    rawItem.VATRate = vm.VATRate;
                    rawItem.VatAmount = vVatAmount;
                    rawItem.VatName = vm.VATName;
                    rawItem.SD = vm.SDRate;
                    rawItem.SDAmount = vSDAmount;
                    rawItem.TradingMarkUp = vm.TradingMarkup;
                    rawItem.CreatedBy = vm.CreatedBy;
                    rawItem.CreatedOn = vm.CreatedOn;
                    rawItem.LastModifiedBy = vm.LastModifiedBy;
                    rawItem.LastModifiedOn = vm.LastModifiedOn;
                    rawItem.Post = "N";
                    rawItem.CustomerID = vm.CustomerID ?? "0";
                    rawItem.FinishItemNo = vm.ItemNo;
                    if (rawItem.RawItemType != "Pack" && rawItem.RawItemType != "Overhead")
                    {
                        rawTotal += rawItem.Cost;
                    }
                    else if (rawItem.RawItemType == "Pack")
                    {
                        packingTotal += rawItem.Cost;
                    }
                    else if (rawItem.RawItemType == "Overhead")
                    {
                        rebateTotal += rawItem.Cost;
                    }

                    rawItem.BranchId = vm.BranchId;

                    countRaw++;
                }
                #endregion

                #region Overhead - OH


                if (vm.Overheads == null)
                {
                    vm.Overheads = new List<BOMOHVM>();
                }
                //////overhead item
                foreach (var overhead in vm.Overheads)
                {
                    decimal vAdditionalCost = 0;

                    overhead.CreatedBy = vm.CreatedBy;
                    overhead.CreatedOn = vm.CreatedOn;
                    overhead.LastModifiedBy = vm.LastModifiedBy;
                    overhead.LastModifiedOn = vm.LastModifiedOn;
                    overhead.Post = "N";
                    overhead.CustomerID = vm.CustomerID ?? "0";
                    overhead.FinishItemNo = vm.ItemNo;
                    overhead.EffectDate = vm.EffectDate;
                    overhead.OHLineNo = countOh.ToString();
                    overhead.RebateAmount = overhead.HeadAmount - overhead.AdditionalCost;
                    additionalTotal += overhead.AdditionalCost;
                    countOh++;
                    rebateAdditionalTotal += overhead.HeadAmount;
                    overhead.RebatePercent = overhead.AdditionalCost;
                }


                BOMOHVM bomOh1 = new BOMOHVM();
                bomOh1.HeadName = "Margin";
                bomOh1.OHCode = "ovh0";
                bomOh1.HeadID = "ovh0";
                bomOh1.HeadAmount = vm.Margin;
                bomOh1.CreatedBy = vm.CreatedBy;
                bomOh1.CreatedOn = vm.CreatedOn;
                bomOh1.LastModifiedBy = vm.LastModifiedBy;
                bomOh1.LastModifiedOn = vm.LastModifiedOn;
                bomOh1.FinishItemNo = vm.ItemNo;
                bomOh1.EffectDate = vm.EffectDate;
                bomOh1.OHLineNo = "100";
                bomOh1.RebatePercent = 0;
                bomOh1.Post = "N";
                bomOh1.AdditionalCost = bomOh1.HeadAmount;
                vm.Overheads.Add(bomOh1);

                #endregion

                //////////////NBR
                //////vm.CreatedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //////vm.CreatedBy = identity.Name;
                //////vm.LastModifiedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                vm.Post = "N";
                vm.ActiveStatus = "Y";

                var abc = rawTotal + packingTotal + rebateTotal + additionalTotal + vm.Margin;
                vm.RawTotal = rawTotal;
                vm.PackingTotal = packingTotal;
                vm.RebateTotal = rebateTotal;
                vm.RebateAdditionTotal = rebateAdditionalTotal;
                vm.AdditionalTotal = additionalTotal;

                var MarkupValue = abc * vm.TradingMarkup / 100;
                var nbrPrice = abc + MarkupValue;
                var SDAmount = (nbrPrice) * vm.SDRate / 100;
                var VatAmount = (nbrPrice + SDAmount) * vm.VATRate / 100;

                vm.PNBRPrice = nbrPrice;
                vm.SDAmount = SDAmount;
                vm.VatAmount = VatAmount;
                vm.ReceiveCost = rawTotal + packingTotal + rebateTotal + additionalTotal;
                vm.RawOHCost = rawTotal + packingTotal + rebateTotal;
                vm.NBRWithSDAmount = nbrPrice + nbrPrice * vm.SDRate / 100;
                #endregion
                #region Insert Data
                if (vm.Operation.ToLower() == "add")
                {
                    retResults = BOMInsert(vm.Items, vm.Overheads, vm);
                }
                else
                {
                    retResults = BOMUpdate(vm.Items, vm.Overheads, vm);
                }
                #endregion
            }
            #endregion
            #region Catch and Finall

            catch (Exception ex)
            {

                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                retResults[2] = ""; //catch ex

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("BOMDAL", "BOMPreInsert", ex.ToString() + "\n" + sqlText);

                return retResults;
            }
            finally
            {

            }

            #endregion Catch and Finall
            #region Result

            return retResults;

            #endregion Result
        }

        #endregion

        //VcurrConn 25-Aug-2020

        public string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

                #region Old connection

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

                #endregion Old connection

                #region open connection and transaction

                //if (VcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMCompanyOverhead";
                sqlText += " where ";
                sqlText += " HeadID='" + itemNo + "' ";
                sqlText += " and vatname='" + VatName + "' ";
                //sqlText += " and effectdate<='" + effectDate.Date + "'";
                sqlText += " and effectdate<='" + effectDate + "'";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                var tt = "";
                if (cmdBomId.ExecuteScalar() == null)
                {
                    //throw new ArgumentNullException("FindBOMID",
                    //                                "No Price declaration found for this item");
                    BomId = string.Empty;
                }
                else
                {
                    BomId = cmdBomId.ExecuteScalar().ToString();
                }
                //BomId = tt;
                #endregion Last BOMId

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "FindBOMIDOverHead", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "FindBOMIDOverHead", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }

            #endregion

            #region finally

            finally
            {
                if (VcurrConn == null && currConn != null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();

                    }
                }
            }

            #endregion

            #endregion

            #region Results

            return BomId;

            #endregion

        }

        public string[] ImportBOM(DataTable boms, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlTransaction transaction = null;
            SqlConnection connection = null;
            string[] result = new[] { "Fail" };

            #endregion

            #region try

            try
            {
                connection = _dbsqlConnection.GetConnection(connVM);
                connection.Open();
                transaction = connection.BeginTransaction();
                var commonDal = new CommonDAL();

                #region Sql Text

                #region Delete and Update Item BOM Temp

                string deleteTemp = @"delete from BOMTempData
ALTER TABLE BOMTempData  DROP CONSTRAINT PK_BOMTempData;   
ALTER TABLE BOMTempData DROP COLUMN SL;
ALTER TABLE BOMTempData add   SL int CONSTRAINT PK_BOMTempData PRIMARY KEY (SL)  identity(1,1); ";
                //+
                //     " DBCC CHECKIDENT ('BOMTempData', RESEED, 0);  ";

                string updateTable = @"
update BOMTempData set FinishItemNo = Products.ItemNo,FUOMn = Products.UOM
from Products where BOMTempData.FCode = Products.ProductCode and FinishItemNo is null

update BOMTempData set FinishItemNo = Products.ItemNo,FUOMn = Products.UOM
from Products where BOMTempData.FName = Products.ProductName and FinishItemNo is null

update BOMTempData set RawItemNo = Products.ItemNo, 
UOMn = Products.UOM, 
VATRate = Products.VATRate,
TradingMarkUp = Products.TradingMarkUp, SD = Products.SD
from Products where BOMTempData.RCode = Products.ProductCode and RawItemNo is null


update BOMTempData set RawItemNo = Products.ItemNo,
UOMn = Products.UOM, 
VATRate = Products.VATRate,
TradingMarkUp = Products.TradingMarkUp, SD = Products.SD
from Products where BOMTempData.RName = Products.ProductName and RawItemNo is null


update BOMTempData set RawItemType = P.IsRaw from
(select IsRaw,ProductCode 
from Products left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID) P
where P.ProductCode = BOMTempData.RCode

update BOMTempData set RawItemType = P.IsRaw from
(select IsRaw,ProductCode, ProductName 
from Products left outer join ProductCategories on Products.CategoryID = ProductCategories.CategoryID) P
where P.ProductName = BOMTempData.RName and BOMTempData.RawItemType is null

update BOMTempData set CustomerID = Customers.CustomerID 
from Customers where Customers.CustomerCode = BOMTempData.CustomerCode and BOMTempData.CustomerId is null 
and BOMTempData.CustomerCode not in ('-','NA')

update BOMTempData set CustomerID = Customers.CustomerID 
from Customers where Customers.CustomerName = BOMTempData.CustomerName and BOMTempData.CustomerId is null
and BOMTempData.CustomerName not in ('-','NA')


update BOMTempData set UOMc = (case when RUOM = UOMn then 1.00 else UOMc end)

update BOMTempData set BOMTempData.UOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = BOMTempData.UOMn 
and UOMs.UOMTo = BOMTempData.RUOM 

update BOMTempData set FUOMc = (case when FUOM = FUOMn then 1.00 else FUOMc end)

update BOMTempData set BOMTempData.FUOMc = UOMs.Convertion from UOMs 
where  UOMs.UOMFrom = BOMTempData.FUOMn 
and UOMs.UOMTo = BOMTempData.FUOM 
";

                #endregion

                #region Update Calulation

                string updateCalculation =
                    @"
update BOMTempData set UseQuantity = TotalQuantity - WastageQuantity where UseQuantity = 0 or UseQuantity is null

update BOMTempData set RebateRate = Products.RebatePercent 
from Products where Products.ItemNo = BOMTempData.RawItemNo and RebateRate is null

update BOMTempData set MarkUpValue = (Cost * TradingMarkUp) / 100

update BOMTempData set  SDAmount=((Cost+MarkUpValue) *SD)/100
update BOMTempData set  VATAmount=(Cost+SDAmount+MarkUpValue)*VATRate/100

update BOMTempData set UOMPrice = (Cost /(UseQuantity+WastageQuantity)) * UOMc
--vuomPrice = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty));
update BOMTempData set UOMUQty = UseQuantity * UOMc
update BOMTempData set UOMWQty = WastageQuantity * UOMc

--  bomItem.UnitCost = Convert.ToDecimal(vSubTotal) / ((vuomUseQty + vuomWastageQty)) * vuomc;
update BOMTempData set UnitCost = convert(decimal(25,15),cost) / convert( decimal(25,15),(UOMUQty+UOMWQty))  * convert(decimal(25,15),UOMc )
where RawItemType != 'Overhead'



update BOMTempData set Post = 'N'
update BOMTempData set ReferenceNo = '-' where ReferenceNo is null

create table #temp(sl int identity(1,1), FCode varchar(100),FName varchar(500),RefNo varchar(500), BOMId varchar(50))

 insert into #temp(FCode,FName,RefNo) 
 select  distinct FCode,FName,ReferenceNo from BOMTempData

declare @count int  = (select count(SL) from #temp )
declare @i int  = 1

declare @lastId int =  (select isnull(max(CAST(BOMId as int)),0)+1 from BOMS )


while @i <= @count
begin 

	update #temp set BOMId = @lastId where SL = @i 

	set @lastId = @lastId +1
    set @i = @i +1
end

update BOMTempData set BOMId = #temp.BOMId from #temp where #temp.FCode = BOMTempData.FCode and #temp.RefNo = BOMTempData.ReferenceNo and (BOMTempData.BOMId is null or BOMTempData.BOMId = '0');
update BOMTempData set BOMId = #temp.BOMId from #temp where #temp.FName = BOMTempData.FName and #temp.RefNo = BOMTempData.ReferenceNo and (BOMTempData.BOMId is null or BOMTempData.BOMId = '0');



--insert into #temp(FCode) 
-- select  distinct FCode from BOMTempData




set @count  = (select count(SL) from BOMTempData )
set @i   = 1
declare @rid varchar(50)
declare @bomRawId int = (select isnull(max(CAST(BOMRawId as int)),0)+1 from BOMRaws )

declare @overHeadId int = (select isnull(max(CAST(BOMOverHeadId as int)),0)+1 from BOMCompanyOverhead)


while @i <= @count
begin 
	
	set @rid = (select RawItemNo from BOMTempData where SL = @i )

	update BOMTempData set PInvoiceNo = 
	(select TOP 1 PurchaseInvoiceDetails.PurchaseInvoiceNo from PurchaseInvoiceDetails   join 
BOMTempData on PurchaseInvoiceDetails.ItemNo = BOMTempData.RawItemNo
where ItemNo = @rid and PurchaseInvoiceDetails.TransactionType in ('Other','PurchaseCN','Trading','Service','ServiceNS','CommercialImporter','InputService','Import','ServiceImport','ServiceNSImport','TradingImport','InputServiceImport')
order by InvoiceDateTime desc), BOMRawId = @bomRawId, overHeadId = @overHeadId
where SL = @i

	set @i = @i +1;
	set @bomRawId = @bomRawId + 1
	set @overHeadId =  @overHeadId + 1
end
update BOMTempData set OverHead_RebatePercent = 100-RebateRate where RawItemType = 'Overhead'
update BOMTempData set OverHead_AdditionalCost = (Cost*OverHead_RebatePercent)/100 where RawItemType = 'Overhead'
update BOMTempData set OverHead_RebateAmount = (cost-OverHead_AdditionalCost) where RawItemType = 'Overhead'
update BOMTempData set TransactionType = PurchaseInvoiceDetails.TransactionType from PurchaseInvoiceDetails
where PurchaseInvoiceDetails.PurchaseInvoiceNo = PInvoiceNo

update BOMTempData set OverHead_AdditionalCost = Cost, OverHead_RebatePercent = 0, OverHead_RebateAmount = 0 
where RawItemType = 'Overhead' and RawItemNo = 'ovh0'


drop table #temp


select * from BOMTempData
--select * from BOMTempData order by BOMId

";

                #endregion

                #region Select For BUlk Insert

                #region Get BOMRaws

                string getBOMRaws = @"
SELECT  
      [BOMId]
      ,BOMRawId
      ,[FinishItemNo]
      ,[RawItemNo]
      ,[RawItemType]
      ,[EffectDate]
      ,[VATName]
      ,[UseQuantity]
      ,[WastageQuantity]
      ,[Cost]
      ,[RUOM] UOM
      ,[VATRate]
      ,[VATAmount]
      ,[SD]
      ,[SDAmount]
      ,[TradingMarkUp]
      ,[MarkUpValue]
      ,[RebateRate]
      ,[UnitCost]
      ,[UOMn]
      ,[UOMc]
      ,[UOMPrice]
      ,[UOMUQty]
      ,[UOMWQty]
      ,[TotalQuantity]
      ,[Post]
      ,[PBOMId]
      ,[PInvoiceNo]
      ,[TransactionType]
      ,[CustomerID]
      ,[IssueOnProduction]
      ,[BranchId]
  FROM BOMTempData  where RawItemType != 'Overhead'";

                #endregion

                string getOverHead = @"SELECT overHeadId BOMOverHeadId
      ,[BOMId]
      ,0 [OHLineNo]
      ,RName [HeadName]
      ,[FinishItemNo]
      ,[EffectDate]
      ,[VATName]
      ,Cost [HeadAmount] 

      ,OverHead_RebatePercent [RebatePercent]
      ,OverHead_RebateAmount [RebateAmount]
      ,OverHead_AdditionalCost [AdditionalCost]
      ,[Post]
      ,RawItemNo [HeadID]
      ,[CustomerID]
      ,[BranchId]
  FROM BOMTempData where RawItemType = 'Overhead'";

                string getOverheadRaws = @"SELECT  
      [BOMId]
      ,BOMRawId
      ,[FinishItemNo]
      ,[RawItemNo]
      ,[RawItemType]
      ,[EffectDate]
      ,[VATName]
      ,1 [UseQuantity]
      ,0 [WastageQuantity]
      ,OverHead_RebateAmount  [Cost]
      ,'-'  UOM
      ,[VATRate]
      ,[VATAmount]
      ,[SD]
      ,[SDAmount]
      ,[TradingMarkUp]
      ,[MarkUpValue]
      ,[RebateRate]
      ,OverHead_RebateAmount [UnitCost]
      ,'-' [UOMn]
      ,1[UOMc]
      ,OverHead_RebateAmount  [UOMPrice]
      ,1 [UOMUQty]
      ,1 [UOMWQty]
      ,1 [TotalQuantity]
      ,[Post]
      ,[PBOMId]
      ,[PInvoiceNo]
      ,[TransactionType]
      ,[CustomerID]
      ,[IssueOnProduction]
      ,[BranchId]
  FROM BOMTempData  where RawItemType = 'Overhead' and RName != 'Margin'";

                string getBOM = @" select distinct [BOMId]
      ,[FinishItemNo]
      ,[EffectDate]
      ,[VATName]
      ,FUOM UOM
      ,FUOMc UOMc
,FUOMn UOMn
      ,0[VATRate]
      ,0[SD]
      ,0[TradingMarkUp]

      ,'Y'[ActiveStatus]

      ,0[RawTotal]
      ,0[PackingTotal]
      ,0[RebateTotal]
      ,0[AdditionalTotal]
      ,0[RebateAdditionTotal]
      ,0[NBRPrice]
      ,0[PacketPrice]
      ,0[RawOHCost]
      ,0[LastNBRPrice]
      ,0[LastNBRWithSDAmount]
      ,0[TotalQuantity]
      ,0[SDAmount]
      ,0[VATAmount]
      ,0[WholeSalePrice]
      ,0[NBRWithSDAmount]
      ,0[MarkUpValue]
      ,0[LastMarkUpValue]
      ,0[LastSDAmount]
      ,0[LastAmount]
      ,[Post]
      ,[CustomerID]

      ,[FirstSupplyDate]
      ,[BranchId]
      ,[ReferenceNo]
      ,isnull(AutoIssue,'Y')AutoIssue
  FROM BOMTempData";

                #endregion

                #region Update BOM Master Data

                string updateBOMs = @" update BOMS set 
--UOM = Products.UOM, 
VATRate = Products.VATRate,
TradingMarkUp = Products.TradingMarkUp, SD = Products.SD
from Products where BOMS.FinishItemNo = Products.ItemNo and BOMId in (select distinct BOMId from BOMTempData)

update BOMs set RawOHCost=BOMRaws.RawOHCost
from(
select distinct BOMId,sum(Cost)RawOHCost from BOMRaws 
group by BOMId
)BOMRaws
where BOMs.BOMId   in (select distinct BOMId from BOMTempData)

update BOMs set RebateAdditionTotal=BOMRaws.RebateAdditionTotal
from(
select distinct BOMId,sum(HeadAmount)RebateAdditionTotal from BOMCompanyOverhead 
group by BOMId
)BOMRaws
where BOMRaws.BOMId=BOMs.BOMId
and  BOMs.BOMId   in (select distinct BOMId from BOMTempData)


update BOMs set AdditionalTotal=BOMRaws.AdditionalTotal
from(
select distinct BOMId,sum(AdditionalCost)AdditionalTotal from BOMCompanyOverhead 
group by BOMId
)BOMRaws
where BOMRaws.BOMId=BOMs.BOMId
and BOMs.BOMId   in (select distinct BOMId from BOMTempData)

update BOMs set RebateTotal=BOMRaws.RebateTotal
from(
select distinct BOMId,sum(Cost)RebateTotal from BOMRaws 
where RawItemType in('Overhead')
group by BOMId
)BOMRaws
where BOMRaws.BOMId=BOMs.BOMId
and  BOMs.BOMId   in (select distinct BOMId from BOMTempData)


update BOMs set RawTotal=br.RawTotal
from(
select distinct BOMId,sum(Cost)RawTotal from BOMRaws 
where RawItemType in('Raw','Trading','Export','NonInventory','Service(NonStock)')
group by BOMId)br
where br.BOMId=BOMs.BOMId
and  BOMs.BOMId   in (select distinct BOMId from BOMTempData)


update BOMs set RawOHCost=br.RawTotal
from(
select distinct BOMId,sum(Cost)RawTotal from BOMRaws 
group by BOMId)br
where br.BOMId=BOMs.BOMId
and BOMs.BOMId   in (select distinct BOMId from BOMTempData)

update BOMs set PackingTotal=BOMRaws.PackingTotal
from(
select distinct BOMId,sum(Cost)PackingTotal from BOMRaws 
where RawItemType in('Pack','WIP')
group by BOMId)BOMRaws
where BOMRaws.BOMId=BOMs.BOMId
and BOMs.BOMId   in (select distinct BOMId from BOMTempData)

update BOMs set NBRPrice = (RawTotal+PackingTotal+RebateTotal+AdditionalTotal) --+ ((RawTotal+PackingTotal+RebateTotal+AdditionalTotal) * TradingMarkUp /100)
where BOMId in (select distinct BOMId from BOMTempData)

update BOMs set SDAmount = NBRPrice * SD /100
where BOMId in (select distinct BOMId from BOMTempData)

update BOMs set VATAmount = (NBRPrice+SDAmount)*VATRate /100
where BOMId in (select distinct BOMId from BOMTempData)

update BOMs set NBRWithSDAmount = SDAmount+NBRPrice
where BOMId in (select distinct BOMId from BOMTempData)

update BOMs set WholeSalePrice = NBRWithSDAmount+((NBRWithSDAmount*VATRate)/100)
where BOMId in (select distinct BOMId from BOMTempData)


update BOMs set UOMPrice = NBRPrice * UOMc  
where BOMId in (select distinct BOMId from BOMTempData)
";

                #endregion

                #region Validation Queries

                string getFinishItem = @"
select top 1 * from BOMTempData where FinishItemNo is null or FinishItemNo = '0'";

                string getRawItemNo = @"
select top 1 * from BOMTempData where RawItemNo is null or RawItemNo = '0'";

                string effectDateCheck = @"
select top 1 bt.FCode from 
(select distinct FinishItemNo, EffectDate,FCode,isnull(nullif(ReferenceNo,''),'-')ReferenceNo from BOMTempData)bt  join 
(select distinct FinishItemNo, EffectDate,isnull(nullif(ReferenceNo,''),'-')ReferenceNo from BOMs) b 
on bt.FinishItemNo = b.FinishItemNo and bt.ReferenceNo = b.ReferenceNo
where bt.EffectDate = b.EffectDate
";

                string getUOMc =
                    @"select  top 1 RName,RCode,UOMn,RUOM from BOMTempData where isnull(uomc,0) =0 --and RawItemType != 'Overhead'";

                string getFUOMc =
                    @"select  top 1 FName,FCode,FUOMn,FUOM from BOMTempData 
where isnull(FUOMc,0) =0 and RawItemType != 'Overhead' and RawItemType !='Raw'";


                string getZeroQuantity =
                    @"select  top 1 RName,RCode,FCode,FName 
from BOMTempData 
where Cost = 0";

                string getZeroTotalQuantity =
                    @"select top 1 * from BOMTempData where TotalQuantity = 0 or TotalQuantity is null ";

                string getZeroUseQuantity =
                    @"select top 1 * from BOMTempData where UseQuantity = 0 or UseQuantity is null ";


                #endregion
                #endregion

                var cmd = new SqlCommand(deleteTemp, connection, transaction);
                int rows = cmd.ExecuteNonQuery();

                result = commonDal.BulkInsert("BOMTempData", boms, connection, transaction, 10000, null, connVM);

                cmd.CommandText = updateTable;

                //--------------------------------------------------
                //if (result[0].ToLower() == "success")
                //{
                //    transaction.Commit();
                //}
                //return result;
                //--------------------------------------------------

                cmd.ExecuteNonQuery();

                #region Validation

                cmd.CommandText = getFinishItem;
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("Finish Product Not Found " + vtable.Rows[0]["FCode"] + vtable.Rows[0]["FName"]);
                }


                cmd.CommandText = getRawItemNo;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("Raw Product Not Found " + vtable.Rows[0]["RCode"] + " " + vtable.Rows[0]["RName"]);
                }


                cmd.CommandText = getZeroQuantity;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("Quanttity or Cost not found for " + vtable.Rows[0]["FCode"] + " " + vtable.Rows[0]["FName"] + " " + vtable.Rows[0]["RName"] + " " + vtable.Rows[0]["RCode"]);
                }

                #region Check UOMc

                cmd.CommandText = getFUOMc;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("UOM conversion not found of " + vtable.Rows[0]["FCode"] + " " +
                                        vtable.Rows[0]["FName"] + " UOM From  " + vtable.Rows[0]["FUOMn"] + " UOM To  " +
                                        vtable.Rows[0]["FUOM"]);
                }



                cmd.CommandText = getUOMc;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("UOM conversion not found of " + vtable.Rows[0]["RCode"] + " " +
                                        vtable.Rows[0]["RName"] + " UOM From  " + vtable.Rows[0]["UOMn"] + " UOM To  " +
                                        vtable.Rows[0]["RUOM"]);
                }

                #endregion

                #region Check Zero

                cmd.CommandText = getZeroTotalQuantity;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception(" Total Quantity can't be zero " + vtable.Rows[0]["FCode"] + " " + vtable.Rows[0]["FName"]);
                }

                cmd.CommandText = getZeroUseQuantity;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception(" Use Quantity can't be zero " + vtable.Rows[0]["FCode"] + " " + vtable.Rows[0]["FName"]);
                }

                #endregion

                cmd.CommandText = effectDateCheck;
                dataAdapter.SelectCommand = cmd;
                vtable = new DataTable();

                dataAdapter.Fill(vtable);

                if (vtable.Rows.Count > 0)
                {
                    throw new Exception("Finish Product Already exist with this date, Product Code:" + vtable.Rows[0]["FCode"]);
                }

                cmd.CommandText = updateCalculation;
                cmd.ExecuteNonQuery();

                #endregion

                cmd.CommandText = getBOMRaws;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable table = new DataTable();

                adapter.Fill(table);

                result = commonDal.BulkInsert("BOMRaws", table, connection, transaction, 10000, null, connVM);

                cmd.CommandText = getOverHead;
                adapter.SelectCommand = cmd;
                table = new DataTable();

                adapter.Fill(table);

                result = commonDal.BulkInsert("BOMCompanyOverhead", table, connection, transaction, 10000, null, connVM);

                cmd.CommandText = getOverheadRaws;
                adapter.SelectCommand = cmd;
                table = new DataTable();
                adapter.Fill(table);
                result = commonDal.BulkInsert("BOMRaws", table, connection, transaction, 10000, null, connVM);


                cmd.CommandText = getBOM;
                adapter.SelectCommand = cmd;
                table = new DataTable();

                adapter.Fill(table);


                // --------------------------------------------------
                //if (result[0].ToLower() == "success")
                //{
                //    transaction.Commit();
                //}
                //return result;
                //--------------------------------------------------


                result = commonDal.BulkInsert("BOMs", table, connection, transaction, 10000, null, connVM);

                cmd.CommandText = updateBOMs;
                cmd.ExecuteNonQuery();

                if (result[0].ToLower() == "success")
                {
                    transaction.Commit();
                }


                return result;
            }
            #endregion

            #region catch

            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                result[0] = "fail";

                FileLogger.Log("BOMDAL", "ImportBOM", e.ToString());

                ////FileLogger.Log("BOM DAL", "BOMIMport", e.Message + "\n" + e.StackTrace);

                throw e;

            }
            #endregion

            #region finally

            finally
            {

                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();

                }

            }
            #endregion

        }

        public DataTable GetBOMExcelData(List<string> BOMIdList, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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


                sqlText = @"
create table #temp(
Id int identity(1,1),
 BOMId varchar(100),
 FCode varchar(100),
 FName varchar(500), 
 FUOM varchar(100), 

 RName varchar(500), 
RCode varchar(100),
RItemNo varchar(100),
RUOM varchar(100), 
CustomerName varchar(100), 
CustomerCode varchar(100),
FirstSupplyDate varchar(100), 
ReferenceNo varchar(100),
EffectDate varchar(100),
VATName varchar(100), 
TotalQuantity decimal(25,9), 
UseQuantity decimal(25,9),
WastageQuantity decimal(25,9),
Cost decimal(25,9), 
RebateRate decimal(25,9), 
IssueOnProduction varchar(1),
AutoIssue varchar(1),
Type varchar(50)

)

insert into #temp
 (BOMId,
 FCode,
 FName, 
  FUOM , 
 RName, 

RCode ,
RItemNo ,
RUOM , 
CustomerName , 
CustomerCode ,
FirstSupplyDate , 
ReferenceNo ,
EffectDate ,
VATName , 
TotalQuantity, 
UseQuantity ,
WastageQuantity ,
Cost, 
RebateRate , 
IssueOnProduction ,
AutoIssue 
)

select distinct  BOMs.BOMId,pf.ProductCode FCode, pf.ProductName FName,BOMs.UOM FUOM, pr.ProductName RName, 
pr.ProductCode RCode, pr.ItemNo RItemNo, BOMRaws.UOM RUOM , Customers.CustomerName, Customers.CustomerCode,
BOMs.FirstSupplyDate, BOMs.ReferenceNo,BOMS.EffectDate ,BOMs.VATName, BOMRaws.TotalQuantity, 
BOMRaws.UseQuantity,BOMRaws.WastageQuantity,
BOMRaws.Cost, BOMRaws.RebateRate, BOMRaws.IssueOnProduction,Isnull(BOMs.AutoIssue,'Y')AutoIssue
from 
BOMs  left outer join BOMRaws on BOMs.BOMId = BOMRaws.BOMId
 left outer join BOMCompanyOverhead on BOMRaws.BOMId = BOMCompanyOverhead.BOMId
 left outer join Products pf on BOMs.FinishItemNo = pf.ItemNo
 left outer join Products pr on BOMRaws.RawItemNo = pr.ItemNo
 left outer join Customers on Customers.CustomerID = BOMRaws.CustomerID
where BOMS.BOMId in(@bomids)  and BOMRaws.RawItemType != 'Overhead'
union
select distinct BOMCompanyOverhead.BOMId,pf.ProductCode FCode, pf.ProductName FName,BOMs.UOM FUOM, BOMCompanyOverhead.HeadName RName, 
BOMCompanyOverhead.HeadID RCode, BOMCompanyOverhead.HeadID RItemNo, 
isnull(BOMRaws.UOM,'-') RUOM , Customers.CustomerName, Customers.CustomerCode,
BOMs.FirstSupplyDate, BOMs.ReferenceNo,BOMS.EffectDate ,BOMs.VATName, isnull(BOMRaws.TotalQuantity,1) TotalQuantity, 
isnull(BOMRaws.UseQuantity,1) UseQuantity, isnull(BOMRaws.WastageQuantity,0)WastageQuantity,
BOMCompanyOverhead.HeadAmount Cost, isnull(BOMRaws.RebateRate,100-BOMCompanyOverhead.RebatePercent)RebateRate, Isnull(BOMRaws.IssueOnProduction,'N')IssueOnProduction ,Isnull(BOMs.AutoIssue,'Y')AutoIssue
from BOMCompanyOverhead 
left outer join BOMRaws on BOMCompanyOverhead.BOMId = BOMRaws.BOMId and BOMCompanyOverhead.HeadID = BOMRaws.RawItemNo
left outer join BOMS on BOMS.BOMId = BOMRaws.BOMId
 left outer join Products pf on BOMs.FinishItemNo = pf.ItemNo
  left outer join Customers on Customers.CustomerID = BOMRaws.CustomerID
where BOMCompanyOverhead.BOMId in(@bomids) 

update #temp 
set FirstSupplyDate = BOMs.FirstSupplyDate,
VATName = BOMs.VATName,
ReferenceNo = BOMs.ReferenceNo,
EffectDate = BOMs.EffectDate,
FCode = Products.ProductCode,
FName = Products.ProductName,
FUOM = BOMs.UOM,

CustomerCode = Customers.CustomerCode,
CustomerName = Customers.CustomerName
from BOMs join Products on BOMs.FinishItemNo = Products.ItemNo
join Customers on BOMs.CustomerID = Customers.CustomerID
where  BOMs.BOMId = #temp.BOMId  and FCode is null


update #temp 
set FirstSupplyDate = BOMs.FirstSupplyDate,
VATName = BOMs.VATName,
ReferenceNo = BOMs.ReferenceNo,
EffectDate = BOMs.EffectDate,
FCode = Products.ProductCode,
FName = Products.ProductName,
FUOM = BOMs.UOM

from BOMs join Products on BOMs.FinishItemNo = Products.ItemNo
where  BOMs.BOMId = #temp.BOMId  and FCode is null


update #temp 
set RCode = Products.ProductCode
from Products where #temp.RItemNo = products.ItemNo

update #temp 
set Type = Products.CategoryID
from Products 
where #temp.RItemNo = products.ItemNo


update #temp 
set Type = ProductCategories.IsRaw
from ProductCategories 
where #temp.Type = ProductCategories.CategoryID


select * from #temp order by FCode asc, IssueOnProduction desc

drop table #temp 





 ";


                string bomids = "";

                var len = BOMIdList.Count;

                for (int i = 0; i < len; i++)
                {
                    bomids += "'" + BOMIdList[i] + "'";

                    if (i != (len - 1))
                    {
                        bomids += ",";
                    }
                }

                if (len == 0)
                {
                    bomids += "''";
                }

                sqlText = sqlText.Replace("@bomids", bomids);

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
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[4] = ex.Message.ToString(); //catch ex
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }

                FileLogger.Log("BOMDAL", "GetBOMExcelData", ex.ToString() + "\n" + sqlText);

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

        public DataTable GetCompareData(List<string> BOMIdList,bool isOverhead = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ
            string sqlText = "";


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


                DataTable dtFirstProduct = GetBOMExcelData(new List<string>() { BOMIdList.Min(x => Convert.ToInt32(x)).ToString() }, currConn, transaction);


                DataTable dtSecondProduct = GetBOMExcelData(new List<string>() { BOMIdList.Max(x => Convert.ToInt32(x)).ToString() }, currConn, transaction);


                DataTable dtResult = dtFirstProduct.AsEnumerable().Join(dtSecondProduct.AsEnumerable(),
                    x => x["RCode"].ToString(),
                    y => y["RCode"].ToString(),
                    ( x,y) => new
                    {
                         FinishCode = x["FCode"].ToString()
                        ,FinishName = x["FName"].ToString()
                        ,FinishUOM = x["FUOM"].ToString()

                        ,RawCode = x["RCode"].ToString()
                        ,RawName = x["RName"].ToString()

                        ,FirstEffectDate = x["EffectDate"].ToString()

                        ,First_TotalQuantity = Convert.ToDecimal(x["TotalQuantity"])
                        ,First_UseQuantity = Convert.ToDecimal(x["UseQuantity"])
                        ,First_WastageQuantity = Convert.ToDecimal(x["WastageQuantity"])
                        ,First_Cost = Convert.ToDecimal(x["Cost"])

                        ,SecondEffectDate = y["EffectDate"].ToString()

                        ,Second_TotalQuantity = Convert.ToDecimal(y["TotalQuantity"])
                        ,Second_UseQuantity = Convert.ToDecimal(y["UseQuantity"])
                        ,Second_WastageQuantity = Convert.ToDecimal(y["WastageQuantity"])
                        ,Second_Cost = Convert.ToDecimal(y["Cost"])

                        ,Type = x["Type"].ToString()

                        ,
                         TotalQuantity_Diff_Percentage = ((100 / (Convert.ToDecimal(x["TotalQuantity"]) == 0 ? 1 : Convert.ToDecimal(x["TotalQuantity"]))) * Convert.ToDecimal(y["TotalQuantity"])) - (Convert.ToDecimal(x["TotalQuantity"]) == 0 && Convert.ToDecimal(y["TotalQuantity"])  == 0? 0: 100)
                        ,
                         UseQuantity_Diff_Percentage = ((100 / (Convert.ToDecimal(x["UseQuantity"]) == 0 ? 1 : Convert.ToDecimal(x["UseQuantity"]))) * Convert.ToDecimal(y["UseQuantity"])) - (Convert.ToDecimal(x["UseQuantity"]) == 0 && Convert.ToDecimal(y["UseQuantity"]) == 0 ? 0 : 100)
                        ,
                         WastageQuantity_Diff_Percentage = ((100 / (Convert.ToDecimal(x["WastageQuantity"]) == 0 ? 1 : Convert.ToDecimal(x["WastageQuantity"]))) * Convert.ToDecimal(y["WastageQuantity"])) - (Convert.ToDecimal(x["WastageQuantity"]) == 0 && Convert.ToDecimal(y["WastageQuantity"]) == 0 ? 0 : 100)
                        ,
                         Cost_Diff_Percentage = ((100 / (Convert.ToDecimal(x["Cost"]) == 0 ? 1 : Convert.ToDecimal(x["Cost"]))) * Convert.ToDecimal(y["Cost"])) - (Convert.ToDecimal(x["Cost"]) == 0 && Convert.ToDecimal(y["Cost"]) == 0 ? 0 : 100)

                    }).ToList().ToDataTable();


                if (!isOverhead)
                {
                    dtResult = dtResult.Select("Type <> 'Overhead'").CopyToDataTable();
                }

                dtResult.Columns.Remove("Type");

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }
                }
                #endregion Commit

                return dtResult;


            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null) { transaction.Rollback(); }
                FileLogger.Log("BOMDAL", "GetBOMCompareData", ex.ToString() + "\n" + sqlText);
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

        public string[] MultiplePost(string[] Ids, SysDBInfoVMTemp connVM = null)
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

                #endregion open connection and transaction
                for (int i = 0; i < Ids.Length - 1; i++)
                {

                    BOMNBRVM master = SelectAllList(Ids[i], null, null, currConn, transaction, null, null).FirstOrDefault();

                    master.Post = "Y";
                    retResults = BOMPost(master, null, transaction, currConn);
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
                if (transaction != null)
                {
                    transaction.Rollback();
                }
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
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Close();
                }
            }
            #region Result
            return retResults;
            #endregion Result
        }

        #region need to parameterize done

        public string[] BOMImport(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
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

            string fItemNo = string.Empty;
            string PostStatus = "";

            int IDExist = 0;
            string VATName = "";
            int nextBOMId = 0;


            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                VATName = bomMaster.VATName;
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException("BOMImport", MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Insert BOM Table

                if (bomItems == null)
                {
                    throw new ArgumentNullException("BOMImport", MessageVM.PurchasemsgNoDataToSave);
                }
                int BOMLineNo = 0;

                #region Find Finish Item Name

                sqlText = "";
                sqlText = "select ItemNo from Products ";
                sqlText += " where ProductName =@bomMasterFinishItemName";

                SqlCommand cmdFItemName = new SqlCommand(sqlText, currConn);
                cmdFItemName.Transaction = transaction;
                cmdFItemName.Parameters.AddWithValueAndNullHandle("@bomMasterFinishItemName", bomMaster.FinishItemName);

                fItemNo = (string)cmdFItemName.ExecuteScalar();

                if (fItemNo == null || string.IsNullOrEmpty(fItemNo))
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,Inserted Finish Item ('" + bomMaster.FinishItemName +
                                                    "') not save in database, Please check the Item information");
                }

                #endregion Find Finish Item Name

                #region Find Finish Item UOM

                sqlText = "";
                sqlText = "select UOM from Products ";
                sqlText += " where ProductName =@bomMasterFinishItemName ";

                SqlCommand cmdFUom = new SqlCommand(sqlText, currConn);
                cmdFUom.Transaction = transaction;
                cmdFUom.Parameters.AddWithValueAndNullHandle("@bomMasterFinishItemName", bomMaster.FinishItemName);

                string fUom = (string)cmdFUom.ExecuteScalar();

                if (fUom == null || string.IsNullOrEmpty(fUom))
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,Inserted Finish Item ('" + bomMaster.FinishItemName +
                                                    "') not save in database, Please check the Item information");
                }

                #endregion Find Finish Item UOM

                #region Checking other BOM after this date

                sqlText = "";
                sqlText = "select count(bomid) from boms ";
                sqlText += " where FinishItemNo =@fItemNo ";
                sqlText += " and effectdate>@bomMasterEffectDate";
                sqlText += " and VATName=@VATName";

                SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                cmdOtherBom.Transaction = transaction;
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@fItemNo", fItemNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                int otherBom = (int)cmdOtherBom.ExecuteScalar();

                if (otherBom > 0)
                {
                    throw new ArgumentNullException("BOMImport",
                                                    "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                }

                #endregion Checking other BOM after this date

                decimal LastNBRPrice = 0;
                decimal NBRWithSDAmount = bomMaster.NBRWithSDAmount;
                decimal LastNBRWithSDAmount = 0;
                decimal TotalQuantity = 0;
                decimal SDAmount = bomMaster.SDAmount;
                decimal VATAmount = bomMaster.VatAmount;
                decimal WholeSalePrice = bomMaster.WholeSalePrice;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                var vFinishItemNo = fItemNo;
                //var vEffectDate = bomMaster.EffectDate.ToString("yyyy-MM-dd HH:mm:ss");
                var vEffectDate = bomMaster.EffectDate;

                sqlText = "";
                sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo=@vFinishItemNo";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPrice.Transaction = transaction;
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                if (objLastNBRPrice != null)
                {
                    LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                }

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);


                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                #region Insert BOMs Master Data

                #region Find Transaction Exist



                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate=@vEffectDate ";
                sqlText += " AND VATName=@VATName";
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                IDExist = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 4.3 (Tender)")
                {
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Price declaration for this item already exist in same date.");
                    }
                }

                #endregion Find Transaction Exist

                #region Generate BOMId

                sqlText = "";
                sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";
                SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                cmdGenId.Transaction = transaction;
                nextBOMId = (int)cmdGenId.ExecuteScalar();

                if (nextBOMId <= 0)
                {
                    throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate BOMId.");
                }

                #endregion Generate BOMId


                #region Insert only BOM


                bomMaster.LastNBRPrice = LastNBRPrice;
                bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                bomMaster.LastSDAmount = LastSDAmount;
                bomMaster.LastMarkupValue = LastMarkupAmount;

                sqlText = "";
                sqlText += " insert into BOMs(";
                sqlText += " BOMId,";
                sqlText += " FinishItemNo,";
                sqlText += " EffectDate,";
                sqlText += " VATName,";
                sqlText += " VATRate,";
                sqlText += " UOM,";
                sqlText += " SD,";
                sqlText += " TradingMarkUp,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " RawTotal,";
                sqlText += " PackingTotal,";
                sqlText += " RebateTotal,";
                sqlText += " AdditionalTotal,";
                sqlText += " RebateAdditionTotal,";
                sqlText += " NBRPrice,";
                sqlText += " Packetprice,";
                sqlText += " RawOHCost,";
                sqlText += " LastNBRPrice,";
                sqlText += " LastNBRWithSDAmount,";
                sqlText += " TotalQuantity,";
                sqlText += " SDAmount,";
                sqlText += " VATAmount,";
                sqlText += " WholeSalePrice,";
                sqlText += " NBRWithSDAmount,";
                sqlText += " MarkUpValue,";
                sqlText += " LastMarkUpValue,";
                sqlText += " LastSDAmount,";
                sqlText += " LastAmount,";
                sqlText += " FirstSupplyDate,";
                sqlText += " BranchId,";
                sqlText += " Post";

                sqlText += " )";
                sqlText += " values( ";
                sqlText += "@nextBOMId ,";
                sqlText += "@vFinishItemNo ,";
                sqlText += "@bomMasterEffectDate ,";
                sqlText += "@bomMasterVATName ,";
                sqlText += "@bomMasterVATRate,";
                sqlText += "@fUom ,";
                sqlText += "@bomMasterSDRate,";
                sqlText += "@bomMasterTradingMarkup,";
                sqlText += "@bomMasterComments ,";
                sqlText += "@bomMasterActiveStatus ,";
                sqlText += "@bomMasterCreatedBy ,";
                sqlText += "@bomMasterCreatedOn ,";
                sqlText += "@bomMasterLastModifiedBy ,";
                sqlText += "@bomMasterLastModifiedOn ,";
                sqlText += "@bomMasterRawTotal,";
                sqlText += "@bomMasterPackingTotal,";
                sqlText += "@bomMasterRebateTotal,";
                sqlText += "@bomMasterAdditionalTotal,";
                sqlText += "@bomMasterRebateAdditionTotal,";
                sqlText += "@bomMasterPNBRPrice,";
                sqlText += "@bomMasterPPacketPrice,";
                sqlText += "@bomMasterRawOHCost,";
                sqlText += "@bomMasterLastNBRPrice,";
                sqlText += "@bomMasterLastNBRWithSDAmount,";
                sqlText += "@bomMasterTotalQuantity,";
                sqlText += "@bomMasterSDAmount,";
                sqlText += "@bomMasterVatAmount,";
                sqlText += "@bomMasterWholeSalePrice,";
                sqlText += "@bomMasterNBRWithSDAmount,";
                sqlText += "@bomMasterMarkupValue,";
                sqlText += "@bomMasterLastMarkupValue,";
                sqlText += "@bomMasterLastSDAmount,";
                sqlText += "@bomMasterLastSDAmount,";
                sqlText += "@FirstSupplyDate,";
                sqlText += "@bomMasterBranchId,";
                sqlText += "@bomMasterPost ";


                sqlText += ")	";




                SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                cmdInsMaster.Transaction = transaction;
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate) ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@fUom", fUom ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedBy", bomMaster.CreatedBy ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedOn", OrdinaryVATDesktop.DateToDate(bomMaster.CreatedOn) ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedOn) ?? Convert.DBNull);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@FirstSupplyDate", OrdinaryVATDesktop.DateToDate(bomMaster.FirstSupplyDate));
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterBranchId", bomMaster.BranchId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post);


                transResult = (int)cmdInsMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgSaveNotSuccessfully);
                }

                #endregion Insert only BOM

                #endregion

                foreach (var BItem in bomItems.ToList())
                {

                    #region Find Raw Item

                    sqlText = "";
                    sqlText = "select ItemNo from Products ";
                    sqlText += " where ProductName =@BItemRawItemName ";

                    SqlCommand cmdRItemNo = new SqlCommand(sqlText, currConn);
                    cmdRItemNo.Transaction = transaction;
                    cmdRItemNo.Parameters.AddWithValueAndNullHandle("@BItemRawItemName", BItem.RawItemName);

                    string RItemNo = (string)cmdRItemNo.ExecuteScalar();

                    if (RItemNo == null || string.IsNullOrEmpty(RItemNo))
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Sorry,Inserted Raw Item ('" + BItem.RawItemName +
                                                        "') not save in database, Please check the Item information");
                    }

                    #endregion Find Raw Item

                    #region Find Raw Item UOM

                    sqlText = "";
                    sqlText = "select UOM from Products ";
                    sqlText += " where ProductName =@BItemRawItemName";

                    SqlCommand cmdRUom = new SqlCommand(sqlText, currConn);
                    cmdRUom.Transaction = transaction;
                    cmdRUom.Parameters.AddWithValueAndNullHandle("@BItemRawItemName", BItem.RawItemName);

                    string rUom = (string)cmdRUom.ExecuteScalar();

                    if (rUom == null || string.IsNullOrEmpty(rUom))
                    {
                        throw new ArgumentNullException("BOMImport",
                                                        "Sorry,Inserted Raw Item ('" + BItem.RawItemName +
                                                        "') not save in database, Please check the Item information");
                    }

                    #endregion Find Raw Item UOM

                    #region UOMc

                    decimal rUomc = 0;
                    if (BItem.UOM == rUom)
                    {
                        rUomc = 1;
                    }
                    else
                    {
                        sqlText = "";
                        sqlText = "SELECT top 1 u.Convertion FROM UOMs u";
                        sqlText += " WHERE u.UOMFrom=@rUom  AND u.UOMTo=@BItemUOM AND u.ActiveStatus='Y'";

                        SqlCommand cmdUomc = new SqlCommand(sqlText, currConn);
                        cmdUomc.Transaction = transaction;
                        cmdUomc.Parameters.AddWithValueAndNullHandle("@rUom", rUom);
                        cmdUomc.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM);


                        object objrUomc = cmdUomc.ExecuteScalar();
                        rUomc = Convert.ToDecimal(objrUomc);
                        if (rUomc <= 0 || objrUomc == null)
                        {
                            throw new ArgumentNullException("BOMImport",
                                                            "Sorry,Inserted UOM  ('" + BItem.UOM + "' and '" + rUom +
                                                            "') conversion not save in database,Item name :'" +
                                                            BItem.RawItemName + "'  Please check the Item information");
                        }
                    }

                    decimal uOMPrice = 0;
                    decimal uOMUQty = 0;
                    decimal uOMWQty = 0;
                    decimal totalQty = 0;
                    decimal unitCost = 0;
                    uOMPrice = BItem.UnitCost / rUomc;
                    uOMUQty = rUomc * BItem.UseQuantity;
                    uOMWQty = rUomc * BItem.WastageQuantity;
                    totalQty = uOMUQty + uOMWQty;
                    //unitCost=BItem

                    #endregion UOMc

                    #region Generate BOMRaw Id

                    int nextBOMRawId = 0;
                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                    SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                    cmdGenRawId.Transaction = transaction;
                    nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                    if (nextBOMRawId <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate BOMRawId.");
                    }

                    #endregion Generate BOMRaw Id


                    BOMLineNo++;
                    sqlText = "";
                    sqlText += " insert into BOMRaws(";
                    sqlText += " BOMRawId,";
                    sqlText += " BOMId,";
                    sqlText += " BOMLineNo,";
                    sqlText += " FinishItemNo,";
                    sqlText += " RawItemNo,";
                    sqlText += " RawItemType,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " UseQuantity,";
                    sqlText += " WastageQuantity,";
                    sqlText += " Cost,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " RebateRate,";
                    sqlText += " MarkUpValue,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " UnitCost,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMUQty,";
                    sqlText += " UOMWQty,";
                    sqlText += " TotalQuantity,";
                    sqlText += " PBOMId,";
                    sqlText += " Post";



                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMRawId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@BOMLineNo ,";
                    sqlText += "@fItemNo,";
                    sqlText += "@RItemNo,";
                    sqlText += "@BItemRawItemType,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@VATName,";
                    sqlText += "@BItemUseQuantity,";
                    sqlText += "@BItemWastageQuantity,";
                    sqlText += "@BItemCost,";
                    sqlText += "@BItemUOM,";
                    sqlText += "@BItemVATRate,";
                    sqlText += "@BItemVatAmount ,";
                    sqlText += "@BItemSD,";
                    sqlText += "@BItemSDAmount,";
                    sqlText += "@BItemTradingMarkUp,";
                    sqlText += "@BItemRebateRate,";
                    sqlText += "@MarkupAmount,";
                    sqlText += "@BItemCreatedBy,";
                    sqlText += "@BItemCreatedOn,";
                    sqlText += "@BItemLastModifiedBy,";
                    sqlText += "@BItemLastModifiedOn,";
                    sqlText += "@BItemUnitCost,";
                    sqlText += "@rUom,";
                    sqlText += "@rUomc,";
                    sqlText += "@uOMPrice,";
                    sqlText += "@uOMUQty,";
                    sqlText += "@uOMWQty,";
                    sqlText += "@totalQty,";
                    sqlText += "0 ,";
                    sqlText += "@bomMasterPost";



                    sqlText += ")	";




                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@fItemNo", fItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@RItemNo", RItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkupAmount", MarkupAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", OrdinaryVATDesktop.DateToDate(BItem.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(BItem.LastModifiedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@rUom", rUom ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@rUomc", rUomc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMPrice", uOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMUQty", uOMUQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@uOMWQty", uOMWQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@totalQty", totalQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post ?? Convert.DBNull);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                }

                #endregion Insert Detail Table

                #region Insert BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }




                int OHLineNo = 0;

                foreach (var OHItem in bomOHs.ToList())
                {
                    #region Generate Overhead Id

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                    SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                    cmdGenOHId.Transaction = transaction;
                    int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                    if (nextOHId <= 0)
                    {
                        throw new ArgumentNullException("BOMImport", "Sorry,Unable to generate Overhead Id.");
                    }

                    #endregion Generate Overhead Id

                    #region Find Transaction Exist

                    OHLineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE FinishItemNo=@vFinishItemNo";
                    sqlText += " AND HeadName=@OHItemHeadName AND EffectDate=@bomMasterEffectDate and VATName=@VATName";
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);

                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (bomMaster.VATName != "VAT 4.3 (Tender)")
                    {
                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException("bOMInsert", MessageVM.PurchasemsgFindExistID);
                        }
                    }

                    #endregion Find Transaction Exist

                    #region Insert only OH

                    decimal vRebateAmount = 0;
                    decimal vAddingAmount = 0;


                    sqlText = "";
                    sqlText += " insert into BOMCompanyOverhead(";
                    sqlText += " BOMOverHeadId,";
                    sqlText += " BOMId,";
                    sqlText += " HeadName,";
                    sqlText += " HeadAmount,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " OHLineNo,";
                    sqlText += " VATName,";
                    sqlText += " RebatePercent, ";
                    sqlText += " RebateAmount,";
                    sqlText += " AdditionalCost, ";
                    sqlText += " Post ";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextOHId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@OHItemHeadName,";
                    sqlText += "@OHItemHeadAmount,";
                    sqlText += "@OHItemCreatedBy,";
                    sqlText += "@OHItemCreatedOn,";
                    sqlText += "@OHItemLastModifiedBy,";
                    sqlText += "@OHItemLastModifiedOn,";
                    sqlText += "@vFinishItemNo,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@OHLineNo,";
                    sqlText += "@VATName,";
                    sqlText += "@OHItemRebatePercent,";
                    sqlText += "@OHItemRebateAmount,";
                    sqlText += "@OHItemAdditionalCost,";
                    sqlText += "@OHItemPost";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OrdinaryVATDesktop.DateToDate(OHItem.CreatedOn));
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(OHItem.LastModifiedOn));
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.PurchasemsgSaveNotSuccessfully);
                    }

                    #endregion Insert only OH
                }

                #endregion Insert BOMCompanyOverhead Table

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
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + nextBOMId;
                retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            catch (SqlException sqlex)
            {
                transaction.Rollback();
                FileLogger.Log("BOMDAL", "BOMImport", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                FileLogger.Log("BOMDAL", "BOMImport", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
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
        public string[] BOMInsertX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
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
            string PostStatus = "";
            int IDExist = 0;
            //string VATName = "";
            int nextBOMId = 0;

            #endregion Initializ

            #region Try

            try
            {

                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd(""@DateTo"", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.bomMsgMethodNameInsert);


                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock

                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                               " where @transactionYearCheck between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@transactionYearCheck", transactionYearCheck);

                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Insert BOM Table

                if (bomItems == null || bomItems.Count <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }

                int BOMLineNo = 0;


                #region finish Product Exists

                sqlText = "";
                sqlText = " select count(ItemNo) from Products ";
                sqlText += " where ItemNo =@bomMasterItemNo ";

                SqlCommand cmdfindFItem = new SqlCommand(sqlText, currConn);
                cmdfindFItem.Transaction = transaction;
                cmdfindFItem.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);

                int findFItem = (int)cmdfindFItem.ExecuteScalar();

                if (findFItem <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration Item not in database please check the Item ('" +
                                                    bomMaster.FinishItemName + "')");
                }

                #endregion Product Exists

                #region Find Transaction Exist

                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                //sqlText += " AND EffectDate=@bomMaster.EffectDate.Date + "' ";
                sqlText += " AND EffectDate=@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int IDExist1 = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 4.3 (Tender)")
                {
                    if (IDExist1 > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                decimal LastNBRPrice = 0;

                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                #region LastNBRPrice

                if (bomMaster.LastNBRPrice <= 0)
                {
                    sqlText = "";
                    sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo=@bomMasterItemNo ";
                    sqlText += " AND EffectDate<@bomMasterEffectDate ";
                    sqlText += " AND VATName=@bomMasterVATName ";
                    sqlText += " and Post='Y'";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    }

                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }
                }
                else
                {
                    LastNBRPrice = bomMaster.LastNBRPrice;
                }

                #endregion LastNBRPrice

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                #region Insert BOMs Master Data


                #region Generate BOMId

                nextBOMId = 0;
                sqlText = "";
                sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";

                SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                cmdGenId.Transaction = transaction;
                nextBOMId = (int)cmdGenId.ExecuteScalar();

                if (nextBOMId <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,Unable to generate BOMId.");
                }

                #endregion Generate BOMId


                #region Insert only BOM

                if (bomMaster.IsImported != "Y")
                {
                    bomMaster.LastNBRPrice = LastNBRPrice;
                    bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    bomMaster.LastSDAmount = LastSDAmount;
                    bomMaster.LastMarkupValue = LastMarkupAmount;

                }

                sqlText = "";
                sqlText += " insert into BOMs(";
                sqlText += " BOMId,";
                sqlText += " FinishItemNo,";
                sqlText += " EffectDate,";
                sqlText += " VATName,";
                sqlText += " VATRate,";
                sqlText += " UOM,";
                sqlText += " SD,";
                sqlText += " TradingMarkUp,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " RawTotal,";
                sqlText += " PackingTotal,";
                sqlText += " RebateTotal,";
                sqlText += " AdditionalTotal,";
                sqlText += " RebateAdditionTotal,";
                sqlText += " NBRPrice,";
                sqlText += " Packetprice,";
                sqlText += " RawOHCost,";
                sqlText += " LastNBRPrice,";
                sqlText += " LastNBRWithSDAmount,";
                sqlText += " TotalQuantity,";
                sqlText += " SDAmount,";
                sqlText += " VATAmount,";
                sqlText += " WholeSalePrice,";
                sqlText += " NBRWithSDAmount,";
                sqlText += " MarkUpValue,";
                sqlText += " LastMarkUpValue,";
                sqlText += " LastSDAmount,";
                sqlText += " LastAmount,";
                sqlText += " CustomerId,";
                sqlText += " FirstSupplyDate,";
                sqlText += " BranchId,";
                sqlText += " Post";

                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@nextBOMId ,";
                sqlText += "@bomMasterItemNo ,";
                sqlText += "@bomMasterEffectDate ,";
                sqlText += "@bomMasterVATName ,";
                sqlText += "@bomMasterVATRate ,";
                sqlText += "@bomMasterUOM ,";
                sqlText += "@bomMasterSDRate ,";
                sqlText += "@bomMasterTradingMarkup ,";
                sqlText += "@bomMasterComments ,";
                sqlText += "@bomMasterActiveStatus ,";
                sqlText += "@bomMasterCreatedBy ,";
                sqlText += "@bomMasterCreatedOn ,";
                sqlText += "@bomMasterLastModifiedBy ,";
                sqlText += "@bomMasterLastModifiedOn ,";
                sqlText += "@bomMasterRawTotal ,";
                sqlText += "@bomMasterPackingTotal ,";
                sqlText += "@bomMasterRebateTotal ,";
                sqlText += "@bomMasterAdditionalTotal ,";
                sqlText += "@bomMasterRebateAdditionTotal ,";
                sqlText += "@bomMasterPNBRPrice ,";
                sqlText += "@bomMasterPPacketPrice ,";
                sqlText += "@bomMasterRawOHCost ,";
                sqlText += "@bomMasterLastNBRPrice ,";
                sqlText += "@bomMasterLastNBRWithSDAmount ,";
                sqlText += "@bomMasterTotalQuantity ,";
                sqlText += "@bomMasterSDAmount ,";
                sqlText += "@bomMasterVatAmount ,";
                sqlText += "@bomMasterWholeSalePrice ,";
                sqlText += "@bomMasterNBRWithSDAmount ,";
                sqlText += "@bomMasterMarkupValue ,";
                sqlText += "@bomMasterLastMarkupValue ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterCustomerID  ,";
                sqlText += "@FirstSupplyDate,";
                sqlText += "@bomMasterBranchId,";
                sqlText += "@bomMasterPost ";


                sqlText += ")	";




                SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                cmdInsMaster.Transaction = transaction;
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedBy", bomMaster.CreatedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedOn", bomMaster.CreatedOn);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                //cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID ?? "0");
                cmdInsMaster.Parameters.AddWithValue("@FirstSupplyDate", bomMaster.FirstSupplyDate);
                cmdInsMaster.Parameters.AddWithValue("@bomMasterBranchId", bomMaster.BranchId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post);


                transResult = (int)cmdInsMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Insert only BOM

                #region Update PacketPrice

                sqlText = "";

                sqlText += " update BOMs set  ";

                sqlText += " Packetprice=@bomMasterPPacketPrice";
                sqlText += " where  FinishItemNo =@bomMasterItemNo  ";
                sqlText += " and EffectDate=@bomMasterEffectDate";
                sqlText += " and VATName= @bomMasterVATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdUpdateP = new SqlCommand(sqlText, currConn);
                cmdUpdateP.Transaction = transaction;
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                transResult = (int)cmdUpdateP.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Update PacketPrice


                #endregion

                #region "@DateTo"

                foreach (var BItem in bomItems.ToList())
                {
                    if (BItem.RawItemNo == "41")
                    {

                    }
                    #region Raw Product Exists

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ItemNo =@BItemRawItemNo  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Material Name not in database please check theMaterial Name ('" +
                                                        BItem.RawItemName + "')");
                    }

                    //GetProductNo

                    #endregion Raw Product Exists

                    #region Generate BOMRaw Id


                    int nextBOMRawId = 0;
                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                    SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                    cmdGenRawId.Transaction = transaction;
                    nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                    if (nextBOMRawId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate BOMRawId.");
                    }

                    #endregion Generate BOMRaw Id


                    BOMLineNo++;

                    sqlText = "";
                    sqlText += " insert into BOMRaws(";
                    sqlText += " BOMRawId,";
                    sqlText += " BOMId,";
                    sqlText += " BOMLineNo,";
                    sqlText += " FinishItemNo,";
                    sqlText += " RawItemNo,";
                    sqlText += " RawItemType,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " UseQuantity,";
                    sqlText += " WastageQuantity,";
                    sqlText += " Cost,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " RebateRate,";

                    sqlText += " MarkUpValue,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " UnitCost,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMUQty,";
                    sqlText += " UOMWQty,";
                    sqlText += " TotalQuantity,";
                    sqlText += " PBOMId,";
                    sqlText += " Post,";
                    sqlText += " PInvoiceNo,";
                    sqlText += " CustomerID,";
                    sqlText += " IssueOnProduction,";
                    sqlText += " BranchId,";
                    sqlText += " TransactionType";


                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMRawId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@BOMLineNo,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@BItemRawItemNo,";
                    sqlText += "@BItemRawItemType,";
                    sqlText += "@bomMaterEffectDate,";
                    sqlText += "@bomMaterVATName,";
                    sqlText += "@BItemUseQuantity,";
                    sqlText += "@BItemWastageQuantity,";
                    sqlText += "@BItemCost,";
                    sqlText += "@BItemUOM,";
                    sqlText += "@BItemVATRate,";
                    sqlText += "@BItemVatAmount,";
                    sqlText += "@BItemSD,";
                    sqlText += "@BItemSDAmount,";
                    sqlText += "@BItemTradingMarkUp,";
                    sqlText += "@BItemRebateRate,";
                    sqlText += "@MarkuAmount,";
                    sqlText += "@BItemCreatedBy,";
                    sqlText += "@BItemCreatedOn,";
                    sqlText += "@BItemLastModifiedBy,";
                    sqlText += "@BItemLastModifiedOn,";
                    sqlText += "@BItemUnitCost,";
                    sqlText += "@BItemUOMn,";
                    sqlText += "@BItemUOMc,";
                    sqlText += "@BItemUOMPrice,";
                    sqlText += "@BItemUOMUQty,";
                    sqlText += "@BItemUOMWQty,";
                    sqlText += "@BItemTotalQuantity,";
                    sqlText += "@BItemPBOMId,";
                    sqlText += "@BItemPost,";
                    sqlText += "@BItemPInvoiceNo,";
                    sqlText += "@bomMaterCustomerID,";
                    sqlText += "@BItemIssueOnProduction,";
                    sqlText += "@BItemBranchId,";
                    sqlText += "@BItemTransactionType";

                    sqlText += ")	";




                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterEffectDate", bomMaster.EffectDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterVATName", bomMaster.VATName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkuAmount", MarkupAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", BItem.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPost", BItem.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterCustomerID", bomMaster.CustomerID ?? "0");
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemBranchId", BItem.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + BItem.RawItemName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                }

                #endregion "@DateTo"

                #endregion Insert Detail Table

                #region Insert BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }


                int OHLineNo = 0;

                foreach (var OHItem in bomOHs.ToList())
                {

                    #region Raw Product Exists

                    //if (OHItem.HeadName.Trim() != "Margin")
                    //{

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ProductName =@OHItemHeadName  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Overhead not in database please check the Overhead Item ('" +
                                                        OHItem.HeadName + "') Code ('" +
                                                        OHItem.OHCode + "') ");
                    }
                    //}
                    //GetProductNo

                    #endregion Raw Product Exists

                    #region Generate Overhead Id

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                    SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                    cmdGenOHId.Transaction = transaction;
                    int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                    if (nextOHId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate Overhead Id.");
                    }

                    #endregion Generate Overhead Id

                    #region Find Transaction Exist

                    OHLineNo++;
                    sqlText = "";
                    sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE FinishItemNo=@bomMasterItemNo ";
                    sqlText += @" AND HeadName=@OHItemHeadName AND EffectDate=@bomMasterEffectDate  and VATName=@bomMasterVATName 
                    and HeadID=@HeadID";

                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                    }
                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    cmdFindId.Transaction = transaction;
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                    cmdFindId.Parameters.AddWithValueAndNullHandle("@HeadID", OHItem.HeadID);

                    IDExist = (int)cmdFindId.ExecuteScalar();
                    if (bomMaster.VATName != "VAT 4.3 (Tender)")
                    {
                        if (IDExist > 0)
                        {
                            throw new ArgumentNullException("BOM Insert", MessageVM.PurchasemsgFindExistID + ", Material name '(" + OHItem.HeadName + ")'");
                        }
                    }

                    #endregion Find Transaction Exist

                    #region Insert only OH



                    sqlText = "";
                    sqlText += " insert into BOMCompanyOverhead(";
                    sqlText += " BOMOverHeadId,";
                    sqlText += " BOMId,";
                    sqlText += " HeadID,";
                    sqlText += " HeadName,";
                    sqlText += " HeadAmount,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " OHLineNo,";
                    sqlText += " VATName,";
                    sqlText += " RebatePercent, ";
                    sqlText += " RebateAmount,";
                    sqlText += " AdditionalCost, ";
                    sqlText += " CustomerID, ";
                    sqlText += " BranchId, ";
                    sqlText += " Post ";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextOHId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@OHItemHeadID,";
                    sqlText += "@OHItemHeadName,";
                    sqlText += "@OHItemHeadAmount,";
                    sqlText += "@OHItemCreatedBy,";
                    sqlText += "@OHItemCreatedOn,";
                    sqlText += "@OHItemLastModifiedBy,";
                    sqlText += "@OHItemLastModifiedOn,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@OHLineNo,";
                    sqlText += "@bomMasterVATName,";
                    sqlText += "@OHItemRebatePercent,";
                    sqlText += "@OHItemRebateAmount,";
                    sqlText += "@OHItemAdditionalCost,";
                    sqlText += "@bomMasterCustomerID,";
                    sqlText += "@bomMasterBranchId,";
                    sqlText += "@OHItemPost";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", nextBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OHItem.CreatedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                    cmdInsDetail.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID ?? "0");
                    cmdInsDetail.Parameters.AddWithValue("@bomMasterBranchId", bomMaster.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert + "sql" + sqlText,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + OHItem.HeadName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                    #endregion Insert only OH
                }

                #endregion Insert BOMCompanyOverhead Table

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
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + nextBOMId;
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
                //    retResults[0] = "Fail";//Success or Fail
                //    retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                //    retResults[2] = ""; //catch ex

                transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);
                FileLogger.Log("BOMDAL", "BOMInsertX", ex.ToString() + "\n" + sqlText);

                return retResults;
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

        //
        public string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
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
            string PostStatus = "";
            int IDExist = 0;
            //string VATName = "";
            int nextBOMIdCreate = 0;

            #endregion Initializ

            #region Try

            try
            {

                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection(connVM);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd(""@DateTo"", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.bomMsgMethodNameInsert);


                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
                {

                    #region YearLock

                    sqlText = "";

                    sqlText += "select distinct isnull(PeriodLock,'Y') MLock,isnull(GLLock,'Y')YLock from fiscalyear " +
                               " where @transactionYearCheck between PeriodStart and PeriodEnd";

                    DataTable dataTable = new DataTable("ProductDataT");
                    SqlCommand cmdIdExist = new SqlCommand(sqlText, currConn);
                    cmdIdExist.Transaction = transaction;
                    cmdIdExist.Parameters.AddWithValueAndNullHandle("@transactionYearCheck", transactionYearCheck);

                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmdIdExist);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Insert BOM Table

                if (bomItems == null || bomItems.Count <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, MessageVM.PurchasemsgNoDataToSave);
                }

                #region finish Product Exists

                sqlText = "";
                sqlText = " select count(ItemNo) from Products ";
                sqlText += " where ItemNo =@bomMasterItemNo ";

                SqlCommand cmdfindFItem = new SqlCommand(sqlText, currConn);
                cmdfindFItem.Transaction = transaction;
                cmdfindFItem.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);

                int findFItem = (int)cmdfindFItem.ExecuteScalar();

                if (findFItem <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration Item not in database please check the Item ('" +
                                                    bomMaster.FinishItemName + "')");
                }

                #endregion Product Exists

                #region Find Transaction Exist

                sqlText = "";
                sqlText += @"
------declare @bomMasterItemNo nvarchar(100) = '1263'
------declare @bomMasterEffectDate nvarchar(100) = '2019-Oct-06' 
------declare @bomMasterVATName nvarchar(100) = 'VAT 4.3' 
------declare @ReferenceNo nvarchar(100) = 'NA'
------declare @bomMasterCustomerID nvarchar(100) = '21'

select COUNT(FinishItemNo) from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                //sqlText += " AND EffectDate=@bomMaster.EffectDate.Date + "' ";
                sqlText += " AND EffectDate=@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " AND ISNULL(ReferenceNo,'NA')=@ReferenceNo";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@ReferenceNo", bomMaster.ReferenceNo);

                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int IDExist1 = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 4.3 (Tender)")
                {
                    if (IDExist1 > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                decimal LastNBRPrice = 0;

                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                #region LastNBRPrice

                if (bomMaster.LastNBRPrice <= 0)
                {
                    sqlText = "";
                    sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo=@bomMasterItemNo ";
                    sqlText += " AND EffectDate<@bomMasterEffectDate ";
                    sqlText += " AND VATName=@bomMasterVATName ";
                    sqlText += " and Post='Y'";
                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    { }
                    else
                    {
                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    }

                    sqlText += " order by EffectDate desc";
                    SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                    cmdFindLastNBRPrice.Transaction = transaction;
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                    if (objLastNBRPrice != null)
                    {
                        LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                    }
                }
                else
                {
                    LastNBRPrice = bomMaster.LastNBRPrice;
                }

                #endregion LastNBRPrice

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate<@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                #region Insert BOMs Master Data

                #region Generate BOMId

                nextBOMIdCreate = 0;
                sqlText = "";
                sqlText = "select isnull(max(cast(BOMId as int)),0)+1 FROM  BOMs";

                SqlCommand cmdGenId = new SqlCommand(sqlText, currConn);
                cmdGenId.Transaction = transaction;
                nextBOMIdCreate = (int)cmdGenId.ExecuteScalar();

                if (nextBOMIdCreate <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert, "Sorry,Unable to generate BOMId.");
                }
                bomMaster.BOMId = nextBOMIdCreate.ToString();
                #endregion Generate BOMId

                #region Insert only BOM

                if (bomMaster.IsImported != "Y")
                {
                    bomMaster.LastNBRPrice = LastNBRPrice;
                    bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                    bomMaster.LastSDAmount = LastSDAmount;
                    bomMaster.LastMarkupValue = LastMarkupAmount;

                }

                sqlText = "";
                sqlText += " insert into BOMs(";
                sqlText += " BOMId,";
                sqlText += " ReferenceNo,";

                sqlText += " FinishItemNo,";
                sqlText += " EffectDate,";
                sqlText += " VATName,";
                sqlText += " VATRate,";
                sqlText += " UOM,";
                sqlText += " SD,";
                sqlText += " TradingMarkUp,";
                sqlText += " Comments,";
                sqlText += " ActiveStatus,";
                sqlText += " CreatedBy,";
                sqlText += " CreatedOn,";
                sqlText += " LastModifiedBy,";
                sqlText += " LastModifiedOn,";
                sqlText += " RawTotal,";
                sqlText += " PackingTotal,";
                sqlText += " RebateTotal,";
                sqlText += " AdditionalTotal,";
                sqlText += " RebateAdditionTotal,";
                sqlText += " NBRPrice,";
                sqlText += " Packetprice,";
                sqlText += " RawOHCost,";
                sqlText += " LastNBRPrice,";
                sqlText += " LastNBRWithSDAmount,";
                sqlText += " TotalQuantity,";
                sqlText += " SDAmount,";
                sqlText += " VATAmount,";
                sqlText += " WholeSalePrice,";
                sqlText += " NBRWithSDAmount,";
                sqlText += " MarkUpValue,";
                sqlText += " LastMarkUpValue,";
                sqlText += " LastSDAmount,";
                sqlText += " LastAmount,";
                sqlText += " CustomerId,";
                sqlText += " FirstSupplyDate,";
                sqlText += " BranchId,";
                sqlText += " Post,";
                sqlText += " UOMn,";
                sqlText += " UOMc,";
                sqlText += " AutoIssue,";
                sqlText += " UOMPrice,";
                sqlText += " MasterComments";


                sqlText += " )";
                sqlText += " values(	";
                sqlText += "@nextBOMId ,";
                sqlText += "@ReferenceNo ,";

                sqlText += "@bomMasterItemNo ,";
                sqlText += "@bomMasterEffectDate ,";
                sqlText += "@bomMasterVATName ,";
                sqlText += "@bomMasterVATRate ,";
                sqlText += "@bomMasterUOM ,";
                sqlText += "@bomMasterSDRate ,";
                sqlText += "@bomMasterTradingMarkup ,";
                sqlText += "@bomMasterComments ,";
                sqlText += "@bomMasterActiveStatus ,";
                sqlText += "@bomMasterCreatedBy ,";
                sqlText += "@bomMasterCreatedOn ,";
                sqlText += "@bomMasterLastModifiedBy ,";
                sqlText += "@bomMasterLastModifiedOn ,";
                sqlText += "@bomMasterRawTotal ,";
                sqlText += "@bomMasterPackingTotal ,";
                sqlText += "@bomMasterRebateTotal ,";
                sqlText += "@bomMasterAdditionalTotal ,";
                sqlText += "@bomMasterRebateAdditionTotal ,";
                sqlText += "@bomMasterPNBRPrice ,";
                sqlText += "@bomMasterPPacketPrice ,";
                sqlText += "@bomMasterRawOHCost ,";
                sqlText += "@bomMasterLastNBRPrice ,";
                sqlText += "@bomMasterLastNBRWithSDAmount ,";
                sqlText += "@bomMasterTotalQuantity ,";
                sqlText += "@bomMasterSDAmount ,";
                sqlText += "@bomMasterVatAmount ,";
                sqlText += "@bomMasterWholeSalePrice ,";
                sqlText += "@bomMasterNBRWithSDAmount ,";
                sqlText += "@bomMasterMarkupValue ,";
                sqlText += "@bomMasterLastMarkupValue ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterLastSDAmount ,";
                sqlText += "@bomMasterCustomerID  ,";
                sqlText += "@FirstSupplyDate,";
                sqlText += "@bomMasterBranchId,";
                sqlText += "@bomMasterPost, ";
                sqlText += "@uomn, ";
                sqlText += "@uomc, ";
                sqlText += "@AutoIssue, ";
                sqlText += "@uomprice, ";
                sqlText += "@bomMasterMasterComments ";


                sqlText += ")	";

                SqlCommand cmdInsMaster = new SqlCommand(sqlText, currConn);
                cmdInsMaster.Transaction = transaction;
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@nextBOMId", bomMaster.BOMId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@ReferenceNo", bomMaster.ReferenceNo);

                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedBy", bomMaster.CreatedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCreatedOn", OrdinaryVATDesktop.DateToDate(bomMaster.CreatedOn));
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedOn));
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                //cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdInsMaster.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID ?? "0");
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@FirstSupplyDate", OrdinaryVATDesktop.DateToDate(bomMaster.FirstSupplyDate));
                cmdInsMaster.Parameters.AddWithValue("@bomMasterBranchId", bomMaster.BranchId);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterPost", bomMaster.Post);

                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@uomn", bomMaster.FUOMn);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@uomc", bomMaster.FUOMc);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@AutoIssue", bomMaster.AutoIssue);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@uomprice", bomMaster.FUOMPrice);
                cmdInsMaster.Parameters.AddWithValueAndNullHandle("@bomMasterMasterComments", bomMaster.MasterComments);



                transResult = (int)cmdInsMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Insert only BOM

                #region Update PacketPrice

                sqlText = "";

                sqlText += " update BOMs set  ";
                sqlText += " Packetprice=@bomMasterPPacketPrice";
                sqlText += " where  FinishItemNo =@bomMasterItemNo  ";
                sqlText += " and EffectDate=@bomMasterEffectDate";
                sqlText += " and VATName= @bomMasterVATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                SqlCommand cmdUpdateP = new SqlCommand(sqlText, currConn);
                cmdUpdateP.Transaction = transaction;
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdUpdateP.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                transResult = (int)cmdUpdateP.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }

                #endregion Update PacketPrice

                retResults = BOMInsert2(bomItems, bomOHs, bomMaster, transaction, currConn);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, retResults[1]);
                }

                #endregion

                #endregion Insert Detail Table

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
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + bomMaster.BOMId;
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
                //    retResults[0] = "Fail";//Success or Fail
                //    retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                //    retResults[2] = ""; //catch ex

                transaction.Rollback();

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("BOMDAL", "BOMInsert", ex.ToString() + "\n" + sqlText);

                return retResults;
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
        public string[] BOMInsert2(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SqlTransaction Vtransaction = null, SqlConnection VcurrConn = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[5];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            retResults[4] = "";
            int transResult = 0;
            string sqlText = "";
            string PostStatus = "";
            int IDExist = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

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

                #region Insert BOM Table

                int BOMLineNo = 0;

                #region Detail Insert

                foreach (var BItem in bomItems.ToList())
                {

                    #region Raw Product Exists

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ItemNo =@BItemRawItemNo  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Material Name not in database please check theMaterial Name ('" +
                                                        BItem.RawItemName + "')");
                    }

                    #endregion Raw Product Exists

                    #region Generate BOMRaw Id


                    int nextBOMRawId = 0;
                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                    SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                    cmdGenRawId.Transaction = transaction;
                    nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                    if (nextBOMRawId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate BOMRawId.");
                    }

                    #endregion Generate BOMRaw Id


                    BOMLineNo++;

                    sqlText = "";
                    sqlText += " insert into BOMRaws(";
                    sqlText += " BOMRawId,";
                    sqlText += " BOMId,";
                    sqlText += " BOMLineNo,";
                    sqlText += " FinishItemNo,";
                    sqlText += " RawItemNo,";
                    sqlText += " RawItemType,";
                    sqlText += " EffectDate,";
                    sqlText += " VATName,";
                    sqlText += " UseQuantity,";
                    sqlText += " WastageQuantity,";
                    sqlText += " Cost,";
                    sqlText += " UOM,";
                    sqlText += " VATRate,";
                    sqlText += " VATAmount,";
                    sqlText += " SD,";
                    sqlText += " SDAmount,";
                    sqlText += " TradingMarkUp,";
                    sqlText += " RebateRate,";

                    sqlText += " MarkUpValue,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " UnitCost,";
                    sqlText += " UOMn,";
                    sqlText += " UOMc,";
                    sqlText += " UOMPrice,";
                    sqlText += " UOMUQty,";
                    sqlText += " UOMWQty,";
                    sqlText += " TotalQuantity,";
                    sqlText += " PBOMId,";
                    sqlText += " Post,";
                    sqlText += " PInvoiceNo,";
                    sqlText += " CustomerID,";
                    sqlText += " IssueOnProduction,";
                    sqlText += " BranchId,";
                    sqlText += " TransactionType";


                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextBOMRawId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@BOMLineNo,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@BItemRawItemNo,";
                    sqlText += "@BItemRawItemType,";
                    sqlText += "@bomMaterEffectDate,";
                    sqlText += "@bomMaterVATName,";
                    sqlText += "@BItemUseQuantity,";
                    sqlText += "@BItemWastageQuantity,";
                    sqlText += "@BItemCost,";
                    sqlText += "@BItemUOM,";
                    sqlText += "@BItemVATRate,";
                    sqlText += "@BItemVatAmount,";
                    sqlText += "@BItemSD,";
                    sqlText += "@BItemSDAmount,";
                    sqlText += "@BItemTradingMarkUp,";
                    sqlText += "@BItemRebateRate,";
                    sqlText += "@MarkuAmount,";
                    sqlText += "@BItemCreatedBy,";
                    sqlText += "@BItemCreatedOn,";
                    sqlText += "@BItemLastModifiedBy,";
                    sqlText += "@BItemLastModifiedOn,";
                    sqlText += "@BItemUnitCost,";
                    sqlText += "@BItemUOMn,";
                    sqlText += "@BItemUOMc,";
                    sqlText += "@BItemUOMPrice,";
                    sqlText += "@BItemUOMUQty,";
                    sqlText += "@BItemUOMWQty,";
                    sqlText += "@BItemTotalQuantity,";
                    sqlText += "@BItemPBOMId,";
                    sqlText += "@BItemPost,";
                    sqlText += "@BItemPInvoiceNo,";
                    sqlText += "@bomMaterCustomerID,";
                    sqlText += "@BItemIssueOnProduction,";
                    sqlText += "@BItemBranchId,";
                    sqlText += "@BItemTransactionType";

                    sqlText += ")	";




                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", bomMaster.BOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterVATName", bomMaster.VATName ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkuAmount", bomMaster.MarkupValue);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", OrdinaryVATDesktop.DateToDate(BItem.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(BItem.LastModifiedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPost", BItem.Post);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMaterCustomerID", bomMaster.CustomerID ?? "0");
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemBranchId", BItem.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType);

                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + BItem.RawItemName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                }

                #endregion

                #endregion Insert Detail Table

                #region Insert BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                    "') and VAT Name ( '" + bomMaster.VATName +
                                                    "' ) not save in date date ('" + bomMaster.EffectDate + "') .");
                }


                int OHLineNo = 0;

                foreach (var OHItem in bomOHs.ToList())
                {

                    #region Raw Product Exists

                    //if (OHItem.HeadName.Trim() != "Margin")
                    //{

                    sqlText = "";
                    sqlText = "select count(ItemNo) from Products ";
                    sqlText += " where ProductName =@OHItemHeadName  ";

                    SqlCommand cmdfindRItem = new SqlCommand(sqlText, currConn);
                    cmdfindRItem.Transaction = transaction;
                    cmdfindRItem.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);

                    int findRItem = (int)cmdfindRItem.ExecuteScalar();

                    if (findRItem <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration Overhead not in database please check the Overhead Item ('" +
                                                        OHItem.HeadName + "') Code ('" +
                                                        OHItem.OHCode + "') ");
                    }
                    //}
                    //GetProductNo

                    #endregion Raw Product Exists

                    #region Generate Overhead Id

                    sqlText = "";
                    sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                    SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                    cmdGenOHId.Transaction = transaction;
                    int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                    if (nextOHId <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Sorry,Unable to generate Overhead Id.");
                    }

                    #endregion Generate Overhead Id

                    OHLineNo++;

                    #region Find Transaction Exist // Commented




                    //////                    sqlText = "";
                    //////                    sqlText += @"
                    //////SELECT COUNT(HeadName) from BOMCompanyOverhead 
                    //////WHERE 1=1 
                    //////AND FinishItemNo=@bomMasterItemNo 
                    //////AND HeadName=@OHItemHeadName 
                    //////AND EffectDate=@bomMasterEffectDate  
                    //////AND VATName=@bomMasterVATName 
                    //////AND HeadID=@HeadID
                    //////";

                    //////                    if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    //////                    { }
                    //////                    else
                    //////                    {
                    //////                        sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                    //////                    }
                    //////                    SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                    //////                    cmdFindId.Transaction = transaction;
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                    //////                    cmdFindId.Parameters.AddWithValueAndNullHandle("@HeadID", OHItem.HeadID);

                    //////                    IDExist = (int)cmdFindId.ExecuteScalar();
                    //////                    if (bomMaster.VATName != "VAT 4.3 (Tender)")
                    //////                    {
                    //////                        if (IDExist > 0)
                    //////                        {
                    //////                            throw new ArgumentNullException("BOM Insert", MessageVM.PurchasemsgFindExistID + ", Material name '(" + OHItem.HeadName + ")'");
                    //////                        }
                    //////                    }

                    #endregion Find Transaction Exist

                    #region Insert only OH



                    sqlText = "";
                    sqlText += " insert into BOMCompanyOverhead(";
                    sqlText += " BOMOverHeadId,";
                    sqlText += " BOMId,";
                    sqlText += " HeadID,";
                    sqlText += " HeadName,";
                    sqlText += " HeadAmount,";
                    sqlText += " CreatedBy,";
                    sqlText += " CreatedOn,";
                    sqlText += " LastModifiedBy,";
                    sqlText += " LastModifiedOn,";
                    sqlText += " FinishItemNo,";
                    sqlText += " EffectDate,";
                    sqlText += " OHLineNo,";
                    sqlText += " VATName,";
                    sqlText += " RebatePercent, ";
                    sqlText += " RebateAmount,";
                    sqlText += " AdditionalCost, ";
                    sqlText += " CustomerID, ";
                    sqlText += " BranchId, ";
                    sqlText += " Post ";
                    sqlText += " )";
                    sqlText += " values(	";
                    sqlText += "@nextOHId,";
                    sqlText += "@nextBOMId,";
                    sqlText += "@OHItemHeadID,";
                    sqlText += "@OHItemHeadName,";
                    sqlText += "@OHItemHeadAmount,";
                    sqlText += "@OHItemCreatedBy,";
                    sqlText += "@OHItemCreatedOn,";
                    sqlText += "@OHItemLastModifiedBy,";
                    sqlText += "@OHItemLastModifiedOn,";
                    sqlText += "@bomMasterItemNo,";
                    sqlText += "@bomMasterEffectDate,";
                    sqlText += "@OHLineNo,";
                    sqlText += "@bomMasterVATName,";
                    sqlText += "@OHItemRebatePercent,";
                    sqlText += "@OHItemRebateAmount,";
                    sqlText += "@OHItemAdditionalCost,";
                    sqlText += "@bomMasterCustomerID,";
                    sqlText += "@bomMasterBranchId,";
                    sqlText += "@OHItemPost";

                    sqlText += ")	";


                    SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    cmdInsDetail.Transaction = transaction;
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMId", bomMaster.BOMId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OrdinaryVATDesktop.DateToDate(OHItem.CreatedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OrdinaryVATDesktop.DateToDate(OHItem.LastModifiedOn));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                    cmdInsDetail.Parameters.AddWithValue("@bomMasterCustomerID", bomMaster.CustomerID ?? "0");
                    cmdInsDetail.Parameters.AddWithValue("@bomMasterBranchId", bomMaster.BranchId);
                    cmdInsDetail.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);


                    transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert + "sql" + sqlText,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') and VAT Name ( '" + bomMaster.VATName +
                                                        "' ) and Material Name ('" + OHItem.HeadName +
                                                        "') not save in date date ('" + bomMaster.EffectDate + "') .");
                    }

                    #endregion Insert only OH
                }

                #endregion Insert BOMCompanyOverhead Table

                #region Update WIPBOMId

                sqlText = "";

                #region Get WIP Items

                sqlText = @"
select RawItemNo WIPItemNo from BOMRaws
where 1=1 and BOMId=@BOMId
and RawItemType='WIP'
";

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@BOMId", bomMaster.BOMId);

                DataTable dtWIPItem = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtWIPItem);

                #endregion

                #region Update WIP BOMId

                if (dtWIPItem != null && dtWIPItem.Rows.Count > 0)
                {

                    sqlText = "";
                    sqlText = @"
------select WIPBOMId, a.BOMId  

update BOMRaws set WIPBOMId=a.BOMId  
from BOMRaws bmr

left outer join (
select top 1 BOMId, RawItemNo, FinishItemNo from BOMRaws
where 1=1 
and FinishItemNo=@WIPItemNo
and EffectDate<=@EffectDate
and post='Y'
order by EffectDate desc
) as a on bmr.RawItemNo=a.FinishItemNo
where 1=1 and bmr.RawItemNo=@WIPItemNo
and bmr.BOMId=@BOMId
";

                    foreach (DataRow dr in dtWIPItem.Rows)
                    {
                        cmd = new SqlCommand(sqlText, currConn, transaction);
                        cmd.Parameters.AddWithValue("@WIPItemNo", dr["WIPItemNo"].ToString());
                        cmd.Parameters.AddWithValue("@EffectDate", bomMaster.EffectDate);
                        cmd.Parameters.AddWithValue("@BOMId", bomMaster.BOMId);

                        transResult = cmd.ExecuteNonQuery();

                    }

                }


                #endregion




                #endregion



                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.bomMsgSaveSuccessfully;
                //nextBOMId
                retResults[2] = "" + bomMaster.BOMId;
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
                //    retResults[0] = "Fail";//Success or Fail
                //    retResults[1] = ex.Message.Split(new[] { '\r', '\n' }).FirstOrDefault(); //catch ex
                //    retResults[2] = ""; //catch ex

                ////FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

                FileLogger.Log("BOMDAL", "BOMInsert2", ex.ToString() + "\n" + sqlText);

                return retResults;
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


        public string[] BOMUpdateX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = bomMaster.BOMId;


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string VATName = "";


            #endregion Initializ

            #region Try

            try
            {


                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                    }

                    #endregion YearLock

                    #region YearNotExist

                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                VATName = bomMaster.VATName;
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region update BOM Table

                if (bomItems == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }




                decimal LastNBRPrice = 0;
                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;
                int BOMLineNo = 0;

                #region Find Last Declared NBRPrice

                var vFinishItemNo = bomItems.First().FinishItemNo;
                var vEffectDate = bomItems.First().EffectDate;
                sqlText = "";
                sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPrice.Transaction = transaction;
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                if (objLastNBRPrice != null)
                {
                    LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                }

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", vEffectDate);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }


                #endregion Find Last Declared NBRPrice

                bomMaster.LastNBRPrice = LastNBRPrice;
                bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                bomMaster.LastSDAmount = LastSDAmount;
                bomMaster.LastMarkupValue = LastMarkupAmount;





                #region BOM Master Update

                sqlText = "";

                sqlText += " update BOMs set  ";
                sqlText += " EffectDate=@bomMasterEffectDate,";
                sqlText += " FirstSupplyDate=@bomMasterFirstSupplyDate,";
                sqlText += " VATName=@bomMasterVATName,";
                sqlText += " VATRate=@bomMasterVATRate ,";
                sqlText += " UOM=@bomMasterUOM,";
                sqlText += " SD=@bomMasterSDRate ,";
                sqlText += " TradingMarkUp=@bomMasterTradingMarkup ,";
                sqlText += " Comments=@bomMasterComments,";
                sqlText += " ActiveStatus=@bomMasterActiveStatus,";
                sqlText += " LastModifiedBy=@bomMasterLastModifiedBy,";
                sqlText += " LastModifiedOn=@bomMasterLastModifiedOn,";
                sqlText += " RawTotal=@bomMasterRawTotal ,";
                sqlText += " PackingTotal=@bomMasterPackingTotal ,";
                sqlText += " RebateTotal=@bomMasterRebateTotal ,";
                sqlText += " AdditionalTotal=@bomMasterAdditionalTotal ,";
                sqlText += " RebateAdditionTotal=@bomMasterRebateAdditionTotal ,";
                sqlText += " NBRPrice=@bomMasterPNBRPrice ,";
                sqlText += " PacketPrice=@bomMasterPPacketPrice ,";
                sqlText += " RawOHCost=@bomMasterRawOHCost ,";
                sqlText += " LastNBRPrice=@bomMasterLastNBRPrice ,";
                sqlText += " LastNBRWithSDAmount=@bomMasterLastNBRWithSDAmount ,";
                sqlText += " TotalQuantity=@bomMasterTotalQuantity, ";
                sqlText += " SDAmount=@bomMasterSDAmount, ";
                sqlText += " VATAmount=@bomMasterVatAmount, ";
                sqlText += " WholeSalePrice=@bomMasterWholeSalePrice, ";
                sqlText += " NBRWithSDAmount=@bomMasterNBRWithSDAmount, ";
                sqlText += " MarkUpValue=@bomMasterMarkupValue, ";
                sqlText += " LastMarkUpValue=@bomMasterLastMarkupValue, ";
                sqlText += " LastSDAmount=@bomMasterLastSDAmount, ";
                sqlText += " LastAmount=@bomMasterLastAmount ";
                sqlText += " where 1=1";
                sqlText += " and BOMId=@bomMasterBOMId";
                //sqlText += " and EffectDate=@bomMasterEffectDate";
                //sqlText += " AND VATName=@VATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }

                SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                cmdMasterUpdate.Transaction = transaction;
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterFirstSupplyDate", OrdinaryVATDesktop.DateToDate(bomMaster.FirstSupplyDate));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedOn));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastAmount", bomMaster.LastAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                //cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                }


                #endregion BOM Master Update


                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM BOMRaws WHERE BOMId=@bomMasterBOMId ";
                sqlText += @" delete FROM BOMCompanyOverhead WHERE BOMId=@bomMasterBOMId ";
                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                foreach (var BItem in bomItems.ToList())
                {
                    //BOMLineNo++;
                    #region Comments
                    if (false)
                    {
                        ////To be obsolete

                        #region Find Transaction Exist

                        sqlText = "";
                        sqlText += "SELECT COUNT(FinishItemNo) from BOMRaws ";
                        sqlText += " WHERE 1=1";
                        sqlText += " AND BOMId=@bomMasterBOMId";

                        //////sqlText += " FinishItemNo=@bomMasterItemNo ";
                        //////sqlText += " and EffectDate=@bomMasterEffectDate ";
                        //////sqlText += " AND VATName=@VATName ";

                        //sqlText += " BOMId ='" + bomMaster.BOMId + "'  ";
                        sqlText += " AND RawItemNo='" + BItem.RawItemNo + "'";
                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                        }
                        //sqlText += " AND VATName='" + VATName + "' ";
                        SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        cmdFindId.Transaction = transaction;
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                        cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                        int IDExist1 = (int)cmdFindId.ExecuteScalar();

                        #endregion Find Transaction Exist
                    }
                    #endregion

                    //////if (IDExist1 <= 0)
                    //////{
                    #region Insert

                    {
                        #region Generate BOMRaw Id

                        int nextBOMRawId = 0;
                        sqlText = "";
                        sqlText = "select isnull(max(cast(BOMRawId as int)),0)+1 FROM  BOMRaws";
                        SqlCommand cmdGenRawId = new SqlCommand(sqlText, currConn);
                        cmdGenRawId.Transaction = transaction;
                        nextBOMRawId = (int)cmdGenRawId.ExecuteScalar();

                        if (nextBOMRawId <= 0)
                        {
                            throw new ArgumentNullException("BOMUpdate", "Sorry,Unable to generate BOMRawId.");
                        }

                        #endregion Generate BOMRaw Id

                        #region Insert only BOM

                        sqlText = "";
                        sqlText += " insert into BOMRaws(";
                        sqlText += " BOMRawId,";
                        sqlText += " BOMId,";
                        sqlText += " BOMLineNo,";
                        sqlText += " FinishItemNo,";
                        sqlText += " RawItemNo,";
                        sqlText += " RawItemType,";
                        sqlText += " EffectDate,";
                        sqlText += " VATName,";
                        sqlText += " UseQuantity,";
                        sqlText += " WastageQuantity,";
                        sqlText += " Cost,";
                        sqlText += " UOM,";
                        sqlText += " VATRate,";
                        sqlText += " VATAmount,";
                        sqlText += " SD,";
                        sqlText += " SDAmount,";
                        sqlText += " TradingMarkUp,";
                        sqlText += " RebateRate,";

                        sqlText += " MarkUpValue,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " UnitCost,";
                        sqlText += " UOMn,";
                        sqlText += " UOMc,";
                        sqlText += " UOMPrice,";
                        sqlText += " UOMUQty,";
                        sqlText += " UOMWQty,";
                        sqlText += " TotalQuantity,";
                        sqlText += " PBOMId,";
                        sqlText += " PInvoiceNo,";
                        sqlText += " Post,";
                        sqlText += " CustomerID,";
                        sqlText += " IssueOnProduction,";
                        sqlText += " BranchId,";
                        sqlText += " TransactionType";



                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@nextBOMRawId,";
                        sqlText += "@bomMasterBOMId,";
                        sqlText += "@BItemBOMLineNo,";
                        sqlText += "@bomMasterItemNo,";
                        sqlText += "@BItemRawItemNo,";
                        sqlText += "@BItemRawItemType,";
                        sqlText += "@bomMasterEffectDate,";
                        sqlText += "@bomMasterVATName,";
                        sqlText += "@BItemUseQuantity,";
                        sqlText += "@BItemWastageQuantity,";
                        sqlText += "@BItemCost,";
                        sqlText += "@BItemUOM,";
                        sqlText += "@BItemVATRate,";
                        sqlText += "@BItemVatAmount,";
                        sqlText += "@BItemSD,";
                        sqlText += "@BItemSDAmount,";
                        sqlText += "@BItemTradingMarkUp,";
                        sqlText += "@BItemRebateRate,";
                        sqlText += "@MarkupAmount,";
                        sqlText += "@BItemCreatedBy,";
                        sqlText += "@BItemCreatedOn,";
                        sqlText += "@BItemLastModifiedBy,";
                        sqlText += "@BItemLastModifiedOn,";
                        sqlText += "@BItemUnitCost,";
                        sqlText += "@BItemUOMn,";
                        sqlText += "@BItemUOMc,";
                        sqlText += "@BItemUOMPrice,";
                        sqlText += "@BItemUOMUQty,";
                        sqlText += "@BItemUOMWQty,";
                        sqlText += "@BItemTotalQuantity,";
                        sqlText += "@BItemPBOMId,";
                        sqlText += "@BItemPInvoiceNo,";
                        sqlText += "@BItemPost,";
                        sqlText += "@bomMasterCustomerID,";
                        sqlText += "@BItemIssueOnProduction,";
                        sqlText += "@BItemBranchId,";
                        sqlText += "@BItemTransactionType";

                        sqlText += ")	";

                        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                        cmdInsDetail.Transaction = transaction;
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@nextBOMRawId", nextBOMRawId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemBOMLineNo", BItem.BOMLineNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@MarkupAmount", MarkupAmount);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedBy", BItem.CreatedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemCreatedOn", BItem.CreatedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemPost", BItem.Post);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemBranchId", BItem.BranchId);
                        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType);

                        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException("BOMUpdate", MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                        #endregion Insert only BOM
                    }
                    #endregion Not Exist then Insert


                    ////}
                    ////else
                    ////{
                    #region Comments
                    if (false)
                    {
                        ////To be obsolete
                        #region else Update



                        sqlText = "";

                        sqlText += " update BOMRaws set  ";
                        sqlText += " BOMLineNo          =@BOMLineNo ,";
                        sqlText += " RawItemType        =@BItemRawItemType ,";
                        sqlText += " EffectDate         =@bomMasterEffectDate ,";
                        sqlText += " VATName            =@bomMasterVATName ,";
                        sqlText += " UseQuantity        =@BItemUseQuantity ,";
                        sqlText += " WastageQuantity    =@BItemWastageQuantity ,";
                        sqlText += " Cost               =@BItemCost ,";
                        sqlText += " UOM                =@BItemUOM ,";
                        sqlText += " VATRate            =@BItemVATRate ,";
                        sqlText += " VATAmount          =@BItemVatAmount, ";
                        sqlText += " SD                 =@BItemSD ,";
                        sqlText += " SDAmount           =@BItemSDAmount, ";
                        sqlText += " TradingMarkUp      =@BItemTradingMarkUp ,";
                        sqlText += " RebateRate         =@BItemRebateRate ,";
                        sqlText += " MarkUpValue        =@BItemMarkUpValue ,";
                        sqlText += " LastModifiedBy     =@BItemLastModifiedBy ,";
                        sqlText += " LastModifiedOn     =@BItemLastModifiedOn ,";
                        sqlText += " UnitCost           =@BItemUnitCost ,";
                        sqlText += " UOMn               =@BItemUOMn ,";
                        sqlText += " UOMc               =@BItemUOMc ,";
                        sqlText += " UOMPrice           =@BItemUOMPrice ,";
                        sqlText += " UOMUQty            =@BItemUOMUQty ,";
                        sqlText += " UOMWQty            =@BItemUOMWQty ,";
                        sqlText += " PBOMId             =@BItemPBOMId ,";
                        sqlText += " PInvoiceNo         =@BItemPInvoiceNo ,";
                        sqlText += " TotalQuantity      =@BItemTotalQuantity, ";
                        sqlText += " IssueOnProduction  =@BItemIssueOnProduction, ";
                        sqlText += " TransactionType    =@BItemTransactionType ";

                        sqlText += " WHERE  ";

                        sqlText += " FinishItemNo       =@bomMasterItemNo";
                        sqlText += " and EffectDate     =@bomMasterEffectDate";
                        sqlText += " AND VATName        =@bomMasterVATName ";
                        sqlText += " AND RawItemNo      =@BItemRawItemNo ";

                        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        { }
                        else
                        {
                            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                        }

                        SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        cmdUpdate.Transaction = transaction;

                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BOMLineNo", BOMLineNo);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRawItemType", BItem.RawItemType ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUseQuantity", BItem.UseQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemWastageQuantity", BItem.WastageQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemCost", BItem.Cost);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOM", BItem.UOM ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemVATRate", BItem.VATRate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemVatAmount", BItem.VatAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemSD", BItem.SD);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemSDAmount", BItem.SDAmount);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTradingMarkUp", BItem.TradingMarkUp);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRebateRate", BItem.RebateRate);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemMarkUpValue", BItem.MarkUpValue);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedBy", BItem.LastModifiedBy ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemLastModifiedOn", BItem.LastModifiedOn);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUnitCost", BItem.UnitCost);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMn", BItem.UOMn ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMc", BItem.UOMc);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMPrice", BItem.UOMPrice);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMUQty", BItem.UOMUQty);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemUOMWQty", BItem.UOMWQty);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemPBOMId", BItem.PBOMId ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemPInvoiceNo", BItem.PInvoiceNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTotalQuantity", BItem.TotalQuantity);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemIssueOnProduction", BItem.IssueOnProduction);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemTransactionType", BItem.TransactionType ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@BItemRawItemNo", BItem.RawItemNo ?? Convert.DBNull);
                        cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);



                        transResult = (int)cmdUpdate.ExecuteNonQuery();
                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                            MessageVM.PurchasemsgUpdateNotSuccessfully);
                        }



                        #endregion else Update
                    }
                    #endregion

                    ////}




                }

                #region Comments
                if (false)
                {
                    ////////To be obsolete
                    ////#region Remove row at BOMRaws

                    ////sqlText = "";
                    ////sqlText += " SELECT  distinct RawItemNo";
                    ////sqlText += " from BOMRaws WHERE  ";
                    ////sqlText += " FinishItemNo=@bomMasterItemNo ";
                    ////sqlText += " and EffectDate=@bomMasterEffectDate";
                    ////sqlText += " AND VATName=@bomMasterVATName ";
                    ////if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    ////{ }
                    ////else
                    ////{
                    ////    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    ////}
                    ////DataTable dt = new DataTable("Previous");
                    ////SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                    ////cmdRIF.Transaction = transaction;
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                    ////SqlDataAdapter dta = new SqlDataAdapter(cmdRIF);
                    ////dta.Fill(dt);
                    ////foreach (DataRow pItem in dt.Rows)
                    ////{
                    ////    var p = pItem["RawItemNo"].ToString();

                    ////    //var tt= Details.Find(x => x.ItemNo == p);
                    ////    var tt = bomItems.Count(x => x.RawItemNo.Trim() == p.Trim());
                    ////    if (tt == 0)
                    ////    {
                    ////        sqlText = "";
                    ////        sqlText += " delete FROM BOMRaws ";
                    ////        sqlText += " WHERE BOMId =@bomMasterBOMId";
                    ////        sqlText += " AND RawItemNo=@p";
                    ////        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    ////        { }
                    ////        else
                    ////        {
                    ////            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                    ////        }
                    ////        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    ////        cmdInsDetail.Transaction = transaction;
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    ////        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    ////    }

                    ////}

                    ////#endregion Remove row at BOMRaws
                }
                #endregion

                #endregion update BOM Table

                #region update BOMCompanyOverhead Table

                if (bomOHs == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }
                int OHLineNo = 0;
                foreach (var OHItem in bomOHs.ToList())
                {


                    OHLineNo++;

                    #region Comments

                    if (false)
                    {
                        ////To be obsolete
                        ////#region Find Transaction Exist

                        ////sqlText = "";
                        ////sqlText += "select COUNT(HeadName) from BOMCompanyOverhead WHERE ";

                        ////sqlText += " FinishItemNo=@bomMasterItemNo";
                        ////sqlText += " and EffectDate=@bomMasterEffectDate";
                        ////sqlText += " AND VATName=@bomMasterVATName";
                        ////if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        ////{ }
                        ////else
                        ////{
                        ////    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                        ////}
                        //////sqlText += " BOMId ='" + bomMaster.BOMId + "'  ";

                        ////sqlText += " AND HeadID=@OHItemHeadID";
                        ////SqlCommand cmdFindId = new SqlCommand(sqlText, currConn);
                        ////cmdFindId.Transaction = transaction;
                        ////cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                        ////cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        ////cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                        ////cmdFindId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                        ////cmdFindId.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);

                        ////int IDExist1 = (int)cmdFindId.ExecuteScalar();

                        ////#endregion Find Transaction Exist
                    }
                    #endregion

                    ////if (IDExist1 <= 0)
                    {
                        #region Generate Overhead Id

                        sqlText = "";
                        sqlText = "select isnull(max(cast(BOMOverHeadId as int)),0)+1 FROM  BOMCompanyOverhead";
                        SqlCommand cmdGenOHId = new SqlCommand(sqlText, currConn);
                        cmdGenOHId.Transaction = transaction;
                        int nextOHId = (int)cmdGenOHId.ExecuteScalar();

                        if (nextOHId <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            "Sorry,Unable to generate Overhead Id.");
                        }

                        #endregion Generate Overhead Id

                        #region Not Exist then Insert

                        #region Insert only OH

                        decimal vRebateAmount = 0;
                        decimal vAddingAmount = 0;


                        sqlText = "";
                        sqlText += " insert into BOMCompanyOverhead(";
                        sqlText += " BOMOverHeadId,";
                        sqlText += " BOMId,";
                        sqlText += " HeadID,";
                        sqlText += " HeadName,";
                        sqlText += " HeadAmount,";
                        sqlText += " CreatedBy,";
                        sqlText += " CreatedOn,";
                        sqlText += " LastModifiedBy,";
                        sqlText += " LastModifiedOn,";
                        sqlText += " FinishItemNo,";
                        sqlText += " EffectDate,";
                        sqlText += " OHLineNo,";
                        sqlText += " VATName,";
                        sqlText += " RebatePercent, ";
                        sqlText += " RebateAmount,";
                        sqlText += " AdditionalCost, ";
                        sqlText += " CustomerID, ";
                        sqlText += " BranchId, ";
                        sqlText += " Post ";
                        sqlText += " )";
                        sqlText += " values(	";
                        sqlText += "@nextOHId,";
                        sqlText += "@bomMasterBOMId,";
                        sqlText += "@OHItemHeadID,";
                        sqlText += "@OHItemHeadName,";
                        sqlText += "@OHItemHeadAmount,";
                        sqlText += "@OHItemCreatedBy,";
                        sqlText += "@OHItemCreatedOn,";
                        sqlText += "@OHItemLastModifiedBy,";
                        sqlText += "@OHItemLastModifiedOn,";
                        sqlText += "@OHItemFinishItemNo,";
                        sqlText += "@bomMasterEffectDate,";
                        sqlText += "@OHLineNo,";
                        sqlText += "@VATName,";
                        sqlText += "@OHItemRebatePercent,";
                        sqlText += "@OHItemRebateAmount,";
                        sqlText += "@OHItemAdditionalCost,";
                        sqlText += "@bomMasterCustomerID,";
                        sqlText += "@bomMasterBranchId,";
                        sqlText += "@OHItemPost";

                        sqlText += ")	";


                        SqlCommand cmdInsDetailOH = new SqlCommand(sqlText, currConn);
                        cmdInsDetailOH.Transaction = transaction;
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@nextOHId", nextOHId);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemCreatedBy", OHItem.CreatedBy);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemCreatedOn", OHItem.CreatedOn);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemFinishItemNo", OHItem.FinishItemNo);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@bomMasterBranchId", bomMaster.BranchId);
                        cmdInsDetailOH.Parameters.AddWithValueAndNullHandle("@OHItemPost", OHItem.Post);

                        transResult = cmdInsDetailOH.ExecuteNonQuery();

                        if (transResult <= 0)
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.PurchasemsgSaveNotSuccessfully);
                        }

                        #endregion Insert only OH

                        #endregion Not Exist then Insert
                    }

                    #region Comments

                    if (false)
                    {
                        ////else
                        ////To be obsolete
                        //////#region else Update

                        //////sqlText = "";

                        //////sqlText += " update BOMCompanyOverhead set  ";
                        //////sqlText += " OHLineNo           = @OHLineNo  ,";
                        //////sqlText += " EffectDate         = @OHItemEffectDate  ,";
                        //////sqlText += " VATName            = @VATName  ,";
                        //////sqlText += " HeadName           = @OHItemHeadName  ,";
                        //////sqlText += " HeadAmount         = @OHItemHeadAmount  ,";
                        //////sqlText += " LastModifiedBy     = @OHItemLastModifiedBy  ,";
                        //////sqlText += " LastModifiedOn     = @OHItemLastModifiedOn , ";
                        //////sqlText += " RebatePercent      = @OHItemRebatePercent  ,";
                        //////sqlText += " RebateAmount       = @OHItemRebateAmount , ";
                        //////sqlText += " AdditionalCost     = @OHItemAdditionalCost  ";
                        //////sqlText += " where  HeadID      = @OHItemHeadID ";
                        //////sqlText += " and FinishItemNo   = @OHItemFinishItemNo ";
                        //////sqlText += " and EffectDate     = @OHItemEffectDate ";
                        //////sqlText += " and VATName        = @VATName ";
                        //////if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                        //////{ }
                        //////else
                        //////{
                        //////    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                        //////}

                        //////SqlCommand cmdUpdate = new SqlCommand(sqlText, currConn);
                        //////cmdUpdate.Transaction = transaction;

                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHLineNo", OHLineNo);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemEffectDate", OHItem.EffectDate);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName ?? Convert.DBNull);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadName", OHItem.HeadName ?? Convert.DBNull);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadAmount", OHItem.HeadAmount);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedBy", OHItem.LastModifiedBy ?? Convert.DBNull);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemLastModifiedOn", OHItem.LastModifiedOn);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemRebatePercent", OHItem.RebatePercent);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemRebateAmount", OHItem.RebateAmount);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemAdditionalCost", OHItem.AdditionalCost);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemHeadID", OHItem.HeadID ?? Convert.DBNull);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@OHItemFinishItemNo", OHItem.FinishItemNo ?? Convert.DBNull);
                        //////cmdUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID ", bomMaster.CustomerID ?? Convert.DBNull);

                        //////transResult = (int)cmdUpdate.ExecuteNonQuery();
                        //////if (transResult <= 0)
                        //////{
                        //////    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                        //////                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                        //////}

                        //////#endregion else Update
                    }
                    #endregion


                }

                #endregion update BOMCompanyOverhead Table

                #region Comments

                if (false)
                {
                    ////#region Remove row at BOMCompanyOverhead

                    ////sqlText = "";
                    ////sqlText += " SELECT  distinct HeadID";
                    ////sqlText += " from BOMCompanyOverhead WHERE  ";
                    ////sqlText += " FinishItemNo=@bomMasterItemNo  ";
                    ////sqlText += " and EffectDate=@bomMasterEffectDate ";
                    ////sqlText += " AND VATName=@bomMasterVATName  ";
                    ////if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    ////{ }
                    ////else
                    ////{
                    ////    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID  ";
                    ////}
                    ////DataTable dt = new DataTable("Previous");
                    ////SqlCommand cmdRIF = new SqlCommand(sqlText, currConn);
                    ////cmdRIF.Transaction = transaction;
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    ////cmdRIF.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                    ////dta = new SqlDataAdapter(cmdRIF);
                    ////dta.Fill(dt);
                    ////foreach (DataRow pHeadID in dt.Rows)
                    ////{
                    ////    var p = pHeadID["HeadID"].ToString();
                    ////    var tt = bomOHs.Count(x => x.HeadID.Trim() == p.Trim());
                    ////    if (tt == 0)
                    ////    {
                    ////        sqlText = "";
                    ////        sqlText += " delete FROM BOMCompanyOverhead WHERE ";

                    ////        sqlText += " FinishItemNo=@bomMasterItemNo  ";
                    ////        sqlText += " and EffectDate=@bomMasterEffectDate ";
                    ////        sqlText += " AND VATName=@bomMasterVATName  ";

                    ////        //sqlText += " WHERE BOMId =@bomMaster.BOMId   ";
                    ////        sqlText += " AND HeadID=@p ";
                    ////        if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                    ////        { }
                    ////        else
                    ////        {
                    ////            sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                    ////        }
                    ////        SqlCommand cmdInsDetail = new SqlCommand(sqlText, currConn);
                    ////        cmdInsDetail.Transaction = transaction;
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@p", p);
                    ////        cmdInsDetail.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                    ////        transResult = (int)cmdInsDetail.ExecuteNonQuery();

                    ////    }

                    ////}

                    ////#endregion Remove row at BOMCompanyOverhead
                }
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
                retResults[1] = MessageVM.bomMsgUpdateSuccessfully;

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
                transaction.Rollback();

                FileLogger.Log("BOMDAL", "BOMUpdateX", ex.ToString() + "\n" + sqlText);

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

        //
        public string[] BOMUpdate(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = bomMaster.BOMId;


            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;
            string sqlText = "";

            string VATName = "";


            #endregion Initializ

            #region Try

            try
            {


                #region open connection and transaction

                if (bomItems == null && !bomItems.Any())
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    "Sorry,No Item found to insert.");
                }
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                    }

                    #endregion YearLock

                    #region YearNotExist

                    sqlText = "";
                    sqlText = sqlText + "select  min(PeriodStart) MinDate, max(PeriodEnd)  MaxDate from fiscalyear";

                    DataTable dtYearNotExist = new DataTable("ProductDataT");

                    SqlCommand cmdYearNotExist = new SqlCommand(sqlText, currConn);
                    cmdYearNotExist.Transaction = transaction;

                    SqlDataAdapter YearNotExistDataAdapt = new SqlDataAdapter(cmdYearNotExist);
                    YearNotExistDataAdapt.Fill(dtYearNotExist);

                    if (dtYearNotExist == null)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK

                #region CheckVATName

                VATName = bomMaster.VATName;
                if (string.IsNullOrEmpty(VATName))
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, MessageVM.msgVatNameNotFound);
                }

                #endregion CheckVATName

                #region update BOM Table

                if (bomItems == null)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                    MessageVM.PurchasemsgNoDataToSave);
                }


                decimal LastNBRPrice = 0;
                decimal LastNBRWithSDAmount = 0;
                decimal LastMarkupAmount = 0;
                decimal LastSDAmount = 0;
                decimal MarkupAmount = bomMaster.MarkupValue;

                #region Find Last Declared NBRPrice

                var vFinishItemNo = bomItems.First().FinishItemNo;
                var vEffectDate = bomItems.First().EffectDate;
                sqlText = "";
                sqlText += "select top 1 NBRPrice from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " AND ISNULL(ReferenceNo,'NA')=@ReferenceNo";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRPrice = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRPrice.Transaction = transaction;
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                cmdFindLastNBRPrice.Parameters.AddWithValueAndNullHandle("@ReferenceNo", bomMaster.ReferenceNo);

                object objLastNBRPrice = cmdFindLastNBRPrice.ExecuteScalar();
                if (objLastNBRPrice != null)
                {
                    LastNBRPrice = Convert.ToDecimal(objLastNBRPrice);
                }

                sqlText = "";
                sqlText += "select top 1 NBRWithSDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastNBRWithSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastNBRWithSDAmount.Transaction = transaction;
                cmdFindLastNBRWithSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastNBRWithSDAmount = cmdFindLastNBRWithSDAmount.ExecuteScalar();
                if (objLastNBRWithSDAmount != null)
                {
                    LastNBRWithSDAmount = Convert.ToDecimal(objLastNBRWithSDAmount);
                }
                sqlText = "";
                sqlText += "select top 1 SDAmount from BOMs WHERE FinishItemNo='" + vFinishItemNo + "' ";
                sqlText += " AND EffectDate<'" + OrdinaryVATDesktop.DateToDate(vEffectDate) + "' ";
                sqlText += " AND VATName='" + VATName + "' ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastSDAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastSDAmount.Transaction = transaction;
                cmdFindLastSDAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastSDAmount = cmdFindLastSDAmount.ExecuteScalar();
                if (objLastSDAmount != null)
                {
                    LastSDAmount = Convert.ToDecimal(objLastSDAmount);
                }

                sqlText = "";
                sqlText += "select top 1 MarkUpValue from BOMs WHERE FinishItemNo=@vFinishItemNo ";
                sqlText += " AND EffectDate<@vEffectDate ";
                sqlText += " AND VATName=@VATName ";
                sqlText += " and Post='Y'";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                { }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " order by EffectDate desc";
                SqlCommand cmdFindLastMarkupAmount = new SqlCommand(sqlText, currConn);
                cmdFindLastMarkupAmount.Transaction = transaction;
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vFinishItemNo", vFinishItemNo);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@vEffectDate", OrdinaryVATDesktop.DateToDate(vEffectDate));
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                cmdFindLastMarkupAmount.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                object objLastMarkupAmount = cmdFindLastMarkupAmount.ExecuteScalar();
                if (objLastMarkupAmount != null)
                {
                    LastMarkupAmount = Convert.ToDecimal(objLastMarkupAmount);
                }

                #endregion Find Last Declared NBRPrice

                bomMaster.LastNBRPrice = LastNBRPrice;
                bomMaster.LastNBRWithSDAmount = LastNBRWithSDAmount;
                bomMaster.LastSDAmount = LastSDAmount;
                bomMaster.LastMarkupValue = LastMarkupAmount;


                #region BOM Master Update

                sqlText = "";

                sqlText += " update BOMs set  ";
                sqlText += " EffectDate=@bomMasterEffectDate,";
                sqlText += " ReferenceNo=@ReferenceNo,";


                sqlText += " FirstSupplyDate=@bomMasterFirstSupplyDate,";
                sqlText += " VATName=@bomMasterVATName,";
                sqlText += " VATRate=@bomMasterVATRate ,";
                sqlText += " UOM=@bomMasterUOM,";
                sqlText += " SD=@bomMasterSDRate ,";
                sqlText += " TradingMarkUp=@bomMasterTradingMarkup ,";
                sqlText += " Comments=@bomMasterComments,";
                sqlText += " ActiveStatus=@bomMasterActiveStatus,";
                sqlText += " LastModifiedBy=@bomMasterLastModifiedBy,";
                sqlText += " LastModifiedOn=@bomMasterLastModifiedOn,";
                sqlText += " RawTotal=@bomMasterRawTotal ,";
                sqlText += " PackingTotal=@bomMasterPackingTotal ,";
                sqlText += " RebateTotal=@bomMasterRebateTotal ,";
                sqlText += " AdditionalTotal=@bomMasterAdditionalTotal ,";
                sqlText += " RebateAdditionTotal=@bomMasterRebateAdditionTotal ,";
                sqlText += " NBRPrice=@bomMasterPNBRPrice ,";
                sqlText += " PacketPrice=@bomMasterPPacketPrice ,";
                sqlText += " RawOHCost=@bomMasterRawOHCost ,";
                sqlText += " LastNBRPrice=@bomMasterLastNBRPrice ,";
                sqlText += " LastNBRWithSDAmount=@bomMasterLastNBRWithSDAmount ,";
                sqlText += " TotalQuantity=@bomMasterTotalQuantity, ";
                sqlText += " SDAmount=@bomMasterSDAmount, ";
                sqlText += " VATAmount=@bomMasterVatAmount, ";
                sqlText += " WholeSalePrice=@bomMasterWholeSalePrice, ";
                sqlText += " NBRWithSDAmount=@bomMasterNBRWithSDAmount, ";
                sqlText += " MarkUpValue=@bomMasterMarkupValue, ";
                sqlText += " LastMarkUpValue=@bomMasterLastMarkupValue, ";
                sqlText += " LastSDAmount=@bomMasterLastSDAmount, ";
                sqlText += " LastAmount=@bomMasterLastAmount, ";
                sqlText += " AutoIssue=@AutoIssue, ";

                sqlText += " CustomerID=@CustomerID, ";


                sqlText += " UOMc=@UOMc, ";
                sqlText += " UOMn=@UOMn, ";
                sqlText += " UOMPrice=@UOMPrice,";
                sqlText += " MasterComments=@bomMasterMasterComments";


                sqlText += " where 1=1";
                sqlText += " and BOMId=@bomMasterBOMId";
                //sqlText += " and EffectDate=@bomMasterEffectDate";
                //sqlText += " AND VATName=@VATName";


                SqlCommand cmdMasterUpdate = new SqlCommand(sqlText, currConn);
                cmdMasterUpdate.Transaction = transaction;
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@ReferenceNo", bomMaster.ReferenceNo);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterFirstSupplyDate", OrdinaryVATDesktop.DateToDate(bomMaster.FirstSupplyDate));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVATRate", bomMaster.VATRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterUOM", bomMaster.UOM ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDRate", bomMaster.SDRate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTradingMarkup", bomMaster.TradingMarkup);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterComments", bomMaster.Comments ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterActiveStatus", bomMaster.ActiveStatus ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy ?? Convert.DBNull);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedOn));
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawTotal", bomMaster.RawTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPackingTotal", bomMaster.PackingTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateTotal", bomMaster.RebateTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterAdditionalTotal", bomMaster.AdditionalTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRebateAdditionTotal", bomMaster.RebateAdditionTotal);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPNBRPrice", bomMaster.PNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterPPacketPrice", bomMaster.PPacketPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterRawOHCost", bomMaster.RawOHCost);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRPrice", bomMaster.LastNBRPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastNBRWithSDAmount", bomMaster.LastNBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterTotalQuantity", bomMaster.TotalQuantity);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterSDAmount", bomMaster.SDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterVatAmount", bomMaster.VatAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterWholeSalePrice", bomMaster.WholeSalePrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterNBRWithSDAmount", bomMaster.NBRWithSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterMarkupValue", bomMaster.MarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastMarkupValue", bomMaster.LastMarkupValue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastSDAmount", bomMaster.LastSDAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterLastAmount", bomMaster.LastAmount);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@AutoIssue", bomMaster.AutoIssue);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo ?? Convert.DBNull);
                //cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@VATName", VATName);
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", "0");
                }
                else
                {
                    cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@CustomerID", bomMaster.CustomerID);

                }


                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@UOMc", bomMaster.FUOMc);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@UOMn", bomMaster.FUOMn);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@UOMPrice", bomMaster.FUOMPrice);
                cmdMasterUpdate.Parameters.AddWithValueAndNullHandle("@bomMasterMasterComments", bomMaster.MasterComments ?? Convert.DBNull);



                transResult = (int)cmdMasterUpdate.ExecuteNonQuery();
                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameUpdate,
                                                    MessageVM.PurchasemsgUpdateNotSuccessfully);
                }

                #endregion BOM Master Update


                #region Delete Existing Detail Data

                sqlText = "";
                sqlText += @" delete FROM BOMRaws WHERE BOMId=@bomMasterBOMId ";
                sqlText += @" delete FROM BOMCompanyOverhead WHERE BOMId=@bomMasterBOMId ";
                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@bomMasterBOMId", bomMaster.BOMId);

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion
                #endregion update BOM Table

                retResults = BOMInsert2(bomItems, bomOHs, bomMaster, transaction, currConn);

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.receiveMsgMethodNameInsert, MessageVM.receiveMsgSaveNotSuccessfully);
                }

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
                retResults[1] = MessageVM.bomMsgUpdateSuccessfully;

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
                transaction.Rollback();
                FileLogger.Log("BOMDAL", "BOMUpdate", ex.ToString() + "\n" + sqlText);

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

        public string[] BOMPostX(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null)
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
            int countId = 0;
            string sqlText = "";



            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                //if (bomItems == null && !bomItems.Any())
                //{
                //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, "Sorry,No Item found to insert.");
                //}
                currConn = _dbsqlConnection.GetConnection(connVM);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                CommonDAL commonDal = new CommonDAL();
                //commonDal.TableFieldAdd("BOMRaws", "PBOMId", "varchar(20)", currConn);

                transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);

                #endregion open connection and transaction

                #region Fiscal Year Check

                /*Checking existance of provided bank Id information*/

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK


                #endregion Fiscal Year Check

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Checking other BOM after this date

                sqlText = "";
                sqlText = "select count(bomid) from boms ";
                sqlText += " where  ";
                sqlText += " FinishItemNo=@bomMasterItemNo ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName  ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";

                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";

                }

                //sqlText += " and effectdate>'" + bomMaster.EffectDate + "'";
                sqlText += " and Post='Y'";
                //sqlText += " and VATName='" + bomMaster.VATName + "'";

                SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                cmdOtherBom.Transaction = transaction;
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int otherBom = (int)cmdOtherBom.ExecuteScalar();

                if (otherBom > 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Sorry,You cannot update this price declaration. Another declaration exist after this.");
                }

                #endregion Checking other BOM after this date

                #region Find Transaction Exist

                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs ";
                sqlText += " where  ";
                sqlText += " FinishItemNo=@bomMasterItemNo ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                sqlText += " and Post='Y'";
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                int IDExist = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 4.3 (Tender)")
                {
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                #region Update Post

                sqlText = "";
                sqlText += " update BOMs set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn , ";
                sqlText += " Post='Y'";

                sqlText += "  where FinishItemNo=@bomMasterItemNo ";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(NULLIF(CustomerId,''),0)='0'  ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                SqlCommand cmdMaster = new SqlCommand(sqlText, currConn);
                cmdMaster.Transaction = transaction;
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdMaster.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                transResult = (int)cmdMaster.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }
                sqlText = "";
                sqlText += " update BOMRaws set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn, ";
                sqlText += " Post='Y'";
                sqlText += "  where FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(NULLIF(CustomerId,''),0)='0'  ";

                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                //sqlText += "  where FinishItemNo='" + bomMaster.ItemNo";
                //sqlText += " AND EffectDate='" + bomMaster.EffectDate.Date ";
                //sqlText += " AND VATName='" + bomMaster.VATName ";
                SqlCommand cmdRaws = new SqlCommand(sqlText, currConn);
                cmdRaws.Transaction = transaction;
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdRaws.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);
                transResult = (int)cmdRaws.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }
                sqlText = "";
                sqlText += " update BOMCompanyOverhead set";
                sqlText += " LastModifiedBy= @bomMasterLastModifiedBy ,";
                sqlText += " LastModifiedOn= @bomMasterLastModifiedOn, ";
                sqlText += " Post='Y'";
                sqlText += "  where FinishItemNo=@bomMasterItemNo";
                sqlText += " AND EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(NULLIF(CustomerId,''),0)='0'  ";
                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";
                }
                //sqlText += "  where FinishItemNo='" + bomMaster.ItemNo + "'";
                //sqlText += " AND EffectDate='" + bomMaster.EffectDate.Date + "' ";
                //sqlText += " AND VATName='" + bomMaster.VATName + "' ";
                SqlCommand cmdOverheads = new SqlCommand(sqlText, currConn);
                cmdOverheads.Transaction = transaction;
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedBy", bomMaster.LastModifiedBy);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterLastModifiedOn", bomMaster.LastModifiedOn);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", bomMaster.EffectDate);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdOverheads.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);

                transResult = (int)cmdOverheads.ExecuteNonQuery();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgUpdateNBRPrice);
                }

                #endregion Update Post

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
                retResults[1] = "Data successfully post.";
                retResults[2] = "" + bomMaster.BOMId;
                //retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
                FileLogger.Log("BOMDAL", "BOMPostX", ex.ToString() + "\n" + sqlText);

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

        //
        public string[] BOMPost(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null, SqlTransaction vtransaction = null, SqlConnection vcurrConn = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";


            SqlConnection currConn = vcurrConn;
            SqlTransaction transaction = vtransaction;
            int transResult = 0;
            int countId = 0;
            string sqlText = "";



            #endregion Initializ

            #region Try

            try
            {
                #region open connection and transaction

                //if (bomItems == null && !bomItems.Any())
                //{
                //    throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert, "Sorry,No Item found to insert.");
                //}
                if (currConn == null)
                {
                    currConn = _dbsqlConnection.GetConnection(connVM);
                    if (currConn.State != ConnectionState.Open)
                    {
                        currConn.Open();
                    }

                    transaction = currConn.BeginTransaction(MessageVM.PurchasemsgMethodNameInsert);
                }

                #endregion open connection and transaction

                CommonDAL commonDal = new CommonDAL();

                #region Fiscal Year Check

                /*Checking existance of provided bank Id information*/

                #region Fiscal Year Check

                string transactionDate = bomMaster.EffectDate;
                string transactionYearCheck = Convert.ToDateTime(bomMaster.EffectDate).ToString("yyyy-MM-dd");
                if (Convert.ToDateTime(transactionYearCheck) > DateTime.MinValue ||
                    Convert.ToDateTime(transactionYearCheck) < DateTime.MaxValue)
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }

                    else if (dataTable.Rows.Count <= 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearisLock);
                    }
                    else
                    {
                        if (dataTable.Rows[0]["MLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
                        }
                        else if (dataTable.Rows[0]["YLock"].ToString() != "N")
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearisLock);
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
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }

                    else if (dtYearNotExist.Rows.Count < 0)
                    {
                        throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                        MessageVM.msgFiscalYearNotExist);
                    }
                    else
                    {
                        if (Convert.ToDateTime(transactionYearCheck) <
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MinDate"].ToString())
                            ||
                            Convert.ToDateTime(transactionYearCheck) >
                            Convert.ToDateTime(dtYearNotExist.Rows[0]["MaxDate"].ToString()))
                        {
                            throw new ArgumentNullException(MessageVM.PurchasemsgMethodNameInsert,
                                                            MessageVM.msgFiscalYearNotExist);
                        }
                    }

                    #endregion YearNotExist

                }


                #endregion Fiscal Year CHECK


                #endregion Fiscal Year Check

                #region CheckVATName

                if (string.IsNullOrEmpty(bomMaster.VATName))
                {
                    throw new ArgumentNullException("BOMPost", MessageVM.msgVatNameNotFound);

                }

                #endregion CheckVATName

                #region Checking other BOM after this date

                sqlText = "";
                sqlText = "select count(bomid) from boms ";
                sqlText += " where  ";
                sqlText += " FinishItemNo=@bomMasterItemNo ";
                sqlText += " and EffectDate=@bomMasterEffectDate ";
                sqlText += " AND VATName=@bomMasterVATName  ";
                sqlText += " AND ISNULL(ReferenceNo,'NA')=@ReferenceNo  ";
                if (bomMaster.CustomerID == "0" || string.IsNullOrEmpty(bomMaster.CustomerID))
                {
                    sqlText += " AND isnull(CustomerId,0)='0' ";

                }
                else
                {
                    sqlText += " AND isnull(CustomerId,0)=@bomMasterCustomerID ";

                }

                //sqlText += " and effectdate>'" + bomMaster.EffectDate + "'";
                sqlText += " and Post='Y'";
                //sqlText += " and VATName='" + bomMaster.VATName + "'";

                SqlCommand cmdOtherBom = new SqlCommand(sqlText, currConn);
                cmdOtherBom.Transaction = transaction;
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterItemNo", bomMaster.ItemNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterEffectDate", OrdinaryVATDesktop.DateToDate(bomMaster.EffectDate));
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterVATName", bomMaster.VATName);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@ReferenceNo", bomMaster.ReferenceNo);
                cmdOtherBom.Parameters.AddWithValueAndNullHandle("@bomMasterCustomerID", bomMaster.CustomerID);


                int otherBom = (int)cmdOtherBom.ExecuteScalar();

                if (otherBom > 0)
                {
                    throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                    "Sorry,You cannot Post this price declaration. Another declaration exist after this.");
                }

                #endregion Checking other BOM after this date

                #region Find Transaction Exist

                sqlText = "";
                sqlText += "select COUNT(FinishItemNo) from BOMs ";
                sqlText += " where  1=1 ";
                sqlText += " AND BOMId=@BOMId ";
                sqlText += " and Post='Y'";
                SqlCommand cmdFindBOMId = new SqlCommand(sqlText, currConn);
                cmdFindBOMId.Transaction = transaction;
                cmdFindBOMId.Parameters.AddWithValueAndNullHandle("@BOMId", bomMaster.BOMId);

                int IDExist = (int)cmdFindBOMId.ExecuteScalar();
                if (bomMaster.VATName != "VAT 4.3 (Tender)")
                {
                    if (IDExist > 0)
                    {
                        throw new ArgumentNullException(MessageVM.bomMsgMethodNameInsert,
                                                        "Price declaration for this item ('" + bomMaster.FinishItemName +
                                                        "') already exist in same date  ('" + bomMaster.EffectDate +
                                                        "') .");
                    }
                }

                #endregion Find Transaction Exist

                #region Post

                sqlText = "";
                sqlText += @" Update  BOMs                  set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE BOMId=@BOMId ";
                sqlText += @" Update  BOMRaws               set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE BOMId=@BOMId ";
                sqlText += @" Update  BOMCompanyOverhead    set  Post ='Y',LastModifiedBy =@MasterLastModifiedBy,LastModifiedOn =@MasterLastModifiedOn    WHERE BOMId=@BOMId ";

                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn);
                cmdDeleteDetail.Transaction = transaction;
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@BOMId", bomMaster.BOMId);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedOn", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedOn));
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@MasterLastModifiedBy", OrdinaryVATDesktop.DateToDate(bomMaster.LastModifiedBy));

                transResult = cmdDeleteDetail.ExecuteNonQuery();


                #endregion

                #region Commit

                if (transaction != null && vtransaction == null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data successfully post.";
                retResults[2] = "" + bomMaster.BOMId;
                //retResults[3] = "" + PostStatus;

                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall

            //catch (SqlException sqlex)
            //{
            //    transaction.Rollback();
            //    throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            //    //, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
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
                if (transaction != null && vtransaction == null)
                {
                    transaction.Rollback();
                }
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "BOMPost", ex.ToString() + "\n" + sqlText);

            }
            finally
            {
                if (currConn != null && vcurrConn == null)
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
        public string FindBOMID(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

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


                #region open connection and transaction

                //if (VcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo='" + itemNo + "'";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId from BOMs";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName ";
                //sqlText += " and effectdate<='" + effectDate.Date + "'";
                sqlText += " and effectdate<=@effectDate";
                sqlText += " and post='Y' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                var tt = "";
                if (cmdBomId.ExecuteScalar() == null)
                {
                    //throw new ArgumentNullException("FindBOMID",
                    //                                "No Price declaration found for this item");
                    BomId = string.Empty;
                }
                else
                {
                    BomId = cmdBomId.ExecuteScalar().ToString();
                }
                //BomId = tt;
                #endregion Last BOMId

                #region Commit
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

            }

            #endregion try

            #region Catch and Finall
            #region Catch
            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BOMDAL", "FindBOMID", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "FindBOMID", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

            }
            #endregion

            #region finally
            finally
            {
                ////if (currConn == null)
                ////{
                //if (VcurrConn.State == ConnectionState.Open)
                //{
                //    VcurrConn.Close();

                //}
                ////}

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }


            }
            #endregion

            #endregion

            #region Results

            return BomId;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string[] DeleteBOM(string itemNo, string VatName, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {

            #region Initializ

            string sqlText = "";
            string BomId = string.Empty;

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            int transResult = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion

            #region Try

            try
            {

                #region Validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data.");
                }
                else if (Convert.ToDateTime(effectDate) < DateTime.MinValue ||
                         Convert.ToDateTime(effectDate) > DateTime.MaxValue)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");

                }

                #endregion Validation

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

                #region open connection and transaction

                //if (VcurrConn == null)
                //{
                //    VcurrConn = _dbsqlConnection.GetConnection(connVM);
                //    if (VcurrConn.State != ConnectionState.Open)
                //    {
                //        VcurrConn.Open();
                //    }
                //}

                #endregion open connection and transaction

                #region ProductExist

                sqlText = "select count(ItemNo) from Products where ItemNo=@itemNo";
                SqlCommand cmdExist = new SqlCommand(sqlText, currConn);
                cmdExist.Transaction = transaction;
                cmdExist.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                int foundId = (int)cmdExist.ExecuteScalar();
                if (foundId <= 0)
                {
                    throw new ArgumentNullException("FindBOMID", "There is No data to find Price");
                }

                #endregion ProductExist

                #region Last BOMId

                sqlText = "  ";
                sqlText += " select top 1  CONVERT(varchar(10), isnull(BOMId,0))BOMId  from BOMs";
                sqlText += " where ";
                sqlText += " FinishItemNo=@itemNo ";
                sqlText += " and vatname=@VatName  ";
                sqlText += " and effectdate<=@effectDate";
                //sqlText += " and post='Y' ";
                sqlText += " and post='N' ";
                sqlText += " order by effectdate desc ";

                SqlCommand cmdBomId = new SqlCommand(sqlText, currConn);
                cmdBomId.Transaction = transaction;
                cmdBomId.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                cmdBomId.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                if (cmdBomId.ExecuteScalar() == null)
                {
                    BomId = string.Empty;

                    #region SuccessResult

                    retResults[0] = "Fail";
                    retResults[1] = "This transaction was posted.";

                    #endregion SuccessResult

                    return retResults;
                }
                else
                {
                    BomId = (string)cmdBomId.ExecuteScalar();
                }

                #endregion Last BOMId

                if (!string.IsNullOrEmpty(BomId))
                {
                    #region Remove row at BOMCompanyOverhead

                    sqlText = "";
                    sqlText += @" Delete from BOMCompanyOverhead    WHERE 1=1  AND BOMId=@BomId";
                    sqlText += @" Delete from BOMRaws    WHERE 1=1  AND BOMId=@BomId";
                    sqlText += @" Delete from BOMs    WHERE 1=1  AND BOMId=@BomId";

                    //////sqlText += " AND FinishItemNo=@itemNo ";
                    //////sqlText += " AND EffectDate=@effectDate";
                    //////sqlText += " AND VATName=@VatName  ";

                    SqlCommand cmdRemove = new SqlCommand(sqlText, currConn, transaction);
                    cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId);

                    //////cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    //////cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                    //////cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                    transResult = cmdRemove.ExecuteNonQuery();
                    if (transResult <= 0)
                    {
                        throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    }

                    #endregion Remove row at BOMCompanyOverhead


                    #region Comments

                    ////#region Remove row at BOMRaws

                    ////sqlText = "";
                    ////sqlText += " Delete from BOMRaws WHERE  ";
                    ////sqlText += " FinishItemNo=@itemNo ";
                    ////sqlText += " and EffectDate=@effectDate";
                    ////sqlText += " AND VATName=@VatName ";
                    ////sqlText += " AND BOMId=@BomId ";

                    ////cmdRemove = new SqlCommand(sqlText, currConn);
                    ////cmdRemove.Transaction = transaction;
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId);
                    ////transResult = (int)cmdRemove.ExecuteNonQuery();
                    ////if (transResult <= 0)
                    ////{
                    ////    throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    ////}

                    ////#endregion Remove row at BOMRaws

                    ////#region Remove row at BOMs

                    ////sqlText = "";
                    ////sqlText += " Delete from BOMs WHERE  ";
                    ////sqlText += " FinishItemNo=@itemNo ";
                    ////sqlText += " and EffectDate=@effectDate";
                    ////sqlText += " AND VATName=@VatName ";
                    ////sqlText += " AND BOMId=@BomId";

                    ////cmdRemove = new SqlCommand(sqlText, currConn);
                    ////cmdRemove.Transaction = transaction;
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@VatName", VatName);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    ////cmdRemove.Parameters.AddWithValueAndNullHandle("@BomId", BomId);
                    ////transResult = (int)cmdRemove.ExecuteNonQuery();
                    ////if (transResult <= 0)
                    ////{
                    ////    throw new ArgumentNullException("RemoveBOM", "There is No data to remove");
                    ////}

                    ////#endregion Remove row at BOMs

                    #endregion


                    #region Commit

                    #region Old

                    //if (transaction != null)
                    //{
                    //    transaction.Commit();

                    //    //////if (transResult > 0)
                    //    //////{
                    //    //////}

                    //}

                    #endregion

                    if (Vtransaction == null && transaction != null)
                    {
                        transaction.Commit();
                    }


                    #endregion Commit

                    #region SuccessResult

                    retResults[0] = "Success";
                    retResults[1] = "Successfully Deleted.";

                    #endregion SuccessResult
                }



            }

            #endregion try

            #region Catch and Finall

            #region Catch

            catch (SqlException sqlex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;

                FileLogger.Log("BOMDAL", "DeleteBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;

                FileLogger.Log("BOMDAL", "DeleteBOM", ex.ToString() + "\n" + sqlText);

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

                //if (VcurrConn.State == ConnectionState.Open)
                //{
                //    VcurrConn.Close();

                //}
            }

            #endregion

            #endregion

            #region Results

            return retResults;

            #endregion

        }

        //currConn to VcurrConn 25-Aug-2020
        public string[] FindCostingFrom(string itemNo, string effectDate, SqlConnection VcurrConn, SqlTransaction Vtransaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null)
        {
            #region Initialize

            string sqlText = "";
            string[] retResults = new string[2];
            retResults[0] = "0";
            retResults[1] = "No Data";
            int transResult = 0;
            string purchaseInvoiceNo = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            #endregion Initialize

            #region Try

            try
            {
                #region validation

                if (string.IsNullOrEmpty(itemNo))
                {
                    throw new ArgumentNullException("FindLastCosting", "There is No data to find ID.");
                }

                #endregion validation

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

                #region Find purchase ID

                sqlText = "";
                sqlText += @" 
select Top 1 PurchaseInvoiceNo from PurchaseInvoiceDetails
where ItemNo=@itemNo 
and ReceiveDate<=@effectDate

and TransactionType Not IN ('TollReceive-WIP')
                
order by ReceiveDate desc ";

                SqlCommand cmd = new SqlCommand(sqlText, currConn);
                cmd.Transaction = transaction;
                cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                cmd.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                if (cmd.ExecuteScalar() == null)
                {
                    purchaseInvoiceNo = string.Empty;

                    sqlText = "";
                    sqlText += " select Top 1 CAST(id as varchar) Id from Costing";
                    sqlText += " where ItemNo=@itemNo ";
                    sqlText += " and InputDate<=@effectDate";
                    sqlText += " order by InputDate desc ";

                    cmd = new SqlCommand(sqlText, currConn);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    cmd.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);
                    if (cmd.ExecuteScalar() != null)
                    {
                        retResults[0] = (string)cmd.ExecuteScalar();
                        retResults[1] = "FromCosting";
                    }

                }
                else
                {
                    purchaseInvoiceNo = (string)cmd.ExecuteScalar();

                    sqlText = "";

                    sqlText += @" 
Declare @InputDate datetime;
DECLARE @ID varchar(120);";

                    sqlText += "  select top 1 @InputDate=InputDate,@ID=id from Costing ";
                    sqlText += " where ItemNo=@itemNo ";
                    sqlText += " and InputDate<=@effectDate";
                    sqlText += " order by InputDate desc ";
                    sqlText += "\r\n";
                    sqlText += @" if @InputDate!=''
                                     begin
                                     select Top 1 case when p.ReceiveDate > @InputDate 
                                     then p.PurchaseInvoiceNo  else CAST(@ID as varchar) end AS ID ,
                                     case when p.ReceiveDate > @InputDate 
                                     then 'P'  else 'C' end AS FromPlace 
                                     from PurchaseInvoiceDetails p
                                     ";

                    sqlText += " where p.ItemNo=@itemNo ";
                    sqlText += " and p.ReceiveDate <=@effectDate";
                    sqlText += " order by p.ReceiveDate desc ";
                    sqlText += " end";

                    DataTable dataTable = new DataTable("CostDataT");
                    cmd = new SqlCommand(sqlText, currConn);
                    cmd.Transaction = transaction;
                    cmd.Parameters.AddWithValueAndNullHandle("@itemNo", itemNo);
                    cmd.Parameters.AddWithValueAndNullHandle("@effectDate", effectDate);

                    SqlDataAdapter reportDataAdapt = new SqlDataAdapter(cmd);
                    reportDataAdapt.Fill(dataTable);

                    if (dataTable.Rows.Count <= 0)
                    {
                        retResults[0] = purchaseInvoiceNo;
                        retResults[1] = "FromPurchase";
                    }
                    else
                    {
                        if (dataTable.Rows[0]["FromPlace"].ToString() == "P")
                        {
                            retResults[0] = dataTable.Rows[0]["ID"].ToString();
                            retResults[1] = "FromPurchase";
                        }
                        else if (dataTable.Rows[0]["FromPlace"].ToString() == "C")
                        {
                            retResults[0] = dataTable.Rows[0]["ID"].ToString();
                            retResults[1] = "FromCosting";
                        }
                        else
                        {
                            retResults[0] = "0";
                            retResults[1] = "No Data";
                        }
                    }




                    //if (cmd.ExecuteScalar() == null)
                    //{
                    //    //retResults[0] = "0";
                    //    //retResults[1] = "No Data";
                    //    retResults[0] = purchaseInvoiceNo;
                    //    retResults[1] = "FromPurchase";
                    //}
                    //else
                    //{
                    //    string id = (string)cmd.ExecuteScalar();
                    //    if (!string.IsNullOrEmpty(id))
                    //    {
                    //        retResults[0] = id;
                    //        retResults[1] = "FromPurchase";
                    //    }
                    //    else
                    //    {
                    //        retResults[0] = id;
                    //        retResults[1] = "FromCosting";
                    //    }
                    //}



                }

                #endregion Find purchase ID

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    if (transResult > 0)
                    {
                        transaction.Commit();
                    }

                }

                #endregion Commit

            }
            #endregion Try

            #region Catch and Finally

            #region Catch

            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "FindCostingFrom", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
                //////throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "FindCostingFrom", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////, "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
                //////throw ex;
            }
            #endregion

            #region finally

            finally
            {

                if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }

                //if (VcurrConn.State == ConnectionState.Open)
                //{
                //    VcurrConn.Close();

                //}

            }

            #endregion

            #endregion

            return retResults;

        }

        public List<BOMNBRVM> SelectBOMList(List<string> ids, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMNBRVM> VMs = new List<BOMNBRVM>();
            BOMNBRVM vm;
            #endregion
            try
            {
                #region sql statement
                #region SqlExecution

                currConn = VcurrConn;
                transaction = Vtransaction;


                DataTable dt = SelectBOM(ids, currConn, transaction, null, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new BOMNBRVM();

                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.BOMId = dr["BOMId"].ToString();
                        vm.ItemNo = dr["FinishItemNo"].ToString();
                        vm.EffectDate = dr["EffectDate"].ToString();
                        vm.VATName = dr["VATName"].ToString();
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.SDRate = Convert.ToDecimal(dr["SD"].ToString());
                        vm.TradingMarkup = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RawTotal = Convert.ToDecimal(dr["RawTotal"].ToString());
                        vm.PackingTotal = Convert.ToDecimal(dr["PackingTotal"].ToString());
                        vm.RebateTotal = Convert.ToDecimal(dr["RebateTotal"].ToString());
                        vm.AdditionalTotal = Convert.ToDecimal(dr["AdditionalTotal"].ToString());
                        vm.RebateAdditionTotal = Convert.ToDecimal(dr["RebateAdditionTotal"].ToString());
                        vm.PNBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.PPacketPrice = Convert.ToDecimal(dr["PacketPrice"].ToString());
                        vm.RawOHCost = Convert.ToDecimal(dr["RawOHCost"].ToString());
                        vm.LastNBRPrice = Convert.ToDecimal(dr["LastNBRPrice"].ToString());
                        vm.LastNBRWithSDAmount = Convert.ToDecimal(dr["LastNBRWithSDAmount"].ToString());
                        vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                        vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                        vm.VatAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                        vm.WholeSalePrice = Convert.ToDecimal(dr["WholeSalePrice"].ToString());
                        vm.NBRWithSDAmount = Convert.ToDecimal(dr["NBRWithSDAmount"].ToString());
                        vm.MarkupValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                        vm.LastMarkupValue = Convert.ToDecimal(dr["LastMarkUpValue"].ToString());
                        vm.LastSDAmount = Convert.ToDecimal(dr["LastSDAmount"].ToString());
                        vm.LastAmount = Convert.ToDecimal(dr["LastAmount"].ToString());
                        vm.Post = dr["Post"].ToString();
                        vm.UOM = dr["UOM"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.FinishItemName = dr["ProductName"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.FinishItemCode = dr["ProductCode"].ToString();
                        vm.FinishCategory = dr["CategoryID"].ToString();
                        vm.FirstSupplyDate = dr["FirstSupplyDate"].ToString();
                        vm.AutoIssue = dr["AutoIssue"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e) { }
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
                FileLogger.Log("BOMDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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

        public List<BOMNBRVM> SelectPreviousBOM(BOMNBRVM model, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<BOMNBRVM> VMs = new List<BOMNBRVM>();
            BOMNBRVM vm;
            #endregion
            try
            {
                #region sql statement
                #region SqlExecution

                currConn = VcurrConn;
                transaction = Vtransaction;


                DataTable dt = SelectPreviousBOMData(model, currConn, transaction, null, false, connVM);

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        vm = new BOMNBRVM();

                        vm.ReferenceNo = dr["ReferenceNo"].ToString();
                        vm.BOMId = dr["BOMId"].ToString();
                        vm.ItemNo = dr["FinishItemNo"].ToString();
                        vm.EffectDate = dr["EffectDate"].ToString();
                        vm.VATName = dr["VATName"].ToString();
                        vm.VATRate = Convert.ToDecimal(dr["VATRate"].ToString());
                        vm.SDRate = Convert.ToDecimal(dr["SD"].ToString());
                        vm.TradingMarkup = Convert.ToDecimal(dr["TradingMarkUp"].ToString());
                        vm.Comments = dr["Comments"].ToString();
                        vm.ActiveStatus = dr["ActiveStatus"].ToString();
                        vm.CreatedBy = dr["CreatedBy"].ToString();
                        vm.CreatedOn = dr["CreatedOn"].ToString();
                        vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                        vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                        vm.RawTotal = Convert.ToDecimal(dr["RawTotal"].ToString());
                        vm.PackingTotal = Convert.ToDecimal(dr["PackingTotal"].ToString());
                        vm.RebateTotal = Convert.ToDecimal(dr["RebateTotal"].ToString());
                        vm.AdditionalTotal = Convert.ToDecimal(dr["AdditionalTotal"].ToString());
                        vm.RebateAdditionTotal = Convert.ToDecimal(dr["RebateAdditionTotal"].ToString());
                        vm.PNBRPrice = Convert.ToDecimal(dr["NBRPrice"].ToString());
                        vm.PPacketPrice = Convert.ToDecimal(dr["PacketPrice"].ToString());
                        vm.RawOHCost = Convert.ToDecimal(dr["RawOHCost"].ToString());
                        vm.LastNBRPrice = Convert.ToDecimal(dr["LastNBRPrice"].ToString());
                        vm.LastNBRWithSDAmount = Convert.ToDecimal(dr["LastNBRWithSDAmount"].ToString());
                        vm.TotalQuantity = Convert.ToDecimal(dr["TotalQuantity"].ToString());
                        vm.SDAmount = Convert.ToDecimal(dr["SDAmount"].ToString());
                        vm.VatAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                        vm.WholeSalePrice = Convert.ToDecimal(dr["WholeSalePrice"].ToString());
                        vm.NBRWithSDAmount = Convert.ToDecimal(dr["NBRWithSDAmount"].ToString());
                        vm.MarkupValue = Convert.ToDecimal(dr["MarkUpValue"].ToString());
                        vm.LastMarkupValue = Convert.ToDecimal(dr["LastMarkUpValue"].ToString());
                        vm.LastSDAmount = Convert.ToDecimal(dr["LastSDAmount"].ToString());
                        vm.LastAmount = Convert.ToDecimal(dr["LastAmount"].ToString());
                        vm.Post = dr["Post"].ToString();
                        vm.UOM = dr["UOM"].ToString();
                        vm.CustomerID = dr["CustomerID"].ToString();
                        vm.FinishItemName = dr["ProductName"].ToString();
                        vm.CustomerName = dr["CustomerName"].ToString();
                        vm.FinishItemCode = dr["ProductCode"].ToString();
                        vm.FinishCategory = dr["CategoryID"].ToString();
                        vm.FirstSupplyDate = dr["FirstSupplyDate"].ToString();
                        vm.AutoIssue = dr["AutoIssue"].ToString();

                        VMs.Add(vm);
                    }
                    catch (Exception e) { }
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
                FileLogger.Log("BOMDAL", "SelectAllList", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
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
        public DataTable SelectBOM(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
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
            #endregion
            int len = ids.Count;

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
                #region SQL Text

                sqlText += @" SELECT

 pc.IsRaw ProductType
,bm.BOMId
,bm.FinishItemNo
,p.ProductName
,p.ProductCode
,c.CustomerName
,ISNULL(bm.ReferenceNo,'NA') ReferenceNo
,isnull(bm.FirstSupplyDate,'1900/01/01') FirstSupplyDate
,bm.EffectDate
,bm.VATName
,isnull(bm.VATRate,0)VATRate
,isnull(bm.SD,0)SD
,isnull(bm.TradingMarkUp,0)TradingMarkUp
,bm.Comments
,bm.ActiveStatus
,bm.CreatedBy
,bm.CreatedOn
,bm.LastModifiedBy
,bm.LastModifiedOn
,isnull(bm.RawTotal,0)RawTotal
,isnull(bm.PackingTotal,0)PackingTotal
,isnull(bm.RebateTotal,0)RebateTotal
,isnull(bm.AdditionalTotal,0)AdditionalTotal
,isnull(bm.RebateAdditionTotal,0)RebateAdditionTotal
,isnull(bm.NBRPrice,0)NBRPrice
,isnull(bm.PacketPrice,0)PacketPrice
,bm.RawOHCost
,isnull(bm.LastNBRPrice,0)LastNBRPrice
,isnull(bm.LastNBRWithSDAmount,0)LastNBRWithSDAmount
,isnull(bm.TotalQuantity,0)TotalQuantity
,isnull(bm.SDAmount,0)SDAmount
,isnull(bm.VATAmount,0)VATAmount
,isnull(bm.WholeSalePrice,0)WholeSalePrice
,isnull(bm.NBRWithSDAmount,0)NBRWithSDAmount
,isnull(bm.MarkUpValue,0)MarkUpValue
,isnull(bm.LastMarkUpValue,0)LastMarkUpValue
,isnull(bm.LastSDAmount,0)LastSDAmount
,isnull(bm.LastAmount,0)LastAmount
,bm.Post
,bm.UOM
,bm.CustomerID
,p.CategoryID
,p.HSCodeNo
, isnull(bm.UOMn,bm.UOM)UOMn
,isnull(bm.BranchId, 0) BranchId
,isnull(bm.AutoIssue, 'Y') AutoIssue

FROM BOMs  bm left outer join Products p 
on bm.FinishItemNo=p.ItemNo left outer join Customers c
on bm.CustomerID=c.CustomerID
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE  1=1 
";
                #endregion



                if (ids.Count > 0)
                {
                    sqlTextParameter += @" and bm.BOMId in ( ";


                    for (int i = 0; i < len; i++)
                    {
                        sqlTextParameter += "'" + ids[i] + "',";
                    }

                    sqlTextParameter = sqlTextParameter.TrimEnd(',');

                    sqlTextParameter += ")";
                }

                sqlTextOrderBy += " order by bm.EffectDate desc, pc.IsRaw, p.ProductName ";

                #endregion SqlText
                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;



                da.Fill(ds);

                #endregion SqlExecution

                ////if (Vtransaction == null && transaction != null)
                ////{
                ////    transaction.Commit();
                ////}
                dt = ds.Tables[0].Copy();

                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SelectBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                ////if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                ////{
                ////    currConn.Close();
                ////}
            }
            #endregion
            return dt;
        }


        public DataTable SelectPreviousBOMData(BOMNBRVM model, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string sqlTextParameter = "";
            string sqlTextOrderBy = "";
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
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
                #region SQL Text

                sqlText += @" SELECT top 1

 pc.IsRaw ProductType
,bm.BOMId
,bm.FinishItemNo
,p.ProductName
,p.ProductCode
,c.CustomerName
,ISNULL(bm.ReferenceNo,'NA') ReferenceNo
,isnull(bm.FirstSupplyDate,'1900/01/01') FirstSupplyDate
,bm.EffectDate
,bm.VATName
,isnull(bm.VATRate,0)VATRate
,isnull(bm.SD,0)SD
,isnull(bm.TradingMarkUp,0)TradingMarkUp
,bm.Comments
,bm.ActiveStatus
,bm.CreatedBy
,bm.CreatedOn
,bm.LastModifiedBy
,bm.LastModifiedOn
,isnull(bm.RawTotal,0)RawTotal
,isnull(bm.PackingTotal,0)PackingTotal
,isnull(bm.RebateTotal,0)RebateTotal
,isnull(bm.AdditionalTotal,0)AdditionalTotal
,isnull(bm.RebateAdditionTotal,0)RebateAdditionTotal
,isnull(bm.NBRPrice,0)NBRPrice
,isnull(bm.PacketPrice,0)PacketPrice
,bm.RawOHCost
,isnull(bm.LastNBRPrice,0)LastNBRPrice
,isnull(bm.LastNBRWithSDAmount,0)LastNBRWithSDAmount
,isnull(bm.TotalQuantity,0)TotalQuantity
,isnull(bm.SDAmount,0)SDAmount
,isnull(bm.VATAmount,0)VATAmount
,isnull(bm.WholeSalePrice,0)WholeSalePrice
,isnull(bm.NBRWithSDAmount,0)NBRWithSDAmount
,isnull(bm.MarkUpValue,0)MarkUpValue
,isnull(bm.LastMarkUpValue,0)LastMarkUpValue
,isnull(bm.LastSDAmount,0)LastSDAmount
,isnull(bm.LastAmount,0)LastAmount
,bm.Post
,bm.UOM
,bm.CustomerID
,p.CategoryID
,p.HSCodeNo
, isnull(bm.UOMn,bm.UOM)UOMn
,isnull(bm.BranchId, 0) BranchId
,isnull(bm.AutoIssue, 'Y') AutoIssue

FROM BOMs  bm left outer join Products p 
on bm.FinishItemNo=p.ItemNo left outer join Customers c
on bm.CustomerID=c.CustomerID
left outer join ProductCategories pc on p.CategoryID=pc.CategoryID
WHERE  1=1 
and  bm.BOMId !=@BOMId
and bm.post ='Y'
and FinishItemNo =@FinishItemNo

and bm.EffectDate<@EffectDate
";
                #endregion




                sqlTextOrderBy += " order by bm.EffectDate desc";

                #endregion SqlText
                #region SqlExecution

                sqlText = sqlText + " " + sqlTextParameter + " " + sqlTextOrderBy;

                  SqlCommand cmd = new SqlCommand(sqlText, currConn);
                             cmd.Transaction = transaction;
                             cmd.Parameters.AddWithValueAndNullHandle("@BOMId", model.BOMId);
                             cmd.Parameters.AddWithValueAndNullHandle("@FinishItemNo", model.FinishItemCode);
                             cmd.Parameters.AddWithValueAndNullHandle("@EffectDate", model.EffectDate);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);

                //SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                //da.SelectCommand.Transaction = transaction;



                da.Fill(ds);

                #endregion SqlExecution

                ////if (Vtransaction == null && transaction != null)
                ////{
                ////    transaction.Commit();
                ////}
                dt = ds.Tables[0].Copy();

                #endregion
            }
            #endregion

            #region catch
            catch (SqlException sqlex)
            {
                FileLogger.Log("BOMDAL", "SelectBOM", sqlex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", sqlex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                FileLogger.Log("BOMDAL", "SelectBOM", ex.ToString() + "\n" + sqlText);

                throw new ArgumentNullException("", ex.Message.ToString());

                ////throw new ArgumentNullException("", "SQL:" + sqlText + FieldDelimeter + ex.Message.ToString());
            }
            #endregion
            #region finally
            finally
            {
                ////if (VcurrConn == null && currConn != null && currConn.State == ConnectionState.Open)
                ////{
                ////    currConn.Close();
                ////}
            }
            #endregion
            return dt;
        }
        #endregion


        public string[] ImportFile(BOMNBRVM VM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
            string Transaction_Type = null;
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            int transResult = 0;

            #endregion

            #region try
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();

                if (string.IsNullOrWhiteSpace(VM.BOMId))
                {
                    throw new Exception("Please select the product first!");
                }

                #region Attachment

                if (VM.File != null && VM.File.ContentLength > 0)
                {
                    Directory.CreateDirectory(VM.ServerPath);

                    // Get the original file name
                    string originalFileName = Path.GetFileName(VM.File.FileName);

                    // Modify the file name as needed (e.g., add a timestamp or a unique identifier)
                    string newFileName = VM.Operation+"_VAT_4_3" + Convert.ToDateTime(VM.CreatedOn).ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);

                    // Get the physical path to the "Questions" directory within your project
                    string questionsDirectoryPath = VM.ServerPath;

                    // Combine the directory path and the modified file name to get the full path to save the file
                    string filePath = Path.Combine(questionsDirectoryPath, newFileName);

                    if (File.Exists(filePath))
                    {
                        // Delete the existing file
                        File.Delete(filePath);
                    }

                    // Save the file with the modified name to the "Questions" directory
                    VM.File.SaveAs(filePath);
                    if (VM.Operation.ToLower() == "submitted")
                    {
                        VM.SubmittedFilePath = newFileName;
                        VM.SubmittedFileName = VM.Operation + "_VAT_4_3" + "_BOMId_" + VM.BOMId;
                    }
                    else
                    {
                        VM.ApprovedFilePath = newFileName;
                        VM.ApprovedFileName = VM.Operation + "_VAT_4_3" + "_BOMId_" + VM.BOMId;
                    }
                    

                }
                else
                {
                    throw new Exception("Attachment not found");
                }

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


                #region Save new data

                sqlText = "";
                if (VM.Operation.ToLower() == "submitted")
                {
                    sqlText = "update BOMs set";
                    sqlText += " SubmittedFileName=@SubmittedFileName";
                    sqlText += " ,SubmittedFilePath=@SubmittedFilePath";
                    sqlText += " where BOMId=@BOMId";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    cmd.Parameters.AddWithValue("@SubmittedFileName", VM.SubmittedFileName);
                    cmd.Parameters.AddWithValue("@SubmittedFilePath", VM.SubmittedFilePath);
                    cmd.Parameters.AddWithValue("@BOMId", VM.BOMId);
                    transResult = cmd.ExecuteNonQuery();

                }
                else
                {
                    sqlText = "update BOMs set";
                    sqlText += " ApprovedFileName=@ApprovedFileName";
                    sqlText += " ,ApprovedFilePath=@ApprovedFilePath";
                    sqlText += " where BOMId=@BOMId";
                    SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                    cmd.Parameters.AddWithValue("@ApprovedFileName", VM.ApprovedFileName);
                    cmd.Parameters.AddWithValue("@ApprovedFilePath", VM.ApprovedFilePath);
                    cmd.Parameters.AddWithValue("@BOMId", VM.BOMId);
                    transResult = cmd.ExecuteNonQuery();
                }

                #endregion

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = "Data Save Successfully.";

                #endregion SuccessResult

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

            }
            #endregion try
            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail"; //catch ex
                retResults[1] = ex.Message.ToString(); //catch ex
                FileLogger.Log("AuditDAL", "ImportFile", ex.ToString() + "\n" + sqlText);
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

            #region Results
            return retResults;
            #endregion
        }

    }
}
