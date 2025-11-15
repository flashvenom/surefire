' ---------------------------------------
' - WrapXML v1.3.2
' - by John Doe / flashvenom.com
' ---------------------------------------
'
' DESCRIPTION:
'   Wraps content control tags (from an AMS template editor) in XML tags so you can create XML docs from extracted data using Word Doc templates
'
' REFERNCES REQUIRED:
'   Microsoft Word 16.0 Object Library
'   Microsoft Offce 16.0 Object Library
'   Microsoft XML 6.0
'
'-----------------------------------------

Sub WrapContentControlsWithXMLTags()
    Dim myDoc As Document
    Dim cc As ContentControl
    Dim xmlTagName As String
    Dim i As Long
    Dim ccStart As Long
    Dim ccEnd As Long
    Dim rngBefore As Range
    Dim rngAfter As Range
    Dim insertSuccess As Boolean
    Dim docRange As Range

    Set myDoc = Application.ActiveDocument

    ' Loop through each content control from end to start
    For i = myDoc.ContentControls.Count To 1 Step -1
        Set cc = myDoc.ContentControls(i)
        xmlTagName = LCase(cc.Range.text)

        ccStart = cc.Range.Start - 1
        ccEnd = cc.Range.End + 1

        ' Debugging: Output content control info
        Debug.Print "Processing Content Control #" & i
        Debug.Print "Tag: " & xmlTagName
        Debug.Print "Range Start: " & ccStart
        Debug.Print "Range End: " & ccEnd


        ' Check if we can insert after the content control
        If CanInsertAtPosition(ccEnd, myDoc) Then
            ' Insert the closing tag after the content control
            Set rngAfter = myDoc.Range(Start:=ccEnd, End:=ccEnd)
            rngAfter.InsertAfter "</" & xmlTagName & ">"
            Debug.Print "Inserted CLOSING tag after content control."
        Else
            Debug.Print "Cannot insert after content control at position " & ccEnd
        End If
        
        ' Check if we can insert before the content control
        If CanInsertAtPosition(ccStart, myDoc) Then
            ' Insert the opening tag before the content control
            Set rngBefore = myDoc.Range(Start:=ccStart, End:=ccStart)
            rngBefore.InsertBefore "<" & xmlTagName & ">"
            Debug.Print "Inserted opening tag before content control."
        Else
            Debug.Print "Cannot insert before content control at position " & ccStart
        End If

    Next i
    
    Call RemoveSpecificTags(myDoc)
End Sub
Function CanInsertAtPosition(pos As Long, doc As Document) As Boolean
    On Error Resume Next
    Dim rngTest As Range
    Set rngTest = doc.Range(Start:=pos, End:=pos)
    rngTest.Collapse Direction:=wdCollapseStart
    rngTest.InsertBefore ""  ' Try to insert an empty string
    If Err.Number = 0 Then
        CanInsertAtPosition = True
        rngTest.Delete  ' Clean up the test insertion
    Else
        CanInsertAtPosition = False
        Err.Clear
    End If
    On Error GoTo 0
End Function

Function SanitizeXMLTagName(tagName As String) As String
    Dim sanitized As String
    Dim i As Long
    Dim ch As String
    
    ' Remove leading and trailing whitespace
    tagName = Trim(tagName)
    
    ' Initialize sanitized tag name
    sanitized = ""
    
    ' XML tag names must start with a letter or underscore
    If Len(tagName) = 0 Then
        sanitized = "Element"
    Else
        ch = Left(tagName, 1)
        If ch Like "[A-Za-z_]" Then
            sanitized = ch
        Else
            ' If the first character is invalid, replace it with 'Element'
            sanitized = "Element"
        End If
    End If
    
    ' Process the rest of the characters
    For i = 2 To Len(tagName)
        ch = Mid(tagName, i, 1)
        If ch Like "[A-Za-z0-9._-]" Then
            sanitized = sanitized & ch
        Else
            ' Replace invalid characters with underscore
            sanitized = sanitized & "_"
        End If
    Next i
    
    ' Ensure tag name is not empty
    If Len(sanitized) = 0 Then
        sanitized = "Element"
    End If
    
    SanitizeXMLTagName = sanitized
End Function

Sub GetContentControlsInfo()
    Dim myDoc As Document
    Dim cc As ContentControl
    Dim ccInfo As String
    
    Set myDoc = Application.ActiveDocument
    
    ' Loop through each content control in the active document
    For Each cc In myDoc.ContentControls
        ccInfo = ccInfo & "Title: " & cc.Title & vbCrLf
        ccInfo = ccInfo & "Tag: " & cc.tag & vbCrLf
        ccInfo = ccInfo & "Type: " & GetContentControlType(cc.Type) & vbCrLf
        ccInfo = ccInfo & "Placeholder Text: " & cc.PlaceholderText & vbCrLf
        ccInfo = ccInfo & "Current Value: " & cc.Range.text & vbCrLf
        ccInfo = ccInfo & "-----------------------------" & vbCrLf
    Next cc
    
    ' Display the information in a message box
    MsgBox ccInfo, vbOKOnly, "Content Controls Information"
End Sub

Function GetContentControlType(ccType As Long) As String
    Select Case ccType
        Case wdContentControlRichText
            GetContentControlType = "Rich Text"
        Case wdContentControlText
            GetContentControlType = "Plain Text"
        Case wdContentControlPicture
            GetContentControlType = "Picture"
        Case wdContentControlComboBox
            GetContentControlType = "Combo Box"
        Case wdContentControlDropdownList
            GetContentControlType = "Dropdown List"
        Case wdContentControlBuildingBlockGallery
            GetContentControlType = "Building Block Gallery"
        Case wdContentControlDate
            GetContentControlType = "Date Picker"
        Case wdContentControlGroup
            GetContentControlType = "Group"
        Case wdContentControlCheckBox
            GetContentControlType = "Check Box"
        Case Else
            GetContentControlType = "Unknown"
    End Select
End Function

Sub RemoveSpecificTags(doc As Document)
    Dim tagsToRemove As Variant
    Dim tag As Variant
    tagsToRemove = Array("<beginrep>", "</beginrep>", "<endrep>", "</endrep>")
    For Each tag In tagsToRemove
        With doc.Content.Find
            .ClearFormatting
            .Replacement.ClearFormatting
            .text = tag
            .Replacement.text = ""
            .Forward = True
            .Wrap = wdFindContinue
            .Format = False
            .MatchCase = False
            .MatchWholeWord = False
            .MatchWildcards = False
            .MatchSoundsLike = False
            .MatchAllWordForms = False
            .Execute Replace:=wdReplaceAll
        End With
    Next tag
End Sub



