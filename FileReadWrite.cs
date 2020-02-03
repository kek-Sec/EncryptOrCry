using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace EncryptOrCry
{
    class FileReadWrite
    {

        //Write File @ path with content.
        public bool WriteFile(string path,string content)
        {
            try
            {
                if(File.Exists(path))
                {
                    StreamWriter sw = new StreamWriter(path);
                    sw.Write(content);
                    sw.Close();
                }
                else
                {
                    if (MessageBox.Show("File doesn't exist , do you want to create one?", "Question!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        File.Create(path);
                        MessageBox.Show(path);
                        StreamWriter sd = new StreamWriter(path);
                        sd.Write(content);
                        sd.Close();
                    }
                }
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }


        //read file @ path
        public string ReadFile(String path)
        {
            String ret;
            if(File.Exists(path))
            {
                try
                {
                    StreamReader sr = new StreamReader(path);
                    ret = sr.ReadToEnd();
                    sr.Close();
                }
                catch(Exception e)
                {
                    return "Error @ ReadFile()\n";
                }
                return ret;
            }
            else
            {
                return "FileNotFound";
            }
        }
    }
}
