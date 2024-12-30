# ----------------------------------------
# CUTTING STOCK USING PATTERNS (Manual)
# This model solves typical cutting stock problem with multiple stock items, but tries to
# find new interesting patterns by shortening orderLengs by specific amount. Size of relaxation
# depends on maxRelax value which User needs to pass to the model.
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        		# length of raw bars [.dat]
param stockNum {STOCK} > 0;        			# num of raw bars [.dat]
param stockCost {STOCK} > 0;				# cost of raw bars [.dat]
set ORDERS;                   				# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;			# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    			# number of each length to be cut [.dat]
param maxRelax {ORDERS};	  				# max relaxation percentage [.dat]
				                            
param nPAT {STOCK} integer >= 0;      							# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      					# set of patterns in STOCK
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
# KNAPSACK SUBPROBLEM WITH RELAXATION
# ----------------------------------------
param relaxCost {ORDERS} default 0.00001;
param price {ORDERS} default 0.0;
param rawBarLength > 0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
	
maximize ReducedCost:
   sum {x in ORDERS} (price[x] * Use[x] - Relax[x] * relaxCost[x]);
subj to LengthLimit:
   sum {x in ORDERS} (orderLengths[x] * Use[x] - Relax[x]) <= rawBarLength;
subj to RelaxLimit {x in ORDERS}:
	Relax[x] <= maxRelax[x]*Use[x];
	
	
#additional params for cut.run
param relaxCostMultiplier default 1;