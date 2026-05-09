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
const versionFilePath = path.join(__dirname, '..', 'VERSION.txt');

if (!fs.existsSync(versionFilePath)) {
    throw new Error(`VERSION.txt file not found: ${versionFilePath}`);
}

// Load the root controller file, resolving #ref
const vdf = loadVdfFile(
    path.join('.', rootVdfPath),
    controllerName
);

// Update the Timestamp property (set in epoch milliseconds)
vdf.controller_mappings.Timestamp = Date.now().toString();

// Read version from VERSION.txt; fallback to timestamp when using the placeholder.
const rawVersion = fs.readFileSync(versionFilePath, 'utf8').trim();
if (!rawVersion) {
    throw new Error(`VERSION.txt is empty: ${versionFilePath}`);
}
let buildVersion = '';
if (rawVersion.includes('%TIMESTAMP%')) {
    const buildDate = new Date().toISOString().replace('T', ' ').replace('Z', '');
    buildVersion = buildDate.replace(/:/g, '-');
} else {
    buildVersion = rawVersion;
}

// Add the resolved build version to the VDF title
vdf.controller_mappings.title += " (" + buildVersion + ")";

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
        path.join(buildDir, rootVdfPath.replace('.vdf', '') + "_" + lang + "_" + buildVersion + ".vdf")
    );
}

console.log('VDF file merged and saved successfully.');
