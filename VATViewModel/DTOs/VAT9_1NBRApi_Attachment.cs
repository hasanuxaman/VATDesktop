using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class VAT9_1NBRApi_AttachmentVM
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string DocType { get; set; }
        public string FileType { get; set; }
        public string PeriodId { get; set; }
        public string FileLocation { get; set; }
        public string Notes { get; set; }
        public string Operation { get; set; }

    }
}
