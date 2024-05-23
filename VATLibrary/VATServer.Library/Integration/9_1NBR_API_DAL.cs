using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;
using VATViewModel.Integration.NBRAPI;

namespace VATServer.Library.Integration
{
    public class _9_1NBR_API_DAL
    {
        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();
        
        #region API URL

        ////private string PeriodKeyUrl = "http://ivasue2.nbr.gov.bd:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_PeriodKeySet";
        private string PeriodKeyUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_PeriodKeySet";

        private string TypeReturnUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_TypeReturnSet";
        private string AnyActivityUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_RefundTypeSet";
        private string refundTypeUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_RefundTypeSet";

        private string goodsServiceUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_GoodsServiceSet";
        private string goodsService_01Url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_GoodsService_01Set";
        private string goodsService_02Url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_GoodsService_02Set";
        private string goodsService_03Url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_GoodsService_03Set";
        private string goodsService_04Url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_GoodsService_04Set";

        private string CategoryNote8Url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_CategoryNote8Set";
        private string BankUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_BankSet";
        private string cpcCodeUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_CPCCodeSet";
        private string unitUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_UnitSet";
        private string BoEDataUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_CAT_BoESet";


        #endregion

        #region NBR API Json Methods

        public ResultVM Get9_1NBR_Json(NBR_APIVM nbrVm, SysDBInfoVMTemp connVM = null)
        {
            ResultVM vm = new ResultVM();
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            _9_1_VATReturnDAL _VATReturnDal = new _9_1_VATReturnDAL();
            MessageIdVM msgVM = new MessageIdVM();

            try
            {
                string companyBIN = ""; 
                string PeriodName = "";
                string PeriodId = "";
                string periodKey = "";
                string MSGID = "";

                CompanyprofileDAL companyprofile = new CompanyprofileDAL();
                UserInformationDAL userInformationDal = new UserInformationDAL();
                Return_9_1_MainFull _MainFull = new Return_9_1_MainFull();

                #region Get Company info

                CompanyProfileVM companyProfileVm = companyprofile.SelectAllList().FirstOrDefault();

                if (companyProfileVm == null) throw new ArgumentNullException("companyProfileVm");

                companyBIN = companyProfileVm.VatRegistrationNo;

                #endregion

                #region User Info

                UserInformationVM userInformation = userInformationDal.SelectAll(Convert.ToInt32(nbrVm.CurrentUserId)).FirstOrDefault();
                if (userInformation == null) throw new ArgumentNullException("user");

                #endregion

                #region Json For 9.1

                #region Initial Document

                ////XmlDocument soapEnvelopeDocument = GetSoapEnvelope();

                DateTime currentDate = nbrVm.Period; // need to be dynamic

                string FBTyp = "R913";

                PeriodName = currentDate.ToString("MMMM-yyyy");
                PeriodId = currentDate.ToString("MMyyyy");

                #endregion

                try
                {
                    #region Get Period Key

                    msgVM = new MessageIdVM();
                    msgVM.BIN = companyBIN;
                    msgVM.PeriodId = PeriodId;

                    ////string PeriodId = currentDate.ToString("MMyyyy");

                    string APIperiodName = currentDate.ToString("MMMM, yyyy");

                    periodKey = GatNewPeriodKey(msgVM);

                    if (string.IsNullOrWhiteSpace(periodKey))
                    {
                        throw new Exception(PeriodName + " is not available for submit VAT Return");
                    }

                    #endregion

                    #region Set API Header

                    msgVM = new MessageIdVM();

                    msgVM.BIN = companyBIN;
                    msgVM.Depositor = userInformation.FullName;
                    msgVM.FBTyp = FBTyp;
                    msgVM.PeriodKey = periodKey;
                    msgVM.PeriodId = PeriodId;

                    insertAPIHeader(msgVM);

                    MSGID = GetMSGId(nbrVm);

                    #endregion

                    #region Get 9_1 Data

                    VATReturnVM vatReturnVm = new VATReturnVM();
                    vatReturnVm.Date = currentDate.ToString("dd-MMM-yyyy");
                    vatReturnVm.PeriodName = PeriodName;
                    vatReturnVm.PeriodStart = currentDate.ToString("MMMM-yyyy");
                    vatReturnVm.post1 = "Y";
                    vatReturnVm.post2 = "Y";

                    DataSet dsVAT9_1 = _VATReturnDal.VAT9_1_V2Load(vatReturnVm);

                    _MainFull = SetMainTag25_68(dsVAT9_1, PeriodName, userInformation);

                    #endregion

                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Return_9_1_MainFull", Message = e.Message });

                }

                #region Setting Params for subforms

                VATReturnSubFormVM subformVm = new VATReturnSubFormVM();

                subformVm.PeriodName = PeriodName;
                subformVm.ExportInBDT = "Y";
                subformVm.post1 = "Y";
                subformVm.post2 = "Y";
                subformVm.IsSummary = true;
                subformVm.PeriodId = PeriodId;
                subformVm.PeriodKey = periodKey;

                #endregion

                #region Notes 01 to 22

                try
                {
                    subformVm.NoteNos = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22 };
                    SetNote01_22(subformVm, MSGID, companyBIN, companyProfileVm);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 01 to 22", Message = e.Message });
                }

                #endregion

                #region Notes 26,31

                try
                {
                    subformVm.NoteNos = new[] { 26, 31 };
                    //subformVm.TableName = "APINote26_31";
                    SetNote26_31(subformVm, MSGID);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 26 31", Message = e.Message });
                }

                #endregion

                #region Notes 58 to 64

                try
                {
                    subformVm.NoteNos = new[] { 58, 59, 60, 61, 62, 63, 64 };
                    //subformVm.TableName = "APINote58_64";
                    SetNote58_64(subformVm, MSGID, companyBIN);

                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 58 to 68", Message = e.Message });
                }

                #endregion

                #region 27 and 32

                try
                {
                    subformVm.NoteNos = new[] { 27, 32 };
                    //////subformVm.TableName = "APINote27_32";
                    SetNote27_32(subformVm, MSGID);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note 27 and 32", Message = e.Message });
                }

                #endregion

                #region Prepare Note 24 and 29

                try
                {
                    subformVm.NoteNos = new[] { 24, 29 };
                    ////subformVm.TableName = "APINote24_29";

                    SetNote24_29(subformVm, MSGID);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note24_29", Message = e.Message });
                }

                #endregion


                #endregion

                vm.ErrorList = errorMessages;

                if (vm.ErrorList.Count == 0)
                {
                    List<Return_9_1_SF_att_fileSet> _fileSetVMs = new List<Return_9_1_SF_att_fileSet>();

                    string FinalJson = makeJson(PeriodId, _MainFull, _fileSetVMs);
                    vm.Json = FinalJson;

                    vm.Status = "success";
                }
            }
            catch (Exception e)
            {
                vm.Message = e.Message;
                vm.Status = "fail";

                FileLogger.Log("_9_1NBR_API_DAL", "Get9_1NBR_Json", e.ToString());

                errorMessages.Add(new ErrorMessage() { ColumnName = "Main Method", Message = e.Message });

            }

            return vm;
        }

