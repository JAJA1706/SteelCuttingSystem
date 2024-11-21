const variables = require('./src/consts/variables.cjs')

module.exports = {
    plugins: [
        require('postcss-preset-mantine'),
        require('postcss-simple-vars')({ variables }),
    ],
};