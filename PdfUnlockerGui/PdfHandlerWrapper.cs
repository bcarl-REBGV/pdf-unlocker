﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PdfHandler;

namespace PdfUnlockerGui;

/// <summary>
/// Wrapper class around pdf library implementation in case of dependency changes.
/// </summary>
internal static class PdfHandlerWrapper
{
    public static Task<Uri?> UnlockPdf(Uri pathToFile)
    {
        Task<Uri?> task = GhostscriptRunner.UnlockPdf(pathToFile);
        return task;
    }
}