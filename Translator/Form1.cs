using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

namespace Translator
{
    public partial class Form1 : Form
    {
        private int sortColumn = -1;

        public Form1()
        {
            StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files|*.txt"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                listView1.Items.Clear();

                String[] dictionary = File.ReadAllLines(openFileDialog.FileName).Distinct().ToArray();
                
                String[] languages = dictionary[0].Split(' ');

                listView1.Columns[0].Text = languages[0];
                listView1.Columns[1].Text = languages[1];

                for (int i = 1; i < dictionary.Length; i++)
                {
                    String[] words = dictionary[i].Split(' ');

                    if (words[0].All(Char.IsLetter) && words[1].All(Char.IsLetter) && words[0] != "" && words[1] != "")
                        listView1.Items.Add(new ListViewItem(words));
                }
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text files|*.txt"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.WriteLine($"{listView1.Columns[0].Text} {listView1.Columns[1].Text}");

                    foreach (ListViewItem item in listView1.Items)
                    {
                        writer.WriteLine($"{item.SubItems[0].Text} {item.SubItems[1].Text}");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String toTranslate = richTextBox1.Text;
            String[] lines = toTranslate.Split('\n');
            richTextBox2.Text = "";

            foreach (String line in lines)
            {
                String[] words = line.Split(' ');
                foreach (String word in words)
                {
                    bool wordsExists = false;
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        if (Regex.IsMatch(word, listView1.Items[i].SubItems[0].Text,
                            RegexOptions.IgnoreCase))
                        {
                            richTextBox2.SelectionColor = Color.Black;
                            richTextBox2.AppendText(listView1.Items[i].SubItems[1].Text + " ");
                            wordsExists = true;
                        }
                    }
                    if (!wordsExists)
                    {
                        richTextBox2.SelectionColor = Color.Red;
                        richTextBox2.AppendText(word + " ");
                    }
                }
                richTextBox2.AppendText("\n");
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            listView1.Sort();
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column,
                                                              listView1.Sorting);
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (ListViewItem item in ((ListView) sender).SelectedItems)
                {
                    item.Remove();
                }
            }
        }

        // Implements the manual sorting of items by column.
        class ListViewItemComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public ListViewItemComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public ListViewItemComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text,
                                        ((ListViewItem)y).SubItems[col].Text);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                return returnVal;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form2 addNewWord = new Form2(listView1);
            addNewWord.ShowDialog(this);
            addNewWord.Label1Text = listView1.Columns[0].Text;
            addNewWord.Label2Text = listView1.Columns[1].Text;

            if (addNewWord.DialogResult == DialogResult.OK)
            {
                String[] word = { addNewWord.word, addNewWord.translation };
                
                listView1.Items.Add(new ListViewItem(word));

                addNewWord.Dispose();
            }
            if (addNewWord.DialogResult == DialogResult.Cancel)
                addNewWord.Dispose();
        }

        private void FontStyleButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font(richTextBox1.Font,
                (toolStripButton1.Checked ? FontStyle.Bold : FontStyle.Regular) |
                (toolStripButton2.Checked ? FontStyle.Italic : FontStyle.Regular) |
                (toolStripButton3.Checked ? FontStyle.Underline : FontStyle.Regular)
                );
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog
            {
                AllowFullOpen = true
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
                richTextBox1.ForeColor = colorDialog.Color;
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog()
            {
                AllowFullOpen = true
            };

            if (colorDialog.ShowDialog() == DialogResult.OK)
                richTextBox1.BackColor = colorDialog.Color;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            List<String> fonts = new List<string>();

            foreach (FontFamily font in FontFamily.Families)
                fonts.Add(font.Name);

            foreach (String font in fonts)
                toolStripComboBox1.Items.Add(font);
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox1.Font = new Font(toolStripComboBox1.SelectedItem.ToString(), 12);
        }
    }
}