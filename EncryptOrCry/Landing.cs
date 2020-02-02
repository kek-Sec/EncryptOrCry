using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncryptOrCry
{
    public partial class Landing : Form
    {
        public Landing()
        {
            InitializeComponent();
        }

        MainForm mf = new MainForm();
        Login l = new Login();
        FirstTimeSetup fts = new FirstTimeSetup();

        private void Landing_Load(object sender, EventArgs e)
        {
            fts.Show();
            this.Hide();
        }
    }
}
