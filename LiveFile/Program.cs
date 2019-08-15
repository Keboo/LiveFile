using System;
using System.CommandLine;
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
        /// <param name="verbose">Display verbose output</param>
        static async Task Main(IConsole console, 
            DateTime? countdownTo = null, 
            string endMessage = null, 
            string timerFormat = "h\\:mm\\:ss",
            string outputFile = "output.txt",
            bool verbose = false)
        {
            console.Out.WriteLine($"Running countdown to {countdownTo} in '{Path.GetFullPath(outputFile)}'");
            while (countdownTo > DateTime.Now)
            {
                string newText;
                try
                {
                    newText = (countdownTo.Value - DateTime.Now).ToString(timerFormat);
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

            console.Out.WriteLine("Countdown complete. Press any key to close");
            Console.ReadKey();
            try
            {
                File.Delete(outputFile);
            }
            catch (IOException)
            {
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
