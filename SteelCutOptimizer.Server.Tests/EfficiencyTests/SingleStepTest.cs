using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Utils;

namespace EfficiencyTests
{
    public class SingleStepTest
    {
        [EfficiencyFact]
        public void SingleStepMultipleIterations()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 3; //100

            List<TestResult> costTestResultList = new List<TestResult>();
            List<TestResult> wasteTestResultList = new List<TestResult>();

            //arrange
            var def = new ProblemGenerationDefinition { OrderCount = 20, StockLength=1200, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
            CuttingStockProblemDataDTO problemDataCost = problemGen.GenerateProblem(def);
            RelaxApplier.ApplyRelax(problemDataCost, 0.5, 0, 0);
            problemDataCost.StockList!.First().NextStepGeneration = true;
            CuttingStockProblemDataDTO problemDataWaste = problemGen.GenerateProblem(def);
            RelaxApplier.ApplyRelax(problemDataWaste, 0.5, 0, 0);
            problemDataWaste.StockList!.First().NextStepGeneration = true;
            problemDataWaste.AlgorithmSettings.MainObjective = "waste";


            //act
            problemDataCost.AlgorithmSettings.RelaxationType = "none";
            TestResult costTestResults = amplSolver.SolveWithAMPL(problemDataCost);
            costTestResultList.Add(costTestResults);
            problemDataCost.AlgorithmSettings.RelaxationType = "singleStep";

            bool isOptimal = false;
            for (int i = 1; i < TEST_ITERATIONS && !isOptimal; ++i)
            {
                AmplResult result = amplSolver.GetLastResult()!;
                problemDataCost.StockLimits = result.StockLimits;
                problemDataCost.OrderPrices = result.OrderPrices;
                problemDataCost.Patterns = [.. result.Patterns.Values];
                try{
                    costTestResults = amplSolver.SolveWithAMPL(problemDataCost);
                    costTestResultList.Add(costTestResults);
                } catch (InvalidDataException e)
                {
                    if (e.Message == "infeasible problem")
                        isOptimal = true;
                }
            }

            problemDataWaste.AlgorithmSettings.RelaxationType = "none";
            TestResult wasteTestResults = amplSolver.SolveWithAMPL(problemDataWaste);
            wasteTestResultList.Add(wasteTestResults);
            problemDataWaste.AlgorithmSettings.RelaxationType = "singleStep";

            isOptimal = false;
            for (int i = 1; i < TEST_ITERATIONS && !isOptimal; ++i)
            {
                AmplResult result = amplSolver.GetLastResult()!;
                problemDataWaste.StockLimits = result.StockLimits;
                problemDataWaste.OrderPrices = result.OrderPrices;
                problemDataWaste.Patterns = [.. result.Patterns.Values];
                try
                {
                    wasteTestResults = amplSolver.SolveWithAMPL(problemDataWaste);
                    wasteTestResultList.Add(wasteTestResults);
                }
                catch (InvalidDataException e)
                {
                    if (e.Message == "infeasible problem")
                        isOptimal = true;
                }
            }

            //save to file
            TestResultSaver.SaveToCSV(costTestResultList, "SingleStepCost");
            TestResultSaver.SaveToCSV(wasteTestResultList, "SingleStepWaste");
        }
    }
}
