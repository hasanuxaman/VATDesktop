using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATServer.License
{
  public  class LicenseExpite
    {
        public static string ExpiteDate(string CompanyName, string CompanyCode=null)
        {
            //kamrul

            string LicenseDate = "20220225";
            string LicenseExpiteDate = "20240723";
            string NestleLicenseExpiteDate = "20240723";
            if (!string.IsNullOrWhiteSpace(CompanyCode) && CompanyCode.ToUpper()=="NESTLE")
            {
                LicenseDate = NestleLicenseExpiteDate;
                                                 
            }
            else if (CompanyName.ToLower().Contains("demo")
                          || CompanyName == "My_Company_DB"
                          || CompanyName == "NP_DB"
                          || CompanyName == "B1_DB"
                          || CompanyName == "SCL_B1_DB"
                          || CompanyName == "PLTLOld_DB"
                )

            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "PCCL_RND_DB" // CPB_DB
                    || CompanyName == "PCCL_RND_DB"
                )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "CPB_DB" // CPB_DB
              || CompanyName == "CPB_DB"
              || CompanyName == "CPB_2012__DB"
               )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "RTCL_DB" // Rupsha
            )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName.ToUpper() == "NITA_DB"
                      )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "PCCL_DB" // Padma Group
                          || CompanyName == "PCCL_DB"
                          || CompanyName == "PCL_DB"
                          || CompanyName == "PLTL_DB"
                           )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "HO_DB" // HO_DB
             || CompanyName == "HO_DB"
              )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "APPL_DB" // Arbab Poly
        || CompanyName == "APPL_DB"
        || CompanyName == "Arbab_DB"
         )
            {
                LicenseDate = LicenseExpiteDate;
            }
            else if (CompanyName == "Demo_DB" // Demo_DB
             || CompanyName == "Demo_DB"
             || CompanyName == "Matador_Group_DB"
             || CompanyName == "MBPI_Demo_DB"
             || CompanyName == "BSCL_DB"
             || CompanyName == "BSCL_2_DB"

              )
            {
                LicenseDate = LicenseExpiteDate;
            }
            
            else if (CompanyName == "BVCPSCTG_DB" // HO_DB
          || CompanyName == "BVCPS_DB"
           )
            {
                LicenseDate = LicenseExpiteDate;
            }
            return LicenseDate;
        }
    }
}
