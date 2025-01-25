' ---------------------------------------
' - CLEAN ACORD XML v0.21
' - by John Doe / flashvenom.com
' ---------------------------------------
'
' DESCRIPTION:
'   Takes XML data exported from custom doc template in Epic and cleans it all up
'
' REFERNCES REQUIRED:
'   Microsoft Word 16.0 Object Library
'   Microsoft Offce 16.0 Object Library
'   Microsoft XML 6.0
'
'-----------------------------------------

Sub CleanAndReplaceDocumentText()
    On Error GoTo ErrorHandler

    Dim myDoc As Document
    Dim docText As String
    Dim cleanedText As String
    
    ' Initialize the active document
    Set myDoc = Application.ActiveDocument
    
    ' Get the entire document content as plain text
    docText = myDoc.Content.text

    ' Clean up the text by removing unwanted parts
    cleanedText = CleanUpText(docText)

    ' Replace the document's entire content with the cleaned text
    myDoc.Content.text = cleanedText

    ' Select all text in the document
    myDoc.Content.Select

    ' Copy the selected text to the clipboard using the function from clipboard.bas
    CopySelectionToClipboard

    Exit Sub

ErrorHandler:
    MsgBox "An error occurred: " & Err.Description, vbCritical
End Sub

Function CleanUpText(text As String) As String
    Dim tempText As String
    Dim regex As Object
    
    ' Initialize regex
    Set regex = CreateObject("VBScript.RegExp")
    regex.Global = True

    ' Remove empty XML-like tags (e.g., <tag></tag>)
    regex.Pattern = "<[^/>]+></[^>]+>"
    tempText = regex.Replace(text, "")

    ' Remove <forms> tags and their content
    regex.Pattern = "<forms>.*?</forms>"
    tempText = regex.Replace(tempText, "")

    ' Remove line breaks and carriage returns for a single condensed line
    tempText = Replace(tempText, vbCr, "")
    tempText = Replace(tempText, vbLf, "")
    tempText = Replace(tempText, vbTab, "")

    ' Remove multiple spaces and condense to a single space
    regex.Pattern = "\s+"
    tempText = regex.Replace(tempText, " ")

    ' Additional cleanup patterns if needed
    regex.Pattern = "<[^/>]+></[^>]+>"
    tempText = regex.Replace(tempText, "")

    regex.Pattern = "<[^>]+>(No|Yes)</[^>]+>"
    tempText = regex.Replace(tempText, "")

    ' Return cleaned text
    CleanUpText = Trim(tempText)
End Function
