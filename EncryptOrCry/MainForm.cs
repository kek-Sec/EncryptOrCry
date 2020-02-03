using Newtonsoft.Json;
using PgpCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace EncryptOrCry
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }

        //  Call this function to remove the key from memory after use for security
        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
        public static bool ShouldEncrypt = false; //to avoid encrypting already encrypted file.
        public static bool Selected = false;
        public static int current_entry;
        public readonly string filepath = Properties.Settings.Default.filepath;
        static List<Entry> Entries = new List<Entry>();
        public static int mode;
        public static int size = 0;
        FileReadWrite frw = new FileReadWrite();

        private void MainForm_Load(object sender, EventArgs e)
        {
            init_decrypt();
            mode = 3;
            ModeHandler(mode); //set view mode.
            LoadEntries();
            Refresh();
        }

        private void init_decrypt()
        {
            string p = Properties.Settings.Default.filepath;
            try
            {
                string tmp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
                File.Copy(p, tmp);
                AES.DecryptFile(tmp, p, Properties.Settings.Default.aes_password_encrypted); ShouldEncrypt = true;
            }
            catch (Exception e)
            { MessageBox.Show("Wrong password."); this.Close(); ShouldEncrypt = false; }

        }
        private void Refresh()
        {
            AddDataToItems(0); //fill browser
            FillListBox();
        }

        #region BrowseDB
        //Browse DB
        //Items Managment
        //Pagination
        private static int page = 0;


        //returns true if the page is full
        //returns false if the page is not full
        private static int[] ids;
        private bool AddDataToItems(int page_to_get)
        {
            int page_magic = page_to_get * 4;
            String[] content = new string[10];
            ids = new int[5];
            int c_ids = 0;
            int c_indx = 0;
            int c_indxd = 1;
            String[] Titles = new string[5];
            int t_indx = 0;
            bool ret = false;
            //pages analoga me data length /5.
            Label[] titlez = { label3, label8, label10, label13, label16 };
            TextBox[] items = { item_1_textbox1, item1_textbox2, item2_textbox1, item2_textbox2, item3_textbox1, item3_textbox2, item4_textbox1, item4_textbox2, item5_textbox1, item5_textbox2 };
            for (int i = 0; i < 5; i++)
            {
                if ((i + page_magic) >= Entries.Count)
                {
                    ret = false;
                    break;
                }
                else
                {
                    ids[c_ids] = Entries[i + page_magic].index;
                    c_ids++;
                    content[c_indx] = Entries[i + page_magic].Email;
                    c_indx = c_indx + 2;
                    content[c_indxd] = Entries[i + page_magic].Password;
                    c_indxd = c_indxd + 2;
                    Titles[t_indx] = Entries[i + page_magic].Title;
                    t_indx++;
                    ret = true;
                }
                //reset the rest ids.
                for (int x = c_ids; x < 5; x++)
                {
                    ids[c_ids] = -1;
                }
            }
            //Clear textboxes.
            foreach (TextBox tb in items)
            {
                tb.Clear();
            }
            //Fill textboxes.
            for (int j = 0; j <= c_indxd - 2; j++)
            {
                items[j].Text = content[j];
            }
            int c = 0;
            foreach (Label l in titlez)
            {
                l.Text = Titles[c++];
            }

            return ret;

        }
        #endregion

        #region Loader-Entry managment

        //Saves the file.
        private void Save()
        {
            frw.WriteFile(filepath, MakeJson());
            page = 0; AddDataToItems(page);
            ClearTextBoxes();
            Refresh();
            mode = 3;
            ModeHandler(mode);
            SystemSounds.Beep.Play();
        }
        private void Add(Entry e)
        {
            Entries.Add(e);
            size++;
            Save();
        }
        //Used by edit mode, removes entry at specific index and adds a new one.
        private void Replace(int index, Entry e)
        {
            Entries[index] = e;
            Save();
        }

        //Delete entry at specific index , move all entries one index back.
        private void Delete(int index)
        {
            size = 0;
            List<Entry> temp = new List<Entry>();
            int i = 0;
            Entries.RemoveAt(index);
            foreach (Entry e in Entries)
            {
                size++;
                temp.Add(e);
                e.index = i++;
            }
            Entries.Clear();
            Entries = temp;
            Save();
        }

        //Takes CoreJson object and adds it to the Entries List.
        private void LoadEntries()
        {
            try
            {
                CoreJson cj = LoadJson();
                size = cj.Size;
                foreach (Entry e in cj.Entries)
                {
                    Entries.Add(e);
                }
            }
            catch (Exception e)
            {

            }
        }

        private void SelectEntry(int index)
        {
            if (index == -1) { }
            else
            {
                Selected = true;
                current_entry = index;
                FillTextboxes(Entries[index]);
                mode = 1;
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
            Console.WriteLine(current_entry);
            if (!Selected) { }
            else
                EnableButtons(mode);
            HandleTextBoxes(mode);

            if (mode == 2 || mode == 3)
            {
                EnableButtons(mode);
                HandleTextBoxes(mode);
            }

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
            if (mode == 1) { Delete_button.Visible = true; }
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
        private void Save_button_Click(object sender, EventArgs e)
        {
            String[] textboxes_values = new string[6];
            textboxes_values = grabTextboxes();
            Entry entry;
            entry = new Entry
            {
                index = size,
                Title = textboxes_values[0],
                Email = textboxes_values[1],
                UserName = textboxes_values[2],
                Password = textboxes_values[3],
                Comment = textboxes_values[4],
                Date = textboxes_values[5]
            };
            if (mode == 2)
            //add mode
            {
                Add(entry);
            }
            if (mode == 1)
            //edit mode
            {
                entry.index = current_entry;
                Replace(current_entry, entry);
            }
            mode = 2;
        }
        private void Edit_button_Click(object sender, EventArgs e)
        {
            mode = 1;
            ModeHandler(mode);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            mode = 2;
            ModeHandler(mode);
        }
        private void Label3_Click(object sender, EventArgs e)
        {
            SelectEntry(ids[0]);
        }

        private void Label8_Click(object sender, EventArgs e)
        {
            SelectEntry(ids[1]);
        }

        private void Label10_Click(object sender, EventArgs e)
        {
            SelectEntry(ids[2]);
        }

        private void Label13_Click(object sender, EventArgs e)
        {
            SelectEntry(ids[3]);
        }

        private void Label16_Click(object sender, EventArgs e)
        {
            SelectEntry(ids[4]);
        }
        private void Insert_button_Click(object sender, EventArgs e)
        {
            mode = 2;
            ModeHandler(mode);
        }
        private void Delete_button_Click(object sender, EventArgs e)
        {
            if (Selected) { Delete(current_entry); }
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
            date_label.Text = "";
        }
        //s textboxes
        private void FillTextboxes(Entry e)
        {
            TextBox[] mt = { title_textbox, email_textbox, user_textbox, password_textbox, note_textbox };
            mt[0].Text = e.Title;
            mt[1].Text = e.Email;
            mt[2].Text = e.UserName;
            mt[3].Text = e.Password;
            mt[4].Text = e.Comment;
            date_label.Text = e.Date;
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


        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectEntry(listBox1.SelectedIndex);
        }

        #endregion
        #region utils

        private void FillListBox()
        {
            listBox1.Items.Clear();
            foreach (Entry e in Entries)
            {
                listBox1.Items.Add(e.Title);
            }
        }

        private void FillListBox(int[] queue)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < queue.Length; i++)
            {
                listBox1.Items.Add(Entries[queue[i]]);
            }
        }

        private static Random random = new Random();
        public static string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+=-qwertyuiopasdfghjkl;'zxcvbnm,./";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private string PGPEncryptMessage(string[] input)
        {
            string FileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".in";
            string OutPutName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".out";
            File.WriteAllLines(FileName, input);
            using (PGP pgp = new PGP())
            {

                using (FileStream inputFileStream = new FileStream(FileName, FileMode.Open))
                using (Stream outputFileStream = File.Create(OutPutName))
                using (Stream publicKeyStream = new FileStream(Properties.Settings.Default.public_key, FileMode.Open))
                    pgp.EncryptStream(inputFileStream, outputFileStream, publicKeyStream, true, true);
            }
            String output = File.ReadAllText(OutPutName);
            File.WriteAllText(FileName, "010101010");
            File.WriteAllText(OutPutName, "0110101010");
            File.Delete(FileName);
            File.Delete(OutPutName);
            return output;
        }

        //returns temp file path
        private string QuickTempFile(string input, string path)
        {
            string FileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            File.WriteAllText(FileName, input);
            return FileName;

        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ShouldEncrypt)
            {
                ShouldEncrypt = false;
                string[] message = new string[3];
                string file_path = Properties.Settings.Default.filepath;
                string aes_password = GeneratePassword(8);
                GCHandle gch = GCHandle.Alloc(aes_password, GCHandleType.Pinned);
                string file_path_content = File.ReadAllText(file_path);
                string tmp_file = QuickTempFile(file_path_content, file_path);
                AES.EncryptFile(tmp_file, file_path, aes_password);
                File.WriteAllText(tmp_file, "1010101010");
                File.Delete(tmp_file);
                message[0] = "EncryptOrDie";
                message[1] = "#----OneTimePassword----#";
                message[2] = "Your password is --> " + aes_password;
                Properties.Settings.Default.aes_password_encrypted = PGPEncryptMessage(message);
                Properties.Settings.Default.Save();
                ZeroMemory(gch.AddrOfPinnedObject(), aes_password.Length * 2);
                gch.Free();
                Application.Exit();
            }
            else
            {
                this.Hide();
            }
        }

    }
}
