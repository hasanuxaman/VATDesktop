using System;
using System.Data;
using System.IO;
using VATServer.Library;
using VATServer.Ordinary;

namespace VMSAPI
{
    public class TransferIssueAPI
    {
        public string SaveTransferIssue(string xml)
        {
            var resultSet = new DataSet();

            DataTable table = new DataTable();
            table.Columns.Add("Result");
            table.Columns.Add("Message");
            var resultXML = "";

            TransferIssueDAL dal = new TransferIssueDAL();

            try
            {
                DataSet dataSet = OrdinaryVATDesktop.GetDataSetFromXML(xml);

                DataTable transferIssueData = dataSet.Tables[0];

                if (transferIssueData == null || transferIssueData.Rows.Count == 0)
                {
                    table.Rows.Add("fail", "Transfer Issue Import Fail");
                    resultSet.Tables.Add(table);

                    return OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                }



                var results = dal.SaveTempTransfer(transferIssueData, "", "", "API", 0, CallBack);

                if (results[0].ToLower() == "success")
                {
                    table.Rows.Add("success", "Successfully Imported.");
                    resultSet.Tables.Add(table);
                }
                else
                {
                    table.Rows.Add("fail", "Transfer Issue Import Fail");
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

        public string GetFileName(string saleXML)
        {
            try
            {
                var dataSet = new DataSet();
                StringReader reader = new StringReader(saleXML);
                BranchProfileDAL branchDAL = new BranchProfileDAL();

                dataSet.ReadXml(reader);

                DataTable table = dataSet.Tables[0];
                
                DataSet resultSet = new DataSet();

                DataTable resTable = new DataTable();
                resTable.Columns.Add("Result");
                resTable.Columns.Add("Message");
                resTable.Columns.Add("FileName");

                #region File Name Generation

                DataView dataView = new DataView(table);

                DataTable refs = dataView.ToTable(true, "ReferenceNo");

                string fileName = "";

                foreach (DataRow dataRow in refs.Rows)
                {
                    fileName += dataRow["ReferenceNo"] + ",";
                }

                fileName = fileName.TrimEnd(',');

                #endregion

                string resultXML = "";

                #region Convert to XML

                MemoryStream memoryStream = new MemoryStream();

                resTable.Rows.Add("success", "Successfully Imported.", fileName);

                resultSet.Tables.Add(resTable);

                resultSet.WriteXml(memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);

                StreamReader sr = new StreamReader(memoryStream);

                resultXML = sr.ReadToEnd();

                #endregion

                return resultXML;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void CallBack()
        {
        }
    }
}