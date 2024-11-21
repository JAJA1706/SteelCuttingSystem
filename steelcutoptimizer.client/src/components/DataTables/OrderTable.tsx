import { useMemo, useState, useEffect, MutableRefObject, useCallback, useRef } from 'react';
import { MRT_Row, type MRT_ColumnDef } from 'mantine-react-table';
import { v4 as uuidv4 } from 'uuid';
import useResetStore from "../../hooks/useResetStore"
import useOrderStore from "../../hooks/useOrderStore"
import { useShallow } from 'zustand/react/shallow';
import DynamicTable, { BaseData } from './DynamicTable';
import { Settings } from '../SettingsPanel/SettingsPanel';
import { Checkbox } from "@mantine/core";

export interface Order extends BaseData {
    maxRelax: number | undefined;
    canBeRelaxed: boolean | undefined;
}

interface DynamicTableOrderProps {
    dataRef: MutableRefObject<Order[]>,
    algorithmSettings: Settings,
}

const DynamicTableOrder = ({ dataRef, algorithmSettings }: DynamicTableOrderProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

    const isCheckboxChecked = useRef(new Map<string, boolean>()); //we are storing this outside of data to not cause rerender of columns definition
    const [data, setData] = useState<Order[]>([]);
    const setResetOrderDataFunction = useResetStore(state => state.setResetOrderDataFunction);
    const setOrderDataFunctions = useOrderStore(useShallow((state) => ({ get: state.setGetOrderDataFunction, set: state.setSetOrderDataFunction })));

    const setOrderData = useCallback((newData: Order[]) => {
        setData(newData);
    }, [setData])

    const getOrderData = useCallback((): Order[] => {
        return data;
    }, [data])

    const onHandleReset = useCallback(() => {
        setData([]);
    }, [setData])

    useEffect(() => {
        setOrderDataFunctions.get(getOrderData);
        setOrderDataFunctions.set(setOrderData);
        setResetOrderDataFunction(onHandleReset);
    }, [setResetOrderDataFunction, onHandleReset, setOrderDataFunctions, getOrderData, setOrderData])

    useEffect(() => {
        if (dataRef !== undefined)
            dataRef.current = data;

        isCheckboxChecked.current.clear();
        data.forEach(row => {
            isCheckboxChecked.current.set(row.id, row.canBeRelaxed ?? false);
        })
    }, [data, dataRef])

    const onCheckboxChange = (row: MRT_Row<Order>, e: React.ChangeEvent<HTMLInputElement>) => {
        isCheckboxChecked.current.set(row.id, e.currentTarget.checked);
        setData((prevData) =>
            prevData.map((item) =>
                item.id === row.id ? { ...item, canBeRelaxed: e.currentTarget.checked } : item
            )
        );
    };

    const columns = useMemo<MRT_ColumnDef<Order>[]>(
        () => {
            const columnsDef: MRT_ColumnDef<Order>[] = [
                {
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
                },
                {
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
                }];

            if (algorithmSettings.relaxationType === "manual") {
                columnsDef.push({
                    accessorKey: 'maxRelax',
                    header: 'Max Relax',
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
                                    item.id === row.id ? { ...item, maxRelax: value } : item
                                )
                            );
                        },
                    }),
                });
            }
            else {
                columnsDef.push({
                    accessorKey: 'canBeRelaxed',
                    header: 'Can be relaxed',
                    size: 80,
                    enableColumnFilter: false,
                    enableSorting: false,
                    Edit: ({row}) => (
                        <Checkbox
                            checked={isCheckboxChecked.current.get(row.id) ?? false}
                            onChange={(e) => onCheckboxChange(row, e)}
                        />
                    ),
                })
            }

            return columnsDef;
        }, [validationErrors, algorithmSettings]);

    const getDefaultNewRow = () => {
        return {
            id: uuidv4(),
            length: 1,
            count: 1,
            maxRelax: undefined,
            canBeRelaxed: false,
        } as Order;
    } 

    return (
        <div>
            <DynamicTable<Order>
                data={data}
                setData={setData}
                columns={columns}
                getDefaultNewRow={getDefaultNewRow}
            />
        </div>
    );
};

const validatePositiveNumber = (value: string) => parseInt(value) >= 0;


export default DynamicTableOrder;

