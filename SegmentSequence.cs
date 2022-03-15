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

        public class Range
        {
            public int start;
            public int end;

            public Range(int start, int end)
            {
                this.start = start;
                this.end = end;
            }
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
            // uses the `FindPhonRuleNodeMatches` to find a matching
            // input spot and check precontext and postcontext.

            //use `FindFirstPhonRuleMatch` to 

            while (true)
            {
                List<Range> inputMatches = FindPhonRuleNodeMatches(rule.Input);

                List<Range> preContextMatches = null;
                if (rule.PreContext != null)
                    preContextMatches = FindPhonRuleNodeMatches(rule.PreContext);

                List<Range> postContextMatches = null;
                if (rule.PostContext != null)
                    postContextMatches = FindPhonRuleNodeMatches(rule.PostContext);

                Range firstMatch = null;
                for (int i = 0; i < inputMatches.Count; i++)
                {
                    // check precontext
                    bool doesPreContextMatch;
                    if (preContextMatches != null)
                    {
                        doesPreContextMatch = false;
                        foreach (Range preContextMatch in preContextMatches)
                        {
                            if (inputMatches[i].start == preContextMatch.end
                                && (preContextMatch.start == 0 || !rule.WordInitial))
                            {
                                doesPreContextMatch = true;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        doesPreContextMatch = !(rule.WordInitial && inputMatches[i].start != 0);
                    }
                    if (!doesPreContextMatch)
                    {
                        inputMatches.RemoveAt(i);
                        i--;
                        continue;
                    }

                    //check postcontext
                    bool doesPostContextMatch;
                    if (postContextMatches != null)
                    {
                        doesPostContextMatch = false;
                        foreach (Range postContextMatch in postContextMatches)
                        {
                            if (inputMatches[i].end == postContextMatch.start
                                && (postContextMatch.end == Count || !rule.WordFinal))
                            {
                                doesPostContextMatch = true;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        doesPostContextMatch = !(rule.WordFinal && inputMatches[i].end != Count);
                    }
                    if (!doesPostContextMatch)
                    {
                        inputMatches.RemoveAt(i);
                        i--;
                        continue;
                    }

                    //we have an input match that also matches contexts.
                    firstMatch = inputMatches[i];
                    break;
                }

                // only when we can't find any more matches
                // can we break out of this loop
                if (firstMatch == null)
                    break;

                //replace input with output
                int size = firstMatch.end - firstMatch.start;
                List<IPAChar> original = sylables[0].segments.GetRange(firstMatch.start, size);
                sylables[0].segments.RemoveRange(firstMatch.start, size);
                for (int i=0; i<rule.Output.Children.Count; i++)
                {
                    IPAChar seg = rule.Output.Children[i].Segment;
                    if (i < original.Count)
                        seg = original[i] + seg;

                    sylables[0].segments.Insert(firstMatch.start, seg);
                    firstMatch.start++;
                }
            }
        }

        /// <summary>
        /// Finds the index and size of sub-sequences in this
        /// sequence that match the given phonological rule node pattern
        /// </summary>
        /// <returns>
        /// an array of match locations.
        ///
        /// []
        /// a{}c
        ///  b,ds
        /// 
        /// abcadcacd
        /// </returns>
        public List<Range> FindPhonRuleNodeMatches(PhonologicalRule.Node pattern)
        {
            List<Range> matches = new List<Range>();

            if(pattern.Children == null || pattern.Children.Count == 0)
            {
                for(int i = 0; i<this.Count; i++)
                {
                    if(pattern.Segment.Includes(this[i]))
                    {
                        matches.Add(new Range(i, i + 1));
                    }
                }
            }
            else
            {
                // get a list of all the matches for every child
                List<List<Range>> matchSets = new List<List<Range>>();
                foreach (var node in pattern.Children)
                {
                    matchSets.Add(FindPhonRuleNodeMatches(node));
                }

                // concatinate the matches that are consecutive for a final list
                if(pattern.Type == PhonologicalRule.Node.ChildrenType.And)
                {
                    while(matchSets.Count > 1)
                    {
                        List<Range> firstMatches = matchSets[0];
                        List<Range> nextMatches = matchSets[1];
                        List<Range> consolidatedMatches = new List<Range>();
                        for (int first = 0; first < firstMatches.Count; first++)
                        {
                            for (int next = 0; next < nextMatches.Count; next++)
                            {
                                if(firstMatches[first].end == nextMatches[next].start)
                                {
                                    consolidatedMatches.Add(new Range(firstMatches[first].start, nextMatches[next].end));
                                }
                            }
                        }
                        matchSets.RemoveAt(1);
                        matchSets[0] = consolidatedMatches;
                    }
                    matches = matchSets[0];
                }
                else
                {
                    foreach(var matchSet in matchSets)
                    {
                        // TODO consolidate repeats
                        matches.AddRange(matchSet);
                    }
                }
            }
            return matches;
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
