﻿using System.Diagnostics;

namespace PdfHandler
{
    public static class GhostscriptRunner
    {
        public static Task<Uri?> UnlockPdf(Uri pathToFile)
        {
            TaskCompletionSource<Uri?> tsc = new TaskCompletionSource<Uri?>();

            if (pathToFile.Segments.Length < 1) throw new ArgumentException("Invalid file path");
            string outputFilename = "(Unlocked) " + pathToFile.Segments[^1];
            string outputPath = string.Concat(pathToFile.Segments[1..^1]) + outputFilename;
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

            if (!File.Exists(pathToFile.AbsolutePath)) throw new ArgumentException("Invalid file path");

            ProcessStartInfo gsInfo = new ProcessStartInfo( "gswin64c.exe", argDict.ToString())
            {
                CreateNoWindow = true
            };
            Process gsProcess = new Process { StartInfo = gsInfo };
            gsProcess.EnableRaisingEvents = true;

            gsProcess.Exited += (_,_) =>
            {
                if (gsProcess.ExitCode != 0)
                {
                    gsProcess.Dispose();
                    tsc.SetResult(null);
                    throw new ApplicationException("Ghostscript process failed!");
                }
                tsc.SetResult(new Uri(outputPath));
                gsProcess.Dispose();
            };
            gsProcess.Start();
            return tsc.Task;
        }
}

}