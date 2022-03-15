using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class Phonology
    {
        public List<Phoneme> Phonemes { get; }
        public List<PhonologicalRule> Rules { get; }
        public PhonotacticGraph Phonotactics { get; set; }

        public Phonology()
        {
            Phonemes = new List<Phoneme>();
            Rules = new List<PhonologicalRule>();
            Phonotactics = new PhonotacticGraph("MORPH = ", this);
        }

        public void AddRule(string rule)
        {
            Rules.Add(new PhonologicalRule(rule));
        }

        public void RemoveRule(PhonologicalRule rule)
        {
            Rules.Remove(rule);
        }

        /// <summary>
        /// Adds a phoneme with a romanization identical to the base realization
        /// </summary>
        public void AddPhoneme(IPAChar baseRealization)
        {
            AddPhoneme(baseRealization, baseRealization.ToString());
        }

        /// <summary>
        /// Adds a phoneme with romanization string and base realization
        /// </summary>
        public void AddPhoneme(IPAChar baseRealization, string romanization)
        {
            foreach (Phoneme p in Phonemes)
            {
                if (p.Romanization == romanization)
                    throw new ArgumentException("The romanization \"" + romanization + "\" already exists");
            }
            Phonemes.Add(new Phoneme(romanization, baseRealization));
        }

        /// <summary>
        /// Gets a sequence of phonemes from the romanization
        /// </summary>
        /// <param name="roman">the romanization to convert into \
        /// phonemes</param>
        /// <returns>A List if phonemes whose romanization matches
        /// `roman`</returns>
        public List<Phoneme> GetPhonemesFromRoman(string roman)
        {
            //strip whitespace
            roman = Regex.Replace(roman, @"\s+", "");

            List<string> allPhonemes = Phonemes.ConvertAll(e => e.Romanization);

            string[] splitRomanization = roman.SplitUsingKeywords(allPhonemes);

            List<Phoneme> phonemes = new();
            foreach (string phonemeRoman in splitRomanization)
            {
                foreach(Phoneme phoneme in Phonemes)
                {
                    if (phonemeRoman == phoneme.Romanization)
                        phonemes.Add(phoneme);
                }
            }

            return phonemes;
        }

        public override string ToString()
        {
            string tostring = "";
            foreach (Phoneme phoneme in Phonemes)
            {
                tostring += "--"+phoneme.ToString()+"\n";
            }
            return tostring;
        }
    }
}
