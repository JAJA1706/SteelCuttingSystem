using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.DTO;
using System.Text;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public class MultipleStockDataConverter : IAmplDataConverter
    {
        public void AdjustEntryData(CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null || data.StockList == null)
                throw new InvalidDataException("Item list cannot be empty");

            foreach (var stock in data.StockList)
            {
                if (stock.Cost == null)
                    stock.Cost = 1;
                if (stock.Count == null)
                    stock.Count = Int32.MaxValue;
            }

            foreach(var order in data.OrderList)
            {
                if (order.MaxRelax == null)
                    order.MaxRelax = 0;
            }
        }

        public void ConvertToAmplDataFile(string dataFilePath, CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null || data.StockList == null)
                throw new InvalidDataException("Item list cannot be empty");

            var fileContent = new StringBuilder();

            fileContent.AppendLine("data;");
            fileContent.AppendLine("param: ORDERS: orderLengths  orderNum  maxRelax :=");
            foreach (var order in data.OrderList)
            {
                fileContent.AppendLine($"{data.OrderList.IndexOf(order) + 1}  {order.Length}  {order.Count}  {order.MaxRelax}");
            }
            fileContent.Append(';');

            fileContent.AppendLine();
            fileContent.AppendLine("param: STOCK: stockLengths stockNum stockCost :=");
            foreach (var stock in data.StockList)
            {
                fileContent.AppendLine($"{data.StockList.IndexOf(stock) + 1}  {stock.Length}  {stock.Count}  {stock.Cost}");
            }
            fileContent.AppendLine(";");

            if(!Directory.Exists(dataFilePath)) {
                Directory.CreateDirectory(dataFilePath);
            }

            File.WriteAllText(dataFilePath + "/cut.dat", fileContent.ToString());
        }

        public CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult amplResult)
        {
            CuttingStockResultsDTO dto = new ();
            foreach(var pattern in amplResult.Patterns)
            {
                ResultItem item = new ResultItem();
                item.StockLength = pattern.Value.StockLength;
                item.UsedOrderLengths = pattern.Value.UsedOrderLengths;
                item.Count = pattern.Value.UseCount;
                if(item.Count > 0)
                {
                    dto.ResultItems.Add(item);
                }
            }

            dto.ResultItems.Sort((itemX, itemY) => 
                itemX.UsedOrderLengths.Count().CompareTo(itemY.UsedOrderLengths.Count())
            );

            for(int i = 0; i < dto.ResultItems.Count; ++i)
            {
                dto.ResultItems[i].PatternId = i + 1;
            }

            return dto;
        }

        public void ValidateResultData(AmplResult amplResult, CuttingStockProblemDataDTO entryData)
        {
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
                if (stock.Count < usedStockLengths[stock.Length])
                    throw new InvalidDataException("infeasible problem");
            }
        }
    }
}
