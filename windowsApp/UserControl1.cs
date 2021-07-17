using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Diagnostics;

namespace windowsApp
{
    public partial class UserControl1 : UserControl
    {
        public Menu menu;
        public JArray array;
        private string mail;
        private string pass;

        public UserControl1()
        {
            InitializeComponent();
            groupBox1.Visible = false;
            progressBar1.Visible = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;

            dataGridView1.AllowUserToAddRows = false;
            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.HeaderText = "会員名";
            nameColumn.Width = 160;


            DataGridViewTextBoxColumn mailColumn = new DataGridViewTextBoxColumn();
            mailColumn.HeaderText = "メールアドレス";
            mailColumn.Width = 240;
            mailColumn.Name = "mail";

            DataGridViewTextBoxColumn telColumn = new DataGridViewTextBoxColumn();
            telColumn.HeaderText = "電話番号";
            telColumn.Width = 155;


            DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn();
            deleteBtn.UseColumnTextForButtonValue = true;
            deleteBtn.Name = "delete";
            deleteBtn.Text = "削除";
            deleteBtn.Width = 160;
            deleteBtn.FlatStyle = FlatStyle.Flat;
            deleteBtn.DefaultCellStyle.BackColor = Color.Red;
            deleteBtn.DefaultCellStyle.ForeColor = Color.White;


            dataGridView1.Columns.Add(nameColumn);
            dataGridView1.Columns.Add(mailColumn);
            dataGridView1.Columns.Add(telColumn);
            dataGridView1.Columns.Add(deleteBtn);

        }

        private void label1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            progressBar1.Minimum = 10;
            progressBar1.Maximum = 100;
            progressBar1.Value = progressBar1.Minimum;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            string usersUrl = "https://uematsu-backend.herokuapp.com/users/index";
            groupBox1.Visible = true;
            progressBar1.Visible = true;
            
           
            for (var i = progressBar1.Minimum; i<progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection();
                    collection.Add("email", this.mail);
                    collection.Add("password", this.pass);
                    webClient.UploadValuesAsync(new Uri(usersUrl), collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        string data = System.Text.Encoding.UTF8.GetString(o.Result);
                        array = JArray.Parse(data);
                        foreach (var obj in array)
                        {
                            dataGridView1.Rows.Add(
                                obj["name"],
                                obj["email"],
                                obj["tel"]
                            );
                        }
                        groupBox1.Visible = false;
                    };

                };
            }
            catch (Exception)
            {
                MessageBox.Show("データの取得に失敗しました。ネットワーク等ご確認ください。");
            }
           
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            this.mail = menu.getMail();
            this.pass = menu.getPass();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UserNew usernew = new UserNew();
            usernew.main = this;
            usernew.SetData(this.mail, this.pass);
            usernew.ShowDialog(this);
            usernew.Dispose();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            JToken obj = array[e.RowIndex];

            DateTime dateTime = DateTime.Now;
            string dt = dateTime.ToString("yyyy/MM/dd");

            DateTime tomorrow = DateTime.Now;
            tomorrow = tomorrow.AddDays(1);
            string tw = tomorrow.ToString("yyyy/MM/dd");


            if (dgv.Columns[e.ColumnIndex].Name == "delete")
            {

                DialogResult result = MessageBox.Show("削除しますか？","",MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    string deleteUrl = "https://uematsu-backend.herokuapp.com/users/" + (string)obj["id"];
                    using (WebClient webClient = new WebClient())
                    {
                        NameValueCollection collection = new NameValueCollection();
                        collection.Add("id", (string)obj["id"]);
                        collection.Add("email", menu.getMail());
                        collection.Add("password", menu.getPass());
                        webClient.UploadValuesAsync(new Uri(deleteUrl), "delete", collection);
                        webClient.UploadValuesCompleted += (s, o) =>
                        {
                            dgv.Rows[e.RowIndex].Visible = false;
                            foreach(DataGridViewRow r in dgv.SelectedRows)
                            {
                                dgv.Rows.Remove(r);
                            }
                            MessageBox.Show("削除しました。");

                        };
                    };

                }
            }
            else if(dgv.Columns[e.ColumnIndex].Name == "mail")
            {
                string to_mail = (string)array[e.RowIndex]["email"];
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(new ProcessStartInfo("outlook")
                {
                    UseShellExecute = true,
                    Arguments = "mailto:" + to_mail
                });
                p.Close();
              
            }
            else
            {
                label2.Text = "集計しています。しばらくおまちください。";
                groupBox1.Visible = true;
                progressBar1.Visible = true;
                progressBar1.Minimum = 10;
                progressBar1.Maximum = 100;
                progressBar1.Value = progressBar1.Minimum;
                for (var i = progressBar1.Minimum; i < progressBar1.Maximum; i += 10)
                {
                    progressBar1.Value = i;
                }

                string orderUrl = "https://uematsu-backend.herokuapp.com/users/show";
                using (WebClient webClient = new WebClient())
                {
                    NameValueCollection collection = new NameValueCollection();
                    collection.Add("id", (string)obj["id"]);
                    collection.Add("email", menu.getMail());
                    collection.Add("password", menu.getPass());
                    webClient.UploadValuesAsync(new Uri(orderUrl), collection);
                    webClient.UploadValuesCompleted += (s, o) =>
                    {
                        string data = System.Text.Encoding.UTF8.GetString(o.Result);
                        JObject obj = JObject.Parse(data);

                       JArray arrs = (JArray)obj["orders"];
                       JArray todayArray = new JArray();
                       JArray historyArray = new JArray();

   
                        foreach(var ar in arrs[0])
                        {
                            if((string)ar["shopping_date"] == dt || (string)ar["shopping_date"] == tw)
                            {
                                todayArray.Add(ar);
                            }
                            else
                            {
                                historyArray.Add(ar);
                            }
                        }

                        UserOrder orderWin = new UserOrder();
                        orderWin.setJArray(arrs);
                        orderWin.setTodayArray(todayArray);
                        orderWin.setHistoryArray(historyArray);
                        orderWin.setMail(menu.getMail());
                        orderWin.setPass(menu.getPass());
                        orderWin.setId((string)obj["id"]);
                        orderWin.ShowDialog(this);
                        orderWin.Dispose();
                        progressBar1.Visible = false;
                       
                        groupBox1.Visible = false;

                    };
                };
              
            }

        }
       
    }

}
