using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    // To be honest, I don't if I will keep the phoneme class, since
    // it is useless for now.
    public class Phoneme
    {
        readonly IPAChar baseRealization;

        public Phoneme(IPAChar baseRealization)
        {
            this.baseRealization = baseRealization;
        }

        public override string ToString()
        {
            return baseRealization.ToString();
        }
    }
}
