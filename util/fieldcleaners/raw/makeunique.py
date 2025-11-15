import PyPDF2

# Input and output PDF filenames
input_pdf = "acord.pdf"
output_pdf = "renamed_output.pdf"

def rename_all_form_fields(reader, writer):
    field_counter = 1  # Counter for unique field names

    for page_number, page in enumerate(reader.pages):
        # Check for annotations (form fields) on the page
        if "/Annots" in page:
            annotations = page["/Annots"]
            for annot_ref in annotations:
                annot = annot_ref.get_object()  # Resolve annotation object

                # Check if the annotation has a field name ("/T")
                if "/T" in annot:
                    original_name = annot["/T"]

                    # Generate a new unique field name
                    new_name = f"Field{field_counter}"
                    print(f"Page {page_number + 1}: Renaming field '{original_name}' to '{new_name}'")

                    # Update the field name in the annotation
                    annot.update({
                        PyPDF2.generic.NameObject("/T"): PyPDF2.generic.createStringObject(new_name)
                    })

                    # Increment the counter for the next field
                    field_counter += 1

        # Add the (possibly modified) page to the writer
        writer.add_page(page)

# Read the PDF with PyPDF2
reader = PyPDF2.PdfReader(input_pdf)
writer = PyPDF2.PdfWriter()

# Execute the renaming function
rename_all_form_fields(reader, writer)

# Save the modified PDF with renamed fields
with open(output_pdf, "wb") as f_out:
    writer.write(f_out)

print(f"\nFinished renaming fields. Output saved as: {output_pdf}")
