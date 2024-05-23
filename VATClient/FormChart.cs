using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VATDesktop.Repo;

namespace VATClient
{
    public partial class FormChart : Form
    {

        public FormChart()
        {
            InitializeComponent();
        }
        ChartRepo _chartRepo = new ChartRepo();
        private void FormChart_Load(object sender, EventArgs e)
        {
            DayWiseSale();
        }
        private void DayWiseSale()
        {
            DataTable dt = new DataTable();
        dt=_chartRepo.DayWiseSale("","");
        chartDayWiseSale.DataSource = dt;

        //set the member of the chart data source used to data bind to the X-values of the series  
        chartDayWiseSale.Series["SubTotal"].XValueMember = "Date";
        chartDayWiseSale.Series["SubTotal"].IsValueShownAsLabel = true;
        //set the member columns of the chart data source used to data bind to the X-values of the series  
        chartDayWiseSale.Series["SubTotal"].YValueMembers = "SubTotal";
        chartDayWiseSale.Titles.Add("Day Wise Chart");

        }
    }
}
