using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using BudgetMgrServiceLib.Models;

namespace BudgetMgrServiceLib.Processors
{
    public static class AppCfgProcessor
    {
        public static string DefaultGroupTypeCfgFileName = "grouptypesettings";
        public static double ToTransAmount(this TestDataCfg testDataCfg, int pounds, int pence) => Double.Parse($"{pounds}.{pence}");

        public static int ToYearVal(this Random rnd, TestDataCfg testDataCfg)
        {
            string[] years = testDataCfg.Years.ToArray();
            return Int32.Parse(years[ToRandomSrcVal(rnd, 0, testDataCfg.Years.Count() - 1)]);
        }

        public static int ToRandomSrcVal(this Random rnd, int min, int max) => rnd.Next(min, max);

        public static IEnumerable<string> ToJson<T>(this T value) => new List<string>() { JsonConvert.SerializeObject(value) };

        public static void JsonDataToFile(this IEnumerable<string> jsonData, string cfgFileName, string cfgFilePath = "")
        {
            Func<string> genAppCfgFilePath = () => (cfgFilePath.Count() == 0) ? $"{Directory.GetCurrentDirectory()}\\..\\..\\.." : cfgFilePath;
            File.WriteAllLines($"{genAppCfgFilePath()}\\{cfgFileName}.json", jsonData);
        }

        public static IEnumerable<string> GroupTypesCfgExToJsonData(this GroupTypesCfgEx groupTypeCfg) => (new { GroupTypesCfgEx = groupTypeCfg }).ToJson();

        public static GroupTypesCfgEx GroupTypesCfgExFromJsonData(this IEnumerable<string> jsonData) => string.Join("", jsonData.ToArray()).FromJson<GroupTypesCfgEx>("GroupTypesCfgEx");

        public static void GroupTypesCfgExToFile(this GroupTypesCfgEx groupTypeCfg, string cfgFileName, string cfgFilePath = "") => (new { GroupTypesCfgEx = groupTypeCfg }).ToJson().JsonDataToFile(cfgFileName, cfgFilePath);

        public static T FromJson<T>(this string jsonContent, string sectionName = "AppCfg") => JObject.Parse(jsonContent)[sectionName].ToObject<T>();

        public static T ToCfgData<T>(string cfgFileName, string cfgSectionName, string cfgFilePath = "")
        {
            Func<string> genAppCfgFilePath = () => (cfgFilePath.Count() == 0) ? $"{Directory.GetCurrentDirectory()}\\..\\..\\.." : cfgFilePath;
            return File.ReadAllText($"{genAppCfgFilePath()}\\{cfgFileName}.json").FromJson<T>(cfgSectionName);
        }

        public static TestDataCfg ToTestDataCfg(string cfgFilePath = "") => ToCfgData<TestDataCfg>("testdatasettings", "TestDataCfg", cfgFilePath);

        public static GroupTypesCfgEx ToGroupTypesCfgEx(string cfgFilePath = "")
        {
            var cfgData = ToCfgData<GroupTypesCfgEx>(DefaultGroupTypeCfgFileName, "GroupTypesCfgEx", cfgFilePath);
            return new GroupTypesCfgEx()
            {
                CreditTypesOfInterest = cfgData.CreditTypesOfInterest.ToArray(),
                DebitTypesOfInterest = cfgData.DebitTypesOfInterest.ToArray(),
                TransNames = cfgData.TransNames.Select(transName => transName.Of(transName.Name.ToUpper())).ToArray(),
                TransTypes = cfgData.TransTypes.ToArray(),
                TransGroupTypes = cfgData.TransGroupTypes.ToArray(),
                TransGroups = cfgData.TransGroups.ToArray(),
                NameRefs = cfgData.NameRefs.ToArray()
            };
        }
    }
}