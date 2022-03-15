using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class Lect
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Phonology Phonology { get; set; }
        public Lexicon Lexicon { get; set; }

        public Lect(string name)
        {
            Name = name;
            Phonology = new Phonology();
            Lexicon = new Lexicon();
        }

        public override string ToString()
        {
            string tostring = Name + " :" + GetHashCode() + "\n";
            tostring += Phonology.ToString();
            return tostring;
        }

        public SegmentSequence GetSurfaceRepresentation(Morpheme morpheme)
        {
            SegmentSequence UR = new SegmentSequence(morpheme);

            foreach(PhonologicalRule rule in Phonology.Rules)
            {
                
                UR.ApplyPhonologicalRule(rule);
            }
            return UR;
        }

        public void AddMorpheme(string romanization)
        {
            Lexicon.AddMorpheme(Phonology.GetPhonemesFromRoman(romanization));
        }

        public Morpheme GenerateMorpheme()
        {
            return Phonology.Phonotactics.GenerateMorpheme();
        }

        /// <summary>
        /// Clears the phonemes and adds the phonemes in the provided file.
        /// </summary>
        /// <param name="path">The path of the phoneme file</param>
        public void ImportPhonemes(string path)
        {
            
            string[] phonemeLines = File.ReadAllLines(path);
            foreach (string l in phonemeLines)
            {
                string line = Regex.Replace(l, @"\s+", "");
                if (line == "")
                    continue;

                string[] values = line.Split(',');


                // add a new phoneme
                if (values.Length > 1)
                    Phonology.AddPhoneme((IPAChar)values[0], values[1]);
                else
                    Phonology.AddPhoneme((IPAChar)values[0]);
            }
        }

        /// <summary>
        /// Imports phonotactic data from a file
        /// </summary>
        /// <param name="path">The file with phonotatic data</param>
        public void ImportPhonotactics(string path)
        {
            Phonology.Phonotactics =
                new PhonotacticGraph(File.ReadAllText(path), Phonology);
        }

        /// <summary>
        /// Imports morphemes from a file
        /// </summary>
        /// <param name="path">The file with morpheme data</param>
        public void ImportMorphemes(string path)
        {
            string[] morphemeLines = File.ReadAllLines(path);
            foreach (string l in morphemeLines)
            {
                string line = Regex.Replace(l, @"\s+", "");
                if (line == "")
                    continue;
                AddMorpheme(line);
            }
        }

        /// <summary>
        /// Imports the phonological rules from a file
        /// </summary>
        /// <param name="path">The file to read from</param>
        public void ImportRules(string path)
        {
            string[] ruleLines = File.ReadAllLines(path);
            foreach (string l in ruleLines)
            {
                string line = Regex.Replace(l, @"\s+", "");
                if (line == "")
                    continue;
                Phonology.AddRule(line);
            }
        }

        /// <summary>
        /// Writes the phoneme data to a file
        /// </summary>
        /// <param name="path">The file to write to</param>
        public void ExportPhonemes(string path)
        {
            string phonemesText = "";
            foreach (Phoneme p in Phonology.Phonemes)
            {
                phonemesText += p.BaseRealization.ToString()
                       + ", " + p.Romanization + "\n";
            }
            File.WriteAllText(path, phonemesText);
        }

        /// <summary>
        /// Writes the phonotactic data to a file
        /// </summary>
        /// <param name="path">The file to write to</param>
        public void ExportPhonotactics(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Writes the morpheme data to a file
        /// </summary>
        /// <param name="path">The file to write to</param>
        public void ExportMorphemes(string path)
        {
            string morphemesText = "";
            foreach (Morpheme m in Lexicon.GetMorphemes())
            {
                foreach (Phoneme p in m.Sylables[0].phonemes)
                {
                    morphemesText += p.Romanization;
                }
                morphemesText += "\n";
            }
            File.WriteAllText(path, morphemesText);
        }

        /// <summary>
        /// Writes the phonological rule data to a file
        /// </summary>
        /// <param name="path">The file to write to</param>
        public void ExportRules(string path)
        {
            string ruleText = "";
            foreach (PhonologicalRule r in Phonology.Rules)
            {
                ruleText += r.ToString() + "\n";
            }
            File.WriteAllText(path, ruleText);
        }
    }
}
