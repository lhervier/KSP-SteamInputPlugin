const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');
const { saveVdfFile, loadVdfFile } = require('./vdf-utils');

const vdf = loadVdfFile(
    '.', 
    'controller_steamcontroller_gordon.vdf'
);
console.log(vdf);
