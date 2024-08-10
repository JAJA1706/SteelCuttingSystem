import React, { useEffect, useRef } from 'react'
import { Button, Paper, Divider } from '@mantine/core'
import DynamicTableStock, { Stock } from "../DataTable/DynamicTableStock"
import DynamicTableOrder, { Order } from "../DataTable/DynamicTableOrder"
import ResultTable from "../ResultTable/ResultTable"
import classes from "./MainContent.module.css"
import useSolveCuttingStockProblem from "../../hooks/useSolveCuttingStockProblem"

interface MainContentProps {
    setResetFunction: (resetFunction: () => void) => void,
}

const MainContent = ({ setResetFunction }: MainContentProps) => {
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

    const onHandleReset = () => {
        console.log("reset");
    }

    useEffect(() => {
        setResetFunction(onHandleReset);
    }, [setResetFunction])

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
                            data={solveCuttingStockMutation.data?.resultItems ?? []}
                        />
                    </div>
                </Paper>
            </div>
        </div>
    );
}

export default MainContent