import os
import subprocess


def flattenPdf(filename: str):
    os.startfile()
    return

def unlockPdf(filename: str) -> str:
    base_path, name = os.path.split(filename)
    base_path = base_path.decode('utf-8')
    name = name.decode('utf-8')
    out_name = f"(Unlocked) {name}"
    full_out = f"{base_path}\\{out_name}"
    subprocess.call("")


if __name__ == "__main__":
    unlockPdf("source.pdf")
