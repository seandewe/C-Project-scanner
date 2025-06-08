using System.Text.RegularExpressions;
using Shared;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;


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
    string pipeName = "agent2";  // This should match the pipe name used by the master process

    SendDataOverNamedPipe(index, pipeName);
}

static void SendDataOverNamedPipe(List<WordIndexEntry> entries, string pipeName)
{
    try
    {
        using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
        {
            Console.WriteLine($"Connecting to master on pipe '{pipeName}'...");
            pipeClient.Connect(); // waits for master

            string json = JsonSerializer.Serialize(entries);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            pipeClient.Write(buffer, 0, buffer.Length);
            pipeClient.Flush();

            Console.WriteLine("Data sent to master successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to send data: {ex.Message}");
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