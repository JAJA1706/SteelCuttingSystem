using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Utils;


namespace EfficiencyTests
{
    public class SingleKnapsackIterationTests
    {
        [EfficiencyFact]
        public void RisingStockSingleKnapsackIteration()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1; //20
            const int BATCH_ITERATIONS = 1; //4

            List<TestResult> testResultList = new List<TestResult>();
            List<TestResult> skTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedResults = new TestResult();
                TestResult averagedSKResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 20, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, StockCount = i, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.MainObjective = "waste";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    TestResult testResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.AlgorithmSettings.RelaxationType = "manualFast";
                    TestResult skTestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedResults.Time += testResults.Time / BATCH_ITERATIONS;
                    averagedResults.ObtainedValue += testResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedResults.ProblemSize = i;
                    averagedSKResults.Time += skTestResults.Time / BATCH_ITERATIONS;
                    averagedSKResults.ObtainedValue += skTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedSKResults.ProblemSize = i;
                }

                testResultList.Add(averagedResults);
                skTestResultList.Add(averagedSKResults);

            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                testResultList,
                skTestResultList,
            };
            List<string> methodNames = new List<string> { "simple_OC20", "sk_OC20" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "RisingStockSingleKnapsack");
        }
    }
}
