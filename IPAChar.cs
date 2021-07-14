using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    /// <summary>
    /// A set of phonological features that may represent an single
    /// segment (an IPA character), a set of segments, or a
    /// modification to any other segment(like ipa diacritics).
    /// </summary>
    public readonly struct IPAChar : IEquatable<IPAChar>
    {
        /// It can't be a char, since affricates are 3 unicode
        /// characters
        static Dictionary<string, IPAChar> _ipaCharLib;
        static Dictionary<string, IPAChar> IpaCharLib
        {
            get
            {
                if (_ipaCharLib == null)
                {
                    _ipaCharLib = new Dictionary<string, IPAChar>();
                    LoadAllIPAChars();
                }
                return _ipaCharLib;
            }
        }
        static Dictionary<char, IPAChar> _diacriticLib;
        public static Dictionary<char, IPAChar> DiacriticLib
        {
            get
            {
                if (_diacriticLib == null)
                {
                    _diacriticLib = new Dictionary<char, IPAChar>();
                    LoadAllDiacritics();
                }
                return _diacriticLib;
            }
        }

        public enum FeatureState
        {
            Zero,
            Positive,
            Negative
        }

        /// <summary>All the feature labels in order</summary>
        static string[] featureNames =
        {
            //Major class
            "syl","stress","long","cons","son",
            //Manner
            "cont","delrel","approx",
            "tap","trill","nasal",
            //Laryngeal
            "voi","spgl","congl",
            //Place
            //LABIAL
            "lab","round","labdent",
            //CORONAL
            "cor","ant","dist","strident","lateral",
            //DORSAL
            "dor","high","low","front","back","tense"
        };

        /// <summary>
        /// A char with only `0` features. ie a ipachar
        /// that includes any segment
        /// </summary>
        public static IPAChar AnyChar
        {
            get
            {
                return new IPAChar(new FeatureState[featureNames.Length]);
            }
        }

        /// <summary>
        /// All the features in this character
        /// </summary>s
        public readonly FeatureState[] features;

        public FeatureState GetFeature(string tag)
        {
            for (int i = 0; i < featureNames.Length; i++)
            {
                if (featureNames[i] == tag)
                    return features[i];
            }
            throw new KeyNotFoundException("The feature " + tag + " was not " +
                "found in the list of features.");
        }

        public IPAChar(FeatureState[] features)
        {
            this.features = features;
        }

        public bool Includes(IPAChar other)
        {
            for(int i=0; i<features.Length; i++)
            {
                if (features[i] != FeatureState.Zero
                    && other.features[i] != features[i])
                    return false;
            }
            return true;
        }

        public int DistanceTo(IPAChar other)
        {
            int[] positions = { 0, 1, -1 };
            int dist = 0;
            for (int i = 0; i < features.Length; i++)
            {
                dist += Math.Abs(positions[(int)features[i]]
                    - positions[(int)other.features[i]]);
            }
            return dist;
        }

        public override string ToString()
        {
            // look for any ipachar in the library that includes ours, then
            // see if we can add diacritics to fit our ipachar
            int dist = int.MaxValue;
            // get base char
            Stack<KeyValuePair<string, IPAChar>> charStack = new Stack<KeyValuePair<string, IPAChar>>();

            // since we need a dictionary with string keys (vs char keys),
            // convert the diacritics dicitonary
            Dictionary<string, IPAChar> diacriticsAsStr = new Dictionary<string, IPAChar>();
            foreach (KeyValuePair<char, IPAChar> d in DiacriticLib)
                diacriticsAsStr.Add(d.Key.ToString(), d.Value);

            while(dist != 0)
            {
                KeyValuePair<string, IPAChar> bestChar =
                    new KeyValuePair<string, IPAChar>();

                // add all the ipachars of the stack together
                IPAChar compiledStack = AnyChar;
                foreach (KeyValuePair<string, IPAChar> c in charStack)
                    compiledStack += c.Value;

                foreach (KeyValuePair<string, IPAChar> ipachar in charStack.Count == 0 ? IpaCharLib : diacriticsAsStr)
                {
                    int nextDist = DistanceTo(compiledStack + ipachar.Value);
                    if (nextDist < dist)
                    {
                        dist = nextDist;
                        bestChar = ipachar;
                    }
                }
                if (dist == int.MaxValue)
                {
                    // in this case, no diacritic was found to improve the
                    // distance to `this`, so we can pop the last
                    // diacritic off the stack.
                    charStack.Pop();
                }
                else
                {
                    charStack.Push(bestChar);
                }
            }

            string finalString = "";
            foreach (KeyValuePair<string, IPAChar> c in charStack)
                finalString = c.Key + finalString;
            return finalString;

            // just list all the non-zero features
            string tostring = "[";
            for(int i=0; i<features.Length; i++)
            {
                if (features[i] != FeatureState.Zero)
                {
                    char s = features[i] == FeatureState.Positive ? '+' : '-';
                    tostring += s + featureNames[i];
                }
            }
            tostring += "]";
            return tostring;
        }

        /// <summary>
        /// initializes the ipaCharLib dictionary with all the base ipa
        /// characters
        /// </summary>
        private static void LoadAllIPAChars()
        {
            // the data was from Bruce Hayes and Eric Biggs:
            // https://www.linguistics.ucla.edu/people/hayes/IP/#features
            using (StreamReader reader = new StreamReader("data/ipa_bases.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    if (values[0] == "ipa")
                        continue;

                    // add a new ipa character to the library
                    string charName = values[0];
                    IPAChar ipaChar = new IPAChar(new FeatureState[featureNames.Length]);
                    for (int i = 1; i < values.Length; i++)
                    {
                        // add the feature and it's state to ipaChar
                        char sc = values[i][0];
                        FeatureState state =
                            sc == '+' ?
                            FeatureState.Positive :
                            sc == '-' ?
                            FeatureState.Negative :
                            FeatureState.Zero;
                        ipaChar.features[i-1] = state;
                    }
                    IpaCharLib.Add(charName, ipaChar);
                }
            }
        }

        private static void LoadAllDiacritics()
        {
            // the data was originally from Bruce Hayes and Eric Biggs:
            // https://www.linguistics.ucla.edu/people/hayes/IP/#features
            using (StreamReader reader = new StreamReader("data/diacritics.csv"))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    if (values[0] == "ipa")
                        continue;

                    // add a new ipa character to the library
                    char diacriticChar = values[0][0];
                    IPAChar ipaChar = new IPAChar(new FeatureState[featureNames.Length]);
                    for (int i = 1; i < values.Length; i++)
                    {
                        // add the feature and it's state to ipaChar
                        char sc = values[i][0];
                        FeatureState state =
                            sc == '+' ?
                            FeatureState.Positive :
                            sc == '-' ?
                            FeatureState.Negative :
                            FeatureState.Zero;
                        ipaChar.features[i - 1] = state;
                    }
                    DiacriticLib.Add(diacriticChar, ipaChar);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is IPAChar c && Equals(c);
        }

        public bool Equals(IPAChar other)
        {
            return Enumerable.SequenceEqual(features, other.features);
        }

        public static explicit operator IPAChar(string c)
        {
            c = c.Trim('[', ']');
            // [] includes all characters, ie all `0` features
            if (c == "")
                return AnyChar;
            try
            { // normal char
                //try to extract any diacritics:
                string diacritics = "";
                char cur = c[^1];
                while (DiacriticLib.ContainsKey(cur))
                {
                    diacritics += cur;
                    c = c[0..^1];
                    cur = c[^1];
                }
                IPAChar finalchar = (IPAChar)IpaCharLib[c].MemberwiseClone();
                foreach(char diacritic in diacritics)
                {
                    finalchar += DiacriticLib[diacritic];
                }
                return finalchar;
            }
            catch
            { // bracketed feature list
                c = Regex.Replace(c, @"\s+", "");
                string[] featureStrings = Regex.Split(c, @"(?<!^)(?=[+-])");
                FeatureState[] features = new FeatureState[featureNames.Length];
                for(int i=0; i<featureStrings.Length; i++)
                {
                    if(featureStrings[i] != "")
                    {
                        string featureName = featureStrings[i].Substring(1);
                        char stateStr = featureStrings[i][0];
                        FeatureState state = stateStr == '+' ?
                            FeatureState.Positive : FeatureState.Negative;

                        features[Array.IndexOf(featureNames, featureName)] = state;
                    }
                }
                return new IPAChar(features);
            }
        }

        /// <summary>
        /// Overrides the features of the left IPAChar to match the non-zero
        /// features of the right IPAChar. In a way, it "overlays" the
        /// modifying char onto the original char
        /// </summary>
        /// <param name="left">The original char</param>
        /// <param name="right">The modifying char</param>
        /// <returns></returns>
        public static IPAChar operator +(IPAChar left, IPAChar right)
        {
            FeatureState[] combinedFeatures = (FeatureState[])left.features.Clone();
            for(int i=0; i<left.features.Length; i++)
            {
                FeatureState f = right.features[i];
                if (f != FeatureState.Zero)
                    combinedFeatures[i] = f;
            }
            return new IPAChar(combinedFeatures);
        }

        public static bool operator !=(IPAChar left, IPAChar right)
        {
            return !(left == right);
        }

        public static bool operator ==(IPAChar left, IPAChar right)
        {
            return left.Equals(right);
        }

    }
}
