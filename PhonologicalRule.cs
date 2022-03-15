using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    /// <summary>
    /// Describes a rule (e>a/t_d for example) with node trees
    ///
    /// Trees are made up of "and" and "or" nodes.
    /// AND nodes indicate a series of segments: (a then b then c)
    /// OR  nodes indicate a series of possible segments  (a or b or c).
    /// </summary>
    public class PhonologicalRule
    {
        public class Node
        {
            public enum ChildrenType
            {
                And,
                Or
            }

            public IPAChar Segment { get; }
            public Node Parent { get; set; }
            public List<Node> Children { get; }
            public ChildrenType Type { get; }

            public Node(string sequenceString)
            {
                Children = new List<Node>();
                sequenceString = Regex.Replace(sequenceString, @"\s+", "");

                Node cur = this;
                string currentReading = "";
                bool canAddChar = false;
                bool readingAndInOr = false;
                for (int i = 0; i < sequenceString.Length; i++)
                {
                    char c = sequenceString[i];
                    if (c == '{')
                    {
                        Node n = new Node(ChildrenType.Or, cur);
                        cur.Children.Add(n);
                        cur = n;
                    }
                    else if (c == '}')
                    {
                        cur = cur.Parent.Parent;
                    }
                    else if (c == ',')
                    {
                        cur = cur.Parent;
                        continue;
                    }
                    else
                    {
                        // read the next char
                        string bestChar = "";
                        string readingChar = "";
                        int bestCharLength = 0;
                        for (int j = i; j < sequenceString.Length; j++)
                        {
                            readingChar += sequenceString[j];
                            try
                            {
                                // if it throws no exxeptions, set it
                                // to bestChar.
                                IPAChar n = (IPAChar)readingChar;
                                bestChar = readingChar;
                                bestCharLength = j - i + 1;
                            }
                            catch { }
                        }
                        if (bestCharLength == 0)
                            throw new ArgumentException(
                                "Could not read an ipachar at " + i + "!");
                        else
                            i += bestCharLength - 1;

                        if (cur.Type == ChildrenType.Or)
                        {
                            Node n = new Node(ChildrenType.And, cur);
                            cur.Children.Add(n);
                            cur = n;
                        }
                        cur.Children.Add(new Node((IPAChar)bestChar, cur));
                    }
                }
            }

            public Node(ChildrenType t, Node parent)
            {
                Type = t;
                Children = new List<Node>();
                Parent = parent;
            }

            public Node(IPAChar c, Node parent)
            {
                Type = ChildrenType.And;
                Children = null;
                Segment = c;
                Parent = parent;
            }

            public string ToString()
            {
                return null;
            }
        }

        public Node Input { get; }
        public Node Output { get; }
        public Node PreContext { get; }
        public Node PostContext { get; }

        public bool WordFinal { get; }
        public bool WordInitial { get; } 

        string name;

        /// <summary>
        /// A rule that maps one segment or set of segments to another using
        /// contexts.
        /// For the syntax of writing a rule:
        /// https://en.wikipedia.org/wiki/Phonological_rule
        /// </summary>
        /// <param name="ruleString">the string defining the rule,
        /// by the wikipedia syntax</param>
        public PhonologicalRule(string ruleString)
        {
            // remove whitespace
            ruleString = Regex.Replace(ruleString, @"\s+", "");
            if (!ruleString.Contains('/'))
                ruleString += "/_";

            string[] s = ruleString.Split('/');

            string[] io = Regex.Split(s[0], "->");
            Input = new Node(io[0]);
            Output = new Node(io[1]);

            string[] context = s[1].Split('_');

            // if there was a # at the start, word initial.
            string trimmedPre = context[0].TrimStart('#');
            WordInitial = trimmedPre != context[0];
            if(trimmedPre != "")
                PreContext = new Node(trimmedPre);

            // if there was a # at the start, word final.
            string trimmedPost = context[1].TrimEnd('#');
            WordFinal = trimmedPost != context[1];
            if(trimmedPost != "")
                PostContext = new Node(trimmedPost);

            name = ruleString;
        }

        public override bool Equals(object obj)
        {
            return obj is PhonologicalRule rule &&
                   name == rule.name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}