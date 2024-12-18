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
using SteelCutOptimizer.Server.AMPLInstruments;

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class AmplSolverInterface
    {
        private readonly AmplInstrumentsFactory _amplInstrumentFactory = new AmplInstrumentsFactory();
        private AmplResult? lastResult;

        public TestResult SolveWithAMPL(CuttingStockProblemDataDTO problemData)
        {
            var settings = problemData.AlgorithmSettings;
            UniqueID uniqueId = new UniqueID();
            var amplDataConverter = _amplInstrumentFactory.CreateConverter(settings, uniqueId.Get());
            var amplDataValidator = _amplInstrumentFactory.CreateValidator(settings);

            amplDataConverter.AdjustEntryData(problemData);
            string dataFilePath = amplDataConverter.ConvertToAmplDataFile(problemData);

            var amplApiService = _amplInstrumentFactory.CreateApiService(settings);

            Stopwatch sw = Stopwatch.StartNew();
            AmplResult amplResults = amplApiService.SolveCuttingStockProblem(dataFilePath);
            lastResult = amplResults;
            sw.Stop();

            amplDataConverter.DisposeDataFile();
            amplDataValidator.ValidateResultData(amplResults, problemData);

            TestResult testResult = new();
            testResult.Time = sw.ElapsedMilliseconds;
            if(amplResults.TotalCost != null)
                testResult.ObtainedValue = (int)amplResults.TotalCost;
            else if(amplResults.TotalWaste != null)
                testResult.ObtainedValue = (int)amplResults.TotalWaste!;

            testResult.ProblemSize = problemData.OrderList!.Aggregate(0, (x, next) => x + next.Count, sumOfItems => sumOfItems);

            return testResult;
        }

        public AmplResult? GetLastResult()
        {
            return lastResult;
        }
    }
}
