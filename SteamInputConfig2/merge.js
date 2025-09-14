const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');
const { saveVdfFile, loadVdfFile } = require('./vdf-utils');

// Récupérer les arguments de ligne de commande
const args = process.argv.slice(2);

// Load the root controller file, resolving #ref
const vdf = loadVdfFile(
    path.join('.', args[0]),
    args[1]
);

// Update the Timestamp property (set in epoch milliseconds)
vdf.controller_mappings.Timestamp = Date.now().toString();

// Add the current date to the title (as YYYY-MM-DD HH:MM:SS.sss)
vdf.controller_mappings.title += " (" + new Date().toISOString().replace('T', ' ').replace('Z', '') + ")";

// Create the "build" directory if it doesn't exist
const buildDir = path.join(__dirname, 'build');
if (!fs.existsSync(buildDir)) {
    fs.mkdirSync(buildDir, { recursive: true });
}

// Save the merged VDF file: Add the build date (as a timestamp) to the filename
let timestamp = new Date().getTime();
saveVdfFile(
    vdf, 
    path.join(buildDir, args[0].replace('.vdf', '') + "_" + timestamp + "_0.vdf")
);

console.log('VDF file merged and saved successfully.');

