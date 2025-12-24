const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');
const { saveVdfFile, loadVdfFile, translateVdf } = require('./vdf-utils');

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
const buildDate = new Date().toISOString().replace('T', ' ').replace('Z', '');
vdf.controller_mappings.title += " (" + buildDate + ")";

// Create a filename-safe version of buildDate (replace ':' with '-' for Windows compatibility)
const buildDateForFilename = buildDate.replace(/:/g, '-');

// Create the "build" directory if it doesn't exist
const buildDir = path.join(__dirname, 'build');
if (!fs.existsSync(buildDir)) {
    fs.mkdirSync(buildDir, { recursive: true });
}

// Translate the VDF file into all known languages
// The list of available languages are in the "localization" property of the vdf
for( const lang of Object.keys(vdf.controller_mappings.localization) ) {
    const translatedVdf = translateVdf(vdf, lang);
    saveVdfFile(
        translatedVdf,
        path.join(buildDir, args[0].replace('.vdf', '') + "_" + lang + "_" + buildDateForFilename + ".vdf")
    );
}

console.log('VDF file merged and saved successfully.');
