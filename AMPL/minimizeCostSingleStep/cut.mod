# ----------------------------------------
# CUTTING STOCK USING PATTERNS (SingleStep)
# This model solves typical cutting stock problem with multiple stock items. In the next stape
# it looks for the next pattern which helps to reduce cost but has the lowest possible relaxation applied.
# Starting lfep and rfep needs to be applied through the .dat file.
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        		# length of raw bars [.dat]
param stockNum {STOCK} > 0;        			# num of raw bars [.dat]
param stockCost {STOCK} > 0;				# cost of raw bars [.dat]
set ORDERS;                   				# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;			# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    			# number of each length to be cut [.dat]
param canBeRelaxed {ORDERS};	  			# can we relax this length in second step [.dat]
				                            
param nPAT {STOCK} integer >= 0;      							# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      					# set of patterns in Stock
param lfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;		# orderLengths for each pattern
param rfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;		# relax for each pattern                            
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 		# how many patterns of each type
	
minimize Cost:
	sum {i in STOCK, j in PATTERNS[i]} stockCost[i] * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} lfep[i,x,j] * usedPatterns[i,j] >= orderNum[x];
subj to StockLimit {i in STOCK}:
	sum{ j in PATTERNS[i]} usedPatterns[i,j] <= stockNum[i];
							
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM WITH MIN RELAX
# ----------------------------------------
param price {ORDERS} default 0.0;
param rawBarLength > 0;
param minNewPatternCost > 0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
	
minimize RelaxSum:
   sum {i in ORDERS} Relax[i];
subj to PatternCost:
	sum {i in ORDERS} price[i] * Use[i] >= minNewPatternCost;
subj to LengthLimitR:
   sum {i in ORDERS} (orderLengths[i] * Use[i] - Relax[i]) <= rawBarLength;
subj to RelaxLimit {i in ORDERS}: #bar length cannot be shorter than 1 + without this constraint relaxation would be assigned randomly
	(orderLengths[i]-1) * Use[i] >= Relax[i];
subj to IsRelaxationAllowed {i in ORDERS}:
	Relax[i] = Relax[i] * canBeRelaxed[i];
	
#additional params for cut.run
param prevStockLimit {STOCK} default 0;
param stockItemToRelax default 1;