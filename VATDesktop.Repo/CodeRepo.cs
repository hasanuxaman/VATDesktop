using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATViewModel.DTOs;
using VATDesktop.Repo.CodeWCF;
using VATServer.Interface;

namespace VATDesktop.Repo
{
    public class CodeRepo : ICode
    {

        CodeWCFClient wcf = new CodeWCFClient();

        public string CodeDataInsert(string CodeGroup, string CodeName, string prefix, string Lenth, SqlConnection currConn, SqlTransaction transaction, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.CodeDataInsert( CodeGroup,  CodeName,  prefix,  Lenth, connVMwcf);

                return result;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string[] CodeUpdate(List<CodeVM> codeVMs, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string codeVMswcf = JsonConvert.SerializeObject(codeVMs);
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.CodeUpdate(codeVMswcf, connVMwcf);

                string[] results = JsonConvert.DeserializeObject<string[]>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public DataSet SearchCodes(SysDBInfoVMTemp connVM = null)
        {
            try
            {
                string connVMwcf = JsonConvert.SerializeObject(connVM);

                string result = wcf.SearchCodes(connVMwcf);

                DataSet results = JsonConvert.DeserializeObject<DataSet>(result);
                return results;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    
    }
}
