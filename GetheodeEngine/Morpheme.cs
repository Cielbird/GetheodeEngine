using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Morpheme
    {
        /// <summary>
        /// The phonemes that make up the morpheme
        /// </summary>
        public List<Phoneme> UnderlyingRepresentation { get; }


        /// <summary>
        /// Constructs a new morpheme with the given phonemes
        /// </summary>
        /// <param name="romanPhonemes">a sequence of phonemes given in
        /// their romanized form, based off the phonemes in sourcePhonology
        /// </param>
        /// <param name="sourcePhonology">The phonology to use when deciphering
        /// the roman character phonemes.
        /// </param>
        public Morpheme(List<Phoneme> phonemes)
        {
            UnderlyingRepresentation = phonemes;
        }

        public override string ToString()
        {
            string roman = "";
            foreach (Phoneme p in UnderlyingRepresentation)
            {
                roman += p.Romanization;
            }
            return roman;
        }
    }
}