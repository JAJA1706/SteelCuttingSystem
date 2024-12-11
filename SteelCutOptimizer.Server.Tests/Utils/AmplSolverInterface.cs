using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Tests.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteelCutOptimizer.Server.Utils;
using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.AmplDataConverters;

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class AmplSolverInterface
    {
        private readonly AmplDataConverterFactory _amplDataConverterFactory = new AmplDataConverterFactory();
        private readonly AmplApiServiceFactory _amplApiServiceFactory = new AmplApiServiceFactory();

        public TestResult SolveWithAMPL(CuttingStockProblemDataDTO problemData)
        {
            var settings = problemData.AlgorithmSettings;
            UniqueID uniqueId = new UniqueID();
            var amplDataConverter = _amplDataConverterFactory.Create(settings, uniqueId.Get());

            amplDataConverter.AdjustEntryData(problemData);
            string dataFilePath = amplDataConverter.ConvertToAmplDataFile(problemData);

            var amplApiService = _amplApiServiceFactory.Create(settings);

            Stopwatch sw = Stopwatch.StartNew();
            AmplResult amplResults = amplApiService.SolveCuttingStockProblem(dataFilePath);
            sw.Stop();

            amplDataConverter.DisposeDataFile();
            amplDataConverter.ValidateResultData(amplResults, problemData);

            TestResult testResult = new();
            testResult.Time = sw.ElapsedMilliseconds;
            testResult.ObtainedValue = (int)amplResults.TotalCost!;
            testResult.ProblemSize = problemData.OrderList!.Aggregate(0, (x, next) => x + next.Count, sumOfItems => sumOfItems);

            return testResult;
        }
    }
}
