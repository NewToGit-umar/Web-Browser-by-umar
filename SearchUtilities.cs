using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedWebBrowser
{
    public static class SearchUtilities
    {
        private static readonly string[] SearchEngines = {
            "https://www.google.com/search?q=",
            "https://www.bing.com/search?q=",
            "https://search.yahoo.com/search?p=",
            "https://duckduckgo.com/?q="
        };

        // Modern sites that need special handling
        private static readonly Dictionary<string, string> ModernSites = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"chat.openai.com", "https://chat.openai.com/"},
            {"chatgpt.com", "https://chatgpt.com/"},
            {"web.whatsapp.com", "https://web.whatsapp.com/"},
            {"gemini.google.com", "https://gemini.google.com/"},
            {"github.com", "https://github.com/"},
            {"archive.org", "https://archive.org/"},
            {"youtube.com", "https://www.youtube.com/"},
            {"twitter.com", "https://twitter.com/"},
            {"facebook.com", "https://www.facebook.com/"},
            {"instagram.com", "https://www.instagram.com/"},
            {"reddit.com", "https://www.reddit.com/"},
            {"discord.com", "https://discord.com/"},
            {"netflix.com", "https://www.netflix.com/"},
            {"spotify.com", "https://www.spotify.com/"}
        };

        public static string FormatSearchUrl(string searchTerm, int searchEngineIndex = 0)
        {
            if (IsUrl(searchTerm))
            {
                return EnsureProtocol(searchTerm);
            }

            // Check if it's a known modern site
            if (!string.IsNullOrEmpty(searchTerm))
            {
                foreach (var site in ModernSites)
                {
                    if (searchTerm.IndexOf(site.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return site.Value;
                    }
                }
            }

            // Clean the search term
            string cleanSearchTerm = (searchTerm ?? string.Empty).Trim();

            if (searchEngineIndex >= 0 && searchEngineIndex < SearchEngines.Length)
            {
                return SearchEngines[searchEngineIndex] + Uri.EscapeDataString(cleanSearchTerm);
            }

            return SearchEngines[0] + Uri.EscapeDataString(cleanSearchTerm);
        }

        public static bool IsUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string lowerInput = input.ToLowerInvariant().Trim();

            // Check for modern sites first
            foreach (var site in ModernSites)
            {
                if (lowerInput.IndexOf(site.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            // Check structural characteristics
            bool hasDot = lowerInput.Contains(".");
            bool hasProtocol = lowerInput.StartsWith("http://") ||
                               lowerInput.StartsWith("https://") ||
                               lowerInput.StartsWith("www.");

            // Common domain suffixes
            bool isCommonDomain = lowerInput.EndsWith(".com") ||
                                  lowerInput.EndsWith(".org") ||
                                  lowerInput.EndsWith(".net") ||
                                  lowerInput.EndsWith(".edu") ||
                                  lowerInput.EndsWith(".gov") ||
                                  lowerInput.EndsWith(".io") ||
                                  lowerInput.EndsWith(".ai");

            // If contains spaces it's unlikely to be a URL
            if (lowerInput.Contains(" ") && !hasProtocol)
                return false;

            return (hasDot && hasProtocol) || (hasDot && isCommonDomain);
        }

        public static string EnsureProtocol(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "about:blank";

            string trimmed = url.Trim();
            string lowerUrl = trimmed.ToLowerInvariant();

            if (lowerUrl.StartsWith("http://") || lowerUrl.StartsWith("https://"))
                return trimmed;

            if (lowerUrl.StartsWith("www."))
                return "https://" + trimmed;

            // If it looks like a domain but missing www, add https://
            if (trimmed.Contains(".") && !trimmed.Contains(" "))
                return "https://" + trimmed;

            return trimmed;
        }

        public static bool IsModernSite(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;

            foreach (var site in ModernSites)
            {
                if (url.IndexOf(site.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        public static List<string> QuickSortSuggestions(List<string> suggestions)
        {
            if (suggestions == null)
                return new List<string>();

            if (suggestions.Count <= 1)
                return suggestions;

            var pivot = suggestions[0];
            var left = new List<string>();
            var right = new List<string>();

            for (int i = 1; i < suggestions.Count; i++)
            {
                if (string.Compare(suggestions[i], pivot, StringComparison.OrdinalIgnoreCase) < 0)
                    left.Add(suggestions[i]);
                else
                    right.Add(suggestions[i]);
            }

            var sorted = new List<string>();
            sorted.AddRange(QuickSortSuggestions(left));
            sorted.Add(pivot);
            sorted.AddRange(QuickSortSuggestions(right));

            return sorted;
        }

        // New helper to allow callers to get the search engine base URL
        public static string GetSearchEngineBase(int index)
        {
            if (index >= 0 && index < SearchEngines.Length)
                return SearchEngines[index];
            return SearchEngines[0];
        }

        public static int SearchEngineCount => SearchEngines.Length;
    }
}