const createDownloadFileName = (): string => {
    let result = '';
    const characters = '0123456789';
    const stringLength = 6
    for (let i = 0; i < stringLength; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }

    return `plan${result}.txt`
}

const downloadFile = async (data: any) => {
    const blob = new Blob([JSON.stringify(data, null, 1)], { type: 'application/json' })
    const a = document.createElement('a');
    a.download = createDownloadFileName();
    a.href = URL.createObjectURL(blob);
    a.addEventListener('click', () => {
        setTimeout(() => URL.revokeObjectURL(a.href), 30 * 1000);
    });
    a.click();
};

export default downloadFile