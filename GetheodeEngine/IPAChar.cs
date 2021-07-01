using System;
using System.Collections.Generic;

namespace GetheodeEngine
{
    /// <summary>
    /// This stores all ipa characters as their distinctive features.
    /// See http://www.artoflanguageinvention.com/papers/features.pdf.
    /// </summary>
    public class IPAChar
    {
        public enum FeatureState
        {
            Positive, //+
            Negative, //-
            Zero,     //0
        }

        public Dictionary<string, FeatureState> features;

        public FeatureState GetFeature(string tag)
        {
            return features[tag];
        }
    }
}
