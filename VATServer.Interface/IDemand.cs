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
    public interface IDemand
    {

        decimal ReturnQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null);
        decimal BanderolStock(string itemNo, string BandProductID, string tranDate, SysDBInfoVMTemp connVM = null);
        string[] DemandInsert(DemandMasterVM Master, List<DemandDetailVM> Details, SqlTransaction transaction, SqlConnection currConn, SysDBInfoVMTemp connVM = null);
        string[] DemandPost(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null);
        string[] DemandUpdate(DemandMasterVM Master, List<DemandDetailVM> Details, SysDBInfoVMTemp connVM = null);
        DataTable SearchDemandDetailDTNew(string DemandNo, SysDBInfoVMTemp connVM = null);
        DataTable SearchDemandHeaderDTNew(string DemandNo, string DemandDateFrom, string DemandDateTo, string TransactionType, string Post, int BranchId = 0, SysDBInfoVMTemp connVM = null);
        DataTable DemandQty(string demandId, string bandeProId, string post, SysDBInfoVMTemp connVM = null);
    

    }
}
