using SteelCutOptimizer.Server.Tests.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelCutOptimizer.Server.Tests.Utils
{


    internal class TestResultSaver
    {
        public static void SaveToCSV( List<TestResult> results, string methodName)
        {
            if (results == null || results.Count == 0)
                throw new ArgumentException("The testResults list cannot be null or empty.");

            var sb = new StringBuilder();

            sb.AppendLine("Time;Value;ProblemSize");

            foreach (var result in results)
            {
                sb.AppendLine($"{result.Time};{result.ObtainedValue};{result.ProblemSize}");
            }

            string dirPath = AppDomain.CurrentDomain.BaseDirectory + "TestResults/";
            string filePath = dirPath + methodName + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(filePath, sb.ToString());
        }

        public static void SaveToCSVCombined(List<List<TestResult>> results, List<string> methodNames, string csvName)
        {
            if (results.Count != methodNames.Count)
                throw new ArgumentException("every results list needs corresponding name");

            int testIterationCount = results.First().Count;
            foreach (var testResult in results)
            {
                if (testResult.Count != testIterationCount)
                    throw new ArgumentException("every test needs to be iterated the same amount of times");
            }

            var sb = new StringBuilder();

            foreach(var method in methodNames)
            {
                sb.Append($"{method}_Time;{method}_Value;");
            }
            sb.AppendLine("ProblemSize");

            for(int i = 0; i < testIterationCount; ++i)
            {
                for (int j = 0; j < results.Count; ++j)
                {
                    sb.Append($"{results[j][i].Time};{results[j][i].ObtainedValue};");
                }
                sb.AppendLine($"{results[0][i].ProblemSize}");
            }

            string dirPath = AppDomain.CurrentDomain.BaseDirectory + "TestResults/";
            string filePath = dirPath + csvName + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllText(filePath, sb.ToString());
        }
    }
}
