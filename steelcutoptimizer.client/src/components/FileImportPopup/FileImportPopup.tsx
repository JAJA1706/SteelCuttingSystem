import { useState } from 'react';
import { FileInput, Button } from '@mantine/core'
import classes from "./FileImportPopup.module.css"
import useStockStore from "../../hooks/useStockStore";
import useOrderStore from "../../hooks/useOrderStore";

interface FileImportPopupProps {
    closePopup: () => void
}

const FileImportPopup = ({ closePopup }: FileImportPopupProps) => {
    const [value, setValue] = useState<File | null>(null);
    const [errorText, setErrorText] = useState<string>("");
    const setStockData = useStockStore(state => state.setStockData);
    const setOrderData = useOrderStore(state => state.setOrderData);

    const onFileChange = (file: File | null) => {
        if (!file)
            return;

        if (file.size > 102400) //100kB
        {
            setErrorText("File Too Big");
            return;
        }

        setErrorText("");
        setValue(file);
    }
    
    const importPlan = async () => {
        const fileContent = await value?.text();
        if (fileContent === undefined)
            return;

        const importedData = JSON.parse(fileContent);
        if (importedData.stockData !== undefined)
            setStockData(importedData.stockData);


        if (importedData.orderData !== undefined)
            setOrderData(importedData.orderData);

        closePopup();
    }

    return (
        <div>
            <div>
                <FileInput
                    className={classes.fileInput}
                    value={value} onChange={onFileChange}
                    error={errorText}
                    placeholder={"Choose file"} />
            </div>
            <div className={classes.buttonLayout}>
                <Button className={classes.button} onClick={importPlan}>
                    Import Plan
                </Button>
            </div>
        </div>
    );
};



export default FileImportPopup;

