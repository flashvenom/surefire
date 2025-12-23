"""
Utility to rename form fields from a CSV file. Helpful for programattically cleaning up form-field enabled PDFs
"""
import sys
import csv
import lxml.etree as ET
import os
import shutil
from datetime import datetime
import glob

def remove_empty_nodes(element):
    """
    Recursively remove child nodes that are "obviously empty":
      - No children,
      - No attributes, and
      - Text is either missing or only whitespace.
    """
    # Process children first (postorder traversal)
    for child in list(element):
        remove_empty_nodes(child)
        if len(child) == 0 and not child.attrib and (child.text is None or child.text.strip() == ""):
            element.remove(child)

def load_attribute_mapping(csv_file):
    """
    Load the CSV file and create a mapping of node names to their attributes.
    Returns a dictionary where keys are node names and values are tuples of (desc, type).
    """
    mapping = {}
    try:
        with open(csv_file, 'r', encoding='utf-8') as f:
            reader = csv.reader(f)
            for row in reader:
                if len(row) >= 3:  # Ensure row has at least 3 elements
                    node_name, desc, type_val = row[0], row[1], row[2]
                    mapping[node_name.lower()] = (desc, type_val)
    except Exception as e:
        print(f"Error reading CSV file: {e}")
    return mapping

def add_attributes(element, attribute_mapping):
    """
    Recursively add attributes to nodes based on the mapping.
    """
    # Process current element
    tag_name = element.tag.lower()
    if tag_name in attribute_mapping:
        desc, type_val = attribute_mapping[tag_name]
        element.set('desc', desc)
        element.set('type', type_val)
    
    # Process children
    for child in element:
        add_attributes(child, attribute_mapping)

def process_xml(input_xml, input_csv, output_file):
    try:
        # Load attribute mapping from CSV
        attribute_mapping = load_attribute_mapping(input_csv)
        
        # Read the entire XML file content
        with open(input_xml, "r", encoding="utf-8") as f:
            raw_xml = f.read()
        
        # Remove XML declaration if it exists
        if raw_xml.strip().startswith('<?xml'):
            # Find the end of the declaration
            decl_end = raw_xml.find('?>') + 2
            content = raw_xml[decl_end:].strip()
        else:
            content = raw_xml.strip()
        
        # Always wrap content in root tags, regardless of existing structure
        content = f"<root>{content}</root>"
        
        # Parse the XML
        parser = ET.XMLParser(recover=True)
        xml_root = ET.fromstring(content, parser)
        
        # Remove empty nodes
        remove_empty_nodes(xml_root)
        
        # Add attributes based on CSV mapping
        add_attributes(xml_root, attribute_mapping)
        
        # Serialize the XML
        cleaned_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + ET.tostring(xml_root, pretty_print=True, encoding="unicode")
        
        with open(output_file, "w", encoding="utf-8") as f:
            f.write(cleaned_xml)
        print(f"Enhanced XML saved to {output_file}")
        return True
    except ET.XMLSyntaxError as e:
        print("Error parsing XML:", e)
        return False
    except Exception as e:
        print("Unexpected error:", e)
        return False

if __name__ == "__main__":
    # Get the current directory
    current_dir = os.path.dirname(os.path.abspath(__file__))

    # Find the first XML file in the current directory
    xml_files = glob.glob(os.path.join(current_dir, "*.xml"))
    if not xml_files:
        print("No XML files found in the current directory")
        exit(1)

    input_file = xml_files[0]  # Use the first XML file found
    input_csv = os.path.join(current_dir, "_variables.csv")

    # Create input and output directories if they don't exist
    input_dir = os.path.join(current_dir, "input")
    output_dir = os.path.join(current_dir, "output")
    os.makedirs(input_dir, exist_ok=True)
    os.makedirs(output_dir, exist_ok=True)

    # Generate unique filenames based on current date and time
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    input_copy_name = f"input_{timestamp}.xml"
    output_copy_name = f"output_{timestamp}.xml"

    # Copy input file to input directory
    input_copy_path = os.path.join(input_dir, input_copy_name)
    shutil.copy2(input_file, input_copy_path)

    # Process the XML file
    output_file = os.path.join(current_dir, f"cleaned_{timestamp}.xml")
    if process_xml(input_file, input_csv, output_file):
        # Copy the output file to output directory
        output_copy_path = os.path.join(output_dir, output_copy_name)
        shutil.copy2(output_file, output_copy_path)
        print(f"Input file copied to: {input_copy_path}")
        print(f"Output file copied to: {output_copy_path}")
        
        # Clean up the temporary output file
        os.remove(output_file)
    else:
        print("Failed to process XML file") 