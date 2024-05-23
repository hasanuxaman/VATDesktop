using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class AuditDAL
    {

        #region Global Variables
        private const string FieldDelimeter = DBConstant.FieldDelimeter;
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        #endregion

        public string[] ImportFile(AuditVM VM, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                if (string.IsNullOrWhiteSpace(VM.FiscalYear))
                {
                    throw new Exception("Please select a Fiscal Year");
                }

                #region Attachment

                if (VM.Attachment != null && VM.Attachment.ContentLength > 0)
                {
                    Directory.CreateDirectory(VM.ServerPath);

                    // Get the original file name
                    string originalFileName = Path.GetFileName(VM.Attachment.FileName);

                    // Modify the file name as needed (e.g., add a timestamp or a unique identifier)
                    string newFileName = "Audit"+Convert.ToDateTime(VM.CreatedOn).ToString("yyyyMMddHHmmss") + Path.GetExtension(originalFileName);

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
                    VM.Attachment.SaveAs(filePath);

                    VM.FilePath = newFileName;
                    VM.FileName = "Audit-" + VM.FiscalYear;

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

                #region Delete exits data

                sqlText = " delete Audits where FiscalYear=@FiscalYear";
                SqlCommand cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYear", VM.FiscalYear);
                cmd.ExecuteNonQuery();

                #endregion

                #region Save new data

                sqlText = "";
                sqlText += @"
INSERT INTO Audits
(
 FiscalYear
,FileName
,FilePath
,CreatedBy
,CreatedOn

)
     VALUES
           (
 @FiscalYear
,@FileName
,@FilePath
,@CreatedBy
,@CreatedOn
)";

                cmd = new SqlCommand(sqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@FiscalYear", VM.FiscalYear);
                cmd.Parameters.AddWithValue("@FileName", VM.FileName);
                cmd.Parameters.AddWithValue("@FilePath", VM.FilePath);
                cmd.Parameters.AddWithValue("@CreatedBy", VM.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedOn", VM.CreatedOn);

                transResult = cmd.ExecuteNonQuery();

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

        public List<AuditVM> SelectAllList(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            List<AuditVM> VMs = new List<AuditVM>();
            AuditVM vm;
            #endregion

            #region try

            try
            {

                #region sql statement

                #region SqlExecution

                DataTable dt = SelectAll(Id, conditionFields, conditionValues, currConn, transaction);

                foreach (DataRow dr in dt.Rows)
                {
                    vm = new AuditVM();
                    vm.Id = Convert.ToInt32(dr["Id"].ToString());
                    vm.FiscalYear = dr["FiscalYear"].ToString();
                    vm.FileName = dr["FileName"].ToString();
                    vm.FilePath = dr["FilePath"].ToString();

                    VMs.Add(vm);
                }

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }
                #endregion

            }
            #endregion

            #region catch
            
            catch (Exception ex)
            {
                FileLogger.Log("AuditDAL", "SelectAllList", ex.ToString() + "\n" + sqlText);

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

        public DataTable SelectAll(string Id = "0", string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
SELECT 
Id
,FiscalYear
,FileName
,FilePath
,CreatedBy
,CreatedOn
,LastModifiedBy
,LastModifiedOn
FROM Audits 
WHERE  1=1 
";


                if (Id != null && Id != "0")
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

                if (Id != null && Id != "0")
                {
                    da.SelectCommand.Parameters.AddWithValue("@Id", Id.ToString());
                }
                SqlDataReader dr;
                da.Fill(dt);

                #endregion SqlExecution

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion
            }

            #region catch
            
            catch (Exception ex)
            {
                FileLogger.Log("AuditDAL", "SelectAll", ex.ToString() + "\n" + sqlText);

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
        


    }
}
