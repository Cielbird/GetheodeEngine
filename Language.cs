using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class Language
    {
        public static Language LoadFromFile(string path)
        {
            Language language = new Language("");
            using (StreamReader langReader = new StreamReader(path))
            {
                string langJson = langReader.ReadToEnd();
                LanguageData langData = JsonSerializer.Deserialize<LanguageData>(langJson);
                language.Name = langData.Name;
                language.Description = langData.Description;


                foreach (string lectPath in langData.LectLocations.Values)
                {
                    // read the lect
                    Lect lect = new Lect("");
                    using (StreamReader lectReader = new StreamReader(path))
                    {
                        string lectJson = lectReader.ReadToEnd();
                        LectData lectData = JsonSerializer.Deserialize<LectData>(lectJson);
                        lect.Name = lectData.Name;
                        lect.Description = lectData.Description;

                        using (StreamReader phonReader = new StreamReader(lectData.PhonemesFilePath))
                        {
                            while (!phonReader.EndOfStream)
                            {
                                string line = Regex.Replace(phonReader.ReadLine(), @"\s+", "");
                                if (line == "")
                                    continue;

                                string[] values = line.Split(',');


                                // add a new phoneme
                                if (values.Length > 1)
                                {
                                    lect.Phonology.AddPhoneme((IPAChar)values[0], values[1]);
                                }
                                else
                                {
                                    lect.Phonology.AddPhoneme((IPAChar)values[0]);
                                }
                            }
                        }

                        using (StreamReader morphReader = new StreamReader($"{path}/morphemes.txt"))
                        {
                            while (!morphReader.EndOfStream)
                            {
                                string line = Regex.Replace(morphReader.ReadLine(), @"\s+", "");
                                if (line == "")
                                    continue;

                                lect.AddMorpheme(line);
                            }
                        }

                        using (StreamReader rulesReader = new StreamReader($"{path}/rules.txt"))
                        {
                            while (!rulesReader.EndOfStream)
                            {
                                string line = Regex.Replace(rulesReader.ReadLine(), @"\s+", "");
                                if (line == "")
                                    continue;

                                lect.Phonology.AddRule(line);
                            }
                        }
                    }
                    language.Lects.Add(lect);
                }
                return language;
            }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string SavePath { get; set; }
        public List<Lect> Lects { get; set; }

        public Language(string name)
        {
            Name = name;
            Lects = new List<Lect>();
            SavePath = $"/Users/guilhemane/Documents/" +
                $"GetheodeEngine/English/lang.json";
        }

        public override string ToString()
        {
            string tostring = Name + " :"+ GetHashCode() + "\n";
            foreach(Lect lect in Lects)
            {
                tostring += "-"+lect.ToString()+"\n";
            }
            return tostring;
        }

        public void Save()
        {
            LanguageData langData;
            if (File.Exists(SavePath))
            {
                langData = JsonSerializer.Deserialize<LanguageData>(File.ReadAllText(SavePath));
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));
                langData = new LanguageData()
                {
                    Name = Name,
                    Description = Description,
                    LectLocations = new Dictionary<string, string>()
                };
            }
            File.WriteAllText(SavePath, JsonSerializer.Serialize(langData,
                        new JsonSerializerOptions() { WriteIndented = true }));

            foreach (Lect lect in Lects)
            {
                LectData lectData;

                string lectPath;
                if (langData.LectLocations.ContainsKey(lect.Name))
                    lectPath = langData.LectLocations[lect.Name];
                else
                    lectPath = $"{SavePath}/{lect.Name}/lect.json";

                if (File.Exists(lectPath))
                {
                    lectData = JsonSerializer.Deserialize<LectData>(File.ReadAllText(SavePath));
                }
                else
                {
                    Directory.CreateDirectory(lectPath);
                    lectData = new LectData()
                    {
                        Name = Name,
                        Description = Description,
                        PhonemesFilePath = $"{SavePath}/{lect.Name}/phonemes.csv",
                        MorphemesFilePath = $"{SavePath}/{lect.Name}/morphemes.csv",
                        RulesFilePath = $"{SavePath}/{lect.Name}/rules.csv"
                    };
                }
                File.WriteAllText(lectPath, JsonSerializer.Serialize(lectData,
                            new JsonSerializerOptions() { WriteIndented = true }));

                // write phonemes to file
                string phonemesText = "";
                foreach(Phoneme p in lect.Phonology.Phonemes)
                {
                    phonemesText += p.BaseRealization.ToString()
                           + ", " + p.Romanization + "\n";
                }
                File.WriteAllText(lectData.PhonemesFilePath, phonemesText);

                // write morphemes to file
                string morphemesText = "";
                foreach (Morpheme m in lect.Lexicon.GetMorphemes())
                {
                    foreach(Phoneme p in m.Sylables[0].phonemes)
                    {
                        morphemesText += p.Romanization;
                    }
                    morphemesText += "\n";
                }
                File.WriteAllText(lectData.MorphemesFilePath, morphemesText);

                // write morphemes to file
                string ruleText = "";
                foreach (PhonologicalRule r in lect.Phonology.Rules)
                {
                    ruleText += r.ToString() + "\n";
                }
                File.WriteAllText(lectData.RulesFilePath, ruleText);
            }
        }
    }
}
