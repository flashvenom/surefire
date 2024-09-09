using Mantis.Domain.Renewals.Models;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Mantis.Domain.Shared.Helpers
{
    public class DataHelper
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public DataHelper(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public string GetBaseUrl()
        {
            // If in Production, prioritize the environment variable
            if (_environment.IsProduction())
            {
                return _configuration["SUREFIRE_BASE_URL"] ?? _configuration["SurefireBaseUrl"];
            }

            // If in Development or Debug, use the value from appsettings.Development.json
            if (_environment.IsDevelopment())
            {
                return _configuration["SurefireBaseUrl"];
            }

            // Fallback to the default base URL in appsettings.json
            return _configuration["SurefireBaseUrl"];
        }
    }

    public class EmptyValidator : ComponentBase
    {
        // No logic required here, this is just to bypass validation.
    }

}
