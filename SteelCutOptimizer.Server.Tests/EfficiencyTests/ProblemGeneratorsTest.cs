using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Utils;

namespace EfficiencyTests
{
    public class ProblemGeneratorsTest
    {
        [EfficiencyFact]
        public void ProblemGeneratorsComparison()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            CuttingStockProblemGenerator cspg = new CuttingStockProblemGenerator();
            const int TEST_ITERATIONS = 1; //18
            const int BATCH_ITERATIONS = 1; //3

            List<CSVColumn> CSVColumns = new List<CSVColumn>();
            CSVColumn cutgenTimeCol = new CSVColumn { columnName = "cutgen_time" };
            CSVColumn cutgenValCol = new CSVColumn { columnName = "cutgen_val" };
            CSVColumn cspgTimeCol = new CSVColumn { columnName = "cspg_time" };
            CSVColumn cspgValCol = new CSVColumn { columnName = "cspg_val" };
            CSVColumn orderCountCol = new CSVColumn { columnName = "order_count" };
            CSVColumn stockCountCol = new CSVColumn { columnName = "stock_count" };
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                TestResult averagedCutgenResults = new TestResult();
                TestResult averagedCspResults = new TestResult();
                int stockCount = (int)Math.Ceiling((i / 6.0));
                int orderCount = 10 + ((i - 1) % 6) * 2;
                for (int j = 1; j <= BATCH_ITERATIONS; ++j)
                {
                    //arrange
                    var def = new ProblemGenerationDefinition { OrderCount = orderCount, StockCount = stockCount, StockLengthLowerBound=900, StockLengthUpperBound=1200, OrderLengthLowerBound = 0.1, OrderLengthUpperBound = 0.8, AverageDemand = 1000 / orderCount };
                    CuttingStockProblemDataDTO problemDataCutgen = problemGen.GenerateProblem(def);
                    problemDataCutgen.AlgorithmSettings.RelaxationType = "manual";
                    RelaxApplier.ApplyRelax(problemDataCutgen, 0.5, 0, 0.2);

                    var problem = cspg.GenerateProblem(def);
                    CuttingStockProblemDataDTO problemDataCSPG = problem.Item1;
                    problemDataCSPG.AlgorithmSettings.RelaxationType = "manual";
                    RelaxApplier.ApplyRelax(problemDataCSPG, 0.5, 0, 0.2);

                    //act
                    TestResult cutgenResults = amplSolver.SolveWithAMPL(problemDataCutgen);
                    TestResult cspgResults = amplSolver.SolveWithAMPL(problemDataCSPG);

                    //average data
                    averagedCutgenResults.Time += cutgenResults.Time / BATCH_ITERATIONS;
                    averagedCutgenResults.ObtainedValue += cutgenResults.ObtainedValue / BATCH_ITERATIONS;
                    averagedCspResults.Time += cspgResults.Time / BATCH_ITERATIONS;
                    averagedCspResults.ObtainedValue += cspgResults.ObtainedValue / BATCH_ITERATIONS;
                }

                cutgenTimeCol.values.Add(averagedCutgenResults.Time.ToString());
                cutgenValCol.values.Add(averagedCutgenResults.ObtainedValue.ToString());
                cspgTimeCol.values.Add(averagedCspResults.Time.ToString());
                cspgValCol.values.Add(averagedCspResults.ObtainedValue.ToString());
                orderCountCol.values.Add(orderCount.ToString());
                stockCountCol.values.Add(stockCount.ToString());
            }

            //save to file
            List<CSVColumn> combinedResults = new()
            {
               cutgenTimeCol,
               cutgenValCol,
               cspgTimeCol,
               cspgValCol,
               orderCountCol,
               stockCountCol,
            };
            TestResultSaver.SaveToCSVCombined(combinedResults, "ProblemGenerators");
        }
    }
}
