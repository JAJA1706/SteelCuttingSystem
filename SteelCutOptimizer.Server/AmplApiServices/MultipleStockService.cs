using ampl.Entities;
using ampl;
using SteelCutOptimizer.Server.Structs;
using SteelCutOptimizer.Server.DTO;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class MultipleStockService : IAmplApiService
    {
        private readonly string modelPath;
        private readonly AlgorithmSettings settings;

        public MultipleStockService(AlgorithmSettings _settings) 
        {
            settings = _settings;
            if (settings.MainObjective == "cost")
            {
                if(settings.RelaxationType == "none")
                    modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeCostNoRelax/";
                else
                    throw new NotImplementedException();
            }
            else if (settings.MainObjective == "waste")
            {
                if (settings.RelaxationType == "none")
                    modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeWasteNoRelax/";
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
        }

        public AmplResult SolveCuttingStockProblem(string dataFilePath)
        {
            using (AMPL ampl = new AMPL())
            {
                ampl.Read(Path.Combine(modelPath, "cut.mod"));
                ampl.ReadData(Path.Combine(dataFilePath, "cut.dat"));
                ampl.Read(Path.Combine(modelPath, "cut.run"));

                Parameter lengthForEachPattern = ampl.GetParameter("lfep");
                Parameter stockLengths = ampl.GetParameter("stockLengths");
                Parameter orderLengths = ampl.GetParameter("orderLengths");
                Variable usedPatterns = ampl.GetVariable("usedPatterns");

                var lfepDataFrame = lengthForEachPattern.GetValues(); //[0] = stockIdx, [1] = orderIdx, [2] = patternPartialIdx, value = Table[stockIdx, orderIdx, patternPartialIdx, usedOrderItemsCount]
                var usedPatternDataFrame = usedPatterns.GetValues(); //[0] - stockIdx, [1] - patternPartialIdx, value = Table[stockIdx, patternPartialIdx, patternCount]
                var result = new AmplResult();
                foreach (var item in lfepDataFrame)
                {
                    int usedOrderItemsCount = (int)item[3].Dbl; 
                    if (usedOrderItemsCount != 0)
                    {
                        int stockIdx = (int)item[0].Dbl;
                        int orderIdx = (int)item[1].Dbl;
                        int patternPartialIdx = (int)item[2].Dbl;
                        int usedOrderLength = (int)orderLengths[orderIdx].Dbl;

                        Tuple<int, int> patternIdx = new(stockIdx, patternPartialIdx);
                        if(result.Patterns.ContainsKey(patternIdx))
                        {
                            Segment newSegment = new(orderIdx, usedOrderLength, 0);
                            List<Segment> relaxedLengths = Enumerable.Repeat(newSegment, usedOrderItemsCount).ToList();
                            result.Patterns[patternIdx].SegmentList.AddRange(relaxedLengths);
                        }
                        else
                        {
                            Segment newSegment = new(orderIdx, usedOrderLength, 0);
                            List<Segment> relaxedLengths = Enumerable.Repeat(newSegment, usedOrderItemsCount).ToList();

                            int usedPatternCount = (int)usedPatternDataFrame[stockIdx, patternPartialIdx][2].Dbl;
                            int stockLength = (int)stockLengths[stockIdx].Dbl;
                            Pattern pattern = new Pattern(stockIdx, stockLength, usedPatternCount, relaxedLengths);
                            result.Patterns.Add(patternIdx, pattern);
                        }
                    }
                }


                
                Parameter stockLimit = ampl.GetParameter("stockLimitSaved");
                var stockLimitDataFrame = stockLimit.GetValues();
                foreach (var limit in stockLimitDataFrame)
                {
                    result.StockLimits.Add(limit[1].Dbl);
                }

                Parameter orderPrices = ampl.GetParameter("price");
                var priceDataFrame = orderPrices.GetValues();
                foreach (var price in priceDataFrame)
                {
                    result.OrderPrices.Add(price[1].Dbl);
                }

                if (settings.MainObjective == "cost")
                    result.TotalCost = (int)ampl.GetObjective("Cost").Value;
                else
                    result.TotalWaste = (int)ampl.GetObjective("Waste").Value;

                return result;
            }
        }
    }
}
