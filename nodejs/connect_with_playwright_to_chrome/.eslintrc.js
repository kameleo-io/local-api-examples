module.exports = {
    root: true,
    extends: 'airbnb',
    rules: {
        indent: ['error', 4],
        quotes: [2, 'single', { avoidEscape: true }],
        'max-len': 'off',
        'object-curly-spacing': [2, 'always'],
        'linebreak-style': ['error', 'windows'],
        'no-console': ['error', { allow: ['log', 'error'] }],
        'func-names': ['error', 'as-needed'],
        'dot-notation': 'off',
        'no-plusplus': 'off',
        'prefer-rest-params': 'off',
        'no-underscore-dangle': 'off',
    },
};