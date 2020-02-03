using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncryptOrDie
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateEntries();
        }

        FileReaderWriter FRW = new FileReaderWriter();
        Entry[] entries = new Entry[200];
        Search s = new Search();
        int size;
        string[] Titles_Array = new string[200];
        public void CreateEntries()
        {
            string line;
            String[] aa = FRW.ReadFile(@"C:\Users\georg\Desktop\qq\output.txt");
            size = aa.Length;
            for(int i=0;i<aa.Length;i++)
            {
                line = aa[i];
                string[] ccc = line.Split(',');
                entries[i] = new Entry(i,ccc[0],ccc[1],ccc[2],ccc[3],ccc[4],ccc[5]);
                Titles_Array[i] = ccc[0];
            }
        }

        private void FIllListBox(int[] order)
        {
            for(int i=0;i<size;i++)
            {
                listBox1.Items.Add(entries[order[i]].Title);
                Console.WriteLine(order[i].ToString());
            }
            listBox1.Refresh();
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }


        private void Search_button_Click(object sender, EventArgs e)
        {
            string[] array_to_feed = new string[size];
            for(int i =0;i<size;i++) { array_to_feed[i] = Titles_Array[i]; }
            FIllListBox(s.doSearch(array_to_feed, search_textbox.Text));
        }
    }
}
