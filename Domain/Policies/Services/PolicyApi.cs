// PolicyApi.cs
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Mantis.Data;
using Mantis.Domain.Policies.Models;

namespace Mantis.Domain.Policies.Services
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PolicyController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public List<Policy> GetPoliciesList()
        {
            return _context.Policies
                           .Include(p => p.Client)
                           .Include(p => p.Carrier)
                           .Include(p => p.Producer)
                           .Include(p => p.Product)
                           .OrderByDescending(p => p.PolicyId)
                           .ToList();
        }

        [HttpPost]
        public object Post2([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Policy> DataSource = GetPoliciesList();

            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }

            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }

            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }

            int TotalRecordsCount = DataSource.Cast<Policy>().Count();

            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }

            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }

            return new { result = DataSource, count = TotalRecordsCount };
        }
        
        [HttpGet("List")]
        public List<PolicyDto> GetPoliciesDto()
        {
            var policies = _context.Policies
                .Include(p => p.Client)
                .Include(p => p.Carrier)
                .Include(p => p.Producer)
                .Include(p => p.Product)
                .Select(p => new PolicyDto
                {
                    PolicyId = p.PolicyId,
                    PolicyNumber = p.PolicyNumber,
                    EffectiveDate = p.EffectiveDate,
                    ExpirationDate = p.ExpirationDate,
                    Premium = p.Premium,
                    Status = p.Status,
                    Notes = p.Notes,
                    ClientName = p.Client.Name,
                    LineType = p.Product.LineNickname,
                    CarrierName = p.Carrier.CarrierName,
                    WholesalerName = p.Wholesaler.CarrierName,
                    ProducerName = p.Producer.UserName
                })
                .OrderByDescending(p => p.ExpirationDate)
                .ToList();

            return policies;
        }

        [HttpPost("List")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<PolicyDto> DataSource = GetPoliciesDto();

            if (DataManagerRequest.Search != null && DataManagerRequest.Search.Count > 0)
            {
                DataSource = DataOperations.PerformSearching(DataSource, DataManagerRequest.Search);
            }

            if (DataManagerRequest.Where != null && DataManagerRequest.Where.Count > 0)
            {
                DataSource = DataOperations.PerformFiltering(DataSource, DataManagerRequest.Where, DataManagerRequest.Where[0].Operator);
            }

            if (DataManagerRequest.Sorted != null && DataManagerRequest.Sorted.Count > 0)
            {
                DataSource = DataOperations.PerformSorting(DataSource, DataManagerRequest.Sorted);
            }

            int TotalRecordsCount = DataSource.Cast<PolicyDto>().Count();

            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }

            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }

            return new { result = DataSource, count = TotalRecordsCount };
        }

    }
}
