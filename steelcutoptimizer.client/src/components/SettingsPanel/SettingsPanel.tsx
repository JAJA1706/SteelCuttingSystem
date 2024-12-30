import { Dispatch, SetStateAction } from 'react'
import { Radio, Group, Text, Divider } from '@mantine/core'
import classes from "./SettingsPanel.module.css"

export interface Settings {
    mainObjective: string,
    relaxationType: string,
    solver: string,
}

interface SettingsPanelProps {
    settings: Settings,
    setSettings: Dispatch<SetStateAction<Settings>>;
}

const algDescriptions = [
    {
        name: "manual",
        desc: `This model assumes that the maximum relaxation value will be provided in the input data. 
            The solution found will include relaxed lengths only if they improve the final result.
            Relaxation values will be applied sparingly to avoid generating additional waste.`,
    },
    {
        name: "auto",
        desc: `This model attempts to determine the relaxation value automatically. 
            It is up to the user to decide which items can be subject to relaxation. 
            Under the 'Generate Result' button, there is a switch that allows for enabling or disabling the generation of basic patterns.
            By basic patterns, we mean patterns consisting of a single type of final item.`,
    },
    {
        name: "singleStep",
        desc: `This model determines increasingly larger relaxation values, starting from the optimal solution without relaxation.
            It is up to the user to decide which items can be subject to relaxation.
            The user also selects a base item for which the next pattern will be determined.
            The generation of the next pattern is performed by pressing the 'Generate Next Pattern' button.`
    },
    {
        name: "manualFast",
        desc: `This model is considered fast because it requires only one iteration of knapsack subproblem. In other aspects it is similar to manual relaxation.`,
    },
];

const SettingsPanel = ({ settings, setSettings }: SettingsPanelProps) => {
    const onMainObjectiveChange = (e: string) => {
        setSettings(prev => {
            return {
                ...prev,
                mainObjective: e,
            };
        })
    };
    const onRelaxationTypeChange = (e: string) => {
        setSettings(prev => {
            return {
                ...prev,
                relaxationType: e,
            };
        })
    };
    const onSolverTypeChange = (e: string) => {
        setSettings(prev => {
            return {
                ...prev,
                solver: e,
            };
        })
    };


    const description = algDescriptions.find(x => x.name === settings.relaxationType)?.desc ?? "";

    return (
        <div className={classes.panelLayout}>
            <Text fw={700}>Algorithm Settings</Text>
            <div className={classes.buttonLayout}>
                <Radio.Group
                    value={settings.mainObjective}
                    onChange={onMainObjectiveChange}
                    label="Minimize:"
                    className={classes.radioGroup}
                >
                    <Group>
                        <div className={classes.radio}>
                            <Radio value="cost" label="Cost" classNames={{ label: classes.radioLabel }} />
                        </div>
                        <div className={classes.radio}>
                            <Radio value="waste" label="Waste" classNames={{ label: classes.radioLabel }} />
                        </div>
                    </Group>
                </Radio.Group>

                <Radio.Group
                    value={settings.relaxationType}
                    onChange={onRelaxationTypeChange}
                    label="Relaxation type:"
                    className={classes.radioGroup}
                >
                    <Group>
                        <div className={classes.radio}>
                            <Radio value="manual" label="Manual" classNames={{ label: classes.radioLabel }} />
                        </div>
                        <div className={classes.radio}>
                            <Radio value="auto" label="Auto" classNames={{ label: classes.radioLabel }} />
                        </div>
                        <div className={classes.radio}>
                            <Radio value="singleStep" label="Single Step" classNames={{ label: classes.radioLabel }} />
                        </div>
                        {settings.mainObjective === "waste" &&
                            <div className={classes.radio}>
                                <Radio value="manualFast" label="Manual Fast" classNames={{ label: classes.radioLabel }} />
                            </div>}
                    </Group>
                </Radio.Group>
                <Radio.Group
                    value={settings.solver}
                    onChange={onSolverTypeChange}
                    label="Solver:"
                    className={classes.radioGroup}
                >
                    <Group>
                        <div className={classes.radio}>
                            <Radio value="cbc" label="Cbc" classNames={{ label: classes.radioLabel }} />
                        </div>
                        <div className={classes.radio}>
                            <Radio value="highs" label="HiGHS" classNames={{ label: classes.radioLabel }} />
                        </div>
                        <div className={classes.radio}>
                            <Radio value="cplex" label="CPLEX" classNames={{ label: classes.radioLabel }} />
                        </div>
                    </Group>
                </Radio.Group>
            </div>
            <Divider />
            <div className={classes.textArea}>
                <Text fw={700}>Algorithm Description</Text>
                <Text> 
                    {description}
                </Text>
            </div>
        </div>
    )
}

export default SettingsPanel