using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.AmplDataConverters;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Utils;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Utils;
using SteelCutOptimizer.Server.Tests.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTests
{
    public class CostManual
    {
        [EfficiencyFact]
        public void BinpackVSColumnGenerationRelaxRisingSize()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            const int TEST_ITERATIONS = 10;
            const int BATCH_ITERATIONS = 4;

            List<TestResult> simpleTestResultList = new List<TestResult>();
            List<TestResult> manual5TestResultList = new List<TestResult>();
            List<TestResult> manual10TestResultList = new List<TestResult>();
            List<TestResult> autoTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedSimpleResults = new TestResult();
                TestResult averagedManual5Results = new TestResult();
                TestResult averagedManual10Results = new TestResult();
                TestResult averagedAutoResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    int stockCount = 1; var stockSizeRange = (1200, 1200); int minOrderCount = 10; int minOrderSize = 1000; var orderSizeRange = (0.1, 0.8);
                    var problem = CuttingStockProblemGenerator.GenerateProblem(stockCount, stockSizeRange, minOrderCount, minOrderSize, orderSizeRange);
                    CuttingStockProblemDataDTO problemData = problem.Item1;
                    AmplResult optimalResultData = problem.Item2;
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult simpleTestResults = amplSolver.SolveWithAMPL(problemData);

                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    TestResult manual5TestResults = amplSolver.SolveWithAMPL(problemData);
                    RelaxApplier.MultiplyRelax(problemData, 2);
                    TestResult AutoTestResults = amplSolver.

                    //average data
                    averagedAmplResults.Time += amplTestResults.Time / BATCH_ITERATIONS;
                    averagedAmplResults.ObtainedValue += amplTestResults.ObtainedValue / BATCH_ITERATIONS / (int)problemData.StockList!.First().Cost!;
                    averagedAmplResults.ProblemSize = amplTestResults.ProblemSize;
                    averagedBinpackResults.Time += binPackResults.Time / BATCH_ITERATIONS;
                    averagedBinpackResults.ObtainedValue += binPackResults.ObtainedValue / BATCH_ITERATIONS / (int)problemData.StockList!.First().Cost!;
                    averagedBinpackResults.ProblemSize = binPackResults.ProblemSize;
                    averagedAmplRelaxResults.Time += amplTestRelaxResults.Time / BATCH_ITERATIONS;
                    averagedAmplRelaxResults.ObtainedValue += amplTestRelaxResults.ObtainedValue / BATCH_ITERATIONS / (int)problemData.StockList!.First().Cost!;
                    averagedAmplRelaxResults.ProblemSize = amplTestRelaxResults.ProblemSize;
                    averagedBinpackRelaxResults.Time += binPackRelaxResults.Time / BATCH_ITERATIONS;
                    averagedBinpackRelaxResults.ObtainedValue += binPackRelaxResults.ObtainedValue / BATCH_ITERATIONS / (int)problemData.StockList!.First().Cost!;
                    averagedBinpackRelaxResults.ProblemSize = binPackRelaxResults.ProblemSize;
                }

                amplTestResultList.Add(averagedAmplResults);
                binpackTestResultList.Add(averagedBinpackResults);
                amplRelaxTestResultList.Add(averagedAmplRelaxResults);
                binpackRelaxTestResultList.Add(averagedBinpackRelaxResults);
            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                amplTestResultList,
                binpackTestResultList,
                amplRelaxTestResultList,
                binpackRelaxTestResultList,
            };
            List<string> methodNames = new List<string> { "ampl", "binpack", "amplRel", "binpackRel" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "BpVsCgRelRisingSize");
        }

        [EfficiencyFact]
        public void TestManual()
        {
            //arrange
            AmplDataConverterFactory _amplDataConverterFactory = new AmplDataConverterFactory();
            AmplApiServiceFactory _amplApiServiceFactory = new AmplApiServiceFactory();

            

            RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2, true);

            //act
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

            TestResult amplTestResults = new();
            amplTestResults.Time = sw.ElapsedMilliseconds;
            amplTestResults.ObtainedValue = (int)amplResults.TotalCost!;
            amplTestResults.ProblemSize = problemData.OrderList!.Aggregate(0, (x, next) => x + next.Count, sumOfItems => sumOfItems);

            TestResult binPackResults = BinPackSolver.SolveWithBinPack(problemData);

            //assert
            Assert.True(amplResults.Patterns.Count > 0);
        }
    }
}
