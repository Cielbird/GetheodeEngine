using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class PhonologicalRule
    {
        IPAChar input;
        IPAChar output;
        IPAChar preContext;
        IPAChar postContext;

        string name;

        /// <summary>
        /// A rule that maps one segment or set of segments to another using
        /// contexts.
        /// For the syntax of writing a rule:
        /// https://en.wikipedia.org/wiki/Phonological_rule
        /// </summary>
        /// <param name="ruleString">the string defining the rule,
        /// by the wikipedia syntax</param>
        public PhonologicalRule(string ruleString)
        {
            // remove whitespace
            ruleString = Regex.Replace(ruleString, @"\s+", "");
            string[] s = ruleString.Split('/');

            string[] io = Regex.Split(s[0], "->");
            input = (IPAChar)io[0];
            output = (IPAChar)io[1];

            string[] context = s[1].Split('_');
            preContext = (IPAChar)context[0];
            postContext = (IPAChar)context[1];

            name = ruleString;
        }

        public override bool Equals(object obj)
        {
            return obj is PhonologicalRule rule &&
                   name == rule.name;
        }

        public override string ToString()
        {
            return name;
        }

        /// <summary>Changes the morpheme according to the rule</summary>
        /// <param name="morpheme">The sequence of segments to modify</param>
        public void ApplyToMorpheme(List<IPAChar> morpheme)
        {   
            for(int i=0; i<morpheme.Count; i++)
            {
                IPAChar cur = morpheme[i];
                // check if matches input
                if (!input.Includes(cur))
                    continue;
                // if matches pre context
                if (i == 0 || !preContext.Includes(morpheme[i - 1]))
                    continue;
                // if matches pre context
                if (i == morpheme.Count - 1 || !postContext.Includes(morpheme[i + 1]))
                    continue;

                morpheme[i] = cur + output;
            }
        }
    }
}