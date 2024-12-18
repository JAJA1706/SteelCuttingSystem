import { AppShell, Image, Button, Burger, Group, Modal, Text } from '@mantine/core';
import { useDisclosure } from '@mantine/hooks';
import { IconRefresh, IconUpload, IconDownload } from '@tabler/icons-react';
import FileImportPopup from "../FileImportPopup/FileImportPopup";
import MainContent from "../MainContent/MainContent";
import classes from "./LayoutWithHeaderSidebar.module.css";
import useResetStore from "../../hooks/useResetStore";
import useStockStore from "../../hooks/useStockStore";
import useOrderStore from "../../hooks/useOrderStore";
import { useShallow } from 'zustand/react/shallow';
import downloadFile from "../../utils/downloadFile"
import logo from "../../assets/logo.webp";

export default function LayoutWithHeaderSidebar() {
    const [opened, { toggle }] = useDisclosure();
    const [openedFileInput, { open: openFileInputModal, close: closeFileInputModal }] = useDisclosure(false);

    const resetFunctions = useResetStore(useShallow((state) => ({
        resetResult: state.resetResultFunction,
        resetStock: state.resetStockDataFunction,
        resetOrder: state.resetOrderDataFunction,
    })));

    const getStockData = useStockStore(state =>  state.getStockData);
    const getOrderData = useOrderStore(state => state.getOrderData);

    const onDownloadPlanClick = () => {
        const data = { stockData: getStockData(), orderData: getOrderData() };
        downloadFile(data);
    }

    const onNewPlanClick = () => {
        resetFunctions.resetResult();
        resetFunctions.resetStock();
        resetFunctions.resetOrder();
    };

    return (
        <AppShell
            header={{ height: 60 }}
            navbar={{ width: 300, breakpoint: "sm", collapsed: { desktop: true, mobile: !opened } }}
            padding="md"
        >
            <AppShell.Header bg='blue.1'>
                <Group h="100%" ml="xs" justify="space-between">
                    <Burger opened={opened} onClick={toggle} hiddenFrom="sm" size="sm" h="100%" px="md" />
                    <Group h="100%" ml={10} gap={10} visibleFrom="sm" justify="flex-start">
                        <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button} onClick={onNewPlanClick}>New Plan</Button>
                        <Button variant="subtle" leftSection={<IconDownload />} className={classes.button} onClick={openFileInputModal}>Import Plan</Button>
                        <Button variant="subtle" leftSection={<IconUpload />} className={classes.button} onClick={onDownloadPlanClick}>Download Plan</Button>
                    </Group>
                    <Group h="100%" ml="xs" gap={5} justify="flex-end" mr={10}>
                        <Image src={logo} h={60} />
                        <Text className={classes.text}>
                            Steel Cutting System
                        </Text>
                    </Group>
                </Group>
            </AppShell.Header>

            <AppShell.Navbar py="md" px={4}>
                <Button variant="subtle" leftSection={<IconRefresh />} className={classes.button} onClick={onNewPlanClick}>New Plan</Button>
                <Button variant="subtle" leftSection={<IconDownload />} className={classes.button} onClick={openFileInputModal}>Import Plan</Button>
                <Button variant="subtle" leftSection={<IconUpload />} className={classes.button} onClick={onDownloadPlanClick}>Download Plan</Button>
            </AppShell.Navbar>

            <AppShell.Main>
                <Modal opened={openedFileInput} onClose={closeFileInputModal} title="Importing Plan">
                    <FileImportPopup closePopup={closeFileInputModal} />
                </Modal>

                <MainContent />
            </AppShell.Main>
        </AppShell>
    );
}