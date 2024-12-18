using Microsoft.AspNetCore.Components.Forms;
using SteelCutOptimizer.Server.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelCutOptimizer.Server.Tests.Utils
{
    internal class RelaxApplier
    {
        /*
         * Example usage ApplyRelax(..., relaxProbability = 0.5, relaxLowerBound = 0, relaxUpperBound = 0.1)
         * will result in ~50% orders being relaxed in range <0,0.1>, this means that if order have a length of 100, applied maxRelax to it will be in range 0 to 10.
         * Changing "extendItems" to true will result in additionaly extending items length by maxRelax length. If we knew optimal result of non relaxed version beforehand, 
         * we can use this option to make it an optimal result of the relaxed version.
         */
        public static void ApplyRelax(CuttingStockProblemDataDTO problem, double relaxProbability, double relaxLowerBound, double relaxUpperBound, bool extendItems = false)
        {
            if (problem.OrderList == null || problem.StockList == null)
                throw new InvalidDataException("Order list nor Stock list cannot be empty");
            if(relaxUpperBound > 1 || relaxUpperBound < 0 || relaxLowerBound > 1 || relaxLowerBound < 0)
                throw new InvalidDataException("Bounds can be only in range <0,1>");

            int longestStockLength = problem.StockList.Max(x => x.Length);
            Random rand = new Random();
            foreach( var order in problem.OrderList )
            {
                var shouldBeRelaxed = rand.NextDouble();
                if (shouldBeRelaxed > relaxProbability)
                    continue;

                var randomVal = rand.NextDouble();
                double relaxPercentage = (relaxUpperBound - relaxLowerBound) * randomVal + relaxLowerBound;
                int maxRelax = (int)(Math.Round(order.Length * relaxPercentage));

                if (extendItems)
                {
                    if (order.Length + maxRelax > longestStockLength)
                        maxRelax = longestStockLength - order.Length;

                    order.Length += maxRelax;
                }

                order.MaxRelax = maxRelax;
                order.CanBeRelaxed = true;
            }
        }

        public static void MultiplyRelax(CuttingStockProblemDataDTO problem, double multiply)
        {
            if (problem.OrderList == null)
                throw new InvalidDataException("Order list cannot be empty");

            foreach (var order in problem.OrderList)
            {
                if(order.MaxRelax != null)
                    order.MaxRelax = (int)(order.MaxRelax * multiply);
            }
        }
    }
}
