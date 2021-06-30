using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public readonly struct IPACons : IPAChar, IEquatable<IPACons>
    {
        /// <summary>Place of articulation</summary>
        public enum PlcOfArt
        {
            Bilab,
            Labdent,
            Dental,
            Alveolar,
            Postalv,
            Palatal,
            Velar,
            Uvular,
            Pharyn,
            Glottal
        }
        /// <summary>Manner of articulation</summary>
        public enum ManOfArt
        {
            Fric,
            Plos,
            Nasal,
            Approx,
            LatApprox,
            Trill
        }

        /// <summary>Library of all the used IPA consonants</summary>
        public static Dictionary<char, IPACons> vowels = new Dictionary<char, IPACons>
        {
            {'p', new IPACons(PlcOfArt.Bilab, ManOfArt.Plos, false) },
            {'t', new IPACons(PlcOfArt.Alveolar, ManOfArt.Plos, false) },
            {'k', new IPACons(PlcOfArt.Velar, ManOfArt.Plos, false) },
            {'n', new IPACons(PlcOfArt.Alveolar, ManOfArt.Nasal, true) },
            {'m', new IPACons(PlcOfArt.Bilab, ManOfArt.Nasal, true) }
        };

        /// <summary>Place of articulation</summary>
        public PlcOfArt PlaceOfArt { get; }
        /// <summary>Manner of articulation</summary>
        public ManOfArt MannerOfArt { get; }
        /// <summary>True: voiced. False: unvoiced</summary>
        public bool Voiced { get; }

        /// <summary>
        /// Constructs a new ipa consonant. See ipachart.com for details.
        /// </summary>
        /// <param name="place">Place of articulation</param>
        /// <param name="manner">Manner of articulation</param>
        /// <param name="voiced">If the consonant is voiced or not.</param>
        /// <exception cref="ArgumentException">Exception is thrown if the consonant does not exist in the library</exception>
        public IPACons(PlcOfArt place, ManOfArt manner, bool voiced)
        {
            PlaceOfArt = place;
            MannerOfArt = manner;
            Voiced = voiced;
            //Throw an exception if the consonant doesn't exist
            EvalToChar();
        }

        /// <summary>Gets the coresponding character</summary>
        public char EvalToChar()
        {
            foreach (KeyValuePair<char, IPACons> vowel in vowels)
            {
                if (vowel.Value == this)
                    return vowel.Key;
            }
            throw new ArgumentException("Consonant could not be evaluated to a character");
        }

        /// <summary>Returns the IPACons that coresponds to the char</summary>
        /// <param name="c">The char to use</param>
        public static explicit operator IPACons(char c)
        {
            if (!vowels.ContainsKey(c))
                throw new ArgumentException("char is not part of the IPA consonant library.");
            return vowels[c];
        }

        public static bool operator ==(IPACons left, IPACons right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IPACons left, IPACons right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is IPACons cons && Equals(cons);
        }

        public bool Equals(IPACons other)
        {
            return PlaceOfArt == other.PlaceOfArt &&
                   MannerOfArt == other.MannerOfArt &&
                   Voiced == other.Voiced;
        }

        /// <summary>Returns the coresponding ipa character</summary>
        /// <returns>The coresponding ipa character as a string</returns>
        public override string ToString()
        {
            return EvalToChar().ToString();
        }
    }
}
