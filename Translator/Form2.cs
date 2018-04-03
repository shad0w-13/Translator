using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translator
{
    public partial class Form2 : Form
    {
        public String word { get; set; }
        public String translation { get; set; }
        public string Label1Text
        {
            get
            {
                return this.label1.Text;
            }
            set
            {
                this.label1.Text = value;
            }
        }
        public string Label2Text
        {
            get
            {
                return this.label2.Text;
            }
            set
            {
                this.label2.Text = value;
            }
        }

        public Form2(ListView listView)
        {
            //Label1Text = listView.Columns[0].Text;
            //Label2Text = listView.Columns[1].Text;
            StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                word = textBox1.Text;
                translation = textBox2.Text;
            }
        }
    }
}
