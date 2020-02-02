using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PgpCore;

namespace EncryptOrCry
{
    public partial class FirstTimeSetup : Form
    {
        public FirstTimeSetup()
        {
            InitializeComponent();
        }

        private static string status = "";
        int[] CheckList = new int[2];
        //if not every checklist item is set to 1 we can't move forward.

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        //Generate pgp keyrign button
        private void Button5_Click(object sender, EventArgs e)
        {
            string password = " ";
            if (InputBox.InputB("PGP generator", "Insert password for the pgp keyring.", ref password) == DialogResult.OK)
            {
                string email = "";
                if (InputBox.InputB("PGP generator", "Insert email for the pgp keyring.", ref email) == DialogResult.OK)
                {

                }
            }
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EncryptOrCry";
            //Create folder if it doesn't exist. @documents
            if(Directory.Exists(path))
            {
                Status("\n FilePath for keys exists!");
                String[] files = Directory.GetFiles(path);
                if(files.Length>0)
                {
                    if(MessageBox.Show("PGP generator","There are files in the default directory.\nClicking yes will replace any keychain with the default file names.",MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        //generate keychain
                    }
                    else
                    {
                        Status("\nPGP Generation was canceled.");
                    }
                }
                //generate keychain
            }
            else
            {
                Directory.CreateDirectory(path);
                Status("\n FilePath for keys generated!");
                //generate keychain
            }
        }

        #region modules
        private void Status(string text)
        {
            status = status + text;
            richTextBox1.Clear();
            richTextBox1.Text = status;
        }
        private void BrowseFile()
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;

                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);
                Properties.Settings.Default.filepath = File.ToString();
                Status("\nAdded filepath ->" + File.ToString());
                CheckList[0] = 1;
                CheckList[1] = 1;
                //OR

                System.IO.StreamReader reader = new System.IO.StreamReader(fileToOpen);
                //etc
            }
        }
        #endregion
        //Browse folder button
        private void Button3_Click(object sender, EventArgs e)
        {
            BrowseFile();
        }

        private void FirstTimeSetup_Load(object sender, EventArgs e)
        {
            timer1.Start();
            button1.Enabled = false;
            button1.BackColor = Color.DarkGray;
        }
        //timer tick checks if every condition has been met to move forward.
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if(CheckList[0] == 1 && CheckList[1] == 1)
            {
                timer1.Stop();
                button1.BackColor = Color.Lime;
                button1.Enabled = true;
            }
            
        }
    }
}
