using System.Text.RegularExpressions;
using Shared;

namespace ScannerA
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the directory containing .txt files:");
            string directoryPath = Console.ReadLine();

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            var index = BuildIndexFromDirectory(directoryPath);

            // Display results locally 
            foreach (var entry in index)
            {
                Console.WriteLine(entry.ToString());
            }

            
        }

        static List<WordIndexEntry> BuildIndexFromDirectory(string directoryPath)
        {
            var entries = new List<WordIndexEntry>();
            var files = Directory.GetFiles(directoryPath, "*.txt");

            foreach (var file in files)
            {
                string content = File.ReadAllText(file);
                string fileName = Path.GetFileName(file);
                var wordCounts = CountWords(content);

                foreach (var pair in wordCounts)
                {
                    entries.Add(new WordIndexEntry(fileName, pair.Key, pair.Value));
                }
            }

            return entries;
        }

        static Dictionary<string, int> CountWords(string content)
        {
            var wordCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var matches = Regex.Matches(content, @"\b\w+\b");

            foreach (Match match in matches)
            {
                string word = match.Value.ToLower();

                if (wordCounts.ContainsKey(word))
                    wordCounts[word]++;
                else
                    wordCounts[word] = 1;
            }

            return wordCounts;
        }
    }
}
// This code reads text files from a specified directory, counts the occurrences of each word in those files,