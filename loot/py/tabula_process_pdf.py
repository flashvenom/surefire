"""
Uses tabula to extract text and structure from a PDF to a text file for use with AI processing
"""
import sys
import os
import pandas as pd
from datetime import datetime
try:
    import tabula
except ImportError:
    print("Installing required dependencies...")
    os.system("pip install tabula-py jpype1")
    import tabula

try:
    import pdfplumber
except ImportError:
    print("Installing pdfplumber for text extraction...")
    os.system("pip install pdfplumber")
    import pdfplumber

def extract_text_from_pdf(pdf_path):
    """Extract all text from a PDF file"""
    try:
        text = []
        with pdfplumber.open(pdf_path) as pdf:
            for page in pdf.pages:
                text.append(page.extract_text())
        return '\n\n'.join(text)
    except Exception as e:
        print(f"Error extracting text: {str(e)}")
        return None

def extract_tables_from_pdf(pdf_path):
    """Extract all tables from a PDF file"""
    try:
        # Read all tables from the PDF
        tables = tabula.read_pdf(pdf_path, pages='all', multiple_tables=True)
        return tables
    except Exception as e:
        print(f"Error extracting tables: {str(e)}")
        return None

def save_tables_to_excel(tables, output_path):
    """Save extracted tables to an Excel file with multiple sheets"""
    try:
        with pd.ExcelWriter(output_path, engine='openpyxl') as writer:
            for i, table in enumerate(tables):
                # Clean the table data if needed
                table.fillna('', inplace=True)
                # Save each table to a different sheet
                table.to_excel(writer, sheet_name=f'Table_{i+1}', index=False)
        return True
    except Exception as e:
        print(f"Error saving to Excel: {str(e)}")
        return False

def save_text_to_file(text, output_path):
    """Save extracted text to a file"""
    try:
        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(text)
        return True
    except Exception as e:
        print(f"Error saving text: {str(e)}")
        return False

def main():
    if len(sys.argv) != 2:
        print("Usage: python process_pdf.py <pdf_file>")
        sys.exit(1)

    pdf_path = sys.argv[1]
    if not os.path.exists(pdf_path):
        print(f"Error: File '{pdf_path}' does not exist")
        sys.exit(1)

    # Create output directory structure using script location as reference
    script_dir = os.path.dirname(os.path.abspath(__file__))  # Get the script's directory
    parent_dir = os.path.dirname(script_dir)  # Go up one level from 'tabula'
    output_dir = os.path.join(parent_dir, "output")
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    output_folder = os.path.join(output_dir, timestamp)
    
    # Create directories if they don't exist
    os.makedirs(output_folder, exist_ok=True)
    
    print(f"Processing {pdf_path}...")
    print(f"Output will be saved to: {output_folder}")
    
    # First try to extract tables
    tables = extract_tables_from_pdf(pdf_path)

    if tables and len(tables) > 0:
        print(f"Found {len(tables)} tables!")
        
        # Save to Excel
        filename = os.path.basename(pdf_path).replace('.pdf', '.xlsx')
        excel_path = os.path.join(output_folder, filename)
        if save_tables_to_excel(tables, excel_path):
            print(f"Successfully saved tables to {excel_path}")
        else:
            print("Failed to save Excel file")
    else:
        print("No tables found, extracting text instead...")
        text = extract_text_from_pdf(pdf_path)
        if text:
            filename = os.path.basename(pdf_path).replace('.pdf', '.txt')
            text_path = os.path.join(output_folder, filename)
            if save_text_to_file(text, text_path):
                print(f"Successfully saved text to {text_path}")
            else:
                print("Failed to save text file")
        else:
            print("Failed to extract text from PDF!")

if __name__ == "__main__":
    main() 