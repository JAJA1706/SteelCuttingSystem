import { useMemo, useState, useEffect, MutableRefObject, useCallback } from 'react';
import { type MRT_ColumnDef } from 'mantine-react-table';
import { v4 as uuidv4 } from 'uuid';
import useResetStore from "../../hooks/useResetStore"
import useOrderStore from "../../hooks/useOrderStore"
import { useShallow } from 'zustand/react/shallow';
import DynamicTable, { BaseData } from '../DynamicTable/DynamicTable';

export interface Order extends BaseData {
    maxRelax: number | undefined;
}

interface DynamicTableOrderProps {
    dataRef: MutableRefObject<Order[]>,
}

const DynamicTableOrder = ({ dataRef }: DynamicTableOrderProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

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
    }, [data, dataRef])

    const columns = useMemo<MRT_ColumnDef<Order>[]>(
        () => [
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
            },
            {
                accessorKey: 'maxRelax',
                header: 'maxRelax',
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
                                item.id === row.id ? { ...item, maxRelax: parseInt(value) } : item
                            )
                        );
                    },
                }),
            },
        ],
        [validationErrors],
    );

    const getDefaultNewRow = () => {
        return {
            id: uuidv4(),
            length: 1,
            count: 1,
            maxRelax: undefined,
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

