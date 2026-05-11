const fs = require('fs');
const path = require('path');
const { getVersion } = require('./version-utils');
const { resetIds, saveVdfFile, loadVdfFile, getIds } = require('./vdf-utils');
const { resolvePresets } = require('./preset-utils');
const { resolveGroupBindings } = require('./group-bindings-utils');
const { duplicateGroups } = require('./group-utils');
const { resolveLayerBindings } = require('./layer-bindings-utils');
const { translateVdf } = require('./translate-utils');

const controllers = JSON.parse(
    fs.readFileSync(path.join(__dirname, 'controllers.json'), 'utf8')
);

// Only one CLI argument is expected: the controller name
const [controllerName] = process.argv.slice(2);
if (!controllerName) {
    throw new Error('Usage: node merge-controller.js <controllerName|all>');
}
const buildVersion = getVersion();

const controllersToBuild = controllerName === 'all'
    ? controllers
    : [controllers.find(controller => controller.controllerName === controllerName)];

if (!controllersToBuild[0]) {
    const knownControllers = controllers
        .map(controller => controller.controllerName)
        .join(', ');
    throw new Error(`Unknown controller "${controllerName}". Known controllers: ${knownControllers}, all`);
}

const buildDir = path.join(__dirname, 'build');
fs.rmSync(buildDir, { recursive: true, force: true });
fs.mkdirSync(buildDir, { recursive: true });

for (const controller of controllersToBuild) {
    const rootVdfPath = controller.rootVdfPath;

    // Load the root controller file, resolving #ref
    const { merged, ids } = loadVdfFile(
        path.join('.', rootVdfPath),
        controller.controllerName
    );
    
    // Resolve the presets, group bindings, duplicate groups and layer bindings
    resolvePresets(merged, ids.group.ids);
    resolveGroupBindings(merged, ids.group.ids);
    duplicateGroups(merged, ids.group.count);
    resolveLayerBindings(merged);

    // Update the Timestamp property (set in epoch milliseconds)
    merged.controller_mappings.Timestamp = Date.now().toString();

    // Add the resolved build version to the VDF title
    merged.controller_mappings.title += " (" + buildVersion + ")";

    // Translate the VDF file into all known languages
    // The list of available languages are in the "localization" property of the vdf
    for (const lang of Object.keys(merged.controller_mappings.localization)) {
        const langDict = merged.controller_mappings.localization[lang];
        const translatedVdf = translateVdf(merged, langDict);
        saveVdfFile(
            translatedVdf,
            path.join(buildDir, rootVdfPath.replace('.vdf', '') + "_" + lang + ".vdf")
        );
    }
}

console.log('VDF file(s) merged and saved successfully.');
