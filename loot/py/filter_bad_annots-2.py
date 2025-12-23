"""
Alternate method of filtering bad annotations from a Form-Field Enabled PDF document so that it can work better with Syncfusion's SfPdfViewer2
"""
import PyPDF2

# Input and output PDF filenames
input_pdf = "acord.pdf"
output_pdf = "filtered_output.pdf"

def retain_text_and_checkboxes(reader, writer):
    for page_number, page in enumerate(reader.pages):
        # Check for annotations (form fields) on the page
        if "/Annots" in page:
            annotations = page["/Annots"]
            filtered_annotations = []
            for annot_ref in annotations:
                annot = annot_ref.get_object()  # Resolve annotation object

                # Check the field type
                if "/FT" in annot:
                    field_type = annot["/FT"]

                    # Keep only text fields or checkboxes
                    if field_type == "/Tx" or field_type == "/Btn":
                        filtered_annotations.append(annot_ref)
                    else:
                        print(f"Page {page_number + 1}: Removing field of type {field_type}")
                else:
                    print(f"Page {page_number + 1}: Annotation without a field type found and removed.")

            # Update the annotations array with filtered annotations
            if filtered_annotations:
                page[PyPDF2.generic.NameObject("/Annots")] = PyPDF2.generic.ArrayObject(filtered_annotations)
            else:
                # Remove annotations if none remain
                if "/Annots" in page:
                    del page["/Annots"]

        # Add the (possibly modified) page to the writer
        writer.add_page(page)

# Read the PDF with PyPDF2
reader = PyPDF2.PdfReader(input_pdf)
writer = PyPDF2.PdfWriter()

# Execute the filtering function
retain_text_and_checkboxes(reader, writer)

# Save the modified PDF with only text fields and checkboxes
with open(output_pdf, "wb") as f_out:
    writer.write(f_out)

print(f"\nFinished processing. Output saved as: {output_pdf}")
