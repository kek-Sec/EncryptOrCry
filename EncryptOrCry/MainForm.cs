using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EncryptOrCry
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public readonly string filepath = Properties.Settings.Default.filepath;
        static List<Entry> Entries = new List<Entry>();
        public static int mode;
        public static int size=0;
        FileReadWrite frw = new FileReadWrite();

        private void MainForm_Load(object sender, EventArgs e)
        {
            mode = 2;
            ModeHandler(mode); //set view mode.
            LoadEntries();
        }

        #region BrowseDB
        //Browse DB
        //Items Managment
        //Pagination
        private static int page = 0;



        private void AddDataToItems(int page_to_get)
        {
            //pages analoga me data length /5.
            TextBox[] items = { item_1_textbox1, item1_textbox2, item2_textbox1, item2_textbox2, item3_textbox1, item3_textbox2, item4_textbox1, item4_textbox2, item5_textbox1, item5_textbox2 };

        }
        #endregion

        #region Loader

        //Takes CoreJson object and adds it to the Entries List.
        private void LoadEntries()
        {
            CoreJson cj = LoadJson();
            foreach(Entry e in cj.Entries)
            {
                Entries.Add(e);
            }
        }

        #endregion


        #region JSON
        readonly static DateTime dateTime = DateTime.UtcNow.Date;
        private string MakeJson()
        {
            CoreJson cj = new CoreJson()
            {
                Size = size,
                LastEditDate = dateTime.ToString("dd/MM/yy"),
                Entries = Entries
            };
            return JsonConvert.SerializeObject(cj, Formatting.Indented);
        }

        private CoreJson LoadJson()
        {
            Entries.Clear();
            var core_json = JsonConvert.DeserializeObject<CoreJson>(frw.ReadFile(filepath));
            return core_json;
        }
        #endregion
        #region Modes
        //3 modes
        //Edit , Add , Read.
        private void ModeHandler(int mode)
        {
            EnableButtons(mode);
            HandleTextBoxes(mode);
        }
        #endregion
        #region buttons

        private void HideAllButtons()
        {
            Button[] all_buttons = { Save_button, cancel_button, Delete_button, Edit_button, insert_button, Generate_password_button };
            foreach (Button b in all_buttons)
            {
                b.Visible = false;
            }
        }
        private void EnableButtons(int mode)
        {
            //1 edit
            //2 add
            //3 view
            HideAllButtons();
            if (mode == 1 || mode == 2)
            {
                Save_button.Visible = true;
                cancel_button.Visible = true;
                Generate_password_button.Visible = true;
            }
            else if (mode == 3)
            {
                Edit_button.Visible = true;
                insert_button.Visible = true;
                Delete_button.Visible = true;
            }
        }
        private void Prevpage_button_Click(object sender, EventArgs e)
        {
            if (page > 0) { AddDataToItems(--page); prevpage_button.Cursor = Cursors.Default; } else { prevpage_button.Cursor = Cursors.No; }

        }
        private void Nextpage_button_Click(object sender, EventArgs e)
        {
            AddDataToItems(++page);
            prevpage_button.Cursor = Cursors.Default;
        }
        private void Button5_Click(object sender, EventArgs e)//Generate password button
        {
            //if textbox is empty just input the password otherwise ask for a yes or no to delete and write.
            if (!String.IsNullOrEmpty(password_textbox.Text))
            {
                if (MessageBox.Show("Delete old password?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    password_textbox.Text = GeneratePassword(16);
                }

            }
            else { password_textbox.Text = GeneratePassword(16); }

        }
        #endregion
        #region TextBoxes
        //Edit , add ,Read
        private void HandleTextBoxes(int mode)
        {
            TextBox[] main_textboxes = { title_textbox, user_textbox, email_textbox, password_textbox, note_textbox };
            if (mode == 2) { ClearTextBoxes(); }
            foreach (TextBox tb in main_textboxes)
            {

                if (mode == 1 || mode == 2) { tb.ReadOnly = false; } else { tb.ReadOnly = true; }
            }
        }
        public void ClearTextBoxes()
        {
            TextBox[] main_textboxes = { user_textbox, email_textbox, password_textbox, note_textbox, title_textbox };
            foreach (TextBox tb in main_textboxes)
            {
                tb.Clear();
            }
        }

        //return all the content of the textboxes in an array, if empty return whitespace.
        public String[] grabTextboxes()
        {
            TextBox[] main_textboxes = { title_textbox, email_textbox, user_textbox, password_textbox, note_textbox };
            int i = 0;
            String[] ret = new string[6];

            foreach (TextBox tb in main_textboxes)
            {
                if (String.IsNullOrEmpty(tb.Text)) { ret[i] = " "; } else { ret[i] = tb.Text; }
                i++;
            }
            DateTime dateTime = DateTime.UtcNow.Date;
            ret[i] = dateTime.ToString("dd/MM/yy");
            return ret;
        }
        #region events
        private void Email_textbox_MouseClick(object sender, MouseEventArgs e)
        {
            email_textbox.SelectAll();
        }

        private void User_textbox_MouseClick(object sender, MouseEventArgs e)
        {
            user_textbox.SelectAll();
        }

        public bool flag = false;//used for password
        private void Password_textbox_MouseClick(object sender, MouseEventArgs e)
        {
            password_textbox.UseSystemPasswordChar = flag;
            password_textbox.SelectAll();
            flag = (flag) ? false : true;
        }

        //Used for textbox color
        public static void OnTimerEvent(object source, EventArgs e, Timer t, TextBox tb)
        {
            t.Stop();
            tb.BackColor = Color.White;
            t.Dispose();
        }
        private void Email_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CopyTextBox(email_textbox, 0);
        }

        private void User_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CopyTextBox(user_textbox, 1);
        }

        private void Password_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            password_textbox.UseSystemPasswordChar = false;
            CopyTextBox(password_textbox, 2);
            password_textbox.UseSystemPasswordChar = true;
        }
        #endregion
        private void CopyTextBox(TextBox tb, int timer_id)
        //Add text to clipboard
        //Set textbox color for 4 seconds.
        {
            Timer t1;
            if (String.IsNullOrEmpty(tb.Text)) { }
            else
            {
                tb.BackColor = Color.LimeGreen;
                Clipboard.SetText(tb.Text);
                t1 = new Timer() { Interval = 4000, Enabled = true };
                t1.Start();
                t1.Tick += delegate (object source, EventArgs e) { OnTimerEvent(source, e, t1, tb); };
            }

        }



        #endregion
        #region utils

        private static Random random = new Random();
        public static string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+=-qwertyuiopasdfghjkl;'zxcvbnm,./";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }



        #endregion

        private void Save_button_Click(object sender, EventArgs e)
        {
            String[] textboxes_values = new string[6];
            textboxes_values = grabTextboxes();
            Entry entry;
            if (mode == 2)
            //add mode
            {
                entry = new Entry
                {
                    index = size,
                    Title = textboxes_values[0] ,
                    Email = textboxes_values[1] ,
                    UserName = textboxes_values[2],
                    Password = textboxes_values[3],
                    Comment = textboxes_values[4],
                    Date = textboxes_values[5]
                };
                Entries.Add(entry);
                size++;
                frw.WriteFile(@"", MakeJson());
                LoadEntries();
            }
        }
    }
}
