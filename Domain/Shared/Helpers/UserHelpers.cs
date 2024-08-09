using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Shared;


namespace Mantis.Domain.Shared.Helpers
{
    public static class UserHelper
    {
        public static string GetInitials(string firstName, string lastName)
        {
            var firstInitial = !string.IsNullOrEmpty(firstName) ? firstName[0].ToString().ToUpper() : string.Empty;
            var lastInitial = !string.IsNullOrEmpty(lastName) ? lastName[0].ToString().ToUpper() : string.Empty;
            return $"{firstInitial}{lastInitial}";
        }
    }
}
