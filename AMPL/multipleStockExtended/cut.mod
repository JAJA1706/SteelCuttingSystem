# ----------------------------------------
# CUTTING STOCK USING PATTERNS
# ----------------------------------------
set STOCK;
param stockLengths {STOCK} > 0;        		# length of raw bars [.dat]
param stockNum {STOCK} > 0;        			# num of raw bars [.dat]
param stockCost {STOCK} > 0;
set ORDERS;                   				# set of orderLengths to be cut [.dat]
param orderLengths {ORDERS} > 0;			# length of ordered bars [.dat]
param orderNum {ORDERS} > 0;    			# number of each length to be cut [.dat]
param maxRelax {ORDERS};	  				# max relaxation percentage [.dat]
				                            
param nPAT {STOCK} integer >= 0;      							# number of patterns for each STOCK
set PATTERNS {i in STOCK} := 1..nPAT[i];      					# set of patterns in Stock
param lfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;		# orderLengths for each pattern
								                            	# defn of patterns: nbr[i,j] = number
								                            	# of bars of length i in pattern j	                            
var usedPatterns {i in STOCK, PATTERNS[i]} integer >= 0; 		#how many patterns of each type
				                            
param rfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;		#relax for each pattern
																#only for display purposes
	
minimize Cost:
	sum {i in STOCK, j in PATTERNS[i]} stockCost[i] * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} lfep[i,x,j] * usedPatterns[i,j] >= orderNum[x];
subj to StockLimit {i in STOCK}:
	sum{ j in PATTERNS[i]} usedPatterns[i,j] <= stockNum[i];
							
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM WITH RELAXATION
# ----------------------------------------
param relaxCost default 0.0001;
param price {ORDERS} default 0.0;
param rawBarLength > 0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
	
maximize ReducedCost:
   sum {i in ORDERS} (price[i] * Use[i] - Relax[i] * relaxCost);
subj to LengthLimit:
   sum {i in ORDERS} (orderLengths[i] * Use[i] - Relax[i]) <= rawBarLength;
subj to RelaxLimit {i in ORDERS}:
	Relax[i] <= maxRelax[i]*Use[i];