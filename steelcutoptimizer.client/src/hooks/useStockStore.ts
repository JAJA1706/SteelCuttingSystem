import { create } from 'zustand';
import { Stock } from '../components/DataTables/StockTable'

interface StockStore {
    getStockData: () => Stock[],
    setGetStockDataFunction: (newGetStockDataFunction: () => Stock[]) => void,

    setStockData: (data: Stock[]) => void,
    setSetStockDataFunction: (newSetStockDataFunction: (data: Stock[]) => void) => void,
}

const useStockStore = create<StockStore>()((set) => ({
    getStockData: () => [],
    setGetStockDataFunction: (newGetStockDataFunction: () => Stock[]) => set(() => ({ getStockData: newGetStockDataFunction })),

    setStockData: () => {},
    setSetStockDataFunction: (newSetStockDataFunction: (data: Stock[]) => void) => set(() => ({ setStockData: newSetStockDataFunction })),
}));

export default useStockStore;