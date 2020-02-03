using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptOrCry
{
    class CoreJson
    {
        public int Size { get; set; }   //number of entries

        public String LastEditDate { get; set; } //Last time of edit

        public List<Entry> Entries { get; set; } //list of entries

    }
}
