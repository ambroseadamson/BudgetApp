using BudgetMgrServiceLib.Models;
using BudgetMgrServiceLib.Processors;
using BudgetMgrWebService.Processors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            // NOTE: The following usage context connects to an in-memory database, which is used for testing purposes.
            // The actual database connection is commented out below.
            // This API method retrieves the complete data set of TransactionGroupCfgs in the datastore,
            // and can be used by the client to update the group data source with the complete
            return await _context.ToTransactionGroupCfgsAsync();

            // TODO:  Revert to the following usage context connects to an actual database    
            //return await _context.TransactionGroupCfg.ToListAsync();
        }


        // POST: api/TransactionGroupType
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TransactionGroupCfg[] groupCfgs)
        {
            // NOTE: The following usage context connects to an in-memory database, which is used for testing purposes.
            // This API method updates the complete data set of TransactionGroupCfgs in the database with the data
            // set recieved from the client.
            if (groupCfgs == null)
            {
                return BadRequest();
            }

            var content = TransactionContent.Of(_context);
            _context.UpdateTransactionGroupCfg(groupCfgs);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
