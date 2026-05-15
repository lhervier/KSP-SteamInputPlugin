const { loadVdfFile } = require('./vdf-utils');

function resolvePresets(vdf, configRoot, context) {
    if (!vdf.controller_mappings.group) {
        vdf.controller_mappings.group = [];
    }
    let nextGroupId = 0;
    let nextPresetId = 0;
    for (const preset of vdf.controller_mappings.preset) {
        const result = {};
        for (const [key, value] of Object.entries(preset.group_source_bindings)) {
            const bindings = Array.isArray(value) ? value : [value];
            for (const binding of bindings) {
                const merged = loadVdfFile(configRoot, key, context);
                if( !merged.ref || !merged.ref.group ) {
                    throw new Error(`Group file ${key} must have "ref" > "group" as root content`);
                }
                const group = merged.ref.group;
                group.id = nextGroupId++ + "";
                group.filepath = key;
                vdf.controller_mappings.group.push(group);
                result[group.id] = binding;
            }
        }
        preset.id = nextPresetId++ + "";
        preset.group_source_bindings = result;
    }
}

module.exports = {
    resolvePresets
}
