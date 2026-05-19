namespace TypoFixer;

public class LevenshteinDistance
{
    private static int CalculateDamerauLevenshteinDistance(string s1, string s2)
    {
        var length1 = s1.Length;
        var length2 = s2.Length;
        var distanceTable = new int[length1 + 1, length2 + 1];
        
        for (int i = 0; i <= length1; i++) distanceTable[i, 0] = i;
        for (int j = 0; j <= length2; j++) distanceTable[0, j] = j;
        
        for (var i = 1; i <= length1; i++)
        {
            for (var j = 1; j <= length2; j++)
            {
                int cost;
                if (s1[i - 1] == s2[j - 1]) 
                {
                    cost = 0;
                } 
                else 
                {
                    cost = 1;
                }
                distanceTable[i, j] = Math.Min(
                        Math.Min(distanceTable[i - 1, j] + 1,
                        distanceTable[i, j - 1] + 1),
                        distanceTable[i - 1, j - 1] + cost);
                
                if (i > 1 && j > 1 && s1[i - 1] == s2[j - 2] && s1[i - 2] == s2[j - 1])
                {
                    distanceTable[i, j] = Math.Min(distanceTable[i, j], distanceTable[i - 2, j - 2] + 1);
                }
                
                if (i > 1 && j > 1 &&
                    s1[i - 2] != s2[j - 2] &&
                    s1[i - 1] != s2[j - 1] &&
                    (s1.Substring(i - 2, 2) != s2.Substring(j - 2, 2)))
                {
                    distanceTable[i, j] = Math.Min(
                        distanceTable[i, j],
                        distanceTable[i - 2, j - 2] + 1
                    );
                }

            }
        }
        return distanceTable[length1, length2];
    }

    public List<(string Word, int Distance)> GetSuggestions(string word, HashSet<string> dictionary)
    {
        var candidates = dictionary
            .Where(w => Math.Abs(w.Length - word.Length) <= 2)
            .Select(dictWord => (
                Word: dictWord,
                Distance: CalculateDamerauLevenshteinDistance(word, dictWord)
            ))
            .OrderBy(entry => entry.Distance)
            .Take(5)
            .ToList();

        return candidates;
    }

}