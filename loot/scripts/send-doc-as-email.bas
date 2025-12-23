' ---------------------------------------
' - Send WordDoc as Email v0.1
' - by John Doe / flashvenom.com
' ---------------------------------------
'
' DESCRIPTION:
'   A macro that takes a Word doc, and uses Outlook interop for turning the doc into an email while doing some var merging
'
' REFERNCES REQUIRED:
'   Microsoft XML, v6.0
'   Microsoft Outlook 16.0 Object Library
'
'-----------------------------------------

Sub SendDocAsMail()
Dim variables As clsVariables
Dim outlookApp As Outlook.Application
Dim oItem As Outlook.MailItem
Dim wdEditor As Document
Dim paragraphCount As Integer

Set variables = New clsVariables
variables.ParseDocument Application.ActiveDocument

'Start Outlook if it isn't running
Set outlookApp = GetObject(, "Outlook.Application")
If Err <> 0 Then
    Set outlookApp = CreateObject("Outlook.Application")
End If

'Create a new message
Set oItem = outlookApp.CreateItem(olMailItem)
oItem.Subject = variables.Subject
oItem.To = variables.Email
oItem.Display

'Set the WordEditor
Set wdEditor = oItem.GetInspector.WordEditor
paragraphCount = Application.ActiveDocument.Paragraphs.Count
Application.ActiveDocument.Range(, Application.ActiveDocument.Paragraphs(paragraphCount - 2).Range.End).Copy 'Copy everything except the last 2 paragraphs
wdEditor.Characters(1).PasteAndFormat wdFormatOriginalFormatting

'Fix the hyperlink
If StrComp(variables.CreateLink, "1") = 0 Then
    wdEditor.Hyperlinks(1).Address = GenerateHyperlink(variables)
End If

'Clean up
Set oItem = Nothing
Set outlookApp = Nothing
Set wdEditor = Nothing

End Sub

Private Function GenerateHyperlink(vars As clsVariables) As String
    GenerateHyperlink = "https://epaypolicy.com/?payer=" + vars.Company + "&emailAddress=" + vars.Email + "&amount=" + CStr(vars.AmountVal) + "&comments=" + Replace(vars.Notes, "#", "%23")
End Function


