using System;
using System.Collections.Generic;
using System.Linq;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Models
{
    public class ExtSrcRef
    {
        public string Type { get; set; }
        public string FilePath { get; set; }

        public static ExtSrcRef Of(string filePath, string type)
        {
            return new ExtSrcRef()
            {
                FilePath = filePath,
                Type = type
            };
        }
    }


    public class SrcData
    {
        public string Type { get; set; }
        public IEnumerable<string> Data { get; set; }

        public static SrcData Of(string fileName, string type, IEnumerable<string> data, DateFilterCfg dateRange)
        {
            return new SrcData()
            {
                Type = type,
                Data = data,
            };
        }
    }

}