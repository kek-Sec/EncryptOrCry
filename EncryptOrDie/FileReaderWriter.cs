using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EncryptOrDie
{
    class FileReaderWriter
    {
        public string[] ReadFile(string path)
        {
            if(File.Exists(path))
            {
                return File.ReadAllLines(path);
            }
            else
            {
                return null;
            }
        }

        public bool WriteFile(string path,string[] content)
        {
            try
            {
                if(File.Exists(path))
                {
                    File.WriteAllLines(path, content);
                    return true;
                }
                else { return false; }
            }
            catch(Exception e)
            {
                return false;
            }
        }

    }
}
