using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class ChartDAL
    {

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        public DataTable DayWiseSale(string InvoiceDateFrom, string InvoiceDateTo)
        {
            #region Variables

            SqlConnection currConn = null;
            string sqlText = "";
            DataTable dataTable = new DataTable("DayWiseSale");

            #endregion

            #region Try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetConnection(null);
                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                #endregion open connection and transaction

                #region SQL Statement

                sqlText =
                    @"
                        select distinct FORMAT(InvoiceDateTime,'yyyy-MMM-dd')[Date],format( sum(SubTotal),'0.000')SubTotal, format(sum(VATAmount),'0.000')VATAmount from SalesInvoiceDetails
where InvoiceDateTime >='01/01/2021' and InvoiceDateTime <='01/31/2021'
group by  FORMAT(InvoiceDateTime,'yyyy-MMM-dd')
order by  FORMAT(InvoiceDateTime,'yyyy-MMM-dd')";



                #endregion

                #region SQL Command

                SqlCommand objCommSaleReport = new SqlCommand();
                objCommSaleReport.Connection = currConn;

                objCommSaleReport.CommandText = sqlText;
                objCommSaleReport.CommandType = CommandType.Text;

                #endregion

                #region Parameters

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommSaleReport);
                dataAdapter.Fill(dataTable);

            }
            #endregion

            #region Catch & Finally

            catch (SqlException sqlex)
            {
                FileLogger.Log("ReportDSDAL", "MonthlySales", sqlex.ToString() + "\n" + sqlText);

                throw sqlex;
            }
            catch (Exception ex)
            {
                FileLogger.Log("ReportDSDAL", "MonthlySales", ex.ToString() + "\n" + sqlText);

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

            return dataTable;
        }


        public DataTable GetDateWiseProduct(string fromDate, string toDate, SysDBInfoVMTemp connVM = null)
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

                sqlText = "";
                sqlText += @"

select distinct z.ZoneName+'-'+format(InvoiceDateTime,'dd-MMM-yy') InvoiceDateTime
,p.ProductName
, sum(d.UOMQty)Quantity 
,sum(LineTotal)Total from SalesInvoiceMPLDetails d
left outer join BranchProfiles b on d.BranchId=b.BranchID
left outer join MPLZoneBranchMapping zm on d.BranchId=zm.BranchID
left outer join MPLZoneProfiles z on zm.ZoneId=z.ZoneID
left outer join Products p on d.ItemNo=p.ItemNo
where 1=1
and InvoiceDateTime >= @fromDate
and InvoiceDateTime< DATEADD(d,1,@toDate)
group by 
z.ZoneName
,format(InvoiceDateTime,'dd-MMM-yy')
,p.ProductName

";

                SqlCommand objCommProduct = new SqlCommand();
                objCommProduct.Connection = currConn;
                objCommProduct.CommandText = sqlText;
                objCommProduct.CommandType = CommandType.Text;

                objCommProduct.Parameters.AddWithValue("@fromDate", fromDate);
                objCommProduct.Parameters.AddWithValue("@toDate", toDate);


                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommProduct);
                dataAdapter.Fill(dataTable);

                #endregion
            }
            #region catch
            catch (Exception ex)
            {
                FileLogger.Log("ChartDAL", "GetDateWiseProduct", ex.ToString() + "\n" + sqlText);

                throw ex;
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
