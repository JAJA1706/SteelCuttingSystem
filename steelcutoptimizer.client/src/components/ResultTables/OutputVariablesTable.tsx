import { useMemo, useState } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import classes from "./OutputVariablesTable.module.css"


interface CostMinimizingVariables {
    cost: number;
    waste: number;
    patternCount: number;
}

interface TableProps {
    data: CostMinimizingVariables[];
}

const OutputVariablesTable = ({ data }: TableProps) => {
    const columns = useMemo<MRT_ColumnDef<CostMinimizingVariables>[]>(() => [
        {
            accessorKey: 'cost',
            header: 'Cost',
            size: 130,
        },
        {
            accessorKey: 'waste',
            header: 'Waste',
            size: 130,
        },
        {
            accessorKey: 'patternCount',
            header: 'Pattern Count',
            size: 130,
        },
    ], []);

    const table = useMantineReactTable({
        columns,
        data: data,
        createDisplayMode: 'row',
        editDisplayMode: 'table',
        enableColumnResizing: false,
        enableEditing: false,
        enableHiding: false,
        enableTopToolbar: false,
        enableBottomToolbar: false,
        positionActionsColumn: 'last',
        mantineTableProps: {
            withColumnBorders: true,
        },
        mantineTableBodyRowProps: {
            className: classes.rows
        },
    });

    return(
        <div className={classes.table}>
            <MantineReactTable table={table} />
        </div>
    );
};



export default OutputVariablesTable;

