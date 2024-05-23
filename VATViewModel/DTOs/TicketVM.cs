using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VATViewModel.DTOs
{
    public class TicketVM
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Coach { get; set; }
        public string Seats { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string SeatFare { get; set; }
        public string TotalFare { get; set; }
        public string PNR { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Boarding { get; set; }
        public string Dropping { get; set; }
        public string IssuedOn { get; set; }
        public string IssuedBy { get; set; }
        public string SerialNo { get; set; }
    }
}
