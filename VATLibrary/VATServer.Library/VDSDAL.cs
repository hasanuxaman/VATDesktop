using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SymphonySofttech.Utilities;
using VATViewModel.DTOs;
using VATServer.Ordinary;
using VATServer.Interface;

namespace VATServer.Library
{
    public class VDSDAL : IVDS
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        public List<VDSMasterVM> SelectVDSDetail(string VDSId, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null, string TransactionType="")
        {
            #region Variables
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            string sqlText = "";
            string alice = "";
            List<VDSMasterVM> VMs = new List<VDSMasterVM>();
            VDSMasterVM vm;
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

                #region Comments 03Aug2021

                ////                sqlText = @"
                ////SELECT
                //// vd.VDSId
                ////,vd.VendorId
                ////,vd.BillNo
                ////,ISNULL(vd.BillAmount,0) BillAmount
                ////,vd.BillDate
                ////,ISNULL(vd.BillDeductAmount,0) BillDeductAmount
                ////,ISNULL(vd.DepositNumber,0) DepositNumber
                ////,vd.DepositDate
                ////,vd.Remarks
                ////,vd.IssueDate
                ////,vd.PurchaseNumber
                ////,vd.CreatedBy
                ////,vd.CreatedOn
                ////,vd.LastModifiedBy
                ////,vd.LastModifiedOn
                ////,ISNULL(vd.VDSPercent,0) VDSPercent
                ////,vd.IsPurchase
                ////,vd.IsPercent
                ////,vd.ReverseVDSId
                ////,vn.VendorName
                ////
                ////FROM VDS  vd left outer join Vendors vn
                ////on vd.VendorId=vn.VendorId
                ////WHERE  1=1
                ////
                ////";

                #endregion


                sqlText = @"SELECT vd.VDSId, vd.VendorId ";

                if (TransactionType.ToLower() == "salevds")
                {
                    sqlText += @",cus.CustomerName VendorName";
                    sqlText += @",ISNULL(cus.Email,'') Email";
                }
                else
                {
                    sqlText += @",vn.VendorName";
                    sqlText += @",ISNULL(vn.Email,'') Email";
                }

                sqlText += @"
 
,vd.BillNo
,ISNULL(vd.BillAmount,0) BillAmount
,vd.BillDate
,ISNULL(vd.BillDeductAmount,0) BillDeductAmount
,ISNULL(vd.DepositNumber,0) DepositNumber
,vd.DepositDate
,vd.Remarks
,vd.IssueDate
,vd.PurchaseNumber
,vd.CreatedBy
,vd.CreatedOn
,vd.LastModifiedBy
,vd.LastModifiedOn
,ISNULL(vd.VDSPercent,0) VDSPercent
,vd.IsPurchase
,vd.IsPercent
,vd.ReverseVDSId
,vd.VATAmount
,ISNULL(vd.IsMailSend,'N') IsMailSend

FROM VDS  vd left outer join 

";
                if (TransactionType.ToLower() == "salevds")
                {
                    sqlText += @"
Customers cus ON vd.VendorId=cus.CustomerID LEFT OUTER JOIN
CustomerGroups vg ON cus.CustomerGroupID=vg.CustomerGroupID";
                }
                else
                {
                    sqlText += @"
Vendors vn ON vd.VendorId = vn.VendorID LEFT OUTER JOIN
VendorGroups ON vn.VendorGroupID = VendorGroups.VendorGroupID";
                    
                }

                sqlText += @" WHERE  1=1";

                if (VDSId != null)
                {
                    sqlText += "AND vd.VDSId=@VDSId";
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

                if (VDSId != null)
                {
                    objComm.Parameters.AddWithValue("@VDSId", VDSId);
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
                    vm = new VDSMasterVM();
                    vm.Id = dr["VDSId"].ToString();
                    vm.VendorId = dr["VendorId"].ToString();
                    vm.VendorName = dr["VendorName"].ToString();
                    vm.BillNo = dr["BillNo"].ToString();
                    
                    vm.BillAmount = Convert.ToDecimal(dr["BillAmount"].ToString());
                    vm.BillDate = OrdinaryVATDesktop.DateTimeToDate(dr["BillDate"].ToString());
                    vm.BillDeductedAmount = Convert.ToDecimal(dr["BillDeductAmount"].ToString());
                    vm.DepositNumber = dr["DepositNumber"].ToString();
                    vm.DepositDate = OrdinaryVATDesktop.DateTimeToDate(dr["DepositDate"].ToString());
                    vm.Remarks = dr["Remarks"].ToString();
                    vm.IssueDate = OrdinaryVATDesktop.DateTimeToDate(dr["IssueDate"].ToString());
                    vm.PurchaseNumber = dr["PurchaseNumber"].ToString();
                    vm.CreatedBy = dr["CreatedBy"].ToString();
                    vm.CreatedOn = dr["CreatedOn"].ToString();
                    vm.LastModifiedBy = dr["LastModifiedBy"].ToString();
                    vm.LastModifiedOn = dr["LastModifiedOn"].ToString();
                    vm.VDSPercent = Convert.ToDecimal(dr["VDSPercent"].ToString());
                    vm.IsPurchase = dr["IsPurchase"].ToString();
                    vm.IsPercent = dr["IsPercent"].ToString();
                    ////newly added fields
                    vm.ReverseVDSId = dr["ReverseVDSId"].ToString();
                    vm.VATAmount = Convert.ToDecimal(dr["VATAmount"].ToString());
                    vm.Email = dr["Email"].ToString();
                    vm.IsMailSend = dr["IsMailSend"].ToString();

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
                throw new ArgumentNullException("", "SQL:" + sqlText + sqlex.Message.ToString());
            }
            catch (Exception ex)
            {
                throw new ArgumentNullException("", "SQL:" + sqlText + ex.Message.ToString());
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

        public string[] VDSMailSendUpdate(VDSMasterVM MasterVM, SqlTransaction Vtransaction, SqlConnection VcurrConn, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ

            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            int transResult = 0;
            string sqlText = "";
            #endregion Initializ

            #region Try
             
            try
            {
                #region Validation for VDS

                if (string.IsNullOrEmpty(MasterVM.DepositNumber))
                {
                    throw new ArgumentNullException(MessageVM.spMsgUpdateNotSuccessfully, MessageVM.spMsgUpdateNotSuccessfully);
                }

                #endregion Validation for VDS

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

                #region Update VDS Issue Data

                sqlText = "";
                sqlText +=
                    @" UPDATE VDS SET IsMailSend = 'Y',LastModifiedBy = @updateBy,LastModifiedOn = GETDATE() WHERE VDSId = @DepositNumber ";

                SqlCommand update = new SqlCommand(sqlText, currConn, transaction);
                update.Parameters.AddWithValueAndNullHandle("@DepositNumber", MasterVM.DepositNumber);
                update.Parameters.AddWithValueAndNullHandle("@updateBy", MasterVM.LastModifiedBy);

                transResult = update.ExecuteNonQuery();

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
                retResults[1] = MessageVM.saleMsgUpdateSuccessfully;
                retResults[2] = "" + MasterVM.DepositNumber;
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

                FileLogger.Log("VDSDAL", "VDSMailSendUpdate", ex.ToString() + "\n" + sqlText);

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


    }
}
