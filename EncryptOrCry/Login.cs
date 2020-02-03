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
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }
        private void DecryptChallenge()
        {
            textBox2.Text = Properties.Settings.Default.aes_password_encrypted;
        }

        private void CheckIfPublicKeyExists()
        {
            if(!File.Exists(Properties.Settings.Default.public_key))
            {
                MessageBox.Show("Public key can not be found! , please show me the way..\n", "Error!");
                var FD = new OpenFileDialog();
                if (FD.ShowDialog() == DialogResult.OK)
                {

                    FileInfo File = new System.IO.FileInfo(FD.FileName);
                    Properties.Settings.Default.public_key = File.ToString();
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void Login_Load(object sender, EventArgs e)
        {
            CheckIfPublicKeyExists();
            DecryptChallenge();

        }

        public void ShowMain()
        {
            MainForm mfm = new MainForm();
            mfm.Show();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.aes_password_encrypted = textBox1.Text;
            ShowMain();
            this.Dispose();
        }
    }
}
