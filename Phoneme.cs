using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    // To be honest, I don't if I will keep the phoneme class, since
    // it is useless for now.
    public class Phoneme
    {
        public string Romanization { get; }
        public IPAChar BaseRealization { get; }

        public Phoneme(string roman, IPAChar baseRealization)
        {
            BaseRealization = baseRealization;
            Romanization = roman;
        }

        public override string ToString()
        {
            if (Romanization == BaseRealization.ToString())
                return Romanization;
            return Romanization + "[" + BaseRealization.ToString() + "]";
        }
    }
}
