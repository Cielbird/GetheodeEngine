using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public readonly struct IPAVow : IPAChar, IEquatable<IPAVow>
    {
        public static Dictionary<char,IPAVow> vowels = new Dictionary<char, IPAVow>
        {
            {'i', new IPAVow(3f, 2f, false) },
            {'y', new IPAVow(3f, 2f, true) },
            {'o', new IPAVow(2f, 0f, true) },
            {'u', new IPAVow(3f, 0f, true) },
            {'e', new IPAVow(2f, 2f, false) },
            {'a', new IPAVow(0f, 2f, false) }
        };

        /// <summary>From 0 (open) to 3 (closed). </summary>
        public float Height { get; }
        /// <summary>From 0 (back) to 2 (front). </summary>
        public float Frontedness { get; }
        /// <summary>True: rounded. False: unrounded</summary>
        public bool Rounded { get; }

        public IPAVow(float height, float frontedness, bool rounded)
        {
            Height = height;
            Frontedness = frontedness;
            Rounded = rounded;
        }

        /// <summary>Gets the coresponding character</summary>
        public char EvalToChar()
        {
            float minDist = float.MaxValue;
            char closestVow = ' ';

            foreach (KeyValuePair<char, IPAVow> vowel in vowels)
            {
                float dist = (float)Math.Sqrt(
                    Math.Pow((vowel.Value.Height - Height)/3, 2.0)
                  + Math.Pow((vowel.Value.Frontedness - Frontedness)/2, 2.0));

                if (minDist > dist)
                {
                    minDist = dist;
                    closestVow = vowel.Key;
                }
            }
            return closestVow;

        }

        /// <summary>Returns the IPAVow that coresponds to the char</summary>
        /// <param name="c">The char to use</param>
        public static explicit operator IPAVow(char c)
        {
            if (!vowels.ContainsKey(c))
                throw new ArgumentException("char is not part of the IPA vowel library.");
            return vowels[c];
        }

        public static bool operator ==(IPAVow left, IPAVow right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IPAVow left, IPAVow right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is IPAVow vow && Equals(vow);
        }

        public bool Equals(IPAVow other)
        {
            return Height == other.Height &&
                   Frontedness == other.Frontedness &&
                   Rounded == other.Rounded;
        }

        /// <summary>Returns the closest ipa character</summary>
        /// <returns>The closest ipa character as a string</returns>
        public override string ToString()
        {
            return EvalToChar().ToString();
        }
    }
}
