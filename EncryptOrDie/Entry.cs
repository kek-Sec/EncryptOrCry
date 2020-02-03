using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrDie
{
    class Entry
    {
        public int id { set; get; }

        public string Title { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }

        string Password { set; get; }

        string comment { set; get; }

        string date { set; get; }

        public Entry(int i,string t,string e,string u,string p,string c,string d)
        {
            id = i;
            Title = t;
            Email = e;
            UserName = u;
            Password = p;
            comment = c;
            date = d;
        }
        public Entry()
        {

        }
    }
}
