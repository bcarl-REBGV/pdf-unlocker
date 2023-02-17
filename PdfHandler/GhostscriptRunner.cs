using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace PdfHandler
{
    public static class GhostscriptRunner
    {
        public static Task<Uri?> UnlockPdf(Uri pathToFile)
        {
            TaskCompletionSource<Uri?> completionSource = new TaskCompletionSource<Uri?>();
            
            if (pathToFile.Segments.Length < 1) throw new ArgumentException("Invalid file path");
            string outputFilename = "(Unlocked) " + pathToFile.Segments[^1];
            string outputPath = Uri.UnescapeDataString(string.Concat(pathToFile.Segments[1..^1]) + outputFilename);
            string inputPath = pathToFile.LocalPath;

            string WrapWithQuotes(string input) => "\"" + input + "\"";

            CommandLineArgs argDict = new CommandLineArgs()
            {
                { "-sDEVICE", "pdfwrite" },
                { "-dPDFSETTINGS", "/default" },
                { "-dNOPAUSE", "" },
                { "-dQUIET", "" },
                { "-dBATCH", ""},
                { "-sOutputFile", WrapWithQuotes(outputPath) },
                { WrapWithQuotes(inputPath), "" }
            };

            if (!File.Exists(Uri.UnescapeDataString(pathToFile.AbsolutePath))) throw new ArgumentException("Invalid file path");

            var cwd = AppContext.BaseDirectory;

            ProcessStartInfo gsInfo = new ProcessStartInfo( $"{cwd}\\gswin64c.exe", argDict.ToString())
            {
                CreateNoWindow = true
            };
            Process gsProcess = new Process { StartInfo = gsInfo };
            gsProcess.EnableRaisingEvents = true;

            gsProcess.Exited += (sender, e) =>
            {
                if (gsProcess.ExitCode != 0)
                {
                    var code = gsProcess.ExitCode;
                    Trace.WriteLine(code);
                    gsProcess.Dispose();
                    completionSource.SetResult(null);
                    throw new ApplicationException("Ghostscript process failed!");
                }
                completionSource.SetResult(new Uri(outputPath));
                gsProcess.Dispose();
            };
            gsProcess.Start();
            return completionSource.Task;
        }
}

}