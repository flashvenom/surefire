using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Outlook;

namespace Quickfire.Tray
{
    public static class OutlookControl
    {
        public static void PerformOutlookFunction(string emberFunction, List<string> parameters)
        {
            switch (emberFunction)
            {
                case "OutlookSearch_EmailStrictToFrom":
                    PerformOutlookSearch_EmailStrictToFrom(parameters);
                    break;
                case "OutlookSearch_EmailBroad":
                    PerformOutlookSearch_EmailBroad(parameters);
                    break;
                case "OutlookSearch_Policy":
                    PerformOutlookSearch_Policy(parameters);
                    break;
                case "OutlookSearch_SmartSearch":
                    PerformOutlookSearch_SmartSearch(parameters);
                    break;
                case "OutlookSearch_Carrier":
                    PerformOutlookSearch_Carrier(parameters);
                    break;
                case "OutlookEmail_CreateNew":
                    CreateNewEmail(parameters);
                    break;
                default:
                    SystemControl.Log($"Unknown ember function: {emberFunction}");
                    break;
            }
        }

        public static void PerformOutlookSearch_EmailStrictToFrom(List<string> emailAddresses)
        {
            try
            {
                SystemControl.Log($"Performing Outlook email strict search for: {string.Join(", ", emailAddresses)}");

                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

                Explorer explorer = outlookApp.ActiveExplorer();

                if (explorer != null)
                {
                    string searchQuery = string.Join(" OR ", emailAddresses.Select(email => $"(to:\"{email}\" OR from:\"{email}\")"));
                    explorer.Search(searchQuery, OlSearchScope.olSearchScopeAllOutlookItems);

                    // Bring the Outlook window to the front
                    SystemControl.BringToFront("rctrl_renwnd32");
                    SystemControl.Log("Search triggered and made Outlook window active");
                }
                else
                {
                    SystemControl.Log("Outlook is not running or no active Explorer window was found.");
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error: {ex.Message}");
            }
        }

        public static void PerformOutlookSearch_EmailBroad(List<string> emailAddresses)
        {
            try
            {
                SystemControl.Log($"Performing Outlook email broad search for: {string.Join(", ", emailAddresses)}");

                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

                Explorer explorer = outlookApp.ActiveExplorer();

                if (explorer != null)
                {
                    string searchQuery = string.Join(" OR ", emailAddresses.Select(email =>
                        $"(to:\"{email}\" OR from:\"{email}\" OR cc:\"{email}\" OR bcc:\"{email}\" OR body:\"{email}\")"));
                    explorer.Search(searchQuery, OlSearchScope.olSearchScopeAllOutlookItems);

                    // Bring the Outlook window to the front
                    SystemControl.BringToFront("rctrl_renwnd32");
                    SystemControl.Log("Broad search triggered and made Outlook window active");
                }
                else
                {
                    SystemControl.Log("Outlook is not running or no active Explorer window was found.");
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error: {ex.Message}");
            }
        }

        public static void PerformOutlookSearch_Policy(List<string> policies)
        {
            try
            {
                SystemControl.Log($"Performing Outlook policy search for: {string.Join(", ", policies)}");

                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

                Explorer explorer = outlookApp.ActiveExplorer();

                if (explorer != null)
                {
                    string searchQuery = string.Join(" OR ", policies.Select(policy => $"(subject:\"{policy}\" OR body:\"{policy}\")"));
                    explorer.Search(searchQuery, OlSearchScope.olSearchScopeAllOutlookItems);

                    // Bring the Outlook window to the front
                    SystemControl.BringToFront("rctrl_renwnd32");
                    SystemControl.Log("Policy search triggered and made Outlook window active");
                }
                else
                {
                    SystemControl.Log("Outlook is not running or no active Explorer window was found.");
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error: {ex.Message}");
            }
        }

        public static void PerformOutlookSearch_SmartSearch(List<string> nameVariations)
        {
            try
            {
                SystemControl.Log($"Performing Outlook Smart (Name) search for: {string.Join(", ", nameVariations)}");

                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

                Explorer explorer = outlookApp.ActiveExplorer();

                if (explorer != null)
                {
                    string searchQuery = string.Join(" OR ", nameVariations.Select(name => $"(subject:\"{name}\" OR body:\"{name}\")"));
                    explorer.Search(searchQuery, OlSearchScope.olSearchScopeAllOutlookItems);

                    // Bring the Outlook window to the front
                    SystemControl.BringToFront("rctrl_renwnd32");
                    SystemControl.Log("Smart search triggered and made Outlook window active");
                }
                else
                {
                    SystemControl.Log("Outlook is not running or no active Explorer window was found.");
                }
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error: {ex.Message}");
            }
        }

        public static void PerformOutlookSearch_Carrier(List<string> parameters)
        {
            SystemControl.Log("PerformOutlookSearch_Carrier method called.");
        }

        public static void CreateNewEmail(List<string> parameters)
        {
            try
            {
                if (parameters == null || parameters.Count < 3)
                {
                    SystemControl.Log("Error: Insufficient parameters for creating new email. Required: To, Subject, Body");
                    return;
                }

                SystemControl.Log("Creating new email...");

                Microsoft.Office.Interop.Outlook.Application outlookApp = new Microsoft.Office.Interop.Outlook.Application();
                MailItem mailItem = (MailItem)outlookApp.CreateItem(OlItemType.olMailItem);

                // Set the To field
                mailItem.To = parameters[0];

                // Set the Subject
                mailItem.Subject = parameters[1];

                // Set the Body with HTML content
                mailItem.HTMLBody = parameters[2];

                // Display the email
                mailItem.Display(false);

                // Bring Outlook window to front
                SystemControl.BringToFront("rctrl_renwnd32");
                
                SystemControl.Log("New email created and displayed.");
            }
            catch (System.Exception ex)
            {
                SystemControl.Log($"Error creating new email: {ex.Message}");
            }
        }
    }
}
