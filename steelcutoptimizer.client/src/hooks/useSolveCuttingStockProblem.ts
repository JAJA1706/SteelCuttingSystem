import { useMutation } from '@tanstack/react-query'
import { Stock } from '../components/DataTables/StockTable';
import { Order } from '../components/DataTables/OrderTable';
import { Settings } from '../components/SettingsPanel/SettingsPanel';

export interface Segment {
    orderId: number;
    length: number;
    relaxAmount: number;
}
interface Pattern {
    stockId: number;
    stockLength: number;
    count: number;
    segmentList: Segment[];
}
export interface ResultPattern {
    patternId: number;
    stockId: number;
    stockLength: number;
    count: number;
    segmentList: Segment[];
}
export interface AmplResults {
    totalCost: number | undefined;
    totalWaste: number;
    totalRelax: number | undefined;
    patterns: ResultPattern[];
    orderPrices: number[];
    stockLimits: number[];
}
export interface CuttingStockProblemBody {
    algorithmSettings: Settings,
    stockList: Stock[],
    orderList: Order[],
    areBasicPatternsAllowed?: boolean | undefined,
    patterns?: Pattern[];
    orderPrices?: number[];
    stockLimits?: number[];
}

const postCuttingStockProblem = (body: CuttingStockProblemBody): Promise<AmplResults>  => {
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

const useSolveCuttingStockProblem = (onError: (error: Error) => void, onSuccess: (data: AmplResults) => void) => {
    return useMutation<AmplResults, Error, CuttingStockProblemBody>({
        mutationFn: postCuttingStockProblem,
        onError: onError,
        onSuccess: onSuccess,
    });
}

export default useSolveCuttingStockProblem;