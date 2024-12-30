using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Utils;
using SteelCutOptimizer.Server.Tests.Structs;

namespace EfficiencyTests
{
    public class OptimalityTest
    {
        [EfficiencyFact]
        public void OptimalValuesTest()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            CuttingStockProblemGenerator gpcs = new CuttingStockProblemGenerator();
            const int TEST_ITERATIONS = 1; //18
            const int BATCH_ITERATIONS = 1; //3

            CSVColumn simpleTimeCol = new CSVColumn { columnName = "simple_time" };
            CSVColumn simpleValCol = new CSVColumn { columnName = "simple_val" };
            CSVColumn manual5TimeCol = new CSVColumn { columnName = "manual5_time" };
            CSVColumn manual5ValCol = new CSVColumn { columnName = "manual5_val" };
            CSVColumn manual10TimeCol = new CSVColumn { columnName = "manual10_time" };
            CSVColumn manual10ValCol = new CSVColumn { columnName = "manual10_val" };
            CSVColumn autoTimeCol = new CSVColumn { columnName = "auto_time" };
            CSVColumn autoValCol = new CSVColumn { columnName = "auto_val" };
            CSVColumn optimalCol = new CSVColumn { columnName = "optimal" };
            CSVColumn problemSizeCol = new CSVColumn { columnName = "problem_size" };
            CSVColumn stockCountCol = new CSVColumn { columnName = "stock_count" };
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedSimpleResults = new TestResult();
                TestResult averagedManual5Results = new TestResult();
                TestResult averagedManual10Results = new TestResult();
                TestResult averagedAutoResults = new TestResult();
                double optimalValAveraged = 0;
                int problemSize = 0;
                int stockCount = 0;
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    stockCount = (int)Math.Ceiling((i / 6.0)); 
                    int orderCount = 10 + ((i - 1) % 6) * 2;
                    var def = new ProblemGenerationDefinition { OrderCount = orderCount, StockCount = stockCount, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, OrderLengthLowerBound = 0.1, OrderLengthUpperBound = 0.8, AverageDemand = 1000 / orderCount };
                    var problem = gpcs.GenerateProblem(def);
                    CuttingStockProblemDataDTO problemData = problem.Item1;
                    AmplResult optimalResultData = problem.Item2;
                    problemData.AlgorithmSettings.RelaxationType = "none";

                    //act
                    TestResult simpleTestResults = amplSolver.SolveWithAMPL(problemData);

                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    TestResult manual5TestResults = amplSolver.SolveWithAMPL(problemData);

                    RelaxApplier.MultiplyRelax(problemData, 2);
                    TestResult manual10TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.AlgorithmSettings.RelaxationType = "auto";
                    TestResult autoTestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedSimpleResults.Time += simpleTestResults.Time / BATCH_ITERATIONS;
                    averagedSimpleResults.ObtainedValue += simpleTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedManual5Results.Time += manual5TestResults.Time / BATCH_ITERATIONS;
                    averagedManual5Results.ObtainedValue += manual5TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedManual10Results.Time += manual10TestResults.Time / BATCH_ITERATIONS;
                    averagedManual10Results.ObtainedValue += manual10TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedAutoResults.Time += autoTestResults.Time / BATCH_ITERATIONS;
                    averagedAutoResults.ObtainedValue += autoTestResults.ObtainedValue / BATCH_ITERATIONS;
                    optimalValAveraged += (double)optimalResultData.TotalCost! / BATCH_ITERATIONS;
                    problemSize = problemData.OrderList!.Count;
                }


                simpleTimeCol.values.Add(averagedSimpleResults.Time.ToString());
                simpleValCol.values.Add(averagedSimpleResults.ObtainedValue.ToString());
                manual5TimeCol.values.Add(averagedManual5Results.Time.ToString());
                manual5ValCol.values.Add(averagedManual5Results.ObtainedValue.ToString());
                manual10TimeCol.values.Add(averagedManual10Results.Time.ToString());
                manual10ValCol.values.Add(averagedManual10Results.ObtainedValue.ToString());
                autoTimeCol.values.Add(averagedAutoResults.Time.ToString());
                autoValCol.values.Add(averagedAutoResults.ObtainedValue.ToString());
                optimalCol.values.Add(optimalValAveraged.ToString());
                problemSizeCol.values.Add(problemSize.ToString());
                stockCountCol.values.Add(stockCount.ToString());
            }

            //save to file
            List<CSVColumn> combinedResults = new()
            {
               simpleTimeCol,
               simpleValCol,
               manual5TimeCol,
               manual5ValCol,
               manual10TimeCol,
               manual10ValCol,
               autoTimeCol,
               autoValCol,
               optimalCol,
               problemSizeCol,
               stockCountCol,
            };
            TestResultSaver.SaveToCSVCombined(combinedResults, "TestWithOptimalVals");
        }
    }
}
