# ----------------------------------------
# CUTTING STOCK USING PATTERNS
# ----------------------------------------
set STOCK;
param stockWidths {STOCK} > 0;        		# width of raw bars [.dat]
param stockNum {STOCK} > 0;        			# num of raw bars [.dat]
param stockCost {STOCK} > 0;
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
				                            
param rfep {i in STOCK,ORDERS,PATTERNS[i]} integer >= 0;	#relax for each pattern
															#only for display purposes
	
minimize Cost:
	sum {i in STOCK, j in PATTERNS[i]} stockCost[i] * usedPatterns[i,j];
subj to FillOrder {x in ORDERS}:
	sum {i in STOCK, j in PATTERNS[i]} wfep[i,x,j] * usedPatterns[i,j] >= barsNum[x];
subj to StockLimit {i in STOCK}:
	sum{ j in PATTERNS[i]} usedPatterns[i,j] <= stockNum[i];
							
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM WITH RELAXATION
# ----------------------------------------
param relaxCost default 0.0001;
param price {ORDERS} default 0.0;
param rawBarWidth > 0;
var Use {ORDERS} integer >= 0;
var Relax {ORDERS} integer >= 0;
	
maximize ReducedCost:
   sum {i in ORDERS} (price[i] * Use[i] - Relax[i] * relaxCost);
subj to WidthLimit:
   sum {i in ORDERS} (widths[i] * Use[i] - Relax[i]) <= rawBarWidth;
subj to RelaxLimit {i in ORDERS}:
	Relax[i] <= maxRelax[i]*Use[i];