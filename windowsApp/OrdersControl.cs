using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Diagnostics;

namespace windowsApp
{
    public partial class OrdersControl : UserControl
    {
        public Menu main;
        public JArray todayData;
        private string mail;
        private string pass;


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
            status.Name = "status";
            status.HeaderText = "状況";
            status.Width = 100;

            DataGridViewTextBoxColumn customer = new DataGridViewTextBoxColumn();
            customer.Name = "name";
            customer.HeaderText = "お客様名";
            customer.Width = 160;

            DataGridViewTextBoxColumn total = new DataGridViewTextBoxColumn();
            total.Name = "num";
            total.HeaderText = "注文数";
            total.Width = 140;



            dataGridView1.Columns.Add(customer);
            dataGridView1.Columns.Add(time);
            dataGridView1.Columns.Add(item);
            dataGridView1.Columns.Add(process);
            dataGridView1.Columns.Add(status);
            dataGridView1.Columns.Add(total);

            dataGridView1.Visible = false;
            dataGridView1.ReadOnly = true;

             mail = main.getMail();
             pass = main.getPass();
             GetIndex();
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Columns[e.ColumnIndex].Name == "status")
            {
                OrderProcess orderProcess = new OrderProcess();
                orderProcess.main = this;
                JToken token = todayData[e.RowIndex];
                orderProcess.SetParameter(token, mail, pass);
                orderProcess.ShowDialog(this);
                orderProcess.Dispose();
            }
             else if(dgv.Columns[e.ColumnIndex].Name == "name")
            {
                string name = (string)todayData[e.RowIndex]["user_name"];
                string item = (string)todayData[e.RowIndex]["name"];
                string url =  "https://uematsu-backend.herokuapp.com/users/user_show";
                using(WebClient web = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection();
                    collection.Add("name", name);
                    try
                    {
                        web.UploadValuesAsync(new Uri(url), "POST", collection);
                        web.UploadValuesCompleted += (s, o) =>
                        {
                            string res = System.Text.Encoding.UTF8.GetString(o.Result);
                            JToken token = JToken.Parse(res);
                            Console.WriteLine(token["email"]);
                            System.Diagnostics.Process p = System.Diagnostics.Process.Start(new ProcessStartInfo("outlook")
                            {
                                UseShellExecute = true,
                                Arguments = "mailto:" + (string)token["email"] + "?subject=ご注文商品:" + item + "ついて"
                            }); 
                            p.Close();

                        };
                    }
                    catch (Exception) { }
                  

                };


                                      

               
            }
            else if(dgv.Columns[e.ColumnIndex].Name == "num")
            {
                JToken token = todayData[e.RowIndex];
                int price = int.Parse((string)token["price"]);
                int num = int.Parse((string)token["num"]);
                int total = price * num;



                string msg = "商品" + (string)token["name"]
                            + "\n" + "単価:" + (string)token["price"]
                            + "\n" + "数量:" + (string)token["num"]
                            +"\n" + "合計金額:" + total.ToString();
                MessageBox.Show(msg);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            todayData.Clear();
            dataGridView1.Rows.Clear();
            GetIndex();
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
        public string SetStatusView(string  str)
        {
            string newstr = "";
            if(str == "0")
            {
                newstr += "申請中";
            }
            else if(str == "1")
            {
                newstr += "加工済み";
            }
            else
            {
                newstr += "受渡し済み";
            }
            return newstr;
        }
        /*文字列をdatetime変換後時間のみ取得*/
        public string GetTime(string dateData)
        {
            string dateformat = "MM/dd/yyyy HH:mm:ss";
            DateTime dateTime = DateTime.ParseExact(dateData, dateformat, null);
            DateTime thistime = dateTime.ToLocalTime();
            return thistime.ToString("HH:mm");
        }
        //データ一覧の取得
        public void GetIndex()
        {
            groupBox1.Visible = true;
            for (var i = progressBar1.Minimum; i < progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
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
                       
                        todayData = setToday(array);  //本日の注文
                        if(todayData.Count > 0)
                        {
                            dataGridView1.Visible = true;
                            panel1.Visible = false;
                        }
                        Console.WriteLine(todayData);
                        foreach (var data in todayData)
                        {
                            dataGridView1.Rows.Add(
                                data["user_name"],
                                GetTime((string)data["receiving_time"]),
                                data["name"],
                                data["process"],
                                SetStatusView((string)data["status"]),
                                data["num"]
                            );
                        }
                        //分岐で変更
                        for (var i = 0; i < todayData.Count; i++)
                        {
                            if (dataGridView1.Rows[i].Cells[4].Value.ToString() == "申請中")
                            {
                                dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Blue;
                                dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                            else if (dataGridView1.Rows[i].Cells[4].Value.ToString() == "加工済み")
                            {
                                dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Orange;
                                dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[4].Style.ForeColor = Color.White;
                                dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Red;
                                dataGridView1.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
        }
        //GetIndex
    }
   
}
