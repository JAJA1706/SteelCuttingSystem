import { useCallback, useEffect, useRef, useState } from 'react'
import { Radio, Group, Button, Paper, Divider } from '@mantine/core'
import StockTable, { Stock } from "../DataTable/StockTable"
import OrderTable, { Order } from "../DataTable/OrderTable"
import ResultTable from "../ResultTable/ResultTable"
import classes from "./SettingsPanel.module.css"
import useSolveCuttingStockProblem from "../../hooks/useSolveCuttingStockProblem"
import useResetStore from "../../hooks/useResetStore"
import { showNotification } from '@mantine/notifications'

const SettingsPanel = () => {
    const [mainObjective, setMainObjective] = useState("cost");
    const onMainObjectiveChange = (e: string) => {
        setMainObjective(e);
    };

    return (
        <div className={classes.panelLayout}>
            <p> hasudfhasdiufhiuadhf </p>
            <Radio.Group
                value={mainObjective}
                onChange={onMainObjectiveChange}
                label="Minimize:"
            >
                <Group>
                    <Radio value="cost" label="Cost" />
                    <Radio value="waste" label="Waste" />
                </Group>
            </Radio.Group>
        </div>
    )
}

export default SettingsPanel