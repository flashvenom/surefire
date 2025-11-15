import sys, pathlib, PyPDF2

def rename_all_fields(src_path: pathlib.Path):
    """Rename every /T field in the PDF to a unique FieldX label."""
    dst_path = src_path.with_name(f"{src_path.stem}_renamedFields.pdf")

    reader = PyPDF2.PdfReader(str(src_path))
    writer = PyPDF2.PdfWriter()

    counter = 1
    for p_num, page in enumerate(reader.pages, 1):
        if "/Annots" in page:
            for ref in page["/Annots"]:
                annot = ref.get_object()
                if "/T" in annot:
                    new_name = f"Field{counter}"
                    print(f"Page {p_num}: {annot['/T']} → {new_name}")
                    annot.update({
                        PyPDF2.generic.NameObject("/T"):
                        PyPDF2.generic.createStringObject(new_name)
                    })
                    counter += 1
        writer.add_page(page)

    # keep metadata & write out
    if reader.metadata:
        writer.add_metadata(reader.metadata)
    with dst_path.open("wb") as f:
        writer.write(f)
    print(f"\n✔ Output saved as: {dst_path}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        sys.exit("Usage: python rename_fields.py <file.pdf>")
    rename_all_fields(pathlib.Path(sys.argv[1]).resolve())
