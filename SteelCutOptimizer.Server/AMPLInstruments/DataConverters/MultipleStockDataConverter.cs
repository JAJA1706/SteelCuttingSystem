using SteelCutOptimizer.Server.DTO;
using System.Text;
using SteelCutOptimizer.Server.Structs;
using System.Data;

namespace SteelCutOptimizer.Server.AMPLInstruments
{
    public class MultipleStockDataConverter : IAmplDataConverter
    {
        private string dataFilePath = AppDomain.CurrentDomain.BaseDirectory + "data/multipleStockExtended/";
        private readonly AlgorithmSettings settings;
        public MultipleStockDataConverter(AlgorithmSettings _settings, string dataId)
        {
            settings = _settings;
            dataFilePath += $"/{dataId}/";
        }

        public void AdjustEntryData(CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null || data.StockList == null ||
                data.OrderList.Count == 0 || data.StockList.Count == 0)
            {
                throw new InvalidDataException("Item list cannot be empty");
            }

            foreach (var stock in data.StockList)
            {
                if (stock.Cost == null)
                    stock.Cost = 1;
                if (stock.Count == null)
                    stock.Count = int.MaxValue;
                if (stock.NextStepGeneration == null)
                    stock.NextStepGeneration = false;
            }

            foreach (var order in data.OrderList)
            {
                if (order.MaxRelax == null)
                    order.MaxRelax = 0;
                if (order.CanBeRelaxed == null)
                    order.CanBeRelaxed = false;
            }
        }

        //returns path to the created .dat file
        public string ConvertToAmplDataFile(CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null || data.StockList == null)
                throw new InvalidDataException("Item list cannot be empty");

            var fileContent = new StringBuilder();


            prepareOrderSet(fileContent, data);
            fileContent.AppendLine();
            prepareStockSet(fileContent, data);
            fileContent.AppendLine();
            prepareAdditionalDataManual(fileContent, data);
            prepareAdditionalDataAuto(fileContent, data);
            prepareAdditionalDataSingleStep(fileContent, data);

            if (!Directory.Exists(dataFilePath))
            {
                Directory.CreateDirectory(dataFilePath);
            }

            File.WriteAllText(dataFilePath + "/cut.dat", fileContent.ToString());

