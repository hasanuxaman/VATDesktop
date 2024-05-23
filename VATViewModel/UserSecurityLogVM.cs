using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel
{
   public class UserSecurityLogVM
    {
       public int id { get; set; }
       public int UserId { get; set; }
       public string IP { get; set; }
       public string UserAgent { get; set; }
       public string type { get; set; }
       public string pcName { get; set; }
       public string LoginTime { get; set; }
    }
}
