using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/TransactionSummmary
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SummaryResult>>> GetSummaryResult()
        {
            // var content = TransactionContent.Of(_context);
            // var summaryResult = SummaryResult.Of(content, content.Transactions, miscellaenousGroupType);
            return await _context.SummaryResult.ToListAsync();
        }

        // GET: api/TransactionSummmary/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SummaryResult>> GetSummaryResult(int id)
        {
            var summaryResult = await _context.SummaryResult.FindAsync(id);

            if (summaryResult == null)
            {
                return NotFound();
            }

            return summaryResult;
        }

        // PUT: api/TransactionSummmary/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSummaryResult(int id, SummaryResult summaryResult)
        {
            if (id != summaryResult.Id)
            {
                return BadRequest();
            }

            _context.Entry(summaryResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SummaryResultExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TransactionSummmary
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SummaryResult>> PostSummaryResult(SummaryResult summaryResult)
        {
            _context.SummaryResult.Add(summaryResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSummaryResult", new { id = summaryResult.Id }, summaryResult);
        }

        // DELETE: api/TransactionSummmary/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSummaryResult(int id)
        {
            var summaryResult = await _context.SummaryResult.FindAsync(id);
            if (summaryResult == null)
            {
                return NotFound();
            }

            _context.SummaryResult.Remove(summaryResult);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SummaryResultExists(int id)
        {
            return _context.SummaryResult.Any(e => e.Id == id);
        }
    }
}
