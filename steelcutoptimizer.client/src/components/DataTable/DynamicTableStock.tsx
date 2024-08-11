import { useMemo, useState, useEffect, MutableRefObject, useCallback } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import { ActionIcon, Button, Text, Tooltip } from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconTrash } from '@tabler/icons-react';
import { v4 as uuidv4 } from 'uuid';
import classes from "./DynamicTableStock.module.css"
import useResetStore from "../../hooks/useResetStore"

export interface Stock {
    id: string;
    length: number | undefined;
    count: number | undefined;
    cost: number | undefined;
}

interface DynamicTableStockProps {
    dataRef: MutableRefObject<Stock[]>,
}

const DynamicTableStock = ({ dataRef }: DynamicTableStockProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

    const [data, setData] = useState<Stock[]>([]);
    const [isLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers] = useState<boolean>(false);
    const [isSaving] = useState<boolean>(false);

    const setResetStockDataFunction = useResetStore(state => state.setResetStockDataFunction);

    const onHandleReset = useCallback(() => {
        setData([]);
    }, [setData])

    useEffect(() => {
        setResetStockDataFunction(onHandleReset);
    }, [setResetStockDataFunction, onHandleReset])

    useEffect(() => {
        if (dataRef !== undefined)
            dataRef.current = data;
    }, [data, dataRef])

    const openDeleteConfirmModal = () =>
        modals.openConfirmModal({
            centered: true,
            children: (
                <Text>
                    Are you sure you want to delete selected rows?
                    This action cannot be undone.
                </Text>
            ),
            labels: { confirm: 'Delete', cancel: 'Cancel' },
            confirmProps: { color: 'red' },
            onConfirm: () => deleteSelectedRows(),
        });

    const deleteSelectedRows = (): void => {
        const selectedRows: Record<string, boolean> = table.getState().rowSelection;
        const rowsToDelete: string[] = [];
        Object.entries(selectedRows).forEach(([key, value]) => {
            if (value) {
                rowsToDelete.push(key);
            }
        });
        const newData = data.filter(x => !rowsToDelete.includes(x.id));
        setData(newData);
        table.setRowSelection({});
    };

    const columns = useMemo<MRT_ColumnDef<Stock>[]>(
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
            },
            {
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
            },
        ],
        [validationErrors],
    );

    const isRemoveButtonDisabled = (): boolean => {
        const selectedRows = table.getState().rowSelection;
        if (Object.keys(selectedRows).length === 0)
            return true
        return false;
    }

    const table = useMantineReactTable({
        columns,
        data: data,
        createDisplayMode: 'row',
        editDisplayMode: 'table',
        enableEditing: true,
        positionActionsColumn: 'last',
        getRowId: (row) => row.id,
        enableRowSelection: true,
        enableTopToolbar: false,
        mantineTableBodyRowProps: {
            className: classes.rows
        },
        renderBottomToolbarCustomActions: () => (
            <>
                <Button
                    onClick={() => {
                        setData((oldData) => {
                            return [...oldData, {
                                id: uuidv4(),
                                length: 1,
                                count: undefined,
                                cost: undefined,
                            }]
                        });
                    }}
                >
                    Add new
                </Button>
                <Tooltip label="Delete">
                    <ActionIcon ml='xs' color="red" onClick={() => openDeleteConfirmModal()} disabled={isRemoveButtonDisabled()} >
                        <IconTrash />
                    </ActionIcon>
                </Tooltip>
            </>
        ),
        
        state: {
            isLoading: isLoadingUsers,
            isSaving: isSaving,
            showAlertBanner: isLoadingUsersError,
            showProgressBars: isFetchingUsers,
        },
    });

    return(
        <div>
            <MantineReactTable table={table} />
        </div>
    );
};

const validatePositiveNumber = (value: string) => parseInt(value) >= 0;


export default DynamicTableStock;

