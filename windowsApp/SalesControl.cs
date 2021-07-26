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
        private int total;

        public SalesControl()
        {
            InitializeComponent();
        }

        private async void SalesControl_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;

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

            dataGridView1.Visible = false;
            dataGridView1.ReadOnly = true;

            
            try
            {
                WebRequest request = WebRequest.Create(dataUrl);
                request.Method = "POST";
                var stream = await request.GetResponseAsync();
                var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                array = JArray.Parse(reader);
                if(array.Count > 0)
                {
                    dataGridView1.Visible = true;
                    panel1.Visible = false;
                }
                total = GetSalesTotal(array);
                label1.Text = "【売り上げ金額】" + total.ToString() + "円";
               
                foreach(var arr in array)
                {
                    dataGridView1.Rows.Add(
                        arr["day"],
                        arr["week"],
                        arr["num"] ?? "",
                        arr["合計"] ?? "",
                        GetRate((string)arr["合計"],　total)
                    );
                }
                SalesChart chart = new SalesChart();
                chart.main = this;
                chart.SetArray(array);
                chart.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("データを取得できません。ネットワークの確認してください。");
            }

         
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        /* 売り上げ合計*/
        public int GetSalesTotal(JArray array)
        {
            int total = 0;
            foreach(var arr in array)
            {
                total += arr["合計"] == null ? 0 : int.Parse((string)arr["合計"]);
            }
            return total;
        }
        /*売り上げ構成比*/
        public string  GetRate(string  sale, int total)
        {
            if(sale != null)
            {
                int num = int.Parse(sale);
                double d = Math.Floor((double)num / total * 100);
                Console.WriteLine(num);
                Console.WriteLine(d);
                return d.ToString() + "%";
            }
            else
            {
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                StreamWriter file = new StreamWriter(@"C:\Users\yukia\Desktop\売り上げ.csv", false, Encoding.UTF8);
                file.WriteLine("日付け", "曜日", "売り上げ数", "合計金額");
                foreach(var arr in array)
                {
                    file.WriteLine(string.Format("{0},{1},{2},{3}",

                        (string)arr["day"],
                        (string)arr["week"],
                        (string)arr["num"] ?? "",
                        (string)arr["合計"] ?? ""

                   ));
                }
                file.Close();
                MessageBox.Show("出力しました。");

            }
            catch (Exception)
            {
                MessageBox.Show("出力失敗しました。");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SalesChart chart = new SalesChart();
            chart.main = this;
            chart.SetArray(array);
            chart.Show();
            button2.Enabled = false;

        }
    }
}
