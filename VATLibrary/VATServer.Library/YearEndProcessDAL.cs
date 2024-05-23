using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class YearEndProcessDAL
    {

        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();


        public DataTable SelectAllData(SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";

            DataTable dataTable = new DataTable("SelectAllData");

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

                #region Sql Command

                sqlText = @"  

select a.SerialNo,a.BranchId,b.BranchCode,b.BranchName,a.ItemNo,p.ProductCode,p.ProductName,a.StartDateTime
,a.StartingQuantity,a.StartingAmount,a.SD,a.VATRate,a.Quantity,a.UnitCost,a.TransID,a.TransType,a.BENumber
,a.InvoiceDateTime,a.AvgRate,a.RunningTotal,a.RunningValue,a.RunningOpeningValue,a.RunningOpeningQuantity
,a.ReportType from (

select SerialNo,BranchId,ItemNo,StartDateTime,StartingQuantity,StartingAmount,SD,isnull(VATRate,0)VATRate,Quantity,UnitCost
,TransID,TransType,BENumber,InvoiceDateTime,AvgRate,RunningTotal,RunningValue,RunningOpeningValue
,RunningOpeningQuantity, 'VAT6_1' ReportType
from VAT6_1_Permanent_Branch
union
select SerialNo,BranchId,ItemNo,StartDateTime,StartingQuantity,StartingAmount,SD,isnull(VATRate,0)VATRate,Quantity,UnitCost
,TransID,TransType,'-' BENumber,StartDateTime InvoiceDateTime,0 AvgRate,RunningTotal,0 RunningValue,0 RunningOpeningValue
,0 RunningOpeningQuantity, 'VAT6_2' ReportType
from VAT6_2_Permanent_Branch
union
select SerialNo,BranchId,ItemNo,StartDateTime,StartingQuantity,StartingAmount,SD,isnull(VATRate,0)VATRate,Quantity,UnitCost
,TransID,TransType,BENumber,InvoiceDateTime,0 AvgRate,RunningTotal,0 RunningValue,0 RunningOpeningValue
,0 RunningOpeningQuantity, 'VAT6_2_1' ReportType
from VAT6_2_1_Permanent_Branch
) as a
left outer join Products p on p.ItemNo=a.ItemNo
left outer join BranchProfiles b on b.BranchID=a.BranchId

";

                var cmd = new SqlCommand(sqlText, currConn, transaction);

                var adapter = new SqlDataAdapter(cmd);

                adapter.Fill(dataTable);

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #region catch
            catch (Exception ex)
            {
                if (transaction != null && Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine + sqlText);

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

            return dataTable;
        }




    }
}
