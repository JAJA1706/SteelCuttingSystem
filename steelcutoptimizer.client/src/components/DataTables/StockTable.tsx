import { useMemo, useState, useEffect, MutableRefObject, useCallback } from 'react';
import { type MRT_ColumnDef } from 'mantine-react-table';
import { v4 as uuidv4 } from 'uuid';
import useResetStore from "../../hooks/useResetStore"
import useStockStore from "../../hooks/useStockStore"
import { useShallow } from 'zustand/react/shallow';
import DynamicTable, { BaseData } from './DynamicTable';
import { Settings } from '../SettingsPanel/SettingsPanel';

export interface Stock extends BaseData {
    cost: number | undefined;
}

interface DynamicTableStockProps {
    dataRef: MutableRefObject<Stock[]>,
    algorithmSettings: Settings,
}

const DynamicTableStock = ({ dataRef, algorithmSettings }: DynamicTableStockProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

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
    }, [data, dataRef])

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
                        const value = event.currentTarget.value;
                        if (value === '')
                            return;

                        const validationError = !validatePositiveNumber(value)
                            ? 'Positive number required'
                            : undefined;
                        setValidationErrors({
                            ...validationErrors,
                            [cell.id]: validationError,
                        });
                        setData((prevData) =>
                            prevData.map((item) =>
                                item.id === row.id ? { ...item, count: parseInt(value) } : item
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
                            const value = event.currentTarget.value;
                            if (value === '')
                                return;

                            const validationError = !validatePositiveNumber(value)
                                ? 'Positive number required'
                                : undefined;
                            setValidationErrors({
                                ...validationErrors,
                                [cell.id]: validationError,
                            });
                            setData((prevData) =>
                                prevData.map((item) =>
                                    item.id === row.id ? { ...item, cost: parseInt(value) } : item
                                )
                            );
                        },
                    }),
                });
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

    return(
        <div>
            <DynamicTable<Stock>
                data={data}
                setData={setData}
                columns={columns}
                getDefaultNewRow={getDefaultNewRow}
            />
        </div>
    );
};

const validatePositiveNumber = (value: string) => parseInt(value) >= 0;


export default DynamicTableStock;