using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Library.Integration
{
    public class IntegrationDataViewDAL
    {
        DataTable dtTableResult = new DataTable();


        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string FieldDelimeter = DBConstant.FieldDelimeter;

        public DataTable GetIntregationPreviewList(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            DataTable dt = new DataTable();

            try
            {
                ImportDAL importDal = new ImportDAL();
                dt = GetPurchaseACIDbData(param, connVM);
            }
            catch (Exception)
            {

                throw;
            }

            return dt;

        }

        public DataTable GetPurchaseACIDbData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
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
            CommonDAL commonDal = new CommonDAL();
            #endregion

            #region try

            try
            {
                #region open connection and transaction

                currConn = _dbsqlConnection.GetDepoConnection(param.dtConnectionInfo);

                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }

                transaction = currConn.BeginTransaction();

                #endregion open connection and transaction

                #region sqlText

                sqlText = @"

      SELECT  
      *
      FROM [PurchaseInvoices] where 1=1 

 
 "; //07/0007401

                VendorDAL vendorDal = new VendorDAL();

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    sqlText += " and SL BETWEEN 1 AND @RefNo";
                }
                //if (!string.IsNullOrEmpty(param.Processed) && param.Processed != "ALL")
                //{
                //    sqlText += " and IsProcessed = '" + param.Processed + "'";
                //}

                //if (param.TransactionType.ToLower() == "other")
                //{
                //    sqlText += " and TransactionType = 'Local' ";
                //}
                //else if (param.TransactionType == "Import")
                //{
                //    sqlText += " and TransactionType = 'Import' ";
                //}
                //else if (param.TransactionType == "InputService")
                //{
                //    sqlText += " and TransactionType = 'InputService' ";
                //}

                //if (!string.IsNullOrEmpty(param.FromDate))
                //{
                //    sqlText += " and Format(cast(Invoice_Date as datetime),'yyyy-MM-dd') >= @fromDate";
                //}

                //if (!string.IsNullOrEmpty(param.ToDate))
                //{
                //    sqlText += " and Format(cast(Invoice_Date as datetime),'yyyy-MM-dd') <= @toDate";
                //}

                //sqlText += "  and CompanyCode = @CompanyCode";

                #endregion

                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);

                #region Peram

                if (!string.IsNullOrEmpty(param.RefNo))
                {
                    cmd.Parameters.AddWithValue("@RefNo", param.RefNo);
                }

                //if (!string.IsNullOrEmpty(param.FromDate))
                //{
                //    cmd.Parameters.AddWithValue("@fromDate", Convert.ToDateTime(param.FromDate).ToString("yyyy-MM-dd"));
                //}

                //if (!string.IsNullOrEmpty(param.ToDate))
                //{
                //    cmd.Parameters.AddWithValue("@toDate", Convert.ToDateTime(param.ToDate).ToString("yyyy-MM-dd"));
                //}

                //string code = commonDal.settingValue("CompanyCode", "Code", connVM);

                //cmd.Parameters.AddWithValue("@CompanyCode", code);

                #endregion

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                adapter.Fill(table);

                #region Commit

                if (transaction != null)
                {
                    transaction.Commit();
                }

                #endregion Commit

                return table;

            }
            #endregion

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults[0] = "Fail";//Success or Fail
                retResults[1] = ex.Message.ToString(); //catch ex
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("ImportDAL", "GetPurchaseACIDbData", ex.ToString() + "\n" + sqlText);

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


    }
}
