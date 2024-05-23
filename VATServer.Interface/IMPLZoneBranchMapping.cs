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
    public interface IMPLZoneBranchMapping
    {

        string[] InsertToMPLZoneBranchMapping(MPLZoneBranchMappingVM vm, SysDBInfoVMTemp connVM = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool IsIntegrationAutoCode = false);

        string[] UpdateMPLZoneBranchMapping(MPLZoneBranchMappingVM vm, SysDBInfoVMTemp connVM = null);

        string[] Delete(MPLZoneBranchMappingVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        DataTable GetExcelData(List<string> ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLZoneBranchMappingVM> DropDownAll(SysDBInfoVMTemp connVM = null);

        List<MPLZoneBranchMappingVM> DropDown(SysDBInfoVMTemp connVM = null);

        List<MPLZoneBranchMappingVM> SelectAllList(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);

        List<MPLZoneBranchMappingVM> GetZoneCodeWise(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null);


    }
}
