using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Utils;


namespace EfficiencyTests
{
    public class SolversTest
    {

        [EfficiencyFact]
        public void CbcVSHighsVSCplex()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 1; //10
            const int BATCH_ITERATIONS = 1; //4

            List<TestResult> cbcTestResultList = new List<TestResult>();
            List<TestResult> highsTestResultList = new List<TestResult>();
            List<TestResult> cplexTestResultList = new List<TestResult>();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedCbcResults = new TestResult();
                TestResult averagedHighsResults = new TestResult();
                TestResult averagedCplexResults = new TestResult();
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = 20 + i-1, StockLength = 1200, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
                    CuttingStockProblemDataDTO problemData = problemGen.GenerateProblem(def);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    TestResult cbcTestResults = amplSolver.SolveWithAMPL(problemData);
                    problemData.AlgorithmSettings.Solver = "highs";
                    TestResult highsTestResults = amplSolver.SolveWithAMPL(problemData);
                    problemData.AlgorithmSettings.Solver = "cplex";
                    TestResult cplexTestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedCbcResults.Time += cbcTestResults.Time / BATCH_ITERATIONS;
                    averagedCbcResults.ObtainedValue += cbcTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedCbcResults.ProblemSize = cbcTestResults.ProblemSize;
                    averagedHighsResults.Time += highsTestResults.Time / BATCH_ITERATIONS;
                    averagedHighsResults.ObtainedValue += highsTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedHighsResults.ProblemSize = highsTestResults.ProblemSize;
                    averagedCplexResults.Time += cplexTestResults.Time / BATCH_ITERATIONS;
                    averagedCplexResults.ObtainedValue += cplexTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedCplexResults.ProblemSize = cbcTestResults.ProblemSize;
                }

                cbcTestResultList.Add(averagedCbcResults);
                highsTestResultList.Add(averagedHighsResults);
                cplexTestResultList.Add(averagedCplexResults);
            }

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                cbcTestResultList,
                highsTestResultList,
                cplexTestResultList,
            };
            List<string> methodNames = new List<string> { "cbc", "highs", "cplex" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "SolversComparison");
        }
    }
}
