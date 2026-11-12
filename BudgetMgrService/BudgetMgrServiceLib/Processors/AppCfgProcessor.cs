using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BudgetMgrServiceLib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var fnTransNameToUpper = (TransName transName) => transName with { Name = transName.Name.ToUpper() };
            var cfgData = ToCfgData<GroupTypesCfgEx>(DefaultGroupTypeCfgFileName, "GroupTypesCfgEx", cfgFilePath);
            return new GroupTypesCfgEx(
                cfgData.CreditTypesOfInterest.ToArray(),
                cfgData.DebitTypesOfInterest.ToArray(),
                cfgData.TransNames.Select(fnTransNameToUpper).ToArray(),
                cfgData.TransTypes.ToArray(),
                cfgData.TransGroupTypes.ToArray(),
                cfgData.TransGroups.ToArray(),
                cfgData.NameRefs.ToArray()
            );
        }
    }
}