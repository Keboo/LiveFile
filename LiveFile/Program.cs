using System.CommandLine;
using System.CommandLine.IO;

namespace LiveFile;
public class Program
{
    /// <summary>
    /// A countdown timer application
    /// </summary>
    /// <param name="countdownTo">The target time to count down to</param>
    /// <param name="outputFile">The output file to write to</param>
    /// <param name="verbosity">The verbosity of the output</param>
    /// <param name="console">The console</param>
    /// <returns></returns>
    public static async Task Main(DateTime countdownTo, FileInfo? outputFile, Verbosity verbosity, IConsole console)
    {
        if (outputFile is null)
        {
            outputFile = new FileInfo("output.txt");
        }
        if (verbosity >= Verbosity.Normal)
        {
            //NB: WriteLine is an extenion method
            console.Out.WriteLine($"Running countdown to {countdownTo} in '{outputFile!.FullName}'");
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
    }
}
