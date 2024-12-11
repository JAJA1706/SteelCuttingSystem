﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelCutOptimizer.Server.Tests.Structs
{
    public class ProblemGenerationDefinition
    {
        public int Size { get; set; }
        public int StockLength { get; set; }
        public double OrderLengthLowerBound { get; set; }
        public double OrderLengthUpperBound { get; set; }
        public int AverageDemand { get; set; }
    }
}
