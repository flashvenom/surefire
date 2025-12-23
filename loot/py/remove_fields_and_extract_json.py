"""
Removes all form-fields and annotations from a PDF
"""
# wipe_fields_v3.py
# PyPDF2 ≥ 3.x, Python ≥ 3.8
# Default:  input.pdf  →  output.pdf   (same directory)

import sys
import PyPDF2

def wipe_pdf(src="input.pdf", dst="output.pdf"):
    reader = PyPDF2.PdfReader(src)
    writer = PyPDF2.PdfWriter()

    for idx, page in enumerate(reader.pages, start=1):
        if "/Annots" in page:
            print(f"Page {idx}: removing annotation array")
            page.pop("/Annots")            # drop widgets, links, etc.
        writer.add_page(page)

    # drop any document-level form dictionary
    writer._root_object.pop("/AcroForm", None)

    # keep basic metadata
    if reader.metadata:
        writer.add_metadata(reader.metadata)

    with open(dst, "wb") as f_out:
        writer.write(f_out)

    print(f"\nClean PDF saved as: {dst}")

# ---------------- CLI ----------------
if __name__ == "__main__":
    src = sys.argv[1] if len(sys.argv) > 1 else "input.pdf"
    dst = sys.argv[2] if len(sys.argv) > 2 else "output.pdf"
    wipe_pdf(src, dst)
