using System;
using System.Globalization;

namespace VATViewModel.DTOs
{
    public class Item
    {
        public string TYPE { get; set; }
        public string GOODS_SERVICE_CODE { get; set; }
        public string GOODS_SERVICE_NAME { get; set; }
        public decimal SD_RATE { get; set; }
        public decimal VAT_RATE { get; set; }
        public string VALID_FROM { get; set; }
        public string VALID_TO { get; set; }
        public string ITEM_ID { get; set; }
        public string NOTE { get; set; }

        public string NoteNo { get; set; }

        public string BANCD { get; set; }
        public string BANKL { get; set; }
        public string BANKN { get; set; }
        public string BRANNM { get; set; }

        public string CUSTOM_NAME { get; set; }

        public DateTime ValidFrom
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VALID_FROM))
                {
                    return DateTime.Now.AddDays(1);
                }

                return DateTime.ParseExact(VALID_FROM, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }

        public DateTime ValidTo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VALID_TO))
                {
                    return DateTime.Now.AddDays(1);
                }

                return DateTime.ParseExact(VALID_TO, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
        }


        public string MSGID { get; set; }
        public string FIELD_NAME { get; set; }
        public string MSGTX { get; set; }
        public string SUBMISSION_ID { get; set; }
        public string STATUS_FORM { get; set; }


    }
}