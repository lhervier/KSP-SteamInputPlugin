const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');
const { saveVdfFile, loadVdfFile } = require('./vdf-utils');

// Load the root controller file, resolving #ref
const vdf = loadVdfFile(
    '.', 
    'controller_steamcontroller_gordon.vdf'
);

// Create the "build" directory if it doesn't exist
const buildDir = path.join(__dirname, 'build');
if (!fs.existsSync(buildDir)) {
    fs.mkdirSync(buildDir, { recursive: true });
}

// Save the merged VDF file
saveVdfFile(
    vdf, 
    path.join(buildDir, 'controller_steamcontroller_gordon.vdf')
);

console.log('VDF file merged and saved successfully.');

