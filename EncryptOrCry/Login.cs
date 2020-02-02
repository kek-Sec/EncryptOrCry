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
            //Generate test file.
            string FileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".in";
            string OutPutName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".out";
            String[] text = { "Welcome,", "If this works I will be so happy." };
            File.WriteAllLines(FileName, text);
            using (PGP pgp = new PGP())
            {
                
                using (FileStream inputFileStream = new FileStream(FileName, FileMode.Open))
                using (Stream outputFileStream = File.Create(OutPutName))
                using (Stream publicKeyStream = new FileStream(Properties.Settings.Default.public_key, FileMode.Open))
                    pgp.EncryptStream(inputFileStream, outputFileStream, publicKeyStream, true, true);
            }
            MessageBox.Show(File.ReadAllText(OutPutName));
            File.Encrypt(FileName);
            File.Delete(FileName);
            File.Encrypt(OutPutName);
            File.Delete(OutPutName);
        }

        private void Login_Load(object sender, EventArgs e)
        {
            DecryptChallenge();
        }
    }
}
