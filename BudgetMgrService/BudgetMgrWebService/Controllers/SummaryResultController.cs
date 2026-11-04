using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryResultController : ControllerBase
    {
        private readonly TransactionContext _context;

        private readonly string miscellaenousGroupType = "misc";
        public SummaryResultController(TransactionContext context)
        {
            _context = context;
        }

        // TODO: Move to SummaryResultController
        // GET: api/SummaryResult
        [HttpGet]
        public async Task<ActionResult<SummaryResult>> GetSummaryResult()
        {
            return await _context.ToSummaryResultAsync(miscellaenousGroupType);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // return await _context.SummaryResult.ToAsync(miscellaenousGroupType);
        }


        // TODO: Move to SummaryResultController
        // GET: api/SummaryResultByDateRange
        [HttpGet("SummaryResultByDateRange")]
        public async Task<ActionResult<SummaryResult>> GetSummaryResultByDateRange([FromQuery(Name = "startdate")] string startDate,
                                                                                   [FromQuery(Name = "enddate")] string endDate)
        {

            var summaryResult = await _context.FindSummaryResultAsync(miscellaenousGroupType, startDate, endDate);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // var summaryResult = await _context.SummaryResult.FindAsync(miscellaenousGroupType, startDate, endDate);

            if (summaryResult == null)
            {
                return NotFound();
            }

            return summaryResult;
        }

        // TODO: Move to SrcDataController
        // POST: api/SrcData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SummaryResult>> Create([FromServices] IOptions<TestDataCfg> testDataCfgAccessor,
                                                              [FromBody] SrcData srcData)
        {
            _context.AddTransactions(srcData, testDataCfgAccessor);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // _context.Transactions.Add(srcData, testDataCfgAccessor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSummaryResult", null);
        }


        private bool SummaryResultExists(int id)
        {
            return _context.SummaryResult.Any(e => e.Id == id);
        }
    }
}
