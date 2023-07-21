module.exports = {
  root: true,
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module'
  },
  rules: {
    indent: ['error', 4],
    quotes: [2, 'single', { avoidEscape: true }],
    'max-len': 'off',
    'object-curly-spacing': [2, 'always'],
    'linebreak-style': ['error', 'windows'],
    'no-console': ['error', { allow: ['log', 'error'] }],
    'func-names': ['error', 'as-needed'],
    'no-multiple-empty-lines': ['error', { 'max': 1, 'maxEOF': 0 }],
    'dot-notation': 'off',
    'no-plusplus': 'off',
    'prefer-rest-params': 'off',
    'no-underscore-dangle': 'off',
    'no-promise-executor-return': 'off',
    'object-curly-newline': ['error', {
        'ObjectExpression': 'always',
        'ObjectPattern': { 'multiline': true },
        'ImportDeclaration': { 'multiline': true, 'minProperties': 0 },
        'ExportDeclaration': 'never'
    }]
  },
};
