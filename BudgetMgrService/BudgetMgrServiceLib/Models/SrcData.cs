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
        public IEnumerable<string> Data { get; set; }

        public static SrcData Of(IEnumerable<string> data)
        {
            return new SrcData()
            {
                Data = data,
            };
        }
    }

}