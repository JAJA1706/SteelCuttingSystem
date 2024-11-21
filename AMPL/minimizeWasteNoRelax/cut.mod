# ----------------------------------------
# CUTTING STOCK USING PATTERNS (Waste, no relax)
# This model looks for the patterns which minimizes amount of produced Waste. Be aware that
# solution given here will often include much more order items than necessary.
# We cannot include stockCost here because we would need to include some sort of cost
# of waste, which is not so straightforward.
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        			# length of raw bars [.dat]
param stockNum {STOCK} > 0;						# limit of used raw bars [.dat]
set ORDERS;                   					# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;				# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    				# number of each width to be cut [.dat]
				                            
param nPAT {STOCK} integer >= 0;      								# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      						# set of patterns in Stock
param lfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# orderLengths for each pattern                          
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 			# how many patterns of each type
				                            
minimize Waste:
	sum {i in STOCK, j in PATTERNS[i]} (stockLengths[i] - (sum{x in ORDERS} lfep[i,x,j] * orderLengths[x])) * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} lfep[i,x,j] * usedPatterns[i,j] >= orderNum[x];
subj to StockLimit {i in STOCK}:
	sum{ j in PATTERNS[i]} usedPatterns[i,j] <= stockNum[i];
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM
# ----------------------------------------
param price {ORDERS} default 0.0;
param stockLength default 0;
var Use {ORDERS} integer >= 0;
	
minimize ReducedCost:
   stockLength - sum{x in ORDERS}(orderLengths[x] * Use[x] + price[x]*Use[x]);
subj to LengthLimit:
   sum {x in ORDERS} (orderLengths[x] * Use[x]) <= stockLength;
	