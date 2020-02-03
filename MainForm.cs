using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
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


        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]//  Call this function to remove the key from memory after use for security
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
        public static bool ShouldEncrypt = false; //to avoid encrypting already encrypted file.
        public static bool Selected = false; //if any entry is selected.
        public static int current_entry; //index of current entry
        public readonly string filepath = Properties.Settings.Default.filepath; //Filepath for the json
        static List<Entry> Entries = new List<Entry>(); //collection of all the entries
        public static int mode; //1 -> edit 2->add 3->view
        public static int size = 0; //size of Entries array. defined in core json.
        FileReadWrite frw = new FileReadWrite();    //FileReadWrite service



        #region events
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.First_time)
            { ShouldEncrypt = true; }
            else
            {
                init_decrypt();
            }
            Properties.Settings.Default.First_time = false;
            Properties.Settings.Default.Save();
            ModeHandler(3); //set view mode.
            LoadEntries(); //Load entries in List<>
            Refresh_items(); //Refresh browser and listbox
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Properties.Settings.Default.use_auto_backup && Properties.Settings.Default.backup_counter == 5) { CreateBackup(); Properties.Settings.Default.backup_counter = 0; }
            Properties.Settings.Default.backup_counter++;
            Properties.Settings.Default.Save();
            EncryptAndExit();
        }
        #endregion

        #region BrowseDB
        //Browse DB
        //Items Managment
        //Pagination
        private static int page = 0; //curent page


        //returns true if the page is full
        //returns false if the page is not full
        private static int[] ids;
        private bool AddDataToItems(int page_to_get) //needs clear-up.
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

        //Saves the json file- No encryption happens at this step.
        private void Save()
        {
            frw.WriteFile(filepath, MakeJson());
            page = 0; AddDataToItems(page);
            ClearTextBoxes();
            Refresh_items();
            ModeHandler(3);
            SystemSounds.Beep.Play();
        }

        //Add entry to Entries array
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
            temp.Clear();
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
                MessageBox.Show(e.Message);
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
                ModeHandler(3);
            }

        }

        #endregion

        #region JSON
        readonly static DateTime dateTime = DateTime.UtcNow.Date;
        //used for the LastEditDate property

        private string MakeJson() //Create json file. Returns json string.
        {
            CoreJson cj = new CoreJson()
            {
                Size = size,
                LastEditDate = dateTime.ToString("dd/MM/yy"),
                Entries = Entries
            };
            return JsonConvert.SerializeObject(cj, Formatting.Indented);
        }

        private CoreJson LoadJson() //load json and parse it.
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
            if (!Selected && mode == 1) { MessageBox.Show("Select Something to edit.."); } //if user tries to edit while nothing is selected.
            else
            {
                EnableButtons(mode);
                HandleTextBoxes(mode);
            }


        }
        #endregion

        #region buttons
        private void Cancel_button_Click(object sender, EventArgs e)
        {
            ModeHandler(3);
        }

        //raw button
        private void Button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(MakeJson());
        }

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
            //if (mode == 1) { Delete_button.Visible = true; }
        }
        private void Prevpage_button_Click(object sender, EventArgs e)
        {
            prevpage();
        } //pagination
        private void Nextpage_button_Click(object sender, EventArgs e)
        {
            nextpage();
        } //pagination
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
            String[] textboxes_values;
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
            ModeHandler(1);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ModeHandler(2);
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
            ModeHandler(2);
        }
        private void Delete_button_Click(object sender, EventArgs e)
        {
            if (Selected)
            {
                if (MessageBox.Show("Delete entry?", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    Delete(current_entry);
                }
            }
        }
        #endregion

        #region TextBoxes
        bool SearchMode = false;
        int[] SearchOrder;
        Search s = new Search();
        //search textbox
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            String[] Titles = new string[size];
            int i = 0;
            foreach (Entry en in Entries)
            {
                Titles[i++] = en.Title;
            }
            SearchMode = true;
            SearchOrder = s.doSearch(Titles, textBox1.Text);
            FillListBox(SearchOrder);
        }

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
            CopyTextBox(email_textbox);
        }

        private void User_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            CopyTextBox(user_textbox);
        }

        private void Password_textbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            password_textbox.UseSystemPasswordChar = false;
            CopyTextBox(password_textbox);
            password_textbox.UseSystemPasswordChar = true;
        }
        #endregion
        private void CopyTextBox(TextBox tb)
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
            if(SearchMode) { SelectEntry(SearchOrder[listBox1.SelectedIndex]); }
            else { SelectEntry(listBox1.SelectedIndex);}
            
        }

        #endregion

        #region utils
        private void init_decrypt()
        {
            try
            {
                string tmp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
                string tmp2 = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tm";
                File.Copy(filepath, tmp);
                //comment out for testing
                AES.DecryptFile(tmp, tmp2, Properties.Settings.Default.aes_password_encrypted); ShouldEncrypt = true;
                File.Delete(filepath);
                File.Copy(tmp2, filepath);
                //up untill here
                File.Delete(tmp);
                File.Delete(tmp2);
            }
            catch (Exception e)
            { MessageBox.Show("Wrong password."); this.Close(); ShouldEncrypt = false; }
        }

        public void Refresh_items()
        {
            AddDataToItems(0); //fill browser
            FillListBox();
        }


        private void CreateBackup()
        {
            String[] message = new string[3];
            message[0] = "EncryptOrCry - Backup";
            string fp = Properties.Settings.Default.filepath;
            if (File.Exists(fp + ".backup")) { File.Delete(fp + ".backup"); }
            File.Copy(fp, fp + ".backup");
            string tmp = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            File.Copy(fp, tmp);
            message[1] = GeneratePassword(8);
            AES.EncryptFile(tmp, fp + ".backup", message[1]);
            if (File.Exists(fp + ".txt")) { File.Delete(fp + ".txt"); }
            File.WriteAllText(fp + ".txt", PGP_Service.PGPEncryptMessage(message));
            File.Delete(tmp);

        }
        private class BtcData
        {
            public string hash { get; set; }
            public int time { get; set; }
            public int block_index { get; set; }
            public int height { get; set; }
            public List<int> txIndexs { get; set; }

        }

        private string getLatestBitcoinBlock()
        {
            BtcData bd;
            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("https://blockchain.info/latestblock");
                bd = JsonConvert.DeserializeObject<BtcData>(htmlCode);
            }
            return bd.hash;
        }

        private void EncryptAndExit()
        {
            if (ShouldEncrypt)
            {
                ShouldEncrypt = false;
                string[] message = new string[6];
                string file_path = Properties.Settings.Default.filepath;
                string login_temp = Properties.Settings.Default.login_test_file;
                string aes_password = GeneratePassword(8);
                GCHandle gch = GCHandle.Alloc(aes_password, GCHandleType.Pinned);
                string file_path_content = File.ReadAllText(file_path);
                string tmp_file = QuickTempFile(file_path_content);
                string tmp_login_file = QuickTempFile(GeneratePassword(256));
                if (File.Exists(login_temp)) { File.Delete(login_temp); }
                AES.EncryptFile(tmp_login_file, file_path + ".login", aes_password);
                Properties.Settings.Default.login_test_file = file_path + ".login";
                Properties.Settings.Default.Save();
                AES.EncryptFile(tmp_file, file_path, aes_password);
                File.WriteAllText(tmp_file, "1010101010");
                File.Delete(tmp_file);
                message[0] = "EncryptOrDie";
                message[1] = "#------------------------------------------OneTimePassword------------------------------------#";
                message[2] = "                                       Password:" + aes_password;
                message[3] = "                            This password will be destroyed after you login.";
                message[4] = "     last btc block -> " + getLatestBitcoinBlock();
                message[5] = "#---------------------------------------------------------------------------------------------#";
                Properties.Settings.Default.aes_password_encrypted = PGP_Service.PGPEncryptMessage(message);
                Properties.Settings.Default.Save();
                ZeroMemory(gch.AddrOfPinnedObject(), aes_password.Length * 2);
                gch.Free();
                Application.Exit();
            }
            else
            {
                Application.Exit();
            }
        }

        private void prevpage()
        {
            if (page > 0) { AddDataToItems(--page); prevpage_button.Cursor = Cursors.Default; } else { prevpage_button.Cursor = Cursors.No; }
        }
        private void nextpage()
        {
            AddDataToItems(++page);
            prevpage_button.Cursor = Cursors.Default;
        }
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
                listBox1.Items.Add(Entries[queue[i]].Title);
            }
        }

        private static Random random = new Random();
        public static string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+=-qwertyuiopasdfghjkl;'zxcvbnm,./";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }




        //returns temp file path
        private string QuickTempFile(string input)
        {
            string FileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            File.WriteAllText(FileName, input);
            return FileName;

        }
        #endregion





    }
}
