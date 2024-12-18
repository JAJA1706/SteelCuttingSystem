using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Utils;
using SteelCutOptimizer.Server.Tests.Utils;
using System.Diagnostics;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Attributes;
using Xunit.Abstractions;
using SteelCutOptimizer.Server.AMPLInstruments;

namespace EfficiencyTests
{
    public class BinPackVsCgTest
    {
        private readonly ITestOutputHelper output;
        public BinPackVsCgTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [EfficiencyFact]
        public void BinpackVSColumnGenerationRelaxRisingDemand()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1; //20
            const int BATCH_ITERATIONS = 1; //4

            List<TestResult> amplTestResultList = new List<TestResult>();
            List<TestResult> binpackTestResultList = new List<TestResult>();
            List<TestResult> amplRelaxTestResultList = new List<TestResult>();
            List<TestResult> binpackRelaxTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedAmplResults = new TestResult();
                TestResult averagedBinpackResults = new TestResult();
                TestResult averagedAmplRelaxResults = new TestResult();
                TestResult averagedBinpackRelaxResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 10, StockLength = 1200, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 500 * i };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult amplTestResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackResults = BinPackSolver.SolveWithBinPack(problemData);

                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    TestResult amplTestRelaxResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackRelaxResults = BinPackSolver.SolveWithBinPack(problemData);

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
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "BpVsCgRelRisingDemand");
        }

        [EfficiencyFact]
        public void BinpackVSColumnGenerationRelaxRisingSize()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1; //16
            const int BATCH_ITERATIONS = 1; //4

            List<TestResult> amplTestResultList = new List<TestResult>();
            List<TestResult> binpackTestResultList = new List<TestResult>();
            List<TestResult> amplRelaxTestResultList = new List<TestResult>();
            List<TestResult> binpackRelaxTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedAmplResults = new TestResult();
                TestResult averagedBinpackResults = new TestResult();
                TestResult averagedAmplRelaxResults = new TestResult();
                TestResult averagedBinpackRelaxResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 5 * i, StockLength = 1200, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult amplTestResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackResults = BinPackSolver.SolveWithBinPack(problemData);

                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    TestResult amplTestRelaxResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackRelaxResults = BinPackSolver.SolveWithBinPack(problemData);

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


        ///
        ///!!!
        ///OBSOLETE TESTS ONLY FOR DEBUG PURPOSES
        ///!!!
        ///

        [EfficiencyFact(Skip = "Obsolete test")]
        public void BinpackVSColumnGenerationRisingDemand()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1;
            const int BATCH_ITERATIONS = 1;

            List<TestResult> amplTestResultList = new List<TestResult>();
            List<TestResult> binpackTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedAmplResults = new TestResult();
                TestResult averagedBinpackResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 10, StockLength = 1200, OrderLengthLowerBound = 0.3, OrderLengthUpperBound = 0.8, AverageDemand = 500 * i };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult amplTestResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackResults = BinPackSolver.SolveWithBinPack(problemData);

                    //average data
                    averagedAmplResults.Time += amplTestResults.Time / BATCH_ITERATIONS;
                    averagedAmplResults.ObtainedValue += amplTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedAmplResults.ProblemSize = amplTestResults.ProblemSize;
                    averagedBinpackResults.Time += binPackResults.Time / BATCH_ITERATIONS;
                    averagedBinpackResults.ObtainedValue += binPackResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedBinpackResults.ProblemSize = binPackResults.ProblemSize;
                }

                amplTestResultList.Add(averagedAmplResults);
                binpackTestResultList.Add(averagedBinpackResults);
            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                amplTestResultList,
                binpackTestResultList
            };
            List<string> methodNames = new List<string> { "ampl", "binpack" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "BpVsCgRisingDemand");
        }

        [EfficiencyFact(Skip = "Obsolete test")]
        public void BinpackVSColumnGenerationRisingSize()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1;
            const int BATCH_ITERATIONS = 1;

            List<TestResult> amplTestResultList = new List<TestResult>();
            List<TestResult> binpackTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedAmplResults = new TestResult();
                TestResult averagedBinpackResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 5 * i, StockLength = 1200, OrderLengthLowerBound = 0.3, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult amplTestResults = amplSolver.SolveWithAMPL(problemData);
                    TestResult binPackResults = BinPackSolver.SolveWithBinPack(problemData);

                    //average data
                    averagedAmplResults.Time += amplTestResults.Time / BATCH_ITERATIONS;
                    averagedAmplResults.ObtainedValue += amplTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedAmplResults.ProblemSize = amplTestResults.ProblemSize;
                    averagedBinpackResults.Time += binPackResults.Time / BATCH_ITERATIONS;
                    averagedBinpackResults.ObtainedValue += binPackResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedBinpackResults.ProblemSize = binPackResults.ProblemSize;
                }

                amplTestResultList.Add(averagedAmplResults);
                binpackTestResultList.Add(averagedBinpackResults);
            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                amplTestResultList,
                binpackTestResultList
            };
            List<string> methodNames = new List<string> { "ampl", "binpack" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "BpVsCgRisingSize");
        }


        [EfficiencyFact(Skip = "Obsolete test")]
        public void TestManual()
        {
            //arrange
            AmplInstrumentsFactory _amplInstrumentsFactory = new AmplInstrumentsFactory();

            List<StockItem> stockItems = new List<StockItem>()
            {
                new StockItem { Length = 100, Cost = 100}
            };
            List<OrderItem> orderItems = new List<OrderItem>()
            {
                new OrderItem { Length = 70, Count = 400},
                new OrderItem { Length = 30, Count = 300},
                new OrderItem { Length = 37, Count = 800},
                new OrderItem { Length = 19, Count = 200},
            };

            var problemData = new CuttingStockProblemDataDTO { StockList = stockItems, OrderList = orderItems, AlgorithmSettings = new() };
            RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);
            AmplResult optimalResultData = new AmplResult();

            //act
            var settings = problemData.AlgorithmSettings;
            UniqueID uniqueId = new UniqueID();
            var amplDataConverter = _amplInstrumentsFactory.CreateConverter(settings, uniqueId.Get());
            var amplDataValidator = _amplInstrumentsFactory.CreateValidator(settings);

            amplDataConverter.AdjustEntryData(problemData);
            string dataFilePath = amplDataConverter.ConvertToAmplDataFile(problemData);

            var amplApiService = _amplInstrumentsFactory.CreateApiService(settings);

            Stopwatch sw = Stopwatch.StartNew();
            AmplResult amplResults = amplApiService.SolveCuttingStockProblem(dataFilePath);
            sw.Stop();

            amplDataConverter.DisposeDataFile();
            amplDataValidator.ValidateResultData(amplResults, problemData);

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