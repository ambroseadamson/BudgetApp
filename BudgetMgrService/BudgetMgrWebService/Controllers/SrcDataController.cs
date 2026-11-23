using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrcDataController : ControllerBase
    {
        private readonly DataStore dataStore;

        public SrcDataController(DataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        // POST: api/SrcData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SummaryResult>> Post([FromServices] IOptions<TestDataCfg> testDataCfgAccessor, [FromBody] SrcData srcData)
        {
            if (srcData.Data == null)
            {
                return NoContent();
            }

            if (srcData.Data.Count() == 0)
            {
                return NoContent();
            }

            // NOTE: SrcData data type will change to meet the requirements of the national bank statement data that will be posted to this endpoint.
            var results = srcData.Data.GenTransactionData(testDataCfgAccessor.Value, dataStore.ToGroupTypeCfgEx(), dataStore.ApplyUniqueIds);
            if (results.Results.IsNullOrEmpty())
            {
                return BadRequest();
            }

            dataStore.Transactions.AddRange(results.Results.RemoveDuplicates(dataStore.Transactions));

            await dataStore.SaveChangesAsync();

            return Ok();
        }
    }
}
