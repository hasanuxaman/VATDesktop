using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.Integration.NBRAPI
{
    public class Return_9_1_MainFull
    {
        public string S2TypeReturn { get; set; }
        public string S2EcoActivities { get; set; }
        public string S2AnyActivities { get; set; }
        public string S5PmtNotBankingChannel { get; set; }
        public string S7SDAgainstExport { get; set; }
        public string S7InterestOverdueVAT { get; set; }
        public string S7InterestOverdueSD { get; set; }
        public string S7FinePenalty { get; set; }
        public string S7FinePenaltyOther { get; set; }
        public string S7ExciseDuty { get; set; }
        public string S7DevelopSurcharge { get; set; }
        public string S7IctDevelopSurcharge { get; set; }
        public string S7HealthCareSurcharge { get; set; }
        public string S7EnvProtectSurcharge { get; set; }
        public string S11Refund { get; set; }
        public string S11RefundVAT { get; set; }
        public string S11RefundSD { get; set; }
        public string S12Name { get; set; }
        public string S12Designation { get; set; }
        public string S12Mobile { get; set; }
        public string S12NationalNIDPP { get; set; }
        public string S12Email { get; set; }
        public string S12Confirm { get; set; }

        public List<Return_9_1_SF_adjustSet> Return_9_1_SF_adjustSet { get; set; }
        public List<Return_9_1_SF_att_fileSet> Return_9_1_SF_att_fileSet { get; set; }
        public List<Return_9_1_SF_challanSet> Return_9_1_SF_challanSet { get; set; }
        public List<Return_9_1_SF_goservSet> Return_9_1_SF_goservSet { get; set; }
        public List<Return_9_1_SF_otherSet> Return_9_1_SF_otherSet { get; set; }
        public List<Return_9_1_SF_vdsSet> Return_9_1_SF_vdsSet { get; set; }

        public Return_9_1_MainFull()
        {
            Return_9_1_SF_adjustSet = new List<Return_9_1_SF_adjustSet>();
            Return_9_1_SF_att_fileSet = new List<Return_9_1_SF_att_fileSet>();
            Return_9_1_SF_challanSet = new List<Return_9_1_SF_challanSet>();
            Return_9_1_SF_goservSet = new List<Return_9_1_SF_goservSet>();
            Return_9_1_SF_otherSet = new List<Return_9_1_SF_otherSet>();
            Return_9_1_SF_vdsSet = new List<Return_9_1_SF_vdsSet>();
        }

    }
}
