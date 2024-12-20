using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Tests.Structs;

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class BinPackSolver
    {
        public static TestResult SolveWithBinPack(CuttingStockProblemDataDTO data)
        {
            if (data.StockList?.Count != 1)
                throw new InvalidDataException("Cutting Stock problem can only be solved as a BinPack problem if we have only one type of stock item.");
            if (data.OrderList?.Any() != true)
                throw new InvalidDataException("OrderList cannot be empty");

            List<int> orderItems = new();
            foreach(var item in data.OrderList)
            {
                orderItems.InsertRange(0, Enumerable.Repeat(item.Length - item.MaxRelax ?? 0, item.Count));
            }

            TestResult result = new TestResult();
            Stopwatch sw = Stopwatch.StartNew();
            var ffdResult = firstFitDecreasingBinPacking(orderItems, data.StockList.First().Length);
            sw.Stop();

            result.Time = sw.ElapsedMilliseconds;
            result.ProblemSize = orderItems.Count;
            result.ObtainedValue = ffdResult.Count * data.StockList.First().Length;

            return result;
        }

        private static List<List<int>> firstFitDecreasingBinPacking(List<int> items, int binCapacity)
        {
            items.Sort((x,y) => {
                if (x < y)
                    return 1;
                if (x > y) 
                    return -1;
                return 0;
            });

            int smallestItem = items.Last();
            List<List<int>> openedBins = new List<List<int>>();
            List<List<int>> closedBins = new List<List<int>>();
            foreach (int item in items)
            {
                bool placed = false;

                for (int i = 0; i < openedBins.Count; i++)
                {
                    int remainingBinCapacity = binCapacity - sumOfItems(openedBins[i]);
                    if (item <= remainingBinCapacity)
                    {
                        openedBins[i].Add(item);
                        if(item + smallestItem > remainingBinCapacity)
                        {
                            closedBins.Add(openedBins[i]);
                            openedBins.RemoveAt(i);
                        }

                        placed = true;
                        break;
                    }
                }

                if (!placed)
                {
                    if(item + smallestItem > binCapacity)
                        closedBins.Add(new List<int> { item });
                    else
                        openedBins.Add(new List<int> { item });
                }
            }

            closedBins.AddRange(openedBins);
            return closedBins;
        }

        private static int sumOfItems(List<int> items)
        {
            int total = 0;
            foreach (var val in items)
            {
                total += val;
            }
            return total;
        }
    }
}
