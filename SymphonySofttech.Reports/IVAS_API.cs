using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
//using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATViewModel.Integration.JsonModels;
using VATViewModel.Integration.NBRAPI;


namespace SymphonySofttech.Reports
{
    public class IVAS_API
    {

        private DBSQLConnection _dbsqlConnection = new DBSQLConnection();

        _9_1NBR_API_DAL _NBR_API_DAL = new _9_1NBR_API_DAL();
        ReportDocument reportDocument = new ReportDocument();

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

        private string CheckAPIUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_MSG_headerSet";
        private string SubmitAPIUrl = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_submitSet";


        #endregion

        #region NBR API Json Methods

        public ResultVM Get9_1NBR_Json(NBR_APIVM nbrVm, SysDBInfoVMTemp connVM = null)
        {
            FileLogger.Log("IVAS_API", "Get9_1NBR_Json", "Start Make Json file : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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
                List<Return_9_1_SF_att_fileSet> _fileSetVMs = new List<Return_9_1_SF_att_fileSet>();

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

                DateTime currentDate = nbrVm.Period; // need to be dynamic

                string FBTyp = "R913";
                PeriodName = currentDate.ToString("MMMM-yyyy");
                PeriodId = currentDate.ToString("MMyyyy");

                #endregion

                #region Check submited period

                string IsSubmited = CheckDataIsSubmited(PeriodId, null, null, connVM);

                if (IsSubmited == "Y")
                {
                    throw new Exception(PeriodName + " This Period VAT Return already submitted");
                }

                #endregion

                try
                {
                    #region Get Period Key

                    msgVM = new MessageIdVM();
                    msgVM.BIN = companyBIN;
                    msgVM.PeriodId = PeriodId;

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
                    SetNote01_22(subformVm, MSGID, companyBIN, companyProfileVm, connVM);
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
                    SetNote26_31(subformVm, MSGID, connVM);
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
                    SetNote27_32(subformVm, MSGID, connVM);
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
                    SetNote24_29(subformVm, MSGID, connVM);
                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Note24_29", Message = e.Message });
                }

                #endregion

                #region Attachments

                try
                {
                    subformVm.NoteNos = new[] { 24, 26, 27, 29, 31, 32 };

                    _fileSetVMs = SetAttachments(subformVm, MSGID, connVM);

                }
                catch (Exception e)
                {
                    errorMessages.Add(new ErrorMessage() { ColumnName = "Attachments", Message = e.Message });
                }

                #endregion

                #endregion

                vm.ErrorList = errorMessages;

                if (vm.ErrorList.Count == 0)
                {
                    _9_1NBR_API_DAL _nbrApiDAL = new _9_1NBR_API_DAL();

                    string FinalJson = _nbrApiDAL.makeJson(PeriodId, _MainFull, _fileSetVMs);
                    vm.Json = FinalJson;

                    FileLogger.Log("IVAS_API", "Get9_1NBR_Json", "End Make Json file : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    OrdinaryVATDesktop.SaveIVASJsonTextFile(vm.Json, "NBR_FinalJson(" + PeriodId + ")", ".json");

                    try
                    {

                        #region Check and submit data

                        FileLogger.Log("IVAS_API", "Get9_1NBR_Json", "Start Send Json file to Check API : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        string APIUrl = CheckAPIUrl;

                        PostData(APIUrl, FinalJson);

                        FileLogger.Log("IVAS_API", "Get9_1NBR_Json", "End Send Json file to Check API : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        ////CallPostData(APIUrl, FinalJson);

                        #endregion

                        ////NBRApiResult sVm = getSubmitData(companyBIN, periodKey).FirstOrDefault();

                        ////MessageIdVM APIVM = new MessageIdVM();

                        ////APIVM.PeriodKey = sVm.PeriodKey;
                        ////APIVM.SubmitDate = sVm.SubmitDate;
                        ////APIVM.SubmitTime = sVm.SubmitTime;
                        ////APIVM.SubmissionId = sVm.SubmissionID;

                        ////ResultVM rVM = UpdateAPIHeader(APIVM);

                        vm.Status = "success";

                    }
                    catch (Exception ex)
                    {
                        FileLogger.Log("IVAS_API", "Get9_1NBR_Json", "Exception from Check API : " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                        if (ex.Message.Contains("\"error\":"))
                        {
                            Root_NBRAPI_Error ApiErrorData = JsonConvert.DeserializeObject<Root_NBRAPI_Error>(ex.Message);

                            foreach (var items in ApiErrorData.error.innererror.errordetails)
                            {
                                errorMessages.Add(new ErrorMessage() { ColumnName = "Send API", Message = items.message });
                            }
                        }
                        else
                        {
                            errorMessages.Add(new ErrorMessage() { ColumnName = "Send API", Message = ex.Message });
                        }

                    }

                }
                else
                {
                    MessageIdVM messageIdVm = new MessageIdVM()
                    {
                        PeriodId = PeriodId,
                    };

                    ReUpdateMaxMsgId(messageIdVm);

                }
            }
            catch (Exception e)
            {
                vm.Message = e.Message;
                vm.Status = "fail";

                FileLogger.Log("IVAS_API", "Get9_1NBR_Json", e.ToString());

                errorMessages.Add(new ErrorMessage() { ColumnName = "Send To IVAS", Message = e.Message });

            }

            vm.ErrorList = errorMessages;

            return vm;
        }

        public ResultVM VAT9_1Submit(NBR_APIVM nbrVm, SysDBInfoVMTemp connVM = null)
        {
            ResultVM vm = new ResultVM();
            List<ErrorMessage> errorMessages = new List<ErrorMessage>();
            _9_1_VATReturnDAL _VATReturnDal = new _9_1_VATReturnDAL();
            MessageIdVM msgVM = new MessageIdVM();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();

            try
            {

                string companyBIN = "";
                string PeriodName = "";
                string PeriodId = "";
                string periodKey = "";
                string MSGID = "";

                #region Get Company info

                CompanyProfileVM companyProfileVm = companyprofile.SelectAllList().FirstOrDefault();

                if (companyProfileVm == null) throw new ArgumentNullException("companyProfileVm");

                companyBIN = companyProfileVm.VatRegistrationNo;

                #endregion

                DateTime currentDate = nbrVm.Period; // need to be dynamic

                PeriodName = currentDate.ToString("MMMM-yyyy");
                PeriodId = currentDate.ToString("MMyyyy");

                #region Get Period Key

                msgVM = new MessageIdVM();
                msgVM.BIN = companyBIN;
                msgVM.PeriodId = PeriodId;

                string APIperiodName = currentDate.ToString("MMMM, yyyy");

                periodKey = GatPeriodKeyValue(msgVM);

                if (string.IsNullOrWhiteSpace(periodKey))
                {
                    throw new Exception(PeriodName + " is not available for submit VAT Return");
                }

                #endregion

                NBRApiResult sVm = getSubmitData(companyBIN, periodKey).FirstOrDefault();

                MessageIdVM APIVM = new MessageIdVM();

                if (sVm != null)
                {
                    APIVM.PeriodKey = sVm.PeriodKey;
                    APIVM.SubmitDate = sVm.SubmitDate;
                    APIVM.SubmitTime = sVm.SubmitTime;
                    APIVM.SubmissionId = sVm.SubmissionID;

                    ResultVM rVM = UpdateAPIHeader(APIVM);

                    vm.Status = "success";
                    vm.SubmissionId = sVm.SubmissionID;

                }
                else
                {
                    vm.Status = "fail";

                    errorMessages.Add(new ErrorMessage() { ColumnName = "Submit API", Message = "VAT return(9.1) data not send." });

                }


            }
            catch (Exception e)
            {
                vm.Message = e.Message;
                vm.Status = "fail";

                FileLogger.Log("IVAS_API", "VAT9_1Submit", e.ToString());

                errorMessages.Add(new ErrorMessage() { ColumnName = "Submit API", Message = e.Message });

            }

            vm.ErrorList = errorMessages;

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

                string fullId = transactionCode + currentDate + currentId.PadLeft(8, '0');

                MessageIdVM messageIdVm = new MessageIdVM()
                {
                    MessageId = currentId,
                    PeriodId = nbrApivm.Period.ToString("MMyyyy"),
                    FullId = fullId
                };

                UpdateMaxMsgId(messageIdVm);

                return fullId;
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

                SqlText = @"select (max(APIMsgId)+1)MessageId from FiscalYear";

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

                FileLogger.Log("IVAS_API", "GetMaxMsgId", e.ToString());
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
update FiscalYear set APIMsgId = @maxId, APIMsgFullId = @MSGID where PeriodID = @periodId
update VAT9_1NBRApiHeader set APIMsgId = @maxId, MSGID = @MSGID where PeriodID = @periodId
";

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

                FileLogger.Log("IVAS_API", "GetLastMsgId", e.ToString());
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

        public ResultVM ReUpdateMaxMsgId(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
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

                int msgId = Convert.ToInt32(msgVM.MessageId);
                msgId = msgId - 1;

                SqlText = @"
update FiscalYear set APIMsgId = APIMsgId-1, APIMsgFullId='' where PeriodID = @periodId
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);

                cmd.Parameters.AddWithValue("@periodId", msgVM.PeriodId);

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

                FileLogger.Log("IVAS_API", "GetLastMsgId", e.ToString());
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
insert into VAT9_1NBRApiHeader (FBTyp,BIN,PeriodKey,Depositor,PeriodID,IsSubmit)
values(@FBTyp,@BIN,@PeriodKey,@Depositor,@PeriodID,'N')
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

                FileLogger.Log("IVAS_API", "insertAPIHeader", e.ToString());
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

        public ResultVM UpdateAPIHeader(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            ResultVM rVM = new ResultVM();
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

                #region Insert API Header

                SqlText = @"
update VAT9_1NBRApiHeader set 
 SubmitDate=@SubmitDate
,SubmitTime=@SubmitTime
,SubmissionId=@SubmissionId
,IsSubmit='Y'
where PeriodKey=@PeriodKey

";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@SubmitDate", msgVM.SubmitDate);
                cmd.Parameters.AddWithValue("@SubmitTime", msgVM.SubmitTime);
                cmd.Parameters.AddWithValue("@SubmissionId", msgVM.SubmissionId);
                cmd.Parameters.AddWithValue("@PeriodKey", msgVM.PeriodKey);

                cmd.ExecuteNonQuery();

                #endregion

                rVM.Status = "Success";
                rVM.Message = "Data send successfully";

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return rVM;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateAPIHeader", ex.ToString());
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

                _Return_9_1_MainFull.S11RefundVAT = "";
                _Return_9_1_MainFull.S11RefundSD = "";

                if (value0 == "1")
                {
                    _Return_9_1_MainFull.S11RefundVAT = value67;
                    _Return_9_1_MainFull.S11RefundSD = value68;
                }

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
                FileLogger.Log("IVAS_API", "SetMainTag25_68", ex.ToString());
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

            value = DecimalCheck(value);

            ////if (OrdinaryVATDesktop.IsNumeric(value))
            ////{
            ////    value = Convert.ToDecimal(value).ToString("0.##");
            ////}

            return value;
        }

        public string DecimalCheck(string dValue)
        {
            string value = "0";
            if (string.IsNullOrEmpty(dValue))
                return value;

            value = dValue;

            if (OrdinaryVATDesktop.IsNumeric(value))
            {
                value = Convert.ToDecimal(value).ToString("0.##");
            }

            return value;
        }

        #region Set Note 26_31

        private void SetNote26_31(VATReturnSubFormVM subformVm, string MSGID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                SaveDataNBRApiNote26_31(subformVm.PeriodId, MSGID, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string SaveDataNBRApiNote26_31(string PeriodID, string MSGID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"

delete VAT9_1NBRApi_SF_adjustSet where PeriodID=@PeriodID

insert into VAT9_1NBRApi_SF_adjustSet( 
MSGID,FieldID,NoteNo,IssueDat,VATChallanNo,VATChallanDate,ReasonIssuance
,ValueChallan,QuantityinChallan,VATChallan,SDChallan,ValueIncDecAdj,QuantityIncDecAdj,VATIncDecAdj
,SDIncDecAdj,PeriodID)

select @MSGID,'SL126','Note '+CONVERT(VARCHAR(50), NoteNo),FORMAT(IssuedDate, 'yyyyMMdd'),REPLACE(REPLACE(REPLACE(TaxChallanNo, '/', ''), '-', ''), 'INV', '')
,FORMAT(TaxChallanDate, 'yyyyMMdd'),ReasonforIssuance
,CAST(ValueinChallan AS DECIMAL(15, 2))ValueinChallan
,CAST(QuantityinChallan AS DECIMAL(15, 2))QuantityinChallan
,CAST(VATinChallan AS DECIMAL(15, 2))VATinChallan
,CAST(SDinChallan AS DECIMAL(15, 2))SDinChallan
,CAST(ValueofIncreasingAdjustment AS DECIMAL(15, 2))ValueofIncreasingAdjustment
,CAST(QuantityofIncreasingAdjustment AS DECIMAL(15, 2))QuantityofIncreasingAdjustment
,CAST(VATofIncreasingAdjustment AS DECIMAL(15, 2))VATofIncreasingAdjustment
,CAST(SDofIncreasingAdjustment AS DECIMAL(15, 2))SDofIncreasingAdjustment
,PeriodID
from VAT9_1SubFormF
where NoteNo=26
and PeriodID=@PeriodID



insert into VAT9_1NBRApi_SF_adjustSet( 
MSGID,FieldID,NoteNo,IssueDat,VATChallanNo,VATChallanDate,ReasonIssuance
,ValueChallan,QuantityinChallan,VATChallan,SDChallan,ValueIncDecAdj,QuantityIncDecAdj,VATIncDecAdj
,SDIncDecAdj,PeriodID)

select @MSGID,'SL131','Note '+CONVERT(VARCHAR(50), NoteNo),FORMAT(IssuedDate, 'yyyyMMdd'),REPLACE(REPLACE(REPLACE(TaxChallanNo, '/', ''), '-', ''), 'INV', ''),
FORMAT(TaxChallanDate, 'yyyyMMdd'),ReasonforIssuance,CAST(ValueinChallan AS DECIMAL(18, 2)) ValueinChallan
,CAST(QuantityinChallan AS DECIMAL(15, 2))QuantityinChallan
,CAST(VATinChallan AS DECIMAL(15, 2))VATinChallan
,CAST(SDinChallan AS DECIMAL(15, 2))SDinChallan
,CAST(ValueofDecreasingAdjustment AS DECIMAL(15, 2))ValueofDecreasingAdjustment
,CAST(QuantityofDecreasingAdjustment AS DECIMAL(15, 2))QuantityofDecreasingAdjustment
,CAST(VATofDecreasingAdjustment AS DECIMAL(15, 2))VATofDecreasingAdjustment
,CAST(SDofDecreasingAdjustment AS DECIMAL(15, 2))SDofDecreasingAdjustment
,PeriodID
from VAT9_1SubFormJ
where NoteNo=31
and PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                cmd.Parameters.AddWithValue("@MSGID", MSGID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "SaveDataNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        #region Old Method

        private void xxSetNote26_31(VATReturnSubFormVM subformVm, string MSGID)
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

                #region Update Invoice

                UpdateAdjustSetInvoiceNo(FieldId, subformVm.PeriodId);

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
                    transaction = connection.BeginTransaction("GetSubNoteData");
                }

                #endregion

                VATReturnSubFormVM sVM = new VATReturnSubFormVM();
                sVM.PeriodName = subformVm.PeriodName;
                sVM.IsSummary = true;
                sVM.NoteNo = subformVm.NoteNo;
                sVM.PeriodId = subformVm.PeriodId;
                sVM.post1 = subformVm.post1;
                sVM.post2 = subformVm.post2;

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

                FileLogger.Log("IVAS_API", "GetSubNoteData", e.ToString());
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

                FileLogger.Log("IVAS_API", "DeleteSubFromData", e.ToString());
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

            if (table.Columns.Contains("TaxChallanNo"))
            {
                table.Columns["TaxChallanNo"].ColumnName = "VATChallanNo";
            }
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

            #region Decimal check

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (table.Columns.Contains("ValueChallan"))
                    {
                        row["ValueChallan"] = DecimalCheck(row["ValueChallan"].ToString());
                    }
                    if (table.Columns.Contains("QuantityinChallan"))
                    {
                        row["QuantityinChallan"] = DecimalCheck(row["QuantityinChallan"].ToString());
                    }
                    if (table.Columns.Contains("VATChallan"))
                    {
                        row["VATChallan"] = DecimalCheck(row["VATChallan"].ToString());
                    }
                    if (table.Columns.Contains("SDChallan"))
                    {
                        row["SDChallan"] = DecimalCheck(row["SDChallan"].ToString());
                    }
                    if (table.Columns.Contains("ValueIncDecAdj"))
                    {
                        row["ValueIncDecAdj"] = DecimalCheck(row["ValueIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("QuantityIncDecAdj"))
                    {
                        row["QuantityIncDecAdj"] = DecimalCheck(row["QuantityIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("VATIncDecAdj"))
                    {
                        row["VATIncDecAdj"] = DecimalCheck(row["VATIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("SDIncDecAdj"))
                    {
                        row["SDIncDecAdj"] = DecimalCheck(row["SDIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("ValueIncDecAdj"))
                    {
                        row["ValueIncDecAdj"] = DecimalCheck(row["ValueIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("VATIncDecAdj"))
                    {
                        row["VATIncDecAdj"] = DecimalCheck(row["VATIncDecAdj"].ToString());
                    }
                    if (table.Columns.Contains("SDIncDecAdj"))
                    {
                        row["SDIncDecAdj"] = DecimalCheck(row["SDIncDecAdj"].ToString());
                    }

                }

            }

            #endregion

            return table;

        }

        public string UpdateAdjustSetInvoiceNo(string FieldID, string PeriodID, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string SqlText = "";
            string result = "";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("UpdateInvoiceNo");
                }

                #endregion

                #region Insert API Header

                SqlText = @"

update VAT9_1NBRApi_SF_adjustSet set VATChallanNo= REPLACE(REPLACE(REPLACE(VATChallanNo, '/', ''), '-', ''), 'INV', '')
 where FieldID=@FieldID and PeriodID=@PeriodID
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@FieldID", FieldID);
                cmd.Parameters.AddWithValue("@periodId", PeriodID);

                cmd.ExecuteNonQuery();

                #endregion

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                result = "Success";

                return result;
            }
            catch (Exception e)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateAdjustSetInvoiceNo", e.ToString());
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

        #endregion

        #endregion

        #region Set note 58 to 64

        private void SetNote58_64(VATReturnSubFormVM subformVm, string MSGID, string CompanyBIN, SysDBInfoVMTemp connVM = null)
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

                if (sunform58_64 != null && sunform58_64.Rows.Count > 0)
                {
                    DataTable dtAPIdata = SetNote58_64Banks(sunform58_64, CompanyBIN);

                    if (dtAPIdata != null && dtAPIdata.Rows.Count > 0)
                    {

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

                    NBRApiResult bank = GatBankData(CompanyBIN, bankName, bankBranchName);

                    if (bank == null)
                        throw new Exception("Bank/Branch Name Does Not Match with IVAS Bank list");

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

            table = view.ToTable(false, "ChallanNumber", "ChallanDate", "Amount", "BankCode", "BranchCode", "Remarks");

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
                var PID = new DataColumn("PeriodID") { DefaultValue = PeriodID };
                table.Columns.Add(PID);
            }

            #endregion

            #region Column name change

            if (table.Columns.Contains("ChallanNumber"))
            {
                table.Columns["ChallanNumber"].ColumnName = "TreasuryChallanToken";
            }
            if (table.Columns.Contains("Amount"))
            {
                table.Columns["Amount"].ColumnName = "Amount";
            }
            if (table.Columns.Contains("Remarks"))
            {
                table.Columns["Remarks"].ColumnName = "Notes";
            }

            #endregion

            #region Decimal check

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (table.Columns.Contains("Amount"))
                    {
                        row["Amount"] = DecimalCheck(row["Amount"].ToString());
                    }
                }
            }

            #endregion

            return table;

        }

        #endregion

        #region Set note 27 and 32

        private void SetNote27_32(VATReturnSubFormVM subformVm, string MSGID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                SaveDataNBRApiNote27_32(subformVm.PeriodId, MSGID, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string SaveDataNBRApiNote27_32(string PeriodID, string MSGID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"

delete VAT9_1NBRApi_SF_otherSet where PeriodID=@PeriodID

insert into VAT9_1NBRApi_SF_otherSet( 
MSGID,FieldID,ChallanNumber,Date,Value,VAT,SD,PurposeNotes,PeriodID)

select @MSGID,'SL127',REPLACE(REPLACE(REPLACE(REPLACE(ChallanNumber, '/', ''), '-', ''), 'ADJ', ''), 'ACP','')
,FORMAT(Date, 'yyyyMMdd')
,CAST(Amount AS DECIMAL(10, 2))Amount
,CAST(VAT AS DECIMAL(10, 2))VAT
,CAST(SD AS DECIMAL(10, 2))SD
,Notes
,PeriodID
from VAT9_1SubFormG
where NoteNo=27
and Amount!=0
and PeriodID=@PeriodID


insert into VAT9_1NBRApi_SF_otherSet( 
MSGID,FieldID,ChallanNumber,Date,Value,VAT,SD,PurposeNotes,PeriodID)

select @MSGID,'SL132',REPLACE(REPLACE(REPLACE(REPLACE(ChallanNumber, '/', ''), '-', ''), 'ADJ', ''), 'ACP','')
,FORMAT(Date, 'yyyyMMdd')
,CAST(Amount AS DECIMAL(10, 2))Amount
,CAST(VAT AS DECIMAL(10, 2))VAT
,CAST(SD AS DECIMAL(10, 2))SD
,Notes
,PeriodID
from VAT9_1SubFormG
where NoteNo=32
and Amount!=0
and PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                cmd.Parameters.AddWithValue("@MSGID", MSGID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "SaveDataNBRApiNote27_32", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        #region Old Process

        private void xxSetNote27_32(VATReturnSubFormVM subformVm, string MSGID)
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

                if (sunform27_32 != null && sunform27_32.Rows.Count > 0)
                {
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

                    #region Update Invoice

                    UpdateOtherSetInvoiceNo(FieldId, subformVm.PeriodId);

                    #endregion

                }

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

            #region Decimal check

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (table.Columns.Contains("Value"))
                    {
                        row["Value"] = DecimalCheck(row["Value"].ToString());
                    }
                    if (table.Columns.Contains("VAT"))
                    {
                        row["VAT"] = DecimalCheck(row["VAT"].ToString());
                    }
                }
            }

            #endregion

            return table;

        }

        public string UpdateOtherSetInvoiceNo(string FieldID, string PeriodID, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            string SqlText = "";
            string result = "";

            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("UpdateInvoiceNo");
                }

                #endregion

                #region Insert API Header

                SqlText = @"

update VAT9_1NBRApi_SF_otherSet set ChallanNumber= REPLACE(REPLACE(REPLACE(ChallanNumber, '/', ''), '-', ''), 'ACP', '')
 where FieldID=@FieldID and PeriodID=@PeriodID
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@FieldID", FieldID);
                cmd.Parameters.AddWithValue("@periodId", PeriodID);

                cmd.ExecuteNonQuery();

                #endregion

                if (Vtransaction == null && transaction != null)
                {
                    transaction.Commit();
                }

                result = "Success";

                return result;
            }
            catch (Exception e)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateAdjustSetInvoiceNo", e.ToString());
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

        #endregion

        #endregion

        #region Set note 24 and 29

        private void SetNote24_29(VATReturnSubFormVM subformVm, string MSGID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                SaveDataNBRApiNote24_29(subformVm.PeriodId, MSGID, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string SaveDataNBRApiNote24_29(string PeriodID, string MSGID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"

delete VAT9_1NBRApi_SF_vdsSet where PeriodID=@PeriodID

insert into VAT9_1NBRApi_SF_vdsSet( 
MSGID,FieldID,BuyerSupplyerBIN,BuyerSupplyerName,BuyerSupplyerAddress,Value,DeductedVAT
,InvoiceNoChallanBillNo,InvoiceChallanBillDate,VATDeductionatSource,VATDeductionatSourceCerDate
,TaxDepositedSerialBookTransfer,TaxDepositedDate,Notes,PeriodID)

select @MSGID,'SL25',VendorBIN,VendorName,VendorAddress
,CAST(TotalPrice AS DECIMAL(10, 2))TotalPrice
,CAST(VDSAmount AS DECIMAL(10, 2))VDSAmount
,InvoiceNo
,FORMAT(InvoiceDate, 'yyyyMMdd')InvoiceDate
,REPLACE(REPLACE(REPLACE(VDSCertificateNo, '/', ''), '-', ''), 'VDS', '')VDSCertificateNo
,FORMAT(VDSCertificateDate, 'yyyyMMdd')VDSCertificateDate
,TaxDepositSerialNo
,FORMAT(TaxDepositDate, 'yyyyMMdd')TaxDepositDate
,Remarks
,PeriodID
from VAT9_1SubFormE
where NoteNo=24
and PeriodID=@PeriodID


insert into VAT9_1NBRApi_SF_vdsSet( 
MSGID,FieldID,BuyerSupplyerBIN,BuyerSupplyerName,BuyerSupplyerAddress,Value,DeductedVAT
,InvoiceNoChallanBillNo,InvoiceChallanBillDate,VATDeductionatSource,VATDeductionatSourceCerDate
,TaxDepositedSerialBookTransfer,TaxDepositedDate,Notes,PeriodID)

select @MSGID,'SL30',CustomerBIN,CustomerName,CustomerAddress
,CAST(TotalPrice AS DECIMAL(10, 2))TotalPrice
,CAST(VDSAmount AS DECIMAL(10, 2))VDSAmount
,InvoiceNo
,FORMAT(InvoiceDate, 'yyyyMMdd')InvoiceDate
,REPLACE(REPLACE(REPLACE(VDSCertificateNo, '/', ''), '-', ''), 'SVD', '')VDSCertificateNo
,FORMAT(VDSCertificateDate, 'yyyyMMdd')VDSCertificateDate
,SerialNo
,FORMAT(TaxDepositDate, 'yyyyMMdd')TaxDepositDate
,Remarks
,PeriodID
from VAT9_1SubFormH
where NoteNo=29
and PeriodID=@PeriodID

update VAT9_1NBRApi_SF_vdsSet set BuyerSupplyerBIN='' where 
BuyerSupplyerBIN in('-','0','N/A','NA')
and PeriodID=@PeriodID 

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                cmd.Parameters.AddWithValue("@MSGID", MSGID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "SaveDataNBRApiNote24_29", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        #region Old Process

        private void xxSetNote24_29(VATReturnSubFormVM subformVm, string MSGID)
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

                if (sunform24_29 != null && sunform24_29.Rows.Count > 0)
                {
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

            #region Decimal check

            if (table != null && table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (table.Columns.Contains("Value"))
                    {
                        row["Value"] = DecimalCheck(row["Value"].ToString());
                    }
                    if (table.Columns.Contains("DeductedVAT"))
                    {
                        row["DeductedVAT"] = DecimalCheck(row["DeductedVAT"].ToString());
                    }
                }
            }

            #endregion

            return table;

        }

        #endregion

        #endregion

        #region Set note 01 to 22

        private void SetNote01_22(VATReturnSubFormVM subformVm, string MSGID, string CompanyBin, CompanyProfileVM companyProfileVm, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                CommonDAL commonDAl = new CommonDAL();
                CompanyprofileDAL companyprofile = new CompanyprofileDAL();

                string[] retResults = new string[3];
                retResults[0] = "Fail";
                retResults[1] = "Fail";
                retResults[2] = "";

                SetDataVAT9_1NBRApi_01_22(subformVm, MSGID, CompanyBin, companyProfileVm, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void xxSetNote01_22(VATReturnSubFormVM subformVm, string MSGID, string CompanyBin, CompanyProfileVM companyProfileVm)
        {
            CommonDAL commonDAl = new CommonDAL();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

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




            foreach (int Note in subformVm.NoteNos)
            {
                dt.Clear();

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

                    if (FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {

                    }
                    else
                    {
                        if (FieldId != "SL06" || FieldId != "SL18")
                        {
                            if (subformData.Columns.Contains("BasevalueofVAT"))
                            {
                                dr["ValueBasevalueVAT"] = DecimalCheck(row["BasevalueofVAT"].ToString());
                            }
                            else if (subformData.Columns.Contains("TotalPrice"))
                            {
                                dr["ValueBasevalueVAT"] = DecimalCheck(row["TotalPrice"].ToString());
                            }
                            else
                            {
                                dr["ValueBasevalueVAT"] = "0.00";
                            }
                        }
                    }


                    #endregion

                    #region SD  VAT

                    //SL01, SL02, SL11, SL13, SL15, SL17, SL23

                    if (FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {
                    }
                    else
                    {
                        if (FieldId == "SL01" || FieldId == "SL02" || FieldId == "SL11" || FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                        {
                            dr["SD"] = DecimalCheck(row["SD"].ToString());
                            dr["VAT"] = DecimalCheck(row["VAT"].ToString());
                            dr["AssessableValue"] = DecimalCheck(row["Assessablevalue"].ToString());

                            if (FieldId != "SL02")
                            {
                                dr["AT"] = DecimalCheck(row["AT"].ToString());
                            }

                            if (FieldId == "SL02")
                            {
                                dr["InvoiceNo"] = row["Invoice/B/E No"].ToString();
                                dr["InvoiceDate"] = Convert.ToDateTime(row["Date"].ToString()).ToString("yyyyMMdd");
                            }

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
                        ////dr["CPCCode"] = row["CPC"].ToString();

                        if (FieldId == "SL01")
                        {
                            dr["CPCCode"] = "1071";
                        }
                        else
                        {
                            dr["CPCCode"] = "4000";
                        }

                    }

                    #endregion

                    #region HSCommercialDescription

                    if (FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {
                        dr["HSCommercialDescription"] = "";
                        dr["Notes"] = "";

                    }
                    else
                    {
                        dr["HSCommercialDescription"] = row["ProductDescription"].ToString();
                        dr["Notes"] = row["DetailRemarks"].ToString();

                    }

                    #endregion

                    #region Quantity

                    if (FieldId == "SL06" || FieldId == "SL18")
                    {
                        dr["Quantity"] = DecimalCheck(row["Quantity"].ToString());
                        dr["ActualSalesPurchasesValue"] = DecimalCheck(row["TotalPrice"].ToString());
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

                    if (string.IsNullOrWhiteSpace(ServiceCode) || ServiceCode.ToLower() == "na" || ServiceCode.ToLower() == "n/a" || ServiceCode.ToLower() == "-")
                    {
                        throw new Exception("Service Code not found.");
                    }

                    #region select product for VAT and SD rate

                    ////ProductDAL dal = new ProductDAL();

                    ////string itemNo = row["ItemNo"].ToString();

                    ////List<ProductVM> vms = dal.SelectAll(itemNo);

                    ////ProductVM vm = vms.FirstOrDefault();

                    ////if (vm == null)
                    ////{
                    ////    throw new Exception("product not found.");
                    ////}

                    #endregion

                    string VATRate = row["VATRate"].ToString();
                    string SDRate = row["SDRate"].ToString();

                    NBRAPI_paramVM PVM = new NBRAPI_paramVM();
                    PVM.CompanyBIN = CompanyBin;
                    PVM.GoodsServiceCode = ServiceCode;
                    PVM.PeriodKey = subformVm.PeriodKey;
                    PVM.Serial = FieldId;
                    PVM.Note = FieldId;
                    PVM.CategoryID = CatagoryId;
                    PVM.VATRate = VATRate.ToString();
                    PVM.SDRate = SDRate.ToString();

                    #region SD

                    //if (subformData.Columns.Contains("SD"))
                    //{
                    //    PVM.SDRate = row["SD"].ToString();
                    //}
                    //else if (subformData.Columns.Contains("SDAmount"))
                    //{
                    //    PVM.SDRate = row["SDAmount"].ToString();
                    //}
                    //else
                    //{
                    //    PVM.SDRate = "0.00";
                    //}

                    #endregion

                    #region VAT

                    //if (subformData.Columns.Contains("VAT"))
                    //{
                    //    PVM.VATRate = row["VAT"].ToString();
                    //}
                    //else if (subformData.Columns.Contains("VATAmount"))
                    //{
                    //    PVM.VATRate = row["VATAmount"].ToString();
                    //}
                    //else
                    //{
                    //    PVM.VATRate = "0.00";
                    //}

                    #endregion

                    NBRApiResult NBRAPIvm = GatGoodsServiceData(PVM);

                    if (NBRAPIvm == null)
                    {
                        throw new Exception("ItemID not found. Please check service code, VAT rate and SD rate");
                    }

                    if (FieldId == "SL13" || FieldId == "SL15" || FieldId == "SL17" || FieldId == "SL23")
                    {
                        dr["ItemID"] = "";

                    }
                    else
                    {
                        ////dr["DataSource"] = "2";

                        dr["ItemID"] = NBRAPIvm.ItemID;

                        ////if (subformData.Columns.Contains("Assessablevalue"))
                        ////{
                        ////    dr["AssessableValue"] = DecimalCheck(row["Assessablevalue"].ToString());
                        ////}

                    }

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

        private void SetDataVAT9_1NBRApi_01_22(VATReturnSubFormVM subformVm, string MSGID, string CompanyBin, CompanyProfileVM companyProfileVm, SysDBInfoVMTemp connVM = null)
        {
            try
            {

                CommonDAL commonDAl = new CommonDAL();
                CompanyprofileDAL companyprofile = new CompanyprofileDAL();

                string[] retResults = new string[3];
                retResults[0] = "Fail";
                retResults[1] = "Fail";
                retResults[2] = "";

                DataTable dt = new DataTable();

                #region Add Data

                AddDataNBRApi_01_22(subformVm.PeriodId, MSGID, companyProfileVm.CompanyType, subformVm.PeriodKey, null, null, connVM);

                #endregion

                #region BoE Data

                dt = GetBoEDataNBRApi_01_22(subformVm.PeriodId, null, null, connVM);

                foreach (DataRow row in dt.Rows)
                {
                    #region BoE information

                    string Note = row["NoteNo"].ToString();

                    if (!string.IsNullOrWhiteSpace(row["BoeNo"].ToString()))
                    {
                        string BENo = row["BoeNo"].ToString();
                        string FieldId = row["FieldID"].ToString();
                        string BoEItemNo = row["BoEItemNo"].ToString();

                        NBRAPI_paramVM ParamVM = new NBRAPI_paramVM();

                        ParamVM.CompanyBIN = CompanyBin;
                        ParamVM.BoENumber = BENo;
                        ParamVM.PeriodKey = subformVm.PeriodKey;
                        ParamVM.Serial = FieldId;
                        ParamVM.BoEItemNo = BoEItemNo;

                        NBRApiResult nbrVM = GatBoEData(ParamVM);

                        if (nbrVM == null)
                        {
                            throw new Exception("BE number is not match with IVAS for note " + Note);
                        }

                    }
                    else
                    {
                        throw new Exception("BE number is mendatory for note : " + Note);
                    }

                    #endregion
                }

                UpdateBoeDataNBRApi(subformVm.PeriodId, MSGID, null, null, connVM);

                #endregion

                #region Item ID

                DataTable HSCodeDt = GetServiceCodeDataNBRApi_01_22(subformVm.PeriodId, true, null, null, connVM);

                if (HSCodeDt != null && HSCodeDt.Rows.Count > 0)
                {
                    throw new Exception("Service Code not found.");
                }

                HSCodeDt = new DataTable();

                HSCodeDt = GetServiceCodeDataNBRApi_01_22(subformVm.PeriodId, false, null, null, connVM);

                if (HSCodeDt != null && HSCodeDt.Rows.Count > 0)
                {
                    foreach (DataRow row in HSCodeDt.Rows)
                    {
                        string VATRate = row["VATRate"].ToString();
                        string SDRate = row["SDRate"].ToString();
                        string ServiceCode = row["ProductCode"].ToString();
                        string FieldId = row["FieldID"].ToString();

                        NBRAPI_paramVM PVM = new NBRAPI_paramVM();
                        PVM.CompanyBIN = CompanyBin;
                        PVM.GoodsServiceCode = ServiceCode;
                        PVM.PeriodKey = subformVm.PeriodKey;
                        PVM.Serial = FieldId;
                        PVM.Note = FieldId;
                        PVM.CategoryID = companyProfileVm.CompanyType;
                        PVM.VATRate = VATRate.ToString();
                        PVM.SDRate = SDRate.ToString();

                        NBRApiResult NBRAPIvm = GatGoodsServiceData(PVM);

                        if (NBRAPIvm == null)
                        {
                            throw new Exception("ItemID not found. Please check service code, VAT rate and SD rate");
                        }

                    }

                }

                UpdateItemIDNBRApi(subformVm.PeriodId, null, null, connVM);

                #endregion

                #region Save goservSet

                SaveDataSF_goservSet(subformVm.PeriodId, null, null, connVM);

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string AddDataNBRApi_01_22(string PeriodID, string MSGID, string Category, string PeriodKey, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

            try
            {
                #region open connection and transaction

                #region New open connection and transaction
                if (Vconnection != null)
                {
                    connection = Vconnection;
                }
                if (Vtransaction != null)
                {
                    transaction = Vtransaction;
                }
                #endregion New open connection and transaction

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                }
                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("");
                }
                #endregion open connection and transaction

                DeleteNBRApi_01_22(PeriodID, MSGID, connection, transaction, connVM);

                SaveDataNBRApi_01_22(PeriodID, MSGID, Category, connection, transaction, connVM);

                UpdateMSGIDNBRApi_01_22(PeriodID, MSGID, PeriodKey, connection, transaction, connVM);

                if (transaction != null & Vtransaction == null)
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

                FileLogger.Log("IVAS_API", "AddDataNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (connection != null && Vconnection == null)
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public string DeleteNBRApi_01_22(string PeriodID, string MSGID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"

delete VAT9_1NBRApi_01_22 where PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "DeleteNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public string SaveDataNBRApi_01_22(string PeriodID, string MSGID, string Category, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"

--delete VAT9_1NBRApi_01_22 where PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,ValueBasevalueVAT,SD,VAT,HSCommercialDescription
,PeriodID,VATRate,SDRate,ProductCode,NoteNo)

select '',TotalPrice,SDAmount,VATAmount,ProductDescription,PeriodID,VATRate,SDRate,ProductCode,NoteNo
from VAT9_1SubFormA where NoteNo in('3','4','5','7','10','12','14','19','20','21')
and PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,ValueBasevalueVAT,SD,VAT,HSCommercialDescription
,CPCName,AssessableValue,AT,PeriodID,VATRate,SDRate,ProductCode,NoteNo,BoeNo,BoEDate,BoEOficeCode,BoEItemNo)

select '',BasevalueofVAT,SD,VAT,ProductDescription,CPC,Assessablevalue,AT,PeriodID,VATRate,SDRate,ProductCode,NoteNo,[Invoice/B/E No]
,FORMAT(Date, 'yyyyMMdd'),OfficeCode,BE_ItemNo
from VAT9_1SubFormB where NoteNo in('1','11')
and PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,ValueBasevalueVAT,SD,VAT,HSCommercialDescription
,CPCName,AssessableValue,PeriodID,VATRate,SDRate,ProductCode,NoteNo,InvoiceNo,InvoiceDate)

select '',BasevalueofVAT,SD,VAT,ProductDescription,CPC,Assessablevalue,PeriodID,VATRate,SDRate,ProductCode,NoteNo
,[Invoice/B/E No],FORMAT(Date, 'yyyyMMdd')
from VAT9_1SubFormB where NoteNo in('2')
and PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,DataSource,BoeNo,PeriodID,NoteNo,BoEItemNo)

select '','1',[Invoice/B/E No],PeriodID,NoteNo,BE_ItemNo
from VAT9_1SubFormB where NoteNo in('13','15','17','22')
and PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,HSCommercialDescription
,Quantity,ActualSalesPurchasesValue,PeriodID,ProductCode,VATRate,SDRate,NoteNo)

select '',ProductDescription,Quantity,TotalPrice,PeriodID,ProductCode,VATRate,SDRate,NoteNo
from VAT9_1SubFormC where NoteNo in('6','18')
and PeriodID=@PeriodID

insert into  VAT9_1NBRApi_01_22 (FieldID,ValueBasevalueVAT,HSCommercialDescription
,PeriodID,ProductCode,VATRate,SDRate,NoteNo,Category)

select '',TotalPrice,ProductDescription,PeriodID,ProductCode,VATRate,SDRate,NoteNo,@Category
from VAT9_1SubFormD where NoteNo in('8')
and PeriodID=@PeriodID

update VAT9_1NBRApi_01_22 
set FieldID= case when NoteNo='1' then 'SL01' when NoteNo='2' then 'SL02' 
when NoteNo='3' then 'SL03' when NoteNo='4' then 'SL04'
when NoteNo='5' then 'SL05' when NoteNo='6' then 'SL06'
when NoteNo='7' then 'SL07' when NoteNo='8' then 'SL08'
when NoteNo='9' then 'SL09' when NoteNo='10' then 'SL10'
when NoteNo='11' then 'SL11' when NoteNo='12' then 'SL12'
when NoteNo='13' then 'SL13' when NoteNo='14' then 'SL14'
when NoteNo='15' then 'SL15' when NoteNo='16' then 'SL16'
when NoteNo='17' then 'SL17' when NoteNo='18' then 'SL18'
when NoteNo='19' then 'SL20' when NoteNo='20' then 'SL21'
when NoteNo='21' then 'SL22' when NoteNo='22' then 'SL23'
else '' end
where PeriodID=@PeriodID

update VAT9_1NBRApi_01_22
set CPCCode=cpc.Code
from VAT9_1NBRApi_01_22 Boe inner join 
CPCDetails cpc
on Boe.CPCName = cpc.Name 
and cpc.Type='Sale'
and Boe.FieldID in('SL01','SL02')
and Boe.PeriodID=@PeriodID

update VAT9_1NBRApi_01_22
set CPCCode=cpc.Code
from VAT9_1NBRApi_01_22 Boe inner join 
CPCDetails cpc
on Boe.CPCName = cpc.Name 
and cpc.Type='Purchase'
and Boe.FieldID in('SL11')
and Boe.PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                cmd.Parameters.AddWithValue("@Category", Category);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "SaveDataNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public string UpdateMSGIDNBRApi_01_22(string PeriodID, string MSGID, string PeriodKey, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"
update VAT9_1NBRApi_01_22 set MSGID=@MSGID,PeriodKey=@PeriodKey where PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@MSGID", MSGID);
                cmd.Parameters.AddWithValue("@PeriodKey", PeriodKey);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public DataTable GetBoEDataNBRApi_01_22(string PeriodID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

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
select FieldID,BoeNo,NoteNo,BoEItemNo from VAT9_1NBRApi_01_22 where 1=1
and FieldID in('SL13','SL15','SL17','SL23')
and PeriodID=@PeriodID
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", PeriodID);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

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
                FileLogger.Log("IVAS_API", "GetBoEDataNBRApi_01_22", ex.ToString() + "\n" + sqlText);

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

            return dt;
        }

        public string UpdateBoeDataNBRApi(string PeriodID, string MSGID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region update Data

                SqlText = @"
update VAT9_1NBRApi_01_22
set BoEDate	=mBoe.BoEDate,BoEOficeCode=mBoe.BoEOffCode, CPCCode=mBoe.CPCCode
from VAT9_1NBRApi_01_22 Boe inner join 
VAT9_1NBRApi_Master_BoEData mBoe
on Boe.BoeNo = mBoe.BoENumber and Boe.FieldID=mBoe.Serial 
and Boe.BoEItemNo=mBoe.BoEItmNo
and Boe.FieldID in('SL13','SL15','SL17','SL23')
and Boe.PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateNBRApi_01_22", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public DataTable GetServiceCodeDataNBRApi_01_22(string PeriodID, bool isCheck = false, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Variables

            SqlConnection currConn = null;
            SqlTransaction transaction = null;

            string sqlText = "";
            DataTable dt = new DataTable();

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
select DISTINCT ProductCode,FieldID,VATRate,SDRate,NoteNo
from VAT9_1NBRApi_01_22 where 1=1
and FieldID not in('SL13','SL15','SL17','SL23')
and PeriodID=@PeriodID
 
";
                if (isCheck)
                {
                    sqlText += @"  and ProductCode in('-','NA','0','n/a','')";
                }

                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, currConn, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", PeriodID);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(dt);

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
                FileLogger.Log("IVAS_API", "GetServiceCodeDataNBRApi_01_22", ex.ToString() + "\n" + sqlText);

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

            return dt;
        }

        public string UpdateItemIDNBRApi(string PeriodID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region update Data

                SqlText = @"
update VAT9_1NBRApi_01_22
set ItemID	=ser.ItemID
from VAT9_1NBRApi_01_22 NBR inner join 
VAT9_1NBRApi_Master_goodsService01_04 ser
on NBR.ProductCode = ser.GoodsServiceCode and NBR.FieldID=ser.NoteNo 
and NBR.VATRate=ser.VATRate and NBR.SDRate=ser.SDRate
and NBR.PeriodKey=ser.PeriodKey
and NBR.FieldID not in('SL13','SL15','SL17','SL23')
and NBR.PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null && Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "UpdateItemIDNBRApi", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        public string SaveDataSF_goservSet(string PeriodID, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string retResult = "Fail";

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

                #region Insert Data

                SqlText = @"
delete VAT9_1NBRApi_SF_goservSet where PeriodID=@PeriodID

insert into  VAT9_1NBRApi_SF_goservSet (
 MSGID,FieldID,ValueBasevalueVAT,SD,VAT,HSCommercialDescription,Notes,Quantity,ActualSalesPurchasesValue,UnitMeasure,ItemID,Category,DataSource
,BoeNo,BoEDate,BoEOficeCode,BoEItemNo,CPCCode,AssessableValue,AT,InvoiceNo,InvoiceDate,PeriodID)

select MSGID,FieldID,ValueBasevalueVAT,SD,VAT,HSCommercialDescription,Notes,Quantity,ActualSalesPurchasesValue,UnitMeasure,ItemID,
Category,DataSource,BoeNo,BoEDate,BoEOficeCode,BoEItemNo,CPCCode,AssessableValue,AT,InvoiceNo,InvoiceDate,PeriodID
from VAT9_1NBRApi_01_22 where PeriodID=@PeriodID

";

                SqlCommand cmd = new SqlCommand(SqlText, currConn, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);

                cmd.ExecuteNonQuery();

                retResult = "Success";

                #endregion

                if (transaction != null & Vtransaction == null)
                {
                    transaction.Commit();
                }

                return retResult;
            }
            catch (Exception ex)
            {
                if (Vtransaction == null && transaction != null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "SaveDataSF_goservSet", ex.ToString());

                throw ex;
            }
            finally
            {
                if (currConn != null && VcurrConn == null)
                {
                    if (currConn.State == ConnectionState.Open)
                    {
                        currConn.Close();
                    }
                }
            }
        }

        #endregion

        #region Set Attachments

        public List<Return_9_1_SF_att_fileSet> SetAttachments(VATReturnSubFormVM subformVm, string MSGID, SysDBInfoVMTemp connVM = null)
        {
            CommonDAL commonDAl = new CommonDAL();
            CompanyprofileDAL companyprofile = new CompanyprofileDAL();

            List<Return_9_1_SF_att_fileSet> _fileSetVMs = new List<Return_9_1_SF_att_fileSet>();
            Return_9_1_SF_att_fileSet _att_fileSetVM = new Return_9_1_SF_att_fileSet();

            string[] retResults = new string[3];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";

            try
            {
                string fileDirectory = "";
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;
                string rootfileDirectory = pathRoot + "\\IVAS Files";

                Directory.CreateDirectory(rootfileDirectory);

                DataTable subformData = GetAttachmentData(subformVm);

                foreach (DataRow row in subformData.Rows)
                {
                    string fileName = row["FileName"].ToString();
                    string fileType = row["FileType"].ToString();
                    string invocieNo = "";
                    string DocType = row["DocType"].ToString();

                    _att_fileSetVM = new Return_9_1_SF_att_fileSet();
                    _att_fileSetVM.MSGID = MSGID;

                    #region Convert file and add in list

                    string fileLocation = row["FileLocation"].ToString();

                    string pdfFilePath = fileLocation + "\\" + fileName;

                    fileName = Path.GetFileNameWithoutExtension(pdfFilePath);

                    string base64String = PdfToBase64(pdfFilePath);

                    _att_fileSetVM.FileName = fileName;
                    _att_fileSetVM.DocType = DocType;
                    _att_fileSetVM.Content = base64String.ToString();
                    _att_fileSetVM.FileType = fileType;

                    _fileSetVMs.Add(_att_fileSetVM);

                    #endregion

                }

                #region Comments

                //////// Loop for note
                //foreach (int Note in subformVm.NoteNos)
                //{
                //    if (subformVm.NoteNo == 24 || subformVm.NoteNo == 29)
                //    {

                //    }

                //    #region Make Datatable

                //    DataTable dt = new DataTable();
                //    dt.Clear();

                //    dt.Columns.Add("MSGID");
                //    dt.Columns.Add("FileName");
                //    dt.Columns.Add("DocType");
                //    dt.Columns.Add("Content");
                //    dt.Columns.Add("FileType");

                //    #endregion

                //    subformVm.NoteNo = Note;

                //    string FieldId = GetFieldIDByNote(subformVm.NoteNo.ToString());

                //    DataTable subformData = GetSubNoteData(subformVm);


                //    ////////// Loop for note waise data
                //    foreach (DataRow row in subformData.Rows)
                //    {
                //        string fileName = "";
                //        string fileType = "";
                //        string invocieNo = "";
                //        string DocType = "";

                //        _att_fileSetVM = new Return_9_1_SF_att_fileSet();
                //        _att_fileSetVM.MSGID = MSGID;

                //        #region Note 24, 29 (6.6)

                //        if (subformVm.NoteNo == 24 || subformVm.NoteNo == 29)
                //        {
                //            fileDirectory = rootfileDirectory + "\\TempVDSPDF";
                //            Directory.CreateDirectory(fileDirectory);

                //            if (subformVm.NoteNo == 24)
                //            {

                //                NBRReports _report = new NBRReports();

                //                string vendorId = "";
                //                string DepositNumber = row["VDSCertificateNo"].ToString();
                //                string DepositFrom = "1753-Jan-01";
                //                string DepositTo = "9998-Dec-31";
                //                string IssueFrom = "1753-Jan-01";
                //                string IssueTo = "9998-Dec-31";
                //                string BillFrom = "1753-Jan-01";
                //                string BillTo = "9998-Dec-31";
                //                string PurchaseNumber = "NA";
                //                bool PurchaseVDS = true;

                //                reportDocument = _report.VDS12KhaNew(vendorId, DepositNumber, DepositFrom, DepositTo, IssueFrom, IssueTo, BillFrom, BillTo,
                //                                                      PurchaseNumber, PurchaseVDS, connVM);

                //                invocieNo = DepositNumber;
                //                fileType = "pdf";
                //                DocType = "04";

                //            }

                //        }

                //        #endregion

                //        #region Note  31 (6.7)

                //        if (subformVm.NoteNo == 31)
                //        {
                //            fileDirectory = rootfileDirectory + "\\TempCreditNotePDF";
                //            Directory.CreateDirectory(fileDirectory);

                //            SaleReport _sReport = new SaleReport();

                //            string CNinvocieNo = row["CreditNoteNo"].ToString();

                //            reportDocument = _sReport.Report_VAT6_3_Completed(CNinvocieNo, "Credit", true, false, false, false, false, false, false, 0, 0
                //                , false, false, false, false, false, false, connVM);

                //            invocieNo = CNinvocieNo;
                //            fileType = "pdf";
                //            DocType = "07";

                //        }

                //        #endregion

                //        #region Note  26 (6.8)

                //        if (subformVm.NoteNo == 26)
                //        {
                //            fileDirectory = rootfileDirectory + "\\TempDebitNotePDF";
                //            Directory.CreateDirectory(fileDirectory);

                //            SaleReport _sReport = new SaleReport();

                //            string CNinvocieNo = row["TaxChallanNo"].ToString();

                //            reportDocument = _sReport.Report_VAT6_3_Completed(CNinvocieNo, "Debit", false, true, false, false, false, false, false, 0, 0
                //                , false, false, false, false, false, false, connVM);

                //            invocieNo = CNinvocieNo;
                //            fileType = "pdf";
                //            DocType = "08";

                //        }

                //        #endregion

                //        #region Note 27 (Any Other Adjustments)

                //        if (subformVm.NoteNo == 27)
                //        {
                //            ////fileDirectory = rootfileDirectory + "\\TempAdjustments27PDF";
                //            ////Directory.CreateDirectory(fileDirectory);

                //            ////SaleReport _sReport = new SaleReport();

                //            ////string CNinvocieNo = row["TaxChallanNo"].ToString();

                //            ////reportDocument = _sReport.Report_VAT6_3_Completed(CNinvocieNo, "Debit", false, true, false, false, false, false, false, 0, 0
                //            ////    , false, false, false, false, false, false, connVM);

                //            ////invocieNo = CNinvocieNo;
                //            ////fileType = "pdf";
                //            ////DocType = "09";

                //        }

                //        #endregion

                //        #region Note 32 (Any Other Adjustments)

                //        if (subformVm.NoteNo == 32)
                //        {
                //            ////fileDirectory = rootfileDirectory + "\\TempAdjustments32PDF";
                //            ////Directory.CreateDirectory(fileDirectory);

                //            ////SaleReport _sReport = new SaleReport();

                //            ////string CNinvocieNo = row["TaxChallanNo"].ToString();

                //            ////reportDocument = _sReport.Report_VAT6_3_Completed(CNinvocieNo, "Debit", false, true, false, false, false, false, false, 0, 0
                //            ////    , false, false, false, false, false, false, connVM);

                //            ////invocieNo = CNinvocieNo;
                //            //fileType = "pdf";
                //            //DocType = "10";

                //        }

                //        #endregion

                //        #region Convert file and add in list

                //        invocieNo = invocieNo.Replace("\\", "-").Replace("/", "-");

                //        if (!string.IsNullOrWhiteSpace(invocieNo))
                //        {
                //            fileName = invocieNo + "~" + DocType + "~" + DateTime.Now.ToString("yyyyMMddhhmmss");

                //            string pdfFilePath = fileDirectory + "\\" + fileName + "." + fileType;

                //            DeleteFiles(pdfFilePath);

                //            ExportPDf(reportDocument, fileDirectory, fileName);

                //            string base64String = PdfToBase64(pdfFilePath);

                //            _att_fileSetVM.FileName = fileName;
                //            _att_fileSetVM.DocType = DocType;
                //            _att_fileSetVM.Content = base64String.ToString();
                //            _att_fileSetVM.FileType = fileType;

                //            _fileSetVMs.Add(_att_fileSetVM);

                //        }

                //        #endregion

                //        #region Note 32 (Any Other Adjustments)

                //        if (subformVm.NoteNo == 32)
                //        {

                //            _att_fileSetVM.Content = _fileSetVMs.FirstOrDefault().Content;

                //            _att_fileSetVM.FileName = "AdjustmentsNote32~10~" + DateTime.Now.ToString("yyyyMMddhhmmss");
                //            _att_fileSetVM.DocType = "10";
                //            _att_fileSetVM.FileType = "pdf";

                //            _fileSetVMs.Add(_att_fileSetVM);
                //        }

                //        #endregion


                //    }

                //}

                #endregion

                #region Comments

                ////_att_fileSetVM.Content = _fileSetVMs.FirstOrDefault().Content;

                ////_att_fileSetVM.FileName = "6_10~06~" + DateTime.Now.ToString("yyyyMMddhhmmss");
                ////_att_fileSetVM.DocType = "06";
                ////_att_fileSetVM.FileType = "pdf";

                ////_fileSetVMs.Add(_att_fileSetVM);

                #endregion

                return _fileSetVMs;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void DeleteFiles(string fileDirectory)
        {
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
        }

        private void ExportPDf(ReportDocument objrpt, string fileDirectory, string fileName)
        {
            Stream st = new MemoryStream();

            st = objrpt.ExportToStream(ExportFormatType.PortableDocFormat);
            string path = fileDirectory + "\\" + fileName + ".pdf";

            using (var output = new FileStream(path, FileMode.Create))
            {
                st.CopyTo(output);
            }

            st.Dispose();
        }

        private string PdfToBase64(string pdfFilePath)
        {
            ////string pdfFilePath = "your_pdf_file.pdf"; // Replace with your PDF file path

            try
            {
                byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                string base64String = Convert.ToBase64String(pdfBytes);

                return base64String;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable GetAttachmentData(VATReturnSubFormVM subformVm, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string sqlText = "";

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
                    transaction = connection.BeginTransaction("GetAttachmentData");
                }

                #endregion

                #region Statement

                sqlText = @" ";

                #region sqlText

                sqlText = @" 
SELECT
Id
,PeriodId
,FileName
,DocType
,FileType
,Notes
,FileLocation
FROM VAT9_1NBRApi_Attachment
where 1=1 
and PeriodID=@PeriodID
 
";
                #endregion

                #endregion

                #region SQL Command

                SqlCommand objCommVAT19 = new SqlCommand(sqlText, connection, transaction);
                objCommVAT19.Parameters.AddWithValue("@PeriodID", subformVm.PeriodId);

                #endregion

                SqlDataAdapter dataAdapter = new SqlDataAdapter(objCommVAT19);
                dataAdapter.Fill(table);

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

                FileLogger.Log("IVAS_API", "GetSubNoteData", e.ToString());
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

        private string makeJson(string PeriodId, Return_9_1_MainFull MainFull)
        {
            string json = "";

            try
            {

                ////DataSet ds = GetNBR_API_SubfromData(PeriodId);

                ////DataTable dtHeader = ds.Tables[0];
                ////DataTable dtAdjustSet = ds.Tables[1];
                ////DataTable dtchallanSet = ds.Tables[2];
                ////DataTable dtotherSet = ds.Tables[3];
                ////DataTable dtvdsSet = ds.Tables[4];
                ////DataTable dtgoservSet = ds.Tables[5];

                ////Return_9_1_Header HeaderVM = new Return_9_1_Header();

                ////if (dtHeader != null && dtHeader.Rows.Count > 0)
                ////{
                ////    HeaderVM = dtHeader.ToList<Return_9_1_Header>().FirstOrDefault();
                ////    HeaderVM.SendDate = DateTime.Now.ToString("yyyyMMdd");
                ////    HeaderVM.SendTime = DateTime.Now.ToString("HHMMss");

                ////    HeaderVM.Return_9_1_MainFull = MainFull;

                ////    if (dtAdjustSet != null && dtAdjustSet.Rows.Count > 0)
                ////    {
                ////        List<Return_9_1_SF_adjustSet> adjustSetVM = dtAdjustSet.ToList<Return_9_1_SF_adjustSet>();

                ////        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_adjustSet = adjustSetVM;
                ////    }

                ////    if (dtchallanSet != null && dtchallanSet.Rows.Count > 0)
                ////    {
                ////        List<Return_9_1_SF_challanSet> challanSet = dtchallanSet.ToList<Return_9_1_SF_challanSet>();

                ////        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_challanSet = challanSet;
                ////    }

                ////    if (dtgoservSet != null && dtgoservSet.Rows.Count > 0)
                ////    {
                ////        List<Return_9_1_SF_goservSet> goservSetVM = dtgoservSet.ToList<Return_9_1_SF_goservSet>();

                ////        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_goservSet = goservSetVM;
                ////    }

                ////    if (dtotherSet != null && dtotherSet.Rows.Count > 0)
                ////    {
                ////        List<Return_9_1_SF_otherSet> otherSet = dtotherSet.ToList<Return_9_1_SF_otherSet>();

                ////        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_otherSet = otherSet;
                ////    }

                ////    if (dtvdsSet != null && dtvdsSet.Rows.Count > 0)
                ////    {
                ////        List<Return_9_1_SF_vdsSet> vdsSet = dtvdsSet.ToList<Return_9_1_SF_vdsSet>();

                ////        HeaderVM.Return_9_1_MainFull.Return_9_1_SF_vdsSet = vdsSet;
                ////    }

                ////    json = JsonConvert.SerializeObject(HeaderVM);

                ////}

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
 MSGID
,FBTyp
,BIN
,PeriodKey
,Depositor
FROM VAT9_1NBRApiHeader where 1=1 and PeriodID=@PeriodID

-------- Note 26 and 31-------1-----
SELECT 
 MSGID
,FieldID
,NoteNo
,IssueDat
,VATChallanNo
,VATChallanDate
,ReasonIssuance
,ValueChallan
,QuantityinChallan
,VATChallan
,SDChallan
,ValueIncDecAdj
,QuantityIncDecAdj
,VATIncDecAdj
,SDIncDecAdj    
FROM VAT9_1NBRApi_SF_adjustSet where 1=1 and PeriodID=@PeriodID

-------- Note 58 to 64--------2----

SELECT 
MSGID
,FieldID
,TreasuryChallanToken
,ChallanDate
,Amount
,Notes
,BankCode
,BranchCode
FROM VAT9_1NBRApi_SF_challanSet where 1=1 and PeriodID=@PeriodID

-------- Note 27 and 32--------3----

SELECT 
 MSGID
,FieldID
,ChallanNumber
,Date
,Value
,VAT
,SD
,PurposeNotes
FROM VAT9_1NBRApi_SF_otherSet where 1=1 and PeriodID=@PeriodID

-------- Note 24 and 29 --------4----

SELECT 
MSGID
,FieldID
,BuyerSupplyerBIN
,BuyerSupplyerName
,BuyerSupplyerAddress
,Value
,DeductedVAT
,InvoiceNoChallanBillNo
,InvoiceChallanBillDate
,VATDeductionatSource
,VATDeductionatSourceCerDate
,TaxDepositedSerialBookTransfer
,TaxDepositedDate
,Notes
FROM VAT9_1NBRApi_SF_vdsSet where 1=1 and PeriodID=@PeriodID

-------- Note 1 and 22 --------5----

SELECT
 MSGID
,FieldID
,ValueBasevalueVAT
,SD
,VAT
,HSCommercialDescription
,Notes
,Quantity
,ActualSalesPurchasesValue
,UnitMeasure
,ItemID
,Category
,DataSource
,BoeNo
,BoEDate
,BoEOficeCode
,BoEItemNo
,CPCCode
,AssessableValue
,AT
,InvoiceNo
,InvoiceDate
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
                FileLogger.Log("IVAS_API", "GetNBR_API_SubfromData", ex.ToString() + "\n" + sqlText);

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

        public string GatNewPeriodKey(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    connection = _dbsqlConnection.GetConnection(connVM);
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
                            string period = item.Txt50;
                            period = period.Replace(", ", "-");

                            string[] cValues = { period };
                            string[] cFields = { "PeriodName" };
                            FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                            varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, connection, transaction).FirstOrDefault();
                            string pId = varFiscalYearVM.PeriodID;

                            mVM.PeriodId = pId;
                            mVM.PeriodKey = pKey;

                            PeriodKey = GatPeriodKeyValue(mVM, connection, transaction);

                            if (string.IsNullOrWhiteSpace(PeriodKey))
                            {
                                NBRApiResult vm = new NBRApiResult();
                                vm.PeriodKey = pKey;
                                vm.Txt50 = period;
                                vm.BIN = msgVM.BIN;
                                vm.DueDate = item.DueDate;
                                vm.Txt50 = vm.Txt50.Replace(", ", "-");
                                vm.PeriodId = pId;

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

                FileLogger.Log("IVAS_API", "GatNewPeriodKey", ex.ToString());
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

        public string GatPeriodKeyValue(MessageIdVM msgVM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    connection = _dbsqlConnection.GetConnection(connVM);
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

                FileLogger.Log("IVAS_API", "GatPeriodKeyValue", e.ToString());
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

        public string SavePeriodKey(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    connection = _dbsqlConnection.GetConnection(connVM);
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

                FileLogger.Log("IVAS_API", "SavePeriodKey", ex.ToString());

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
                    transaction = connection.BeginTransaction("GatBoEData");
                }

                #endregion

                #region Get Exits Period key

                int check = CheckBoEData(VM, connection, transaction);

                #endregion

                if (check == 0)
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
                            pvm.GoodsServiceCode = item.GoodService;

                            check = CheckBoEData(pvm, connection, transaction);

                            if (check == 0)
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
                                vm.AssessValue = item.AssessValue.Trim();
                                vm.VAT = item.VAT.Trim();
                                vm.SD = item.SD.Trim();
                                vm.AT = item.AT.Trim();

                                SaveBoEData(vm, connection, transaction);

                            }

                        }
                    }
                }

                #region get data

                APIResult = _NBR_API_DAL.SelectBoEData(VM, connection, transaction).FirstOrDefault();

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

                FileLogger.Log("IVAS_API", "GatBoEData", ex.ToString());
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

                SqlText = @"
select count(BoENumber) from VAT9_1NBRApi_Master_BoEData where BoENumber =@BoENumber 
and PeriodKey=@PeriodKey and Serial=@Serial ";

                if (!string.IsNullOrWhiteSpace(VM.GoodsServiceCode))
                {
                    SqlText += @" and GoodService=@GoodService";
                }
                if (!string.IsNullOrWhiteSpace(VM.BoEItemNo))
                {
                    SqlText += @" and BoEItmNo=@BoEItemNo";
                }

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BoENumber", VM.BoENumber);
                cmd.Parameters.AddWithValue("@PeriodKey", VM.PeriodKey);
                cmd.Parameters.AddWithValue("@Serial", VM.Serial);

                if (!string.IsNullOrWhiteSpace(VM.GoodsServiceCode))
                {
                    cmd.Parameters.AddWithValue("@GoodService", VM.GoodsServiceCode);
                }
                if (!string.IsNullOrWhiteSpace(VM.BoEItemNo))
                {
                    cmd.Parameters.AddWithValue("@BoEItemNo", VM.BoEItemNo);
                }
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

                FileLogger.Log("IVAS_API", "CheckBoEData", e.ToString());
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

                FileLogger.Log("IVAS_API", "SaveBoEData", ex.ToString());

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
            //if (!string.IsNullOrWhiteSpace(vm.BoENumber))
            //{
            //    filterText += " and BoENumber eq '" + vm.BoENumber + "'";
            //}
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
                            string note = item.Note.Trim(';');

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
                                    vm.ValidFrom = item.ValidFrom;
                                    vm.ValidTo = item.ValidTo;
                                    vm.ItemID = item.ItemID;
                                    vm.Note = item.Note;
                                    vm.ManualInput = item.ManualInput;
                                    vm.CategoryID = "";
                                    vm.TotTate = "0";
                                    vm.StandardVatRate = "0";
                                    vm.SpecRate06 = "0";

                                    if (!string.IsNullOrWhiteSpace(item.SpecRate06))
                                    {
                                        vm.SpecRate06 = item.SpecRate06;
                                    }
                                    if (!string.IsNullOrWhiteSpace(item.CategoryID))
                                    {
                                        vm.CategoryID = item.CategoryID;
                                    }
                                    if (!string.IsNullOrWhiteSpace(item.TotTate))
                                    {
                                        vm.TotTate = item.TotTate;
                                    }
                                    if (!string.IsNullOrWhiteSpace(item.StandardVatRate))
                                    {
                                        vm.StandardVatRate = item.StandardVatRate;
                                    }
                                    vm.NoteNo = substring;

                                    SaveGoodsServiceData(vm, connection, transaction);

                                }

                            }

                        }
                    }
                }

                #region get data

                APIResult = _NBR_API_DAL.SelectGoodsServiceData(VM, true, connection, transaction).FirstOrDefault();

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

                FileLogger.Log("IVAS_API", "GatGoodsServiceData", ex.ToString());
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
where GoodsServiceCode =@GoodsServiceCode and PeriodKey=@PeriodKey and NoteNo=@Serial ";

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

                FileLogger.Log("IVAS_API", "CheckGoodsServiceData", ex.ToString());

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

                FileLogger.Log("IVAS_API", "SaveGoodsServiceData", ex.ToString());

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

                FileLogger.Log("IVAS_API", "GatNewCompanyCategory", ex.ToString());
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

                FileLogger.Log("IVAS_API", "GatCompanyCategoryValue", e.ToString());
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

                FileLogger.Log("IVAS_API", "SavePeriodKey", ex.ToString());

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

        public NBRApiResult GatBankData(string CompanyBIN, string BankName, string BankBranch, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
        {
            NBRAPI_paramVM VM = new NBRAPI_paramVM();

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
                    transaction = connection.BeginTransaction("GatBoEData");
                }

                #endregion

                #region Check data

                int check = CheckBankData(BankName, BankBranch, connection, transaction);

                #endregion

                if (check == 0)
                {
                    List<NBRApiResult> APIresults = get_bank(CompanyBIN, BankName, BankBranch);

                    if (APIresults != null && APIresults.Count > 0)
                    {
                        foreach (var item in APIresults)
                        {

                            string BnkName = item.Bankn;
                            string BranchName = item.Brannm;

                            check = CheckBankData(BnkName, BranchName, connection, transaction);

                            if (check == 0)
                            {
                                NBRApiResult vm = new NBRApiResult();

                                vm.BIN = item.BIN;
                                vm.BanCD = item.BanCD;
                                vm.RoutingNumber = item.RoutingNumber;
                                vm.Bankn = item.Bankn;
                                vm.Brannm = item.Brannm;
                                vm.BanCDDatefrom = item.BanCDDatefrom;
                                vm.BanCDDateto = item.BanCDDateto;
                                vm.RoutingNumberDatefrom = item.RoutingNumberDatefrom;
                                vm.RoutingNumberDateto = item.RoutingNumberDateto;

                                SaveBankData(vm, connection, transaction);

                            }

                        }
                    }
                }

                #region get data

                APIResult = _NBR_API_DAL.SelectBankData(BankName, BankBranch, connection, transaction).FirstOrDefault();

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

                FileLogger.Log("IVAS_API", "GatBoEData", ex.ToString());
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


        public int CheckBankData(string BankName, string BankBranch, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
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

                SqlText = @"
select count(Id) from VAT9_1NBRApi_Master_Bank where Bankn =@Bankn 
and Brannm=@Brannm ";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@Bankn", BankName);
                cmd.Parameters.AddWithValue("@Brannm", BankBranch);

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

                FileLogger.Log("IVAS_API", "CheckBankData", e.ToString());
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

        public string SaveBankData(NBRApiResult VM, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null)
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
insert into VAT9_1NBRApi_Master_Bank (
 BIN
,BanCD
,RoutingNumber
,Bankn
,Brannm
,BanCDDatefrom
,BanCDDateto
,RoutingNumberDatefrom
,RoutingNumberDateto
)
values(
 @BIN
,@BanCD
,@RoutingNumber
,@Bankn
,@Brannm
,@BanCDDatefrom
,@BanCDDateto
,@RoutingNumberDatefrom
,@RoutingNumberDateto
)
";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@BIN", VM.BIN);
                cmd.Parameters.AddWithValue("@BanCD", VM.BanCD);
                cmd.Parameters.AddWithValue("@RoutingNumber", VM.RoutingNumber);
                cmd.Parameters.AddWithValue("@Bankn", VM.Bankn);
                cmd.Parameters.AddWithValue("@Brannm", VM.Brannm);
                cmd.Parameters.AddWithValue("@BanCDDatefrom", VM.BanCDDatefrom);
                cmd.Parameters.AddWithValue("@BanCDDateto", VM.BanCDDateto);
                cmd.Parameters.AddWithValue("@RoutingNumberDatefrom", VM.RoutingNumberDatefrom);
                cmd.Parameters.AddWithValue("@RoutingNumberDateto", VM.RoutingNumberDateto);

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

                FileLogger.Log("IVAS_API", "SaveBankData", ex.ToString());

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

        private List<NBRApiResult> get_bank(string CompanyBIN, string BankName, string BankBranch)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetBankApiData(CompanyBIN);

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

            return APIresults;

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
        }

        public string PostData(string url, string payLoad)
        {
            string responseMessage = "";
            try
            {
                #region Comments

                //////WebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //////request.Method = "POST";
                ////////request.Headers.Add("Authorization", "Bearer " + auth.Access_token);
                //////byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                //////request.ContentLength = byteArray.Length;
                //////request.ContentType = "application/json";
                //////////request.Accept = "application/json";

                //////NetworkCredential creds = GetCredentials();
                //////request.Credentials = creds;

                //////Stream datastream = request.GetRequestStream();
                //////datastream.Write(byteArray, 0, byteArray.Length);
                //////datastream.Close();

                //////WebResponse response = request.GetResponse();
                //////datastream = response.GetResponseStream();

                //////StreamReader reader = new StreamReader(datastream);
                //////string responseMessage = reader.ReadToEnd();

                //////reader.Close();

                //return responseMessage;

                //------------------------------

                #endregion

                ////url = "http://10.34.5.106:8050/sap/opu/odata/sap/ZOD_ERP_RETURN_SRV/Return_9_1_MSG_headerSet";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.Timeout = 1200000; //20 min
                request.Headers.Add("X-Requested-With", "X");
                request.KeepAlive = true;
                //request.SendChunked = true;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.DefaultConnectionLimit = 20;

                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                using (Stream datastream = request.GetRequestStream())
                {
                    datastream.Write(byteArray, 0, byteArray.Length);
                }

                try
                {
                    using (WebResponse response = request.GetResponse())
                    using (Stream dataStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        responseMessage = reader.ReadToEnd();
                        return responseMessage;
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        using (WebResponse errorResponse = ex.Response)
                        using (Stream dataStream = errorResponse.GetResponseStream())
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            string errorResponseMessage = reader.ReadToEnd();
                            throw new ArgumentNullException("", errorResponseMessage);
                            //responseMessage = errorResponseMessage;
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("", ex.Message);
                    }

                }
                return responseMessage;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task CallPostData(string apiUrl, string jsonPayload)
        {

            try
            {
                string result = await PostData_await(apiUrl, jsonPayload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> PostData_await(string url, string payLoad)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(payLoad);
                request.ContentLength = byteArray.Length;
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.KeepAlive = true;
                //request.Timeout = 2400000;
                request.Headers.Add("X-Requested-With", "X");

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                NetworkCredential creds = GetCredentials();
                request.Credentials = creds;

                //////using (Stream datastream = await Task<Stream>.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null))
                //////{
                //////    await datastream.WriteAsync(byteArray, 0, byteArray.Length);
                //////}

                //////using (WebResponse response = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null))
                //////using (Stream dataStream = response.GetResponseStream())
                //////using (StreamReader reader = new StreamReader(dataStream))
                //////{
                //////    return await reader.ReadToEndAsync();
                //////}

                using (Stream datastream = await request.GetRequestStreamAsync())
                {
                    await datastream.WriteAsync(byteArray, 0, byteArray.Length);
                }

                // Get the response
                using (WebResponse response = await request.GetResponseAsync())
                using (Stream dataStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(dataStream))
                {
                    // Read and return the response content
                    return await reader.ReadToEndAsync();
                }


            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (WebResponse errorResponse = ex.Response)
                    using (Stream dataStream = errorResponse.GetResponseStream())
                    using (StreamReader reader = new StreamReader(dataStream))
                    {
                        string errorResponseMessage = reader.ReadToEnd();
                        throw new ArgumentNullException("", errorResponseMessage);
                        //responseMessage = errorResponseMessage;
                    }
                }
                else
                {
                    throw new ArgumentNullException("", ex.Message);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task<string> HandleWebExceptionAsync(WebException ex)
        {
            if (ex.Response != null)
            {
                using (WebResponse errorResponse = ex.Response)
                using (Stream errorDataStream = errorResponse.GetResponseStream())
                {
                    if (errorDataStream != null)
                    {
                        using (MemoryStream errorMemoryStream = new MemoryStream())
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            while ((bytesRead = await errorDataStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await errorMemoryStream.WriteAsync(buffer, 0, bytesRead);
                            }
                            errorMemoryStream.Position = 0;

                            using (StreamReader errorReader = new StreamReader(errorMemoryStream))
                            {
                                return await errorReader.ReadToEndAsync();
                            }
                        }
                    }
                }
            }

            throw new ArgumentNullException("", ex.Message);
        }

        #region Submit API

        private List<NBRApiResult> getSubmitData(string CompanyBIN, string PeriodKey)
        {
            List<NBRApiResult> APIresults = new List<NBRApiResult>();

            try
            {

                Root_NBRAPI apiData = GetSubmitApiData(CompanyBIN, PeriodKey);

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

        private Root_NBRAPI GetSubmitApiData(string CompanyBIN, string PeriodKey)
        {

            string url;

            url = SubmitAPIUrl;

            UriBuilder uriBuilder = new UriBuilder(url);

            NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);

            string filterText = "";

            filterText = "BIN eq '" + CompanyBIN + "'";

            if (!string.IsNullOrWhiteSpace(PeriodKey))
            {
                filterText += " and PeriodKey eq '" + PeriodKey + "'";
            }

            query["$filter"] = filterText;
            query["$format"] = "json";

            uriBuilder.Query = query.ToString();

            string result = GetData(uriBuilder.ToString());

            Root_NBRAPI ApiData = JsonConvert.DeserializeObject<Root_NBRAPI>(result);

            return ApiData;
        }

        public string CheckDataIsSubmited(string PeriodID, SqlConnection Vconnection = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            SqlConnection connection = Vconnection;
            SqlTransaction transaction = Vtransaction;
            DBSQLConnection _dbsqlConnection = new DBSQLConnection();
            DataTable table = new DataTable();
            string SqlText = "";
            string IsSubmit = "";
            try
            {
                #region Transaction Connection

                if (connection == null)
                {
                    connection = _dbsqlConnection.GetConnection(connVM);
                    connection.Open();
                }

                if (transaction == null)
                {
                    transaction = connection.BeginTransaction("GetLastMsgId");
                }

                #endregion

                #region Check IsSubmit

                SqlText = @"select isnull(IsSubmit,'N')IsSubmit from VAT9_1NBRApiHeader where PeriodID = @PeriodID";

                SqlCommand cmd = new SqlCommand(SqlText, connection, transaction);
                cmd.Parameters.AddWithValue("@PeriodID", PeriodID);
                IsSubmit = (string)cmd.ExecuteScalar();

                #endregion

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                return IsSubmit;
            }
            catch (Exception e)
            {
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "CheckDataIsSubmited", e.ToString());
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


        #endregion

        #region VAT9_1NBRApi_Attachment

        public string[] VAT9_1NBRApi_Attachment(VAT9_1NBRApi_AttachmentVM paramVm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable table = new DataTable();
            string SqlText = "";

            #endregion Initializ

            try
            {
                #region New open
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

                #region Check Exist

                sqlText = "";
                sqlText = sqlText + " SELECT COUNT(Id) Id FROM VAT9_1NBRApi_Attachment WHERE Notes = @Notes AND PeriodId = @PeriodId ";
                SqlCommand cmdExistTran = new SqlCommand(sqlText, currConn, transaction);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@Notes", paramVm.Notes);
                cmdExistTran.Parameters.AddWithValueAndNullHandle("@PeriodId", paramVm.PeriodId);

                IDExist = (int)cmdExistTran.ExecuteScalar();

                if (IDExist > 0)
                {
                    throw new ArgumentNullException(MessageVM.insert, MessageVM.msgFindExistSame);
                }

                #endregion

                #region Insert new Information

                retResults = Insert_VAT9_1NBRApi_Attachment(paramVm, currConn, transaction, null);

                Id = retResults[3];

                if (retResults[0] != "Success")
                {
                    throw new ArgumentNullException(MessageVM.insert, retResults[1]);
                }

                #endregion  Insert new Information

                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                #region SuccessResult
                retResults = new string[4];
                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + paramVm.Id;
                retResults[3] = Id;
                #endregion SuccessResult
            }
            catch (Exception e)
            {
                retResults = new string[4];
                retResults[0] = "Fail";
                retResults[1] = e.Message;
                retResults[2] = "0";
                retResults[3] = "N";

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IVAS_API", "VAT9_1NBRApi_Attachment", e.ToString());
                throw;
            }
            finally
            {
                if (currConn != null && VcurrConn == null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }

            return retResults;
        }

        public string[] Insert_VAT9_1NBRApi_Attachment(VAT9_1NBRApi_AttachmentVM Master, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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
                    transaction = currConn.BeginTransaction("VAT9_1NBRApi_Attachment");
                }
                #endregion open connection and transaction

                #region Insert

                sqlText = "";
                sqlText += " INSERT INTO VAT9_1NBRApi_Attachment";
                sqlText += " (";
                sqlText += "  FileName";
                sqlText += " ,DocType";
                sqlText += " ,FileType";
                sqlText += " ,PeriodId";
                sqlText += " ,FileLocation";
                sqlText += " ,Notes";



                sqlText += " )";

                sqlText += " VALUES";
                sqlText += " (";
                sqlText += "   @FileName";
                sqlText += "  ,@DocType";
                sqlText += "  ,@FileType";
                sqlText += "  ,@PeriodId";
                sqlText += "  ,@FileLocation";
                sqlText += "  ,@Notes";



                sqlText += ")  SELECT SCOPE_IDENTITY() ";

                SqlCommand cmdInsert = new SqlCommand(sqlText, currConn, transaction);

                cmdInsert.Parameters.AddWithValueAndNullHandle("@FileName", Master.FileName);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@DocType", Master.DocType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FileType", Master.FileType);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@PeriodId", Master.PeriodId);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@FileLocation", Master.FileLocation);
                cmdInsert.Parameters.AddWithValueAndNullHandle("@Notes", Master.Notes);


                transResult = Convert.ToInt32(cmdInsert.ExecuteScalar());
                retResults[3] = transResult.ToString();

                if (transResult <= 0)
                {
                    throw new ArgumentNullException(MessageVM.saleMsgMethodNameInsert, MessageVM.saleMsgSaveNotSuccessfully);
                }

                #endregion ID generated completed,Insert new Information in Header

                #region Commit
                if (Vtransaction == null)
                {
                    if (transaction != null)
                    {
                        if (transResult > 0)
                        {
                            transaction.Commit();
                        }

                    }
                }

                #endregion Commit

                #region SuccessResult

                retResults[0] = "Success";
                retResults[1] = MessageVM.saleMsgSaveSuccessfully;
                retResults[2] = "" + Master.Id;
                retResults[3] = "0";
                #endregion SuccessResult

            }
            #endregion Try

            #region Catch and Finall
            catch (Exception ex)
            {
                retResults = new string[4];
                retResults[0] = "Fail";
                retResults[1] = ex.Message;
                retResults[2] = "0";
                retResults[3] = "N";
                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }

                FileLogger.Log("IVAS_API", "Insert_VAT9_1NBRApi_Attachment", ex.ToString() + "\n" + sqlText);
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
            #endregion Catch and Finall

            #region Result
            return retResults;
            #endregion Result
        }

        public DataTable SelectAll(string PeriodId = "", SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
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

                #region SqlText

                sqlText += @"

SELECT PeriodId,Notes,DocType,FileName,FileType,FileLocation FROM VAT9_1NBRApi_Attachment
WHERE  1=1 ";

                if (!string.IsNullOrEmpty(PeriodId))
                {
                    sqlText += @" AND PeriodId=@PeriodId";
                }

                #endregion SqlText

                #endregion SqlText

                sqlText += " ORDER BY Id DESC";

                #region SqlExecution

                SqlDataAdapter da = new SqlDataAdapter(sqlText, currConn);
                da.SelectCommand.Transaction = transaction;
                da.SelectCommand.CommandTimeout = 500;

                if (!string.IsNullOrEmpty(PeriodId))
                {
                    da.SelectCommand.Parameters.AddWithValue("@PeriodId", PeriodId);
                }

                da.Fill(dt);

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
                FileLogger.Log("IVAS_API", "SelectAll", sqlex.ToString() + "\n" + sqlText);
                throw new ArgumentNullException("", sqlex.Message.ToString());

            }
            catch (Exception ex)
            {
                FileLogger.Log("IVAS_API", "SelectAll", ex.ToString() + "\n" + sqlText);
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


        public string[] Delete(VAT9_1NBRApi_AttachmentVM paramVm, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            #region Initializ
            string[] retResults = new string[4];
            retResults[0] = "Fail";
            retResults[1] = "Fail";
            retResults[2] = "";
            retResults[3] = "";
            string Id = "";

            int transResult = 0;
            string sqlText = "";
            string newIDCreate = "";
            string PostStatus = "";
            int IDExist = 0;

            SqlConnection currConn = null;
            SqlTransaction transaction = null;
            DataTable table = new DataTable();
            string SqlText = "";

            #endregion Initializ

            try
            {
                #region New open
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

                #region Delete  Data

                sqlText = "";
                sqlText +=
                    @" DELETE FROM VAT9_1NBRApi_Attachment WHERE PeriodId=@PeriodId AND Notes=@Notes ";


                SqlCommand cmdDeleteDetail = new SqlCommand(sqlText, currConn, transaction);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@Notes", paramVm.Notes);
                cmdDeleteDetail.Parameters.AddWithValueAndNullHandle("@PeriodId", paramVm.PeriodId);


                transResult = cmdDeleteDetail.ExecuteNonQuery();

                #endregion


                if (Vtransaction == null)
                {
                    transaction.Commit();
                }

                #region SuccessResult
                retResults = new string[4];
                retResults[0] = "Success";
                retResults[1] = MessageVM.msgDelete;
                retResults[2] = "" + paramVm.Id;
                retResults[3] = Id;
                #endregion SuccessResult
            }
            catch (Exception e)
            {
                retResults = new string[4];
                retResults[0] = "Fail";
                retResults[1] = e.Message;
                retResults[2] = "0";
                retResults[3] = "N";

                if (Vtransaction == null)
                {
                    transaction.Rollback();
                }
                FileLogger.Log("IVAS_API", "Delete", e.ToString());
                throw;
            }
            finally
            {
                if (currConn != null && VcurrConn == null && currConn.State == ConnectionState.Open)
                {
                    currConn.Close();
                }
            }


            return retResults;
        }

        #endregion

    }
}
