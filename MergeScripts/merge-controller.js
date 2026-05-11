const fs = require('fs');
const path = require('path');
const { getVersion } = require('./src/version-utils');
const { saveVdfFile, loadVdfFile } = require('./src/vdf-utils');
const { resolvePresets } = require('./src/preset-utils');
const { resolveGroupBindings } = require('./src/group-bindings-utils');
const { duplicateGroups } = require('./src/group-utils');
const { resolveLayerBindings } = require('./src/layer-bindings-utils');
const { translateVdf } = require('./src/translate-utils');

const [controllersJsonPath, controllerName] = process.argv.slice(2);
if (!controllersJsonPath || !controllerName) {
    throw new Error(
        'Usage: node merge-controller.js <path/to/controllers.json> <controllerName|all>'
    );
}

const controllers = JSON.parse(fs.readFileSync(controllersJsonPath, 'utf8'));
const configDir = path.dirname(path.resolve(controllersJsonPath));
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

    // Load the root controller file, resolving #ref (leading "/" is relative to this VDF's directory)
    const entryVdfPath = path.join(configDir, rootVdfPath);
    const handlebarsContext = {
        controllerName: controller.controllerName,
    };
    handlebarsContext[controller.controllerName] = true;
    const { merged, ids } = loadVdfFile(entryVdfPath, handlebarsContext);
    
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
