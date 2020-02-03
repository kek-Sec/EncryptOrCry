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

        private void abel2_Click(object sender, EventArgs e)
        {

        }

        //Generate pgp keyrign button
        private void Button5_Click(object sender, EventArgs e)
        {
            bool ret = false; string ret_str; //to return the GenerateKeyChain function
            string password = " ";
            if (InputBox.InputB("PGP generator", "Insert password for the pgp keyring.", ref password) == DialogResult.OK)
            {
                string email = "";
                if (InputBox.InputB("PGP generator", "Insert email for the pgp keyring.", ref email) == DialogResult.OK)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\EncryptOrCry";
                    //Create folder if it doesn't exist. @documents
                    if (Directory.Exists(path))
                    {
                        Status("\n FilePath for keys exists!");
                        String[] files = Directory.GetFiles(path);
                        if (files.Length > 0)
                        {
                            if (MessageBox.Show("There are files in the default directory.\nClicking yes will replace any keychain with the default file names.", "PGP generator", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                ret = GenerateKeyChain(path, email, password);
                                ret_str = (ret) ? "Success" : "Failed!";
                                Status("\nAttempt to Generate key chain ->" + ret_str);
                            }
                            else
                            {
                                Status("\nPGP Generation was canceled.");
                            }
                        }
                        else
                        {
                            ret = GenerateKeyChain(path, email, password);
                            ret_str = (ret) ? "Success" : "Failed!";
                            Status("\nAttempt to Generate key chain ->" + ret_str);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                        Status("\n FilePath for keys generated!");
                        ret = GenerateKeyChain(path, email, password);
                        ret_str = (ret) ? "Success" : "Failed!";
                        Status("\nAttempt to Generate key chain ->" + ret_str);
                    }
                }
            }
        }

        #region modules
        private bool GenerateKeyChain(string path,string email,string password)
        {
            using (PGP pgp = new PGP())
            {
                try
                {
                    pgp.GenerateKey(path + @"\public.asc", path + @"\private.asc", email, password,4096);
                    CheckList[1] = 1;
                    Properties.Settings.Default.public_key = path + @"\public.asc";
                    Properties.Settings.Default.Save();
                    return true;
                }
                catch(Exception e)
                {
                    CheckList[1] = 0;
                    return false;
                }
            }
        }
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

                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);
                Properties.Settings.Default.filepath = File.ToString();
                Properties.Settings.Default.Save();
                Status("\nAdded filepath ->" + File.ToString());
                CheckList[0] = 1;
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
            if(Properties.Settings.Default.use_auto_backup) { checkBox1.Checked = true; } else { checkBox1.Checked = false; }
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

        MainForm mf = new MainForm();
        private void Button1_Click_1(object sender, EventArgs e)
        {
            mf.Show();
            this.Close();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {

            Properties.Settings.Default.use_auto_backup = checkBox1.Checked;
            Properties.Settings.Default.Save();
            Status("\n Use AutoBackup -> " + Properties.Settings.Default.use_auto_backup);
        }

        //import pgp key ring
        private void Button6_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);
                Properties.Settings.Default.public_key = File.ToString();
                Properties.Settings.Default.Save();
                Status("\nAdded pub key ->" + File.ToString());
                CheckList[1] = 1;
            }
        }
    }
}
