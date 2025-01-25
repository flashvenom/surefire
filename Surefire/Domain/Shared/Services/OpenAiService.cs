using Surefire.Data;
using Surefire.Domain.Logs;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Surefire.Domain.Shared.Services;
using Humanizer;
using Sprache;
using OpenAI.Assistants;

namespace Surefire.Domain.OpenAI
{
    public class OpenAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly StateService _stateService;
        private readonly ILoggingService _logService;

        public OpenAiService(HttpClient httpClient, IConfiguration configuration, StateService stateService, IDbContextFactory<ApplicationDbContext> dbContextFactory, ILoggingService logService)
        {
            _httpClient = httpClient;
            _apiKey = "[APIKEY]"; // Get from database
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "[APIKEY]");
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
            _dbContextFactory = dbContextFactory;
            _stateService = stateService;
            _logService = logService;
        }

        // Smart Paste and XML
        public async Task<string?> GetPromptByIdAsync(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            var promptRecord = await context.OpenAIPrompt.FirstOrDefaultAsync(p => p.OpenAIPromptId == id);
            return promptRecord?.prompt;
        }
        public async Task<LeadData> ExtractLeadDataAsync(string text)
        {
            var prompt = $"Extract the following information from the text:\nFirst Name, Last Name, Company Name, Contact Title, Phone Number (including Office, Direct, Mobile, Cell, etc.), Address, City, State, Zip, Email, Website, and Lead Type (e.g., Work Comp, General Liability, etc).\n\nText:\n{text}\n\nProvide the output in JSON format.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "user", content = prompt }
        },
                temperature = 0.2
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                try
                {
                    var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString);
                    Console.WriteLine("responseString---------------------");
                    Console.WriteLine(responseString);
                    var assistantMessage = openAiResponse?.Choices?[0]?.Message?.Content;

                    if (!string.IsNullOrEmpty(assistantMessage))
                    {
                        assistantMessage = assistantMessage.Trim();

                        // Deserialize the message to LeadData
                        var leadData = JsonSerializer.Deserialize<LeadData>(assistantMessage, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (leadData != null && leadData.PhoneNumbers != null)
                        {
                            // Logic to select the best phone numbers for Phone and Mobile fields
                            leadData.PhoneNumbers["BestPhone"] = GetBestPhoneNumber(leadData.PhoneNumbers);
                        }

                        return leadData;
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON deserialization error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"OpenAI API returned status code {response.StatusCode}: {response.ReasonPhrase}");
            }

            return null;
        }
        public async Task<string> ParseXmlToJsonAsync(string xmlContent, int promptid)
        {
            var prompt = await GetPromptByIdAsync(promptid);

            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[] { new { role = "user", content = prompt + "\n\n" + xmlContent } },
                temperature = 0.2
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString);
                return openAiResponse?.Choices?[0]?.Message?.Content ?? string.Empty;
            }

            Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {response.ReasonPhrase}");
            return string.Empty;
        }
        public async Task<string> ParseXmlToBusinessDetailsWithFunction(string xmlContent)
        {
            // Load and read the schema JSON file
            var schemaPath = Path.Combine("wwwroot", "schemas", "extractor.json");
            var schemaJson = await File.ReadAllTextAsync(schemaPath);

            // Deserialize the schema JSON to use as function parameters
            var schemaObject = JsonSerializer.Deserialize<object>(schemaJson);

            // Prepare the request body with model, message, and function call
            var requestBody = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
            new
            {
                role = "user",
                content =
                    "Extract key information from the provided XML file and convert it into structured JSON. Use the following guidelines:\n" +
                    "\n1. Identify and label fields precisely as specified in the schema.\n" +
                    "2. Retain any hierarchy or relationships between data elements when applicable.\n" +
                    "3. Ensure numerical data, dates, and text fields are formatted correctly, adhering to standard representations (e.g., YYYY-MM-DD for dates).\n" +
                    "4. If any field data is missing, attempt to find the answer by using context clues in other relevent data, for example, if you have the 'Years in Business' you can calculate the date started, annual payroll can be used for current year payroll.\n" +
                    "5. If the business descriptions, industry, specialty data is not found, try to fill these fields in using context clues from other data and also using outside data you have access to, like SIC codes and business directories..\n" +
                    "6. Prioritize clarity and accuracy, favoring succinct field names over descriptive ones if they meet schema requirements.\n" +
                    "\nPlease present the extracted data in JSON format, with strict adherence to the schema. Include any metadata or context-specific identifiers if present in the XML.\n\n" +
                    "Here is the XML content:\n\n" + xmlContent
            }
        },
                functions = new[]
                {
            new
            {
                name = "acordxml-extractor-v1",
                parameters = schemaObject // Embedding schema for accurate extraction
            }
        },
                function_call = new { name = "acordxml-extractor-v1" }
            };

            // Serialize the request body
            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            Console.WriteLine("Request JSON:");
            Console.WriteLine(jsonRequestBody);

            // Send the request to OpenAI API
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseString);
                var resultContent = openAiResponse?.Choices?[0]?.Message?.FunctionCall?.Arguments ?? string.Empty;


                Console.WriteLine("Response JSON:");
                Console.WriteLine(responseString);

                return resultContent;
            }
            else
            {
                Console.WriteLine($"OpenAI API Error: {response.StatusCode} - {response.ReasonPhrase}");
                var errorDetails = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error Details:");
                Console.WriteLine(errorDetails);
                return string.Empty;
            }
        }

        // Settlement and Invoices
        public async Task<string> UploadFileAsync(string filePath)
        {
            //return "file-S6SmnCrghyK87Wp8T3kgWi"; //Skip the upload
            _stateService.UpdateStatus("Uploading file to AI...", true);
            try
            {
                var fileStream = File.OpenRead(filePath);
                var formData = new MultipartFormDataContent
                {
                    { new StreamContent(fileStream), "file", Path.GetFileName(filePath) },
                    { new StringContent("assistants"), "purpose" }
                };
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/files", formData);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Upload Response: " + responseContent);

                    // Deserialize into the strongly-typed class
                    var fileData = JsonSerializer.Deserialize<FileUploadResponse>(responseContent);

                    if (fileData != null)
                    {
                        Console.WriteLine($"File uploaded successfully. File ID: {fileData.id}");
                        return fileData.id; // Return the extracted file ID
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _stateService.UpdateStatus($"Upload Error: {response.StatusCode} - {error}", false);
                    await _logService.LogAsync(LogLevel.Error, $"Upload Error1: {response.StatusCode} - {error}", "OpenAIService");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _stateService.UpdateStatus($"Upload Error: {ex.Message}", false);
                await _logService.LogAsync(LogLevel.Error, $"Upload Error2: {ex.Message}", "OpenAIService");
                return string.Empty;
            }
        }
        public async Task<string> SendMessageWithFileAsync(string threadId, string fileId, string messageContent)
        {
            string newmsg = "Extract the data from the invoice file: " + fileId + "    Make sure your final response returns the output data in the defined JSON format.\r\n\r\n{ \"name\": \"invoice_schema\", \"strict\": true, \"schema\": { \"type\": \"object\", \"properties\": { \"policyNumber\": { \"type\": \"string\", \"description\": \"The unique identifier for the policy.\" }, \"carrier\": { \"type\": \"string\", \"description\": \"The insurance carrier associated with the policy.\" }, \"billingCompany\": { \"type\": \"string\", \"description\": \"The name of the billing company.\" }, \"invoiceNumber\": { \"type\": \"string\", \"description\": \"The unique identifier for the invoice.\" }, \"invoiceFrom\": { \"type\": \"string\", \"description\": \"The entity or person responsible for issuing the invoice.\" }, \"dueDate\": { \"type\": \"string\", \"description\": \"The due date for the invoice payment in YYYY-MM-DD format.\" }, \"basePremium\": { \"type\": \"string\", \"description\": \"The base premium amount for the policy.\" }, \"fees\": { \"type\": \"array\", \"description\": \"List of additional fees associated with the invoice.\", \"items\": { \"type\": \"object\", \"properties\": { \"description\": { \"type\": \"string\", \"description\": \"Description of the fee.\" }, \"amount\": { \"type\": \"string\", \"description\": \"Amount for the fee.\" } }, \"required\": [ \"description\", \"amount\" ], \"additionalProperties\": false } }, \"commissionRate\": { \"type\": \"string\", \"description\": \"The commission rate expressed as a percentage.\" }, \"commissionNet\": { \"type\": \"string\", \"description\": \"The net commission received.\" }, \"basePremiumNetAmount\": { \"type\": \"string\", \"description\": \"The net amount for the base premium.\" }, \"netDue\": { \"type\": \"string\", \"description\": \"The total amount due after adjustments.\" }, \"fileId\": { \"type\": \"string\", \"description\": \"The unique identifier for the associated file.\" }, \"fileName\": { \"type\": \"string\", \"description\": \"The name of the associated file.\" }, \"comments\": { \"type\": \"string\", \"description\": \"Additional comments or notes regarding the invoice.\" } }, \"required\": [ \"policyNumber\", \"carrier\", \"billingCompany\", \"invoiceNumber\", \"invoiceFrom\", \"dueDate\", \"basePremium\", \"fees\", \"commissionRate\", \"commissionNet\", \"basePremiumNetAmount\", \"netDue\", \"fileId\", \"fileName\", \"comments\" ], \"additionalProperties\": false } }";
            var requestBody = new
            {
                role = "user",
                content = new[]
                {
                new { type = "text", text = newmsg }
                },
                        attachments = new[]
                        {
                    new
                    {
                        file_id = fileId,
                        tools = new[]
                        {
                            new { type = "file_search" }
                        }
                    }
                }
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/messages", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Message sent successfully----------------------------");
                Console.WriteLine(responseContent);
                Console.WriteLine("----------------------------");
                return responseContent;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error sending message: {response.StatusCode} - {error}");
                return string.Empty;
            }
        }
        public async Task<string> RunThreadAsync(string threadId, string assistantId, string fileId)
        {
            _stateService.UpdateStatus($"Running thread...", true);
            var requestBody = new
            {
                assistant_id = assistantId,
                additional_instructions = (string?)null,
                tool_choice = (string?)null
            };

            var jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"https://api.openai.com/v1/threads/{threadId}/runs", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _stateService.UpdateStatus($"Upload Error: Error running thread: {response.StatusCode} - {error}", false);
                await _logService.LogAsync(LogLevel.Error, $"Error running thread: {response.StatusCode} - {error}", "OpenAIService");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("RunThreadResponse: " + responseContent);
            var runData = JsonSerializer.Deserialize<RunStatusResponse>(responseContent);

            if (runData == null || string.IsNullOrEmpty(runData.id))
            {
                _stateService.UpdateStatus("Failed to parse run return", false);
                await _logService.LogAsync(LogLevel.Error, "Failed to parse run return", "OpenAIService");
                return null;
            }

            Console.WriteLine("Thread run successfully initiated. Run ID: " + runData.id);

            // Poll for the final response
            return await PollForFinalResponseAsync(threadId, runData.id, fileId);
        }
        public async Task<string> PollForFinalResponseAsync(string threadId, string runId, string fileId)
        {
            _stateService.UpdateStatus($"Polling run status...", true);
            const int PollIntervalMs = 2000;
            string runStatusUrl = $"https://api.openai.com/v1/threads/{threadId}/runs/{runId}";
            string finalResponse = null;

            while (true)
            {
                var response = await _httpClient.GetAsync(runStatusUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var runStatus = JsonSerializer.Deserialize<RunStatusResponse>(responseContent);

                    if (runStatus.status == "completed")
                    {
                        finalResponse = runStatus.finalOutput; // Adjust if the final output needs parsing
                        break;
                    }
                    else if (runStatus.status == "requires_action")
                    {
                        await HandleRequiredActionAsync(threadId, runStatus, fileId);
                    }
                    else if (runStatus.status == "failed")
                    {
                        _stateService.UpdateStatus($"Run failed: {runStatus.last_error}", false);
                        await _logService.LogAsync(LogLevel.Error, $"Run failed: {runStatus.last_error}", "OpenAIService");
                        throw new Exception();
                    }
                }

                await Task.Delay(PollIntervalMs);
            }

            return finalResponse ?? "No final response received.";
        }
        public async Task HandleRequiredActionAsync(string threadId, RunStatusResponse runStatus, string fileId)
        {
            _stateService.UpdateStatus($"Handling required actions.....", true);
            if (runStatus.required_action?.type == "submit_tool_outputs")
            {
                foreach (var toolCall in runStatus.required_action.submit_tool_outputs.tool_calls)
                {
                    var toolOutput = new
                    {
                        
                        tool_call_id = toolCall.id,
                        output = toolCall.function.arguments
                    };

                    var requestBody = new
                    {
                        tool_outputs = new[] { toolOutput }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    var submitUrl = $"https://api.openai.com/v1/threads/{threadId}/runs/{runStatus.id}/submit_tool_outputs";

                    var response = await _httpClient.PostAsync(submitUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var error = await response.Content.ReadAsStringAsync();
                        _stateService.UpdateStatus($"Error submitting tool output: {error}", false);
                        await _logService.LogAsync(LogLevel.Error, $"Error submitting tool output: {error}", "OpenAIService");
                        throw new Exception($"Error submitting tool output: {error}");
                    }
                }
            }
        }
        public async Task<string> CreateNewThreadAsync()
        {
            _stateService.UpdateStatus($"Creating thread.", true);
            try
            {
                // Prepare the request
                var requestBody = new
                {
                    // Add any additional fields if required by your setup
                };

                var jsonRequestBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                // Send the request to the API
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/threads", content);
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response to get the thread ID
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var threadResponse = JsonSerializer.Deserialize<ThreadResponse>(responseContent);

                    if (!string.IsNullOrEmpty(threadResponse?.Id))
                    {
                        return threadResponse.Id; // Return the thread ID
                    }
                    else
                    {
                        await _logService.LogAsync(LogLevel.Error, "Failed to extract thread ID from the response.", "OpenAIService");
                        return string.Empty;
                    }
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    await _logService.LogAsync(LogLevel.Error, $"Error creating thread: {response.StatusCode} - {error}", "OpenAIService");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, $"Error in CreateNewThreadAsync: {ex.Message}", "OpenAIService");
                _stateService.UpdateStatus($"Error in CreateNewThreadAsync: {ex.Message}", false);
                return string.Empty;
            }
        }
        public async Task<string> GetLastMessageAsync(string threadId)
        {
            _stateService.UpdateStatus($"Getting response..........", true);
            string url = $"https://api.openai.com/v1/threads/{threadId}/messages";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                await _logService.LogAsync(LogLevel.Error, $"Error fetching messages: {response.StatusCode} - {error}", "OpenAIService");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            try
            {
                // Parse the response to extract the required value
                var jsonDocument = JsonDocument.Parse(responseContent);
                var root = jsonDocument.RootElement;

                // Navigate to `data[0] > content[0] > text > value`
                var value = root
                    .GetProperty("data")[0]
                    .GetProperty("content")[0]
                    .GetProperty("text")
                    .GetProperty("value")
                    .GetString();
                return value ?? "No value found.";
            }
            catch (Exception ex)
            {
                await _logService.LogAsync(LogLevel.Error, $"Error parsing JSON: {ex.Message}", "OpenAIService");
                _stateService.UpdateStatus($"Error parsing JSON: {ex.Message}", false);
                return "Error extracting value.";
            }
        }
        public string ExtractJsonFromString(string input)
        {
            try
            {
                // Locate the indices of the first and last ```json and ```
                int startIndex = input.IndexOf("```json") + "```json".Length;
                int endIndex = input.LastIndexOf("```");

                // Validate indices
                if (startIndex > 0 && endIndex > startIndex)
                {
                    // Extract and trim the JSON substring
                    string jsonSubstring = input.Substring(startIndex, endIndex - startIndex).Trim();
                    Console.WriteLine("Extracted JSON:");
                    Console.WriteLine(jsonSubstring);
                    return jsonSubstring;
                }
                else
                {
                    Console.WriteLine("Could not find valid JSON markers.");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting JSON: {ex.Message}");
                return string.Empty;
            }
        }
        public string GetBestPhoneNumber(Dictionary<string, string> phoneNumbers)
        {
            // Prioritize Direct, Cell/Mobile, Office, and Desk in this order
            if (phoneNumbers.ContainsKey("Direct"))
                return phoneNumbers["Direct"];
            if (phoneNumbers.ContainsKey("Mobile"))
                return phoneNumbers["Mobile"];
            if (phoneNumbers.ContainsKey("Cell"))
                return phoneNumbers["Cell"];
            if (phoneNumbers.ContainsKey("Office"))
                return phoneNumbers["Office"];
            if (phoneNumbers.ContainsKey("Desk"))
                return phoneNumbers["Desk"];

            // If none of the preferred types are available, return the first number
            return phoneNumbers.Values.FirstOrDefault();
        }
    }
}
