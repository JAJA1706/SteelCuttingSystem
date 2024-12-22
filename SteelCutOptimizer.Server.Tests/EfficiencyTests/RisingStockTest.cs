using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Utils;
using SteelCutOptimizer.Server.Tests.Structs;
using Xunit.Abstractions;

namespace EfficiencyTests
{
    public class RisingStockTest
    {
        private readonly ITestOutputHelper output;
        public RisingStockTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [EfficiencyFact]
        public void RisingStockSize()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1; //20
            const int BATCH_ITERATIONS = 1; //4

            List<TestResult> amplTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedAmplResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    output.WriteLine($"iteration: {i}; batch: {j}");

                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 20, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, StockCount=i, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100};
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    TestResult amplTestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedAmplResults.Time += amplTestResults.Time / BATCH_ITERATIONS;
                    averagedAmplResults.ObtainedValue += amplTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedAmplResults.ProblemSize = i;
                }

                amplTestResultList.Add(averagedAmplResults);
            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                amplTestResultList,
            };
            List<string> methodNames = new List<string> { "orderC20" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "RisingStockSize");
        }
    }
}
