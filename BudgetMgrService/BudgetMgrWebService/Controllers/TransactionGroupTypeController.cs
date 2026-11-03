using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionGroupTypeController : ControllerBase
    {
        private readonly TransactionContext _context;

        public TransactionGroupTypeController(TransactionContext context)
        {
            _context = context;
        }

        // GET: api/TransactionGroupType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionGroupCfg>>> GetTransactionGroupCfg()
        {
            return await _context.TransactionGroupCfg.ToListAsync();
        }

        // GET: api/TransactionGroupType/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionGroupCfg>> GetTransactionGroupCfg(long id)
        {
            var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(id);

            if (transactionGroupCfg == null)
            {
                return NotFound();
            }

            return transactionGroupCfg;
        }

        // PUT: api/TransactionGroupType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransactionGroupCfg(long id, TransactionGroupCfg transactionGroupCfg)
        {
            if (id != transactionGroupCfg.Id)
            {
                return BadRequest();
            }

            _context.Entry(transactionGroupCfg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionGroupCfgExists(id))
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

        // POST: api/TransactionGroupType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransactionGroupCfg>> PostTransactionGroupCfg(TransactionGroupCfg transactionGroupCfg)
        {
            _context.TransactionGroupCfg.Add(transactionGroupCfg);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTransactionGroupCfg", new { id = transactionGroupCfg.Id }, transactionGroupCfg);
        }

        // DELETE: api/TransactionGroupType/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionGroupCfg(long id)
        {
            var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(id);
            if (transactionGroupCfg == null)
            {
                return NotFound();
            }

            _context.TransactionGroupCfg.Remove(transactionGroupCfg);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TransactionGroupCfgExists(long id)
        {
            return _context.TransactionGroupCfg.Any(e => e.Id == id);
        }
    }
}
