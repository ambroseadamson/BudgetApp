using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BudgetMgrServiceLib.Models;
using BudgetMgrService_WebAPI.Models;
using BudgetMgrService_WebAPI.Processors;
using BudgetMgrServiceLib.Processors;
using System;

namespace BudgetMgrService_WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TransactionGroupTypeController : Controller
    {

        private readonly TransactionContext _context;

        public TransactionGroupTypeController(TransactionContext context)
        {
            _context = context;
        }

        private string CommitChanges()
        {
            _context.SaveChanges();

            _context.WriteDataToFile(_context.ContentFilePath.First().Value, AppCfgProcessor.DefaultGroupTypeCfgFileName);

            return string.Join(",", _context.ToGroupTypeCfgEx().ToTransNamesEx().Select(group => group.GroupName));
        }

        private string UpdateGroup(TransactionGroupCfg groupCfg)
        {
            // update existing entries in the groupType Data set...
            var groupsToUpdate = groupCfg.AsCollection().ToArray();

            var groupTypesCfg = _context.ToGroupTypeCfgEx();
            groupTypesCfg.UpdateGroupDataWithDefault(groupsToUpdate).UpdateGroupDataSource(_context);

            return CommitChanges();
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

        [HttpGet]
        public IEnumerable<TransactionGroupCfg> Get()
        {
            var content = TransactionContent.Of(_context);

            var _groups = content.GroupTypeCfg.ToTransNamesEx();

            var miscGroup = (new TransactionGroupCfg()
            {
                Id = _groups.GenerateUniqueId(cfg => (int)cfg.Id),
                GroupName = "misc",
                GroupType = "expense",
                Names = content.Transactions.ToMiscTransNamesOfInterest(content)
            }).AsCollection();
            return _groups.Concat(miscGroup).ToArray();
        }

        [HttpPost]
        public IActionResult Create([FromBody] TransactionGroupCfg groupCfg)
        {
            if (groupCfg == null)
            {
                return BadRequest();
            }

            var _groupNames = CommitChanges();
            return new ObjectResult(new { GroupNames = _groupNames, GroupTypes = _context.ToGroupTypeCfgEx().ToTransNamesEx().ToArray() });
        }

        [HttpPut("TransactionName")]
        public IActionResult Update([FromBody] UpdateTransactionName transactionName)
        {
            if (transactionName == null)
            {
                return BadRequest();
            }

            Func<IEnumerable<string>, string, IEnumerable<string>> fnUpdater = (names, nameToBeAdded) => names.Concat(nameToBeAdded.AsCollection()).ToArray();
            var _groupNames = UpdateGroup(UpdateTransactionNameInGroup(transactionName, fnUpdater));
            return new ObjectResult(new { GroupNames = _groupNames, GroupTypes = _context.ToGroupTypeCfgEx().ToTransNamesEx().ToArray() });
        }

        [HttpDelete("TransactionName")]
        public IActionResult Delete([FromBody] UpdateTransactionName transactionName)
        {
            if (transactionName == null)
            {
                return BadRequest();
            }

            Func<IEnumerable<string>, string, IEnumerable<string>> fnUpdater = (names, nameToBeRemoved) => names.Where(name => name != nameToBeRemoved.ToUpper()).ToArray();
            var _groupNames = UpdateGroup(UpdateTransactionNameInGroup(transactionName, fnUpdater));
            return new ObjectResult(new { GroupNames = _groupNames, GroupTypes = _context.ToGroupTypeCfgEx().ToTransNamesEx().ToArray() });
        }
    }
}