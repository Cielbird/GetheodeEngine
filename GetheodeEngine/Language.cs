using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Language
    {
        public string Name { get; set; }
        public List<Lect> Lects { get; set; }

        public Language(string name)
        {
            Name = name;
            Lects = new List<Lect>();
        }

        public override string ToString()
        {
            string tostring = Name + " :"+ GetHashCode() + "\n";
            foreach(Lect lect in Lects)
            {
                tostring += "\t" + lect.ToString();
            }
            return tostring;
        }
    }
}
