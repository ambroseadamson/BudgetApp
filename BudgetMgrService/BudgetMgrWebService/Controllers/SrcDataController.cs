using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Models;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SrcDataController : ControllerBase
    {
        private readonly TransactionContext _context;

        public SrcDataController(TransactionContext context)
        {
            _context = context;
        }

        // POST: api/SrcData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SummaryResult>> Post([FromServices] IOptions<TestDataCfg> testDataCfgAccessor,
                                                            [FromBody] SrcData srcData)
        {
            // TODO: See testSrcData.json for the format of the data that is expected to be posted to this endpoint.
            // NOTE: SrcData data type will change to meet the requirements of the national bank statement data that will be posted to this endpoint.
            var results = srcData.Data.GenTransactionData(testDataCfgAccessor.Value, _context.ToGroupTypeCfgEx(), _context.ApplyUniqueIds);

            _context.Transactions.AddRange(results.Results.RemoveDuplicates(_context.Transactions));

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
