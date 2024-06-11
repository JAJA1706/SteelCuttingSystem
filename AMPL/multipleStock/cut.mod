# ----------------------------------------
# CUTTING STOCK USING PATTERNS
# ----------------------------------------
set STOCK;
param stockWidths {STOCK} > 0;        		# width of raw bars [.dat]
#param stockNum {STOCK} > 0;        		# num of raw bars [.dat]
set ORDERS;                   				# set of widths to be cut [.dat]
param widths {ORDERS} > 0;					# width of ordered bars [.dat]
param barsNum {ORDERS} > 0;    				# number of each width to be cut [.dat]
param maxRelax {ORDERS};	  				# max relaxation percentage [.dat]
				                            
param nPAT {STOCK} integer >= 0;      								# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      					# set of patterns in Stock
param wfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;			# widths for each pattern
								                            		# defn of patterns: nbr[i,j] = number
								                            		# of bars of width i in pattern j	                            
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 		#how many patterns of each type
				                            
#param rfep i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;	#relax for each pattern
															#only for display purposes
	
minimize Waste:
	sum {i in STOCK, j in PATTERNS[i]} (stockWidths[i] - (sum{x in ORDERS} wfep[i,x,j] * widths[x])) * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} wfep[i,x,j] * usedPatterns[i,j] >= barsNum[x];

#subj to StockLimit {i in STOCK}:
#	stockNum[i] >= sum{ j in PATTERNS[i]} usedPatterns[i,j];
							
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM WITH RELAXATION
# ----------------------------------------
param relaxCost default 0.00000001;
param price {ORDERS} default 0.0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
var StockUse {STOCK} integer >= 0;
	
minimize ReducedCost:
	sum {i in STOCK} stockWidths[i] * StockUse[i] - sum{x in ORDERS}(widths[x] * Use[x] + price[x]*Use[x]);
subj to WidthLimit:
   sum {x in ORDERS} (widths[x] * Use[x]) <= sum {i in STOCK} stockWidths[i] * StockUse[i];
subj to oneStock:
	sum {i in STOCK} StockUse[i] = 1;
	