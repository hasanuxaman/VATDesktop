using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class ErrorLogVM
    {
        public string ImportId { get; set; }
        public string ErrorDate { get; set; }
        public string FileName { get; set; }
        public string ErrorMassage { get; set; }
        public string SourceName { get; set; }
        public string ActionName { get; set; }
        public string TransactionName { get; set; }

    }
}
