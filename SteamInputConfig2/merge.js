const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');
const { saveVdfFile, loadVdfFile, translateVdf } = require('./vdf-utils');

const controllers = JSON.parse(
    fs.readFileSync(path.join(__dirname, 'controllers.json'), 'utf8')
);

// Only one CLI argument is expected: the controller name
const [controllerName] = process.argv.slice(2);
if (!controllerName) {
    throw new Error('Usage: node merge.js <controllerName>');
}

const controllerConfig = controllers.find(
    controller => controller.controllerName === controllerName
);
if (!controllerConfig) {
    const knownControllers = controllers
        .map(controller => controller.controllerName)
        .join(', ');
    throw new Error(`Unknown controller "${controllerName}". Known controllers: ${knownControllers}`);
}

const rootVdfPath = controllerConfig.rootVdfPath;

// Load the root controller file, resolving #ref
const vdf = loadVdfFile(
    path.join('.', rootVdfPath),
    controllerName
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
        path.join(buildDir, rootVdfPath.replace('.vdf', '') + "_" + lang + "_" + buildDateForFilename + ".vdf")
    );
}

console.log('VDF file merged and saved successfully.');
