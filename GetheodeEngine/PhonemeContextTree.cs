using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GetheodeEngine
{
    public class PhonemeContextTree
    {
        public class ContextTreeNode
        {
            /// <summary>
            /// If true, the children are read as: A or B or C...
            /// If false, the children are read as A then B then C...
            /// </summary>
            public bool IsOrTree { get; set; }
            public ContextTreeNode Parent { get; set; }
            public List<ContextTreeNode> Children { get; }
            public string Symbol { get; set; }

            public ContextTreeNode()
            {
                Children = new List<ContextTreeNode>();
            }

            /// <summary>Adds a child to the tree</summary>
            /// <param name="symbol">The symbol for the child to have</param>
            /// <returns>The new child</returns>
            public ContextTreeNode AddChild(string symbol)
            {
                ContextTreeNode child = new ContextTreeNode();
                child.Parent = this;
                child.Symbol = symbol;
                Children.Add(child);
                return child;
            }

            /// <summary>
            /// If this node only has one child and it's symbol is null,
            /// then it is unnecesary, and will be removed. This method will
            /// then be called on children nodes.
            /// </summary>
            public void CollapseUnnecesaryNodes()
            {
                if(Parent != null)
                {
                    if (Children.Count == 1 && Symbol==null)
                    {
                        //the index of this node in the parent's children list
                        int myIndex = Parent.Children.FindIndex(x => x == this);
                        Parent.Children[myIndex] = Children[0];
                        Children[0].Parent = Parent;
                    }
                }
                // Recurse on children
                for (int i = 0; i < Children.Count; i++)
                    Children[i].CollapseUnnecesaryNodes();
            }
        }

        ContextTreeNode beforeRoot;
        ContextTreeNode afterRoot;

        public PhonemeContextTree(string contextString)
        {
            string[] split = contextString.Split('_');
            beforeRoot = ParseContextString(split[0]);
            afterRoot = ParseContextString(split[1]);
        }

        public static ContextTreeNode ParseContextString(string contextString)
        {
            //characters that indicate the end of a symbol
            char[] symbolFinishedChars = new char[] { 'C', 'V', '(', ')' };

            ContextTreeNode root = new ContextTreeNode();
            bool isInSymbol = false; //is reading a symbol: V, or C[m=plos]
            string curSymbol = ""; //Temp var for the symbol currently being read
            foreach (char c in contextString)
            {
                if (isInSymbol)
                {
                    if (c == 'C' || c == 'V' || c == '('||c==')')
                    { //exit the symbol
                        root.AddChild(curSymbol);
                        isInSymbol = false;
                        //don't continue so that the current
                        //symbol will still be parsed
                    }
                    else
                    {
                        curSymbol += c;
                        continue;
                    }
                }
                if (c == '(')
                {
                    root = root.AddChild(null);
                }
                else if (c==')')
                {
                    ContextTreeNode parent = root.Parent;
                    if (parent == null)
                        throw new ArgumentException("contextString " + contextString + " is unbalanced!");

                    if (root.Parent != null)
                        root = root.Parent;
                }
                else if (c == 'C' || c == 'V')
                {
                    isInSymbol = true;
                    curSymbol = c.ToString();
                }
            }

            root.CollapseUnnecesaryNodes();
            return root;
        }
    }

}
