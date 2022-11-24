$TextDocumentPath = $args[0]
Add-Type -AssemblyName System.Drawing
$doc = New-Object System.Drawing.Printing.PrintDocument
$doc.Documentname = $TextDocumentPath
$doc.PrinterSettings = new-Object System.Drawing.Printing.PrinterSettings
$doc.PrinterSettings.PrinterName = "Microsoft Print to PDF"
$doc.Printersettings.PrintToFile = $true
$file = [io.fileinfo]$TextDocumentPath
$pdf = [io.path]::Combine($file.DirectoryName, "(Unlocked) ", $file.BaseName) + '.pdf'
$doc.PrinterSettings.PrintFileName = $pdf
$doc.Print()
$doc.Dispose()