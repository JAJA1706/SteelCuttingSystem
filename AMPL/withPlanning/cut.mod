# ----------------------------------------
# CUTTING STOCK USING PATTERNS
# ----------------------------------------
param rawBarWidth > 0;        	# width of raw bars [.dat]
set ORDERS;                   	# set of widths to be cut [.dat]
param widths {ORDERS} > 0;		# width of ordered bars [.dat]
param barsNum {ORDERS} > 0;    	# number of each width to be cut [.dat]

param stageNum integer > 0;		# how long will construction last
set STAGES := 1..daysNum;
param plan  {ORDERS, STAGES} integer; #plan of construction (number of bars for each stage of construction)

param nPAT integer >= 0;      	# number of patterns
set PATTERNS := 1..nPAT;      	# set of patterns
param wfep {ORDERS,PATTERNS} integer >= 0;	#widths for each pattern
				                            # defn of patterns: nbr[i,j] = number
				                            # of bars of width i in pattern j

var Cut {PATTERNS, STAGES} integer >= 0;	# bars cut using each pattern for each stage

minimize Number:					# minimize total raw bars cut
	sum {j in PATTERNS, k in STAGES} Cut[j,k];
subj to Fill {i in ORDERS}:			#bars cut meets total orders
	sum {j in PATTERNS, k in STAGES} wfep[i,j] * Cut[j,k] >= barsNum[i];
#subj to FillPlan {i in ORDERS}:
	
							
   
# ----------------------------------------
# KNAPSACK SUBPROBLEM WITH RELAXATION
# ----------------------------------------
param price {ORDERS} default 0.0;
var Use {ORDERS} integer >= 0;

maximize ReducedCost:
   sum {i in ORDERS} (price[i] * Use[i]) - 1;
subj to WidthLimit:
   sum {i in ORDERS} (widths[i] * Use[i]) <= rawBarWidth;