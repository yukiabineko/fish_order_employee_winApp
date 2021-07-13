using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class ItemEdit : Form
    {
        private string itemId;
        string phpUrl = "http://yukiabineko.sakura.ne.jp/windowsFomUpdate.php";
        public ItemControl main;
       

        public ItemEdit()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            textBox1.Text = openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string railsUrl = "https://uematsu-backend.herokuapp.com/items/" + itemId;
            main.groupBox1.Visible = true;
            for (var i = main.progressBar1.Minimum; i < main.progressBar1.Maximum; i += 10)
            {
                main.progressBar1.Value = i;
            }
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("name", textBox2.Text);
                webClient.QueryString = collection;
                webClient.UploadFileAsync(new Uri(phpUrl), textBox1.Text);

            };
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection collection = new NameValueCollection();
                collection.Add("name", textBox2.Text);
                collection.Add("price", textBox3.Text);
                collection.Add("category", comboBox1.SelectedItem == null ? "未選択" : comboBox1.SelectedItem.ToString());
                webClient.UploadValuesAsync(new Uri(railsUrl), "PATCH",  collection);
                webClient.UploadValuesCompleted += async (s, o) =>
                {
                    try
                    {
                        main.dataGridView1.Rows.Clear();
                        WebRequest request = WebRequest.Create("https://uematsu-backend.herokuapp.com/items");
                        var stream = await request.GetResponseAsync();
                        var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                        main.array = JArray.Parse(reader);
                        Bitmap bitmap = windowsApp.Properties.Resources.question;
                        using (WebClient wc = new WebClient())
                        {
                            foreach (var data in main.array)
                            {
                                try
                                {
                                    string imgUrl = "http://yukiabineko.sakura.ne.jp/react/" + (string)data["name"] + ".jpg";
                                    var st = wc.OpenRead(imgUrl);
                                    bitmap = new Bitmap(st);
                                    st.Close();
                                    main.dataGridView1.Rows.Add(
                                         bitmap,
                                         data["name"],
                                         data["price"],
                                         data["category"]
                                     );
                                }
                                catch (Exception)
                                {

                                    string imgUrl = "http://yukiabineko.sakura.ne.jp/react/question.png";
                                    var st = wc.OpenRead(imgUrl);
                                    bitmap = new Bitmap(st);
                                    st.Close();
                                    main.dataGridView1.Rows.Add(
                                        bitmap,
                                        data["name"],
                                        data["price"],
                                        data["category"]
                                    );
                                }
                            }
                            main.groupBox1.Visible = false;
                        };
                    }
                    catch (Exception) { }

                };
            };
            MessageBox.Show("更新しました。");
            this.Close();
            
        }

        private void ItemEdit_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
        }
        public void GetItemId(string id)
        {
            this.itemId = id;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                //押されたキーが 0～9でない場合は、イベントをキャンセルする
                e.Handled = true;
            }
        }
    }
}
