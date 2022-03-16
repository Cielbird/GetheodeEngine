using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    /// <summary>
    /// Based on the (very genius) word generation
    /// javascript tool by chrisdd.
    /// https://chridd.nfshost.com/makewords/
    /// </summary>
    public class PhonotacticGraph
    {
        class Node
        {
            public string name;
            public Phoneme phoneme;
            // a list of options, each option is a list of consecutive nodes
            public List<List<Node>> options;
            public List<int> optionWeights;

            public Node(string n)
            {
                name = n;
                phoneme = null;
                options = new List<List<Node>>();
                optionWeights = new List<int>();
            }

            public Node(Phoneme p)
            {
                name = p.Romanization;
                phoneme = p;
                options = null;
                optionWeights = null;
            }
        }

        Phonology sourcePhonology;
        // the node to start on when building a morpheme
        Node morphemeRoot;
        // the node to start on when building a sylable
        Node sylableRoot;

        /// <summary>
        /// constructs a phonotactic tree based on the
        /// rules, following the structure explained here:
        /// https://chridd.nfshost.com/makewords/
        /// </summary>
        public PhonotacticGraph(string text, Phonology phonology)
        {
            sourcePhonology = phonology;

            Dictionary<string, string> entries = new();
            Dictionary<string, Node> nodes = new();
            List<string> bans = new();
            foreach (string line in text.GetParsedLines())
            {
                Match definitionMatch = Regex.Match(line, @"^([a-zA-Z]+)=([a-zA-Z|]*)$");
                Match banMatch = Regex.Match(line, @"!=([a-zA-Z|]+)$");

                if (definitionMatch.Success)
                {
                    entries.Add(definitionMatch.Groups[1].Value,
                                definitionMatch.Groups[2].Value);
                }
                else if (banMatch.Success)
                    bans.Add(banMatch.Groups[1].Value);
                else
                    throw new ArgumentException($"Couldn't parse the line {line}");
            }

            morphemeRoot = BuildGraph("MORPH", entries, nodes);
        }


        /// <summary>
        /// Recursively builds a graph using a dictionary of entries and a
        /// list of existing nodes and returns the node with a given label.
        /// </summary>
        /// <param name="label">the label of the root node to return</param>
        /// <param name="remainingEntries">source labels and their definitions</param>
        /// <param name="existingNodes">nodes already added to the graph</param>
        /// <returns>the node of the graph with label `label`</returns>z
        Node BuildGraph(string label, Dictionary<string, string> remainingEntries,
                              Dictionary<string, Node> existingNodes)
        {
            string value;
            remainingEntries.TryGetValue(label, out value);
            if (value == null)
                throw new Exception($"Label {label} wasn't found in entries!");

            // construct keyword list used to parse the text.
            List<string> keywords = new List<string>();
            foreach (var entry in remainingEntries)
            {
                keywords.Add(entry.Key);
            }
            foreach (var existingNode in existingNodes)
            {
                keywords.Add(existingNode.Key);
            }
            foreach (Phoneme phoneme in sourcePhonology.Phonemes)
            {
                keywords.Add(phoneme.Romanization);
            }

            //make the node
            Node node = new Node(label);
            remainingEntries.Remove(label);
            existingNodes.Add(label, node);

            // strip whitespace
            value = Regex.Replace(value, @"\s+", "");
            // seperate and parse children
            string[] children = value.Split("|");
            foreach (string child in children)
            {
                // each option is an "or"
                List<Node> option = new List<Node>();

                string[] splitChild = child.SplitUsingKeywords(keywords);
                foreach (string keyword in splitChild)
                {
                    if (existingNodes.ContainsKey(keyword))
                    {
                        // the keyword is an existing node
                        option.Add(existingNodes[keyword]);
                    }
                    else if (remainingEntries.ContainsKey(keyword))
                    {
                        // the keyword is an unparsed entry
                        option.Add(BuildGraph(keyword, remainingEntries, existingNodes));
                    }
                    else
                    {
                        // the keyword is a phoneme
                        Phoneme p = sourcePhonology.Phonemes.Find(
                                                e => e.Romanization == keyword);
                        option.Add(new Node(p));
                    }
                }

                node.options.Add(option);
                node.optionWeights.Add(1);
            }
            return node;
        }

        public Morpheme GenerateMorpheme()
        {
            return new Morpheme(GetPhonemesFromNode(morphemeRoot));
        }

        List<Phoneme> GetPhonemesFromNode(Node node)
        {
            List<Phoneme> phonemes = new List<Phoneme>();
            List<Node> option = null;
            int sum = 0;
            foreach (int weight in node.optionWeights)
            {
                sum += weight;
            }
            int rand = new Random().Next(sum);
            sum = 0;
            for (int i = 0; i < node.options.Count; i++)
            {
                sum += node.optionWeights[i];
                if (rand < sum)
                {
                    option = node.options[i];
                    break;
                }
            }

            foreach (Node child in option)
            {
                if (child == null)
                    continue;
                if (child.options == null)
                    phonemes.Add(child.phoneme);
                else
                    phonemes.AddRange(GetPhonemesFromNode(child));
            }
            return phonemes;
        }
    }
}