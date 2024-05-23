using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
   public class UomDTO
    {
        public string UOMId { get; set; }
        public string UOMFrom { get; set; }
        public string UOMTo { get; set; }
        public decimal Convertion { get; set; }
        public string CTypes { get; set; }

    }
}
