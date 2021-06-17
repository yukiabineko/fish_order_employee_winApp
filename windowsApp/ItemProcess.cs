using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace windowsApp
{
    public partial class Process : Form
    {
        public Process()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string geturl = "https://uematsu-backend.herokuapp.com/processings/${props.item.id}";
            string posturl = "https://uematsu-backend.herokuapp.com/processings";
        }

        private void Process_Load(object sender, EventArgs e)
        {
           
            DataGridViewTextBoxColumn processName = new DataGridViewTextBoxColumn();
            processName.Width = dataGridView1.Width / 2;
            dataGridView1.AllowUserToAddRows = false;

            DataGridViewButtonColumn deleteBtn = new DataGridViewButtonColumn();
            deleteBtn.Text = "削除";
            deleteBtn.Width = dataGridView1.Width / 2;
            deleteBtn.UseColumnTextForButtonValue = true;
            deleteBtn.FlatStyle = FlatStyle.Flat;
            deleteBtn.DefaultCellStyle.BackColor = Color.Red;

            dataGridView1.Columns.Add(processName);
            dataGridView1.Columns.Add(deleteBtn);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string s = (string)listBox.SelectedItem;
            Console.WriteLine(s);

        }
    }
}
