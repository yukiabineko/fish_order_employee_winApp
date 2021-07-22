using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Newtonsoft.Json.Linq;

namespace windowsApp
{
    public partial class SalesChart : Form
    {
        private JArray array;
        public SalesControl main;

        public SalesChart()
        {
            InitializeComponent();
        }

        private void SalesChart_Load(object sender, EventArgs e)
        {
            Chart chart = new Chart();
            chart.Width = this.Width - 100;
            chart.Height = this.Height - 50;
            chart.Location = new Point(10, 10);
            chart.Parent = this;

            Series series = new Series();
            series.ChartType = SeriesChartType.Column;

            ChartArea area = new ChartArea();
            Axis axis = new Axis();
            axis.IntervalOffsetType = DateTimeIntervalType.Days;
            axis.Interval = 1.0d;
            area.AxisX = axis;
            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.Enabled = false;
            area.AxisX.Minimum = DateTime.Now.AddDays(-DateTime.Now.Day + 1).ToOADate();
            
            area.AxisX.Title = "日付け";
            area.AxisY.Title = "売り上げ金額";

            foreach(var arr in array)
            {
                DateTime tim = SetDay((string)arr["day"]);
                DateTime time = tim.AddDays(0);
                int sales = arr["合計"] == null? 0 : int.Parse((string)arr["合計"]);
                series.Points.AddXY(time, sales);
            }
            chart.Series.Add(series);
            chart.ChartAreas.Add(area);
           




        }
        public void SetArray(JArray array)
        {
            this.array = array;
        }
        public DateTime SetDay(string sendday)
        {
            DateTime nowData = DateTime.Now;
            string getYear = nowData.ToString("yyyy");
            string setDay = getYear + "/" + sendday;
            DateTime newDay = DateTime.Parse(setDay);
            return newDay;

        }

        private void SalesChart_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.button2.Enabled = true;
        }
    }
}
