using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class Phonology
    {
        public List<Phoneme> Phonemes { get; }
        public List<PhonologicalRule> Rules { get; }

        public Phonology()
        {
            Phonemes = new List<Phoneme>();
            Rules = new List<PhonologicalRule>();
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
            List<Phoneme> phonemes = new List<Phoneme>();
            int parseIndex = 0;
            while (parseIndex < roman.Length)
            {
                //get best match (ex, 'ts' is a better match than 't')
                Phoneme bestMatch = null;
                foreach (Phoneme phoneme in Phonemes)
                {
                    MatchCollection matches = Regex.Matches(roman, phoneme.Romanization);
                    foreach (Match match in matches)
                    {
                        if (match.Success && match.Index == parseIndex)
                        {
                            if (bestMatch == null || phoneme.Romanization.Length > bestMatch.Romanization.Length)
                            {
                                bestMatch = phoneme;
                                break;
                            }
                        }
                    }
                }

                //if no match is found, keep looking 
                if (bestMatch == null)
                    throw new ArgumentException(
                        "the romanization is invalid, couldn't find any " +
                        "phonemes for \"" + roman.Substring(parseIndex) + "\"");

                phonemes.Add(bestMatch);
                parseIndex += bestMatch.Romanization.Length;
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
