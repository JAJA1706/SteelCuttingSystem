import { useCallback, useEffect, useRef, useState } from 'react'
import { Button, Paper, Divider } from '@mantine/core'
import StockTable, { Stock } from "../DataTables/StockTable"
import OrderTable, { Order } from "../DataTables/OrderTable"
import PatternTable from "../ResultTables/PatternTable"
import OutputVariablesTable from "../ResultTables/OutputVariablesTable"
import classes from "./MainContent.module.css"
import useSolveCuttingStockProblem from "../../hooks/useSolveCuttingStockProblem"
import useResetStore from "../../hooks/useResetStore"
import { showNotification } from '@mantine/notifications'
import SettingsPanel, {Settings} from '../SettingsPanel/SettingsPanel'

const MainContent = () => {
    const stockDataRef = useRef<Stock[]>([]);
    const orderDataRef = useRef<Order[]>([]);
    const setResetResultFunction = useResetStore((state) => state.setResetResultFunction);
    const [algorithmSettings, setAlgorithmSettings] = useState<Settings>({ mainObjective: "cost", relaxationType: "manual" });

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
            <Paper className={classes.algorithmEditor} shadow="md" withBorder>
                <SettingsPanel settings={algorithmSettings} setSettings={setAlgorithmSettings} />
            </Paper>
            <Divider hiddenFrom='sm' w={"100%"} size="lg"/>
            <Divider visibleFrom="sm" size="lg" orientation="vertical" />
            <div className={classes.mainBody}>
                <Paper className={classes.upperBody} shadow="md" withBorder>
                    <div className={classes.dynTables}>
                        <div className={classes.tableWithButtons}>
                            <div className={classes.table}>
                                <StockTable dataRef={stockDataRef} algorithmSettings={algorithmSettings} />
                            </div>
                            <div className={classes.buttons}>
                                <Button className={classes.button} onClick={onGenerateResultClick}>
                                    Generate Result
                                </Button>
                            </div>
                        </div>
                        <div className={classes.table}>
                            <OrderTable dataRef={orderDataRef} algorithmSettings={algorithmSettings} />
                        </div>
                    </div>
                </Paper>
                <Divider my="md" />
                <Paper shadow="md" withBorder>
                    <div className={classes.resultTables}>
                        <OutputVariablesTable data={[{ cost: 5, waste: 5, patternCount: 2 }]} />
                        <PatternTable
                            data={resultData?.resultItems ?? []}
                        />
                    </div>
                </Paper>
            </div>
        </div>
    );
}

export default MainContent