using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VATViewModel.DTOs
{
    public class MailSettings
    {

        public string MailToAddress { get; set; }
        public string MailFromAddress = "Alamgir.Hossain@symphonysoftt.com";//bcsbl.invoices@britishcouncil.org
        public bool USsel = false;
        public string Password = "";
        public string UserName = "Alamgir.Hossain@symphonysoftt.com"; //bcsbl.invoices@britishcouncil.org

        public string ServerName = "SMTP.gmail.com"; //SMTP.britishcouncil.org
        public string MailBody { get; set; }
        public string MailHeader { get; set; }
        public string Fiscalyear { get; set; }
        public int Port = 587;
        public HttpPostedFileBase fileUploader { get; set; }
        public string FileName { get; set; }

    }
}
