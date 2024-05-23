using System;
using System.Data;
using VATServer.Library;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace VMSAPI
{
    public class ReceiveAPI
    {
        public string SaveReceive(string xml)
        {
            var resultSet = new DataSet();

            DataTable table = new DataTable();
            table.Columns.Add("Result");
            table.Columns.Add("Message");
            var resultXML = "";
            string transactionType = "";

            ReceiveDAL dal = new ReceiveDAL();

            try
            {
                DataSet dataSet = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                DataTable receiveData = dataSet.Tables[0];

                CommonDAL commonDal = new CommonDAL();
                string code = commonDal.settingValue("CompanyCode", "Code");

                if (receiveData == null || receiveData.Rows.Count == 0)
                {
                    table.Rows.Add("fail", "Receive Import Fail");
                    resultSet.Tables.Add(table);

                    return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                }

                if (code.ToLower() == "cp")
                {
                    if (receiveData != null && receiveData.Rows.Count > 0)
                    {
                        try
                        {
                            DataTable auditDt = receiveData.Copy();
                            var ProcessDate = new DataColumn("ProcessDate") { DefaultValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };

                            auditDt.Columns.Add(ProcessDate);

                            commonDal.BulkInsert("ReceiveAudit", auditDt);
                        }
                        catch (Exception ex)
                        {

                            #region Error log

                            try
                            {
                                ErrorLogVM evm = new ErrorLogVM();

                                evm.ImportId = "0";
                                evm.FileName = "ReceiveAudit";
                                evm.ErrorMassage = ex.Message;
                                evm.ErrorDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                evm.SourceName = "SaveReceive";
                                evm.ActionName = "ReceiveAPI";
                                evm.TransactionName = "Receive";

                                CommonDAL _cDal = new CommonDAL();

                                string[] result = _cDal.InsertErrorLogs(evm);

                            }
                            catch (Exception e)
                            {

                            }

                            #endregion

                        }
                    }
                }

                transactionType = receiveData.Rows[0]["Transection_Type"].ToString();

                receiveData.Columns.Remove("Transection_Type");

                var results = dal.SaveTempReceive(receiveData, transactionType, "API", 0, CallBack);

                if (results[0].ToLower() == "success")
                {
                    table.Rows.Add("success", "Successfully Imported.");
                    resultSet.Tables.Add(table);
                }
                else
                {
                    table.Rows.Add("fail", "Receive Import Fail");
                    resultSet.Tables.Add(table);
                }

                resultXML = OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);

                return resultXML;


            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private void CallBack()
        {

        }
    }
}