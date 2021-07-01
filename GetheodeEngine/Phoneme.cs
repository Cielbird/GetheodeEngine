using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Phoneme
    {
        class PhonemeRealization
        {
            readonly string context;
            readonly IPAChar realization;

            public PhonemeRealization(string context, IPAChar realization)
            {
                this.context = context;
                this.realization = realization;
            }

            public override string ToString()
            {
                return "<" + context + "," + realization + ">";
            }
        }

        readonly char label;
        readonly List<PhonemeRealization> realizations;

        public Phoneme(char label)
        {
            this.label = label;
            realizations = new List<PhonemeRealization>();
        }

        /// <summary>Adds a realization with context and ipachar</summary>
        public void AddRealization(string context, IPAChar realization)
        {
            realizations.Add(new PhonemeRealization(context, realization));
        }

        /// <summary>
        /// Evaluates whether this phoneme can follow after the `other`
        /// phoneme based on their PhonemeContextTrees. 
        /// </summary>
        /// <param name="other">The other phoneme that would precede this
        /// phoneme</param>
        /// <returns>True if this phoneme is allowed to go after the
        /// `other` phoneme, false otherwise.</returns>
        public bool CanFollow(Phoneme other)
        {
            //TODO plan this to figure out how PhonemeContextTree will work
            return true;
        }

        public override string ToString()
        {
            string realizationsStr = "";
            foreach (PhonemeRealization r in realizations)
                realizationsStr += r.ToString()+", ";
            return label + ": " + realizationsStr;
        }
    }
}
