using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public static class ParsingExtentions
    {
        /// <summary>
        /// Using a list of keywords, this seperates the given string into the given keywords.
        ///
        /// For example, using keywords "ONE", "TWO", "and", and "ONEHUNDRED", and input
        /// ONEONEHUNDREDandTWO, we get [ONE, ONEHUNDRED, and, TWO].
        ///
        /// Whitespace is not ignored
        ///
        /// Priority is given to large keywords.
        /// </summary>
        public static string[] SplitUsingKeywords(this string input,
                                                  IEnumerable<string> keywords)
        {
            List<string> output = new List<string>();
            List<Match> curMatches = new List<Match>();
            List<Match> failedMatches = new List<Match>();
            int parseIndex = 0;
            while (parseIndex < input.Length)
            {
                Match bestMatch = null;
                foreach (string keyword in keywords)
                {
                    if (keyword == null || keyword == "")
                        throw new Exception("Keyword cannot be empty or null!");

                    MatchCollection matches = Regex.Matches(input, keyword);
                    foreach (Match match in matches)
                    {
                        if (match.Success && match.Index == parseIndex)
                        {
                            bool isFailed = false;
                            foreach (Match fail in failedMatches)
                            {
                                if (match.Index == fail.Index && match.Length == fail.Length)
                                    isFailed = true;
                            }
                            if (!isFailed)
                            {
                                bestMatch = match;
                                break;
                            }
                        }
                    }
                }

                // if no match is found, go back, "blacklist" the last match
                // since it doesn't work, and continue 
                if (bestMatch == null)
                {
                    if (curMatches.Count == 0)
                    {
                        throw new ArgumentException(
                            "the string cannot be split with the given " +
                            "keywords, couldn't find any keywords at:" +
                            input[parseIndex..]);
                    }
                    int finalIndex = curMatches.Count - 1;
                    failedMatches.Add(curMatches[finalIndex]);
                    parseIndex -= curMatches[finalIndex].Length;
                    curMatches.RemoveAt(finalIndex);
                    output.RemoveAt(finalIndex);
                    continue;
                }

                curMatches.Add(bestMatch);
                output.Add(bestMatch.Value);
                parseIndex += bestMatch.Length;
            }
            return output.ToArray();
        }

        /// <summary>
        /// Parses the lines and enumerates throught them.
        ///
        /// Removes comments with //, clears whitespace and skips empty lines.
        /// </summary>
        /// <param name="text">The source lines</param>
        /// <returns>An enumerator that returns each parsed line.</returns>
        public static IEnumerable<string> GetParsedLines(this string text)
        {
            string[] lines = text.Split("\n");
            foreach (string l in lines)
            {
                string line = l;
                // remove comments
                line = Regex.Replace(line, @"//.*", "");
                // clear whitespace
                line = Regex.Replace(line, @"\s+", "");
                // skip empty lines
                if (line == "")
                    continue;

                yield return line;
            }
        }
    }
}
