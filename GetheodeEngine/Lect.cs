using System;
namespace GetheodeEngine
{
    public class Lect
    {
        public string Name { get; set; }
        public Phonology Phonology { get; set; }
        public Lexicon Lexicon { get; set; }

        public Lect(string name)
        {
            Name = name;
            Phonology = new Phonology();
        }

        public override string ToString()
        {
            string tostring = Name + " :" + GetHashCode() + "\n";
            tostring += Phonology.ToString();
            return tostring;
        }
    }
}
