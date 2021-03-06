using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;


namespace windowsApp
{
    public partial class NewItem : Form
    {
        string phpUrl = "http://yukiabineko.sakura.ne.jp/windowsFomUpdate.php";
        string railsUrl = "https://uematsu-backend.herokuapp.com/items";
        public ItemControl itemControl;

        public NewItem()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            /*
            if(this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var DataFile = this.openFileDialog1.FileName;
                viewBitmap = (Bitmap)Image.FromFile(DataFile);
            }
            */
            using(WebClient webClient = new WebClient())
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
                collection.Add("category", comboBox1.SelectedItem == null? "未選択" : comboBox1.SelectedItem.ToString());
                webClient.UploadValuesAsync(new Uri(railsUrl), collection);
                webClient.UploadValuesCompleted += async (s, o) =>
                {
                    try
                    {
                       itemControl.dataGridView1.Rows.Clear();
                       itemControl.groupBox1.Visible = true;
                        for (var i = itemControl.progressBar1.Minimum; i < itemControl.progressBar1.Maximum;  i += 10)
                        {
                          itemControl.progressBar1.Value = i;
                        }
                        WebRequest request = WebRequest.Create("https://uematsu-backend.herokuapp.com/items");
                        var stream = await request.GetResponseAsync();
                        var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                        itemControl.array  = JArray.Parse(reader);
                        Bitmap bitmap = windowsApp.Properties.Resources.question;
                        using (WebClient wc = new WebClient())
                        {
                            foreach (var data in itemControl.array)
                            {
                                try
                                {
                                    string imgUrl = "http://yukiabineko.sakura.ne.jp/react/" + (string)data["name"] + ".jpg";
                                    var st = wc.OpenRead(imgUrl);
                                    bitmap = new Bitmap(st);
                                    st.Close();
                                   itemControl.dataGridView1.Rows.Add(
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
                                    itemControl.dataGridView1.Rows.Add(
                                        bitmap,
                                        data["name"],
                                        data["price"],
                                        data["category"]
                                    );
                                }
                            }
                            itemControl.groupBox1.Visible = false;
                        };
                    }
                    catch (Exception) {
                        button2.Enabled = true;
                    }
                   
                };
            };
            MessageBox.Show("登録しました。");
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog(this);
            textBox1.Text = openFileDialog1.FileName;
        }

        private void NewItem_Load(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar < '0' || '9' < e.KeyChar)&& e.KeyChar != '\b')
            {
                //押されたキーが 0～9でない場合は、イベントをキャンセルする
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
