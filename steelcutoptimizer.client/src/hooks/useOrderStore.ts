import { create } from 'zustand';
import { Order } from '../components/DataTable/OrderTable'

interface OrderStore {
    getOrderData: () => Order[],
    setGetOrderDataFunction: (newGetOrderDataFunction: () => Order[]) => void

    setOrderData: (data: Order[]) => void,
    setSetOrderDataFunction: (newSetOrderDataFunction: (data: Order[]) => void) => void,
}

const useOrderStore = create<OrderStore>()((set) => ({
    getOrderData: () => [],
    setGetOrderDataFunction: (newGetOrderDataFunction: () => Order[]) => set(() => ({ getOrderData: newGetOrderDataFunction })),

    setOrderData: () => { },
    setSetOrderDataFunction: (newSetOrderDataFunction: (data: Order[]) => void) => set(() => ({ setOrderData: newSetOrderDataFunction })),
}));

export default useOrderStore;