using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelCutOptimizer.Server.Tests.Attributes
{
    public class EfficiencyFactAttribute : FactAttribute
    {
        public EfficiencyFactAttribute()
        {
        #if TestEfficiency
                // run efficiency tests
        #else
                Skip = "Skipped due to TestEfficiency being disabled";
        #endif
        }
    }
}
