using SymphonySofttech.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATServer.Library;
using VATServer.Library.Integration;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using VATViewModel.Integration.NBRAPI;

namespace VATClient.Integration.NBR
{
    public partial class FormNBRAPI : Form
    {
        public FormNBRAPI()
        {
            InitializeComponent();
            connVM = Program.OrdinaryLoad();
        }
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        ResultVM vm = new ResultVM();
        NBR_APIVM _api = new NBR_APIVM();

        private const string END_POINT =
            @"http://175.29.140.41:55000/XISOAPAdapter/MessageServlet?senderParty=&senderService=BC_SOAP_BIDA_TO_IVAS&receiverParty=&receiverService=&interface=SI_RET_SENDER&interfaceNamespace=http://nbr.gov.bd/regist91";

        private void btnXML_Click(object sender, EventArgs e)
        {
            try
            {
                progressBar2.Visible = true;

                _api.CurrentUserId = Program.CurrentUserID;
                _api.Period = dptMonth.Value;
                _api.TransactionType = "9.1";

                #region RQ Documents

                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "01",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Treasury Challan Copy (Not applicable, if taxpayer paid via e-payment)"
                //});               
                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "02",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.1"
                //});                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "03",
                //    CHKBOX = "X",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.2.1"
                //});                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "04",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.6 [Mandatory, if taxpayer input data in Note 24 and/or 29 (VDS)]"
                //});             
                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "05",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.10 (Mandatory, if any transaction is above 2 lakh)"
                //});                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "07",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.7 [Mandatory, if taxpayer input data in Note 31]"
                //});                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "08",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Mushak-6.8 [Mandatory, if taxpayer input data in Note 26]"
                //});               
                
                //_api.CheckedDocuments.Add(new DocumentVM()
                //{
                //    ATT_DOCTYPE = "06",
                //    CHKBOX = "",
                //    NOTES = "-",
                //    TEXT = "Any Other Documents"
                //});


                //_api.CheckedDocuments.ForEach(x =>
                //{
                //    if (x.ATT_DOCTYPE == "06")
                //    {
                //        x.CHKBOX = "X";
                //    }
                //});

                #endregion

                //List<AttachedFileVM> files = SelectFiles();

                //_api.AttachedFiles = files;

                bgwLoadXML.RunWorkerAsync();

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormNBRAPI", "GETXML", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwLoadXML_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _9_1_VATReturnDAL vatReturn = new _9_1_VATReturnDAL();

                vm = vatReturn.Get9_1NBR_XML(_api);

            }
            catch (Exception exception)
            {
                vm.Status = "Fail";
                vm.Message = exception.Message;
            }
        }

        private void bgwLoadXML_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (vm.IsSuccess)
                {
                    txtXMLText.Text = vm.XML;

                    string formattedXMl = OrdinaryVATDesktop.PrintXML(vm.XML);

                    OrdinaryVATDesktop.SaveTextFile(formattedXMl, "NBR_API_XML", ".xml");
                }
                else
                {
                    FormErrorMessage formErrorMessage = new FormErrorMessage();
                    formErrorMessage.ShowDetails_NS(vm.ErrorList);
                    formErrorMessage.ShowDialog();
                }

                //txtXMLText.Text = vm.XML;

                //string formattedXMl1 = OrdinaryVATDesktop.PrintXML(vm.XML);

                //OrdinaryVATDesktop.SaveTextFile(formattedXMl1, "NBR_API_XML", ".xml");

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar2.Visible = false;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string requestMesg = GetSampleXML();

                WebRequest request = (HttpWebRequest)WebRequest.Create(END_POINT);
                request.Method = "POST";

                byte[] byteArray = Encoding.UTF8.GetBytes(requestMesg);
                request.ContentLength = byteArray.Length;

                request.ContentType = "text/xml;charset=UTF-8";

                NetworkCredential creds = new NetworkCredential("user01", "Ivas@2020");
                request.Credentials = creds;

