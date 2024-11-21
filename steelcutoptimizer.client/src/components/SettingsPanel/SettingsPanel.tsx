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
                    </Group>
                </Radio.Group>
            </div>
            <Divider />
            <div className={classes.textArea}>
                <Text fw={700}>Algorithm Description</Text>
                <Text> Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?
                </Text>
            </div>
        </div>
    )
}

export default SettingsPanel