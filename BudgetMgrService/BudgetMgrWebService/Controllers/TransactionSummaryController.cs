using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionSummaryController : ControllerBase
    {
        private readonly TransactionContext _context;
        public TransactionSummaryController(TransactionContext context)
        {
            _context = context;
        }

        // GET: api/TransactionSummary
        [HttpGet("TransactionSummary")]
        public async Task<ActionResult<TransactionSummary>> GetTransactionSummary()
        {
            var summary = await _context.FindTransactionSummaryAsync();

            // TODO:  Revert to the following usage context connects to an actual database    
            // var summary = await _context.TransactionSummary.FindAsync();

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
            var summary = await _context.FindTransactionSummaryAsync(dateFilter.StartDate, dateFilter.EndDate);

            // TODO:  Revert to the following usage context connects to an actual database    
            // var summary = await _context.TransactionSummary.FindAsync(dateFilter.StartDate, dateFilter.EndDate);

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
            var summary = await _context.FindTransactionSummaryAsync(dateFilterAndGroupType.GroupType, dateFilterAndGroupType.StartDate, dateFilterAndGroupType.EndDate);

            // TODO:  Revert to the following usage context connects to an actual database    
            // var summary = await _context.TransactionSummary.FindAsync(dateFilterAndGroupType.GroupType, dateFilterAndGroupType.StartDate, dateFilterAndGroupType.EndDate);

            if (summary == null)
            {
                return NotFound();
            }

            return summary;
        }

    }
}
