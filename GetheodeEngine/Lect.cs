using System;
namespace GetheodeEngine
{
    public class Lect
    {
        public string Name { get; set; }

        public Lect(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            string tostring = Name + " :" + GetHashCode() + "\n";
            return tostring;
        }
    }
}
