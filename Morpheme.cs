using System;
using System.Collections;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Morpheme : IEnumerable
    {
        public class PhonemeSylable
        {
            public List<Phoneme> phonemes = new List<Phoneme>();
        }

        /// <summary>
        /// The underlying representation of the morpheme:
        /// The phonemes that make up the morpheme grouped in sylables.
        /// </summary>
        public List<PhonemeSylable> Sylables { get; }

        // easy indexing
        public Phoneme this[int key]
        {
            get
            {
                int syl = 0;
                int seg = 0;
                int count = 0;
                while (syl < Sylables.Count && seg < Sylables[syl].phonemes.Count)
                {
                    if (count == key)
                    {
                        return Sylables[syl].phonemes[seg];
                    }

                    // increment indices
                    if (seg == Sylables[syl].phonemes.Count - 1)
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
                while (syl < Sylables.Count && seg < Sylables[syl].phonemes.Count)
                {
                    if (count == key)
                    {
                        Sylables[syl].phonemes[seg] = value;
                    }

                    // increment indices
                    if (seg == Sylables[syl].phonemes.Count - 1)
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
                while (syl < Sylables.Count && seg < Sylables[syl].phonemes.Count)
                {
                    // increment indices
                    if (seg == Sylables[syl].phonemes.Count - 1)
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
            Sylables = new List<PhonemeSylable>();
            Sylables.Add(new PhonemeSylable() { phonemes = phonemes });
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
            while (syl < Sylables.Count && phon < Sylables[syl].phonemes.Count)
            {
                yield return Sylables[syl].phonemes[phon];
                // increment indices
                if (phon == Sylables[syl].phonemes.Count - 1)
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