import { useMemo, useState, useEffect, MutableRefObject, useCallback, useRef } from 'react';
import { MRT_Row, type MRT_ColumnDef } from 'mantine-react-table';
import { v4 as uuidv4 } from 'uuid';
import useResetStore from "../../hooks/useResetStore"
import useStockStore from "../../hooks/useStockStore"
import { useShallow } from 'zustand/react/shallow';
import DynamicTable, { BaseData } from './DynamicTable';
import { Settings } from '../SettingsPanel/SettingsPanel';
import { Radio, Title, Stack } from "@mantine/core";

export interface Stock extends BaseData {
    cost: number | undefined;
    nextStepGeneration: boolean;
}

interface DynamicTableStockProps {
    dataRef: MutableRefObject<Stock[]>,
    algorithmSettings: Settings,
}

const DynamicTableStock = ({ dataRef, algorithmSettings }: DynamicTableStockProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

    const radioChecked = useRef<string | undefined>();
    const [data, setData] = useState<Stock[]>([]);
    const setResetStockDataFunction = useResetStore(state => state.setResetStockDataFunction);
    const setStockDataFunctions = useStockStore(useShallow((state) => ({ get: state.setGetStockDataFunction, set: state.setSetStockDataFunction })));

    const setStockData = useCallback((newData: Stock[]) => {
        setData(newData);
    }, [setData])

    const getStockData = useCallback((): Stock[] => {
        return data;
    }, [data])

    const onHandleReset = useCallback(() => {
        setData([]);
    }, [setData])

    useEffect(() => {
        setStockDataFunctions.get(getStockData);
        setStockDataFunctions.set(setStockData);
        setResetStockDataFunction(onHandleReset);
    }, [setResetStockDataFunction, onHandleReset, setStockDataFunctions, getStockData, setStockData])

    useEffect(() => {
        if (dataRef !== undefined)
            dataRef.current = data;

        const row = data.find(row => row.nextStepGeneration === true);
        radioChecked.current = row?.id;
        
    }, [data, dataRef])

    const onCheckboxChange = (row: MRT_Row<Stock>) => {
        radioChecked.current = row.id;
        setData((prevData) =>
            prevData.map((item) =>
                item.id === row.id ? { ...item, nextStepGeneration: true } : { ...item, nextStepGeneration: false }
            )
        );
    };

    const columns = useMemo<MRT_ColumnDef<Stock>[]>(
        () => {
            const columnsDef: MRT_ColumnDef<Stock>[] = [];
            columnsDef.push({
                accessorKey: 'length',
                header: 'Length',
                size: 80,
                mantineEditTextInputProps: ({ cell, row }) => ({
                    type: 'number',
                    required: true,
                    error: validationErrors?.[cell.id],
                    onFocus: (event) => {
                        event.target.select();
                    },
                    onBlur: (event) => {
                        const value = event.currentTarget.value;
                        const validationError = !validatePositiveNumber(value)
                            ? 'Positive number required'
                            : undefined;
                        setValidationErrors({
                            ...validationErrors,
                            [cell.id]: validationError,
                        });

                        setData((prevData) =>
                            prevData.map((item) =>
                                item.id === row.id ? { ...item, length: parseInt(value) } : item
                            )
                        );
                    },
                }),
            });
            columnsDef.push({
                accessorKey: 'count',
                header: 'Count',
                size: 80,
                mantineEditTextInputProps: ({ cell, row }) => ({
                    type: 'number',
                    required: true,
                    error: validationErrors?.[cell.id],
                    onFocus: (event) => {
                        event.target.select();
                    },
                    onBlur: (event) => {
                        let value: string | number | undefined = event.currentTarget.value;
                        if (value !== '') {
                            const validationError = !validatePositiveNumber(value)
                                ? 'Positive number required'
                                : undefined;
                            setValidationErrors({
                                ...validationErrors,
                                [cell.id]: validationError,
                            });
                            value = parseInt(value);
                        }
                        else {
                            value = undefined;
                        }

                        setData((prevData) =>
                            prevData.map((item) =>
                                item.id === row.id ? { ...item, count: value } : item
                            )
                        );
                    },
                }),
            });

            if (algorithmSettings.mainObjective === "cost")
            {
                columnsDef.push({
                    accessorKey: 'cost',
                    header: 'Cost',
                    size: 80,
                    mantineEditTextInputProps: ({ cell, row }) => ({
                        type: 'number',
                        required: true,
                        error: validationErrors?.[cell.id],
                        onFocus: (event) => {
                            event.target.select();
                        },
                        onBlur: (event) => {
                            let value: string | number | undefined = event.currentTarget.value;
                            if (value !== '') {
                                const validationError = !validatePositiveNumber(value)
                                    ? 'Positive number required'
                                    : undefined;
                                setValidationErrors({
                                    ...validationErrors,
                                    [cell.id]: validationError,
                                });
                                value = parseInt(value);
                            }
                            else {
                                value = undefined;
                            }

                            setData((prevData) =>
                                prevData.map((item) =>
                                    item.id === row.id ? { ...item, cost: value } : item
                                )
                            );
                        },
                    }),
                });
            }

            if (algorithmSettings.relaxationType === "singleStep") {
                columnsDef.push({
                    accessorKey: 'generate',
                    header: 'Generate next step',
                    size: 80,
                    enableColumnFilter: false,
                    enableSorting: false,
                    Edit: ({ row }) => (
                        <Radio
                            checked={radioChecked.current === row.id}
                            onChange={() => onCheckboxChange(row)}
                        />
                    ),
                })
            }

            return columnsDef;
        },
        [validationErrors, algorithmSettings],
    );

    const getDefaultNewRow = () => {
        return {
            id: uuidv4(),
            length: 1,
            count: undefined,
            cost: undefined,
        } as Stock;
    } 

    return (
        <Stack align="center" gap="3px">
            <Title size="h4">Stock Table</Title>
            <DynamicTable<Stock>
                data={data}
                setData={setData}
                columns={columns}
                getDefaultNewRow={getDefaultNewRow}
            />
        </Stack>
    );
};

const validatePositiveNumber = (value: string) => parseInt(value) >= 0;


export default DynamicTableStock;