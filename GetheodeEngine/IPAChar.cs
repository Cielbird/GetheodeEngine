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
        /// All the features in this character
        /// </summary>
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

        public override string ToString()
        {
            //
            foreach(KeyValuePair<string, IPAChar> ipachar in IpaCharLib)
            {
                if (ipachar.Value == this)
                    return ipachar.Key;
            }
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
            try
            {
                return IpaCharLib[c];
            }
            catch
            {
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
            FeatureState[] combinedFeatures = left.features;
            for(int i=0; i<left.features.Length; i++)
            {
                FeatureState f = right.features[i];
                if (f != FeatureState.Zero)
                    left.features[i] = right.features[i];
            }
            return new IPAChar(combinedFeatures);
        }

        public static bool operator ==(IPAChar left, IPAChar right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IPAChar left, IPAChar right)
        {
            return !(left == right);
        }
    }
}
