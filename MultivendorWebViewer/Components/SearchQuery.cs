using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultivendorWebViewer.Components
{
    public enum SearchQueryNodeType { Text, And, Or };

    public class SearchQueryNode
    {
        public SearchQueryNodeType Type { get; set; }

        public string Text { get; set; }

        public List<SearchQueryNode> Nodes { get; set; }

        public virtual IEnumerable<T> GetHits<T>(Func<string, IEnumerable<T>> hitsProvider, IEqualityComparer<T> comparer)
        {
            if (Type == SearchQueryNodeType.Text)
            {
                return Text != null ? hitsProvider(Text) : Enumerable.Empty<T>();
            }

            if (Nodes.Count == 1)
            {
                return Nodes[0].GetHits(hitsProvider, comparer);
            }
            else if (Nodes.Count > 1)
            {
                var h = Nodes[0].GetHits(hitsProvider, comparer);
                var hits = new HashSet<T>(h, comparer);
                for (int i = 1; i < Nodes.Count; i++)
                {
                    if (Type == SearchQueryNodeType.And)
                    {
                        var nextHits = Nodes[i].GetHits(hitsProvider, comparer);
                        hits.IntersectWith(nextHits);
                    }
                    else
                    {
                        var nextHits = Nodes[i].GetHits(hitsProvider, comparer);
                        hits.UnionWith(nextHits);
                    }
                }
                return hits;
            }

            return Enumerable.Empty<T>();
        }

        public override string ToString()
        {
            return Text != null ? Text : Nodes.Count > 0 ? "(" + string.Join((Type == SearchQueryNodeType.And ? " AND " : " OR "), Nodes.Count) + ")" : "";
        }

        private static string GetParenthes(string str)
        {
            int i = 0;
            int level = 0;
            int start = 0;
            while (i < str.Length)
            {
                char c = str[i];
                if (c == '(')
                {
                    level++;
                }
                else if (c == ')')
                {
                    level--;
                    if (level == 0)
                    {
                        return str.Substring(start, (i + 1) - start);
                    }
                }
                else if (level == 0)
                {
                    if (char.IsWhiteSpace(c) == false)
                    {
                        return null;
                    }
                    else
                    {
                        start = i;
                    }
                }

                i++;
            }
            return null;
        }

        private static string TrimOuterParentheses(string str)
        {
            var trimmedStr = str.Trim();
            do
            {
                string paran = GetParenthes(trimmedStr);
                if (paran != null && paran.Length == trimmedStr.Length)
                {
                    if (paran.Length > 2)
                    {
                        trimmedStr = trimmedStr.Substring(1, trimmedStr.Length - 2);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return trimmedStr;
                }
            } while (true);       
        }

        private static List<SearchQueryNode> GetAsOrNode(string queryString)
        {
            var nodes = new List<SearchQueryNode>();

            queryString = TrimOuterParentheses(queryString).Trim();

            if (queryString.Length == 0) return nodes;

            int i = 0;
            int nodeStart = 0;
            int level = 0;
            int nodeLength;
            while (i < queryString.Length)
            {
                char c = queryString[i];

                if (c == '(')
                {
                    level++;
                }
                else if (c == ')')
                {
                    level--;
                }
                else if (level == 0)
                {
                    if (c == '|')
                    {
                        nodeLength = (i - nodeStart);
                        if (nodeLength > 0)
                        {
                            string nodeQueryString = queryString.Substring(nodeStart, nodeLength);
                            nodes.Add(SearchQueryNode.CreateInternal(nodeQueryString));
                            nodeStart = i + 1;
                        }
                    }
                }
                i++;
            }

            if (nodes.Count > 0)
            {
                nodeLength = (i - nodeStart);
                if (nodeLength > 0)
                {
                    string nodeQueryString = queryString.Substring(nodeStart, nodeLength);
                    nodes.Add(SearchQueryNode.CreateInternal(nodeQueryString));
                }
            }

            return nodes;
        }

        private static List<SearchQueryNode> GetAsAndNode(string queryString)
        {
            var nodes = new List<SearchQueryNode>();

            queryString = TrimOuterParentheses(queryString);

            if (queryString.Length == 0) return nodes;

            int i = 0;
            int nodeStart = 0;
            int level = 0;
            int nodeLength = 0;
            while (i < queryString.Length)
            {
                char c = queryString[i];

                if (c == '(')
                {
                    level++;
                }
                else if (c == ')')
                {
                    level--;
                }
                else if (level == 0)
                {
                    if (char.IsWhiteSpace(c) == true)
                    {
                        nodeLength = (i - nodeStart);
                        if (nodeLength > 0)
                        {
                            string nodeQueryString = queryString.Substring(nodeStart, nodeLength);
                            nodes.Add(SearchQueryNode.CreateInternal(nodeQueryString));
                            nodeStart = i + 1;
                        }
                    }
                }
                i++;
            }

            if (nodes.Count > 0)
            {
                nodeLength = (i - nodeStart);
                if (nodeLength > 0)
                {
                    string nodeQueryString = queryString.Substring(nodeStart, nodeLength);
                    nodes.Add(SearchQueryNode.CreateInternal(nodeQueryString));
                }
            }

            return nodes;
        }

        public static SearchQueryNode Create(string queryString)
        {
            return SearchQueryNode.CreateInternal(queryString.Trim());
        }

        private static SearchQueryNode CreateInternal(string queryString)
        {
            var orNodes = GetAsOrNode(queryString);
            if (orNodes.Count > 0)
            {
                return new SearchQueryNode { Type = SearchQueryNodeType.Or, Nodes = orNodes };
            }

            var andNodes = GetAsAndNode(queryString);
            if (andNodes.Count > 0)
            {
                return new SearchQueryNode { Type = SearchQueryNodeType.And, Nodes = andNodes };
            }

            return new SearchQueryNode { Type = SearchQueryNodeType.Text, Text = TrimOuterParentheses(queryString) };
        }
    }

    public class SearchQuery
    {
        public SearchQuery(string queryString)
        {
            if (queryString != null)
            {
                InputString = queryString.Trim();

                TransformedString = InputString;
            }
        }

        public static SearchQuery Empty { get; private set; } = new SearchQuery(null);

        public bool IsEmpty => string.IsNullOrWhiteSpace(InputString) || InputString == "*";

        private SearchQueryNode node;
        public SearchQueryNode Node 
        {
            get { return node ?? (node = SearchQueryNode.Create(TransformedString)); } 
        }

        public string InputString { get; private set; }

        private string transformedString;
        public string TransformedString 
        {
            get { return transformedString; }
            set
            {
                if (transformedString != value)
                {
                    node = null;
                    transformedString = value;
                }
            }
        }

        public IEnumerable<string> Words 
        { 
            get 
            { 
                if (Node.Text != null)
                {
                    yield return Node.Text;
                }
                
                //if (Node.Nodes != null)
                //{
                //    foreach (var node in Node.Nodes.SelectRecursive(n => n.Nodes))
                //    {
                //        if (node.Text != null)
                //        {
                //            yield return node.Text;
                //        }
                //    }
                //}
            } 
        }

        public IEnumerable<T> GetHits<T>(Func<string, IEnumerable<T>> hitsProvider, IEqualityComparer<T> comparer)
        {
            return Node.GetHits(hitsProvider, comparer);
        }

        public override string ToString()
        {
            return InputString;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchQuery && InputString.Equals(((SearchQuery)obj).InputString);
        }

        public override int GetHashCode()
        {
            return InputString.GetHashCode();
        }

        public static SearchQuery Create(string queryString)
        {
            return new SearchQuery(queryString);
        }
    }
}
