using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATServer.Library
{
    public class VATName
    {
        public static string[] vATName = new string[] 
        { 
           "VAT 4.3",//0
           "VAT 4.3 (Toll Issue)",//6
           "VAT 4.3 (Toll Receive)",//7
           "VAT 4.3 (Tender)",//8
           "VAT 4.3 (Package)" ,//9
           "VAT 4.3 (Wastage)", //10
           "VAT 4.3 (Internal Issue)",//5
           "VAT Porisisto Ka"//5
           //",VAT 4.3 (ProductionWithToll)"//5


           //////"VAT 1",//0
           //////"VAT 1 Ka (Tarrif)",//1
           //////"VAT 1 Kha (Trading)", //2
           //////"VAT 1 Ga (Export)",//3
           //////"VAT 1 Gha (Fixed)",//4
           //////"VAT 4.3 (Internal Issue)",//5
           //////"VAT 4.3 (Toll Issue)",//6
           //////"VAT 4.3 (Toll Receive)",//7
           //////"VAT 4.3 (Tender)",//8
           //////"VAT 4.3 (Package)" ,//9
           //////"VAT 4.3 (Wastage)" //10
       };

        public IList<string> VATNameList
        {
            get
            {
                return vATName.ToList<string>();
            }
        }

    }
    public class TypeName
    {
        public static string[] typeName = new string[] { 
"All",
"NonVAT",
"Exempted",
"VAT",
"OtherRate",
"FixedVAT",
"Ternover",
"UnRegister",
"NonRebate",
"NonVAT-Local",
"NonVAT-Import",
"Exempted-Local",
"Exempted-Import",
"VAT-Local",
"VAT-Import",
"OtherRate-Local",
"OtherRate-Import",
//"FixedVAT",
//"Ternover",
//"UnRegister",
"NonRebate-Local",
"NonRebate-Import"//,"Truncated",
          
       };

        public IList<string> TypeNameList
        {
            get
            {
                return typeName.ToList<string>();
            }
        }

    }

    public class RecordSelect
    {
        public static string[] vrecordSelect = new string[] 
        { 
           "100",//0
           "200",//0
           "300",//0
           "400",//0
           "500",//0
           "1000",//0
           "2000",//0
           "3000",//0
           "4000",//0
           "5000",//0
           "10000",//0
           "20000",//0
           "25000",//0
           "50000",//0
           "75000",//0
           "100000",//0
           "All"//0
       };

        public IList<string> RecordSelectList
        {
            get
            {
                return vrecordSelect.ToList<string>();
            }
        }

    }


}
