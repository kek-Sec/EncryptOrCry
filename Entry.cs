using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrCry
{
    class Entry //entry object used to describe the properties of each entry.
    {
        public int index { set; get; }   
        public string Title { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }
        public string Password { set; get; }
        public string Comment { set; get; }
        public string Date { set; get; }
    }
}
