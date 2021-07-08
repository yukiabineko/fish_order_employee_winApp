using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace windowsApp
{
    public partial class SalesChart : Form
    {
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
            axis.IntervalType = DateTimeIntervalType.Days;
            axis.Interval = 1.0d;
            area.AxisX = axis;
            area.AxisX.Title = "日付け";
            area.AxisY.Title = "売り上げ金額";

            DateTime time = DateTime.Now;
            DateTime time2 = time.AddDays(1);
            series.Points.AddXY(time, 30);
            series.Points.AddXY(time2, 100);
            chart.Series.Add(series);
            chart.ChartAreas.Add(area);


        }
    }
}
