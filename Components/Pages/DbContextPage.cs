// File: Pages/DbContextPage.cs
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Mantis.Pages
{
    public class DbContextPage<TContext> : ComponentBase where TContext : DbContext
    {
        [Inject]
        protected TContext DbContext { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // Initialize or load data if needed
        }

        // Add any common methods or properties
    }
}
