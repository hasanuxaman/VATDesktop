using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATViewModel.DTOs;

namespace VATServer.Library
{
    public class ACIIntegration
    {
        string[] sqlResults = new string[5];

        public DataTable LoadData(IntegrationParam param) 
        {
            #region try
            
            try
            {
                ImportDAL importDal = new ImportDAL();
                BranchProfileDAL dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(param.DefaultBranchId.ToString(), null, null, null, null, true);
                param.dtConnectionInfo = dt;

                DataTable dtTableResult = importDal.GetSaleACIDbData(param);

                return dtTableResult;
            }
            #endregion

            #region catch

            catch (Exception e)
            {
                FileLogger.Log("ACIIntegration", "LoadData", e.ToString());

                throw e;
            }
            #endregion

        }

        public string[] SaveSale(DataTable salesData, IntegrationParam param, string UserId = "")
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
                FileLogger.Log("ACIIntegration", "SaveSale", ex.ToString());

                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                throw ex;
            }
            #endregion

        }

        private void TableValidation(DataTable salesData, IntegrationParam param)
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

        private void UpdateOtherDB(DataTable salesData, IntegrationParam param)
        {
            if (sqlResults[0].ToLower() == "success")
            {
                ImportDAL importDal = new ImportDAL();
                BranchProfileDAL dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(param.DefaultBranchId.ToString(), null, null, null, null, true);

                DataTable table = salesData;//salesDal.GetInvoiceNoFromTemp();

                string[] results = importDal.UpdateACITransactions(table, dt, null, "SaleInvoices");

                if (results[0].ToLower() != "success")
                {
                    string message = "These Id and SalesInvoiceNo failed to insert to other database\n";

                    foreach (DataRow row in table.Rows)
                    {
                        message += row["ID"] + "-" + row["SalesInvoiceNo"] + "\n";
                    }

                    FileLogger.Log("ACI", "UpdateOtherDB", message);

                }
            }
        }
    }


    public class BCLIntegration
    {
        string[] sqlResults = new string[5];

        public DataTable LoadData(IntegrationParam param, SysDBInfoVMTemp connVM = null)
        {
            #region try

            try
            {
                ImportDAL importDal = new ImportDAL();
                BranchProfileDAL dal = new BranchProfileDAL();

                DataTable dt = dal.SelectAll(param.DefaultBranchId.ToString(), null, null, null, null, true,connVM);

                DataTable dtTableResult = importDal.GetSaleBCLDbData(param.RefNo, dt, param.FromDate, param.ToDate, param.Top);

                return dtTableResult;
            }
            #endregion

            #region catch

            catch (Exception e)
            {
                FileLogger.Log("BCLIntegration", "LoadData", e.ToString());

                throw e;
            }
            #endregion

        }

        public string[] SaveSale(DataTable salesData, IntegrationParam param, SysDBInfoVMTemp connVM = null, string UserId = "")
        {
            #region try

            try
            {
                SaleDAL salesDal = new SaleDAL();

                TableValidation(salesData, param);
                sqlResults = salesDal.SaveAndProcess(salesData, () => { }, param.DefaultBranchId,"",null,null,null,null,UserId);

                return sqlResults;

            }
            #endregion

            #region catch

            catch (Exception ex)
            {
                FileLogger.Log("BCLIntegration", "SaveSale", ex.ToString());

                //MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                throw ex;
            }
            #endregion

        }

        private void TableValidation(DataTable salesData, IntegrationParam param)
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


    }
}
