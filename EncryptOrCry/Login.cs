﻿using System;
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
            label3.Visible = false;
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
            try { AES.DecryptFile(Properties.Settings.Default.login_test_file, Properties.Settings.Default.login_test_file + ".x", Properties.Settings.Default.aes_password_encrypted); }
            catch (Exception e1) { }
            File.Delete(Properties.Settings.Default.login_test_file + ".x");
            ShowMain();
            this.Dispose();
        }

        private void TextBox2_MouseClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
            label3.Visible = true;

        }

        private void TextBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Clipboard.SetText(textBox2.Text);
            label3.Visible = true;
        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
