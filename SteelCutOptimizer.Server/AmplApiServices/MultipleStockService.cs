using ampl.Entities;
using ampl;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AmplApiServices
{
    public class MultipleStockService : IAmplApiService
    {
        private readonly string modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/multipleStockExtended/";

        public AmplResult SolveCuttingStockProblem(string dataFilePath)
        {
            using (AMPL ampl = new AMPL())
            {
                ampl.Read(Path.Combine(modelPath, "cut.mod"));
                ampl.ReadData(Path.Combine(dataFilePath, "cut.dat"));
                ampl.Read(Path.Combine(modelPath, "cut.run"));

                Objective totalcost = ampl.GetObjective("Cost");
                Parameter lengthForEachPattern = ampl.GetParameter("lfep");
                Parameter relaxForEachPattern = ampl.GetParameter("rfep");
                Parameter stockLengths = ampl.GetParameter("stockLengths");
                Parameter orderLengths = ampl.GetParameter("orderLengths");
                Variable usedPatterns = ampl.GetVariable("usedPatterns");

                var lfepDataFrame = lengthForEachPattern.GetValues(); //[0] = stockIdx, [1] = orderIdx, [2] = patternPartialIdx, value = Table[stockIdx, orderIdx, patternPartialIdx, usedOrderItemsCount]
                var rfepDataFrame = relaxForEachPattern.GetValues(); //[0] = stockIdx, [1] = orderIdx, [2] = patternPartialIdx, value = Table[stockIdx, orderIdx, patternPartialIdx, relaxAmount]
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
                        int relaxSum = (int)rfepDataFrame[stockIdx, orderIdx, patternPartialIdx][3].Dbl;

                        Tuple<int, int> patternIdx = new(stockIdx, patternPartialIdx);
                        if(result.Patterns.ContainsKey(patternIdx))
                        {
                            List<RelaxableLength> relaxedLengths = distributeRelaxEvenly(usedOrderLength, usedOrderItemsCount, relaxSum);
                            result.Patterns[patternIdx].UsedOrderLengths.AddRange(relaxedLengths);
                        }
                        else
                        {
                            int usedPatternCount = (int)usedPatternDataFrame[stockIdx, patternPartialIdx][2].Dbl;
                            int stockLength = (int)stockLengths[stockIdx].Dbl;
                            List<RelaxableLength> relaxedLengths = distributeRelaxEvenly(usedOrderLength, usedOrderItemsCount, relaxSum);
                            Pattern pattern = new Pattern(stockLength, usedPatternCount, relaxedLengths);
                            result.Patterns.Add(patternIdx, pattern);
                        }
                    }
                }

                result.TotalCost = (int)totalcost.Value;
                return result;
            }
        }

        private List<RelaxableLength> distributeRelaxEvenly(int orderLength, int orderCount, int relaxSum)
        {
            List<RelaxableLength> result = [];
            int orderSum = orderLength * orderCount;
            int flooredRelaxLength = (orderSum - relaxSum) / orderCount;
            for(int i = 0; i < orderCount; ++i)
            {
                int relaxedLength = flooredRelaxLength;
                if (flooredRelaxLength * orderCount < orderSum - relaxSum - i)
                    ++relaxedLength;

                result.Add(new RelaxableLength(relaxedLength, orderLength - relaxedLength));
            }
            return result;
        }
    }
}
