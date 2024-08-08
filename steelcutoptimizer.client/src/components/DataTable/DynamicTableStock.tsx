import { useMemo, useState, useEffect, MutableRefObject } from 'react';
import {
    MantineReactTable,
    createRow,
    type MRT_ColumnDef,
    type MRT_Row,
    type MRT_TableOptions,
    useMantineReactTable,
} from 'mantine-react-table';
import { ActionIcon, Button, Text, Tooltip } from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconTrash } from '@tabler/icons-react';
import { v4 as uuidv4 } from 'uuid';
import classes from "./DynamicTableStock.module.css"

export interface Stock {
    id: string;
    length: number;
    count: number;
    cost: number;
}

interface DynamicTableStockProps {
    dataRef: MutableRefObject<Stock[]>,
}

const DynamicTableStock = ({ dataRef }: DynamicTableStockProps) => {
    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

    const [data, setData] = useState<Stock[]>([]);
    const [isLoadingUsers, setIsLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError, setIsLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers, setIsFetchingUsers] = useState<boolean>(false);
    const [isSaving, setIsSaving] = useState<boolean>(false);

    useEffect(() => {
        if (dataRef !== undefined)
            dataRef.current = data;
    }, [data, dataRef])

    //DELETE action
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
                                length: 0,
                                count: 0,
                                cost: 0,
                            }]
                        });
                    }}
                >
                    Add new
                </Button>
                <Tooltip label="Delete">
                    <ActionIcon ml='xs' color="red" onClick={() => openDeleteConfirmModal()}>
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

const validateRequired = (value: string) => !!value?.length;

const validatePositiveNumber = (value: string) => parseInt(value) >= 0;


export default DynamicTableStock;

