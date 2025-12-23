"""
Removes all form-fields and annotations from a PDF
"""
import sys, pathlib, PyPDF2

def wipe_pdf(src_path: pathlib.Path):
    dst_path = src_path.with_name(f"{src_path.stem}_noFields.pdf")

    reader = PyPDF2.PdfReader(str(src_path))
    writer = PyPDF2.PdfWriter()

    for i, page in enumerate(reader.pages, 1):
        if "/Annots" in page:                 # remove page-level widgets / links
            print(f"Page {i}: removing annotation array")
            page.pop("/Annots")
        writer.add_page(page)

    writer._root_object.pop("/AcroForm", None)  # strip document-level forms
    if reader.metadata:
        writer.add_metadata(reader.metadata)     # keep basic metadata

    with dst_path.open("wb") as f:
        writer.write(f)

    print(f"✔ Clean PDF saved as: {dst_path}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit("Usage: python wipe_fields.py <file.pdf>")
    wipe_pdf(pathlib.Path(sys.argv[1]).resolve())
