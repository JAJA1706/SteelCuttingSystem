import { useCallback, useEffect, useRef, useState } from 'react'
import { Button, Paper, Divider, Switch, Tooltip, ThemeIcon, Slider, Text } from '@mantine/core'
import StockTable, { Stock } from "../DataTables/StockTable"
import OrderTable, { Order } from "../DataTables/OrderTable"
import PatternTable from "../ResultTables/PatternTable"
import OutputVariablesTable from "../ResultTables/OutputVariablesTable"
import classes from "./MainContent.module.css"
import useSolveCuttingStockProblem, { CuttingStockProblemBody, AmplResults } from "../../hooks/useSolveCuttingStockProblem"
import useResetStore from "../../hooks/useResetStore"
import { showNotification } from '@mantine/notifications'
import SettingsPanel, { Settings } from '../SettingsPanel/SettingsPanel'
import { IconQuestionMark } from '@tabler/icons-react'

const MainContent = () => {
    const stockDataRef = useRef<Stock[]>([]);
    const orderDataRef = useRef<Order[]>([]);
    const setResetResultFunction = useResetStore((state) => state.setResetResultFunction);
    const [algorithmSettings, setAlgorithmSettings] = useState<Settings>({ mainObjective: "cost", relaxationType: "manual", solver: "cbc" });
    const [switchValue, setSwitchValue] = useState(false);
    const sliderValue = useRef(1);
    const [fetchedData, setFetchedData] = useState<AmplResults | undefined>(undefined);

    const onSolvingError = (err: Error) => {
        showNotification({
            color: 'red',
            title: "Error!",
            message: err.message,
            className: classes.notification,
            autoClose: 5000
        });
    }

    const onSolvingSuccess = (data: AmplResults) => {
        setFetchedData(data);
        showNotification({
            color: 'green',
            title: "Problem solved successfully",
            message: "",
            className: classes.notification,
            autoClose: 3000
        });
    };

    const {
        mutate: solveCuttingStockProblem,
        reset: resetResultData
    } = useSolveCuttingStockProblem(onSolvingError, onSolvingSuccess);

    const onGenerateResultClick = () => {
        const requestBody: CuttingStockProblemBody = {
            algorithmSettings: {...algorithmSettings},
            stockList: [...stockDataRef.current],
            orderList: [...orderDataRef.current],
        };

        if (algorithmSettings.relaxationType !== "singleStep")
            requestBody.relaxCostMultiplier = sliderValue.current;

        if (algorithmSettings.relaxationType === "auto")
            requestBody.areBasicPatternsAllowed = switchValue;

        if (algorithmSettings.relaxationType === "singleStep")
            requestBody.algorithmSettings.relaxationType = "none";

        solveCuttingStockProblem(requestBody);
    }

    const onGenerateNextPatternClick = () => {
        const requestBody: CuttingStockProblemBody = {
            algorithmSettings: algorithmSettings,
            stockList: [...stockDataRef.current],
            orderList: [...orderDataRef.current],
            orderPrices: [...fetchedData?.orderPrices ?? []],
            stockLimits: [...fetchedData?.stockLimits ?? []],
            patterns: fetchedData?.patterns.map(resPattern => {
                return {
                    stockId: resPattern.stockId,
                    stockLength: resPattern.stockLength,
                    useCount: resPattern.useCount,
                    segmentList: resPattern.segmentList,
                }
            })
        };

        solveCuttingStockProblem(requestBody);
    }

    const onHandleReset = useCallback(() => {
        setFetchedData(undefined);
        resetResultData();
    }, [resetResultData])

    useEffect(() => {
        setResetResultFunction(onHandleReset);
    }, [setResetResultFunction, onHandleReset])

    const checkIfGenerateNextStepIsDisabled = () => {
        if (fetchedData === undefined)
            return true;

        const row = stockDataRef.current.find(row => row.nextStepGeneration === true)
        if (row === undefined)
            return true;

        return false;
    }

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
                                {algorithmSettings.relaxationType === "singleStep" && (
                                    <Button className={classes.button} onClick={onGenerateNextPatternClick} disabled={checkIfGenerateNextStepIsDisabled()}> 
                                        Generate Next Pattern
                                    </Button>
                                )}
                                {algorithmSettings.relaxationType === "auto" && (
                                    <div className={classes.switch}>
                                        <Switch
                                            checked={switchValue}
                                            onChange={(e) => setSwitchValue(e.currentTarget.checked)}
                                            label={switchValue ? "Basic Patterns Allowed" : "Basic Patterns Forbidden"}
                                        />
                                        <Tooltip label="Basic pattern is a pattern which consists only of rods of the same length">
                                            <ThemeIcon color="gray" size="xs">
                                                <IconQuestionMark />
                                            </ThemeIcon>
                                        </Tooltip>
                                    
                                    </div>
                                )}
                                {(algorithmSettings.relaxationType !== "singleStep") && (
                                    <div className={classes.slider}>
                                        <Text size="sm">Relax Cost Multiplier</Text>
                                        <Slider
                                            onChange={(value) => sliderValue.current = (1 / (1 - value))}
                                            defaultValue={0}
                                            min={0}
                                            max={0.99}
                                            label={(value) => `${(1 / (1 - value)).toFixed(2)}x`}
                                            step={0.01}
                                        />
                                    </div>
                                )}
                            </div>
                        </div>
                        <div className={classes.table}>
                            <OrderTable dataRef={orderDataRef} algorithmSettings={algorithmSettings} />
                        </div>
                    </div>
                </Paper>
                <Divider my="md" size="lg"/>
                <Paper shadow="md" withBorder>
                    <div className={classes.resultTables}>
                        <OutputVariablesTable data={fetchedData} />
                        <PatternTable
                            data={fetchedData?.patterns ?? []}
                        />
                    </div>
                </Paper>
            </div>
        </div>
    );
}

export default MainContent