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
    public interface IBOM
    {

        DataTable SearchBOMMasterNew(string BOMId, SysDBInfoVMTemp connVM = null);

        DataTable SearchBOMRawNew(string BOMId, SysDBInfoVMTemp connVM = null);
        
        DataTable SearchOHNew(string BOMId, SysDBInfoVMTemp connVM = null);

        string[] BOMInsert(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string[] BOMUpdate(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string[] DeleteBOM(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        string[] BOMPost(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null, SqlTransaction vtransaction = null, SqlConnection vcurrConn = null);

        string[] FindCostingFrom(string itemNo, string effectDate, SqlConnection curConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, 
            SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, bool Dt = false, SysDBInfoVMTemp connVM = null);

        List<BOMNBRVM> SelectAllList(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, BOMNBRVM likeVM = null, SysDBInfoVMTemp connVM = null);

        DataTable SearchBOMMaster(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        DataTable SearchBOMRaw(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        DataTable SearchOH(string finItem, string vatName, string effectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        string[] ServiceInsert(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null);

        string[] ServiceUpdate(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null);

        string[] ServicePost(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null);

        string[] ServiceDelete(List<BOMNBRVM> Details, SysDBInfoVMTemp connVM = null);

        DataTable SearchInputValues(string FinishItemName, string EffectDate, string VATName, string post,
                                    string FinishItemNo, SysDBInfoVMTemp connVM = null);

        DataTable SearchServicePrice(string BOMId, SysDBInfoVMTemp connVM = null);

        DataTable CustomerByBomId(string BOMId, SysDBInfoVMTemp connVM = null);

        DataTable UseQuantityDT(string FinishItemNo, decimal Quantity, string EffectDate, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        List<BOMItemVM> SelectAllItems(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<BOMOHVM> SelectAllOverheads(string BOMId = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        string[] BOMPreInsert(BOMNBRVM vm, SysDBInfoVMTemp connVM = null);

        string FindBOMIDOverHead(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);

        string[] BOMImport(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string[] BOMInsertX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string[] BOMInsert2(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SqlTransaction transaction = null, SqlConnection currConn = null, SysDBInfoVMTemp connVM = null);

        string[] BOMUpdateX(List<BOMItemVM> bomItems, List<BOMOHVM> bomOHs, BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string[] BOMPostX(BOMNBRVM bomMaster, SysDBInfoVMTemp connVM = null);

        string FindBOMID(string itemNo, string VatName, string effectDate, SqlConnection currConn, SqlTransaction transaction, string CustomerID = "0", SysDBInfoVMTemp connVM = null);












    }
}
