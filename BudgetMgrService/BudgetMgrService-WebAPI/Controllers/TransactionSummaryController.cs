using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BudgetMgrServiceLib.Models;
using BudgetMgrService_WebAPI.Models;
using BudgetMgrService_WebAPI.Processors;
using BudgetMgrServiceLib.Generators;
using BudgetMgrServiceLib.Processors;
using Optional;
using System;

namespace BudgetMgrService_WebAPI.Controllers
{
    [Route("api/[controller]")]

    public class TransactionSummaryController : Controller
    {

        private readonly TransactionContext _context;
        private readonly string miscellaenousGroupType = "misc";

        public TransactionSummaryController(TransactionContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var content = TransactionContent.Of(_context);
            return Ok(SummaryResult.Of(content, content.Transactions, miscellaenousGroupType));
        }

        [HttpGet("SummaryByDateRange")]
        public IActionResult SummaryByDateRange([FromQuery(Name = "startdate")] string startDate,
                                                [FromQuery(Name = "enddate")] string endDate)
        {
            var content = TransactionContent.Of(_context);

            return Ok(SummaryResult.Of(content,
                                       content.Transactions.FilterByDateRange(startDate, endDate),
                                       miscellaenousGroupType));
        }

        [HttpGet("TransactionsByGroupType")]
        public IActionResult GetByGroup([FromQuery(Name = "grouptype")] string groupType,
                                        [FromQuery(Name = "startdate")] string startDate,
                                        [FromQuery(Name = "enddate")] string endDate)
        {
            var content = TransactionContent.Of(_context);
            var allTransactions = content.Transactions.FilterByDateRange(startDate, endDate);

            var miscTransNamesOfInterest = allTransactions.ToMiscTransNamesOfInterest(content);

            var expenditureGroupNames = (groupType == miscellaenousGroupType) ? miscTransNamesOfInterest : content.GroupTypeCfg.ToExpenseGroup(groupType).Names.ToArray();

            return Ok(allTransactions.GenTransactionSummaryData(content.IncomeGroupNames, expenditureGroupNames));
        }

        [HttpPost]
        public IActionResult Create([FromServices] IOptions<TestDataCfg> testDataCfgAccessor,
                                    [FromBody] SrcData srcData)
        {
            var results = TransactionGenerator.GenTransactionData(srcData.Data, testDataCfgAccessor.Value,
                                                                  _context.ToGroupTypeCfgEx(),
                                                                  srcData.Type, _context.ApplyUniqueIds);

            _context.Transactions.AddRange(results.Results.RemoveDuplicates(_context.Transactions));
            _context.SaveChanges();

            return Ok(srcData);
        }
    }
}