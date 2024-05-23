using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library.Integration;
using VATViewModel.DTOs;

namespace VATApiRepo
{
    public class GDICWebApiRepo
    {

        public string[] SaveSaleAPI(DataTable dt, SysDBInfoVMTemp conVM)
        {
            string[] retResult = new string[6];
            try
            {
                
               GDICIntegrationDAL _Service = new GDICIntegrationDAL();

                //retResult = _Service.SaveSaleAPI(dt, conVM);

                return retResult;

            }
            catch (Exception e)
            {
                throw e;
            }
        }



    }
}
