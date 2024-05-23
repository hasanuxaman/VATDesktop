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
    public interface ISaleExport
    {
        string[] SaleExportInsert(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] SaleExportPost(SaleExportVM Master, SysDBInfoVMTemp connVM = null);
        string[] SaleExportUpdate(SaleExportVM Master, List<SaleExportInvoiceVM> Details, SysDBInfoVMTemp connVM = null);
        DataTable SearchSaleExportDetailDTNew(string SaleExportNo, SysDBInfoVMTemp connVM = null);
        DataTable SearchSaleExportDTNew(string SaleExportNo, string SaleExportDateFrom, string SaleExportDateTo, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null);
    
    }
}
