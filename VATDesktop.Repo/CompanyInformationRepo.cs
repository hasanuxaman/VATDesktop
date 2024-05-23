using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.CompanyInformationWCF;

namespace VATDesktop.Repo
{
    public class CompanyInformationRepo
    {
        CompanyInformationWCFClient wcf = new CompanyInformationWCFClient();

        public List<SymphonyVATSysCompanyInformationVM> DropDown(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.DropDown(connVMwcf);
                List<SymphonyVATSysCompanyInformationVM> results = JsonConvert.DeserializeObject<List<SymphonyVATSysCompanyInformationVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<SymphonyVATSysCompanyInformationVM> SelectAll(string CompanyID = null, string[] conditionFields = null,
            string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null,
            SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string conditionFieldswcf = JsonConvert.SerializeObject(conditionFields);
                string conditionValueswcf = JsonConvert.SerializeObject(conditionValues);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SelectAll(CompanyID , conditionFieldswcf , conditionValueswcf,connVMwcf);
                List<SymphonyVATSysCompanyInformationVM> results = JsonConvert.DeserializeObject<List<SymphonyVATSysCompanyInformationVM>>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
