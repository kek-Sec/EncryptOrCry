using System;

namespace EncryptOrDie
{
    class Search
    {
        string[] content { get; set; }
        string searchtxt { get; set; }

        public Search()
        {

        }

        public int[] doSearch(String[] cnt, string search)
        {
            this.content = cnt;
            this.searchtxt = search;
            return Magic();
        }

        //returns ids with the order it thinks is right
        private int[] Magic()
        {
            int percent;
            int[] probs = new int[content.Length];
            int c,i;
            for (i = 0; i < content.Length; i++)
            {
                c = 0;
                foreach (char search_char in searchtxt)
                {
                    foreach (char content_char in content[i])
                    {
                        bool equal = char.ToUpperInvariant(search_char) == char.ToUpperInvariant(content_char);
                        if (equal) { c++; break; }
                    }
                }
                percent = (int)Math.Round((double)(100 * c) / content[i].Length);
                probs[i] = percent;

            }

            //Now sort by percentages.
            //Create new array that we will return

            int[] ret = new int[probs.Length];
            int retindex = 0;
            int max = 0;

            //time for mayhem
            for (int j = 0; j < probs.Length; j++)
            {
                for (i = 0; i < probs.Length; i++)
                {
                    if (probs[i] > max) { max = probs[i]; }
                }
                if (max == -1) { break; }
                for (i = 0; i < probs.Length; i++)
                {
                    if (probs[i] == max) { ret[retindex] = i; probs[i] = -1; retindex++; }
                }
                max = 0;
            }
            return ret;
        }
    }
}
