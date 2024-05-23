using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class VATReturnHeaderVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }                                  
        public string Branch { get; set; }                                    
        public string Remarks { get; set; }                                   
        public string PeriodID { get; set; }                                  
        public string PeriodStart { get; set; }                               
        public int BranchId { get; set; }                                  
        public bool MainOrginalReturn { get; set; }                         
        public bool LateReturn { get; set; }                                
        public bool AmendReturn { get; set; }
        public bool AlternativeReturn { get; set; }
        public bool NoActivites { get; set; }                               
        public string NoActivitesDetails { get; set; }                        
        public string DateOfSubmission { get; set; }
        public string SignatoryName { get; set; }
        public string SignatoryDesig { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string NationalID { get; set; }
        public string BranchName { get; set; }
        public bool IsTraderVAT { get; set; }                               


        public object PeriodName { get; set; }

        public string PostStatus { get; set; }
    }
}
