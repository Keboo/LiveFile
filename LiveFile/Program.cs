using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Threading.Tasks;

namespace LiveFile
{
    class Program
    {
        /// <summary>
        /// Write updates to a file
        /// </summary>
        /// <param name="console"></param>
        /// <param name="countdownTo">The target time to count down to</param>
        /// <param name="endMessage">The message to display when the count down is not running.</param>
        /// <param name="timerFormat">The format for the time display. Any valid C# format string. </param>
        /// <param name="outputFile">The file to write to.</param>
        /// <param name="keepFile">Delete file when program terminates.</param>
        /// <param name="verbose">Display verbose output</param>
        static async Task Main(IConsole console,
            DateTime? countdownTo = null,
            string endMessage = null,
            string timerFormat = @"{0:mm\:ss}",
            string outputFile = "output.txt",
            bool keepFile = false,
            bool verbose = false)
        {
            console.Out.WriteLine($"Running countdown to {countdownTo} in '{Path.GetFullPath(outputFile)}'");
            while (countdownTo > DateTime.Now)
            {
                string newText;
                try
                {

                    newText = string.Format(timerFormat, (countdownTo.Value - DateTime.Now));
                }
                catch (FormatException)
                {
                    console.Out.WriteLine($"'{timerFormat}' is not a valid format specifier. See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings");
                    return;
                }

                WriteFile(console, newText, outputFile, verbose);
                await Task.Delay(250);
            }

            WriteFile(console, $"{endMessage}", outputFile, verbose);

            console.Out.Write("Countdown complete.");
            if (!keepFile)
            {
                console.Out.WriteLine(" Press any key to close");
                Console.ReadKey();

                try
                {
                    File.Delete(outputFile);
                }
                catch (IOException)
                {
                }
            }
        }

        private static string _lastFileContents;
        private static void WriteFile(IConsole console, string contents, string file, bool verbose)
        {
            if (contents != _lastFileContents)
            {
                try
                {
                    if (verbose)
                    {
                        console.Out.WriteLine(contents);
                    }
                    File.WriteAllText(file, contents);
                    _lastFileContents = contents;
                }
                catch (IOException)
                { }
            }
        }
    }
}
