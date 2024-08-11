import { useMutation } from '@tanstack/react-query'
import { Stock } from '../components/DataTable/DynamicTableStock';
import { Order } from '../components/DataTable/DynamicTableOrder';

interface CuttingStockProblemBody {
    problemType: string,
    stockList: Stock[],
    orderList: Order[],
}
const postCuttingStockProblem = (body: CuttingStockProblemBody) => {
    return fetch(import.meta.env.VITE_BACKEND_API_URL,{
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
    }).then(async (response) => {
        if (response.ok) {
            return response.json();
        } else {
            const errorBody = await response.text();
            throw new Error(`${response.status}: ${errorBody}`);
        }
    }).catch(error => {
        console.error('Request failed:', error);
        throw error;
    });
};

const useSolveCuttingStockProblem = (onError: (error: Error) => void) => {
    return useMutation({
        mutationFn: postCuttingStockProblem,
        onError: onError,
    });
}

export default useSolveCuttingStockProblem;