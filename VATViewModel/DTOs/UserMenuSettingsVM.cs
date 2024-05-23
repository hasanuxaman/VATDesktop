using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
   public class UserMenuSettingsVM
    {
       public string FormID { get; set; }
       public string UserID { get; set; }
       public string Access { get; set; }
       public string FormName { get; set; }
       public string PostAccess { get; set; }
       public string AddAccess { get; set; }
       public string EditAccess { get; set; }
       public string CreatedBy { get; set; }
       public string CreatedOn { get; set; }
       public string LastModifiedBy { get; set; }
       public string LastModifiedOn { get; set; }


    }
}
