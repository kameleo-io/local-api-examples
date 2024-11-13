module.exports = {
  root: true,
  parserOptions: {
    ecmaVersion: 'latest',
    sourceType: 'module'
  },
  rules: {
    indent: ['warn', 4],
    quotes: ['warn', 'single', { avoidEscape: true }],
    'max-len': 'off',
    "no-trailing-spaces": 'warn',
    'object-curly-spacing': ["warn", 'always'],
    'linebreak-style': ['warn', 'windows'],
    'no-console': ['error', { allow: ['log', 'error'] }],
    'func-names': ['error', 'as-needed'],
    'no-multiple-empty-lines': ['warn', { 'max': 1, 'maxEOF': 0 }],
    'dot-notation': 'off',
    'no-plusplus': 'off',
    'prefer-rest-params': 'off',
    'no-underscore-dangle': 'off',
    'no-promise-executor-return': 'off',
    'object-curly-newline': ['warn', {
        'ObjectExpression': 'always',
        'ObjectPattern': { 'multiline': true },
        'ImportDeclaration': { 'multiline': true, 'minProperties': 0 },
        'ExportDeclaration': 'never'
    }]
  },
};
