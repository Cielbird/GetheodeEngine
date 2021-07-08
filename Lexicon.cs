using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class Lexicon
    {
        List<Morpheme> morphemes = new List<Morpheme>();

        public void AddMorpheme(List<Phoneme> phonemes)
        {
            morphemes.Add(new Morpheme(phonemes));
        }

        public List<Morpheme> GetMorphemes()
        {
            return morphemes;
        }


    }
}