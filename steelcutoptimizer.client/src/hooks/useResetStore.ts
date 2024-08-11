import { create } from 'zustand';

interface ToolbarStore {
    resetResultFunction: () => void,
    resetStockDataFunction: () => void,
    resetOrderDataFunction: () => void,
    setResetResultFunction: (newResetFunction: () => void) => void
    setResetStockDataFunction: (newResetStockDataFunction: () => void) => void
    setResetOrderDataFunction: (newResetStockDataFunction: () => void) => void
}

const useResetStore = create<ToolbarStore>()((set) => ({
    resetResultFunction: () => { },
    resetStockDataFunction: () => { },
    resetOrderDataFunction: () => { },
    setResetResultFunction: (newResetResultFunction: () => void) => set(() => ({ resetResultFunction: newResetResultFunction })),
    setResetStockDataFunction: (newResetStockDataFunction: () => void) => set(() => ({ resetStockDataFunction: newResetStockDataFunction })),
    setResetOrderDataFunction: (newResetOrderDataFunction: () => void) => set(() => ({ resetOrderDataFunction: newResetOrderDataFunction })),
}));

export default useResetStore;