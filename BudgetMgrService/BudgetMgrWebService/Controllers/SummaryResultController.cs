using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryResultController : ControllerBase
    {
        private readonly TransactionContext _context;

        public SummaryResultController(TransactionContext context)
        {
            _context = context;
        }

        // GET: api/SummaryResult
        [HttpGet]
        public async Task<ActionResult<SummaryResult>> GetSummaryResult()
        {
            return await _context.ToSummaryResultAsync();

            // TODO:  Revert to the following usage context connects to an actaul database    
            // return await _context.SummaryResult.ToAsync();
        }


        // GET: api/SummaryResultByDateRange
        [HttpGet("SummaryResultByDateRange")]
        public async Task<ActionResult<SummaryResult>> GetSummaryResultByDateRange([FromQuery] DateFilterAndGroupTypeBodyParams dateFilter)
        {

            var summaryResult = await _context.FindSummaryResultAsync(dateFilter.StartDate, dateFilter.EndDate, dateFilter.GroupType);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // var summaryResult = await _context.SummaryResult.FindAsync(miscellaenousGroupType, startDate, endDate);

            if (summaryResult == null)
            {
                return NotFound();
            }

            return summaryResult;
        }
    }
}
