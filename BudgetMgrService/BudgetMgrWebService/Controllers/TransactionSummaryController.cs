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
    public class TransactionSummmaryController : ControllerBase
    {
        private readonly TransactionContext _context;
        private readonly string miscellaenousGroupType = "misc";

        public TransactionSummmaryController(TransactionContext context)
        {
            _context = context;
        }

        // GET: api/TransactionSummary
        [HttpGet("TransactionSummaryByGroupTypeAndDate")]
        public async Task<ActionResult<TransactionSummary>> GetTransactionSummaryByGroupTypeAndDate([FromQuery(Name = "grouptype")] string groupType,
                                                                                                    [FromQuery(Name = "startdate")] string startDate,
                                                                                                    [FromQuery(Name = "enddate")] string endDate)
        {
            var summary = await _context.FindTransactionSummaryAsync(miscellaenousGroupType, groupType, startDate, endDate);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // var summary = await _context.TransactionSummary.FindAsync(miscellaenousGroupType, groupType, startDate, endDate);

            if (summary == null)
            {
                return NotFound();
            }

            return summary;
        }
    }
}
