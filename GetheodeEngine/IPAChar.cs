using System;
using System.Collections.Generic;
using System.IO;

namespace GetheodeEngine
{
    /// <summary>
    /// This stores all ipa characters as their distinctive features.
    /// See http://www.artoflanguageinvention.com/papers/features.pdf.
    /// </summary>
    public readonly struct IPAChar
    {
        /// It can't be a char, since affricates are 3 unicode
        /// characters
        static Dictionary<string, IPAChar> ipaCharLib;

        public enum FeatureState
        {
            Positive,
            Negative,
            Zero
        }

        /// <summary>
        /// All the features and their states in this character
        /// </summary>
        public readonly Dictionary<string, FeatureState> features;

        public FeatureState GetFeature(string tag)
        {
            return features[tag];
        }

        public static explicit operator IPAChar(string c)
        {
            if (ipaCharLib == null)
                LoadAllIPAChars();

            return ipaCharLib[c];
        }

        public IPAChar(Dictionary<string, FeatureState> features)
        {
            this.features = features;
        }

        /// <summary>
        /// initializes the ipaCharLib dictionary with all the base ipa
        /// characters
        /// </summary>
        private static void LoadAllIPAChars()
        {
            ipaCharLib = new Dictionary<string, IPAChar>();
            using(StreamReader reader = new StreamReader("data/ipa_bases.csv"))
            {
                List<string> featureNames = new List<string>();
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');
                    if (values[0] == "ipa")
                    {
                        // define the feature name list
                        for (int i = 1; i < values.Length; i++)
                        {
                            featureNames.Add(values[i]);
                        }
                    }
                    else
                    {
                        // add a new ipa character to the library
                        string charName = values[0];
                        IPAChar ipaChar = new IPAChar(new Dictionary<string, FeatureState>());
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
                            ipaChar.features.Add(featureNames[i - 1], state);
                        }
                        ipaCharLib.Add(charName, ipaChar);
                    }
                }
            }
        }
    }
}
