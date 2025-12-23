Sub UnlockAllContentControls()
    Dim cc As ContentControl
    For Each cc In ActiveDocument.ContentControls
        cc.LockContentControl = False
        cc.LockContents = False
    Next
    MsgBox "All content controls unlocked!"
End Sub

