using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Tests.Structs;

/*
 * This class generates new cutting stock problems with known optimal solution
 */

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class CuttingStockProblemGenerator 
    {
        private Random rGen = new Random();

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

        public (CuttingStockProblemDataDTO, AmplResult) GenerateProblem(ProblemGenerationDefinition problemDef)
        {
            if (problemDef.StockCount == null || problemDef.StockLengthLowerBound == null || problemDef.StockLengthUpperBound == null)
                throw new NullReferenceException("Invalid problem definition");

            int minOrderSize = problemDef.AverageDemand * problemDef.OrderCount;
            int minOrderCount = problemDef.OrderCount;

            var problemData = new CuttingStockProblemDataDTO();
            problemData.StockList = [];
            problemData.OrderList = [];


            problemData.StockList = generateStockLengths((int)problemDef.StockCount, (int)problemDef.StockLengthLowerBound, (int)problemDef.StockLengthUpperBound);

            var patternList = generatePatterns(problemData.StockList, minOrderCount, problemDef.OrderLengthLowerBound, problemDef.OrderLengthUpperBound);

            var patternListWithCount = generateDemand(patternList, minOrderSize);

            var amplResult = new AmplResult();
            amplResult.TotalCost = patternListWithCount.Aggregate(0, (x, next) => x + next.count * next.pattern!.stockItemLength);

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

        private List<StockItem> generateStockLengths(int stockCount, int lb, int ub)
        {
            var result = new List<StockItem>();
            var stockSet = new HashSet<int>();
            for (int i = 0; i < stockCount; ++i)
            {
                int stockLength;
                do
                {
                    stockLength = rGen.Next(lb, ub + 1);
                } while (stockSet.Contains(stockLength));

                stockSet.Add(stockLength);
                result.Add(new StockItem { Length = stockLength, Cost = stockLength });
            }

            return result;
        }

        private List<Pattern> generatePatterns(List<StockItem> stockItems, int minOrderCount, double lb, double ub)
        {
            var patternList = new List<Pattern>();
            int currOrderCount = 0;
            while (currOrderCount < minOrderCount)
            {
                var stockItem = stockItems[rGen.Next(0, stockItems.Count)];
                int remainingLength = stockItem.Length;
                int minLength = (int)(lb * stockItem.Length);
                List<int> pattern = new();
                while (remainingLength > 0)
                {
                    var randomVal = rGen.NextDouble();
                    double lengthPercentage = (ub - lb) * randomVal + lb;
                    int newOrderLength = (int)Math.Round(stockItem.Length * lengthPercentage);
                    if (remainingLength - newOrderLength < minLength)
                        newOrderLength = remainingLength;

                    pattern.Add(newOrderLength);

                    ++currOrderCount;
                    remainingLength -= newOrderLength;
                }
                patternList.Add(new Pattern { values = pattern, stockItemLength = stockItem.Length });
            }

            return patternList;
        }

        private List<PatternWithCount> generateDemand(List<Pattern> patterns, int minOrderSize)
        {
            List<PatternWithCount> patternListWithCount = new();
            patternListWithCount.AddRange(patterns.Select(x => new PatternWithCount { pattern = x, count = 1 }));
            int currOrderSize = patterns.Aggregate(0, (x, next) => { return next.values.Count; });
            while (currOrderSize < minOrderSize)
            {
                var randIdx = rGen.Next(0, patternListWithCount.Count);
                ++patternListWithCount[randIdx].count;
                currOrderSize += patternListWithCount[randIdx].pattern!.values.Count;
            }

            return patternListWithCount;
        }
    }
}
