using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;

/*
 * This class generates new cutting stock problems with known optimal solution
 */

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class CuttingStockProblemGenerator 
    {
        /* stockCount = number of stock items
         * stockSizeRange = lower bound and upper bound of stock lengths in integers
         * minOrderCount = number of order items
         * minOrderSize = sum of order items demand
         * orderSizeRange = lower bound and upper bound of order lengths in range <0,1>
         * orderLengthGap = min value gap of order items length
         */
        //public static (CuttingStockProblemDataDTO, AmplResult)  GenerateProblem(int stockCount, (int, int) stockSizeRange, int minOrderCount, int minOrderSize, (double, double) orderSizeRange, int orderLengthGap )
        //{
        //    Random rand = new Random();
        //    var problemData = new CuttingStockProblemDataDTO();
        //    problemData.StockList = [];
        //    problemData.OrderList = [];
        //    var amplResult = new AmplResult();
        //    amplResult.TotalCost = 0;

        //    var stockSet = new HashSet<int>();
        //    for (int i = 0; i < stockCount; ++i)
        //    {
        //        int stockLength;
        //        do
        //        {
        //            stockLength = rand.Next(stockSizeRange.Item1, stockSizeRange.Item2);
        //        } while (stockSet.Contains(stockLength));

        //        stockSet.Add(stockLength);
        //        problemData.StockList.Add(new StockItem { Length = stockLength, Cost=stockLength });
        //    }

        //    var patternList = new List<int>();
        //    var orderMap = new Dictionary<int, int>();
        //    int currOrderCount = 0;
        //    int currOrderSize = 0;
        //    while (currOrderCount < minOrderCount && currOrderSize < minOrderSize)
        //    {
        //        var stockItem = problemData.StockList[rand.Next(0, problemData.StockList.Count)];
        //        amplResult.TotalCost += stockItem.Length;
        //        int remainingLength = stockItem.Length;
        //        while (remainingLength > 0)
        //        {
        //            int maxNewOrderLength = Math.Min(remainingLength, orderSizeRange.Item2);
        //            int x = rand.Next(orderSizeRange.Item1, maxNewOrderLength);
        //            x += orderLengthGap - (x % orderLengthGap);

        //            if (remainingLength > x)
        //            {
        //                if (orderMap.TryGetValue(x, out int val))
        //                {
        //                    orderMap[x] = val + 1;
        //                }
        //                else
        //                {
        //                    ++currOrderCount;
        //                    orderMap[x] = 1;
        //                }
        //            }
        //            else
        //            {
        //                if (orderMap.TryGetValue(remainingLength, out int val))
        //                {
        //                    orderMap[remainingLength] = val + 1;
        //                }

        //                else
        //                {
        //                    ++currOrderCount;
        //                    orderMap[remainingLength] = 1;
        //                }
        //            }

        //            remainingLength -= x;
        //            ++currOrderSize;
        //        }
        //    }

        //    foreach(var orderItem in orderMap)
        //    {
        //        problemData.OrderList.Add(new OrderItem { Length = orderItem.Key, Count = orderItem.Value });
        //    }

        //    return (problemData, amplResult);
        //}
        private class Pattern
        {
            public List<int> values = new List<int>();
            public int stockItemLength = 0;
        }
        private class PatternWithCount
        {
            public Pattern? pattern;
            public int count;
        }

        public static (CuttingStockProblemDataDTO, AmplResult) GenerateProblem(int stockCount, (int, int) stockSizeRange, int minOrderCount, int minOrderSize, (double, double) orderSizeRange)
        {
            Random rand = new Random();
            var problemData = new CuttingStockProblemDataDTO();
            problemData.StockList = [];
            problemData.OrderList = [];
            var amplResult = new AmplResult();
            amplResult.TotalCost = 0;

            //generate stock items
            var stockSet = new HashSet<int>();
            for (int i = 0; i < stockCount; ++i)
            {
                int stockLength;
                do
                {
                    stockLength = rand.Next(stockSizeRange.Item1, stockSizeRange.Item2);
                } while (stockSet.Contains(stockLength));

                stockSet.Add(stockLength);
                problemData.StockList.Add(new StockItem { Length = stockLength, Cost = stockLength });
            }

            //generate patterns
            var patternList = new List<Pattern>();
            int currOrderCount = 0;
            while (currOrderCount < minOrderCount)
            {
                var stockItem = problemData.StockList[rand.Next(0, problemData.StockList.Count)];
                int remainingLength = stockItem.Length;
                double lb = orderSizeRange.Item1;
                double ub = orderSizeRange.Item2;
                int minLength = (int)(lb * stockItem.Length);
                List<int> pattern = new();
                while (remainingLength > 0)
                {
                    var randomVal = rand.NextDouble();
                    double lengthPercentage = (ub - lb) * randomVal + lb;
                    int newOrderLength = (int)Math.Round(stockItem.Length * lengthPercentage);
                    if (remainingLength - newOrderLength < minLength)
                        newOrderLength = remainingLength;

                    pattern.Add(newOrderLength);

                    ++currOrderCount;
                    remainingLength -= newOrderLength;
                }
                patternList.Add(new Pattern { values = pattern, stockItemLength=stockItem.Length });
            }

            amplResult.TotalCost = patternList.Aggregate(0, (x, next) => next.stockItemLength);

            //generate sufficient demand (minOrderSize)
            List<PatternWithCount> patternListWithCount = new();
            patternListWithCount.AddRange(patternList.Select(x => new PatternWithCount { pattern=x, count=1 }));
            int currOrderSize = patternList.Aggregate(0, (x, next) => { return next.values.Count;});
            while( currOrderSize < minOrderSize)
            {
                var randIdx = rand.Next(0, patternListWithCount.Count);
                ++patternListWithCount[randIdx].count;
                currOrderSize += patternListWithCount[randIdx].pattern!.values.Count;
                amplResult.TotalCost += patternListWithCount[randIdx].pattern!.stockItemLength;
            }

            //transform patterns into OrderItems
            foreach ( var patternWithCount in patternListWithCount)
            {
                foreach(int length in patternWithCount.pattern!.values)
                {
                    var idx = problemData.OrderList.FindIndex(x => x.Length == length);
                    if (idx != -1)
                    {
                        problemData.OrderList[idx].Count += patternWithCount.count;
                    }
                    else
                    {
                        problemData.OrderList.Add(new OrderItem { Length = length, Count = patternWithCount.count });
                    }
                }
                    
            }

            return (problemData, amplResult);
        }
    }
}
