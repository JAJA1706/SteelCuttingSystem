//import { useEffect, useState } from 'react';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css'; //if using mantine date picker features
import 'mantine-react-table/styles.css'; //make sure MRT styles were imported in your app root (once)
import { Notifications } from '@mantine/notifications'
import { MantineProvider } from '@mantine/core';
import { ModalsProvider } from '@mantine/modals';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import LayoutWithHeaderSidebar from "./components/LayoutWithHeaderSidebar/LayoutWithHeaderSidebar"
//import classes from './App.module.css';

//interface Forecast {
//    date: string;
//    temperatureC: number;
//    temperatureF: number;
//    summary: string;
//}

//const theme = createTheme({
//    components: {
//        Input: Input.extend({ classNames: classes }),
//    },
//});

function App() {
    //const [forecasts, setForecasts] = useState<Forecast[]>();

    //useEffect(() => {
    //    populateWeatherData();
    //}, []);

    //const contents = forecasts === undefined
    //    ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
    //    : <table className="table table-striped" aria-labelledby="tabelLabel">
    //        <thead>
    //            <tr>
    //                <th>Date</th>
    //                <th>Temp. (C)</th>
    //                <th>Temp. (F)</th>
    //                <th>Summary</th>
    //            </tr>
    //        </thead>
    //        <tbody>
    //            {forecasts.map(forecast =>
    //                <tr key={forecast.date}>
    //                    <td>{forecast.date}</td>
    //                    <td>{forecast.temperatureC}</td>
    //                    <td>{forecast.temperatureF}</td>
    //                    <td>{forecast.summary}</td>
    //                </tr>
    //            )}
    //        </tbody>
    //    </table>;
    const queryClient = new QueryClient();

    return (
        <MantineProvider >
            <ModalsProvider>
                <QueryClientProvider client={queryClient}>
                    <Notifications />
            {/*<div>*/}
            {/*    <h1 id="tabelLabel">Weather forecast</h1>*/}
            {/*    <p>This component demonstrates fetching data from the server.</p>*/}
            {/*    {contents}*/}
            {/*    <Button>hehe</Button>*/}
            {/*</div>*/}
                    <LayoutWithHeaderSidebar />
                </QueryClientProvider>
            </ModalsProvider>
        </MantineProvider>
    );

    //async function populateWeatherData() {
    //    const response = await fetch('weatherforecast');
    //    const data = await response.json();
    //    setForecasts(data);
    //}
}

export default App;