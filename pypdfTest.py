from PyPDF2 import PdfFileReader, PdfFileWriter
def readFile(filename: str) -> str:
    with open(filename, 'rb') as input:
        pages = PdfFileReader(input).pages[:]
        output = PdfFileWriter()
        for page in pages:
            output.add_page(page)
        with open("destination.pdf", "wb") as out:
            output.write(out)
readFile("source.pdf")