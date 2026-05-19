namespace TypoFixer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

internal static class Program
{
    private static void Main()
    {
        var wordsList = new HashSet<string>();
        const string wordsListFile = "/Users/admin/Documents/unik/it/ads/assignment-4-typo-fixer-asd-1-ozinchuk-oturash/TypoFixer/TypoFixer/words_list.txt";
            
        foreach (var line in File.ReadLines(wordsListFile)) 
        { 
            wordsList.Add(line.Trim());
        }

        Console.Write("");
        var input = Console.ReadLine();
        if (input == null) return;
        
        var words = input.Split(new[] { ' ', ',', '.', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
        var unknownWords = words.Where(w => !string.IsNullOrEmpty(w) && !wordsList.Contains(w)).ToList();
        
        var distance = new LevenshteinDistance();
        if (unknownWords.Count > 0)
        {
            Console.WriteLine($"You have typos in words '{string.Join("', '", unknownWords)}'\n");
            foreach (var word in unknownWords)
            {
                var suggestions = distance.GetSuggestions(word, wordsList);
                Console.WriteLine($"Did you mean by '{word}':");
                foreach (var suggestion in suggestions)
                {
                    Console.WriteLine($"  → {suggestion.Word} (distance: {suggestion.Distance})");
                }
                Console.WriteLine();
            }

        }
    }
}