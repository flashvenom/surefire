import PyPDF2

# Input and output file names
input_pdf = "acord.pdf"
fieldnames_file = "pdf-fieldnames.txt"
duplicatefields_file = "pdf-duplicatefields.txt"
propertyissues_file = "pdf-propertyissues.txt"
metadata_file = "pdf-metadata.txt"
security_file = "pdf-security.txt"
other_file = "pdf-other.txt"

# Read the PDF with PyPDF2
reader = PyPDF2.PdfReader(input_pdf)

# Data storage
field_names = []
duplicate_fields = []
property_issues = []

# Extract Metadata
def extract_metadata(reader):
    with open(metadata_file, 'w') as f:
        metadata = reader.metadata
        f.write("PDF Metadata:\n")
        for key, value in metadata.items():
            f.write(f"{key}: {value}\n")

# Extract Field Names and Check for Duplicates
def extract_field_names(reader):
    field_name_set = set()
    for page_number, page in enumerate(reader.pages):
        if "/Annots" in page:
            for annot in page["/Annots"]:
                field = annot.get_object()
                if "/T" in field:
                    field_name = field["/T"]
                    field_names.append(field_name)
                    if field_name in field_name_set:
                        duplicate_fields.append(field_name)
                    else:
                        field_name_set.add(field_name)

    # Write all field names to the text file
    with open(fieldnames_file, 'w') as f:
        f.write("All Form Field Names:\n")
        for field in sorted(field_names):
            f.write(f"{field}\n")

    # Write duplicate field names to the text file
    with open(duplicatefields_file, 'w') as f:
        f.write("Duplicate Form Field Names:\n")
        for field in duplicate_fields:
            f.write(f"{field}\n")

# Check for Field Properties (Required or Read-Only)
def check_field_properties(reader):
    for page_number, page in enumerate(reader.pages):
        if "/Annots" in page:
            for annot in page["/Annots"]:
                field = annot.get_object()
                if "/T" in field:
                    field_name = field["/T"]
                    required = field.get("/Ff", 0) & 0x2 != 0
                    readonly = field.get("/Ff", 0) & 0x1 != 0
                    if required or readonly:
                        property_issues.append(f"Field: {field_name} | Required: {required} | ReadOnly: {readonly}")

    # Write property issues to the text file
    with open(propertyissues_file, 'w') as f:
        f.write("Fields with Required or Read-Only Properties:\n")
        for issue in property_issues:
            f.write(f"{issue}\n")

# Extract Security and Permissions Information
def extract_security_info(reader):
    with open(security_file, 'w') as f:
        f.write("PDF Security Information:\n")
        f.write(f"Is Encrypted: {reader.is_encrypted}\n")
        
        if reader.is_encrypted:
            try:
                # If the PDF is encrypted, try decrypting it (assuming no password required)
                reader.decrypt('')
                f.write("PDF decrypted successfully.\n")
            except Exception as e:
                f.write(f"Failed to decrypt PDF: {e}\n")
        else:
            # Decode permissions
            try:
                permission_bits = reader.get_page(0).get('/P', None)
                if permission_bits is not None:
                    permissions = PyPDF2.generic.decode_permissions(permission_bits)
                    f.write(f"Permissions: {permissions}\n")
                else:
                    f.write("No permissions found.\n")
            except Exception as e:
                f.write(f"Error retrieving permissions: {e}\n")

# Additional checks and settings
def extract_other_info(reader):
    with open(other_file, 'w') as f:
        f.write("Other File Information:\n")
        try:
            num_pages = len(reader.pages)
            f.write(f"Number of Pages: {num_pages}\n")
        except Exception as e:
            f.write(f"Error getting number of pages: {e}\n")

# Process the PDF
extract_metadata(reader)
extract_field_names(reader)
check_field_properties(reader)
extract_security_info(reader)
extract_other_info(reader)

print("Finished processing the PDF and exporting the required files.")
