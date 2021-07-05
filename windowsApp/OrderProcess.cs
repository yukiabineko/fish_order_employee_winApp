using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class OrderProcess : Form
    {
        private string mail;
        private string pass;
        private string id = "";
        private string user_name = "";
        private string item_name = "";
        private string price_data = "";
        private string num_data = "";
        private string total_data = "";
        private string order_status = "";
        public OrdersControl main;


        public OrderProcess()
        {
            InitializeComponent();
        }

        private void OrderProcess_Load(object sender, EventArgs e)
        {
            item.Text = item_name;
            user.Text = user_name;
            pirce.Text = price_data;
            num.Text = num_data;
            total.Text = total_data;
            status.Text = order_status;

            groupBox1.Visible = false;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 10;
            progressBar1.Value = progressBar1.Minimum;

        }
        public void SetParameter(JToken token, string mail, string pass)
        {
            this.mail = mail;
            this.pass = pass;
            this.id = (string)token["id"];
            this.user_name = (string)token["user_name"];
            this.item_name = (string)token["name"];
            this.price_data = (string)token["price"];
            this.num_data = (string)token["num"];
            this.total_data = (int.Parse(this.price_data) * int.Parse(this.num_data)).ToString();
            if( (string)token["status"] == "0")
            {
                this.order_status = "申請中";
                status.ForeColor = Color.Blue;
                button1.Visible = false;
                button2.Location = new Point(65, 288);
                button3.Location = new Point(65, 353);
              
            }
            else if ((string)token["status"] == "1")
            {
                this.order_status = "加工済み";
                status.ForeColor = Color.Orange;
                button2.Visible = false;
                button3.Location = new Point(65, 353);
            }
            else
            {
                this.order_status = "受け渡し済み";
                status.ForeColor = Color.Red;
                button3.Visible = false;
                button2.Location = new Point(65, 353);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StatusChange("0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StatusChange("1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StatusChange("2");
        }
        //各ボタンにより状況ステータス変更
        public void StatusChange(string statusNo)
        {
            groupBox1.Visible = true;
            for (var i = progressBar1.Minimum; i < progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            string url = "https://uematsu-backend.herokuapp.com/shoppings/" + this.id;
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("id", this.id);
                collection.Add("name", this.user_name);
                collection.Add("status", statusNo);

                try
                {
                    webClient.UploadValuesAsync(new Uri(url), "PATCH", collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        string resStr = System.Text.Encoding.UTF8.GetString(o.Result);
                        JToken token = JToken.Parse(resStr);
                        MessageBox.Show((string)token["message"]);
                        this.Close();

                        //メインの一覧更新
                        using (WebClient webClient = new WebClient())
                        {
                            main.todayData.Clear();
                            main.dataGridView1.Rows.Clear();
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
                                    main.todayData = main.setToday(array);  //本日の注文
                                    Console.WriteLine(main.todayData);
                                    foreach (var data in main.todayData)
                                    {
                                        main.dataGridView1.Rows.Add(
                                            data["user_name"],
                                            main.GetTime((string)data["receiving_time"]),
                                            data["name"],
                                            data["process"],
                                            main.SetStatusView((string)data["status"]),
                                            data["num"]
                                        );
                                    }
                                    //分岐で変更
                                    for (var i = 0; i < main.todayData.Count; i++)
                                    {
                                        if (main.dataGridView1.Rows[i].Cells[4].Value.ToString() == "申請中")
                                        {
                                            main.dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                            main.dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Blue;
                                            main.dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                        }
                                        else if (main.dataGridView1.Rows[i].Cells[4].Value.ToString() == "加工済み")
                                        {
                                            main.dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                            main.dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Orange;
                                            main.dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                        }
                                        else
                                        {
                                            main.dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                            main.dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                            main.dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                        }
                                    }
                                    groupBox1.Visible = false;

                                };
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("データの取得失敗しました。ネットワークの確認してください。");
                            }
                        };

                    };
                   
                }
                catch (Exception)
                {
                    MessageBox.Show("更新に失敗しました。ネットワークの確認お願いします。");
                    groupBox1.Visible = false;
                }
               
            };
        }
    }
}
