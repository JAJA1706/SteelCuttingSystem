//import { useEffect, useState } from 'react';
import '@mantine/core/styles.css';
import '@mantine/dates/styles.css'; //if using mantine date picker features
import 'mantine-react-table/styles.css'; //make sure MRT styles were imported in your app root (once)
import { Notifications } from '@mantine/notifications'
import { MantineProvider, createTheme } from '@mantine/core';
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

const theme = createTheme({
    breakpoints: {
      xs: '30em',
      sm: '1000px',
      md: '1300px',
      lg: '74em',
      xl: '90em',
    },
  });

function App() {
    const queryClient = new QueryClient();

    return (
        <MantineProvider theme={theme}>
            <ModalsProvider>
                <QueryClientProvider client={queryClient}>
                    <Notifications />
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