        private string GetMSGId(NBR_APIVM nbrApivm)
        {
            try
            {
                // Get message Id from database
                DataTable dtMsgId = GetMaxMsgId();

                ////string transactionCode = nbrApivm.TransactionType == "9.1" ? "R913" : "R920";
                string transactionCode = "R913";
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string currentId = dtMsgId.Rows[0]["MessageId"].ToString();

                string messageId = transactionCode + currentDate + currentId.PadLeft(8, '0');

                MessageIdVM messageIdVm = new MessageIdVM()
                {
                    MessageId = currentId,
                    PeriodId = nbrApivm.Period.ToString("MMyyyy"),
                    FullId = messageId
                };

                UpdateMaxMsgId(messageIdVm);

                return messageId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataTable GetMaxMsgId(SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Max Id

                SqlText = @"select (max(APIMsgId)+1)MessageId from VAT9_1NBRApiHeader";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                sqlDataAdapter.Fill(table);

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return table;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GetMaxMsgId", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public ResultVM UpdateMaxMsgId(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            ResultVM resultVm = new ResultVM();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("UpdateMaxMsgId");
                }

                #endregion

                SqlText = @"
update VAT9_1NBRApiHeader set APIMsgId = @maxId, MSGID = @MSGID where PeriodID = @periodId";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                cmd.Parameters.AddWithValue("@maxId", msgVM.MessageId);
                cmd.Parameters.AddWithValue("@periodId", msgVM.PeriodId);
                cmd.Parameters.AddWithValue("@MSGID", msgVM.FullId);

                cmd.ExecuteNonQuery();

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return resultVm;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GetLastMsgId", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public DataTable insertAPIHeader(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Delete Exits Period

                SqlText = @"delete VAT9_1NBRApiHeader where PeriodID = @periodId";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@periodId", msgVM.PeriodId);

                cmd.ExecuteNonQuery();

                #endregion

                #region Insert API Header

                SqlText = @"
insert into VAT9_1NBRApiHeader (FBTyp,BIN,PeriodKey,Depositor,PeriodID)
values(@FBTyp,@BIN,@PeriodKey,@Depositor,@PeriodID)
";

                cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@FBTyp", msgVM.FBTyp);
                cmd.Parameters.AddWithValue("@BIN", msgVM.BIN);
                cmd.Parameters.AddWithValue("@PeriodKey", msgVM.PeriodKey);
                cmd.Parameters.AddWithValue("@Depositor", msgVM.Depositor);
                cmd.Parameters.AddWithValue("@periodId", msgVM.PeriodId);

                cmd.ExecuteNonQuery();


                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return table;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "insertAPIHeader", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public Return_9_1_MainFull SetMainTag25_68(DataSet dsVAT9_1, string PerName, UserInformationVM userInformation)
        {
            _9_1_VATReturnDAL rDAL = new _9_1_VATReturnDAL();
            try
            {

                string S2TypeReturn = "";
                string S2AnyActivities = "";

                DataTable vatReternHeader = rDAL.SelectAll_VATReturnHeader(new VATReturnVM() { BranchId = 0, PeriodName = PerName });

                if (vatReternHeader.Rows[0]["MainOrginalReturn"].ToString() == "Y")
                    S2TypeReturn = "1";
                else if (vatReternHeader.Rows[0]["LateReturn"].ToString() == "Y")
                    S2TypeReturn = "4";

                if (string.IsNullOrWhiteSpace(S2TypeReturn))
                {
                    throw new ArgumentNullException("Only accept MainOrginalReturn/LateReturn");
                }

                S2AnyActivities = vatReternHeader.Rows[0]["NoActivites"] == "Y" ? "2" : "1";

                Return_9_1_MainFull _Return_9_1_MainFull = new Return_9_1_MainFull();

                _Return_9_1_MainFull.S2TypeReturn = S2TypeReturn;
                _Return_9_1_MainFull.S2EcoActivities = "2";
                _Return_9_1_MainFull.S2AnyActivities = S2AnyActivities;
                _Return_9_1_MainFull.S5PmtNotBankingChannel = GetNoteValue(dsVAT9_1, "25");
                _Return_9_1_MainFull.S7SDAgainstExport = GetNoteValue(dsVAT9_1, "40");
                _Return_9_1_MainFull.S7InterestOverdueVAT = GetNoteValue(dsVAT9_1, "41");
                _Return_9_1_MainFull.S7InterestOverdueSD = GetNoteValue(dsVAT9_1, "42");
                _Return_9_1_MainFull.S7FinePenalty = GetNoteValue(dsVAT9_1, "43");
                _Return_9_1_MainFull.S7FinePenaltyOther = GetNoteValue(dsVAT9_1, "44");
                _Return_9_1_MainFull.S7ExciseDuty = GetNoteValue(dsVAT9_1, "45");
                _Return_9_1_MainFull.S7DevelopSurcharge = GetNoteValue(dsVAT9_1, "46");
                _Return_9_1_MainFull.S7IctDevelopSurcharge = GetNoteValue(dsVAT9_1, "47");
                _Return_9_1_MainFull.S7HealthCareSurcharge = GetNoteValue(dsVAT9_1, "48");
                _Return_9_1_MainFull.S7EnvProtectSurcharge = GetNoteValue(dsVAT9_1, "49");

                string value67 = GetNoteValue(dsVAT9_1, "67");

                string value68 = GetNoteValue(dsVAT9_1, "68");
                string value0 = (Convert.ToDecimal(value67) + Convert.ToDecimal(value68)) > 0 ? "1" : "2";

                _Return_9_1_MainFull.S11Refund = value0;
                _Return_9_1_MainFull.S11RefundVAT = value67;
                _Return_9_1_MainFull.S11RefundSD = value68;
                _Return_9_1_MainFull.S12Confirm = "X";

                if (!OrdinaryVATDesktop.IsPhoneNumberValid(userInformation.Mobile))
                {
                    throw new ArgumentNullException("Invalid user mobile number.");
                }
                if (string.IsNullOrWhiteSpace(userInformation.FullName) || userInformation.FullName == "-")
                {
                    throw new ArgumentNullException("Invalid user full Name.");
                }
                if (string.IsNullOrWhiteSpace(userInformation.NationalId) || userInformation.NationalId == "-" || userInformation.NationalId == "0")
                {
                    throw new ArgumentNullException("Invalid user NationalId.");
                }
                if (!OrdinaryVATDesktop.IsEmailValid(userInformation.Email))
                {
                    throw new ArgumentNullException("Invalid email address.");
                }

                _Return_9_1_MainFull.S12Mobile = userInformation.Mobile;
                _Return_9_1_MainFull.S12Name = userInformation.FullName;
                _Return_9_1_MainFull.S12Designation = userInformation.Designation;
                _Return_9_1_MainFull.S12Email = userInformation.Email;
                _Return_9_1_MainFull.S12NationalNIDPP = userInformation.NationalId;

                return _Return_9_1_MainFull;

            }
            catch (Exception ex)
            {
                FileLogger.Log("_9_1NBR_API_DAL", "SetMainTag25_68", ex.ToString());
                throw ex;
            }
        }

        public string GetNoteValue(DataSet dsVAT9_1, string NoteNo)
        {
            string value = "0";
            if (string.IsNullOrEmpty(NoteNo))
                return value;

            DataRow[] row = dsVAT9_1.Tables[0].Select("NoteNo = '" + NoteNo + "'");
            value = row[0]["LineA"].ToString();

            if (OrdinaryVATDesktop.IsNumeric(value))
            {
                value = Convert.ToDecimal(value).ToString("0.##");
            }


            return value;
        }

        #region Set Note 26_31

        private void SetNote26_31(VATReturnSubFormVM subformVm, string MSGID)
        {

            CommonDAL commonDAl = new CommonDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DeleteSubFromData("VAT9_1NBRApi_SF_adjustSet", subformVm.PeriodId, FieldId);

                DataTable sunform26_31 = GetSubNoteData(subformVm);

                #region Date formate change

                var VATChallanDate = new DataColumn("VATChallanDate") { DefaultValue = "" };
                var IssueDat = new DataColumn("IssueDat") { DefaultValue = "" };

                sunform26_31.Columns.Add(VATChallanDate);
                sunform26_31.Columns.Add(IssueDat);

                //string[] column = new[] { "VATChallanDate", "IssueDat" };

                DateFormateChange(sunform26_31, "IssuedDate", "IssueDat");
                DateFormateChange(sunform26_31, "TaxChallanDate", "VATChallanDate");

                #endregion

                #region Final sunform26_31 data

                DataTable finalsunform26_31 = Note26_31ColumnNameChage(sunform26_31, FieldId, MSGID, subformVm.NoteNo.ToString(), subformVm.PeriodId);

                #endregion

                #region bulk insert

                retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_adjustSet", finalsunform26_31);

                #endregion
            }

        }

        public DataTable GetSubNoteData(VATReturnSubFormVM subformVm, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            _9_1_VATReturnDAL _VATReturnDAL = new _9_1_VATReturnDAL();
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                VATReturnSubFormVM sVM = new VATReturnSubFormVM();
                sVM.PeriodName = subformVm.PeriodName;
                sVM.IsSummary = true;
                sVM.NoteNo = subformVm.NoteNo;
                sVM.PeriodId = subformVm.PeriodId;

                #region Exits check

                SqlText = @"select count(Id) from VAT9_1SubFormCheck where PeriodID=@PeriodID";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", subformVm.PeriodId);
                int count = (int)cmd.ExecuteScalar();

                #endregion

                if (count == 0)
                {
                    table = _VATReturnDAL.VAT9_1_SubForm(sVM, connVM, true);
                }
                else
                {
                    table = _VATReturnDAL.GetDataVAT9_1_SubFormTable(sVM, connection, transaction, connVM);
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return table;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GetSubNoteData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string[] DeleteSubFromData(string TableName, string periodId, string FieldId, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string SqlText = "";
            string[] retResult = new string[6];
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction();
                }

                #endregion

                #region Delete

                SqlText = @"delete @TableName where PeriodID=@PeriodID and FieldID=@FieldID";

                SqlText = SqlText.Replace("@TableName", TableName);

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", periodId);
                cmd.Parameters.AddWithValue("@FieldID", FieldId);
                cmd.ExecuteNonQuery();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "DeleteSubFromData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public DataTable Note26_31ColumnNameChage(DataTable dt, string FieldID, string MSGID, string NoteNo, string PeriodID)
        {
            DataTable table = new DataTable();

            table = dt.Copy();

            #region Remove Column

            if (table.Columns.Contains("UserName"))
            {
                table.Columns.Remove("UserName");
            }
            if (table.Columns.Contains("Branch"))
            {
                table.Columns.Remove("Branch");
            }
            if (table.Columns.Contains("NoteNo"))
            {
                table.Columns.Remove("NoteNo");
            }
            if (table.Columns.Contains("SubNoteNo"))
            {
                table.Columns.Remove("SubNoteNo");
            }
            if (table.Columns.Contains("Remarks"))
            {
                table.Columns.Remove("Remarks");
            }
            if (table.Columns.Contains("ItemNo"))
            {
                table.Columns.Remove("ItemNo");
            }
            if (table.Columns.Contains("ProductDescription"))
            {
                table.Columns.Remove("ProductDescription");
            }
            if (table.Columns.Contains("ProductName"))
            {
                table.Columns.Remove("ProductName");
            }
            if (table.Columns.Contains("SubFormName"))
            {
                table.Columns.Remove("SubFormName");
            }
            if (table.Columns.Contains("DebitNoteNo"))
            {
                table.Columns.Remove("DebitNoteNo");
            }
            if (table.Columns.Contains("CreditNoteNo"))
            {
                table.Columns.Remove("CreditNoteNo");
            }
            if (table.Columns.Contains("BranchId"))
            {
                table.Columns.Remove("BranchId");
            }
            if (table.Columns.Contains("IssuedDate"))
            {
                table.Columns.Remove("IssuedDate");
            }
            if (table.Columns.Contains("TaxChallanDate"))
            {
                table.Columns.Remove("TaxChallanDate");
            }

            #endregion

            #region Column Add

            if (!table.Columns.Contains("MSGID"))
            {
                var messageID = new DataColumn("MSGID") { DefaultValue = MSGID };
                table.Columns.Add(messageID);
            }
            if (!table.Columns.Contains("FieldID"))
            {
                var FID = new DataColumn("FieldID") { DefaultValue = FieldID };
                table.Columns.Add(FID);
            }
            if (!table.Columns.Contains("NoteNo"))
            {
                var Note = new DataColumn("NoteNo") { DefaultValue = "Note " + NoteNo };
                table.Columns.Add(Note);
            }
            if (!table.Columns.Contains("PeriodID"))
            {
                var FID = new DataColumn("PeriodID") { DefaultValue = PeriodID };
                table.Columns.Add(FID);
            }

            #endregion

            #region Column name change

            ////if (table.Columns.Contains("IssuedDate"))
            ////{
            ////    table.Columns["IssuedDate"].ColumnName = "IssueDat";
            ////}
            if (table.Columns.Contains("TaxChallanNo"))
            {
                table.Columns["TaxChallanNo"].ColumnName = "VATChallanNo";
            }
            //////if (table.Columns.Contains("TaxChallanDate"))
            //////{
            //////    table.Columns["TaxChallanDate"].ColumnName = "VATChallanDate";
            //////}
            if (table.Columns.Contains("ReasonforIssuance"))
            {
                table.Columns["ReasonforIssuance"].ColumnName = "ReasonIssuance";
            }
            if (table.Columns.Contains("ValueinChallan"))
            {
                table.Columns["ValueinChallan"].ColumnName = "ValueChallan";
            }
            if (table.Columns.Contains("QuantityinChallan"))
            {
                table.Columns["QuantityinChallan"].ColumnName = "QuantityinChallan";
            }
            if (table.Columns.Contains("VATinChallan"))
            {
                table.Columns["VATinChallan"].ColumnName = "VATChallan";
            }
            if (table.Columns.Contains("SDinChallan"))
            {
                table.Columns["SDinChallan"].ColumnName = "SDChallan";
            }
            if (table.Columns.Contains("ValueofIncreasingAdjustment"))
            {
                table.Columns["ValueofIncreasingAdjustment"].ColumnName = "ValueIncDecAdj";
            }
            if (table.Columns.Contains("QuantityofIncreasingAdjustment"))
            {
                table.Columns["QuantityofIncreasingAdjustment"].ColumnName = "QuantityIncDecAdj";
            }
            if (table.Columns.Contains("VATofIncreasingAdjustment"))
            {
                table.Columns["VATofIncreasingAdjustment"].ColumnName = "VATIncDecAdj";
            }
            if (table.Columns.Contains("SDofIncreasingAdjustment"))
            {
                table.Columns["SDofIncreasingAdjustment"].ColumnName = "SDIncDecAdj";
            }

            if (table.Columns.Contains("ValueofDecreasingAdjustment"))
            {
                table.Columns["ValueofDecreasingAdjustment"].ColumnName = "ValueIncDecAdj";
            }
            if (table.Columns.Contains("QuantityofDecreasingAdjustment"))
            {
                table.Columns["QuantityofDecreasingAdjustment"].ColumnName = "QuantityIncDecAdj";
            }
            if (table.Columns.Contains("VATofDecreasingAdjustment"))
            {
                table.Columns["VATofDecreasingAdjustment"].ColumnName = "VATIncDecAdj";
            }
            if (table.Columns.Contains("SDofDecreasingAdjustment"))
            {
                table.Columns["SDofDecreasingAdjustment"].ColumnName = "SDIncDecAdj";
            }

            #endregion

            return table;

        }

        #endregion

        #region Set note 58 to 64

        private void SetNote58_64(VATReturnSubFormVM subformVm, string MSGID, string CompanyBIN)
        {

            CommonDAL commonDAl = new CommonDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DeleteSubFromData("VAT9_1NBRApi_SF_challanSet", subformVm.PeriodId, FieldId);

                DataTable sunform58_64 = GetSubNoteData(subformVm);

                DataTable dtAPIdata = SetNote58_64Banks(sunform58_64, CompanyBIN);

                #region Date formate change

                var ChallanDate = new DataColumn("ChallanDate") { DefaultValue = "" };
                dtAPIdata.Columns.Add(ChallanDate);

                DateFormateChange(dtAPIdata, "Date", "ChallanDate");

                #endregion

                #region Final sunform26_31 data

                DataTable finalsunform58_64 = Note58_64ColumnNameChage(dtAPIdata, FieldId, MSGID, subformVm.NoteNo.ToString(), subformVm.PeriodId);

                #endregion

                #region bulk insert

                retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_challanSet", finalsunform58_64);

                #endregion
            }

        }

        private DataTable SetNote58_64Banks(DataTable dt, string CompanyBIN)
        {
            DataTable sunform58_64 = new DataTable();

            try
            {

                sunform58_64 = dt.Copy();

                if (!sunform58_64.Columns.Contains("BankCode"))
                {
                    var BankCode = new DataColumn("BankCode") { DefaultValue = "" };
                    sunform58_64.Columns.Add(BankCode);
                }
                if (!sunform58_64.Columns.Contains("BranchCode"))
                {
                    var BranchCode = new DataColumn("BranchCode") { DefaultValue = "" };
                    sunform58_64.Columns.Add(BranchCode);
                }

                foreach (DataRow dataRow in sunform58_64.Rows)
                {
                    string bankName = dataRow["BankName"].ToString();
                    string bankBranchName = dataRow["BankBranch"].ToString();

                    NBRApiResult bank = get_bank(CompanyBIN, bankName, bankBranchName);

                    if (bank == null)
                        throw new Exception("Bank/Branch Name Does Not Match with NBR");

                    dataRow["BankCode"] = bank.BanCD;
                    dataRow["BranchCode"] = bank.RoutingNumber;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return sunform58_64;
        }

        public DataTable Note58_64ColumnNameChage(DataTable dt, string FieldID, string MSGID, string NoteNo, string PeriodID)
        {
            DataTable table = new DataTable();

            //table = dt.Copy();

            DataView view = new DataView(dt);

            table = view.ToTable(false, "ChallanNumber", "ChallanDate", "Amount", "Remarks");


            #region Remove Column

            ////if (table.Columns.Contains("UserName"))
            ////{
            ////    table.Columns.Remove("UserName");
            ////}
            ////if (table.Columns.Contains("Branch"))
            ////{
            ////    table.Columns.Remove("Branch");
            ////}
            ////if (table.Columns.Contains("NoteNo"))
            ////{
            ////    table.Columns.Remove("NoteNo");
            ////}
            ////if (table.Columns.Contains("SubNoteNo"))
            ////{
            ////    table.Columns.Remove("SubNoteNo");
            ////}
            ////if (table.Columns.Contains("VATAmount"))
            ////{
            ////    table.Columns.Remove("VATAmount");
            ////}            
            ////if (table.Columns.Contains("SubFormName"))
            ////{
            ////    table.Columns.Remove("SubFormName");
            ////}
            ////if (table.Columns.Contains("SDAmount"))
            ////{
            ////    table.Columns.Remove("SDAmount");
            ////}
            ////if (table.Columns.Contains("BankName"))
            ////{
            ////    table.Columns.Remove("BankName");
            ////}
            ////if (table.Columns.Contains("BankBranch"))
            ////{
            ////    table.Columns.Remove("BankBranch");
            ////}
            ////if (table.Columns.Contains("AccountCode"))
            ////{
            ////    table.Columns.Remove("AccountCode");
            ////}
            ////if (table.Columns.Contains("DepositId"))
            ////{
            ////    table.Columns.Remove("DepositId");
            ////}
            ////if (table.Columns.Contains("DetailRemarks"))
            ////{
            ////    table.Columns.Remove("DetailRemarks");
            ////}

            #endregion

            #region Column Add

            if (!table.Columns.Contains("MSGID"))
            {
                var messageID = new DataColumn("MSGID") { DefaultValue = MSGID };
                table.Columns.Add(messageID);
            }
            if (!table.Columns.Contains("FieldID"))
            {
                var FID = new DataColumn("FieldID") { DefaultValue = FieldID };
                table.Columns.Add(FID);
            }
            if (!table.Columns.Contains("PeriodID"))
            {
                var FID = new DataColumn("PeriodID") { DefaultValue = PeriodID };
                table.Columns.Add(FID);
            }
            //////if (!table.Columns.Contains("NoteNo"))
            //////{
            //////    var Note = new DataColumn("NoteNo") { DefaultValue = "Note " + NoteNo };
            //////    table.Columns.Add(Note);
            //////}

            #endregion

            #region Column name change

            if (table.Columns.Contains("ChallanNumber"))
            {
                table.Columns["ChallanNumber"].ColumnName = "TreasuryChallanToken";
            }
            ////if (table.Columns.Contains("Date"))
            ////{
            ////    table.Columns["Date"].ColumnName = "ChallanDate";
            ////}
            if (table.Columns.Contains("Amount"))
            {
                table.Columns["Amount"].ColumnName = "Amount";
            }
            if (table.Columns.Contains("Remarks"))
            {
                table.Columns["Remarks"].ColumnName = "Notes";
            }

            #endregion

            return table;

        }

        #endregion

        #region Set note 27 and 32

        private void SetNote27_32(VATReturnSubFormVM subformVm, string MSGID)
        {

            CommonDAL commonDAl = new CommonDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DeleteSubFromData("VAT9_1NBRApi_SF_otherSet", subformVm.PeriodId, FieldId);

                DataTable sunform27_32 = GetSubNoteData(subformVm);

                #region Date formate change

                var ChallanDate = new DataColumn("ChallanDate") { DefaultValue = "" };
                sunform27_32.Columns.Add(ChallanDate);

                DateFormateChange(sunform27_32, "Date", "ChallanDate");

                #endregion

                #region Final sunform 27_32 data

                DataTable finalsunform27_32 = Note27_32ColumnNameChage(sunform27_32, FieldId, MSGID, subformVm.NoteNo.ToString(), subformVm.PeriodId);

                #endregion

                #region bulk insert

                retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_otherSet", finalsunform27_32);

                #endregion
            }

        }

        public DataTable Note27_32ColumnNameChage(DataTable dt, string FieldID, string MSGID, string NoteNo, string PeriodID)
        {
            DataTable table = new DataTable();

            DataView view = new DataView(dt);

            table = view.ToTable(false, "ChallanNumber", "ChallanDate", "Amount", "VAT", "Notes");

            #region Remove Column

            ////if (table.Columns.Contains("UserName"))
            ////{
            ////    table.Columns.Remove("UserName");
            ////}
            ////if (table.Columns.Contains("Branch"))
            ////{
            ////    table.Columns.Remove("Branch");
            ////}
            ////if (table.Columns.Contains("NoteNo"))
            ////{
            ////    table.Columns.Remove("NoteNo");
            ////}
            ////if (table.Columns.Contains("SubNoteNo"))
            ////{
            ////    table.Columns.Remove("SubNoteNo");
            ////}
            ////if (table.Columns.Contains("VATAmount"))
            ////{
            ////    table.Columns.Remove("VATAmount");
            ////}            
            ////if (table.Columns.Contains("SubFormName"))
            ////{
            ////    table.Columns.Remove("SubFormName");
            ////}
            ////if (table.Columns.Contains("SDAmount"))
            ////{
            ////    table.Columns.Remove("SDAmount");
            ////}
            ////if (table.Columns.Contains("BankName"))
            ////{
            ////    table.Columns.Remove("BankName");
            ////}
            ////if (table.Columns.Contains("BankBranch"))
            ////{
            ////    table.Columns.Remove("BankBranch");
            ////}
            ////if (table.Columns.Contains("AccountCode"))
            ////{
            ////    table.Columns.Remove("AccountCode");
            ////}
            ////if (table.Columns.Contains("DepositId"))
            ////{
            ////    table.Columns.Remove("DepositId");
            ////}
            ////if (table.Columns.Contains("DetailRemarks"))
            ////{
            ////    table.Columns.Remove("DetailRemarks");
            ////}

            #endregion

            #region Column Add

            if (!table.Columns.Contains("MSGID"))
            {
                var messageID = new DataColumn("MSGID") { DefaultValue = MSGID };
                table.Columns.Add(messageID);
            }
            if (!table.Columns.Contains("FieldID"))
            {
                var FID = new DataColumn("FieldID") { DefaultValue = FieldID };
                table.Columns.Add(FID);
            }
            if (!table.Columns.Contains("PeriodID"))
            {
                var FID = new DataColumn("PeriodID") { DefaultValue = PeriodID };
                table.Columns.Add(FID);
            }

            #endregion

            #region Column name change

            if (table.Columns.Contains("ChallanDate"))
            {
                table.Columns["ChallanDate"].ColumnName = "Date";
            }
            if (table.Columns.Contains("Amount"))
            {
                table.Columns["Amount"].ColumnName = "Value";
            }
            if (table.Columns.Contains("Notes"))
            {
                table.Columns["Notes"].ColumnName = "PurposeNotes";
            }

            #endregion

            return table;

        }

        #endregion

        #region Set note 24 and 29

        private void SetNote24_29(VATReturnSubFormVM subformVm, string MSGID)
        {

            CommonDAL commonDAl = new CommonDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DeleteSubFromData("VAT9_1NBRApi_SF_vdsSet", subformVm.PeriodId, FieldId);

                DataTable sunform24_29 = GetSubNoteData(subformVm);

                #region Date formate change

                var InvoiceChallanBillDate = new DataColumn("InvoiceChallanBillDate") { DefaultValue = "" };
                var VATDeductionatSourceCerDate = new DataColumn("VATDeductionatSourceCerDate") { DefaultValue = "" };
                var TaxDepositedDate = new DataColumn("TaxDepositedDate") { DefaultValue = "" };

                sunform24_29.Columns.Add(InvoiceChallanBillDate);
                sunform24_29.Columns.Add(VATDeductionatSourceCerDate);
                sunform24_29.Columns.Add(TaxDepositedDate);

                DateFormateChange(sunform24_29, "InvoiceDate", "InvoiceChallanBillDate");
                DateFormateChange(sunform24_29, "VDSCertificateDate", "VATDeductionatSourceCerDate");
                DateFormateChange(sunform24_29, "TaxDepositDate", "TaxDepositedDate");

                #endregion

                #region Final sunform 24_29 data

                DataTable finalsunform24_29 = Note24_29ColumnNameChage(sunform24_29, FieldId, MSGID, subformVm.NoteNo.ToString(), subformVm.PeriodId);

                #endregion

                #region bulk insert

                retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_vdsSet", finalsunform24_29);

                #endregion
            }

        }

        public DataTable Note24_29ColumnNameChage(DataTable dt, string FieldID, string MSGID, string NoteNo, string PeriodID)
        {
            DataTable table = new DataTable();

            DataView view = new DataView(dt);

            if (NoteNo == "24")
            {
                table = view.ToTable(false, "VendorBIN", "VendorName", "VendorAddress", "TotalPrice", "VDSAmount", "InvoiceNo", "InvoiceChallanBillDate", "VDSCertificateNo", "VATDeductionatSourceCerDate", "TaxDepositSerialNo", "TaxDepositedDate", "Remarks");
            }
            else if (NoteNo == "29")
            {
                table = view.ToTable(false, "CustomerBIN", "CustomerName", "CustomerAddress", "TotalPrice", "VDSAmount", "InvoiceNo", "InvoiceChallanBillDate", "VDSCertificateNo", "VATDeductionatSourceCerDate", "SerialNo", "TaxDepositedDate", "Remarks");
            }

            #region Column Add

            if (!table.Columns.Contains("MSGID"))
            {
                var messageID = new DataColumn("MSGID") { DefaultValue = MSGID };
                table.Columns.Add(messageID);
            }
            if (!table.Columns.Contains("FieldID"))
            {
                var FID = new DataColumn("FieldID") { DefaultValue = FieldID };
                table.Columns.Add(FID);
            }
            if (!table.Columns.Contains("PeriodID"))
            {
                var FID = new DataColumn("PeriodID") { DefaultValue = PeriodID };
                table.Columns.Add(FID);
            }

            #endregion

            #region Column name change

            if (table.Columns.Contains("VendorBIN"))
            {
                table.Columns["VendorBIN"].ColumnName = "BuyerSupplyerBIN";
            }
            if (table.Columns.Contains("VendorName"))
            {
                table.Columns["VendorName"].ColumnName = "BuyerSupplyerName";
            }
            if (table.Columns.Contains("VendorAddress"))
            {
                table.Columns["VendorAddress"].ColumnName = "BuyerSupplyerAddress";
            }
            if (table.Columns.Contains("CustomerBIN"))
            {
                table.Columns["CustomerBIN"].ColumnName = "BuyerSupplyerBIN";
            }
            if (table.Columns.Contains("CustomerName"))
            {
                table.Columns["CustomerName"].ColumnName = "BuyerSupplyerName";
            }
            if (table.Columns.Contains("CustomerAddress"))
            {
                table.Columns["CustomerAddress"].ColumnName = "BuyerSupplyerAddress";
            }
            if (table.Columns.Contains("TotalPrice"))
            {
                table.Columns["TotalPrice"].ColumnName = "Value";
            }
            if (table.Columns.Contains("VDSAmount"))
            {
                table.Columns["VDSAmount"].ColumnName = "DeductedVAT";
            }
            if (table.Columns.Contains("InvoiceNo"))
            {
                table.Columns["InvoiceNo"].ColumnName = "InvoiceNoChallanBillNo";
            }
            if (table.Columns.Contains("VDSCertificateNo"))
            {
                table.Columns["VDSCertificateNo"].ColumnName = "VATDeductionatSource";
            }
            if (table.Columns.Contains("SerialNo"))
            {
                table.Columns["SerialNo"].ColumnName = "TaxDepositedSerialBookTransfer";
            }
            if (table.Columns.Contains("TaxDepositSerialNo"))
            {
                table.Columns["TaxDepositSerialNo"].ColumnName = "TaxDepositedSerialBookTransfer";
            }
            if (table.Columns.Contains("Remarks"))
            {
                table.Columns["Remarks"].ColumnName = "Notes";
            }

            #endregion

            return table;

        }

        #endregion

        #region Set note 01 to 22

        private void SetNote01_22(VATReturnSubFormVM subformVm, string MSGID, string CompanyBin, CompanyProfileVM companyProfileVm)
        {
            CommonDAL commonDAl = new CommonDAL();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                #region Make Datatable

                DataTable dt = new DataTable();
                dt.Clear();

                dt.Columns.Add("MSGID");
                dt.Columns.Add("FieldID");
                dt.Columns.Add("ValueBasevalueVAT");
                dt.Columns.Add("SD");
                dt.Columns.Add("VAT");
                dt.Columns.Add("HSCommercialDescription");
                dt.Columns.Add("Notes");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("ActualSalesPurchasesValue");
                dt.Columns.Add("UnitMeasure");
                dt.Columns.Add("ItemID");
                dt.Columns.Add("Category");
                dt.Columns.Add("DataSource");
                dt.Columns.Add("BoeNo");
                dt.Columns.Add("BoEDate");
                dt.Columns.Add("BoEOficeCode");
                dt.Columns.Add("BoEItemNo");
                dt.Columns.Add("CPCCode");
                dt.Columns.Add("AssessableValue");
                dt.Columns.Add("AT");
                dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("InvoiceDate");
                dt.Columns.Add("PeriodID");

                #endregion

                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DeleteSubFromData("VAT9_1NBRApi_SF_goservSet", subformVm.PeriodId, FieldId);

                DataTable subformData = GetSubNoteData(subformVm);

                foreach (DataRow row in subformData.Rows)
                {
                    DataRow dr = dt.NewRow();

                    dr["MSGID"] = MSGID;
                    dr["FieldID"] = FieldId;

                    string CatagoryId = "";

                    #region Note 13=SL13, 15=SL15, 17=SL17, 23=SL23

                    //Note 13=SL13, 15=SL15, 17=SL17, 23=SL23
                    if (FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {

                        #region Comments

                        ////////var DataSource = new DataColumn("DataSource") { DefaultValue = "1" };
                        ////////var BoeNo = new DataColumn("BoeNo") { DefaultValue = "" };
                        ////////var BoEDate = new DataColumn("BoEDate") { DefaultValue = "" };
                        ////////var BoEOficeCode = new DataColumn("BoEOficeCode") { DefaultValue = "" };
                        ////////var BoEItemNo = new DataColumn("BoEItemNo") { DefaultValue = "" };
                        ////////var CPCCode = new DataColumn("CPCCode") { DefaultValue = "" };

                        ////////subformData.Columns.Add(DataSource);
                        ////////subformData.Columns.Add(BoeNo);
                        ////////subformData.Columns.Add(BoEDate);
                        ////////subformData.Columns.Add(BoEOficeCode);
                        ////////subformData.Columns.Add(BoEItemNo);
                        ////////subformData.Columns.Add(CPCCode);

                        #endregion

                        #region BoE information

                        if (!string.IsNullOrWhiteSpace(row["Invoice/B/E No"].ToString()))
                        {
                            string BENo = row["Invoice/B/E No"].ToString();

                            NBRAPI_paramVM ParamVM = new NBRAPI_paramVM();

                            ParamVM.CompanyBIN = CompanyBin;
                            ParamVM.BoENumber = BENo;
                            ParamVM.PeriodKey = subformVm.PeriodKey;
                            ParamVM.Serial = FieldId;

                            NBRApiResult nbrVM = GatBoEData(ParamVM);

                            if (nbrVM == null)
                            {
                                throw new Exception("BE number is not match with IVAS for note " + Note);
                            }

                            dr["DataSource"] = "1";
                            dr["BoeNo"] = nbrVM.BoENumber;
                            dr["BoEDate"] = nbrVM.BoEDate;
                            dr["BoEOficeCode"] = nbrVM.BoEOffCode;
                            dr["BoEItemNo"] = nbrVM.BoEItmNo;
                            dr["CPCCode"] = nbrVM.CPCCode;

                        }
                        else
                        {
                            throw new Exception("BE number is mendatory for note " + Note);
                        }

                        #endregion

                    }

                    #endregion

                    #region ValueBasevalueVAT

                    if (FieldId != "SL06" || FieldId != "SL18")
                    {
                        dr["ValueBasevalueVAT"] = row["BasevalueofVAT"].ToString();
                    }

                    #endregion

                    #region SD  VAT

                    //SL01, SL02, SL11, SL13, SL15, SL17, SL23

                    if (FieldId == "SL01" || FieldId == "SL02" || FieldId == "SL11" || FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {
                        dr["SD"] = row["SD"].ToString();
                        dr["VAT"] = row["VAT"].ToString();
                        dr["AssessableValue"] = row["Assessablevalue"].ToString();

                        if (FieldId != "SL02")
                        {
                            dr["AT"] = row["AT"].ToString();
                        }

                        if (FieldId == "SL02")
                        {
                            dr["InvoiceNo"] = row["Invoice/B/E No"].ToString();
                            dr["InvoiceDate"] = Convert.ToDateTime(row["Date"].ToString()).ToString("yyyyMMdd");
                        }

                    }

                    #endregion

                    #region BoE SL01 SL11

                    if (FieldId == "SL01" || FieldId == "SL11")
                    {
                        dr["BoeNo"] = row["Invoice/B/E No"].ToString();
                        dr["BoEDate"] = Convert.ToDateTime(row["Date"].ToString()).ToString("yyyyMMdd");
                        dr["BoEOficeCode"] = row["OfficeCode"].ToString();
                        dr["BoEItemNo"] = row["BE_ItemNo"].ToString();
                        dr["CPCCode"] = row["CPC"].ToString();

                    }

                    #endregion

                    #region HSCommercialDescription

                    dr["HSCommercialDescription"] = row["ProductDescription"].ToString();
                    dr["Notes"] = row["DetailRemarks"].ToString();

                    #endregion

                    #region Quantity

                    if (FieldId == "SL06" || FieldId == "SL18")
                    {
                        dr["Quantity"] = row["Quantity"].ToString();
                        dr["ActualSalesPurchasesValue"] = row["TotalPrice"].ToString();
                    }

                    #endregion

                    #region Category

                    if (FieldId == "SL08")
                    {
                        CompanyCategoryVM companyCategoryVM = companyprofile.GetCompanyTypes(null, null, companyProfileVm.CompanyType).FirstOrDefault();

                        NBRAPI_paramVM cpVM = new NBRAPI_paramVM();

                        cpVM.CompanyBIN = CompanyBin;
                        cpVM.PeriodKey = subformVm.PeriodKey;

                        CatagoryId = GatNewCompanyCategory(cpVM);

                        dr["Category"] = CatagoryId;

                    }

                    #endregion

                    #region Get Item ID

                    string ServiceCode = row["ProductCode"].ToString();

                    NBRAPI_paramVM PVM = new NBRAPI_paramVM();
                    PVM.CompanyBIN = CompanyBin;
                    PVM.GoodsServiceCode = ServiceCode;
                    PVM.PeriodKey = subformVm.PeriodKey;
                    PVM.Serial = FieldId;
                    PVM.Note = FieldId;
                    PVM.CategoryID = CatagoryId;

                    NBRApiResult NBRAPIvm = GatGoodsServiceData(PVM);

                    dr["ItemID"] = NBRAPIvm.ItemID;

                    #endregion

                    dr["PeriodID"] = subformVm.PeriodId;

                    dt.Rows.Add(dr);

                }

                #region bulk insert

                if (dt != null && dt.Rows.Count > 0)
                {
                    retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_goservSet", dt);
                }

                #endregion

            }

        }

        #endregion

        #region Set Attachments

        private void SetAttachments(VATReturnSubFormVM subformVm, string MSGID, string CompanyBin, CompanyProfileVM companyProfileVm)
        {
            CommonDAL commonDAl = new CommonDAL();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();
            ////ReportDocument reportDocument = new ReportDocument();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            foreach (int Note in subformVm.NoteNos)
            {
                #region Make Datatable

                DataTable dt = new DataTable();
                dt.Clear();

                dt.Columns.Add("MSGID");
                dt.Columns.Add("FieldID");
                dt.Columns.Add("ValueBasevalueVAT");
                dt.Columns.Add("SD");
                dt.Columns.Add("VAT");
                dt.Columns.Add("HSCommercialDescription");
                dt.Columns.Add("Notes");
                dt.Columns.Add("Quantity");
                dt.Columns.Add("ActualSalesPurchasesValue");
                dt.Columns.Add("UnitMeasure");
                dt.Columns.Add("ItemID");
                dt.Columns.Add("Category");
                dt.Columns.Add("DataSource");
                dt.Columns.Add("BoeNo");
                dt.Columns.Add("BoEDate");
                dt.Columns.Add("BoEOficeCode");
                dt.Columns.Add("BoEItemNo");
                dt.Columns.Add("CPCCode");
                dt.Columns.Add("AssessableValue");
                dt.Columns.Add("AT");
                dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("InvoiceDate");
                dt.Columns.Add("PeriodID");

                #endregion

                subformVm.NoteNo = Note;

                string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                DataTable subformData = GetSubNoteData(subformVm);

                foreach (DataRow row in subformData.Rows)
                {
                    DataRow dr = dt.NewRow();

                    dr["MSGID"] = MSGID;
                    dr["FieldID"] = FieldId;

                    string CatagoryId = "";

                    #region Note 13=SL13, 15=SL15, 17=SL17, 23=SL23

                    //Note 13=SL13, 15=SL15, 17=SL17, 23=SL23
                    if (subformVm.NoteNo == 24 || subformVm.NoteNo == 29)
                    {
                        if (subformVm.NoteNo == 24)
                        {
                            ////NBRReports _report = new NBRReports();
                            ////reportDocument = _report.VDS12KhaNew(txtVendorId.Text.Trim(), txtDepositNumber.Text.Trim(), DepositFrom,
                            ////                                      DepositTo, IssueFrom, IssueTo, BillFrom, BillTo,
                            ////                                      txtPurchaseNumber.Text.Trim(), chkPurchaseVDS.Checked, connVM);

                        }

                    }

                    #endregion

                    #region ValueBasevalueVAT

                    if (FieldId != "SL06" || FieldId != "SL18")
                    {
                        dr["ValueBasevalueVAT"] = row["BasevalueofVAT"].ToString();
                    }

                    #endregion

                    #region SD  VAT

                    //SL01, SL02, SL11, SL13, SL15, SL17, SL23

                    if (FieldId == "SL01" || FieldId == "SL02" || FieldId == "SL11" || FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {
                        dr["SD"] = row["SD"].ToString();
                        dr["VAT"] = row["VAT"].ToString();
                        dr["AssessableValue"] = row["Assessablevalue"].ToString();

                        if (FieldId != "SL02")
                        {
                            dr["AT"] = row["AT"].ToString();
                        }

                        if (FieldId == "SL02")
                        {
                            dr["InvoiceNo"] = row["Invoice/B/E No"].ToString();
                            dr["InvoiceDate"] = Convert.ToDateTime(row["Date"].ToString()).ToString("yyyyMMdd");
                        }

                    }

                    #endregion

                    #region BoE SL01 SL11

                    if (FieldId == "SL01" || FieldId == "SL11")
                    {
                        dr["BoeNo"] = row["Invoice/B/E No"].ToString();
                        dr["BoEDate"] = Convert.ToDateTime(row["Date"].ToString()).ToString("yyyyMMdd");
                        dr["BoEOficeCode"] = row["OfficeCode"].ToString();
                        dr["BoEItemNo"] = row["BE_ItemNo"].ToString();
                        dr["CPCCode"] = row["CPC"].ToString();

                    }

                    #endregion

                    #region HSCommercialDescription

                    dr["HSCommercialDescription"] = row["ProductDescription"].ToString();
                    dr["Notes"] = row["DetailRemarks"].ToString();

                    #endregion

                    #region Quantity

                    if (FieldId == "SL06" || FieldId == "SL18")
                    {
                        dr["Quantity"] = row["Quantity"].ToString();
                        dr["ActualSalesPurchasesValue"] = row["TotalPrice"].ToString();
                    }

                    #endregion

                    #region Category

                    if (FieldId == "SL08")
                    {
                        CompanyCategoryVM companyCategoryVM = companyprofile.GetCompanyTypes(null, null, companyProfileVm.CompanyType).FirstOrDefault();

                        NBRAPI_paramVM cpVM = new NBRAPI_paramVM();

                        cpVM.CompanyBIN = CompanyBin;
                        cpVM.PeriodKey = subformVm.PeriodKey;

                        CatagoryId = GatNewCompanyCategory(cpVM);

                        dr["Category"] = CatagoryId;

                    }

                    #endregion

                    #region Get Item ID

                    string ServiceCode = row["ProductCode"].ToString();

                    NBRAPI_paramVM PVM = new NBRAPI_paramVM();
                    PVM.CompanyBIN = CompanyBin;
                    PVM.GoodsServiceCode = ServiceCode;
                    PVM.PeriodKey = subformVm.PeriodKey;
                    PVM.Serial = FieldId;
                    PVM.Note = FieldId;
                    PVM.CategoryID = CatagoryId;

                    NBRApiResult NBRAPIvm = GatGoodsServiceData(PVM);

                    dr["ItemID"] = NBRAPIvm.ItemID;

                    #endregion

                    dr["PeriodID"] = subformVm.PeriodId;

                    dt.Rows.Add(dr);

                }

                #region bulk insert

                if (dt != null && dt.Rows.Count > 0)
                {
                    retResults = commonDAl.BulkInsert("VAT9_1NBRApi_SF_goservSet", dt);
                }

                #endregion

            }

        }


        #endregion

        public string GetFieldIDByNote(string noteNo)
        {
            ////string noteNo = "1";
            string FieldID;

            switch (noteNo)
            {
                case "1":
                    FieldID = "SL01";
                    break;
                case "2":
                    FieldID = "SL02";
                    break;
                case "3":
                    FieldID = "SL03";
                    break;
                case "4":
                    FieldID = "SL04";
                    break;
                case "5":
                    FieldID = "SL05";
                    break;
                case "6":
                    FieldID = "SL06";
                    break;
                case "7":
                    FieldID = "SL07";
                    break;
                case "8":
                    FieldID = "SL08";
                    break;
                case "9":
                    FieldID = "SL09";
                    break;
                case "10":
                    FieldID = "SL10";
                    break;
                case "11":
                    FieldID = "SL11";
                    break;
                case "12":
                    FieldID = "SL12";
                    break;
                case "13":
                    FieldID = "SL13";
                    break;
                case "14":
                    FieldID = "SL14";
                    break;
                case "15":
                    FieldID = "SL15";
                    break;
                case "16":
                    FieldID = "SL16";
                    break;
                case "17":
                    FieldID = "SL17";
                    break;
                case "18":
                    FieldID = "SL18";
                    break;
                case "19":
                    FieldID = "SL20";
                    break;
                case "20":
                    FieldID = "SL21";
                    break;
                case "21":
                    FieldID = "SL22";
                    break;
                case "22":
                    FieldID = "SL23";
                    break;
                case "24":
                    FieldID = "SL25";
                    break;
                case "26":
                    FieldID = "SL126";
                    break;
                case "27":
                    FieldID = "SL127";
                    break;
                case "29":
                    FieldID = "SL30";
                    break;
                case "31":
                    FieldID = "SL131";
                    break;
                case "32":
                    FieldID = "SL132";
                    break;
                case "58":
                    FieldID = "SL53";
                    break;
                case "59":
                    FieldID = "SL54";
                    break;
                case "60":
                    FieldID = "SL58";
                    break;
                case "61":
                    FieldID = "SL59";
                    break;
                case "62":
                    FieldID = "SL60";
                    break;
                case "63":
                    FieldID = "SL61";
                    break;
                case "64":
                    FieldID = "SL62";
                    break;
                default:
                    // Handle the case when noteNo is not found in the mapping
                    FieldID = "";
                    break;
            }


            return FieldID;

        }

        private void DateFormateChange(DataTable table, string ColumnName, string formatedColumn)
        {
            if (table != null && table.Rows.Count > 0)
            {
                if (table.Columns.Contains(ColumnName))
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(row[ColumnName].ToString()))
                        {
                            DateTime date = Convert.ToDateTime(row[ColumnName]);

                            string NBRDate = date.ToString("yyyyMMdd");

                            row[formatedColumn] = NBRDate;
                        }
                    }
                }
            }

        }

        public string makeJson(string PeriodId, Return_9_1_MainFull MainFull, List<Return_9_1_SF_att_fileSet> _fileSetVMs)
        {
            string json = "";

            try
            {

                DataSet ds = GetNBR_API_SubfromData(PeriodId);

                DataTable dtHeader = ds.Tables[0];
                DataTable dtAdjustSet = ds.Tables[1];
                DataTable dtchallanSet = ds.Tables[2];
                DataTable dtotherSet = ds.Tables[3];
                DataTable dtvdsSet = ds.Tables[4];
                DataTable dtgoservSet = ds.Tables[5];

                Return_9_1_Header HeaderVM = new Return_9_1_Header();

                if (dtHeader != null && dtHeader.Rows.Count > 0)
                {
                    HeaderVM = dtHeader.ToList<Return_9_1_Header>().FirstOrDefault();
                    HeaderVM.SendDate = DateTime.Now.ToString("yyyyMMdd");
                    HeaderVM.SendTime = DateTime.Now.ToString("HHMMss");

                    HeaderVM.Return_9_1_MainFull = MainFull;

                    if (dtAdjustSet != null && dtAdjustSet.Rows.Count > 0)
                    {
                        List<Return_9_1_SF_adjustSet> adjustSetVM = dtAdjustSet.ToList<Return_9_1_SF_adjustSet>();

                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_adjustSet = adjustSetVM;
                    }
                    if (_fileSetVMs != null && _fileSetVMs.Count > 0)
                    {
                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_att_fileSet = _fileSetVMs;
                    }

                    if (dtchallanSet != null && dtchallanSet.Rows.Count > 0)
                    {
                        List<Return_9_1_SF_challanSet> challanSet = dtchallanSet.ToList<Return_9_1_SF_challanSet>();

                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_challanSet = challanSet;
                    }

                    if (dtgoservSet != null && dtgoservSet.Rows.Count > 0)
                    {
                        List<Return_9_1_SF_goservSet> goservSetVM = dtgoservSet.ToList<Return_9_1_SF_goservSet>();

                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_goservSet = goservSetVM;
                    }

                    if (dtotherSet != null && dtotherSet.Rows.Count > 0)
                    {
                        List<Return_9_1_SF_otherSet> otherSet = dtotherSet.ToList<Return_9_1_SF_otherSet>();

                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_otherSet = otherSet;
                    }

                    if (dtvdsSet != null && dtvdsSet.Rows.Count > 0)
                    {
                        List<Return_9_1_SF_vdsSet> vdsSet = dtvdsSet.ToList<Return_9_1_SF_vdsSet>();

                        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_vdsSet = vdsSet;
                    }

                    json = JsonConvert.SerializeObject(HeaderVM);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return json;

        }

        public DataSet GetNBR_API_SubfromData(string PeriodId, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataSet ds = new DataSet();

            #endregion

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

                CommonDAL commonDal = new CommonDAL();

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 

----------API Header----------0-----
SELECT 
 isnull(MSGID,'')MSGID
,isnull(FBTyp,'')FBTyp
,isnull(BIN,'')BIN
,isnull(PeriodKey,'')PeriodKey
,isnull(Depositor,'')Depositor
FROM VAT9_1NBRApiHeader where 1=1 and PeriodID=@PeriodID

-------- Note 26 and 31-------1-----
SELECT 
 isnull(MSGID,'')            MSGID
,isnull(FieldID,'')          FieldID
,isnull(NoteNo,'')           NoteNo
,isnull(IssueDat,'')         IssueDate
,isnull(VATChallanNo,'')     VATChallanNo
,isnull(VATChallanDate,'')   VATChallanDate
,isnull(ReasonIssuance,'')   ReasonIssuance
,isnull(ValueChallan,'')     ValueChallan
,isnull(QuantityinChallan,'')QuantityinChallan
,isnull(VATChallan,'')       VATChallan
,isnull(SDChallan,'')        SDChallan
,isnull(ValueIncDecAdj,'')   ValueIncDecAdj
,isnull(QuantityIncDecAdj,'')QuantityIncDecAdj
,isnull(VATIncDecAdj,'')     VATIncDecAdj
,isnull(SDIncDecAdj,'')      SDIncDecAdj
FROM VAT9_1NBRApi_SF_adjustSet where 1=1 and PeriodID=@PeriodID

-------- Note 58 to 64--------2----

SELECT 
 isnull(MSGID,'')               MSGID
,isnull(FieldID,'')             FieldID
,isnull(TreasuryChallanToken,'')TreasuryChallanToken
,isnull(ChallanDate,'')         ChallanDate
,isnull(Amount,'')              Amount
,isnull(Notes,'')               Notes
,isnull(BankCode,'')            BankCode
,isnull(BranchCode,'')          BranchCode
FROM VAT9_1NBRApi_SF_challanSet where 1=1 and PeriodID=@PeriodID

-------- Note 27 and 32--------3----

SELECT 
 isnull(MSGID,'')        MSGID
,isnull(FieldID,'')      FieldID
,isnull(ChallanNumber,'')ChallanNumber
,isnull(Date,'')         Date
,isnull(Value,'')        Value
,isnull(VAT,'')          VAT
,isnull(SD,'')           SD
,isnull(PurposeNotes,'') PurposeNotes
FROM VAT9_1NBRApi_SF_otherSet where 1=1 and PeriodID=@PeriodID

-------- Note 24 and 29 --------4----

SELECT 
 isnull(MSGID,'')                          MSGID
,isnull(FieldID,'')                        FieldID
,isnull(BuyerSupplyerBIN,'')               BuyerSupplyerBIN
,isnull(BuyerSupplyerName,'')              BuyerSupplyerName
,isnull(BuyerSupplyerAddress,'')           BuyerSupplyerAddress
,isnull(Value,'')                          Value
,isnull(DeductedVAT,'')                    DeductedVAT
,isnull(InvoiceNoChallanBillNo,'')         InvoiceNoChallanBillNo
,isnull(InvoiceChallanBillDate,'')         InvoiceChallanBillDate
,isnull(VATDeductionatSource,'')           VATDeductionatSource
,isnull(VATDeductionatSourceCerDate,'')    VATDeductionatSourceCerDate
,isnull(TaxDepositedSerialBookTransfer,'') TaxDepositedSerialBookTransfer
,isnull(TaxDepositedDate,'')               TaxDepositedDate
,isnull(Notes,'')                          Notes
FROM VAT9_1NBRApi_SF_vdsSet where 1=1 and PeriodID=@PeriodID

-------- Note 1 and 22 --------5----

SELECT
 isnull(MSGID,'')                       MSGID
,isnull(FieldID,'')                     FieldID
,isnull(ValueBasevalueVAT,'')           ValueBasevalueVAT
,isnull(SD,'')                          SD
,isnull(VAT,'')                         VAT
,isnull(HSCommercialDescription,'')     HSCommercialDescription
,isnull(Notes,'')                       Notes
,isnull(Quantity,'')                    Quantity
,isnull(ActualSalesPurchasesValue,'')   ActualSalesPurchasesValue
,isnull(UnitMeasure,'')                 UnitMeasure
,isnull(ItemID,'')                      ItemID
,isnull(Category,'')                    Category
,isnull(DataSource,'')                  DataSource
,isnull(BoeNo,'')                       BoENumber
,isnull(BoEDate,'')                     BoEDate
,isnull(BoEOficeCode,'')                BoEOficeCode
,isnull(BoEItemNo,'')                   BoEItemNo
,isnull(CPCCode,'')                     CPCCode
,isnull(AssessableValue,'')             AssessableValue
,isnull(AT,'')                          AT
,isnull(InvoiceNo,'')                   InvoiceNo
,isnull(InvoiceDate,'')                 InvoiceDate
FROM VAT9_1NBRApi_SF_goservSet where 1=1 and PeriodID=@PeriodID
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", PeriodId);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(ds);

                #region Commit

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                #endregion

            }
            #endregion

            #region Catch & Finally

            catch (Exception ex)
            {
                FileLogger.Log("_9_1NBR_API_DAL", "GetNBR_API_SubfromData", ex.ToString() + "\n" + sqlText);

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

            return ds;
        }

        #endregion

        #region Master API

        #region PeriodKey API

        public string GatNewPeriodKey(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits Period key

                string PeriodKey = GatPeriodKeyValue(msgVM, connection, transaction);

                #endregion

                if (string.IsNullOrWhiteSpace(PeriodKey))
                {
                    List<NBRApiResult> APIresults = GatPeriodKey(msgVM.BIN);

                    if (APIresults != null && APIresults.Count > 0)
                    {
                        foreach (var item in APIresults)
                        {
                            MessageIdVM mVM = new MessageIdVM();

                            string pKey = item.PeriodKey.ToString();

                            mVM.PeriodId = msgVM.PeriodId;
                            mVM.PeriodKey = pKey;

                            PeriodKey = GatPeriodKeyValue(mVM, connection, transaction);

                            if (string.IsNullOrWhiteSpace(PeriodKey))
                            {
                                NBRApiResult vm = new NBRApiResult();
                                vm.PeriodKey = pKey;
                                vm.Txt50 = item.Txt50;
                                vm.BIN = msgVM.BIN;
                                vm.DueDate = item.DueDate;
                                vm.Txt50 = vm.Txt50.Replace(", ", "-");

                                string[] cValues = { vm.Txt50 };
                                string[] cFields = { "PeriodName" };
                                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, connection, transaction).FirstOrDefault();

                                vm.PeriodId = varFiscalYearVM.PeriodID;

                                SavePeriodKey(vm, connection, transaction);

                            }

                        }
                    }
                }

                PeriodKey = GatPeriodKeyValue(msgVM, connection, transaction);

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return PeriodKey;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatNewPeriodKey", ex.ToString());
                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string GatPeriodKeyValue(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string PeriodKey = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits Period key

                SqlText = @"select PeriodKey from VAT9_1NBRApi_Master_periodKey where PeriodID = @PeriodID";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", msgVM.PeriodId);
                PeriodKey = (string)cmd.ExecuteScalar();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return PeriodKey;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatPeriodKeyValue", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string SavePeriodKey(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Insert Data

                SqlText = @"
insert into VAT9_1NBRApi_Master_periodKey (BIN,PeriodKey,Txt50,DueDate,PeriodID)
values(@BIN,@PeriodKey,@Txt50,@DueDate,@PeriodID)
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BIN", VM.BIN);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@Txt50", VM.Txt50);
                cmd.Parameters.AddWithValue("@DueDate", VM.DueDate);
                cmd.Parameters.AddWithValue("@PeriodID", VM.PeriodId);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SavePeriodKey", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private List<NBRApiResult> GatPeriodKey(string CompanyBIN)//, string PeriodName
        {
            string PeriodKey = "";

            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                //////PeriodKey = "0723";

                Root_NBRAPI apiData = GetPeriodKeyApiData(CompanyBIN);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;

                    ////foreach (var items in apiData.d.results)
                    ////{
                    ////    string BIN = items.BIN;
                    ////    string pName = items.Txt50;

                    ////    if (PeriodName == pName)
                    ////    {
                    ////        PeriodKey = items.PeriodKey;
                    ////    }

                    ////}

                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetPeriodKeyApiData(string CompanyBIN)
        {

            string url;

            url = PeriodKeyUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";

            //apiKeyValue = "$filter=BIN eq " + CompanyBIN + "&$format=json";

            query["$filter"] = "BIN eq '" + CompanyBIN + "'";
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_BoEData API

        public NBRApiResult GatBoEData(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";

            NBRApiResult APIResult = new NBRApiResult();
            List<NBRApiResult> vms = new List<NBRApiResult>();


            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits Period key

                int check = CheckBoEData(VM, connection, transaction);

                #endregion

                if (check != 0)
                {
                    List<NBRApiResult> APIresults = get_BoEData(VM);

                    if (APIresults != null && APIresults.Count > 0)
                    {
                        foreach (var item in APIresults)
                        {

                            NBRAPI_paramVM pvm = new NBRAPI_paramVM();

                            pvm.BoENumber = item.BoENumber;
                            pvm.PeriodKey = item.PeriodKey;
                            pvm.Serial = item.Serial;

                            check = CheckBoEData(pvm, connection, transaction);

                            if (check != 0)
                            {
                                NBRApiResult vm = new NBRApiResult();

                                vm.BIN = item.BIN;
                                vm.PeriodKey = item.PeriodKey;
                                vm.Serial = item.Serial;
                                vm.BoENumber = item.BoENumber;
                                vm.BoEDate = item.BoEDate;
                                vm.BoEOffCode = item.BoEOffCode;
                                vm.BoEItmNo = item.BoEItmNo;
                                vm.CPCCode = item.CPCCode;
                                vm.Description = item.Description;
                                vm.GoodService = item.GoodService;
                                vm.Name = item.Name;
                                vm.AssessValue = item.AssessValue;
                                vm.VAT = item.VAT;
                                vm.SD = item.SD;
                                vm.AT = item.AT;

                                SaveBoEData(vm, connection, transaction);

                            }

                        }
                    }
                }

                #region get data

                APIResult = SelectBoEData(VM, connection, transaction).FirstOrDefault();

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

                return APIResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatBoEData", ex.ToString());
                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public int CheckBoEData(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            int count;

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits Period key

                SqlText = @"select count(BoENumber) from VAT9_1NBRApi_Master_BoEData where BoENumber =@BoENumber and PeriodKey=@PeriodKey and Serial=@Serial";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BoENumber", VM.BoENumber);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@Serial", VM.Serial);
                count = (int)cmd.ExecuteScalar();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return count;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "CheckBoEData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<NBRApiResult> SelectBoEData(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            List<NBRApiResult> VMs = new List<NBRApiResult>();

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get data

                SqlText = @"
SELECT 
 BIN
,PeriodKey
,Serial
,BoENumber
,BoEDate
,BoEOffCode
,BoEItmNo
,CPCCode
,Description
,GoodService
,Name
,AssessValue
,VAT
,SD
,AT
FROM VAT9_1NBRApi_Master_BoEData 
where 1=1

";
                if (!string.IsNullOrWhiteSpace(VM.BoENumber))
                {
                    SqlText += @" and BoENumber=@BoENumber";
                }
                if (!string.IsNullOrWhiteSpace(VM.PeriodKey))
                {
                    SqlText += @" and PeriodKey=@PeriodKey";
                }
                if (!string.IsNullOrWhiteSpace(VM.Serial))
                {
                    SqlText += @" and Serial=@Serial";
                }

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                if (!string.IsNullOrWhiteSpace(VM.BoENumber))
                {
                    cmd.Parameters.AddWithValue("@BoENumber", VM.BoENumber);
                }
                if (!string.IsNullOrWhiteSpace(VM.PeriodKey))
                {
                    cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                }
                if (!string.IsNullOrWhiteSpace(VM.Serial))
                {
                    cmd.Parameters.AddWithValue("@Serial", VM.Serial);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                #endregion

                if (table != null && table.Rows.Count > 0)
                {
                    VMs = table.ToList<NBRApiResult>();
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return VMs;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SelectBoEData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string SaveBoEData(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Insert Data

                SqlText = @"
insert into VAT9_1NBRApi_Master_BoEData (
 BIN
,PeriodKey
,Serial
,BoENumber
,BoEDate
,BoEOffCode
,BoEItmNo
,CPCCode
,Description
,GoodService
,Name
,AssessValue
,VAT
,SD
,AT
)
values(
 @BIN
,@PeriodKey
,@Serial
,@BoENumber
,@BoEDate
,@BoEOffCode
,@BoEItmNo
,@CPCCode
,@Description
,@GoodService
,@Name
,@AssessValue
,@VAT
,@SD
,@AT
)
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BIN", VM.BIN);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@Serial", VM.Serial);
                cmd.Parameters.AddWithValue("@BoENumber", VM.BoENumber);
                cmd.Parameters.AddWithValue("@BoEDate", VM.BoEDate);
                cmd.Parameters.AddWithValue("@BoEOffCode", VM.BoEOffCode);
                cmd.Parameters.AddWithValue("@BoEItmNo", VM.BoEItmNo);
                cmd.Parameters.AddWithValue("@CPCCode", VM.CPCCode);
                cmd.Parameters.AddWithValue("@Description", VM.Description);
                cmd.Parameters.AddWithValue("@GoodService", VM.GoodService);
                cmd.Parameters.AddWithValue("@Name", VM.Name);
                cmd.Parameters.AddWithValue("@AssessValue", VM.AssessValue);
                cmd.Parameters.AddWithValue("@VAT", VM.VAT);
                cmd.Parameters.AddWithValue("@SD", VM.SD);
                cmd.Parameters.AddWithValue("@AT", VM.AT);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SaveBoEData", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private List<NBRApiResult> get_BoEData(NBRAPI_paramVM vm)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetBoEApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetBoEApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = BoEDataUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.BoENumber))
            {
                filterText += " and BoENumber eq '" + vm.BoENumber + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Serial))
            {
                filterText += " and Serial eq '" + vm.Serial + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString());

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region TypeReturn API

        private string get_TypeReturn(string CompanyBIN, string PeriodName)
        {
            string PeriodKey = "";

            try
            {

                Root_NBRAPI apiData = GetTypeReturnApiData(CompanyBIN);

                //if (apiData != null)
                //{
                //    foreach (var items in apiData.d.results)
                //    {
                //        string BIN = items.BIN;
                //        string pName = items.Txt50;

                //        if (PeriodName == pName)
                //        {
                //            PeriodKey = items.Persl;
                //        }

                //    }

                //}

                return PeriodKey;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetTypeReturnApiData(string CompanyBIN)
        {

            //#region StaticCode

            //List<StaticCodeVM> codeList = new List<StaticCodeVM>();
            //CommonDAL commonDal = new CommonDAL();

            //string[] conditionFields;
            //string[] conditionValues;
            //conditionFields = new string[] {"Field_Name", "File_Name", "Class_Name" };
            //conditionValues = new string[] { "TypeReturnUrl","VATServer.Library.Integration", "_9_1NBR_API_DAL" };

            //codeList = commonDal.GetStaticCodes(conditionFields, conditionValues, connVM, null, null);

            //#endregion

            string url;

            url = TypeReturnUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";

            query["$filter"] = "BIN eq '" + CompanyBIN + "'";
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_anyActivity API

        private string get_anyActivity(string CompanyBIN, string PeriodName)
        {
            string PeriodKey = "";

            try
            {

                Root_NBRAPI apiData = GetAnyActivityApiData(CompanyBIN);

                //if (apiData != null)
                //{
                //    foreach (var items in apiData.d.results)
                //    {
                //        string BIN = items.BIN;
                //        string pName = items.Txt50;

                //        if (PeriodName == pName)
                //        {
                //            PeriodKey = items.Persl;
                //        }

                //    }

                //}

                return PeriodKey;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetAnyActivityApiData(string CompanyBIN)
        {

            string url;

            url = AnyActivityUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";

            query["$filter"] = "BIN eq '" + CompanyBIN + "'";
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_refundType API

        private string get_refundType(string CompanyBIN, string PeriodName)
        {
            string PeriodKey = "";

            try
            {

                Root_NBRAPI apiData = GetRefundTypeApiData(CompanyBIN);

                //if (apiData != null)
                //{
                //    foreach (var items in apiData.d.results)
                //    {
                //        string BIN = items.BIN;
                //        string pName = items.Txt50;

                //        if (PeriodName == pName)
                //        {
                //            PeriodKey = items.Persl;
                //        }

                //    }

                //}

                return PeriodKey;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetRefundTypeApiData(string CompanyBIN)
        {

            string url;

            url = refundTypeUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";

            query["$filter"] = "BIN eq '" + CompanyBIN + "'";
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region goods Service API

        public NBRApiResult GatGoodsServiceData(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";

            NBRApiResult APIResult = new NBRApiResult();
            List<NBRApiResult> vms = new List<NBRApiResult>();

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Check exits

                string ItemID = CheckGoodsServiceData(VM, false, connection, transaction);

                #endregion

                if (string.IsNullOrWhiteSpace(ItemID))
                {
                    List<NBRApiResult> APIresults = GetGoodsAPIData(VM);

                    if (APIresults != null && APIresults.Count > 0)
                    {
                        foreach (var item in APIresults)
                        {
                            string note = item.Note;

                            string[] substrings = note.Split(';');

                            foreach (string substring in substrings)
                            {
                                NBRAPI_paramVM pvm = new NBRAPI_paramVM();

                                pvm.GoodsServiceCode = item.GoodsServiceCode;
                                pvm.PeriodKey = item.PeriodKey;
                                pvm.Serial = substring;
                                pvm.ItemID = item.ItemID;

                                ItemID = CheckGoodsServiceData(pvm, true, connection, transaction);

                                if (string.IsNullOrWhiteSpace(ItemID))
                                {
                                    NBRApiResult vm = new NBRApiResult();

                                    vm.BIN = item.BIN;
                                    vm.PeriodKey = item.PeriodKey;
                                    vm.Type = item.Type;
                                    vm.GoodsServiceCode = item.GoodsServiceCode;
                                    vm.GoodsServiceName = item.GoodsServiceName;
                                    vm.SDRate = item.SDRate;
                                    vm.VATRate = item.VATRate;
                                    vm.SpecRate06 = item.SpecRate06;
                                    vm.ValidFrom = item.ValidFrom;
                                    vm.ValidTo = item.ValidTo;
                                    vm.ItemID = item.ItemID;
                                    vm.Note = item.Note;
                                    vm.ManualInput = item.SD;
                                    vm.CategoryID = item.CategoryID;
                                    vm.TotTate = item.TotTate;
                                    vm.NoteNo = substring;

                                    SaveGoodsServiceData(vm, connection, transaction);

                                }

                            }

                        }
                    }
                }

                #region get data

                APIResult = SelectGoodsServiceData(VM, true, connection, transaction).FirstOrDefault();

                #endregion

                #region Commit

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                #endregion

                return APIResult;
            }

            #region catch finally

            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatGoodsServiceData", ex.ToString());
                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            #endregion

        }

        public string CheckGoodsServiceData(NBRAPI_paramVM VM, bool isCheckforNew, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string ItemID;

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Check

                SqlText = @"select top 1 ItemID from VAT9_1NBRApi_Master_goodsService01_04 
where GoodsServiceCode =@GoodsServiceCode and PeriodKey=@PeriodKey and Serial=@Serial ";

                if (!isCheckforNew)
                {
                    SqlText += @" and VATRate=@VATRate and SDRate=@SDRate ";
                }
                if (!string.IsNullOrWhiteSpace(VM.ItemID))
                {
                    SqlText += @" and ItemID=@ItemID";
                }


                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@GoodsServiceCode", VM.GoodsServiceCode);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@Serial", VM.Serial);

                if (!isCheckforNew)
                {
                    cmd.Parameters.AddWithValue("@VATRate", VM.VATRate);
                    cmd.Parameters.AddWithValue("@SDRate", VM.SDRate);
                }
                if (!string.IsNullOrWhiteSpace(VM.ItemID))
                {
                    cmd.Parameters.AddWithValue("@ItemID", VM.ItemID);
                }

                ItemID = (string)cmd.ExecuteScalar();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return ItemID;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "CheckGoodsServiceData", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<NBRApiResult> SelectGoodsServiceData(NBRAPI_paramVM VM, bool IsSDVAT, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            List<NBRApiResult> VMs = new List<NBRApiResult>();

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get data

                SqlText = @"
SELECT 
 BIN
,PeriodKey
,CategoryID
,Type
,GoodsServiceCode
,GoodsServiceName
,SDRate
,VATRate
,StandardVatRate
,TotTate
,SpecRate06
,ValidFrom
,ValidTo
,ItemID
,Note
,ManualInput
,NoteNo
FROM VAT9_1NBRApi_Master_goodsService01_04
where 1=1 

";
                if (!string.IsNullOrWhiteSpace(VM.PeriodKey))
                {
                    SqlText += @" and PeriodKey=@PeriodKey";
                }
                ////if (!string.IsNullOrWhiteSpace(VM.CategoryID))
                ////{
                ////    SqlText += @" and CategoryID=@CategoryID";
                ////}
                if (!string.IsNullOrWhiteSpace(VM.GoodsServiceCode))
                {
                    SqlText += @" and GoodsServiceCode=@GoodsServiceCode";
                }
                if (!string.IsNullOrWhiteSpace(VM.Note))
                {
                    SqlText += @" and NoteNo=@NoteNo";
                }
                if (IsSDVAT)
                {
                    SqlText += @" and SDRate=@SDRate and VATRate=@VATRate ";
                }

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                if (!string.IsNullOrWhiteSpace(VM.PeriodKey))
                {
                    cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                }
                if (!string.IsNullOrWhiteSpace(VM.CategoryID))
                {
                    cmd.Parameters.AddWithValue("@CategoryID", VM.CategoryID);
                }
                if (!string.IsNullOrWhiteSpace(VM.GoodsServiceCode))
                {
                    cmd.Parameters.AddWithValue("@GoodsServiceCode", VM.GoodsServiceCode);
                }
                if (!string.IsNullOrWhiteSpace(VM.Note))
                {
                    cmd.Parameters.AddWithValue("@NoteNo", VM.Note);
                }
                if (IsSDVAT)
                {
                    cmd.Parameters.AddWithValue("@SDRate", VM.SDRate);
                    cmd.Parameters.AddWithValue("@VATRate", VM.VATRate);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                #endregion

                if (table != null && table.Rows.Count > 0)
                {
                    VMs = table.ToList<NBRApiResult>();
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return VMs;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SelectBoEData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string SaveGoodsServiceData(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Insert Data

                SqlText = @"
insert into VAT9_1NBRApi_Master_goodsService01_04 (
 BIN
,PeriodKey
,CategoryID
,Type
,GoodsServiceCode
,GoodsServiceName
,SDRate
,VATRate
,StandardVatRate
,TotTate
,SpecRate06
,ValidFrom
,ValidTo
,ItemID
,Note
,ManualInput
,NoteNo
)
values(
 @BIN
,@PeriodKey
,@CategoryID
,@Type
,@GoodsServiceCode
,@GoodsServiceName
,@SDRate
,@VATRate
,@StandardVatRate
,@TotTate
,@SpecRate06
,@ValidFrom
,@ValidTo
,@ItemID
,@Note
,@ManualInput
,@NoteNo
)
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BIN", VM.BIN);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@CategoryID", VM.CategoryID);
                cmd.Parameters.AddWithValue("@Type", VM.Type);
                cmd.Parameters.AddWithValue("@GoodsServiceCode", VM.GoodsServiceCode);
                cmd.Parameters.AddWithValue("@GoodsServiceName", VM.GoodsServiceName);
                cmd.Parameters.AddWithValue("@SDRate", VM.SDRate);
                cmd.Parameters.AddWithValue("@VATRate", VM.VATRate);
                cmd.Parameters.AddWithValue("@StandardVatRate", VM.StandardVatRate);
                cmd.Parameters.AddWithValue("@TotTate", VM.TotTate);
                cmd.Parameters.AddWithValue("@SpecRate06", VM.SpecRate06);
                cmd.Parameters.AddWithValue("@ValidFrom", VM.ValidFrom);
                cmd.Parameters.AddWithValue("@ValidTo", VM.ValidTo);
                cmd.Parameters.AddWithValue("@ItemID", VM.ItemID);
                cmd.Parameters.AddWithValue("@Note", VM.Note);
                cmd.Parameters.AddWithValue("@ManualInput", VM.ManualInput);
                cmd.Parameters.AddWithValue("@NoteNo", VM.NoteNo);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SaveGoodsServiceData", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private List<NBRApiResult> GetGoodsAPIData(NBRAPI_paramVM vm)
        {

            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {
                string FieldId = vm.Serial;

                if (FieldId == "SL01" || FieldId == "SL02" || FieldId == "SL03" || FieldId == "SL04" || FieldId == "SL05" || FieldId == "SL07"
                    || FieldId == "SL08" || FieldId == "SL10" || FieldId == "SL11" || FieldId == "SL12" || FieldId == "SL13" || FieldId == "SL16"
                    || FieldId == "SL17" || FieldId == "SL21" || FieldId == "SL22" || FieldId == "SL23")
                {

                    APIresults = get_goodsService(vm);

                }
                else if (FieldId == "SL06" || FieldId == "SL18")
                {

                    APIresults = get_goodsService_01(vm);

                }
                else if (FieldId == "SL14" || FieldId == "SL15")
                {
                    //get_goodsService_02
                    APIresults = get_goodsService_02(vm);

                }
                else if (FieldId == "SL20")
                {
                    //get_goodsService_03
                    APIresults = get_goodsService_03(vm);

                }
                else if (FieldId == "SL08")
                {
                    //get_goodsService_04
                    APIresults = get_goodsService_04(vm);

                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #region get_goodsService API

        private List<NBRApiResult> get_goodsService(NBRAPI_paramVM vm)
        {

            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetGoodsServiceApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetGoodsServiceApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = goodsServiceUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.GoodsServiceCode))
            {
                filterText += " and GoodsServiceCode eq '" + vm.GoodsServiceCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.ItemID))
            {
                filterText += " and ItemID eq '" + vm.ItemID + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Note))
            {
                filterText += " and Note eq '" + vm.Note + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_goodsService_01 API

        private List<NBRApiResult> get_goodsService_01(NBRAPI_paramVM vm)
        {

            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetGoodsService_01ApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetGoodsService_01ApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = goodsService_01Url;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.GoodsServiceCode))
            {
                filterText += " and GoodsServiceCode eq '" + vm.GoodsServiceCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.ItemID))
            {
                filterText += " and ItemID eq '" + vm.ItemID + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Note))
            {
                filterText += " and Note eq '" + vm.Note + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_goodsService_02 API

        private List<NBRApiResult> get_goodsService_02(NBRAPI_paramVM vm)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetGoodsService_02ApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetGoodsService_02ApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = goodsService_02Url;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.GoodsServiceCode))
            {
                filterText += " and GoodsServiceCode eq '" + vm.GoodsServiceCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.ItemID))
            {
                filterText += " and ItemID eq '" + vm.ItemID + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Note))
            {
                filterText += " and Note eq '" + vm.Note + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_goodsService_03 API

        private List<NBRApiResult> get_goodsService_03(NBRAPI_paramVM vm)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetGoodsService_03ApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetGoodsService_03ApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = goodsService_03Url;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.GoodsServiceCode))
            {
                filterText += " and GoodsServiceCode eq '" + vm.GoodsServiceCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.ItemID))
            {
                filterText += " and ItemID eq '" + vm.ItemID + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Note))
            {
                filterText += " and Note eq '" + vm.Note + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_goodsService_04 API

        private List<NBRApiResult> get_goodsService_04(NBRAPI_paramVM vm)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetGoodsService_04ApiData(vm);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetGoodsService_04ApiData(NBRAPI_paramVM vm)
        {

            string url;

            url = goodsService_04Url;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + vm.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(vm.CategoryID))
            {
                filterText += " and CategoryID eq '" + vm.CategoryID + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + vm.PeriodKey + "'";
            }
            if (!string.IsNullOrWhiteSpace(vm.Note))
            {
                filterText += " and Note eq '" + vm.Note + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #endregion

        #region get_categoryNote8 API

        public string GatNewCompanyCategory(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits

                string CategoryID = GatCompanyCategoryValue(VM, connection, transaction);

                #endregion

                if (string.IsNullOrWhiteSpace(CategoryID))
                {
                    List<NBRApiResult> APIresults = get_categoryNote8(VM);

                    if (APIresults != null && APIresults.Count > 0)
                    {
                        foreach (var item in APIresults)
                        {
                            #region Check exits

                            NBRAPI_paramVM ParamVM = new NBRAPI_paramVM();

                            string pKey = item.PeriodKey.ToString();

                            ParamVM.PeriodKey = pKey;
                            ParamVM.CategoryName = VM.CategoryName;

                            CategoryID = GatCompanyCategoryValue(ParamVM, connection, transaction);

                            #endregion

                            #region Save Data

                            if (string.IsNullOrWhiteSpace(CategoryID))
                            {
                                NBRApiResult vm = new NBRApiResult();
                                vm.PeriodKey = pKey;
                                vm.CategoryID = item.CategoryID;
                                vm.BIN = item.BIN;
                                vm.Category = item.Category;

                                SaveCompanyCategory(vm, connection, transaction);

                            }

                            #endregion

                        }
                    }
                }

                CategoryID = GatCompanyCategoryValue(VM, connection, transaction);

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return CategoryID;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatNewCompanyCategory", ex.ToString());
                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string GatCompanyCategoryValue(NBRAPI_paramVM VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string CategoryID = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get Exits

                SqlText = @"select CategoryID from VAT9_1NBRApi_Master_categoryNote where Category = @Category and PeriodKey=@PeriodKey";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@Category", VM.CategoryName);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                CategoryID = (string)cmd.ExecuteScalar();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return CategoryID;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "GatCompanyCategoryValue", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public string SaveCompanyCategory(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Insert Data

                SqlText = @"
insert into VAT9_1NBRApi_Master_categoryNote (BIN,PeriodKey,CategoryID,Category)
values(@BIN,@PeriodKey,@CategoryID,@Category)
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BIN", VM.BIN);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@CategoryID", VM.CategoryID);
                cmd.Parameters.AddWithValue("@Category", VM.Category);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SavePeriodKey", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private List<NBRApiResult> get_categoryNote8(NBRAPI_paramVM VM)
        {

            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetCategoryNote8ApiData(VM);

                if (apiData != null)
                {
                    APIresults = apiData.d.results;
                }

                return APIresults;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        private Root_NBRAPI GetCategoryNote8ApiData(NBRAPI_paramVM VM)
        {

            string url;

            url = CategoryNote8Url;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string apiKeyValue = "";
            string filterText = "";

            filterText = "BIN eq '" + VM.CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(VM.PeriodKey))
            {
                filterText += " and PeriodKey eq '" + VM.PeriodKey + "'";
            }

            filterText += " and CategoryID eq ''";

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString(), apiKeyValue);

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_bank API

        public List<NBRApiResult> SelectBankData(string BankName, string BankBranch, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            List<NBRApiResult> VMs = new List<NBRApiResult>();

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection();
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Get data

                SqlText = @"
SELECT 
 BIN
,BanCD
,RoutingNumber
,Bankn
,Brannm
,BanCDDatefrom
,BanCDDateto
,RoutingNumberDatefrom
,RoutingNumberDateto
FROM VAT9_1NBRApi_Master_Bank 
where 1=1

";
                if (!string.IsNullOrWhiteSpace(BankName))
                {
                    SqlText += @" and Bankn=@Bankn";
                }
                if (!string.IsNullOrWhiteSpace(BankBranch))
                {
                    SqlText += @" and Brannm=@Brannm";
                }

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                if (!string.IsNullOrWhiteSpace(BankName))
                {
                    cmd.Parameters.AddWithValue("@Bankn", BankName);
                }
                if (!string.IsNullOrWhiteSpace(BankBranch))
                {
                    cmd.Parameters.AddWithValue("@Brannm", BankBranch);
                }

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(table);

                #endregion

                if (table != null && table.Rows.Count > 0)
                {
                    VMs = table.ToList<NBRApiResult>();
                }

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return VMs;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("_9_1NBR_API_DAL", "SelectBankData", e.ToString());
                throw;
            }
            finally
            {
                if (connection != null && Vconnection == null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        private NBRApiResult get_bank(string CompanyBIN, string BankName, string BankBranch)
        {
            NBRApiResult bank = new NBRApiResult();

            try
            {

                Root_NBRAPI apiData = GetBankApiData(CompanyBIN);

                if (apiData != null)
                {
                    if (apiData.d.results.Count > 0)
                    {
                        bank = apiData.d.results.SingleOrDefault(x => x.Bankn == BankName && x.Brannm == BankBranch);
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }

            return bank;

        }

        private Root_NBRAPI GetBankApiData(string CompanyBIN)
        {

            string url;

            url = BankUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string filterText = "";

            filterText = "BIN eq '" + CompanyBIN + "'";

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString());

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_cpccode API

        private string get_cpccode(string CompanyBIN, string PeriodName, string CpcCode = "", string Serial = "")
        {
            string PeriodKey = "";

            try
            {

                Root_NBRAPI apiData = GetCpcCodeApiData(CompanyBIN, CpcCode, Serial);

                //if (apiData != null)
                //{
                //    foreach (var items in apiData.d.results)
                //    {
                //        string BIN = items.BIN;
                //        string pName = items.Txt50;

                //        if (PeriodName == pName)
                //        {
                //            PeriodKey = items.Persl;
                //        }

                //    }

                //}

                return PeriodKey;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetCpcCodeApiData(string CompanyBIN, string CpcCode = "", string Serial = "")
        {

            string url;

            url = cpcCodeUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string filterText = "";

            filterText = "BIN eq '" + CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(CpcCode))
            {
                filterText += " and CpcCode eq '" + CpcCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(Serial))
            {
                filterText += " and Serial eq '" + Serial + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString());

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #region get_unit API

        private string get_unit(string CompanyBIN, string PeriodName, string CpcCode = "", string Serial = "")
        {
            string PeriodKey = "";

            try
            {

                Root_NBRAPI apiData = GetUnitApiData(CompanyBIN, CpcCode, Serial);

                //if (apiData != null)
                //{
                //    foreach (var items in apiData.d.results)
                //    {
                //        string BIN = items.BIN;
                //        string pName = items.Txt50;

                //        if (PeriodName == pName)
                //        {
                //            PeriodKey = items.Persl;
                //        }

                //    }

                //}

                return PeriodKey;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private Root_NBRAPI GetUnitApiData(string CompanyBIN, string CpcCode = "", string Serial = "")
        {

            string url;

            url = unitUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string filterText = "";

            filterText = "BIN eq '" + CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(CpcCode))
            {
                filterText += " and CpcCode eq '" + CpcCode + "'";
            }
            if (!string.IsNullOrWhiteSpace(Serial))
            {
                filterText += " and Serial eq '" + Serial + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString());

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        #endregion

        #endregion

        private string GetData(string url, string apiKeyValue = "")
        {
            try
            {
                WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                ////string apiKey = "";

                ////apiKey = apiKeyValue;

                ////byte[] byteArray = Encoding.UTF8.GetBytes(apiKey);
                ////request.ContentLength = byteArray.Length;

                ////request.ContentType = "application/json";

                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                ////Stream datastream = request.GetRequestStream();
                ////datastream.Write(byteArray, 0, byteArray.Length);
                ////datastream.Close();

                WebResponse response = request.GetResponse();
                Stream datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);
                string responseMessage = reader.ReadToEnd();

                reader.Close();

                return responseMessage;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private NetworkCredential GetCredentials(SysDBInfoVMTemp connVM = null)
        {

            return new NetworkCredential("IVAS_WS_RT01", "123456a@");


            CommonDAL commonDal = new CommonDAL();

            ////string code = commonDal.settingValue("CompanyCode", "Code");

            ////if (code.ToLower() == "purofood")
            ////{
            ////    ////return new NetworkCredential("rest", "123456");
            ////    ////return new NetworkCredential("vatuser", "123456");

            ////}
            ////else
            ////{
            ////    //////// read from config file
            ////    ////return new NetworkCredential("rest", "123456");
            ////    ////return new NetworkCredential("vatuser", "123456");
            ////}


        }



    }
}
