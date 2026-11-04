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

        private async Task<string> CommitChangesAsync(TransactionGroupCfg groupCfg)
        {
            _context.TransactionGroupCfg.Add(groupCfg);
            await _context.SaveChangesAsync();

            _context.WriteDataToFile(_context.ContentFilePath.First().Value, AppCfgProcessor.DefaultGroupTypeCfgFileName);

            return string.Join(",", _context.ToGroupTypeCfgEx().ToTransNamesEx().Select(group => group.GroupName));
        }

        private async Task<string> UpdateGroup(TransactionGroupCfg groupCfg)
        {
            // update existing entries in the groupType Data set...
            var groupsToUpdate = groupCfg.AsCollection().ToArray();

            var groupTypesCfg = _context.ToGroupTypeCfgEx();
            groupTypesCfg.UpdateGroupDataWithDefault(groupsToUpdate).UpdateGroupDataSource(_context);

            return await CommitChangesAsync(groupCfg);
        }

        private TransactionGroupCfg UpdateTransactionNameInGroup(UpdateTransactionName transactionName, Func<IEnumerable<string>, string, IEnumerable<string>> fnUpdater)
        {
            var groupOfInterest = _context.ToGroupTypeCfgEx()
                                          .ToTransNamesEx()
                                          .First(group => group.Id == transactionName.Id);

            return new TransactionGroupCfg()
            {
                Id = groupOfInterest.Id,
                GroupName = groupOfInterest.GroupName,
                GroupType = groupOfInterest.GroupType,
                Names = fnUpdater(groupOfInterest.Names, transactionName.Name).ToArray()
            };
        }


        // GET: api/TransactionGroupType
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionGroupCfg>>> GetTransactionGroupCfg()
        {
            // var content = TransactionContent.Of(_context);

            // var _groups = content.GroupTypeCfg.ToTransNamesEx();

            // var miscGroup = (new TransactionGroupCfg()
            // {
            //     Id = _groups.GenerateUniqueId(cfg => (int)cfg.Id),
            //     GroupName = "misc",
            //     GroupType = "expense",
            //     Names = content.Transactions.ToMiscTransNamesOfInterest(content)
            // }).AsCollection();
            // return _groups.Concat(miscGroup).ToArray();
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

            _ = await CommitChangesAsync(groupCfg);
            return CreatedAtAction("GetTransactionGroupCfg", null);
        }

        // TODO: Move to TransactionNameController
        // PUT: api/TransactionName
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("TransactionName")]
        public async Task<IActionResult> Update([FromBody] UpdateTransactionName transactionName)
        {
            if (transactionName == null)
            {
                return BadRequest();
            }

            Func<IEnumerable<string>, string, IEnumerable<string>> fnUpdater = (names, nameToBeAdded) => names.Concat(nameToBeAdded.AsCollection()).ToArray();
            _ = await UpdateGroup(UpdateTransactionNameInGroup(transactionName, fnUpdater));

            return NoContent();
        }

        // TODO: Move to TransactionNameController
        // DELETE: api/TransactionName
        [HttpDelete("TransactionName")]
        public async Task<IActionResult> Delete([FromBody] UpdateTransactionName transactionName)
        {
            // var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(transactionName);
            // if (transactionGroupCfg == null)
            // {
            //     return NotFound();
            // }

            // _context.TransactionGroupCfg.Remove(transactionGroupCfg);
            // await _context.SaveChangesAsync();
            // return NoContent();


            if (transactionName == null)
            {
                return BadRequest();
            }

            Func<IEnumerable<string>, string, IEnumerable<string>> fnUpdater = (names, nameToBeRemoved) => names.Where(name => name != nameToBeRemoved.ToUpper()).ToArray();
            _ = await UpdateGroup(UpdateTransactionNameInGroup(transactionName, fnUpdater));
            return NoContent();
        }

        // PUT: api/TransactionGroupType/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutTransactionGroupCfg(long id, TransactionGroupCfg transactionGroupCfg)
        // {
        //     if (id != transactionGroupCfg.Id)
        //     {
        //         return BadRequest();
        //     }

        //     _context.Entry(transactionGroupCfg).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!TransactionGroupCfgExists(id))
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


        // DELETE: api/TransactionGroupType/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteTransactionGroupCfg(long id)
        // {
        //     var transactionGroupCfg = await _context.TransactionGroupCfg.FindAsync(id);
        //     if (transactionGroupCfg == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.TransactionGroupCfg.Remove(transactionGroupCfg);
        //     await _context.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool TransactionGroupCfgExists(long id)
        // {
        //     return _context.TransactionGroupCfg.Any(e => e.Id == id);
        // }
    }
}
