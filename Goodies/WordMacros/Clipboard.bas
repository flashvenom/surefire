' ---------------------------------------
' - Clipboard Helper v0.1
' - by John Doe / flashvenom.com
' ---------------------------------------
'
' DESCRIPTION:
'   Used in other methods to do stuff with the clipboard
'
'-----------------------------------------

Private Declare PtrSafe Function OpenClipboard Lib "user32" (ByVal hwnd As LongPtr) As Long
Private Declare PtrSafe Function EmptyClipboard Lib "user32" () As Long
Private Declare PtrSafe Function CloseClipboard Lib "user32" () As Long
Private Declare PtrSafe Function SetClipboardData Lib "user32" (ByVal wFormat As Long, ByVal hMem As LongPtr) As LongPtr
Private Declare PtrSafe Function GlobalAlloc Lib "kernel32" (ByVal wFlags As Long, ByVal dwBytes As LongPtr) As LongPtr
Private Declare PtrSafe Function GlobalLock Lib "kernel32" (ByVal hMem As LongPtr) As LongPtr
Private Declare PtrSafe Function GlobalUnlock Lib "kernel32" (ByVal hMem As LongPtr) As Long
Private Declare PtrSafe Sub RtlMoveMemory Lib "kernel32" (ByVal dest As LongPtr, ByVal src As Any, ByVal Length As LongPtr)

Const CF_TEXT As Long = 1
Const GMEM_MOVEABLE As Long = &H2

Sub CopySelectionToClipboard()
    Dim selectedText As String
    Dim hGlobalMemory As LongPtr
    Dim lpGlobalMemory As LongPtr

    ' Check if text is selected
    If Selection.Type <> wdNoSelection Then
        selectedText = Selection.text
    Else
        MsgBox "No text selected."
        Exit Sub
    End If

    ' Open the clipboard and empty it
    OpenClipboard 0
    EmptyClipboard

    ' Allocate global memory for the text
    hGlobalMemory = GlobalAlloc(GMEM_MOVEABLE, Len(selectedText) + 1)
    lpGlobalMemory = GlobalLock(hGlobalMemory)

    ' Copy the selected text into the allocated memory
    If lpGlobalMemory <> 0 Then
        RtlMoveMemory ByVal lpGlobalMemory, ByVal selectedText, Len(selectedText) + 1
        GlobalUnlock hGlobalMemory
        SetClipboardData CF_TEXT, hGlobalMemory
    End If

    ' Close the clipboard
    CloseClipboard
End Sub

