using System;
using System.Collections;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Morpheme : IEnumerable
    {
        class PhonemeSylable
        {
            public List<Phoneme> phonemes = new List<Phoneme>();
        }

        /// <summary>
        /// The underlying representation of the morpheme:
        /// The phonemes that make up the morpheme grouped in sylables.
        /// </summary>
        List<PhonemeSylable> sylables = new List<PhonemeSylable>();

        // easy indexing
        public Phoneme this[int key]
        {
            get
            {
                int syl = 0;
                int seg = 0;
                int count = 0;
                while (syl < sylables.Count && seg < sylables[syl].phonemes.Count)
                {
                    if (count == key)
                    {
                        return sylables[syl].phonemes[seg];
                    }

                    // increment indices
                    if (seg == sylables[syl].phonemes.Count - 1)
                    {
                        seg = 0;
                        syl++;
                    }
                    else
                    {
                        seg++;
                    }
                    count++;
                }
                throw new IndexOutOfRangeException("Index was " + key + ", but " +
                    "SegmentSequence only has " + Count + " segments.");
            }
            set
            {
                int syl = 0;
                int seg = 0;
                int count = 0;
                while (syl < sylables.Count && seg < sylables[syl].phonemes.Count)
                {
                    if (count == key)
                    {
                        sylables[syl].phonemes[seg] = value;
                    }

                    // increment indices
                    if (seg == sylables[syl].phonemes.Count - 1)
                    {
                        seg = 0;
                        syl++;
                    }
                    else
                    {
                        seg++;
                    }
                    count++;
                }
                throw new IndexOutOfRangeException("Index was " + key + ", but " +
                    "SegmentSequence only has " + Count + " segments.");
            }
        }

        public int Count
        {
            get
            {
                int syl = 0;
                int seg = 0;
                int count = 0;
                while (syl < sylables.Count && seg < sylables[syl].phonemes.Count)
                {
                    // increment indices
                    if (seg == sylables[syl].phonemes.Count - 1)
                    {
                        seg = 0;
                        syl++;
                    }
                    else
                    {
                        seg++;
                    }
                    count++;
                }
                return count;
            }
        }


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
            sylables = new List<PhonemeSylable>();
            sylables.Add(new PhonemeSylable() { phonemes = phonemes });
        }

        public override string ToString()
        {
            string roman = "";
            foreach (Phoneme p in this)
            {
                roman += p.Romanization;
            }
            return roman;
        }

        public IEnumerator GetEnumerator()
        {
            int syl = 0;
            int phon = 0;
            int count = 0;
            while (syl < sylables.Count && phon < sylables[syl].phonemes.Count)
            {
                yield return sylables[syl].phonemes[phon];
                // increment indices
                if (phon == sylables[syl].phonemes.Count - 1)
                {
                    phon = 0;
                    syl++;
                }
                else
                {
                    phon++;
                }
                count++;
            }
        }
    }
}