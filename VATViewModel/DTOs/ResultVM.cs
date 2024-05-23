using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class ResultVM
    {

        public ResultVM()
        {
            Status = "Fail";
            Message = "Fail";
            ErrorList = new List<ErrorMessage>();
        }

        public string[] GetResultArray()
        {
            string[] result = new[] {Status, Message, Exception, MethodName};

            return result;
        }

        public string Status { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
        public string SQLText { get; set; }
        public string Exception { get; set; }
        public string MethodName { get; set; }
        public string InvoiceNo { get; set; }
        public string Post { get; set; }
        public string Print { get; set; }
        public string Step { get; set; }
        public int RowCount { get; set; }
        public string SubmissionId { get; set; }

        public List<ErrorMessage> ErrorList { get; set; }

        public bool IsSuccess
        {
            get
            {
                return Status.ToLower() == "success";
            }
        }

        public string XML { get; set; }
        public string XML2 { get; set; }

        public string FileName { get; set; }

        public string Json { get; set; }

        public string ResponseFileName { get; set; }


    }


    public class MessageIdVM
    {
        public string MessageId { get; set; }
        public string PeriodId { get; set; }

        public string FullId { get; set; }

        public string SubmissionId { get; set; }

        public string FBTyp { get; set; }
        public string BIN { get; set; }
        public string PeriodKey { get; set; }
        public string Depositor { get; set; }
        public string SubmitDate { get; set; }
        public string SubmitTime { get; set; }

    }
}
