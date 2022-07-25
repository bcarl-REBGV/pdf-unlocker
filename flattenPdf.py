import os
from PyPDF2 import PdfFileReader, PdfFileWriter


def unlockPdf(filename: str) -> str:
    with open(filename, 'rb') as input:
        pages = PdfFileReader(input).pages[:]
        output = PdfFileWriter()
        output.add_metadata({"/Producer": "REBGV"})
        base_path, name = os.path.split(filename)
        base_path = base_path.decode('utf-8')
        name = name.decode('utf-8')
        out_name = f"(Unlocked) {name}"
        full_out = f"{base_path}\\{out_name}"
        for page in pages:
            output.add_page(page)
        with open(full_out, "wb") as out:
            output.write(out)
        return full_out


if __name__ == "__main__":
    unlockPdf("source.pdf")
