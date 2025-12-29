using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedWebBrowser
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; private set; }
        public bool IsEndOfWord { get; set; }
        public int SearchCount { get; set; }

        public TrieNode()
        {
            Children = new Dictionary<char, TrieNode>();
            IsEndOfWord = false;
            SearchCount = 0;
        }
    }

    public class SearchSuggestionTree
    {
        private TrieNode root;
        private readonly int maxSuggestions;

        public SearchSuggestionTree(int maxSuggestions = 10)
        {
            root = new TrieNode();
            this.maxSuggestions = maxSuggestions;
        }

        public void AddSuggestion(string word)
        {
            var currentNode = root;

            foreach (char c in word.ToLower())
            {
                if (!currentNode.Children.ContainsKey(c))
                {
                    currentNode.Children[c] = new TrieNode();
                }
                currentNode = currentNode.Children[c];
            }

            currentNode.IsEndOfWord = true;
            currentNode.SearchCount++;
        }

        public void IncrementSearchCount(string word)
        {
            var node = FindNode(word.ToLower());
            if (node != null)
            {
                node.SearchCount++;
            }
        }

        public List<string> GetSuggestions(string prefix)
        {
            var suggestions = new List<(string word, int count)>();
            var prefixNode = FindNode(prefix.ToLower());

            if (prefixNode != null)
            {
                CollectSuggestions(prefixNode, prefix, suggestions);
            }

            // Sort by search count (popularity) and then alphabetically
            return suggestions
                .OrderByDescending(s => s.count)
                .ThenBy(s => s.word)
                .Take(maxSuggestions)
                .Select(s => s.word)
                .ToList();
        }

        private TrieNode FindNode(string prefix)
        {
            var currentNode = root;

            foreach (char c in prefix)
            {
                if (!currentNode.Children.ContainsKey(c))
                    return null;
                currentNode = currentNode.Children[c];
            }

            return currentNode;
        }

        private void CollectSuggestions(TrieNode node, string currentWord, List<(string word, int count)> suggestions)
        {
            if (node.IsEndOfWord)
            {
                suggestions.Add((currentWord, node.SearchCount));
            }

            foreach (var kvp in node.Children)
            {
                CollectSuggestions(kvp.Value, currentWord + kvp.Key, suggestions);
            }
        }

        public List<string> GetAllWords()
        {
            var words = new List<string>();
            CollectAllWords(root, "", words);
            return words;
        }

        private void CollectAllWords(TrieNode node, string currentWord, List<string> words)
        {
            if (node.IsEndOfWord)
            {
                words.Add(currentWord);
            }

            foreach (var kvp in node.Children)
            {
                CollectAllWords(kvp.Value, currentWord + kvp.Key, words);
            }
        }
    }
}