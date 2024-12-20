using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.Tests.Structs;

/*
 * This code was implemented by Victor Balabanov "akavrt" for JAVA lanugage. https://github.com/akavrt/cutgen
 * 
 * Original algorithm comes from science paper:
 * "CUTGEN1: A problem generator for the standard one-dimensional cutting stock problem" by  T. Gau, G. Wäscher
 */


namespace SteelCutOptimizer.Server.Tests.Utils
{
    public class Cutgen
    {
        private Random rGen;

        public Cutgen()
        {
            rGen = new Random();
        }

        public CuttingStockProblemDataDTO GenerateProblem(ProblemGenerationDefinition problemDef)
        {
            CuttingStockProblemDataDTO result = new();
            result.StockList = new List<StockItem>();
            result.OrderList = new List<OrderItem>();

            //single stock item
            if (problemDef.StockCount == null && problemDef.StockLength != null)
            {
                int[] lengths = generateLengths(problemDef.OrderCount, problemDef.OrderLengthLowerBound, problemDef.OrderLengthUpperBound, (int)problemDef.StockLength);
                int[] demands = generateDemands(problemDef.OrderCount, problemDef.AverageDemand);
                var orders = merge(lengths, demands);
                result.OrderList.AddRange(orders);
                result.StockList.Add(new StockItem { Length = (int)problemDef.StockLength, Cost = problemDef.StockLength });
            }
            //multiple stock items
            else if(problemDef.StockCount != null && problemDef.StockLengthLowerBound != null && problemDef.StockLengthUpperBound != null && problemDef.StockLength == null)
            {
                Dictionary<int, OrderItem> orderMap = new();
                List<int> orderSize = divideOrdersBetweenStockItems(problemDef.OrderCount, (int)problemDef.StockCount);
                for(int i = 0; i < problemDef.StockCount; i++)
                {
                    int stockLength = rGen.Next((int)problemDef.StockLengthLowerBound, (int)problemDef.StockLengthUpperBound + 1);
                    int[] lengths = generateLengths(orderSize[i], problemDef.OrderLengthLowerBound, problemDef.OrderLengthUpperBound, stockLength);
                    int[] demands = generateDemands(orderSize[i], problemDef.AverageDemand);
                    var orders = merge(lengths, demands);
                    foreach( var item in orders)
                    {
                        if (orderMap.ContainsKey(item.Length))
                            orderMap[item.Length].Count += item.Count;
                        else
                            orderMap[item.Length] = new OrderItem { Length = item.Length, Count = item.Count };
                    }
                    result.StockList.Add(new StockItem { Length = stockLength, Cost = stockLength });
                }
                var uniqueOrders = orderMap.Values.ToList();
                result.OrderList.AddRange(uniqueOrders);
            }

            return result;
        }

        private int[] generateLengths(int size, double lowerb, double upperb, int stockLength)
        {
            int[] result = new int[size];

            for (int i = 0; i < result.Length; i++)
            {
                double rValue = rGen.NextDouble();
                double length = (lowerb + (upperb - lowerb) * rValue) * stockLength + rValue;

                result[i] = (int)length;
            }

            descendingSort(result);

            return result;
        }

        private int[] generateDemands(int size, int averageDemand)
        {
            int[] result = new int[size];

            double sum = 0;
            double[] rands = new double[size];
            for (int i = 0; i < result.Length; i++)
            {
                rands[i] = rGen.NextDouble();
                sum += rands[i];
            }

            int totalDemand = averageDemand * size;
            int rest = totalDemand;
            for (int i = 0; i < result.Length - 1; i++)
            {
                double demand = totalDemand * rands[i] / sum + 0.5;
                result[i] = Math.Max(1, (int)demand);

                rest -= result[i];
            }

            result[result.Length - 1] = Math.Max(1, rest);

            return result;
        }

        private static List<OrderItem> merge(int[] lengths, int[] demands)
        {
            var orders = new List<OrderItem>();
            for (int i = 0; i < lengths.Length; i++)
            {
                if (i == lengths.Length - 1 || lengths[i] != lengths[i + 1])
                {
                    var order = new OrderItem { Length = lengths[i], Count = demands[i] };
                    orders.Add(order);
                }
                else
                {
                    demands[i + 1] += demands[i];
                }
            }

            return orders;
        }

        public static void descendingSort(int[] a)
        {
            Array.Sort(a);

            int left = 0;
            int right = a.Length - 1;

            while (left < right)
            {
                int temp = a[left];
                a[left] = a[right];
                a[right] = temp;

                left++;
                right--;
            }
        }

        private List<int> divideOrdersBetweenStockItems(int orderCount, int stockCount)
        {
            List<int> result = new();
            int avrCount = orderCount / stockCount;
            result.AddRange(Enumerable.Repeat(avrCount, stockCount));

            for(int i = 0; i < orderCount % stockCount; ++i)
            {
                int rIdx = rGen.Next(0, result.Count);
                ++result[rIdx];
            }

            return result;
        }

    }
}
