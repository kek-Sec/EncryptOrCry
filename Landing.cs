using System;
using System.Windows.Forms;

namespace EncryptOrCry
{
    /*
     * Works as a routing module
     * if its the first time , display FirstTimeSetup
     * otherwise show login.
     */
    public partial class Landing : Form
    {
        Login l = new Login();
        FirstTimeSetup fts = new FirstTimeSetup();


        public Landing()
        {
            InitializeComponent();
        }

        #region events
        private void Landing_Load(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.First_time)
            {
                fts.Show(); //Show first Time setup.   
            }
            else
            {
                l.Show(); //Show login page.
            }
            this.Close();

        }

        private void Landing_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
                Application.Exit();
        }
        #endregion

    }
}
