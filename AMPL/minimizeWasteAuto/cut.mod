# ----------------------------------------
# CUTTING STOCK USING PATTERNS (Waste, Auto)
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        			# length of raw bars [.dat]
param stockNum {STOCK} > 0;						# limit of used raw bars [.dat]
set ORDERS;                   					# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;				# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    				# number of each width to be cut [.dat]
param canBeRelaxed {ORDERS} default 0;			# can we relax this length [.dat]

param nPAT {STOCK} integer >= 0;      								# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      						# set of patterns in Stock
param lfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# orderLengths for each pattern
param rfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# relax for each pattern               
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 			# how many patterns of each type
				                            
minimize Waste:
	sum {i in STOCK, j in PATTERNS[i]} (stockLengths[i] - sum{x in ORDERS} (lfep[i,x,j] * orderLengths[x] - rfep[i,x,j])) * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} lfep[i,x,j] * usedPatterns[i,j] >= orderNum[x];
subj to StockLimit {i in STOCK}:
	sum{ j in PATTERNS[i]} usedPatterns[i,j] <= stockNum[i];
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM AUTO RELAX
# ----------------------------------------
param price {ORDERS} default 0.0;
param stockLength default 0;
param relaxCost {ORDERS} default 0.00001;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
	
minimize ReducedCost:
   stockLength - sum{x in ORDERS}(orderLengths[x] * Use[x] - Relax[x] + price[x]*Use[x] - Relax[x]*relaxCost[x]);
subj to LengthLimit:
   sum {x in ORDERS} (orderLengths[x] * Use[x] - Relax[x]) <= stockLength;
subj to RelaxLimit {x in ORDERS}: #bar length cannot be shorter than 1
	(orderLengths[x]-1) * Use[x] >= Relax[x];
subj to IsRelaxationAllowed {x in ORDERS}:
	Relax[x] = Relax[x] * canBeRelaxed[x];
	
	
#additional params for cut.run
param allowBasicPatterns default 0;
param relaxCostMultiplier default 1;