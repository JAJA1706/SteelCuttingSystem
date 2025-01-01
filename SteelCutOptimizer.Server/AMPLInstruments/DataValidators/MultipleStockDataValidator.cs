using SteelCutOptimizer.Server.DTO;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public class MultipleStockDataValidator : IAmplDataValidator
    {
        public void ValidateEntryData(CuttingStockProblemDataDTO entryData)
        {
            if(entryData.OrderList == null || entryData.StockList == null)
                throw new InvalidDataException("Stock list nor Order list cannot be null");

            //orderLengthSum < stockLengthSum
            long orderLengthSum = entryData.OrderList.Aggregate((long)0, (sum, next) => sum += next.Length * next.Count);

            long stockLengthSum;
            var unlimitedStock = entryData.StockList.Find(x => x.Count == null);
            if (unlimitedStock != null)
                stockLengthSum = long.MaxValue;
            else
                stockLengthSum = entryData.StockList.Aggregate((long)0, (sum, next) => sum += next.Length * (int)next.Count!);

            if(orderLengthSum > stockLengthSum)
                throw new InvalidDataException("infeasible problem, sum of order lengths is higher than sum of stock lengths");

            //maxOrderLength < maxStockLength
            if(unlimitedStock == null)
            {
                int maxOrderLength = entryData.OrderList.Max(x => x.Length);
                int maxStockLength = entryData.StockList.Max(y => y.Length);
                if (maxOrderLength > stockLengthSum)
                    throw new InvalidDataException("infeasible problem, max order length is higher than max stock length.");
            }

            return ;
        }

        public void ValidateResultData(AmplResult amplResult, CuttingStockProblemDataDTO entryData)
        {
            if (amplResult.IsFeasible == false)
                throw new InvalidDataException("infeasible problem");

            //Check if at least one proper Pattern exists 
            if (!amplResult.Patterns.Values.Any(x => x.UseCount > 0))
                throw new InvalidDataException("infeasible problem");

            //Checking if amount of stock items used is not greater than allowed
            Dictionary<int, int> usedStockLengths = [];
            foreach (var pattern in amplResult.Patterns)
            {
                if (!usedStockLengths.ContainsKey(pattern.Value.StockLength))
                    usedStockLengths[pattern.Value.StockLength] = 0;

                usedStockLengths[pattern.Value.StockLength] += pattern.Value.UseCount;
            }
            foreach (var stock in entryData.StockList ?? [])
            {
                if (usedStockLengths.TryGetValue(stock.Length, out int usedStockCount))
                {
                    if (stock.Count < usedStockCount)
                        throw new InvalidDataException("infeasible problem");
                }
            }

            //Check if virtual stock items has been used
            if (entryData.StockList!.Any(x => x.Cost == int.MaxValue))
            {
                var virtualStockId = entryData.StockList!.Count() - 1; //virtual is added as last so it will be placed on the end.
                foreach (var pattern in amplResult.Patterns.Values)
                {
                    if (pattern.StockId == virtualStockId && pattern.UseCount > 0)
                        throw new InvalidDataException("infeasible problem");
                }
            }
        }
    }
}
