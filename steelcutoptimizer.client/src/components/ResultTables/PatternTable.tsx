import { useMemo, useState } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import classes from "./PatternTable.module.css"
import { ResultPattern } from "../../hooks/useSolveCuttingStockProblem"


interface TableProps {
    data: ResultPattern[];
}

const PatternTable = ({ data }: TableProps) => {
    const [isLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers] = useState<boolean>(false);
    const [isSaving] = useState<boolean>(false);

    const displayedData = useMemo<ResultPattern[]>(() => {
        return [...data.filter(pattern => pattern.count > 0)];
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
                    accessorKey: 'count',
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

