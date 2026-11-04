using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Processors;

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


        // POST: api/TransactionGroupType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TransactionGroupCfg>> Create([FromBody] TransactionGroupCfg groupCfg)
        {
            if (groupCfg == null)
            {
                return BadRequest();
            }

            _ = await _context.CommitChangesAsync(groupCfg);
            return CreatedAtAction("GetTransactionGroupCfg", null);
        }

        // PUT: api/TransactionGroupCfg
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("TransactionGroupCfg")]
        public async Task<IActionResult> PutTransactionGroupCfg([FromBody] UpdateTransactionName transactionName)
        {

            var transactionGroupCfg = await _context.FindTransactionGroupCfgAsync(transactionName);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(transactionName);
            if (transactionGroupCfg == null)
            {
                return NotFound();
            }


            _context.UpdateTransactionGroupCfg(transactionGroupCfg);

            // TODO:  Revert to the following usage context connects to an actaul database    
            //_context.TransactionGroupCfg.Update(transactionGroupCfg);
            _context.Entry(transactionGroupCfg).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransactionGroupCfgExists(transactionGroupCfg.Id))
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

        // DELETE: api/TransactionGroupCfg
        [HttpDelete("TransactionGroupCfg")]
        public async Task<IActionResult> DeleteTransactionGroupCfg([FromBody] UpdateTransactionName transactionName)
        {
            var transactionGroupCfg = await _context.FindTransactionGroupCfgAsync(transactionName);

            // TODO:  Revert to the following usage context connects to an actaul database    
            // var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(transactionName);
            if (transactionGroupCfg == null)
            {
                return NotFound();
            }


            _context.RemoveTransactionGroupCfg(transactionGroupCfg);

            // TODO:  Revert to the following usage context connects to an actaul database    
            //_context.TransactionGroupCfg.Remove(transactionGroupCfg);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool TransactionGroupCfgExists(long id)
        {
            return _context.TransactionGroupCfg.Any(e => e.Id == id);
        }
    }
}
