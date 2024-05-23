using System;
using System.Data;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class DHLIntegration
    {
        string[] sqlResults = new string[5];

        public DataTable LoadUnprocessedData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {
                ImportDAL importDal = new ImportDAL();
                BranchProfileDAL dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(param.DefaultBranchId.ToString(), null, null, null, null, true);

                DataTable dtTableResult = importDal.GetSaleDHLAirportDbData(param.RefNo, dt);

                return dtTableResult;
            }
            #endregion

            #region catch

            catch (Exception e)
            {
                FileLogger.Log("DHLIntegration", "LoadUnprocessedData", e.ToString());

                throw e;
            }
            #endregion
             
        }

        public string[] SaveSale(DataTable salesData, IntegrationParam param, string UserId = "", SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {
                SaleDAL salesDal = new SaleDAL();

                TableValidation(salesData, param);
                sqlResults = salesDal.SaveAndProcess(salesData, () => { }, param.DefaultBranchId,"",null,null,null,null,UserId);
                UpdateOtherDB(salesData, param);

                return sqlResults;

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FileLogger.Log("DHLIntegration", "SaveSale", ex.ToString());

                throw ex;
            }
            #endregion

        }

        private void TableValidation(DataTable salesData, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {

                if (!salesData.Columns.Contains("Branch_Code"))
                {
                    var columnName = new DataColumn("Branch_Code") { DefaultValue = param.DefaultBranchCode };
                    salesData.Columns.Add(columnName);
                }

                var column = new DataColumn("SL") { DefaultValue = "" };
                var CreatedBy = new DataColumn("CreatedBy") { DefaultValue = param.CurrentUser };
                var ReturnId = new DataColumn("ReturnId") { DefaultValue = 0 };
                var BOMId = new DataColumn("BOMId") { DefaultValue = 0 };
                var TransactionType = new DataColumn("TransactionType") { DefaultValue = param.TransactionType };
                var CreatedOn = new DataColumn("CreatedOn") { DefaultValue = DateTime.Now.ToString() };

                salesData.Columns.Add(column);
                salesData.Columns.Add(CreatedBy);
                salesData.Columns.Add(CreatedOn);

                if (!salesData.Columns.Contains("ReturnId"))
                {
                    salesData.Columns.Add(ReturnId);
                }

                if (!salesData.Columns.Contains("TransactionType"))
                {
                    salesData.Columns.Add(TransactionType);
                }

                salesData.Columns.Add(BOMId);
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DHLIntegration", "TableValidation", ex.ToString());

                throw ex;
            }
            #endregion

        }

        private void UpdateOtherDB(DataTable salesData, IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {
                if (sqlResults[0].ToLower() == "success")
                {
                    ImportDAL importDal = new ImportDAL();
                    BranchProfileDAL dal = new BranchProfileDAL();

                    DataTable dt = dal.SelectAll(param.DefaultBranchId.ToString(), null, null, null, null, true);

                    DataTable table = salesData;//salesDal.GetInvoiceNoFromTemp();

                    string[] results = importDal.UpdateDHLTransactions(table, dt);

                    if (results[0].ToLower() != "success")
                    {
                        string message = "These Id failed to insert to other database\n";

                        foreach (DataRow row in table.Rows)
                        {
                            message += row["ID"] + "\n";
                        }

                        FileLogger.Log("DHL", "UpdateOtherDB", message);

                    }
                }
            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("DHLIntegration", "UpdateOtherDB", ex.ToString());

                throw ex;
            }
            #endregion

        }
    }
}