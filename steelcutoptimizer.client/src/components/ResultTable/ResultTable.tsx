import { useMemo, useState } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import classes from "./ResultTable.module.css"

interface UsedBars {
    length: number,

}
interface Pattern {
    id: string;
    patternNum: number;
    count: number;
    usedBars: number[], 
}

interface TableProps {
    data: Pattern[];
}

const ResultTable = ({ data }: TableProps) => {
    const [isLoadingUsers, setIsLoadingUsers] = useState<boolean>(false);
    const [isLoadingUsersError, setIsLoadingUsersError] = useState<boolean>(false);
    const [isFetchingUsers, setIsFetchingUsers] = useState<boolean>(false);
    const [isSaving, setIsSaving] = useState<boolean>(false);

    const columns = useMemo<MRT_ColumnDef<Pattern>[]>(
        () => {
            const result = [
                {
                    accessorKey: 'patternNum',
                    header: 'PatternNum',
                    size: 80,
                },
                {
                    accessorKey: 'count',
                    header: 'Count',
                    size: 80,
                },
                //{
                //    id: 'what',
                //    accessorFn: (row: Pattern) => {
                //        if (row.usedBars?.length > 0)
                //            return `${row.usedBars[0]}`
                //        else
                //            return '';
                //    },
                //    header: '',
                //    size: 80,
                //},
            ];

            if (data.length === 0)
                return result;

            let idxOfDataWithMostBars = 0;
            let maxBarNum = 0;
            for (let i = 0; i < data.length; ++i) {
                if (data[i].usedBars.length > maxBarNum) {
                    maxBarNum = data[i].usedBars.length;
                    idxOfDataWithMostBars = i;
                }
            }

            let tempIdx = 0;
            data[idxOfDataWithMostBars].usedBars.forEach(o => {
                const idx = tempIdx;
                result.push({
                    id: idx.toString(),
                    accessorFn: (row: Pattern) => {
                        if (row.usedBars.length > idx)
                            return `${row.usedBars[idx]}`
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
        enableEditing: false,
        enableHiding: false,
        positionActionsColumn: 'last',
        mantineTableProps: {
            withColumnBorders: true,
        },
        getRowId: (row) => row.id,

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

    return(
        <div>
            <MantineReactTable table={table} />
        </div>
    );
};



export default ResultTable;

