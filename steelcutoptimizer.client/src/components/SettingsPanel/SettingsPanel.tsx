import { Dispatch, SetStateAction } from 'react'
import { Radio, Group, Text, Divider } from '@mantine/core'
import classes from "./SettingsPanel.module.css"

export interface Settings {
    mainObjective: string,
    relaxationType: string,
}

interface SettingsPanelProps {
    settings: Settings,
    setSettings: Dispatch<SetStateAction<Settings>>;
}

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

    return (
        <div className={classes.panelLayout}>
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
                    </Group>
                </Radio.Group>
            </div>
            <Divider />
            <div className={classes.textArea}>
                <Text fw={700}>Algorithm Description</Text>
                <Text> Ten algorytm robi takie rzeczy że ludziom się nie śniły. Największe mocarstwa nękaja mnie żebym podesłał im szczegóły tego zajebiście dowalonego w kosmos algorytmu mocy.
                    Użycie go co najmniej pozwala zawładnąć światem, a co najwyżej całkowicie go zniszczyć. Zastanów się dobrze przed kliknięciem "generuj".</Text>
            </div>
        </div>
    )
}

export default SettingsPanel