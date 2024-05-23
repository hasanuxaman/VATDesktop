using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VATViewModel.DTOs
{
    public class SettingsVM
    {
        public string SettingId { get; set; }
        public string SettingGroup { get; set; }
        public string SettingName { get; set; }
        public string SettingValue { get; set; }
        public string SettingType { get; set; }
        public string ActiveStatus { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }

    }

    public class SettingModel
    {

        public List<SettingsVM> vms { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string groupName { get; set; }
        
    }

}
