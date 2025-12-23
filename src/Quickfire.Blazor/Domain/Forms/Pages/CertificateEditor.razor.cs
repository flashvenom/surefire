using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quickfire.Blazor.Domain.Clients.Models;
using Quickfire.Blazor.Domain.Clients.Services;
using Quickfire.Blazor.Domain.Forms.Models;
using Quickfire.Blazor.Domain.Forms.Services;
using Quickfire.Blazor.Domain.Policies.Models;
using Quickfire.Blazor.Domain.Policies.Services;
using Quickfire.Blazor.Domain.Shared.Helpers;
using Syncfusion.Blazor.SfPdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace Quickfire.Blazor.Domain.Forms.Pages
{
    public partial class CertificateEditor
    {
    [Parameter]
    public int CertificateId { get; set; }

    private string DocumentPath { get; set; } = "";
    private string ClearPoliciesPath { get; set; } = "";
    private string JsonPath { get; set; } = "";
    private SfPdfViewer2 pdfViewer;
    private Stream stream;
    public int clientId { get; set; } = 0;
    public Client client { get; set; } =  new Client();
    public Certificate certificate { get; set; } = new Certificate();
    public List<Policy> policies { get; set; } = new List<Policy>();
    private bool _policiesVisible = false;
    private bool _attachmentsVisible = false;
    private int _missingFieldCount = 0;
    private List<Policy> selectedpolicies = new List<Policy>();
    [CascadingParameter] public Action<string> UpdateHeader { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            UpdateHeader?.Invoke("Forms");
            DocumentPath = "wwwroot/forms/a25-2016-03.pdf";
            JsonPath = Path.Combine(Environment.WebRootPath, "forms", "_json/a25-2016-03.json");
            ClearPoliciesPath = Path.Combine(Environment.WebRootPath, "forms", "_json/a25-2016-03.json");
            
            client = await ClientService.GetClientByCertificateId(CertificateId);
            certificate = await FormService.GetCertificateByIdAsync(CertificateId);
            policies = await PolicyService.GetCurrentPoliciesByClientIdAsync(client.ClientId);
            clientId = client.ClientId;

            // Validate JSON fields against PDF form on initialization
            await ValidateJsonFields();
            if (_missingFieldCount > 0)
            {
                Logger.LogWarning("Certificate Editor initialized with {Count} missing/renamed PDF form fields. Check logs for details.", _missingFieldCount);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error initializing Certificate Editor for CertificateId: {CertificateId}", CertificateId);
            throw;
        }
    }

    private bool IsPolicySelected(Policy policy)
    {
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("ISIS: Policy " + policy.PolicyNumber + "selected is " + selectedpolicies.Contains(policy));
        return selectedpolicies.Contains(policy);
    }

    public async Task LoadJsonDataIntoPdfAsync()
    {
        Console.WriteLine("Loading DATABASE JSON data into PDF viewer");
        if (certificate != null && !string.IsNullOrEmpty(certificate.JSONData))
        {
            // Convert JSONData to a Stream
            var jsonDataStream = new MemoryStream();
            using (var writer = new StreamWriter(jsonDataStream, leaveOpen: true))
            {
                await writer.WriteAsync(certificate.JSONData);
                await writer.FlushAsync();
                jsonDataStream.Position = 0;
            }

            // Load the JSON data into the PDF viewer
            await pdfViewer.ImportFormFieldsAsync(jsonDataStream, FormFieldDataFormat.Json);
        }
    }

    public void TogglePolicySelection(Microsoft.AspNetCore.Components.ChangeEventArgs e, Policy policy)
    {
        if (!selectedpolicies.Contains(policy))
        {
            selectedpolicies.Add(policy);
        }
        else
        {
            selectedpolicies.Remove(policy);
        }
    }

    public async void ToggleBlockAttachments(Microsoft.AspNetCore.Components.ChangeEventArgs e, Certificate certificate)
    {
        if (certificate.BlockAttachments == false)
        {
            certificate.BlockAttachments = true;
        }
        else
        {
            certificate.BlockAttachments = false;
        }
        await FormService.UpdateCertificate(certificate);
    }

    public async Task ResetForm()
    {
        await HandleLoadEverything(true, false, false, false);
    }

    public async Task DownloadFile(bool print)
    {
        await pdfViewer.DownloadAsync();
    }

    public async Task<string> ExportCertificate(bool? openNewWindow, bool? printNow, bool? attachToEmail, bool? saveToDisk)
    {
        await SaveToDatabase(false);

        byte[] data = await pdfViewer.GetDocumentAsync();
        data = FormService.FlattenPdf(data);
        using (MemoryStream originalPdfStream = new MemoryStream(data))
        {
            PdfLoadedDocument originalDocument = new PdfLoadedDocument(originalPdfStream);

            if (certificate.AttachGLAI == true && certificate.AttachGLAIfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachGLAIfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }

            if (certificate.AttachGLWOS == true && certificate.AttachGLWOSfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachGLWOSfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }

            if (certificate.AttachWCWOS == true && certificate.AttachWCWOSfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachWCWOSfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }


            using (MemoryStream combinedPdfStream = new MemoryStream())
            {
                originalDocument.Save(combinedPdfStream);

                byte[] combinedPdfBytes = combinedPdfStream.ToArray();
                string base64Pdf = Convert.ToBase64String(combinedPdfBytes);


                
                if (printNow == true)
                {
                    await JsInterop.OpenPdfInNewWindowAsync(base64Pdf);
                }

                if (saveToDisk == true)
                {
                    string certname = StringHelper.GenerateCertificateName(client.Name, certificate.HolderName);
                    await JsInterop.DownloadPdfAsync(base64Pdf, certname);
                }

                if (openNewWindow == true)
                {
                    await JsInterop.OpenPdfInNewWindowAsync(base64Pdf);
                }
            }
            originalDocument.Close(true);
        }
        return string.Empty;
    }

    public async Task AttachPages(bool? addAttachments)
    {
        await SaveToDatabase(false);

        byte[] data = await pdfViewer.GetDocumentAsync();
        data = FormService.FlattenPdf(data);
        using (MemoryStream originalPdfStream = new MemoryStream(data))
        {
            PdfLoadedDocument originalDocument = new PdfLoadedDocument(originalPdfStream);

            if (certificate.AttachGLAI == true && certificate.AttachGLAIfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachGLAIfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }

            if (certificate.AttachGLWOS == true && certificate.AttachGLWOSfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachGLWOSfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }

            if (certificate.AttachWCWOS == true && certificate.AttachWCWOSfilename != null)
            {
                string attachmentPath = Path.Combine(Environment.WebRootPath, "uploads", certificate.AttachWCWOSfilename);
                using (FileStream attachmentStream = new FileStream(attachmentPath, FileMode.Open, FileAccess.Read))
                {
                    PdfLoadedDocument attachmentDocument = new PdfLoadedDocument(attachmentStream);
                    originalDocument.ImportPageRange(attachmentDocument, 0, attachmentDocument.Pages.Count - 1);
                    attachmentDocument.Close(true);
                }
            }


            using (MemoryStream combinedPdfStream = new MemoryStream())
            {
                originalDocument.Save(combinedPdfStream);

                byte[] combinedPdfBytes = combinedPdfStream.ToArray();
                string base64Pdf = Convert.ToBase64String(combinedPdfBytes);
                await JsInterop.OpenPdfInNewWindowAsync(base64Pdf);
            }
            originalDocument.Close(true);
        }
    }

    public async void FlattenForm()
    {
        byte[] data = await pdfViewer.GetDocumentAsync();
        byte[] flattenedPdfBytes = FormService.FlattenPdf(data);
        string base64Pdf = Convert.ToBase64String(flattenedPdfBytes);
        await JsInterop.OpenPdfInNewWindowAsync(base64Pdf);
    }

    public async void StoreTempData()
    {
        Console.WriteLine("Storing Temp Data");
        stream = await pdfViewer.ExportFormFieldsAsync(FormFieldDataFormat.Json);

        stream.Position = 0;
        using (var reader = new StreamReader(stream))
        {
            string jsonData = await reader.ReadToEndAsync();
            var jsonObject = JObject.Parse(jsonData);
            certificate.JSONDataTemp = jsonData;
        }
    }

    public async Task SaveToDatabase(bool? exit)
    {
        _attachmentsVisible = false;
        _policiesVisible = false;
        stream = await pdfViewer.ExportFormFieldsAsync(FormFieldDataFormat.Json);

        stream.Position = 0;
        using (var reader = new StreamReader(stream))
        {
            string jsonData = await reader.ReadToEndAsync();
            var jsonObject = JObject.Parse(jsonData);

            string holderName = jsonObject["CertificateHolder_FullName"]?.ToString();
            if (!string.IsNullOrEmpty(holderName))
            {
                certificate.HolderName = holderName;
            }

            string holderdescription = jsonObject["CertificateOfLiabilityInsurance_ACORDForm_RemarkText"]?.ToString();
            if (!string.IsNullOrEmpty(holderdescription))
            {
                certificate.ProjectName = holderName;
            }

            string attachGLAI = jsonObject["CertificateOfInsurance_GeneralLiability_AdditionalInsuredCode"]?.ToString();
            if (!string.IsNullOrEmpty(attachGLAI)) 
            { 
                certificate.AttachGLAI = true;
            }

            string attachGlWOS = jsonObject["Policy_GeneralLiability_SubrogationWaivedCode"]?.ToString();
            if (!string.IsNullOrEmpty(attachGlWOS)) { certificate.AttachGLWOS = true; }

            string attachWCWOS = jsonObject["Policy_WorkersCompensation_SubrogationWaivedCode"]?.ToString();
            if (!string.IsNullOrEmpty(attachWCWOS)) { certificate.AttachWCWOS = true; }

            // Save the JSON data to the certificate's JSONData field
            certificate.JSONData = jsonData;

            // Update the certificate in the database
            await FormService.UpdateCertificate(certificate);
        }

        if(exit == true)
        {
            Navigation.NavigateTo($"/Clients/{clientId}");
        }
    }

    public async Task DuplicateCertificate()
    {
        int newcertid = await FormService.DuplicateCertificateAsync(certificate);
        Navigation.NavigateTo($"/Forms/Certificate/{newcertid}");

    }

    public async Task<JObject> GetViewerJSON()
    {
        stream = await pdfViewer.ExportFormFieldsAsync(FormFieldDataFormat.Json);

        stream.Position = 0;
        using (var reader = new StreamReader(stream))
        {
            string jsonData = await reader.ReadToEndAsync();
            var jsonObject = JObject.Parse(jsonData);
            return jsonObject;
        }
    }

    public async Task<JObject> ClearPoliciesJSON()
    {
        var savedJsonObject = await GetViewerJSON();

        JObject clearPoliciesJsonObject;
        using (var fileStream = new FileStream(ClearPoliciesPath, FileMode.Open, FileAccess.Read))
        {
            using (var streamReader = new StreamReader(fileStream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                clearPoliciesJsonObject = await JObject.LoadAsync(jsonReader);
            }
        }

        foreach (var property in clearPoliciesJsonObject.Properties())
        {
            if (savedJsonObject.ContainsKey(property.Name))
            {
                savedJsonObject[property.Name] = property.Value;
            }
        }

        Console.WriteLine();
        return savedJsonObject;
    }

    public async Task HandleLoadEverything(bool? loadClient = null, bool? loadPolicies = null, bool? loadHolder = null, bool? loadDescription = null)
    {
        try
        {
            _attachmentsVisible = false;
            _policiesVisible = false;
            var jsonObject = await ClearPoliciesJSON();

            // Safely set fields with validation
            SetJsonField(jsonObject, "Form_CompletionDate", DateTime.UtcNow.ToString("MM/dd/yyyy"));
            SetJsonField(jsonObject, "CertificateOfInsurance_CertificateNumberIdentifier", "MSF-24-" + CertificateId);


            if(loadClient == true)
            {
                if (client != null)
                {
                    SetJsonField(jsonObject, "NamedInsured_FullName", client.Name ?? "");
                    
                    if (client.Address != null)
                    {
                        SetJsonField(jsonObject, "NamedInsured_MailingAddress_LineOne", client.Address.AddressLine1 ?? "");
                        
                        if (!string.IsNullOrEmpty(client.Address.AddressLine2))
                        {
                            SetJsonField(jsonObject, "NamedInsured_MailingAddress_LineTwo", client.Address.AddressLine2);
                            SetJsonField(jsonObject, "NamedInsured_MailingAddress_CityName", 
                                $"{client.Address.City}, {client.Address.State} {client.Address.PostalCode}");
                        }
                        else
                        {
                            SetJsonField(jsonObject, "NamedInsured_MailingAddress_LineTwo", 
                                $"{client.Address.City}, {client.Address.State} {client.Address.PostalCode}");
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Client address is null for ClientId: {ClientId}", client.ClientId);
                    }
                }
                else
                {
                    Logger.LogWarning("Client is null when attempting to load client data");
                }
            }



            if(loadPolicies == true)
            {
                string[] letterArray = { "A", "B", "C", "D", "E" };
                int currentposition = 1;

                //General Liability
                var glpolicy = selectedpolicies?.Where(p => p.ProductId == 3).FirstOrDefault();
                if (glpolicy is not null)
                {
                    SetJsonField(jsonObject, "GeneralLiability_CoverageIndicator", "1");

                    //Set Carrier Assignments
                    SetJsonField(jsonObject, "GeneralLiability_InsurerLetterCode", letterArray[currentposition-1]);
                    
                    if (glpolicy.Carrier != null && !string.IsNullOrEmpty(glpolicy.Carrier.CarrierName))
                    {
                        string carrierFieldName = currentposition switch
                        {
                            1 => "Insurer_FullName",
                            2 => "Insurer_FullName_001",
                            3 => "Insurer_FullName_003",
                            4 => "Insurer_FullName_005",
                            5 => "Insurer_FullName_007",
                            _ => null
                        };
                        
                        if (carrierFieldName != null)
                        {
                            SetJsonField(jsonObject, carrierFieldName, glpolicy.Carrier.CarrierName);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("GL Policy carrier is null or has no name for PolicyId: {PolicyId}", glpolicy.PolicyId);
                    }
                    currentposition++;

                    if (!string.IsNullOrEmpty(glpolicy.PolicyNumber))
                        SetJsonField(jsonObject, "Policy_GeneralLiability_PolicyNumberIdentifier", glpolicy.PolicyNumber);

                    if (glpolicy.EffectiveDate != null)
                        SetJsonField(jsonObject, "Policy_GeneralLiability_EffectiveDate", glpolicy.EffectiveDate.ToString("MM/dd/yyyy"));

                    if (glpolicy.ExpirationDate != null)
                        SetJsonField(jsonObject, "Policy_GeneralLiability_ExpirationDate", glpolicy.ExpirationDate.ToString("MM/dd/yyyy"));

                    // Check if GeneralLiabilityCoverage exists before accessing its properties
                    if (glpolicy.GeneralLiabilityCoverage != null)
                    {
                        if (glpolicy.GeneralLiabilityCoverage.EachOccurrence != null)
                            SetJsonField(jsonObject, "GeneralLiability_EachOccurrence_LimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.EachOccurrence.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.DamageToPremises != null)
                            SetJsonField(jsonObject, "GeneralLiability_FireDamageRentedPremises_EachOccurrenceLimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.DamageToPremises.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.MedicalExpenses != null)
                            SetJsonField(jsonObject, "GeneralLiability_MedicalExpense_EachPersonLimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.MedicalExpenses.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.PersonalInjury != null)
                            SetJsonField(jsonObject, "GeneralLiability_PersonalAndAdvertisingInjury_LimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.PersonalInjury.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.GeneralAggregate != null)
                            SetJsonField(jsonObject, "GeneralLiability_GeneralAggregate_LimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.GeneralAggregate.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.ProductsAggregate != null)
                            SetJsonField(jsonObject, "GeneralLiability_ProductsAndCompletedOperations_AggregateLimitAmount", 
                                glpolicy.GeneralLiabilityCoverage.ProductsAggregate.Value.ToString("N0"));

                        if (!string.IsNullOrEmpty(glpolicy.GeneralLiabilityCoverage.AdditionalCoverageName))
                            SetJsonField(jsonObject, "GeneralLiability_OtherCoverageLimitDescription", 
                                glpolicy.GeneralLiabilityCoverage.AdditionalCoverageName);

                        if (glpolicy.GeneralLiabilityCoverage.AdditionalCoverageLimit != null)
                            SetJsonField(jsonObject, "GeneralLiability_OtherCoverageLimitDescription", 
                                glpolicy.GeneralLiabilityCoverage.AdditionalCoverageLimit.Value.ToString("N0"));

                        if (glpolicy.GeneralLiabilityCoverage.ClaimsMade is not null)
                            SetJsonField(jsonObject, "GeneralLiability_ClaimsMadeIndicator", "On");

                        if (glpolicy.GeneralLiabilityCoverage.Occurence is not null)
                            SetJsonField(jsonObject, "GeneralLiability_OccurrenceIndicator", "On");
                        
                        //Add filename to certificate for when/if we export/combine PDFs
                        if (glpolicy.GeneralLiabilityCoverage.AdditionalInsuredAttachment != null)
                        {
                            certificate.AttachGLAIfilename = glpolicy.GeneralLiabilityCoverage.AdditionalInsuredAttachment.OriginalFileName;
                        }

                        //Add filename to certificate for when/if we export/combine PDFs
                        if (glpolicy.GeneralLiabilityCoverage.WaiverOfSubAttachment != null)
                        {
                            certificate.AttachGLWOSfilename = glpolicy.GeneralLiabilityCoverage.WaiverOfSubAttachment.OriginalFileName;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("GeneralLiabilityCoverage is null for GL policy {PolicyNumber}", glpolicy.PolicyNumber ?? "Unknown");
                    }
                }


                //Auto
                var autopolicy = selectedpolicies?.Where(p => p.ProductId == 4).FirstOrDefault();
                if (autopolicy is not null)
                {
                    //Set Carrier Assignments
                    SetJsonField(jsonObject, "Vehicle_InsurerLetterCode", letterArray[currentposition-1]);
                    
                    if (autopolicy.Carrier != null && !string.IsNullOrEmpty(autopolicy.Carrier.CarrierName))
                    {
                        string carrierFieldName = currentposition switch
                        {
                            1 => "Insurer_FullName",
                            2 => "Insurer_FullName_001",
                            3 => "Insurer_FullName_003",
                            4 => "Insurer_FullName_005",
                            5 => "Insurer_FullName_007",
                            _ => null
                        };
                        
                        if (carrierFieldName != null)
                        {
                            SetJsonField(jsonObject, carrierFieldName, autopolicy.Carrier.CarrierName);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Auto Policy carrier is null or has no name for PolicyId: {PolicyId}", autopolicy.PolicyId);
                    }
                    currentposition++;

                    if (!string.IsNullOrEmpty(autopolicy.PolicyNumber))
                        SetJsonField(jsonObject, "Policy_AutomobileLiability_PolicyNumberIdentifier", autopolicy.PolicyNumber);

                    if (autopolicy.EffectiveDate != null)
                        SetJsonField(jsonObject, "Policy_AutomobileLiability_EffectiveDate", autopolicy.EffectiveDate.ToString("MM/dd/yyyy"));

                    if (autopolicy.ExpirationDate != null)
                        SetJsonField(jsonObject, "Policy_AutomobileLiability_ExpirationDate", autopolicy.ExpirationDate.ToString("MM/dd/yyyy"));

                    if (autopolicy.AutoCoverage != null)
                    {
                        if (autopolicy.AutoCoverage.CombinedLimit != null)
                            SetJsonField(jsonObject, "Vehicle_CombinedSingleLimit_EachAccidentAmount", 
                                autopolicy.AutoCoverage.CombinedLimit.Value.ToString("N0"));

                        if (autopolicy.AutoCoverage.BodilyInjuryPerPerson != null)
                            SetJsonField(jsonObject, "Vehicle_BodilyInjury_PerPersonLimitAmount", 
                                autopolicy.AutoCoverage.BodilyInjuryPerPerson.Value.ToString("N0"));

                        if (autopolicy.AutoCoverage.BodilyInjuryPerAccident != null)
                            SetJsonField(jsonObject, "Vehicle_BodilyInjury_PerAccidentLimitAmount", 
                                autopolicy.AutoCoverage.BodilyInjuryPerAccident.Value.ToString("N0"));

                        if (autopolicy.AutoCoverage.PropertyDamage != null)
                            SetJsonField(jsonObject, "Vehicle_PropertyDamage_PerAccidentLimitAmount", 
                                autopolicy.AutoCoverage.PropertyDamage.Value.ToString("N0"));

                        if (autopolicy.AutoCoverage.ForAny is not null)
                            SetJsonField(jsonObject, "Vehicle_AnyAutoIndicator", "On");

                        if (autopolicy.AutoCoverage.ForOwned is not null)
                            SetJsonField(jsonObject, "Vehicle_AllOwnedAutosIndicator", "On");

                        if (autopolicy.AutoCoverage.ForHired is not null)
                            SetJsonField(jsonObject, "Vehicle_HiredAutosIndicator", "On");

                        if (autopolicy.AutoCoverage.ForScheduled is not null)
                            SetJsonField(jsonObject, "Vehicle_ScheduledAutosIndicator", "On");

                        if (autopolicy.AutoCoverage.ForNonOwned is not null)
                            SetJsonField(jsonObject, "Vehicle_NonOwnedAutosIndicator", "On");
                    }
                    else
                    {
                        Logger.LogWarning("AutoCoverage is null for Auto policy {PolicyNumber}", autopolicy.PolicyNumber ?? "Unknown");
                    }
                }

                //Umbrella
                var umbrellapolicy = selectedpolicies?.Where(p => p.ProductId == 7).FirstOrDefault();
                if (umbrellapolicy is not null)
                {
                    //Set Carrier Assignments
                    SetJsonField(jsonObject, "ExcessUmbrella_InsurerLetterCode", letterArray[currentposition-1]);
                    
                    if (umbrellapolicy.Carrier != null && !string.IsNullOrEmpty(umbrellapolicy.Carrier.CarrierName))
                    {
                        string carrierFieldName = currentposition switch
                        {
                            1 => "Insurer_FullName",
                            2 => "Insurer_FullName_001",
                            3 => "Insurer_FullName_003",
                            4 => "Insurer_FullName_005",
                            5 => "Insurer_FullName_007",
                            _ => null
                        };
                        
                        if (carrierFieldName != null)
                        {
                            SetJsonField(jsonObject, carrierFieldName, umbrellapolicy.Carrier.CarrierName);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Umbrella Policy carrier is null or has no name for PolicyId: {PolicyId}", umbrellapolicy.PolicyId);
                    }
                    currentposition++;

                    if (!string.IsNullOrEmpty(umbrellapolicy.PolicyNumber))
                        SetJsonField(jsonObject, "Policy_ExcessLiability_PolicyNumberIdentifier", umbrellapolicy.PolicyNumber);

                    if (umbrellapolicy.EffectiveDate != null)
                        SetJsonField(jsonObject, "Policy_ExcessLiability_EffectiveDate", umbrellapolicy.EffectiveDate.ToString("MM/dd/yyyy"));

                    if (umbrellapolicy.ExpirationDate != null)
                        SetJsonField(jsonObject, "Policy_ExcessLiability_ExpirationDate", umbrellapolicy.ExpirationDate.ToString("MM/dd/yyyy"));

                    if (umbrellapolicy.UmbrellaCoverage != null)
                    {
                        if (umbrellapolicy.UmbrellaCoverage.DeductibleRetentionAmount != null)
                            SetJsonField(jsonObject, "ExcessUmbrella_Umbrella_DeductibleOrRetentionAmount", 
                                umbrellapolicy.UmbrellaCoverage.DeductibleRetentionAmount.Value.ToString("N0"));

                        if (umbrellapolicy.UmbrellaCoverage.EachOccurrence != null)
                            SetJsonField(jsonObject, "ExcessUmbrella_Umbrella_EachOccurrenceAmount", 
                                umbrellapolicy.UmbrellaCoverage.EachOccurrence.Value.ToString("N0"));

                        if (umbrellapolicy.UmbrellaCoverage.GeneralAggregate != null)
                            SetJsonField(jsonObject, "ExcessUmbrella_Umbrella_AggregateAmount", 
                                umbrellapolicy.UmbrellaCoverage.GeneralAggregate.Value.ToString("N0"));

                        if (umbrellapolicy.UmbrellaCoverage.IsUmbrella is not null)
                            SetJsonField(jsonObject, "Policy_PolicyType_UmbrellaIndicator", "On");

                        if (umbrellapolicy.UmbrellaCoverage.IsExcess is not null)
                            SetJsonField(jsonObject, "Policy_PolicyType_ExcessIndicator", "On");

                        if (umbrellapolicy.UmbrellaCoverage.HasDeductible is not null)
                            SetJsonField(jsonObject, "ExcessUmbrella_DeductibleIndicator", "On");

                        if (umbrellapolicy.UmbrellaCoverage.HasRetention is not null)
                            SetJsonField(jsonObject, "ExcessUmbrella_RetentionIndicator", "On");

                        if (umbrellapolicy.UmbrellaCoverage.ClaimsMade is not null)
                            SetJsonField(jsonObject, "ExcessUmbrella_ClaimsMadeIndicator", "On");

                        if (umbrellapolicy.UmbrellaCoverage.Occurrence is not null)
                            SetJsonField(jsonObject, "ExcessUmbrella_OccurrenceIndicator", "On");
                    }
                    else
                    {
                        Logger.LogWarning("UmbrellaCoverage is null for Umbrella policy {PolicyNumber}", umbrellapolicy.PolicyNumber ?? "Unknown");
                    }
                }


                //Work Comp
                var wcpolicy = selectedpolicies?.Where(p => p.ProductId == 2).FirstOrDefault();
                if (wcpolicy is not null)
                {
                    //Set Carrier Assignments
                    SetJsonField(jsonObject, "WorkersCompensationEmployersLiability_InsurerLetterCode", letterArray[currentposition-1]);
                    
                    if (wcpolicy.Carrier != null && !string.IsNullOrEmpty(wcpolicy.Carrier.CarrierName))
                    {
                        string carrierFieldName = currentposition switch
                        {
                            1 => "Insurer_FullName",
                            2 => "Insurer_FullName_001",
                            3 => "Insurer_FullName_003",
                            4 => "Insurer_FullName_005",
                            5 => "Insurer_FullName_007",
                            _ => null
                        };
                        
                        if (carrierFieldName != null)
                        {
                            SetJsonField(jsonObject, carrierFieldName, wcpolicy.Carrier.CarrierName);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Workers Comp Policy carrier is null or has no name for PolicyId: {PolicyId}", wcpolicy.PolicyId);
                    }
                    currentposition++;

                    if (!string.IsNullOrEmpty(wcpolicy.PolicyNumber))
                        SetJsonField(jsonObject, "Policy_WorkersCompensationAndEmployersLiability_PolicyNumberIdentifier", wcpolicy.PolicyNumber);

                    if (wcpolicy.EffectiveDate != null)
                        SetJsonField(jsonObject, "Policy_WorkersCompensationAndEmployersLiability_EffectiveDate", wcpolicy.EffectiveDate.ToString("MM/dd/yyyy"));

                    if (wcpolicy.ExpirationDate != null)
                        SetJsonField(jsonObject, "Policy_WorkersCompensationAndEmployersLiability_ExpirationDate", wcpolicy.ExpirationDate.ToString("MM/dd/yyyy"));

                    // THIS IS LINE 771 - Check if WorkCompCoverage exists before accessing its properties
                    if (wcpolicy.WorkCompCoverage != null)
                    {
                        if (wcpolicy.WorkCompCoverage.EachAccident != null)
                            SetJsonField(jsonObject, "WorkersCompensationEmployersLiability_EmployersLiability_EachAccidentLimitAmount", 
                                wcpolicy.WorkCompCoverage.EachAccident.Value.ToString("N0"));

                        if (wcpolicy.WorkCompCoverage.DiseaseEachEmployee != null)
                            SetJsonField(jsonObject, "WorkersCompensationEmployersLiability_EmployersLiability_DiseaseEachEmployeeLimitAmount", 
                                wcpolicy.WorkCompCoverage.DiseaseEachEmployee.Value.ToString("N0"));

                        if (wcpolicy.WorkCompCoverage.DiseasePolicyLimit != null)
                            SetJsonField(jsonObject, "WorkersCompensationEmployersLiability_EmployersLiability_DiseasePolicyLimitAmount", 
                                wcpolicy.WorkCompCoverage.DiseasePolicyLimit.Value.ToString("N0"));

                        //Add filename to certificate for when/if we export/combine PDFs
                        if (wcpolicy.WorkCompCoverage.WaiverOfSubAttachment != null)
                        {
                            certificate.AttachWCWOSfilename = wcpolicy.WorkCompCoverage.WaiverOfSubAttachment.OriginalFileName;
                        }
                    }
                    else
                    {
                        Logger.LogWarning("WorkCompCoverage is null for Workers Comp policy {PolicyNumber}", wcpolicy.PolicyNumber ?? "Unknown");
                    }
                }


                //Property
                var propertypolicy = selectedpolicies?.Where(p => p.ProductId == 14).FirstOrDefault();
                if (propertypolicy is not null)
                {
                    //Set Carrier Assignments
                    SetJsonField(jsonObject, "OtherPolicy_InsurerLetterCode", letterArray[currentposition-1]);
                    SetJsonField(jsonObject, "OtherPolicy_OtherPolicyDescription", "PROPERTY");

                    if (propertypolicy.Carrier != null && !string.IsNullOrEmpty(propertypolicy.Carrier.CarrierName))
                    {
                        string carrierFieldName = currentposition switch
                        {
                            1 => "Insurer_FullName",
                            2 => "Insurer_FullName_001",
                            3 => "Insurer_FullName_003",
                            4 => "Insurer_FullName_005",
                            5 => "Insurer_FullName_007",
                            _ => null
                        };
                        
                        if (carrierFieldName != null)
                        {
                            SetJsonField(jsonObject, carrierFieldName, propertypolicy.Carrier.CarrierName);
                        }
                    }
                    else
                    {
                        Logger.LogWarning("Property Policy carrier is null or has no name for PolicyId: {PolicyId}", propertypolicy.PolicyId);
                    }
                    currentposition++;

                    if (!string.IsNullOrEmpty(propertypolicy.PolicyNumber))
                        SetJsonField(jsonObject, "OtherPolicy_PolicyNumberIdentifier", propertypolicy.PolicyNumber);

                    if (propertypolicy.EffectiveDate != null)
                        SetJsonField(jsonObject, "OtherPolicy_PolicyEffectiveDate", propertypolicy.EffectiveDate.ToString("MM/dd/yyyy"));

                    if (propertypolicy.ExpirationDate != null)
                        SetJsonField(jsonObject, "OtherPolicy_PolicyExpirationDate", propertypolicy.ExpirationDate.ToString("MM/dd/yyyy"));

                    if (propertypolicy.PropertyCoverage != null)
                    {
                        if (propertypolicy.PropertyCoverage.Equipment != null)
                        {
                            SetJsonField(jsonObject, "OtherPolicy_CoverageCode_102", "Equipment");
                            SetJsonField(jsonObject, "OtherPolicy_CoverageLimitAmount_103", 
                                propertypolicy.PropertyCoverage.Equipment.Value.ToString("N0"));
                        }

                        if (propertypolicy.PropertyCoverage.BusinessPersonalProperty != null)
                        {
                            SetJsonField(jsonObject, "OtherPolicy_CoverageCode_102", "BUS. PER. PROP.");
                            SetJsonField(jsonObject, "OtherPolicy_CoverageLimitAmount_103", 
                                propertypolicy.PropertyCoverage.BusinessPersonalProperty.Value.ToString("N0"));
                        }
                    }
                    else
                    {
                        Logger.LogWarning("PropertyCoverage is null for Property policy {PolicyNumber}", propertypolicy.PolicyNumber ?? "Unknown");
                    }
                }
            }

            string updatedJsonContent = jsonObject.ToString();
            var jsonBytes = System.Text.Encoding.UTF8.GetBytes(updatedJsonContent);
            stream = new MemoryStream(jsonBytes);

            await pdfViewer.ImportFormFieldsAsync(stream, FormFieldDataFormat.Json);
        }
        catch (NullReferenceException ex)
        {
            Logger.LogError(ex, "NullReferenceException in HandleLoadEverything at line {LineNumber}. Check for null policy coverage objects or missing JSON fields.", ex.StackTrace);
            throw new InvalidOperationException($"Error loading certificate data: A required field or policy coverage is missing. Details: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error in HandleLoadEverything");
            throw;
        }
    }

    /// <summary>
    /// Safely sets a JSON field value with validation and logging
    /// </summary>
    private void SetJsonField(JObject jsonObject, string fieldName, string value)
    {
        try
        {
            if (jsonObject.ContainsKey(fieldName))
            {
                jsonObject[fieldName] = value;
            }
            else
            {
                Logger.LogWarning("JSON field '{FieldName}' not found in PDF form. This field may have been renamed or removed.", fieldName);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error setting JSON field '{FieldName}' with value '{Value}'", fieldName, value);
        }
    }

    /// <summary>
    /// Validates that all expected JSON fields exist in the PDF form
    /// </summary>
    private async Task ValidateJsonFields()
    {
        var missingFields = new ArrayList();
        
        try
        {
            JObject pdfFormFields;
            using (var fileStream = new FileStream(JsonPath, FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fileStream))
                using (var jsonReader = new JsonTextReader(streamReader))
                {
                    pdfFormFields = await JObject.LoadAsync(jsonReader);
                }
            }

            // List of all fields used in the code
            var expectedFields = new[]
            {
                "Form_CompletionDate",
                "CertificateOfInsurance_CertificateNumberIdentifier",
                "NamedInsured_FullName",
                "NamedInsured_MailingAddress_LineOne",
                "NamedInsured_MailingAddress_LineTwo",
                "NamedInsured_MailingAddress_CityName",
                "CertificateHolder_FullName",
                "CertificateOfLiabilityInsurance_ACORDForm_RemarkText",
                "CertificateOfInsurance_GeneralLiability_AdditionalInsuredCode",
                "Policy_GeneralLiability_SubrogationWaivedCode",
                "Policy_WorkersCompensation_SubrogationWaivedCode",
                "GeneralLiability_CoverageIndicator",
                "GeneralLiability_InsurerLetterCode",
                "Insurer_FullName",
                "Insurer_FullName_001",
                "Insurer_FullName_003",
                "Insurer_FullName_005",
                "Insurer_FullName_007",
                "Policy_GeneralLiability_PolicyNumberIdentifier",
                "Policy_GeneralLiability_EffectiveDate",
                "Policy_GeneralLiability_ExpirationDate",
                "GeneralLiability_EachOccurrence_LimitAmount",
                "GeneralLiability_FireDamageRentedPremises_EachOccurrenceLimitAmount",
                "GeneralLiability_MedicalExpense_EachPersonLimitAmount",
                "GeneralLiability_PersonalAndAdvertisingInjury_LimitAmount",
                "GeneralLiability_GeneralAggregate_LimitAmount",
                "GeneralLiability_ProductsAndCompletedOperations_AggregateLimitAmount",
                "GeneralLiability_OtherCoverageLimitDescription",
                "GeneralLiability_ClaimsMadeIndicator",
                "GeneralLiability_OccurrenceIndicator",
                "Vehicle_InsurerLetterCode",
                "Policy_AutomobileLiability_PolicyNumberIdentifier",
                "Policy_AutomobileLiability_EffectiveDate",
                "Policy_AutomobileLiability_ExpirationDate",
                "Vehicle_CombinedSingleLimit_EachAccidentAmount",
                "Vehicle_BodilyInjury_PerPersonLimitAmount",
                "Vehicle_BodilyInjury_PerAccidentLimitAmount",
                "Vehicle_PropertyDamage_PerAccidentLimitAmount",
                "Vehicle_AnyAutoIndicator",
                "Vehicle_AllOwnedAutosIndicator",
                "Vehicle_HiredAutosIndicator",
                "Vehicle_ScheduledAutosIndicator",
                "Vehicle_NonOwnedAutosIndicator",
                "ExcessUmbrella_InsurerLetterCode",
                "Policy_ExcessLiability_PolicyNumberIdentifier",
                "Policy_ExcessLiability_EffectiveDate",
                "Policy_ExcessLiability_ExpirationDate",
                "ExcessUmbrella_Umbrella_DeductibleOrRetentionAmount",
                "ExcessUmbrella_Umbrella_EachOccurrenceAmount",
                "ExcessUmbrella_Umbrella_AggregateAmount",
                "Policy_PolicyType_UmbrellaIndicator",
                "Policy_PolicyType_ExcessIndicator",
                "ExcessUmbrella_DeductibleIndicator",
                "ExcessUmbrella_RetentionIndicator",
                "ExcessUmbrella_ClaimsMadeIndicator",
                "ExcessUmbrella_OccurrenceIndicator",
                "WorkersCompensationEmployersLiability_InsurerLetterCode",
                "Policy_WorkersCompensationAndEmployersLiability_PolicyNumberIdentifier",
                "Policy_WorkersCompensationAndEmployersLiability_EffectiveDate",
                "Policy_WorkersCompensationAndEmployersLiability_ExpirationDate",
                "WorkersCompensationEmployersLiability_EmployersLiability_EachAccidentLimitAmount",
                "WorkersCompensationEmployersLiability_EmployersLiability_DiseaseEachEmployeeLimitAmount",
                "WorkersCompensationEmployersLiability_EmployersLiability_DiseasePolicyLimitAmount",
                "OtherPolicy_InsurerLetterCode",
                "OtherPolicy_OtherPolicyDescription",
                "OtherPolicy_PolicyNumberIdentifier",
                "OtherPolicy_PolicyEffectiveDate",
                "OtherPolicy_PolicyExpirationDate",
                "OtherPolicy_CoverageCode_102",
                "OtherPolicy_CoverageLimitAmount_103"
            };

            foreach (var field in expectedFields)
            {
                if (!pdfFormFields.ContainsKey(field))
                {
                    missingFields.Add(field);
                    Logger.LogWarning("Expected field '{FieldName}' not found in PDF form JSON", field);
                }
            }

            if (missingFields.Count > 0)
            {
                Logger.LogWarning("Total missing fields: {Count}. Fields: {Fields}", missingFields.Count, string.Join(", ", missingFields.ToArray()));
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error validating JSON fields");
        }

        _missingFieldCount = missingFields.Count;
    }
    }
}
