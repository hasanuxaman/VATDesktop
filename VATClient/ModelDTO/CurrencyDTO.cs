using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATClient.ModelDTO
{
    public class CurrencyDTO
    {
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string Country { get; set; }
        public string Comments { get; set; }
        public string ActiveStatus { get; set; }
    }
}
