using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Threading.Channels;
using Mantis.Domain.Carriers.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mantis.Domain.Shared;

namespace Mantis.Controllers
{
    [ApiController]
    public class GridController : ControllerBase
    {
        [HttpPost]
        [Route("ipa/")]
        public object Post([FromBody] DataManagerRequest DataManagerRequest)
        {
            IEnumerable<Carrier> DataSource = GetOrderData();
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
            int TotalRecordsCount = DataSource.Cast<Carrier>().Count();
            IDictionary<string, object> Aggregates = null;
            if (DataManagerRequest.Aggregates != null) // Aggregation
            {
                Aggregates = DataUtil.PerformAggregation(DataSource, DataManagerRequest.Aggregates);
            }
            if (DataManagerRequest.Skip != 0)
            {
                DataSource = DataOperations.PerformSkip(DataSource, DataManagerRequest.Skip);
            }
            if (DataManagerRequest.Take != 0)
            {
                DataSource = DataOperations.PerformTake(DataSource, DataManagerRequest.Take);
            }
            return new { result = DataSource, count = TotalRecordsCount, aggregates = Aggregates };
        }
        [Route("ipa/Grid")]
        public List<Carrier> GetOrderData()
        {
            //BREAKPOINT WILL HIT HERE
            string ConnectionString = @"";
            string QueryStr = "SELECT * FROM dbo.Carriers ORDER BY CarrierId;";
            SqlConnection sqlConnection = new(ConnectionString);
            sqlConnection.Open();
            SqlCommand SqlCommand = new(QueryStr, sqlConnection);
            SqlDataAdapter DataAdapter = new(SqlCommand);
            DataTable DataTable = new();
            DataAdapter.Fill(DataTable);
            sqlConnection.Close();
            var DataSource = (from DataRow Data in DataTable.Rows
                              select new Carrier()
                              {
                                  CarrierId = Convert.ToInt32(Data["CarrierId"]),
                                  CarrierName = Data["CarrierName"].ToString()
                              }).ToList();
            return DataSource;
        }

        [HttpPost]
        [Route("ipa/GridInsert")]
        public void Insert([FromBody] CRUDModel<Carrier> Value)
        {
            //BREAKPOINT DOES ***NOT*** HIT IN HERE
            string ConnectionString = @"";
        }

        public class CRUDModel<T> where T : class
        {
            [JsonProperty("action")]
            public string? Action { get; set; }
            [JsonProperty("keyColumn")]
            public string? KeyColumn { get; set; }
            [JsonProperty("key")]
            public object? Key { get; set; }
            [JsonProperty("value")]
            public T? Value { get; set; }
            [JsonProperty("added")]
            public List<T>? Added { get; set; }
            [JsonProperty("changed")]
            public List<T>? Changed { get; set; }
            [JsonProperty("deleted")]
            public List<T>? Deleted { get; set; }
            [JsonProperty("params")]
            public IDictionary<string, object>? Params { get; set; }
        }
    }
}
