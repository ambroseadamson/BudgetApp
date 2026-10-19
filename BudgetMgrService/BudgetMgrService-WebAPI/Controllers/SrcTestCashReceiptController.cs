using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using BudgetMgrServiceLib.Models;
using BudgetMgrService_WebAPI.Models;
using BudgetMgrService_WebAPI.Processors;
using BudgetMgrServiceLib.Processors;
using BudgetMgrServiceLib.Generators;

namespace BudgetMgrService_WebAPI.Controllers
{
    [Route("api/[controller]")]

    public class SrcTestCashReceiptController : Controller
    {
        private readonly TransactionContext _context;
        public SrcTestCashReceiptController(TransactionContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IOptions<TestDataCfg> testDataCfgAccessor)
        {
            return new ObjectResult(testDataCfgAccessor.Value.GenCashReceiptData(_context.ToGroupTypeCfgEx(), DateTime.Now, new Random()));
        }
    }
}