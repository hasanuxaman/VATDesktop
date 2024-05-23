using System;
using System.Data;
using VATServer.Library;
using VATServer.Ordinary;

namespace VMSAPI
{
    public class IssueAPI
    {
        public string SaveIssue(string xml)
        {
            var resultSet = new DataSet();

            DataTable table = new DataTable();
            table.Columns.Add("Result");
            table.Columns.Add("Message");
            var resultXML = "";
            string transactionType = "";
            IssueDAL dal = new IssueDAL();


            try
            {
                DataSet dataSet = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                DataTable issueData = dataSet.Tables[0];

                if (issueData == null || issueData.Rows.Count == 0)
                {
                    table.Rows.Add("fail", "Issue Import Fail");
                    resultSet.Tables.Add(table);

                    return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                }


                transactionType = issueData.Rows[0]["Transection_Type"].ToString();

                issueData.Columns.Remove("Transection_Type");


                var results = dal.SaveTempIssue(issueData, transactionType, "API", 0, CallBack);

                if (results[0].ToLower() == "success")
                {
                    table.Rows.Add("success", "Successfully Imported.");
                    resultSet.Tables.Add(table);
                }
                else
                {
                    table.Rows.Add("fail", "Issue Import Fail");
                    resultSet.Tables.Add(table);
                }

                resultXML = OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);

                return resultXML;


            }
            catch (Exception e)
            {
                FileLogger.LogWeb("Issue", "SaveIssue", e.ToString());

                throw;
            }
        }


        private void CallBack()
        {
        }
    }
}