                Stream datastream = request.GetRequestStream();
                datastream.Write(byteArray, 0, byteArray.Length);
                datastream.Close();

                WebResponse response = request.GetResponse();
                datastream = response.GetResponseStream();

                StreamReader reader = new StreamReader(datastream);

                string responseMessage = reader.ReadToEnd();

                reader.Close();

                MessageBox.Show(responseMessage);


            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }


        private void btnFile_Click(object sender, EventArgs e)
        {
            try
            {
                SelectFiles();
            }
            catch (Exception exception)
            {
                FileLogger.Log("FormNBR", "file select", exception.ToString());
                MessageBox.Show(exception.ToString());

            }
        }

        private List<AttachedFileVM> SelectFiles()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.RestoreDirectory = true;
            fileDialog.Multiselect = true;

            FileInfo fileInfo = null;

            List<AttachedFileVM> attachedFiles = new List<AttachedFileVM>();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (String file in fileDialog.FileNames)
                {
                    fileInfo = new FileInfo(file);

                    long size = (fileInfo.Length / 1024) / 1024;

                    if (size > 4)
                    {
                        throw new Exception(fileInfo.Name + " is over 4 MB");
                    }

                    byte[] bytes = File.ReadAllBytes(file);

  
                    AttachedFileVM item = new AttachedFileVM()
                    {
                        FILENAME = Path.GetFileNameWithoutExtension(file),
                        FILETYPE = fileInfo.Extension.Replace(".",""),
                        CONTENT = Convert.ToBase64String(bytes)
                    };
                    attachedFiles.Add(item);
                }
            }

            return attachedFiles;
        }


        private string GetSampleXML()
        {
            string xml = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:reg=""http://nbr.gov.bd/regist91"">
   <soapenv:Header/>
   <soapenv:Body>
      <reg:MT_RET_MSG_REQ>
         <I_MSG_HDR>
            <MSGID>M9102020122200000116</MSGID>
            <TRANS_CODE>R910</TRANS_CODE>
            <SEND_DATE>22/12/2020</SEND_DATE>
            <SEND_TIME>00:00:00</SEND_TIME>
         </I_MSG_HDR>
         <I_MAIN_FORM>
            <BIN>000143369</BIN>
            <PERIOD_KEY>1910</PERIOD_KEY>
            <A_F_S2_TYPE_RETURN>1</A_F_S2_TYPE_RETURN>
            <A_F_S2_ANY_ACTIVITIES>1</A_F_S2_ANY_ACTIVITIES>
            <A_F_S2_SUBMISSION_DAT>20.11.2020</A_F_S2_SUBMISSION_DAT>
            <A_F_S5_PAYMENT_NOT_BANKING_AMT>100000.33</A_F_S5_PAYMENT_NOT_BANKING_AMT>
            <A_F_S5_ISSUANCE_DEBIT_AMT>200000.33</A_F_S5_ISSUANCE_DEBIT_AMT>
            <A_F_S5_OTHER_ADJUST_AMT>84562.30</A_F_S5_OTHER_ADJUST_AMT>
            <A_F_S6_ISSUANCE_CREDIT_AMT>78952.00</A_F_S6_ISSUANCE_CREDIT_AMT>
            <A_F_S6_OTHER_ADJUST_AMT>962222.01</A_F_S6_OTHER_ADJUST_AMT>
            <A_F_S7_SD_AGAINST_DEBIT_NOTE>7895555.32</A_F_S7_SD_AGAINST_DEBIT_NOTE>
            <A_F_S7_SD_AGAINST_CREDIT_NOTE>3634634</A_F_S7_SD_AGAINST_CREDIT_NOTE>
            <A_F_S7_SD_AGAINST_EXPORT>346436</A_F_S7_SD_AGAINST_EXPORT>
            <A_F_S7_FINE_PENALTY_OTHER>10000</A_F_S7_FINE_PENALTY_OTHER>
            <A_F_S7_EXCISE_DUTY>8955555.22</A_F_S7_EXCISE_DUTY>
            <A_F_S7_DEVELOP_SURCHARGE>854745</A_F_S7_DEVELOP_SURCHARGE>
            <A_F_S7_ICT_DEVELOP_SURCHARGE>4533</A_F_S7_ICT_DEVELOP_SURCHARGE>
            <A_F_S7_HEALTH_CARE_SURCHARGE>236626</A_F_S7_HEALTH_CARE_SURCHARGE>
            <A_F_S7_ENV_PROTECT_SURCHARGE>679769</A_F_S7_ENV_PROTECT_SURCHARGE>
            <A_F_S7_CB_LAST_TP_VAT>0</A_F_S7_CB_LAST_TP_VAT>
            <A_F_S7_CB_LAST_TP_SD>0</A_F_S7_CB_LAST_TP_SD>
            <A_F_S11_REFUND_VAT>2</A_F_S11_REFUND_VAT>
            <A_F_S11_REFUND_SD></A_F_S11_REFUND_SD>
            <A_F_S10_REFUND></A_F_S10_REFUND>
            <A_F_S10_NAME>Vientt</A_F_S10_NAME>
            <A_F_S10_DESIGNATION>Owner</A_F_S10_DESIGNATION>
            <A_F_S10_MOBILE>02345678910</A_F_S10_MOBILE>
            <A_F_S10_NID_PP>23523532</A_F_S10_NID_PP>
            <A_F_S10_EMAIL>vientt@vat.com.vn</A_F_S10_EMAIL>
            <A_F_S10_CONFIRM>X</A_F_S10_CONFIRM>
            <A_F_S7_INTEREST_OVERDUE_VAT>30000</A_F_S7_INTEREST_OVERDUE_VAT>
            <A_F_S7_INTEREST_OVERDUE_SD>2000</A_F_S7_INTEREST_OVERDUE_SD>
            <A_F_S7_FINE_PENALTY>20000</A_F_S7_FINE_PENALTY>
            <A_F_S5_OTHER_ADJUST_CMT>Adjust for Deecember,2020</A_F_S5_OTHER_ADJUST_CMT>
            <A_F_S6_OTHER_ADJUST_CMT>Adjust for Octocber,2020</A_F_S6_OTHER_ADJUST_CMT>
         </I_MAIN_FORM>
         <IT_SUBF_GOSERV>
            <ITEM>
               <FIELD_ID>SL01</FIELD_ID>
               <!--Optional:-->
               <GOOD_SERVICE>0101.21.00</GOOD_SERVICE>
               <VALUE>10000.00</VALUE>
               <SD></SD>
               <VAT></VAT>
               <DESCRIPTION>Test</DESCRIPTION>
               <NOTES>Test Note 01</NOTES>
               <QUANTITY></QUANTITY>
               <ACT_SALE_VALUE></ACT_SALE_VALUE>
               <!--Optional:-->
               <UNIT></UNIT>
               <ITEM_ID>0000035000</ITEM_ID>
               <!--Optional:-->
               <NAME>Pure-bred breeding animals of horses..</NAME>
               <CATEGORY></CATEGORY>
            </ITEM>
            <ITEM>
               <FIELD_ID>SL06</FIELD_ID>
               <!--Optional:-->
               <GOOD_SERVICE>0101.21.00</GOOD_SERVICE>
               <VALUE></VALUE>
               <SD></SD>
               <VAT></VAT>
               <DESCRIPTION></DESCRIPTION>
               <NOTES></NOTES>
               <QUANTITY>20</QUANTITY>
               <ACT_SALE_VALUE>20000.20</ACT_SALE_VALUE>
               <!--Optional:-->
               <UNIT>01</UNIT>
               <ITEM_ID>0000045262</ITEM_ID>
               <!--Optional:-->
               <NAME>Pure-bred breeding animals of horses..</NAME>
               <CATEGORY></CATEGORY>
            </ITEM>
            <ITEM>
               <FIELD_ID>SL08</FIELD_ID>
               <!--Optional:-->
               <GOOD_SERVICE>0101.21.00</GOOD_SERVICE>
               <VALUE>456899</VALUE>
               <SD></SD>
               <VAT></VAT>
               <DESCRIPTION></DESCRIPTION>
               <NOTES></NOTES>
               <QUANTITY></QUANTITY>
               <ACT_SALE_VALUE></ACT_SALE_VALUE>
               <!--Optional:-->
               <UNIT></UNIT>
               <ITEM_ID>0000045262</ITEM_ID>
               <!--Optional:-->
               <NAME>Pure-bred breeding animals of horses..</NAME>
               <CATEGORY>01</CATEGORY>
               <ITEM>
               <FIELD_ID>SL10</FIELD_ID>
               <!--Optional:-->
               <GOOD_SERVICE>0101.21.00</GOOD_SERVICE>
               <VALUE>5460000.00</VALUE>
               <SD></SD>
               <VAT></VAT>
               <DESCRIPTION>Test</DESCRIPTION>
               <NOTES>Test Note</NOTES>
               <QUANTITY></QUANTITY>
               <ACT_SALE_VALUE></ACT_SALE_VALUE>
               <!--Optional:-->
               <UNIT></UNIT>
               <ITEM_ID>0000000001</ITEM_ID>
               <!--Optional:-->
               <NAME>Pure-bred breeding animals of horses..</NAME>
               <CATEGORY></CATEGORY>
            </ITEM>
            </ITEM>
         </IT_SUBF_GOSERV>
         <IT_SUBF_VDS>
            <ITEM>
               <FIELD_ID>SL25</FIELD_ID>
               <BIN>123456789</BIN>
               <NAME>Shekh Sumon</NAME>
               <ADDRESS>06/08 Dahanmodi, Dhaka,Bangladesh</ADDRESS>
               <VALUE>7896666.23</VALUE>
               <DEDUCT_VAT>5633</DEDUCT_VAT>
               <INVOICE_NO>AA/590</INVOICE_NO>
               <INVOICE_DATE>25.12.2020</INVOICE_DATE>
               <CERT_NO>EF20</CERT_NO>
               <CERT_DATE>25.12.2020</CERT_DATE>
               <ACCOUNT_CODE>1/1133/0010/1141</ACCOUNT_CODE>
               <DEPOSIT_NO>DE123</DEPOSIT_NO>
               <DEPOSIT_DATE>25.12.2020</DEPOSIT_DATE>
               <NOTE></NOTE>
            </ITEM>
         </IT_SUBF_VDS>
         <IT_SUBF_BOE>
            <ITEM>
               <FIELD_ID>SL31</FIELD_ID>
               <BOE>636</BOE>
               <BOE_DATE>20.11.2020</BOE_DATE>
               <CUS_STATION>01</CUS_STATION>
               <ATV_AMOUNT>2360000</ATV_AMOUNT>
               <NOTES>Testtt</NOTES>
            </ITEM>
         </IT_SUBF_BOE>
         <IT_SUBF_CHALLAN>
            <ITEM>
               <FIELD_ID>SL53</FIELD_ID>
               <CHALL_NO>346346</CHALL_NO>
               <CHALL_DATE>20.11.2020</CHALL_DATE>
               <ACCOUNT_CODE>1/1133/0010/1141</ACCOUNT_CODE>
               <CHAN_AMT>8922222</CHAN_AMT>
               <NOTES>Bangladesh</NOTES>
               <BANCD>055</BANCD>
               <BANKL>055260437</BANKL>
               <A_NAMEOFBANK>Bangladesh Bank</A_NAMEOFBANK>
               <A_BRANCH>Banani</A_BRANCH>
            </ITEM>
            <ITEM>
               <FIELD_ID>SL60</FIELD_ID>
               <CHALL_NO>346346</CHALL_NO>
               <CHALL_DATE>20.11.2020</CHALL_DATE>
               <ACCOUNT_CODE>1/1133/0010/1141</ACCOUNT_CODE>
               <CHAN_AMT>8922222</CHAN_AMT>
               <NOTES>Bangladesh</NOTES>
               <BANCD>055</BANCD>
               <BANKL>055260437</BANKL>
               <A_NAMEOFBANK>Bangladesh Bank</A_NAMEOFBANK>
               <A_BRANCH>Banani</A_BRANCH>
            </ITEM>
         </IT_SUBF_CHALLAN>
         <IT_SUBF_ATTACH>
            <ITEM>
               <ATT_DOCTYPE>01</ATT_DOCTYPE>
               <TEXT>Treasury Challan Copy (Not applicable, if taxpayer paid via e-payment) </TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>02</ATT_DOCTYPE>
               <TEXT>Mushak-6.1</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>03</ATT_DOCTYPE>
               <TEXT>Mushak-6.2.1</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>04</ATT_DOCTYPE>
               <TEXT>Mushak-6.6 [Mandatory, if taxpayer input data in Note 24 and/or 29 (VDS)]</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>05</ATT_DOCTYPE>
               <TEXT>Mushak-6.10 (Mandatory, if any transaction is above 2 lakh)</TEXT>
               <NOTES></NOTES>
               <CHKBOX>X</CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>07</ATT_DOCTYPE>
               <TEXT>Mushak-6.7 [Mandatory, if taxpayer input data in Note 31]</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>08</ATT_DOCTYPE>
               <TEXT>Mushak-6.8 [Mandatory, if taxpayer input data in Note 26]</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
            <ITEM>
               <ATT_DOCTYPE>06</ATT_DOCTYPE>
               <TEXT>Any Other Documents</TEXT>
               <NOTES></NOTES>
               <CHKBOX></CHKBOX>
            </ITEM>
         </IT_SUBF_ATTACH>
         <ATTACH_DOCUMENT>
            <ITEM>
               <FILENAME>118.jpg</FILENAME>";

            if (!string.IsNullOrEmpty(txtXMLText.Text))
            {
                xml = txtXMLText.Text.Trim();

            }

            return xml;
        }

        private void FormNBRAPI_Load(object sender, EventArgs e)
        {

        }

        private void btnJson_Click(object sender, EventArgs e)
        {
            try
            {

                string Message = "Do you want to send VAT return (9.1) for month of " + dptMonth.Value.ToString("MMMM-yyyy");
                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }

                string PeriodName = dptMonth.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new ArgumentException(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost != "Y")
                {
                    throw new ArgumentException(PeriodName + ": VAT Return (9.1) not posted!");
                }

                progressBar2.Visible = true;
                btnJson.Enabled = false;
                btnSubmit.Enabled = false;

                _api.CurrentUserId = Program.CurrentUserID;
                _api.Period = dptMonth.Value;
                _api.TransactionType = "9.1";

                bgwLoadJson.RunWorkerAsync();

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormNBRAPI", "btnJson_Click", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwLoadJson_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _9_1_VATReturnDAL vatReturn = new _9_1_VATReturnDAL();
                _9_1NBR_API_DAL vatReturnAPI = new _9_1NBR_API_DAL();

                IVAS_API VATAPI = new IVAS_API();

                vm = VATAPI.Get9_1NBR_Json(_api, connVM);

                ////vm = vatReturnAPI.Get9_1NBR_Json(_api);

                ////vm = vatReturn.Get9_1NBR_XML(_api);

                ////progressBar2.Visible = false;


            }
            catch (Exception exception)
            {
                vm.Status = "Fail";
                vm.Message = exception.Message;

                //progressBar2.Visible = false;

            }
        }

        private void bgwLoadJson_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (vm.IsSuccess)
                {
                    MessageBox.Show("VAT return(9.1) send to IVAS successfully");

                    //txtXMLText.Text = vm.Json;

                    ////string formattedXMl = OrdinaryVATDesktop.PrintXML(vm.Json);

                    OrdinaryVATDesktop.SaveIVASJsonTextFile(vm.Json, "NBR_API_Json", ".json");
                }
                else
                {
                    FormErrorMessage formErrorMessage = new FormErrorMessage();
                    formErrorMessage.ShowDetails_NS(vm.ErrorList);
                    formErrorMessage.ShowDialog();
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar2.Visible = false;
                btnJson.Enabled = true;
                btnSubmit.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IVAS_API VATAPI = new IVAS_API();

            VATReturnSubFormVM subformVm = new VATReturnSubFormVM();

            string MSGID = "";

            subformVm.PeriodName = dptMonth.Value.ToString("MMMM-yyyy");
            subformVm.ExportInBDT = "Y";
            subformVm.post1 = "Y";
            subformVm.post2 = "Y";
            subformVm.IsSummary = true;
            subformVm.PeriodId = dptMonth.Value.ToString("MMyyyy");
            subformVm.PeriodKey = dptMonth.Value.ToString("MMyy");

            subformVm.NoteNos = new[] { 24, 26, 27, 29, 31, 32 };

            List<Return_9_1_SF_att_fileSet>  _fileSetVMs = VATAPI.SetAttachments(subformVm, MSGID, connVM);


        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {

                string Message = "Do you want to submit VAT return (9.1) for month of " + dptMonth.Value.ToString("MMMM-yyyy");
                DialogResult dlgRes = MessageBox.Show(Message, this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dlgRes != DialogResult.Yes)
                {
                    return;
                }

                string PeriodName = dptMonth.Value.ToString("MMMM-yyyy");
                string[] cValues = { PeriodName };
                string[] cFields = { "PeriodName" };
                FiscalYearVM varFiscalYearVM = new FiscalYearVM();
                varFiscalYearVM = new FiscalYearDAL().SelectAll(0, cFields, cValues, null, null, connVM).FirstOrDefault();

                if (varFiscalYearVM == null)
                {
                    throw new ArgumentException(PeriodName + ": This Fiscal Period is not Exist!");
                }

                if (varFiscalYearVM.VATReturnPost != "Y")
                {
                    throw new ArgumentException(PeriodName + ": VAT Return (9.1) not posted!");
                }

                progressBar2.Visible = true;
                btnJson.Enabled = false;
                btnSubmit.Enabled = false;

                _api.CurrentUserId = Program.CurrentUserID;
                _api.Period = dptMonth.Value;
                _api.TransactionType = "9.1";

                bgwSubmitAPI.RunWorkerAsync();

            }
            catch (Exception exception)
            {
                FileLogger.Log("FormNBRAPI", "btnSubmit_Click", exception.ToString());
                MessageBox.Show(exception.Message);
            }
        }

        private void bgwSubmitAPI_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _9_1_VATReturnDAL vatReturn = new _9_1_VATReturnDAL();
                _9_1NBR_API_DAL vatReturnAPI = new _9_1NBR_API_DAL();

                IVAS_API VATAPI = new IVAS_API();

                vm = VATAPI.VAT9_1Submit(_api, connVM);

            }
            catch (Exception exception)
            {
                vm.Status = "Fail";
                vm.Message = exception.Message;

                //progressBar2.Visible = false;

            }
        }

        private void bgwSubmitAPI_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (vm.IsSuccess)
                {
                    MessageBox.Show("VAT return(9.1) Submit successfully. Submission ID : " + vm.SubmissionId);
                }
                else
                {
                    FormErrorMessage formErrorMessage = new FormErrorMessage();
                    formErrorMessage.ShowDetails_NS(vm.ErrorList);
                    formErrorMessage.ShowDialog();
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                progressBar2.Visible = false;
                btnJson.Enabled = true;
                btnSubmit.Enabled = true;
            }
        }

       
    }
}
