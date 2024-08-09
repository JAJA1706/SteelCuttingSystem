import React, { useRef } from 'react'
import { Button, Paper, Divider } from '@mantine/core'
import DynamicTableStock, { Stock } from "../DataTable/DynamicTableStock"
import DynamicTableOrder, { Order } from "../DataTable/DynamicTableOrder"
import ResultTable from "../ResultTable/ResultTable"
import classes from "./MainContent.module.css"
import useSolveCuttingStockProblem from "../../hooks/useSolveCuttingStockProblem"

const MainContent = () => {
    const resultData = [
        {
            id: '1',
            patternNum: 1,
            count: 1,
            usedBars: [1]
        },
        {
            id: '2',
            patternNum: 2,
            count: 2,
            usedBars: [1]
        },
        {
            id: '3',
            patternNum: 3,
            count: 3,
            usedBars: [1, 2, 3, 4, 5 ,6 ,7 ,8, 9, 10, 11, 12]
        },
        {
            id: '4',
            patternNum: 4,
            count: 4,
            usedBars: [1, 2]
        },
        {
            id: '5',
            patternNum: 5,
            count: 5,
            usedBars: [1, 2]
        }
    ]

    const stockDataRef = useRef<Stock[]>([]);
    const orderDataRef = useRef<Order[]>([]);

    const solveCuttingStockMutation = useSolveCuttingStockProblem();

    const onGenerateResultClick = () => {
        const requestBody = {
            problemType: "MultipleStock",
            stockList: [...stockDataRef.current],
            orderList: [...orderDataRef.current],
        };
        solveCuttingStockMutation.mutate(requestBody);
    }

    return (
        <div className={classes.layout}>
            <div className={classes.mainBody}>
                <Paper className={classes.upperBody} shadow="md" withBorder>
                    <div className={classes.dynTables}>
                        <div className={classes.table}>
                            <DynamicTableStock dataRef={stockDataRef} />
                        </div>
                        <div className={classes.table}>
                            <DynamicTableOrder dataRef={orderDataRef} />
                        </div>
                    </div>
                    <Button onClick={onGenerateResultClick}>
                        Generate Result
                    </Button>
                </Paper>
                <Divider my="md" />
                <Paper className={classes.resultTable} shadow="md" withBorder>
                    <div>
                        <ResultTable
                            data={resultData}
                        />
                    </div>
                </Paper>
            </div>
        </div>
    );
}

export default MainContent