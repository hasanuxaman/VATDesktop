using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;

namespace VATServer.Interface
{
    public interface IBranch
    {

        string[] Delete(string Name, SysDBInfoVMTemp connVM = null);

        string[] UpdateBranchName(BranchVM vm, SysDBInfoVMTemp connVM = null);

        string[] InsertBranchName(BranchVM vm, SysDBInfoVMTemp connVM = null);

        List<BranchVM> DropDown(SysDBInfoVMTemp connVM = null);

        DataTable SearchBranchName(string Name, SysDBInfoVMTemp connVM = null);

        DataTable SearchBranchNameByParam(string Name = null, string DBName = null, SysDBInfoVMTemp connVM = null);

    }
}
