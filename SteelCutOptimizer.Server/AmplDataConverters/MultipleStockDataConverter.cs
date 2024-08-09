using SteelCutOptimizer.Server.AmplApiServices;
using SteelCutOptimizer.Server.DTO;
using System.Text;

namespace SteelCutOptimizer.Server.AmplDataConverters
{
    public class MultipleStockDataConverter : IAmplDataConverter
    {
        public void ConvertToAmplDataFile(string dataFilePath, CuttingStockProblemDataDTO data)
        {
            if (data.OrderList == null || data.StockList == null)
                throw new InvalidDataException("Item list cannot be empty");

            var fileContent = new StringBuilder();

            fileContent.AppendLine("data;");
            fileContent.AppendLine("param: ORDERS: widths  barsNum  maxRelax :=");
            foreach (var order in data.OrderList)
            {
                fileContent.AppendLine($"{data.OrderList.IndexOf(order) + 1}  {order.Length}  {order.Count}  {order.MaxRelax}");
            }
            fileContent.Append(';');

            fileContent.AppendLine();
            fileContent.AppendLine("param: STOCK: stockWidths stockNum stockCost :=");
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

        public CuttingStockResultsDTO ConvertResultDataToDTO(AmplResult result)
        {
            return new CuttingStockResultsDTO(); ;
        }
    }
}
