using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.NBRAPI
{
    public class Return_9_1_Header
    {

        public string MSGID { get; set; }
        public string FBTyp { get; set; }
        public string BIN { get; set; }
        public string PeriodKey { get; set; }
        public string SendDate { get; set; }
        public string SendTime { get; set; }
        public string Depositor { get; set; }

        public Return_9_1_MainFull Return_9_1_MainFull { get; set; }

        public Return_9_1_Header()
        {
            Return_9_1_MainFull = new Return_9_1_MainFull();
        }

    }
}
