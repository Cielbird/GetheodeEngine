using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Phonology
    {
        List<Phoneme> phonemes;
        List<PhonologicalRule> rules;

        public Phonology()
        {
            phonemes = new List<Phoneme>();
            rules = new List<PhonologicalRule>();
        }

        public void AddRule(string rule)
        {
            rules.Add(new PhonologicalRule(rule));
        }

        public void RemoveRule(Phoneme phoneme)
        {
            phonemes.Remove(phoneme);
        }

        public override string ToString()
        {
            string tostring = "";
            foreach (Phoneme phoneme in phonemes)
            {
                tostring += "--"+phoneme.ToString()+"\n";
            }
            return tostring;
        }
    }
}
