using SymphonySofttech.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VATServer.Interface;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class CompanyInformationDAL : ICompanyInformation
    {
        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        private const string LineDelimeter = DBConstant.LineDelimeter;
        private static string PassPhrase = DBConstant.PassPhrase;
        private static string EnKey = DBConstant.EnKey;
        #endregion
        public List<SymphonyVATSysCompanyInformationVM> DropDown(SysDBInfoVMTemp connVM = null, string CompanyList = "")
        {
            #region Variables
            SqlConnection currConn = null;
            string sqlText = "";
            List<SymphonyVATSysCompanyInformationVM> VMs = new List<SymphonyVATSysCompanyInformationVM>();
            SymphonyVATSysCompanyInformationVM vm;
            #endregion
            try
            {
                #region open connection and transaction
                //////currConn = _dbsqlConnection.GetConnection(connVM);
                currConn = _dbsqlConnection.GetConnectionSys(connVM);


                if (currConn.State != ConnectionState.Open)
                {
                    currConn.Open();
                }
                #endregion open connection and transaction
                #region sql statement
                sqlText = @"
                SELECT
                CompanyID Id
                ,CompanyName Name
                FROM CompanyInformations  
                WHERE  1=1 AND ActiveStatus = 'mXSJfsAdbf0='
                
";
                if (CompanyList.ToLower() != "all")
                {
                    List<string> result = CompanyList.Split(',').ToList();

                    sqlText += " and DatabaseName in (";

                    foreach (string Company in result)
                    {
                        sqlText +=  "'"+Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, Company) +"',";
                    }

                    sqlText = sqlText.TrimEnd(',') + ")";

                }

                sqlText += @" order by Serial";

                SqlCommand objComm = new SqlCommand(sqlText, currConn);

                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymphonyVATSysCompanyInformationVM();
                    vm.Id = dr["Id"].ToString();

                    vm.Name = Converter.DESDecrypt(PassPhrase, EnKey, dr["Name"].ToString());
                    VMs.Add(vm);
                }
                dr.Close();

                if (VMs != null && VMs.Count > 0)
                {
                    VMs = VMs.OrderBy(c => c.Name).ToList();
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

        public List<SymphonyVATSysCompanyInformationVM> SelectAll(string CompanyID = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<SymphonyVATSysCompanyInformationVM> VMs = new List<SymphonyVATSysCompanyInformationVM>();
            SymphonyVATSysCompanyInformationVM vm;
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
                    currConn = _dbsqlConnection.GetConnectionSys(connVM);
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
 CompanySl
,CompanyID
,CompanyName
,DatabaseName
,ActiveStatus
,Serial

FROM CompanyInformations  
WHERE  1=1 

";
                if (CompanyID != null)
                {
                    sqlText += @" and CompanyID=@CompanyID";
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

                sqlText += @" order by Serial";

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
                        objComm.Parameters.AddWithValue("@" + cField, conditionValues[j]);
                    }
                }

                if (CompanyID != null)
                {
                    objComm.Parameters.AddWithValue("@CompanyID", CompanyID);
                }
                SqlDataReader dr;
                dr = objComm.ExecuteReader();
                while (dr.Read())
                {
                    vm = new SymphonyVATSysCompanyInformationVM();

                    vm.CompanySl = Convert.ToInt32(dr["CompanySl"]);
                    vm.CompanyID = dr["CompanyID"].ToString();
                    vm.CompanyName = dr["CompanyName"].ToString();
                    vm.DatabaseName = Converter.DESDecrypt(PassPhrase, EnKey, dr["DatabaseName"].ToString());

                    vm.ActiveStatus = dr["ActiveStatus"].ToString();
                    vm.Serial = Convert.ToInt32(dr["Serial"]);


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


    }
}
