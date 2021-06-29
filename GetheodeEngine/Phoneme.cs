using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    public class Phoneme
    {
        class PhonemeRealization
        {
            readonly string context;
            readonly char realization;

            public PhonemeRealization(string context, char realization)
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

        public void AddRealization(string context, char realization)
        {
            realizations.Add(new PhonemeRealization(context, realization));
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
