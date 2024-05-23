using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.NBRAPI
{
    public class Return_9_1_SF_adjustSet
    {
        public string MSGID { get; set; }
        public string FieldID { get; set; }
        public string NoteNo { get; set; }
        public string IssueDate { get; set; }
        public string VATChallanNo { get; set; }
        public string VATChallanDate { get; set; }
        public string ReasonIssuance { get; set; }
        public string ValueChallan { get; set; }
        public string QuantityinChallan { get; set; }
        public string VATChallan { get; set; }
        public string SDChallan { get; set; }
        public string ValueIncDecAdj { get; set; }
        public string QuantityIncDecAdj { get; set; }
        public string VATIncDecAdj { get; set; }
        public string SDIncDecAdj { get; set; }

    }
}
