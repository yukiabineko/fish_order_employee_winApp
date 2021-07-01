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
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class OrdersControl : UserControl
    {
        public Menu main;
       
        public OrdersControl()
        {
            InitializeComponent();
        }

        private  void OrdersControl_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowTemplate.Height = 60;

            groupBox1.Visible = false;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 10;
            progressBar1.Value = progressBar1.Minimum;


            DataGridViewTextBoxColumn time = new DataGridViewTextBoxColumn();
            time.HeaderText = "受け渡し時間";
            time.Width = 160;

            DataGridViewTextBoxColumn item = new DataGridViewTextBoxColumn();
            item.HeaderText = "商品名";
            item.Width = 140;

            DataGridViewTextBoxColumn process = new DataGridViewTextBoxColumn();
            process.HeaderText = "加工法";
            process.Width = 120;

            DataGridViewTextBoxColumn status = new DataGridViewTextBoxColumn();
            status.HeaderText = "状況";
            status.Width = 100;

            DataGridViewTextBoxColumn customer = new DataGridViewTextBoxColumn();
            customer.HeaderText = "お客様名";
            customer.Width = 160;

            DataGridViewTextBoxColumn total = new DataGridViewTextBoxColumn();
            total.HeaderText = "注文数";
            total.Width = 140;



            dataGridView1.Columns.Add(customer);
            dataGridView1.Columns.Add(time);
            dataGridView1.Columns.Add(item);
            dataGridView1.Columns.Add(process);
            dataGridView1.Columns.Add(status);
            dataGridView1.Columns.Add(total);

            string mail = main.getMail();
            string pass = main.getPass();
          
            using (WebClient webClient = new WebClient())
            {
                string url = "https://uematsu-backend.herokuapp.com/shoppings/index";
                NameValueCollection collection = new NameValueCollection();
                collection.Add("email", mail);
                collection.Add("password", pass);
                try
                {
                    webClient.UploadValuesAsync(new Uri(url), collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        string resStr = System.Text.Encoding.UTF8.GetString(o.Result);
                        JArray array = JArray.Parse(resStr);
                        JArray todayData = setToday(array);  //本日の注文
                        foreach(var data in todayData)
                        {
                            dataGridView1.Rows.Add(
                                data["user_name"],
                                data["receiving_time"],
                                data["name"],
                                data["process"],
                                data["status"]
                            );
                        }
                       
                    };
                }
                catch (Exception)
                {
                    MessageBox.Show("データの取得失敗しました。ネットワークの確認してください。");
                }
            };

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        /*本日のデータの取得*/
        public JArray setToday(JArray array)
        {
            /*本日の日付け文字列変換*/

            DateTime today = DateTime.Now;
            string today_str = today.ToString("d");

            JArray newArray = new JArray();
            foreach(var arr in array)
            {
                if((string)arr["shopping_date"] == today_str)
                {
                    newArray.Add(arr);
                }
            }
            return newArray;
        }
        /*送られたステータスによる表示変化*/
        public string setStatusView(string  str)
        {
            string newstr = "";
            if(str == "0")
            {
                newstr += "申請中";
            }
            else if(str == "1")
            {
                newstr += "";
            }

        }
    }
   
}
