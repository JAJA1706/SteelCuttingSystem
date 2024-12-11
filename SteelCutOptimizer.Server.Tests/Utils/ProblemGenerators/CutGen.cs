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
    public class Problem
    {
        public List<OrderItem>? Orders;
        public int StockLength;
    }

    public class Cutgen
    {
        private Random rGen;

        public Cutgen()
        {
            rGen = new Random();
        }

        public CuttingStockProblemDataDTO GenerateProblem(ProblemGenerationDefinition problemDef)
        {
            int[] lengths = generateLengths(problemDef);
            int[] demands = generateDemands(problemDef);

            var cutgenProblem = merge(lengths, demands);

            CuttingStockProblemDataDTO result = new();
            result.OrderList = cutgenProblem.Orders;
            result.StockList = new List<StockItem>();
            result.StockList.Add(new StockItem { Length = problemDef.StockLength, Cost = problemDef.StockLength });

            return result;
        }

        private int[] generateLengths(ProblemGenerationDefinition problemDef)
        {
            int[] result = new int[problemDef.Size];

            double lb = problemDef.OrderLengthLowerBound;
            double ub = problemDef.OrderLengthUpperBound;
            for (int i = 0; i < result.Length; i++)
            {
                double rValue = rGen.NextDouble();
                double length = (lb + (ub - lb) * rValue) * problemDef.StockLength + rValue;

                result[i] = (int)length;
            }

            descendingSort(result);

            return result;
        }

        private int[] generateDemands(ProblemGenerationDefinition problemDef)
        {
            int[] result = new int[problemDef.Size];

            double sum = 0;
            double[] rands = new double[problemDef.Size];
            for (int i = 0; i < result.Length; i++)
            {
                rands[i] = rGen.NextDouble();
                sum += rands[i];
            }

            int totalDemand = problemDef.AverageDemand * problemDef.Size;
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

        private Problem merge(int[] lengths, int[] demands)
        {
            Problem problem = new Problem();
            problem.Orders = new List<OrderItem>();
            for (int i = 0; i < lengths.Length; i++)
            {
                if (i == lengths.Length - 1 || lengths[i] != lengths[i + 1])
                {
                    var order = new OrderItem { Length = lengths[i], Count = demands[i] };
                    problem.Orders.Add(order);
                }
                else
                {
                    demands[i + 1] += demands[i];
                }
            }

            return problem;
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

    }
}