            return dataFilePath;
        }

        private void prepareOrderSet(StringBuilder fileContent, CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null)
                throw new InvalidDataException("Order list cannot be empty");

            fileContent.AppendLine("data;");
            if (settings.RelaxationType == "manual" || settings.RelaxationType == "manualFast")
            {
                fileContent.AppendLine("param: ORDERS: orderLengths  orderNum  maxRelax :=");
                foreach (var order in data.OrderList)
                {
                    fileContent.AppendLine($"{data.OrderList.IndexOf(order) + 1}  {order.Length}  {order.Count}  {order.MaxRelax}");
                }
            }
            else if (settings.RelaxationType == "auto" || settings.RelaxationType == "singleStep")
            {
                fileContent.AppendLine("param: ORDERS: orderLengths  orderNum  canBeRelaxed :=");
                foreach (var order in data.OrderList)
                {
                    fileContent.AppendLine($"{data.OrderList.IndexOf(order) + 1}  {order.Length}  {order.Count}  {convertBoolToInt(order.CanBeRelaxed)}");
                }
            }
            else if (settings.RelaxationType == "none")
            {
                fileContent.AppendLine("param: ORDERS: orderLengths  orderNum :=");
                foreach (var order in data.OrderList)
                {
                    fileContent.AppendLine($"{data.OrderList.IndexOf(order) + 1}  {order.Length}  {order.Count}");
                }
            }
            else
            {
                throw new InvalidDataException("Unknown Relaxation Type");
            }
            fileContent.Append(';');
        }

        private static int convertBoolToInt(bool? x)
        {
            return x != null && x == true ? 1 : 0;
        }

        private void prepareStockSet(StringBuilder fileContent, CuttingStockProblemDataDTO data)
        {
            if (data.StockList == null)
                throw new InvalidDataException("Stock list cannot be empty");

            if (settings.MainObjective == "cost")
            {
                fileContent.AppendLine("param: STOCK: stockLengths stockNum stockCost :=");
                foreach (var stock in data.StockList)
                {
                    fileContent.AppendLine($"{data.StockList.IndexOf(stock) + 1}  {stock.Length}  {stock.Count}  {stock.Cost}");
                }
            }
            else if (settings.MainObjective == "waste")
            {
                fileContent.AppendLine("param: STOCK: stockLengths stockNum :=");
                foreach (var stock in data.StockList)
                {
                    fileContent.AppendLine($"{data.StockList.IndexOf(stock) + 1}  {stock.Length}  {stock.Count}");
                }
            }
            else
            {
                throw new InvalidDataException("Unknown Main Objective");
            }
            fileContent.AppendLine(";");
        }

        private void prepareAdditionalDataManual(StringBuilder fileContent, CuttingStockProblemDataDTO data)
        {
            if (settings.RelaxationType != "manual" && settings.RelaxationType != "manualFast")
                return;

            if (data.RelaxCostMultiplier != null)
            {
                fileContent.AppendLine($"param relaxCostMultiplier := {data.RelaxCostMultiplier.ToString()!.Replace(',', '.')};");
            }
        }

        private void prepareAdditionalDataAuto(StringBuilder fileContent, CuttingStockProblemDataDTO data)
        {
            if (settings.RelaxationType != "auto")
                return;

            if (data.RelaxCostMultiplier != null)
            {
                fileContent.AppendLine($"param relaxCostMultiplier := {data!.RelaxCostMultiplier.ToString()!.Replace(',', '.')};");
            }
            if (data.AreBasicPatternsAllowed != null)
            {
                fileContent.AppendLine($"param allowBasicPatterns := {convertBoolToInt(data.AreBasicPatternsAllowed)};");
            }
        }
        private void prepareAdditionalDataSingleStep(StringBuilder fileContent, CuttingStockProblemDataDTO data)
        {
            if (settings.RelaxationType != "singleStep")
                return;

            int stockItemIdxToRelax = data.StockList!.FindIndex(x => x.NextStepGeneration == true);
            if (stockItemIdxToRelax != -1)
            {
                fileContent.AppendLine($"param stockItemToRelax := {stockItemIdxToRelax + 1};");
            }

            if (data.Patterns != null && data.Patterns.Count > 0)
            {
                Dictionary<int, int> patternCountForStockItem = new Dictionary<int, int>();
                for (int stockIdx = 1; stockIdx <= data.StockList!.Count; ++stockIdx)
                {
                    patternCountForStockItem[stockIdx] = 0; //stockItems and orderItems are indexed from 1
                }

                fileContent.AppendLine("param lfep :=");
                foreach (var pattern in data.Patterns)
                {
                    int stockIdx = pattern.StockId;
                    ++patternCountForStockItem[stockIdx];
                    int patternIdx = patternCountForStockItem[stockIdx];

                    for (int orderIdx = 1; orderIdx <= data.OrderList!.Count; ++orderIdx)
                    {
                        int orderUseCount = pattern.SegmentList.Count(x => x.OrderId == orderIdx);
                        fileContent.Append($"[{stockIdx},{orderIdx},{patternIdx}] {orderUseCount}, ");
                    }
                    fileContent.AppendLine();
                }
                fileContent.Replace(",", ";", fileContent.Length - 4, 2); //new line can contain 2 characters


                for (int stockIdx = 1; stockIdx <= data.StockList!.Count; ++stockIdx)
                {
                    patternCountForStockItem[stockIdx] = 0;
                }

                fileContent.AppendLine("param rfep :=");
                foreach (var pattern in data.Patterns)
                {
                    int stockIdx = pattern.StockId;
                    ++patternCountForStockItem[stockIdx];
                    int patternIdx = patternCountForStockItem[stockIdx];

                    for (int orderIdx = 1; orderIdx <= data.OrderList!.Count; ++orderIdx)
                    {
                        List<Segment> segments = new List<Segment>();
                        int relaxSumForOrderItem = pattern.SegmentList.Where(x => x.OrderId == orderIdx).Sum(x => x.RelaxAmount);
                        fileContent.Append($"[{stockIdx},{orderIdx},{patternIdx}] {relaxSumForOrderItem}, ");
                    }
                    fileContent.AppendLine();
                }
                fileContent.Replace(",", ";", fileContent.Length - 4, 2);

                fileContent.Append("param nPAT := ");
                for (int stockIdx = 1; stockIdx <= data.StockList!.Count; ++stockIdx)
                {
                    fileContent.Append($"[{stockIdx}] {patternCountForStockItem[stockIdx]}, ");
                }
                fileContent.Replace(",", ";", fileContent.Length - 2, 1);
                fileContent.AppendLine();
            }

            if (data.OrderPrices != null && data.OrderPrices.Count > 0)
            {
                fileContent.Append("param price := ");
                for (int i = 0; i < data.OrderPrices.Count; ++i)
                    fileContent.Append($"[{i + 1}] {data.OrderPrices[i].ToString().Replace(',', '.')}, ");
                fileContent.Replace(",", ";", fileContent.Length - 2, 1);
                fileContent.AppendLine();
            }

            if (data.StockLimits != null && data.StockList.Count > 0)
            {
                fileContent.AppendLine("param prevStockLimit := ");
                for (int i = 0; i < data.StockLimits.Count; ++i)
                    fileContent.Append($"[{i + 1}] {data.StockLimits[i].ToString().Replace(',', '.')}, ");
                fileContent.Replace(",", ";", fileContent.Length - 2, 1);
                fileContent.AppendLine();
            }
        }

        public void DisposeDataFile()
        {
            Directory.Delete(dataFilePath, true);
        }

        public CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult amplResult)
        {
            CuttingStockResultsDTO dto = new();
            foreach (var pattern in amplResult.Patterns)
            {
                ResultPattern item = new ResultPattern();
                item.StockLength = pattern.Value.StockLength;
                item.StockId = pattern.Value.StockId;
                item.SegmentList = pattern.Value.SegmentList;
                item.UseCount = pattern.Value.UseCount;

                if (item.UseCount > 0
                    || settings.RelaxationType == "none"
                    || settings.RelaxationType == "singleStep"
                    )
                {
                    dto.Patterns.Add(item);
                }
            }

            dto.OrderPrices = amplResult.OrderPrices;
            dto.StockLimits = amplResult.StockLimits;
            dto.TotalCost = amplResult.TotalCost;
            dto.TotalWaste = amplResult.TotalWaste ?? calculateTotalWaste(dto.Patterns);
            dto.TotalRelax = calculateTotalRelax(dto.Patterns);

            if (settings.RelaxationType == "singleStep")
            {
                dto.Patterns.Sort((itemX, itemY) =>
                {
                    int relaxSumX = itemX.SegmentList.Sum(o => o.RelaxAmount);
                    int relaxSumY = itemY.SegmentList.Sum(o => o.RelaxAmount);
                    return relaxSumY.CompareTo(relaxSumX);
                });
            }

            for (int i = 0; i < dto.Patterns.Count; ++i)
            {
                dto.Patterns[i].PatternId = i + 1;
            }

            return dto;
        }

        private static int calculateTotalWaste(List<ResultPattern> Patterns)
        {
            int stockLengthSum = 0;
            int segmentsLengthSum = 0;
            foreach (var pattern in Patterns)
            {
                stockLengthSum += pattern.StockLength * pattern.UseCount;
                foreach (var segment in pattern.SegmentList)
                {
                    segmentsLengthSum += segment.Length * pattern.UseCount;
                }
            }
            return stockLengthSum - segmentsLengthSum;
        }

        private static int calculateTotalRelax(List<ResultPattern> Patterns)
        {
            int totalRelax = 0;
            foreach (var pattern in Patterns)
            {
                foreach (var segment in pattern.SegmentList)
                {
                    totalRelax += segment.RelaxAmount * pattern.UseCount;
                }
            }
            return totalRelax;
        }
    }
}
