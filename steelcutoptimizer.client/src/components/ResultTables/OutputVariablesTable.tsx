import { useMemo } from 'react';
import {
    MantineReactTable,
    type MRT_ColumnDef,
    useMantineReactTable,
} from 'mantine-react-table';
import classes from "./OutputVariablesTable.module.css"
import { AmplResults } from "../../hooks/useSolveCuttingStockProblem"


interface OutputVariables {
    totalCost: string;
    totalWaste: string;
    totalRelax: string;
}

interface TableProps {
    data: AmplResults | undefined;
}

const OutputVariablesTable = ({ data }: TableProps) => {
    const displayedData = useMemo<OutputVariables[]>(() => {
        return [{
            totalCost: data?.totalCost?.toString() ?? "---",
            totalWaste: data?.totalWaste.toString() ?? "---",
            totalRelax: data?.totalRelax?.toString() ?? "---",
        }]
    }, [data]);

    const columns = useMemo<MRT_ColumnDef<OutputVariables>[]>(() => [
        {
            accessorKey: 'totalCost',
            header: 'Cost',
            size: 130,
        },
        {
            accessorKey: 'totalWaste',
            header: 'Waste',
            size: 130,
        },
        {
            accessorKey: 'totalRelax',
            header: 'Total Relax',
            size: 130,
        },
    ], []);

    const table = useMantineReactTable({
        columns,
        data: displayedData,
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

