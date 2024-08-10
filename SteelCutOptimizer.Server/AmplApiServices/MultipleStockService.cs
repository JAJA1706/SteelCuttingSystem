using ampl.Entities;
using ampl;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class MultipleStockService : IAmplApiService
    {
        private readonly string modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/multipleStockExtended/";

        public AmplResult SolveCuttingStockProblem(string dataFilePath)
        {
            using (AMPL a = new AMPL())
            {
                a.Read(Path.Combine(modelPath, "cut.mod"));
                a.ReadData(Path.Combine(dataFilePath, "cut.dat"));
                a.Read(Path.Combine(modelPath, "cut.run"));

                Objective totalcost = a.GetObjective("Cost");
                Parameter lengthForEachPattern = a.GetParameter("lfep");
                Parameter stockLengths = a.GetParameter("stockLengths");
                Parameter orderLengths = a.GetParameter("orderLengths");
                Variable usedPatterns = a.GetVariable("usedPatterns");

                var lfepDataFrame = lengthForEachPattern.GetValues(); //[0] = stockIdx, [1] = orderIdx, [2] = patternPartialIdx, value = Table[stockIdx, orderIdx, patternPartialIdx, usedOrderItemsCount]
                var usedPatternDataFrame = usedPatterns.GetValues(); //[0] - stockIdx, [1] - patternPartialIdx, value = Table[stockIdx, patternPartialIdx, patternCount]
                var result = new AmplResult();
                foreach (var item in lfepDataFrame)
                {
                    //item[0] = stockIdx, item[1] = orderIdx, item[2] = patternPartialIdx, item[3] = usedOrderItemsCount
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
                            for(int i = 0; i < usedOrderItemsCount; ++i)
                                result.Patterns[patternIdx].UsedOrderLengths.Add(usedOrderLength);
                        }
                        else
                        {
                            int usedPatternCount = (int)usedPatternDataFrame[stockIdx, patternPartialIdx][2].Dbl;
                            int stockLength = (int)stockLengths[stockIdx].Dbl;

                            List<int> usedOrderLengths = new List<int>();
                            for (int i = 0; i < usedOrderItemsCount; ++i)
                                usedOrderLengths.Add(usedOrderLength);

                            Pattern pattern = new Pattern(stockLength, usedPatternCount, usedOrderLengths);
                            result.Patterns.Add(patternIdx, pattern);
                        }
                    }
                }

                result.TotalCost = (int)totalcost.Value;
                return result;
            }
        }
    }
}
