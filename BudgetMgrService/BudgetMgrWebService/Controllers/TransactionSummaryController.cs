using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Mvc;

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionSummaryController : ControllerBase
    {
        private readonly DataStore dataStore;
        public TransactionSummaryController(DataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        // GET: api/TransactionSummary
        [HttpGet("TransactionSummary")]
        public async Task<ActionResult<TransactionSummary>> GetTransactionSummary()
        {
            var summary = await dataStore.FindTransactionSummaryAsync();

            // TODO:  Revert to the following usage dataStore connects to an actual database    
            // var summary = await dataStore.TransactionSummary.FindAsync();

            if (summary == null)
            {
                return NotFound();
            }

            return summary;
        }

        // GET: api/TransactionSummary
        [HttpGet("TransactionSummaryByDate")]
        public async Task<ActionResult<TransactionSummary>> GetTransactionSummaryByDate([FromQuery] DateFilterBodyParams dateFilter)
        {
            var summary = await dataStore.FindTransactionSummaryAsync(dateFilter.StartDate, dateFilter.EndDate);

            // TODO:  Revert to the following usage dataStore connects to an actual database    
            // var summary = await dataStore.TransactionSummary.FindAsync(dateFilter.StartDate, dateFilter.EndDate);

            if (summary == null)
            {
                return NotFound();
            }

            return summary;
        }

        // GET: api/TransactionSummary
        [HttpGet("TransactionSummaryByGroupTypeAndDate")]
        public async Task<ActionResult<TransactionSummary>> GetTransactionSummaryByGroupTypeAndDate([FromQuery] DateFilterAndGroupTypeBodyParams dateFilterAndGroupType)
        {
            var summary = await dataStore.FindTransactionSummaryAsync(dateFilterAndGroupType.GroupType, dateFilterAndGroupType.StartDate, dateFilterAndGroupType.EndDate);

            // TODO:  Revert to the following usage dataStore connects to an actual database    
            // var summary = await dataStore.TransactionSummary.FindAsync(dateFilterAndGroupType.GroupType, dateFilterAndGroupType.StartDate, dateFilterAndGroupType.EndDate);

            if (summary == null)
            {
                return NotFound();
            }

            return summary;
        }

    }
}
