import { useMutation } from '@tanstack/react-query'
import { Stock } from '../components/DataTable/DynamicTableStock';
import { Order } from '../components/DataTable/DynamicTableOrder';

interface CuttingStockProblemBody {
    problemType: string,
    stockList: Stock[],
    orderList: Order[],
}
const postCuttingStockProblem = (body: CuttingStockProblemBody) => {
    return fetch('https://localhost:7040/CuttingStock', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body),
    }).then(o => {
        return o.json();
    });
};

const useSolveCuttingStockProblem = () => {
    return useMutation({
        mutationFn: postCuttingStockProblem,
    });
}

export default useSolveCuttingStockProblem;