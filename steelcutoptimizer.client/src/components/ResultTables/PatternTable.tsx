import { useMemo, useState } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
    MRT_ToggleGlobalFilterButton,
    MRT_ToggleFiltersButton,
    MRT_ToggleDensePaddingButton,
    MRT_ToggleFullScreenButton,
} from 'mantine-react-table';
import { Box, ActionIcon, Tooltip } from '@mantine/core';
import { IconDownload } from '@tabler/icons-react';
import { mkConfig, generateCsv, download } from 'export-to-csv';
import classes from "./PatternTable.module.css"
import { ResultPattern } from "../../hooks/useSolveCuttingStockProblem"

interface TableProps {
    data: ResultPattern[];
}

const csvConfig = mkConfig({
    fieldSeparator: ',',
    decimalSeparator: '.',
    useKeysAsHeaders: true,
    filename: "patterns"
});

const PatternTable = ({ data }: TableProps) => {
    const [isLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers] = useState<boolean>(false);
    const [isSaving] = useState<boolean>(false);

    const displayedData = useMemo<ResultPattern[]>(() => {
        return [...data.filter(pattern => pattern.useCount > 0)];
        //return data;
    }, [data]);

    const columns = useMemo<MRT_ColumnDef<ResultPattern>[]>(
        () => {
            const result: MRT_ColumnDef<ResultPattern>[] = [
                {
                    accessorKey: 'patternId',
                    header: 'Pattern ID',
                    size: 130,
                },
                {
                    accessorKey: 'stockLength',
                    header: 'Stock Length',
                    size: 165,
                },
                {
                    accessorKey: 'useCount',
                    header: 'Count',
                    size: 120,
                },
            ];

            if (!displayedData || displayedData.length === 0)
                return result;

            let idxOfDataWithMostBars = 0;
            let maxBarNum = 0;
            for (let i = 0; i < displayedData.length; ++i) {
                if (displayedData[i].segmentList.length > maxBarNum) {
                    maxBarNum = displayedData[i].segmentList.length;
                    idxOfDataWithMostBars = i;
                }
            }

            let tempIdx = 0;
            displayedData[idxOfDataWithMostBars].segmentList.forEach(() => {
                const idx = tempIdx;
                result.push({
                    id: idx.toString(),
                    accessorFn: (row: ResultPattern) => {
                        if (row.segmentList.length > idx) {
                            if (row.segmentList[idx].relaxAmount !== 0)
                                return `${row.segmentList[idx].length} (+${row.segmentList[idx].relaxAmount})`;
                            else
                                return `${row.segmentList[idx].length}`;
                        }
                        else
                            return '';
                    },
                    header: idx.toString(),
                    size: 80,
                    enableColumnActions: false,
                    enableSorting: false,
                });
                ++tempIdx;
            })

            return result;
        },
        [displayedData],
    );

    const makeDataExportable = (data: ResultPattern[]) => {
        return data.flatMap(pattern =>
            pattern.segmentList.map(segment => ({
                patternId: pattern.patternId,
                stockId: pattern.stockId,
                stockLength: pattern.stockLength,
                count: pattern.useCount,
                orderId: segment.orderId,
                length: segment.length,
                relaxAmount: segment.relaxAmount,
            }))
        );
    }

    const handleExportToCsv = () => {
        const exportableData = makeDataExportable(displayedData);
        const csv = generateCsv(csvConfig)(exportableData);
        download(csvConfig)(csv);
    }

    const table = useMantineReactTable({
        columns,
        data: displayedData,
        createDisplayMode: 'row',
        editDisplayMode: 'table',
        enableColumnResizing: true,
        enableEditing: false,
        enableHiding: false,
        positionActionsColumn: 'last',
        mantineTableProps: {
            withColumnBorders: true,
        },
        getRowId: (row) => row.patternId.toString(),

        mantineTableBodyRowProps: {
            className: classes.rows
        },
        renderToolbarInternalActions: ({ table }) => (
            <Box>
                <Tooltip label="Export to CSV">
                    <ActionIcon
                        disabled={displayedData.length === 0}
                        color="gray"
                        variant="subtle"
                        onClick={handleExportToCsv}
                    >
                
                        <IconDownload />
                    </ActionIcon>
                </Tooltip>
                <MRT_ToggleGlobalFilterButton table={table} />
                <MRT_ToggleFiltersButton table={table} />
                <MRT_ToggleDensePaddingButton table={table} />
                <MRT_ToggleFullScreenButton table={table} />
            </Box>
        ),

        state: {
            isLoading: isLoadingUsers,
            isSaving: isSaving,
            showAlertBanner: isLoadingUsersError,
            showProgressBars: isFetchingUsers,
        },

    });

    return (
        <div>
            <MantineReactTable table={table} />
        </div>
    );
};



export default PatternTable;

