using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using Shared;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MasterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SetProcessorAffinity(0); // Set Master to Core 0

            Console.WriteLine("Master started. Waiting for agents...");

            var allEntries = new List<WordIndexEntry>();

            Thread t1 = new Thread(() => ReceiveFromAgent("agent1", allEntries));
            Thread t2 = new Thread(() => ReceiveFromAgent("agent2", allEntries));

            t1.Start();
            t2.Start();

            t1.Join();
            t2.Join();

            Console.WriteLine("\nFinal Aggregated Word Index:\n");

            foreach (var entry in allEntries)
            {
                Console.WriteLine(entry.ToString());
            }
        }

        static void ReceiveFromAgent(string pipeName, List<WordIndexEntry> allEntries)
        {
            try
            {
                using (NamedPipeServerStream pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
                {
                    Console.WriteLine($"Waiting for connection on pipe '{pipeName}'...");
                    pipeServer.WaitForConnection();

                    using (StreamReader reader = new StreamReader(pipeServer, Encoding.UTF8))
                    {
                        string json = reader.ReadToEnd();
                        var entries = JsonSerializer.Deserialize<List<WordIndexEntry>>(json);

                        if (entries != null)
                        {
                            lock (allEntries)
                            {
                                allEntries.AddRange(entries);
                            }

                            Console.WriteLine($"Received {entries.Count} entries from {pipeName}.");
                        }
                        else
                        {
                            Console.WriteLine($"Received null data from {pipeName}.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving from {pipeName}: {ex.Message}");
            }
        }

        static void SetProcessorAffinity(int core)
        {
            Process proc = Process.GetCurrentProcess();
            proc.ProcessorAffinity = (IntPtr)(1 << core);
        }
    }
}
