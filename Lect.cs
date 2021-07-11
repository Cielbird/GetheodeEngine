using System;
using System.Collections.Generic;

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
            Lexicon = new Lexicon();
        }

        public override string ToString()
        {
            string tostring = Name + " :" + GetHashCode() + "\n";
            tostring += Phonology.ToString();
            return tostring;
        }

        public SegmentSequence GetSurfaceRepresentation(Morpheme morpheme)
        {
            SegmentSequence UR = new SegmentSequence(morpheme);

            foreach(PhonologicalRule rule in Phonology.Rules)
            {
                UR.ApplyPhonologicalRule(rule);
            }
            return UR;
        }

        public void AddMorpheme(string romanization)
        {
            Lexicon.AddMorpheme(Phonology.GetPhonemesFromRoman(romanization));
        }
    }
}
