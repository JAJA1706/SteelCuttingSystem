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
                Parameter wfep = a.GetParameter("wfep");
                Variable lol = a.GetVariable("usedPatterns");

                var what = totalcost.Value;
                var ehh = wfep.GetValues();
                List<double> jebsie = new List<double>();
                foreach (var item in ehh)
                {
                    var bruh = item[3];
                    if (bruh.Dbl != 0)
                    {
                        //Console.WriteLine(bruh.Dbl);
                        jebsie.Add(bruh.Dbl);
                    }
                }
                foreach (var item in lol)
                {
                    var bruh = item.Name;
                    Console.WriteLine(bruh);
                    var bruh2 = item.Value;
                    Console.WriteLine(bruh2);
                }

            }

            return new AmplResult();
        }
    }
}
