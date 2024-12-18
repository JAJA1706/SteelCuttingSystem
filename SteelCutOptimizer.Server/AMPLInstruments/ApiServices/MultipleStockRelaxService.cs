using ampl.Entities;
using ampl;
using SteelCutOptimizer.Server.Structs;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public class MultipleStockRelaxService : IAmplApiService
    {
        private readonly string modelPath;
        private readonly AlgorithmSettings settings;

        public MultipleStockRelaxService(AlgorithmSettings _settings)
        {
            settings = _settings;
            if (settings.MainObjective == "cost")
            {
                switch (settings.RelaxationType)
                {
                    case "manual":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeCostManual/";
                        break;
                    case "auto":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeCostAuto/";
                        break;
                    case "singleStep":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeCostSingleStep/";
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (settings.MainObjective == "waste")
            {
                switch (settings.RelaxationType)
                {
                    case "manual":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeWasteManual/";
                        break;
                    case "manualFast":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeWasteManualFast/";
                        break;
                    case "auto":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeWasteAuto/";
                        break;
                    case "singleStep":
                        modelPath = AppDomain.CurrentDomain.BaseDirectory + "AmplModels/minimizeWasteSingleStep/";
                        break;
                    default:
                        throw new NotImplementedException();
                }
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
                initializeSolver(ampl);
                ampl.Read(Path.Combine(modelPath, "cut.run"));

                Parameter lengthForEachPattern = ampl.GetParameter("lfep");
                Parameter relaxForEachPattern = ampl.GetParameter("rfep");
                Parameter stockLengths = ampl.GetParameter("stockLengths");
                Parameter orderLengths = ampl.GetParameter("orderLengths");
                Parameter isFeasible = ampl.GetParameter("isFeasible");
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
                        if (result.Patterns.ContainsKey(patternIdx))
                        {
                            List<Segment> relaxedLengths = distributeRelaxEvenly(orderIdx, usedOrderLength, usedOrderItemsCount, relaxSum);
                            result.Patterns[patternIdx].SegmentList.AddRange(relaxedLengths);
                        }
                        else
                        {
                            int usedPatternCount = (int)usedPatternDataFrame[stockIdx, patternPartialIdx][2].Dbl;
                            int stockLength = (int)stockLengths[stockIdx].Dbl;
                            List<Segment> relaxedLengths = distributeRelaxEvenly(orderIdx, usedOrderLength, usedOrderItemsCount, relaxSum);
                            Pattern pattern = new Pattern(stockIdx, stockLength, usedPatternCount, relaxedLengths);
                            result.Patterns.Add(patternIdx, pattern);
                        }
                    }
                }

                var isFeasibleVar = isFeasible.Get();
                if (isFeasibleVar != null && isFeasibleVar.Dbl == 0)
                    result.IsFeasible = false;

                if (settings.MainObjective == "cost")
                    result.TotalCost = (int)ampl.GetObjective("Cost").Value;
                else
                    result.TotalWaste = (int)ampl.GetObjective("Waste").Value;


                if (settings.RelaxationType == "singleStep")
                {
                    Parameter orderPrices = ampl.GetParameter("price");
                    Constraint stockLimit = ampl.GetConstraint("StockLimit");

                    var stockLimitDataFrame = stockLimit.GetValues();
                    foreach (var limit in stockLimitDataFrame)
                    {
                        result.StockLimits.Add(limit[1].Dbl);
                    }

                    var priceDataFrame = orderPrices.GetValues();
                    foreach (var price in priceDataFrame)
                    {
                        result.OrderPrices.Add(price[1].Dbl);
                    }
                }
                return result;
            }
        }

        private List<Segment> distributeRelaxEvenly(int orderId, int orderLength, int orderCount, int relaxSum)
        {
            List<Segment> result = [];
            int orderSum = orderLength * orderCount;
            int flooredRelaxLength = (orderSum - relaxSum) / orderCount;
            for (int i = 0; i < orderCount; ++i)
            {
                int relaxedLength = flooredRelaxLength;
                if (flooredRelaxLength * orderCount < orderSum - relaxSum - i)
                    ++relaxedLength;

                result.Add(new Segment(orderId, relaxedLength, orderLength - relaxedLength));
            }
            return result;
        }

        private void initializeSolver(AMPL ampl)
        {
            if (settings.Solver != "cbc" && settings.Solver != "highs" && settings.Solver != "cplex")
                throw new NotImplementedException();

            ampl.SetOption("solver", settings.Solver);
            ampl.SetOption($"{settings.Solver}_options", "timelimit=240");
        }
    }
}
