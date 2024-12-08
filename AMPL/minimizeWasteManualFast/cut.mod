# ----------------------------------------
# CUTTING STOCK USING PATTERNS (Waste, Fast)
# This model is considered fast because we only need 1 iteration of knapsack subproblem.
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        			# length of raw bars [.dat]
set ORDERS;                   					# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;				# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    				# number of each width to be cut [.dat]
param maxRelax {ORDERS} >= 0;

param nPAT {STOCK} integer >= 0;      								# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      						# set of patterns in Stock
param lfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# orderLengths for each pattern   
param rfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# relax for each pattern                        		           
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 			# how many patterns of each type
				                            
minimize Waste:
	sum {i in STOCK, j in PATTERNS[i]} (stockLengths[i] - (sum{x in ORDERS} (lfep[i,x,j] * orderLengths[x] - rfep[i,x,j]))) * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} lfep[i,x,j] * usedPatterns[i,j] >= orderNum[x];
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM
# ----------------------------------------
param relaxCost default 0.01;
param price {ORDERS} default 0.0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
var StockUse {STOCK} integer >= 0;
	
minimize ReducedCost:
	sum {i in STOCK} stockLengths[i] * StockUse[i] - sum{x in ORDERS}(orderLengths[x] * Use[x] - Relax[x] + price[x]*Use[x] - Relax[x]*relaxCost);
subj to LengthLimit:
   sum {x in ORDERS} (orderLengths[x] * Use[x] - Relax[x]) <= sum {i in STOCK} stockLengths[i] * StockUse[i];
subj to OneStock:
	sum {i in STOCK} StockUse[i] = 1;
subj to MaxRelax {x in ORDERS}:
	Relax[x] <= maxRelax[x] * Use[x];