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
    public interface ICompanyprofile
    {

       List<CompanyProfileVM> SelectAllList(string Id = null, string[] conditionFields = null
            , string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null
            , SysDBInfoVMTemp connVM = null);

        DataTable SearchCompanyProfile(SysDBInfoVMTemp connVM = null);

        DataTable SelectAll(string Id = null, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, bool Dt = true, SysDBInfoVMTemp connVM = null);

        string[] UpdateCompanyProfileNew(CompanyProfileVM companyProfiles, SysDBInfoVMTemp connVM = null);


    }
}
