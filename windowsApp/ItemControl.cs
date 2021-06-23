using System;

using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;

namespace windowsApp
{
    public partial class ItemControl : UserControl
    {
        public JArray array;

        public ItemControl()
        {
            InitializeComponent();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.RowTemplate.Height = 60;
           
            groupBox1.Visible = false;
            progressBar1.Maximum = 100;
            progressBar1.Minimum = 10;
            progressBar1.Value = progressBar1.Minimum;

            DataGridViewImageColumn imgCell = new DataGridViewImageColumn();
            imgCell.HeaderText = "画像";
            imgCell.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCell.Width = 80;


            DataGridViewTextBoxColumn nameCell = new DataGridViewTextBoxColumn();
            nameCell.HeaderText = "商品名";
            nameCell.Width = 170;

            DataGridViewTextBoxColumn priceCell = new DataGridViewTextBoxColumn();
            priceCell.HeaderText = "価格";
            priceCell.Width = 100;

            DataGridViewButtonColumn processCell = new DataGridViewButtonColumn();
            processCell.HeaderText = "加工法管理";
            processCell.Name = "process";
            processCell.Text = "管理";
            processCell.UseColumnTextForButtonValue = true;
            processCell.Width = 150;
            processCell.FlatStyle = FlatStyle.Flat;
            processCell.DefaultCellStyle.BackColor = Color.Blue;
            processCell.DefaultCellStyle.ForeColor = Color.White;


            DataGridViewButtonColumn editCell = new DataGridViewButtonColumn();
            editCell.HeaderText = "編集";
            editCell.Name = "edit";
            editCell.Text = "編集";
            editCell.UseColumnTextForButtonValue = true;
            editCell.Width = 120;
            editCell.FlatStyle = FlatStyle.Flat;
            editCell.DefaultCellStyle.BackColor = Color.Blue;
            editCell.DefaultCellStyle.ForeColor = Color.White;

            DataGridViewButtonColumn deleteCell = new DataGridViewButtonColumn();
            deleteCell.HeaderText = "削除";
            deleteCell.Name = "delete";
            deleteCell.Text = "削除";
            deleteCell.UseColumnTextForButtonValue = true;
            deleteCell.Width = 120;
            deleteCell.FlatStyle = FlatStyle.Flat;
            deleteCell.DefaultCellStyle.BackColor = Color.Red;
            deleteCell.DefaultCellStyle.ForeColor = Color.White;

            dataGridView1.Columns.Add(imgCell);
            dataGridView1.Columns.Add(nameCell);
            dataGridView1.Columns.Add(priceCell);
            dataGridView1.Columns.Add(processCell);
            dataGridView1.Columns.Add(editCell);
            dataGridView1.Columns.Add(deleteCell);

        }

        private void ItemControl_Load(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            groupBox1.Visible = true;
            for(var i = progressBar1.Minimum; i<progressBar1.Maximum; i += 10)
            {
                progressBar1.Value = i;
            }
            WebRequest request = WebRequest.Create("https://uematsu-backend.herokuapp.com/items");
            var stream = await request.GetResponseAsync();
            var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
            array = JArray.Parse(reader);
            Bitmap bitmap = windowsApp.Properties.Resources.question;
            using (WebClient wc = new WebClient())
            {
                foreach (var data in array)
                {
                    try
                    {
                        string imgUrl = "http://yukiabineko.sakura.ne.jp/react/" + (string)data["name"] + ".jpg";
                        var st = wc.OpenRead(imgUrl);
                        bitmap = new Bitmap(st);
                        st.Close();
                        dataGridView1.Rows.Add(
                            bitmap,
                            data["name"],
                            data["price"],
                            data["category"]
                        );
                    }
                    catch (Exception) {

                        string imgUrl = "http://yukiabineko.sakura.ne.jp/react/question.png";
                        var st = wc.OpenRead(imgUrl);
                        bitmap = new Bitmap(st);
                        st.Close();
                        dataGridView1.Rows.Add(
                            bitmap,
                            data["name"],
                            data["price"],
                            data["category"]
                        );
                    }
                   
                }
                groupBox1.Visible = false;
            };
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewItem newItem = new NewItem();
            newItem.itemControl = this;
            newItem.ShowDialog(this);
            newItem.Dispose();
        }

        private  void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private async void button_action(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            Console.WriteLine(array);
            Console.WriteLine("番号");
            Console.WriteLine(e.RowIndex);

            JToken obj = array[e.RowIndex];

            if(dgv.Columns[e.ColumnIndex].Name == "edit")
            {
                ItemEdit itemEdit = new ItemEdit();
                itemEdit.pictureBox1.ImageLocation = "http://yukiabineko.sakura.ne.jp/react/" + (string)obj["name"] + ".jpg";
                itemEdit.label1.Text = (string)obj["name"] + "編集";
                itemEdit.textBox2.Text = (string)obj["name"];
                itemEdit.textBox3.Text = (string)obj["price"];
                itemEdit.comboBox1.SelectedItem = (string)obj["category"];


                itemEdit.ShowDialog(this);
                itemEdit.Dispose();

            }
            else if(dgv.Columns[e.ColumnIndex].Name == "delete")
            {
                DialogResult result = MessageBox.Show("削除しますか？", "", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    string deleteUrl = "http://yukiabineko.sakura.ne.jp/delete.php/";
                    string deleteItem = "https://uematsu-backend.herokuapp.com/items/" + (string)obj["id"];

                    DataGridView dg = (DataGridView)sender;


                    using (WebClient webClient =new WebClient())
                    {
                        NameValueCollection collection = new NameValueCollection();
                        collection.Add("name", (string)obj["name"]);
                        webClient.UploadValuesAsync(new Uri(deleteUrl), collection);
                    };
                    try
                    {
                        WebRequest delrequest = WebRequest.Create(deleteItem);
                        delrequest.Method = "DELETE";
                        var stream = await delrequest.GetResponseAsync();
                        var reader = new StreamReader(stream.GetResponseStream()).ReadToEnd();
                        JObject ob = JObject.Parse(reader);
                        stream.Close();
                        MessageBox.Show((string)ob["message"]);
                        dg.Rows[e.RowIndex].Visible = false;
                        foreach (DataGridViewRow r in dgv.SelectedRows)
                        {
                            dataGridView1.Rows.Remove(r);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("削除失敗");
                    }
                }
            }
            else if(dgv.Columns[e.ColumnIndex].Name == "process")
            {
                Process process = new Process();
                process.setId((string)obj["id"]);
                process.ShowDialog(this);
                process.Dispose();
            }
            else
            {
                MessageBox.Show("normal");
            }
        }
    }
}
