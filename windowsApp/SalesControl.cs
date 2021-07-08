using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;


namespace windowsApp
{
    public partial class SalesControl : UserControl
    {
        private string dataUrl = "https://uematsu-backend.herokuapp.com/sales";
        public JArray array;

        public SalesControl()
        {
            InitializeComponent();
        }

        private async void SalesControl_Load(object sender, EventArgs e)
        {
            SalesChart chart = new SalesChart();
            chart.Show();

            groupBox1.Visible = false;
            progressBar1.Visible = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;

            dataGridView1.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn dayColumn = new DataGridViewTextBoxColumn();
            dayColumn.HeaderText = "日付け";
            dayColumn.Width = 160;


            DataGridViewTextBoxColumn weekColumn = new DataGridViewTextBoxColumn();
            weekColumn.HeaderText = "曜日";
            weekColumn.Width = 100;


            DataGridViewTextBoxColumn numColumn = new DataGridViewTextBoxColumn();
            numColumn.HeaderText = "販売数";
            numColumn.Width = 100;

            DataGridViewTextBoxColumn saleColumn = new DataGridViewTextBoxColumn();
            saleColumn.HeaderText = "売り上げ金額";
            saleColumn.Width = 220;


            DataGridViewTextBoxColumn rateColumn = new DataGridViewTextBoxColumn();
            rateColumn.HeaderText = "構成比(%)";
            rateColumn.Width = 220;


            dataGridView1.Columns.Add(dayColumn);
            dataGridView1.Columns.Add(weekColumn);
            dataGridView1.Columns.Add(numColumn);
            dataGridView1.Columns.Add(saleColumn);
            dataGridView1.Columns.Add(rateColumn);

            groupBox1.Visible = true;
            for (var i = progressBar1.Minimum; i < progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            try
            {
                WebRequest request = WebRequest.Create(dataUrl);
                request.Method = "POST";
                var stream = await request.GetResponseAsync();
                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                array = JArray.Parse(reader);
                Console.WriteLine(array);
            }
            catch (Exception)
            {
                MessageBox.Show("データを取得できません。ネットワークの確認してください。");
            }
         
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
