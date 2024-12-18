using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Tests.Attributes;
using SteelCutOptimizer.Server.Tests.Structs;
using SteelCutOptimizer.Server.Tests.Utils;
using Xunit.Abstractions;

namespace EfficiencyTests
{
    public class CuttingWithStorageTest
    {
        private readonly ITestOutputHelper output;
        public CuttingWithStorageTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [EfficiencyFact]
        public void CostVSWasteNoRelax()
        {
            AmplSolverInterface amplSolver = new AmplSolverInterface();
            Cutgen problemGen = new Cutgen();
            const int TEST_ITERATIONS = 2; //40

            List<TestResult> costResultList = new List<TestResult>();
            List<TestResult> wasteResultList = new List<TestResult>();
            Dictionary<int, int> costOrderItemsInStorage = new();
            Dictionary<int, int> wasteOrderItemsInStorage = new();
            for (int i = 1; i <= TEST_ITERATIONS; ++i)
            {
                //arrange
                var def = new ProblemGenerationDefinition { OrderCount = 30, StockLength = 1200, OrderLengthLowerBound = 0.2, OrderLengthUpperBound = 0.8, AverageDemand = 100 };
                CuttingStockProblemDataDTO problemDataCost = problemGen.GenerateProblem(def);
                addMinimalGapBetweenOrderLengths(problemDataCost.OrderList, 100);
                problemDataCost.AlgorithmSettings.RelaxationType = "none";

                CuttingStockProblemDataDTO problemDataWaste = new CuttingStockProblemDataDTO(problemDataCost);
                problemDataWaste.AlgorithmSettings.MainObjective = "waste";

                useMaterialFromStorage(costOrderItemsInStorage, problemDataCost);
                useMaterialFromStorage(wasteOrderItemsInStorage, problemDataWaste);


                //act
                TestResult costResult = amplSolver.SolveWithAMPL(problemDataCost);
                AmplResult result = amplSolver.GetLastResult()!;
                costResult.ObtainedValue = getNumOfUsedStockItems(result);
                moveExcessMaterialToStorage(costOrderItemsInStorage, problemDataCost, result);



                TestResult wasteResult = amplSolver.SolveWithAMPL(problemDataWaste);
                result = amplSolver.GetLastResult()!;
                wasteResult.ObtainedValue = getNumOfUsedStockItems(result);
                moveExcessMaterialToStorage(wasteOrderItemsInStorage, problemDataWaste, result);


                costResultList.Add(costResult);
                wasteResultList.Add(wasteResult);
            }

            //write to Log
            int costStoredItems = costOrderItemsInStorage.Aggregate(0, (x, next) => x += next.Value);
            int wasteStoredItems = wasteOrderItemsInStorage.Aggregate(0, (x, next) => x += next.Value);
            output.WriteLine($"items in costStorage: {costStoredItems}");
            output.WriteLine($"items in wasteStorage: {wasteStoredItems}");

            //save to file
            List<List<TestResult>> combinedResults = new List<List<TestResult>>
            {
                costResultList,
                wasteResultList,
            };
            List<string> methodNames = new List<string> { "cost", "waste" };
            TestResultSaver.SaveToCSVCombined(combinedResults, methodNames, "CuttingWithStorage");
        }

        private void addMinimalGapBetweenOrderLengths(List<OrderItem>? orderList, int gap)
        {
            foreach(var item in orderList!)
            {
                int integralPart = item.Length / gap;
                double fractionalPart = item.Length / (double)gap - integralPart;
                item.Length = integralPart * gap + (int)Math.Round(fractionalPart) * gap;
            }
        }

        private void useMaterialFromStorage(Dictionary<int, int> storage, CuttingStockProblemDataDTO problem)
        {
            for (int i = problem.OrderList!.Count - 1; i >= 0; --i)
            {
                var order = problem.OrderList[i];
                if (!storage.TryGetValue(order.Length, out int storeCount))
                    continue;

                if (order.Count > storeCount)
                {
                    order.Count -= storeCount;
                    storage[order.Length] = 0;
                }
                else
                {
                    storage[order.Length] -= order.Count;
                    order.Count = 0;
                    problem.OrderList.RemoveAt(i);
                }
            }
        }

        private void moveExcessMaterialToStorage(Dictionary<int, int> storage, CuttingStockProblemDataDTO problem, AmplResult result)
        {
            Dictionary<int, int> requiredMaterial = new();
            foreach(var order in problem.OrderList!)
            {
                if (requiredMaterial.ContainsKey(order.Length))
                    requiredMaterial[order.Length] += order.Count;
                else
                    requiredMaterial[order.Length] = order.Count;
            }

            Dictionary<int, int> producedMaterial = new(requiredMaterial);
            foreach (var key in producedMaterial.Keys)
            {
                producedMaterial[key] = 0;
            }
            foreach (var pattern in result.Patterns)
            {
                foreach(var item in pattern.Value.SegmentList)
                {
                    producedMaterial[item.Length] += pattern.Value.UseCount;
                }
            }

            foreach(var key in producedMaterial.Keys)
            {
                int excessMaterial = producedMaterial[key] - requiredMaterial[key];

                if (storage.ContainsKey(key))
                    storage[key] += excessMaterial;
                else
                    storage[key] = excessMaterial;
            }
        }

        private int getNumOfUsedStockItems(AmplResult result)
        {
            return result.Patterns.Aggregate(0, (x, next) => x += next.Value.UseCount);
        }

    }
}
