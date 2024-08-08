import { AppShell, Button, Burger, Group } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconRefresh, IconUpload, IconDownload } from '@tabler/icons-react';
//import DataTable from "../DataTable/DataTable"
import DynamicTableStock from "../DataTable/DynamicTableStock"
import DynamicTableOrder from "../DataTable/DynamicTableOrder"
import classes from "./LayoutWithHeaderSidebar.module.css"

export default function LayoutWithHeaderSidebar() {
    const [opened, { toggle }] = useDisclosure();

    return (
        <AppShell
            header={{ height: 60 }}
            navbar={{ width: 300, breakpoint: 'sm', collapsed: { desktop: true, mobile: !opened } }}
            padding="md"
        >
            <AppShell.Header bg='blue.1'>
                    <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" h="100%" px="md"/>
                <Group h="100%" ml="xl" gap={10} visibleFrom="sm" justify="flex-end" mr={10}>
                    <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button}>Nowy Plan</Button>
                    <Button variant="subtle" leftSection={<IconUpload />} className={classes.button}>Importuj Plan</Button>
                    <Button variant="subtle" leftSection={<IconDownload />} className={classes.button}>Pobierz Plan</Button>
                </Group>
            </AppShell.Header>

            <AppShell.Navbar py="md" px={4}>
                <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button}>Nowy Plan</Button>
                <Button variant="subtle" leftSection={<IconUpload />} className={classes.button}>Importuj Plan</Button>
                <Button variant="subtle" leftSection={<IconDownload />} className={classes.button}>Pobierz Plan</Button>
            </AppShell.Navbar>

            <AppShell.Main>
                <div className={classes.mainBody}>
                    <DynamicTableStock />
                    <DynamicTableOrder />
                </div>
            </AppShell.Main>
        </AppShell>
    );
}