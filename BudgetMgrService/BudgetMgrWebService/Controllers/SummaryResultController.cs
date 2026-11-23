using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
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
        private readonly DataStore dataStore;

        public SummaryResultController(DataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        // GET: api/SummaryResult
        [HttpGet]
        public async Task<ActionResult<SummaryResult>> GetSummaryResult()
        {
            return await dataStore.ToSummaryResultAsync();

            // TODO:  Revert to the following usage dataStore connects to an actual database    
            // return await dataStore.SummaryResult.ToAsync();
        }


        // GET: api/SummaryResultByDateRange
        [HttpGet("SummaryResultByDateRange")]
        public async Task<ActionResult<SummaryResult>> GetSummaryResultByDateRange([FromQuery] DateFilterBodyParams dateFilter)
        {

            var summaryResult = await dataStore.FindSummaryResultAsync(dateFilter.StartDate, dateFilter.EndDate);

            // TODO:  Revert to the following usage dataStore connects to an actual database    
            // var summaryResult = await dataStore.SummaryResult.FindAsync(startDate, endDate);

            if ((summaryResult == null) || (summaryResult.Value == null))
            {
                return NotFound();
            }


            return summaryResult;
        }

        // GET: api/SummaryResultByDateRangeAndGroupType
        [HttpGet("SummaryResultByDateRangeAndGroupType")]
        public async Task<ActionResult<SummaryResult>> GetSummaryResultByDateRangeAndGroupType([FromQuery] DateFilterAndGroupTypeBodyParams dateFilter)
        {
            if (dateFilter.GroupType != TransactionGroupTypeProcessor.ExpenseTransGroupType)
            {
                return BadRequest();
            }

            var summaryResult = await dataStore.FindSummaryResultAsync(dateFilter.StartDate, dateFilter.EndDate, dateFilter.GroupType);
            if (summaryResult == null)
            {
                return NotFound();
            }

            return summaryResult;
        }
    }
}
