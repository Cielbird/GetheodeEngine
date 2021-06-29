using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Phonology
    {
        readonly List<Phoneme> phonemes;

        public Phonology()
        {
            phonemes = new List<Phoneme>();
        }

        public void AddPhoneme(Phoneme phoneme)
        {
            phonemes.Add(phoneme);
        }

        public void RemovePhoneme(Phoneme phoneme)
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
