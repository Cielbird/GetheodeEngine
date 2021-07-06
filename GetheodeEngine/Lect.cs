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

        public List<IPAChar> GetSurfaceRepresentation(Morpheme morpheme)
        {
            List<IPAChar> UR = morpheme.UnderlyingRepresentation.ConvertAll(
                x => x.BaseRealization);

            foreach(PhonologicalRule rule in Phonology.Rules)
            {
                rule.ApplyToSurfaceRep(UR);
            }
            return UR;
        }

        public void AddMorpheme(string romanization)
        {
            Lexicon.AddMorpheme(Phonology.GetPhonemesFromRoman(romanization));
        }
    }
}
