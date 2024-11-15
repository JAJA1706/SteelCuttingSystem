import { useMemo, useState } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import classes from "./PatternTable.module.css"


interface RelaxableLength {
    length: number,
    relaxAmount: number,
}
interface Pattern {
    patternId: number;
    count: number;
    usedOrderLengths: RelaxableLength[], 
}

interface TableProps {
    data: Pattern[];
}

const PatternTable = ({ data }: TableProps) => {
    const [isLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers] = useState<boolean>(false);
    const [isSaving] = useState<boolean>(false);

    const columns = useMemo<MRT_ColumnDef<Pattern>[]>(
        () => {
            const result: MRT_ColumnDef<Pattern>[] = [
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

            if (!data || data.length === 0)
                return result;

            let idxOfDataWithMostBars = 0;
            let maxBarNum = 0;
            for (let i = 0; i < data.length; ++i) {
                if (data[i].usedOrderLengths.length > maxBarNum) {
                    maxBarNum = data[i].usedOrderLengths.length;
                    idxOfDataWithMostBars = i;
                }
            }

            let tempIdx = 0;
            data[idxOfDataWithMostBars].usedOrderLengths.forEach(() => {
                const idx = tempIdx;
                result.push({
                    id: idx.toString(),
                    accessorFn: (row: Pattern) => {
                        if (row.usedOrderLengths.length > idx) {
                            if (row.usedOrderLengths[idx].relaxAmount !== 0)
                                return `${row.usedOrderLengths[idx].length} (+${row.usedOrderLengths[idx].relaxAmount})`;
                            else
                                return `${row.usedOrderLengths[idx].length}`;
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
        [data],
    );

    const table = useMantineReactTable({
        columns,
        data: data,
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

