using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IDispose
    {


        string[] DisposeInsert(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null);
        string[] DisposePost(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null);
        string[] DisposeUpdate(DisposeMasterVM Master, List<DisposeDetailVM> Details, SysDBInfoVMTemp connVM = null);
        DataTable SearchDisposeDetailDTNew(string DisposeNumber, SysDBInfoVMTemp connVM = null);
        DataTable SearchDisposeHeaderDTNew(string DisposeNumber, string DisposeDateFrom, string DisposeDateTo, string transactionType, string Post, string databasename, int BranchId, SysDBInfoVMTemp connVM = null);
    

    }
}
