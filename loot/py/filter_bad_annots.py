"""
Filters Bad Annotations from a Form-Field Enabled PDF document so that it can work better with Syncfusion's SfPdfViewer2
"""
import sys, pathlib, PyPDF2

def retain_text_and_checkboxes(reader, writer):
    """Copy pages but keep only /Tx (text) and /Btn (checkbox) fields."""
    for page in reader.pages:
        if "/Annots" in page:
            annots     = page["/Annots"]
            kept_annots = [
                ref for ref in annots
                if (ft := ref.get_object().get("/FT")) in ("/Tx", "/Btn")
            ]
            if kept_annots:
                page[PyPDF2.generic.NameObject("/Annots")] = PyPDF2.generic.ArrayObject(kept_annots)
            else:
                del page["/Annots"]
        writer.add_page(page)

def main(pdf_path: str):
    src  = pathlib.Path(pdf_path).resolve()
    if not src.exists() or src.suffix.lower() != ".pdf":
        sys.exit("Provide a valid PDF file.")

    dst = src.with_name(f"{src.stem}_noBadAnnots.pdf")

    reader = PyPDF2.PdfReader(str(src))
    writer = PyPDF2.PdfWriter()
    retain_text_and_checkboxes(reader, writer)

    with dst.open("wb") as f:
        writer.write(f)
    print(f"✔ Saved: {dst}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit("Usage: python filter_bad_annots.py <file.pdf>")
    main(sys.argv[1])
