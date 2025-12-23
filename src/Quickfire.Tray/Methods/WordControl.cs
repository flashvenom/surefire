using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Word;

namespace Quickfire.Tray
{
    public static class WordControl
    {
        public static void PerformWordFunction(string emberFunction, List<string> parameters)
        {
            SystemControl.Log($"[WordControl] PerformWordFunction called with function: '{emberFunction}'");
            SystemControl.Log($"[WordControl] Parameters count: {parameters?.Count ?? 0}");
            if (parameters != null && parameters.Count > 0)
            {
                SystemControl.Log($"[WordControl] Parameters: [{string.Join(", ", parameters)}]");
            }

            switch (emberFunction)
            {
                case "GetWordDocContents":
                    SystemControl.Log($"[WordControl] Routing to GetWordDocContents method");
                    GetWordDocContents(parameters);
                    break;
                default:
                    SystemControl.Log($"[WordControl] ERROR: Unknown ember word function: {emberFunction}");
                    SystemControl.Log($"[WordControl] Available functions: GetWordDocContents");
                    break;
            }
        }

        public static void GetWordDocContents(List<string> parameters)
        {
            SystemControl.Log($"[WordControl] ========== GetWordDocContents START ==========");
            SystemControl.Log($"[WordControl] Method called with {parameters?.Count ?? 0} parameters");
            
            try
            {
                SystemControl.Log($"[WordControl] Step 1: Attempting to connect to Word application...");

                // Try to get the active Word application
                Application wordApp = null;
                try
                {
                    SystemControl.Log($"[WordControl] Calling Marshal.GetActiveObject('Word.Application')...");
                    wordApp = (Application)Marshal.GetActiveObject("Word.Application");
                    SystemControl.Log($"[WordControl] SUCCESS: Connected to Word application");
                    SystemControl.Log($"[WordControl] Word Version: {wordApp.Version}");
                    SystemControl.Log($"[WordControl] Word Visible: {wordApp.Visible}");
                    
                    // Run diagnostics to get full picture
                    LogWordDiagnostics(wordApp);
                }
                catch (COMException comEx)
                {
                    SystemControl.Log($"[WordControl] COMException while connecting to Word: {comEx.Message}");
                    SystemControl.Log($"[WordControl] COMException HRESULT: 0x{comEx.HResult:X8}");
                    SystemControl.Log($"[WordControl] This usually means Word is not running");
                    SystemControl.Log($"[WordControl] Sending error response...");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { "ERROR: No active Word application found" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Word not running) ==========");
                    return;
                }
                catch (System.Exception ex)
                {
                    SystemControl.Log($"[WordControl] Unexpected exception while connecting to Word: {ex.Message}");
                    SystemControl.Log($"[WordControl] Exception type: {ex.GetType().Name}");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { $"ERROR: Failed to connect to Word: {ex.Message}" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Connection failed) ==========");
                    return;
                }

                if (wordApp == null)
                {
                    SystemControl.Log($"[WordControl] ERROR: Word application object is null after connection attempt");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { "ERROR: Word application is null" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Word app null) ==========");
                    return;
                }

                SystemControl.Log($"[WordControl] Step 2: Attempting to get active document...");
                
                // Get the active document
                Document activeDoc = null;
                try
                {
                    SystemControl.Log($"[WordControl] Checking for active document...");
                    
                    // First check if there are any documents open
                    int documentCount = wordApp.Documents.Count;
                    SystemControl.Log($"[WordControl] Total documents open in Word: {documentCount}");
                    
                    if (documentCount == 0)
                    {
                        SystemControl.Log($"[WordControl] No documents are open in Word");
                        SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { "ERROR: No documents are open in Word. Please open a document first." });
                        SystemControl.Log($"[WordControl] ========== GetWordDocContents END (No documents open) ==========");
                        return;
                    }
                    
                    activeDoc = wordApp.ActiveDocument;
                    SystemControl.Log($"[WordControl] SUCCESS: Got active document reference");
                    
                    if (activeDoc != null)
                    {
                        SystemControl.Log($"[WordControl] Document Name: {activeDoc.Name}");
                        SystemControl.Log($"[WordControl] Document Path: {activeDoc.FullName}");
                        SystemControl.Log($"[WordControl] Document Saved: {activeDoc.Saved}");
                        SystemControl.Log($"[WordControl] Document ReadOnly: {activeDoc.ReadOnly}");
                    }
                }
                catch (COMException comDocEx)
                {
                    SystemControl.Log($"[WordControl] COMException while getting active document: {comDocEx.Message}");
                    SystemControl.Log($"[WordControl] COMException HRESULT: 0x{comDocEx.HResult:X8}");
                    
                    // Check for specific error codes
                    if (comDocEx.Message.Contains("no document is open") || comDocEx.Message.Contains("not available"))
                    {
                        SystemControl.Log($"[WordControl] Word is running but no document is active");
                        SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { "ERROR: Word is running but no document is open or active. Please open a document and make sure it's the active window." });
                    }
                    else
                    {
                        SystemControl.Log($"[WordControl] Unexpected COM error accessing document");
                        SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { $"ERROR: Cannot access Word document: {comDocEx.Message}" });
                    }
                    
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Document access COM error) ==========");
                    return;
                }
                catch (System.Exception docEx)
                {
                    SystemControl.Log($"[WordControl] Exception while getting active document: {docEx.Message}");
                    SystemControl.Log($"[WordControl] Exception type: {docEx.GetType().Name}");
                    SystemControl.Log($"[WordControl] This might mean no document is open or active");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { $"ERROR: Failed to access active document: {docEx.Message}" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Document access failed) ==========");
                    return;
                }
                
                if (activeDoc == null)
                {
                    SystemControl.Log($"[WordControl] ERROR: No active Word document found");
                    SystemControl.Log($"[WordControl] User needs to open a document in Word");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { "ERROR: No active Word document found" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (No active doc) ==========");
                    return;
                }

                SystemControl.Log($"[WordControl] Step 3: Extracting document text content...");

                // Get the text content (plain text only)
                string documentText = "";
                try
                {
                    documentText = activeDoc.Content.Text;
                    SystemControl.Log($"[WordControl] SUCCESS: Extracted document text");
                    SystemControl.Log($"[WordControl] Raw text length: {documentText?.Length ?? 0} characters");
                    
                    if (documentText != null && documentText.Length > 0)
                    {
                        // Log first 100 characters for debugging (safely)
                        string preview = documentText.Length > 100 ? 
                            documentText.Substring(0, 100) + "..." : 
                            documentText;
                        SystemControl.Log($"[WordControl] Text preview: '{preview.Replace('\r', ' ').Replace('\n', ' ')}'");
                        
                        // Count lines and words for additional info
                        int lineCount = documentText.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        int wordCount = documentText.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
                        SystemControl.Log($"[WordControl] Document stats - Lines: {lineCount}, Words: {wordCount}");
                    }
                }
                catch (System.Exception textEx)
                {
                    SystemControl.Log($"[WordControl] Exception while extracting text: {textEx.Message}");
                    SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { $"ERROR: Failed to extract document text: {textEx.Message}" });
                    SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Text extraction failed) ==========");
                    return;
                }
                
                if (string.IsNullOrEmpty(documentText))
                {
                    SystemControl.Log($"[WordControl] WARNING: Document appears to be empty or contains no extractable text");
                    documentText = "";
                }

                SystemControl.Log($"[WordControl] Step 4: Sending response via SignalR...");
                SystemControl.Log($"[WordControl] Final text length: {documentText.Length} characters");

                // Send the document contents back via SignalR
                SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { documentText });
                SystemControl.Log($"[WordControl] SUCCESS: Document contents sent via SignalR");
                SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Success) ==========");
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"[WordControl] ========== UNEXPECTED ERROR ==========");
                SystemControl.Log($"[WordControl] Exception Type: {ex.GetType().Name}");
                SystemControl.Log($"[WordControl] Exception Message: {ex.Message}");
                SystemControl.Log($"[WordControl] Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    SystemControl.Log($"[WordControl] Inner Exception: {ex.InnerException.Message}");
                }
                
                SystemControl.Log($"[WordControl] Sending error response...");
                SystemTray.SendEmberResponse("GetWordDocContents", new List<string> { $"ERROR: {ex.Message}" });
                SystemControl.Log($"[WordControl] ========== GetWordDocContents END (Exception) ==========");
            }
        }

        private static void LogWordDiagnostics(Application wordApp)
        {
            try
            {
                SystemControl.Log($"[WordControl] ===== Word Diagnostics =====");
                SystemControl.Log($"[WordControl] Word Application State:");
                SystemControl.Log($"[WordControl]   Version: {wordApp.Version}");
                SystemControl.Log($"[WordControl]   Visible: {wordApp.Visible}");
                SystemControl.Log($"[WordControl]   Documents Count: {wordApp.Documents.Count}");
                
                if (wordApp.Documents.Count > 0)
                {
                    SystemControl.Log($"[WordControl] Available Documents:");
                    for (int i = 1; i <= wordApp.Documents.Count; i++)
                    {
                        var doc = wordApp.Documents[i];
                        SystemControl.Log($"[WordControl]   [{i}] Name: '{doc.Name}', Path: '{doc.FullName}', Saved: {doc.Saved}");
                    }
                    
                    try
                    {
                        var activeDoc = wordApp.ActiveDocument;
                        SystemControl.Log($"[WordControl] Active Document: '{activeDoc.Name}'");
                    }
                    catch (System.Exception ex)
                    {
                        SystemControl.Log($"[WordControl] Cannot get active document: {ex.Message}");
                    }
                }
                else
                {
                    SystemControl.Log($"[WordControl] No documents are currently open");
                }
                
                SystemControl.Log($"[WordControl] ===== End Diagnostics =====");
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"[WordControl] Error during diagnostics: {ex.Message}");
            }
        }
    }
} 
