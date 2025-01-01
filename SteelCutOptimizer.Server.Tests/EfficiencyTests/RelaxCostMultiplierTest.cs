using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Utils;
using SteelCutOptimizer.Server.Tests.Structs;
using Xunit.Abstractions;
using SteelCutOptimizer.Server.Structs;
using Newtonsoft.Json.Linq;

namespace EfficiencyTests
{
    public class RelaxCostMultiplierTest
    {
        [EfficiencyFact]
        public void DifferentMultipliersManual()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            CuttingStockProblemGenerator gpcs = new CuttingStockProblemGenerator();
            const int TEST_ITERATIONS = 1; //18
            const int BATCH_ITERATIONS = 1; //3

            CSVColumn valDefaultCol = new CSVColumn { columnName = "default_val" };
            CSVColumn val20Col = new CSVColumn { columnName = "20%_val" };
            CSVColumn val40Col = new CSVColumn { columnName = "40%_val" };
            CSVColumn val60Col = new CSVColumn { columnName = "60%_val" };
            CSVColumn val80Col = new CSVColumn { columnName = "80%_val" };
            CSVColumn val100Col = new CSVColumn { columnName = "100%_val" };
            CSVColumn optimalCol = new CSVColumn { columnName = "optimal" };
            CSVColumn orderCountCol = new CSVColumn { columnName = "order_count" };
            CSVColumn stockCountCol = new CSVColumn { columnName = "stock_count" };
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedMultiDefaultResults = new TestResult();
                TestResult averagedMulti20Results = new TestResult();
                TestResult averagedMulti40Results = new TestResult();
                TestResult averagedMulti60Results = new TestResult();
                TestResult averagedMulti80Results = new TestResult();
                TestResult averagedMulti100Results = new TestResult();
                int optimalValAveraged = 0;
                int stockCount = (int)Math.Ceiling((i / 6.0));
                int orderCount = 10 + ((i - 1) % 6) * 2;
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = orderCount, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, StockCount = stockCount, OrderLengthLowerBound = 0.1, OrderLengthUpperBound = 0.8, AverageDemand = 1000 / orderCount };
                    var problem = gpcs.GenerateProblem(def);
                    CuttingStockProblemDataDTO problemData = problem.Item1;
                    AmplResult optimalResultData = problem.Item2;
                    problemData.AlgorithmSettings.RelaxationType = "manual";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    TestResult defaultTestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.2));
                    TestResult Mutli20TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.4));
                    TestResult Mutli40TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.6));
                    TestResult Mutli60TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.8));
                    TestResult Mutli80TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = 100;
                    TestResult Mutli100TestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedMultiDefaultResults.ObtainedValue += defaultTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti20Results.ObtainedValue += Mutli20TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti40Results.ObtainedValue += Mutli40TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti60Results.ObtainedValue += Mutli60TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti80Results.ObtainedValue += Mutli80TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti100Results.ObtainedValue += Mutli100TestResults.ObtainedValue / BATCH_ITERATIONS;
                    optimalValAveraged += (int)optimalResultData.TotalCost! / BATCH_ITERATIONS;
                }

                valDefaultCol.values.Add(averagedMultiDefaultResults.ObtainedValue.ToString());
                val20Col.values.Add(averagedMulti20Results.ObtainedValue.ToString());
                val40Col.values.Add(averagedMulti40Results.ObtainedValue.ToString());
                val60Col.values.Add(averagedMulti60Results.ObtainedValue.ToString());
                val80Col.values.Add(averagedMulti80Results.ObtainedValue.ToString());
                val100Col.values.Add(averagedMulti100Results.ObtainedValue.ToString());
                optimalCol.values.Add(optimalValAveraged.ToString());
                orderCountCol.values.Add(orderCount.ToString());
                stockCountCol.values.Add(stockCount.ToString());
            }

            //save to file
            List<CSVColumn> combinedResults = new()
            {
               valDefaultCol,
               val20Col,
               val40Col,
               val60Col,
               val80Col,
               val100Col,
               optimalCol,
               orderCountCol,
               stockCountCol,
            };
            TestResultSaver.SaveToCSVCombined(combinedResults, "RelaxCostMultiplierManual");
        }

        [EfficiencyFact]
        public void DifferentMultipliersAuto()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            CuttingStockProblemGenerator gpcs = new CuttingStockProblemGenerator();
            const int TEST_ITERATIONS = 1; //18
            const int BATCH_ITERATIONS = 1; //3

            CSVColumn valDefaultCol = new CSVColumn { columnName = "default_val" };
            CSVColumn val20Col = new CSVColumn { columnName = "20%_val" };
            CSVColumn val40Col = new CSVColumn { columnName = "40%_val" };
            CSVColumn val60Col = new CSVColumn { columnName = "60%_val" };
            CSVColumn val80Col = new CSVColumn { columnName = "80%_val" };
            CSVColumn val100Col = new CSVColumn { columnName = "100%_val" };
            CSVColumn optimalCol = new CSVColumn { columnName = "optimal" };
            CSVColumn orderCountCol = new CSVColumn { columnName = "order_count" };
            CSVColumn stockCountCol = new CSVColumn { columnName = "stock_count" };
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedMultiDefaultResults = new TestResult();
                TestResult averagedMulti20Results = new TestResult();
                TestResult averagedMulti40Results = new TestResult();
                TestResult averagedMulti60Results = new TestResult();
                TestResult averagedMulti80Results = new TestResult();
                TestResult averagedMulti100Results = new TestResult();
                int optimalValAveraged = 0;
                int stockCount = (int)Math.Ceiling((i / 6.0));
                int orderCount = 10 + ((i - 1) % 6) * 2;
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = orderCount, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, StockCount=stockCount, OrderLengthLowerBound = 0.1, OrderLengthUpperBound = 0.8, AverageDemand = 1000 / orderCount };
                    var problem = gpcs.GenerateProblem(def);
                    CuttingStockProblemDataDTO problemData = problem.Item1;
                    AmplResult optimalResultData = problem.Item2;
                    problemData.AlgorithmSettings.RelaxationType = "auto";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    TestResult defaultTestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.2));
                    TestResult Mutli20TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.4));
                    TestResult Mutli40TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.6));
                    TestResult Mutli60TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = (1 / (1 - 0.8));
                    TestResult Mutli80TestResults = amplSolver.SolveWithAMPL(problemData);

                    problemData.RelaxCostMultiplier = 100;
                    TestResult Mutli100TestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedMultiDefaultResults.ObtainedValue += defaultTestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti20Results.ObtainedValue += Mutli20TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti40Results.ObtainedValue += Mutli40TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti60Results.ObtainedValue += Mutli60TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti80Results.ObtainedValue += Mutli80TestResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedMulti100Results.ObtainedValue += Mutli100TestResults.ObtainedValue / BATCH_ITERATIONS;
                    optimalValAveraged += (int)optimalResultData.TotalCost! / BATCH_ITERATIONS;
                }

                valDefaultCol.values.Add(averagedMultiDefaultResults.ObtainedValue.ToString());
                val20Col.values.Add(averagedMulti20Results.ObtainedValue.ToString());
                val40Col.values.Add(averagedMulti40Results.ObtainedValue.ToString());
                val60Col.values.Add(averagedMulti60Results.ObtainedValue.ToString());
                val80Col.values.Add(averagedMulti80Results.ObtainedValue.ToString());
                val100Col.values.Add(averagedMulti100Results.ObtainedValue.ToString());
                optimalCol.values.Add(optimalValAveraged.ToString());
                orderCountCol.values.Add(orderCount.ToString());
                stockCountCol.values.Add(stockCount.ToString());
            }

            //save to file
            List<CSVColumn> combinedResults = new()
            {
               valDefaultCol,
               val20Col,
               val40Col,
               val60Col,
               val80Col,
               val100Col,
               optimalCol,
               orderCountCol,
               stockCountCol,
            };
            TestResultSaver.SaveToCSVCombined(combinedResults, "RelaxCostMultiplierAuto");
        }

        [EfficiencyFact]
        public void MaxMultiplierHiGHS()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            CuttingStockProblemGenerator gpcs = new CuttingStockProblemGenerator();
            const int TEST_ITERATIONS = 1; //18
            const int BATCH_ITERATIONS = 1; //3

            CSVColumn val100HiGHSCol = new CSVColumn { columnName = "100%HiGHS_val" };
            CSVColumn optimalCol = new CSVColumn { columnName = "optimal" };
            CSVColumn orderCountCol = new CSVColumn { columnName = "order_count" };
            CSVColumn stockCountCol = new CSVColumn { columnName = "stock_count" };
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {

                TestResult averagedMulti100HiGHSResults = new TestResult();
                int optimalValAveraged = 0;
                int stockCount = (int)Math.Ceiling((i / 6.0));
                int orderCount = 10 + ((i - 1) % 6) * 2;
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = orderCount, StockLengthLowerBound = 900, StockLengthUpperBound = 1200, StockCount = stockCount, OrderLengthLowerBound = 0.1, OrderLengthUpperBound = 0.8, AverageDemand = 1000 / orderCount };
                    var problem = gpcs.GenerateProblem(def);
                    CuttingStockProblemDataDTO problemData = problem.Item1;
                    AmplResult optimalResultData = problem.Item2;
                    problemData.AlgorithmSettings.RelaxationType = "auto";
                    RelaxApplier.ApplyRelax(problemData, 0.5, 0, 0.2);

                    //act
                    problemData.RelaxCostMultiplier = 100;
                    problemData.AlgorithmSettings.Solver = "highs";
                    TestResult Mutli100HiGHSTestResults = amplSolver.SolveWithAMPL(problemData);

                    //average data
                    averagedMulti100HiGHSResults.ObtainedValue += Mutli100HiGHSTestResults.ObtainedValue / BATCH_ITERATIONS;
                    optimalValAveraged += (int)optimalResultData.TotalCost! / BATCH_ITERATIONS;
                }

                val100HiGHSCol.values.Add(averagedMulti100HiGHSResults.ObtainedValue.ToString());
                optimalCol.values.Add(optimalValAveraged.ToString());
                orderCountCol.values.Add(orderCount.ToString());
                stockCountCol.values.Add(stockCount.ToString());
            }

            //save to file
            List<CSVColumn> combinedResults = new()
            {
               val100HiGHSCol,
               optimalCol,
               orderCountCol,
               stockCountCol,
            };
            TestResultSaver.SaveToCSVCombined(combinedResults, "RelaxCostMultiplierAutoHiGHS");
        }
    }
}
