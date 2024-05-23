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
    public interface ICode
    {


        string CodeDataInsert(string CodeGroup, string CodeName, string prefix, string Lenth, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null);
        string[] CodeUpdate(List<CodeVM> codeVMs, SysDBInfoVMTemp connVM = null);
        DataSet SearchCodes(SysDBInfoVMTemp connVM = null);
    

    }
}
