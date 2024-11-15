import { Dispatch, SetStateAction} from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import { ActionIcon, Button, Text, Tooltip } from '@mantine/core';
import { modals } from '@mantine/modals';
import { IconTrash } from '@tabler/icons-react';
import classes from "./DynamicTable.module.css"

export interface BaseData {
    id: string;
    length: number | undefined;
    count: number | undefined;
}

interface DynamicTableProps<T extends BaseData> {
    data: T[];
    setData: Dispatch<SetStateAction<T[]>>;
    columns: MRT_ColumnDef<T>[];
    getDefaultNewRow: () => T;
}

const DynamicTable = <T extends BaseData>({
    data,
    setData,
    columns,
    getDefaultNewRow,
}: DynamicTableProps<T>) => {
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

    const isRemoveButtonDisabled = (): boolean => {
        const selectedRows = table.getState().rowSelection;
        return Object.keys(selectedRows).length === 0;
    };

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
        localization: {
            rowsPerPage: ""
        },
        renderBottomToolbarCustomActions: () => (
            <>
                <Button
                    onClick={() => {
                        setData((oldData) => [
                            ...oldData,
                            {...getDefaultNewRow()}
                        ]);
                    }}
                >
                    Add new
                </Button>
                <Tooltip label="Delete">
                    <ActionIcon ml="xs" color="red" onClick={() => openDeleteConfirmModal()} disabled={isRemoveButtonDisabled()}>
                        <IconTrash />
                    </ActionIcon>
                </Tooltip>
            </>
        ),
        state: {
            isLoading: false,
            isSaving: false,
            showAlertBanner: false,
            showProgressBars: false,
        },
    });

    return (
        <div>
            <MantineReactTable table={table} />
        </div>
    );
};

export default DynamicTable;