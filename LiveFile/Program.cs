using LiveFile;
using System.CommandLine;
using System.CommandLine.IO;

Option<DateTime> countdownToOption = new(
    name: "--countdown-to",
    description: "The target time to count down to");
Option<FileInfo> outputFileOption = new(
    name: "--output-file",
    getDefaultValue: () => new FileInfo("output.txt"),
    description: "The output file to write to");
Option<Verbosity> verbosityOption = new(
    aliases: new[] { "--verbosity", "-v" },
    description: "The verbosity of the output");


RootCommand rootCommand = new(description: "A countdown timer application")
{
    countdownToOption,
    outputFileOption,
    verbosityOption,
};
rootCommand.SetHandler(
    async (DateTime countdownTo, FileInfo outputFile, Verbosity verbosity, IConsole console) =>
    {
        if (verbosity >= Verbosity.Normal)
        {
            //NB: WriteLine is an extenion method
            console.Out.WriteLine($"Running countdown to {countdownTo} in '{outputFile.FullName}'");
        }

        while (countdownTo > DateTime.Now)
        {
            string newText = string.Format(@"{0:mm\:ss}", (countdownTo - DateTime.Now));
            if (verbosity >= Verbosity.Detailed)
            {
                console.Out.WriteLine(newText);
            }
            try
            {
                await File.WriteAllTextAsync(outputFile.FullName, newText);
            }
            catch (IOException)
            { }
            await Task.Delay(250);
        }

        await File.WriteAllTextAsync(outputFile.FullName, "Starting soon");

        if (verbosity > Verbosity.Normal)
        {
            console.Out.Write("Countdown complete.");
        }
    },
    countdownToOption,
    outputFileOption,
    verbosityOption);

await rootCommand.InvokeAsync(args);

//using System;
//using System.CommandLine;
//using System.CommandLine.IO;
//using System.IO;
//using System.Threading.Tasks;

//namespace LiveFile;

//class Program
//{
//    /// <summary>
//    /// Write updates to a file
//    /// </summary>
//    /// <param name="console"></param>
//    /// <param name="countdownTo">The target time to count down to</param>
//    /// <param name="endMessage">The message to display when the count down is not running.</param>
//    /// <param name="timerFormat">The format for the time display. Any valid C# format string. </param>
//    /// <param name="outputFile">The file to write to.</param>
//    /// <param name="keepFile">Delete file when program terminates.</param>
//    /// <param name="verbosity">The verbosity of the output</param>
//    static async Task Main2(
//        IConsole console,
//        DateTime countdownTo,
//        string? endMessage = null,
//        string timerFormat = @"{0:mm\:ss}",
//        string outputFile = "output.txt",
//        bool keepFile = false,
//        Verbosity verbosity = Verbosity.Normal)
//    {
//        if (verbosity >= Verbosity.Normal)
//        {
//            console.Out.WriteLine($"Running countdown to {countdownTo} in '{Path.GetFullPath(outputFile)}'");
//        }

//        while (countdownTo > DateTime.Now)
//        {
//            string newText;
//            try
//            {

//                newText = string.Format(timerFormat, (countdownTo - DateTime.Now));
//            }
//            catch (FormatException)
//            {
//                console.Out.WriteLine($"'{timerFormat}' is not a valid format specifier. See: https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings");
//                return;
//            }

//            await WriteFileAsync(console, newText, outputFile, verbosity);
//            await Task.Delay(250);
//        }

//        await WriteFileAsync(console, $"{endMessage}", outputFile, verbosity);

//        if (verbosity > Verbosity.Normal)
//        {
//            console.Out.Write("Countdown complete.");
//        }

//        if (!keepFile)
//        {
//            if (verbosity > Verbosity.Normal)
//            {
//                console.Out.WriteLine("Press any key to close");
//                Console.ReadKey();
//            }
//            try
//            {
//                File.Delete(outputFile);
//            }
//            catch (IOException)
//            { }
//        }
//    }

//    private static string? _lastFileContents;
//    private static async Task WriteFileAsync(IConsole console, string contents, string file, Verbosity verbosity)
//    {
//        if (contents != _lastFileContents)
//        {
//            try
//            {
//                if (verbosity >= Verbosity.Detailed)
//                {
//                    console.Out.WriteLine(contents);
//                }
//                await File.WriteAllTextAsync(file, contents);
//                _lastFileContents = contents;
//            }
//            catch (IOException)
//            { }
//        }
//    }
//}
