import { useCallback, useEffect, useRef } from 'react'
import { Button, Paper, Divider } from '@mantine/core'
import DynamicTableStock, { Stock } from "../DataTable/DynamicTableStock"
import DynamicTableOrder, { Order } from "../DataTable/DynamicTableOrder"
import ResultTable from "../ResultTable/ResultTable"
import classes from "./MainContent.module.css"
import useSolveCuttingStockProblem from "../../hooks/useSolveCuttingStockProblem"
import useResetStore from "../../hooks/useResetStore"
import { showNotification } from '@mantine/notifications'

const MainContent = () => {
    const stockDataRef = useRef<Stock[]>([]);
    const orderDataRef = useRef<Order[]>([]);
    const setResetResultFunction = useResetStore((state) => state.setResetResultFunction);

    const onSolvingError = (err: Error) => {
        showNotification({
            color: 'red',
            title: "Error!",
            message: err.message,
            className: classes.notification,
            autoClose: 5000
        });
    }

    const { data: resultData,
        mutate: solveCuttingStockProblem,
        reset: resetResultData
    } = useSolveCuttingStockProblem(onSolvingError);

    const onGenerateResultClick = () => {
        const requestBody = {
            problemType: "MultipleStock",
            stockList: [...stockDataRef.current],
            orderList: [...orderDataRef.current],
        };
        solveCuttingStockProblem(requestBody);
    }

    const onHandleReset = useCallback(() => {
        resetResultData();
    }, [resetResultData])

    useEffect(() => {
        setResetResultFunction(onHandleReset);
    }, [setResetResultFunction, onHandleReset])

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
                            data={resultData?.resultItems ?? []}
                        />
                    </div>
                </Paper>
            </div>
        </div>
    );
}

export default MainContent