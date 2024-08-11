import { AppShell, Button, Burger, Group } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconRefresh, IconUpload, IconDownload } from '@tabler/icons-react';
import MainContent from "../MainContent/MainContent"
import classes from "./LayoutWithHeaderSidebar.module.css"
import useResetStore from "../../hooks/useResetStore"
import { useShallow } from 'zustand/react/shallow';

export default function LayoutWithHeaderSidebar() {
    const [opened, { toggle }] = useDisclosure();

    const resetFunctions = useResetStore(useShallow((state) => ({
        resetResult: state.resetResultFunction,
        resetStock: state.resetStockDataFunction,
        resetOrder: state.resetOrderDataFunction,
    })));

    const onNewPlanClick = () => {
        resetFunctions.resetResult();
        resetFunctions.resetStock();
        resetFunctions.resetOrder();
    };

    return (
        <AppShell
            header={{ height: 60 }}
            navbar={{ width: 300, breakpoint: 'sm', collapsed: { desktop: true, mobile: !opened } }}
            padding="md"
        >
            <AppShell.Header bg='blue.1'>
                    <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" h="100%" px="md"/>
                <Group h="100%" ml="xl" gap={10} visibleFrom="sm" justify="flex-end" mr={10}>
                    <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button} onClick={onNewPlanClick}>New Plan</Button>
                    <Button variant="subtle" leftSection={<IconDownload />} className={classes.button}>Import Plan</Button>
                    <Button variant="subtle" leftSection={<IconUpload />} className={classes.button}>Download Plan</Button>
                </Group>
            </AppShell.Header>

            <AppShell.Navbar py="md" px={4}>
                <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button} onClick={onNewPlanClick}>New Plan</Button>
                <Button variant="subtle" leftSection={<IconDownload />} className={classes.button}>Import Plan</Button>
                <Button variant="subtle" leftSection={<IconUpload />} className={classes.button}>Download Plan</Button>
            </AppShell.Navbar>

            <AppShell.Main>
                <MainContent />
            </AppShell.Main>
        </AppShell>
    );
}