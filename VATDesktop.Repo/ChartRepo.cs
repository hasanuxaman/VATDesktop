using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VATServer.Library;

namespace VATDesktop.Repo
{
  public  class ChartRepo
    {
      ChartDAL _chartDAL = new ChartDAL();

      public DataTable DayWiseSale(string InvoiceDateFrom, string InvoiceDateTo)
      {
          return _chartDAL.DayWiseSale(InvoiceDateFrom, InvoiceDateTo);
      }
    }
}
