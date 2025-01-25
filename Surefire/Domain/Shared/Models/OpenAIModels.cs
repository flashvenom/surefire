using System.Text.Json.Serialization;
using System.Text.Json;

namespace Surefire.Domain.OpenAI
{
    public class OpenAIPrompt
    {
        public int OpenAIPromptId { get; set; }
        public string prompt { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string? note { get; set; }
    }

    public class OpenAIResponse
    {
        public int OpenAIResponseId { get; set; }
        public string? promptId { get; set; }
        public string? response { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
    public class OpenAiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        // Include other properties as needed

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }
    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("function_call")]
        public FunctionCall FunctionCall { get; set; }
    }
    public class FunctionCall
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("arguments")]
        public string Arguments { get; set; }
    }
    public class LeadData
    {
        [JsonPropertyName("First Name")]
        public string FirstName { get; set; }

        [JsonPropertyName("Last Name")]
        public string LastName { get; set; }

        [JsonPropertyName("Company Name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("Contact Title")]
        public string ContactTitle { get; set; }

        [JsonPropertyName("Phone Number")]
        [JsonConverter(typeof(PhoneNumberConverter))]
        public Dictionary<string, string> PhoneNumbers { get; set; }

        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [JsonPropertyName("Website")]
        public string Website { get; set; }

        [JsonPropertyName("Address")]
        public string Address { get; set; }

        [JsonPropertyName("City")]
        public string City { get; set; }

        [JsonPropertyName("State")]
        public string State { get; set; }

        [JsonPropertyName("Zip")]
        public string Zip { get; set; }

        [JsonPropertyName("Lead Type")]
        public string LeadType { get; set; }
    }
    public class PhoneNumberConverter : JsonConverter<Dictionary<string, string>>
    {
        public override Dictionary<string, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // When "Phone Number" is a simple string, return it as the default type (e.g., "Phone")
                return new Dictionary<string, string> { { "Phone", reader.GetString() } };
            }
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                // When "Phone Number" is a dictionary
                return JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader, options);
            }

            throw new JsonException("Invalid type for Phone Number");
        }
        public override void Write(Utf8JsonWriter writer, Dictionary<string, string> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
    public class ThreadResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
    public class FileUploadResponse
    {
        public string id { get; set; } // Matches the JSON "id"
        public string @object { get; set; } // Matches the JSON "object"
        public string purpose { get; set; } // Matches the JSON "purpose"
        public string filename { get; set; } // Matches the JSON "filename"
        public int bytes { get; set; } // Matches the JSON "bytes"
        public long created_at { get; set; } // Matches the JSON "created_at"
        public string status { get; set; } // Matches the JSON "status"
        public object status_details { get; set; } // Matches the JSON "status_details"
    }
    public class RunStatusResponse
    {
        public string status { get; set; }
        public RequiredAction required_action { get; set; }
        public string id { get; set; }
        public string last_error { get; set; }
        public string finalOutput { get; set; } // Assuming there's a field for the final output
    }
    public class RequiredAction
    {
        public string type { get; set; }
        public SubmitToolOutputs submit_tool_outputs { get; set; }
    }
    public class SubmitToolOutputs
    {
        public List<ToolCall> tool_calls { get; set; }
    }
    public class ToolCall
    {
        public string id { get; set; }
        public FunctionDetails function { get; set; }
    }
    public class FunctionDetails
    {
        public string name { get; set; }
        public string arguments { get; set; } // The JSON arguments
    }
    public class MessageListResponse
    {
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }
    }
}
