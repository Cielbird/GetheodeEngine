using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    /// <summary>
    /// A class that puts `IPAChar`s together to represent the pronounciation of a
    /// word, morpheme, or a sequence of sounds. As opposed to a simple `List<IPAChar>`,
    /// `SegmentSequence` supports sylables.
    /// </summary>
    public class SegmentSequence
    {
        class Sylable
        {
            public List<IPAChar> segments = new List<IPAChar>();
        }

        List<Sylable> sylables;

        // easy indexing
        public IPAChar this[int key]
        {
            get
            {
                int syl = 0;
                int seg = 0;
                int count = 0;
                while(syl < sylables.Count && seg < sylables[syl].segments.Count)
                {
                    if(count == key)
                    {
                        return sylables[syl].segments[seg];
                    }

                    // increment indices
                    if(seg == sylables[syl].segments.Count - 1)
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
                while (syl < sylables.Count && seg < sylables[syl].segments.Count)
                {
                    if (count == key)
                    {
                        sylables[syl].segments[seg] = value;
                        return;
                    }

                    // increment indices
                    if (seg == sylables[syl].segments.Count - 1)
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
                while (syl < sylables.Count && seg < sylables[syl].segments.Count)
                {
                    // increment indices
                    if (seg == sylables[syl].segments.Count - 1)
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
        /// Constructs a segment sequence from a morpheme's underlying representation.
        ///
        /// In a way, this "gets" the underlying representation as a segment sequence.
        /// </summary>
        /// <param name="morpheme"></param>
        public SegmentSequence(Morpheme morpheme)
        {
            sylables = new List<Sylable>();
            foreach(Morpheme.PhonemeSylable phonSyl in morpheme.Sylables)
            {
                Sylable sylable = new Sylable();
                foreach(Phoneme phoneme in phonSyl.phonemes)
                {
                    sylable.segments.Add(phoneme.BaseRealization);
                }
                sylables.Add(sylable);
            }
        }


        /// <summary>Changes the segment sequence according to the rule</summary>
        /// <param name="morpheme">The sequence of segments to modify</param>
        public void ApplyPhonologicalRule(PhonologicalRule rule)
        {
            for(int i=0; i<Count; i++)
            {
                IPAChar cur = this[i];

                // check if matches input
                if (!rule.Input.Includes(cur))
                    continue;
                // if matches pre context
                if (i == 0 || !rule.PreContext.Includes(this[i - 1]))
                    continue;
                // if matches pre context
                if (i == Count - 1 || !rule.PostContext.Includes(this[i + 1]))
                    continue;

                this[i] = cur + rule.Output;
            }
        }

        public override string ToString()
        {
            string output = "[";
            foreach(Sylable sylable in sylables)
            {
                if(sylable != sylables[0])
                {
                    output += ".";
                }
                foreach (IPAChar segment in sylable.segments)
                {
                    output += segment.ToString();
                }
            }
            output += "]";
            return output;
        }
    }
}
