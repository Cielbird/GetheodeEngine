using System;
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

        /// <summary>Changes the list of ipachars according to the rule</summary>
        /// <param name="morpheme">The sequence of segments to modify</param>
        public void ApplyToSurfaceRep(List<IPAChar> surfaceRep)
        {   
            for(int i=0; i< surfaceRep.Count; i++)
            {
                IPAChar cur = surfaceRep[i];
                // check if matches input
                if (!input.Includes(cur))
                    continue;
                // if matches pre context
                if (i == 0 || !preContext.Includes(surfaceRep[i - 1]))
                    continue;
                // if matches pre context
                if (i == surfaceRep.Count - 1 || !postContext.Includes(surfaceRep[i + 1]))
                    continue;

                surfaceRep[i] = cur + output;
            }
        }
    }
}