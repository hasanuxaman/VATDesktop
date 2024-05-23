using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IDutyDrawBack
    {

        string[] DutyDrawBacknsert(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null);
        string[] DutyDrawBackPost(DDBHeaderVM Master, List<DDBDetailsVM> Details, SysDBInfoVMTemp connVM = null);
        string[] DutyDrawBackUpdate(DDBHeaderVM Master, List<DDBDetailsVM> Details, List<DDBSaleInvoicesVM> ddbSaleInvoices, SysDBInfoVMTemp connVM = null);
        DataTable Purchase_DDBQty(string PurchaseInvoiceNo, string PurItemNo, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        DataTable SearchddBackDetails(string DDBackNo, string oldSaleID, SysDBInfoVMTemp connVM = null);
        DataTable SearchDDBackHeader(string DDBackNo, string DDBackFromDate, string DDBackToDate, string DDBackSaleFromDate, string DDBackSaleToDate, string SalesInvoicNo, string CustomerName, string FinishGood, string Post, string BranchId = "0", string TransactionType = "DDB", SysDBInfoVMTemp connVM = null);
        DataTable SearchddbSaleInvoices(string DDBackNo, SysDBInfoVMTemp connVM = null);
        DataTable VAT7_1(string ddbackno, string salesInvoice, SysDBInfoVMTemp connVM = null);
    

    }
}
