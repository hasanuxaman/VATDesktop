using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class ParameterVM
    {
        public string CurrentUserID;
        public string TableName { get; set; }
        public string[] selectFields { get; set; }
        public string[] conditionFields { get; set; }
        public string[] conditionValues { get; set; }

        public string JoinClause { get; set; }

        public string OrderBy { get; set; }

        public string ItemNo { get; set; }

        public string Date { get; set; }

        public bool IsDBMigration { get; set; }

        public List<string> IDs { get; set; }

        public DataTable dt { get; set; }

        public string AdditionalWhereClause { get; set; }

        private int _branchId;
        public int BranchId
        {
            get
            {
                if (AllBranch) return 0;
                return _branchId;
            }
            set
            {
                _branchId = value;
            }
        }

        public string InvoiceNo { get; set; }

        public object SignatoryName { get; set; }

        public object SignatoryDesig { get; set; }
        public string CurrentUser { get; set; }

        public bool IsChecked { get; set; }
        public string ProductName { get; set; }
        public string ReportType { get; set; }

        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public bool AllBranch { get; set; }
        public bool FromSP { get; set; }
        public string SPSQLText { get; set; }
    }
}
