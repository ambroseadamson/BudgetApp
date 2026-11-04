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

        // TODO: Move to SummaryResultController
        // GET: api/SummaryResult
        [HttpGet]
        public async Task<ActionResult<SummaryResult>> GetSummaryResult()
        {
            var content = TransactionContent.Of(_context);
            return Ok(SummaryResult.Of(content, content.Transactions, miscellaenousGroupType));
//            return await _context.SummaryResult.ToListAsync();
        }


        // TODO: Move to SummaryResultController
        // GET: api/SummaryResult
        [HttpGet("SummaryResultByDateRange")]
        public Task<ActionResult<SummaryResult>> GetSummaryResultByDateRange([FromQuery(Name = "startdate")] string startDate,
                                                                             [FromQuery(Name = "enddate")] string endDate)
        {
            // var summaryResult = await _context.SummaryResult.FindAsync(id);

            // if (summaryResult == null)
            // {
            //     return NotFound();
            // }

            // return summaryResult;

            var content = TransactionContent.Of(_context);
            var transactions = await content.Transactions.FindByDateRangeAsync(startDate, endDate);
            if ((transactions == null) || (transactions.Count() == 0))
            {
                return NotFound();
            }
            return Ok(SummaryResult.Of(content, transactions, miscellaenousGroupType));
        }

        // POST: api/TransactionSummary
        [HttpGet("TransactionsByGroupType")]
        public Task<ActionResult<TransactionSummary>> GetByGroup([FromQuery(Name = "grouptype")] string groupType,
                                                                 [FromQuery(Name = "startdate")] string startDate,
                                                                 [FromQuery(Name = "enddate")] string endDate)
        {
            var content = TransactionContent.Of(_context);
            var transactions = await content.Transactions.FindByDateRangeAsync(startDate, endDate);
            if ((transactions == null) || (transactions.Count() == 0))
            {
                return NotFound();
            }

            var miscTransNamesOfInterest = transactions.ToMiscTransNamesOfInterest(content);

            var expenditureGroupNames = (groupType == miscellaenousGroupType) 
                                        ? miscTransNamesOfInterest 
                                        : content.GroupTypeCfg.ToExpenseGroup(groupType).Names.ToArray();

            return Ok(transactions.GenTransactionSummaryData(content.IncomeGroupNames, expenditureGroupNames));
        }

        // // PUT: api/TransactionSummmary/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutSummaryResult(int id, SummaryResult summaryResult)
        // {
        //     if (id != summaryResult.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(summaryResult).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!SummaryResultExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }


        // TODO: Move to SrcDataController
        // POST: api/SrcData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SrcData>> Create([FromServices] IOptions<TestDataCfg> testDataCfgAccessor,
                                                        [FromBody] SrcData srcData)
        {
            var results = TransactionGenerator.GenTransactionData(srcData.Data, testDataCfgAccessor.Value,
                                                                  _context.ToGroupTypeCfgEx(),
                                                                  srcData.Type, _context.ApplyUniqueIds);

            _context.Transactions.AddRange(results.Results.RemoveDuplicates(_context.Transactions));
            await _context.SaveChangesAsync();

            return Ok(srcData);
            // return CreatedAtAction("GetSummaryResult", new { id = summaryResult.Id }, summaryResult);
        }

        // POST: api/TransactionSummmary
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<SummaryResult>> PostSummaryResult(SummaryResult summaryResult)
        // {
        //     _context.SummaryResult.Add(summaryResult);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetSummaryResult", new { id = summaryResult.Id }, summaryResult);
        // }

        // DELETE: api/TransactionSummmary/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteSummaryResult(int id)
        // {
        //     var summaryResult = await _context.SummaryResult.FindAsync(id);
        //     if (summaryResult == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.SummaryResult.Remove(summaryResult);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        private bool SummaryResultExists(int id)
        {
            return _context.SummaryResult.Any(e => e.Id == id);
        }
    }
}
