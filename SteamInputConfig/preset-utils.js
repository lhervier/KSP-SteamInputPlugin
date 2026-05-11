function resolvePresets(vdf, groupIds) {
    for( const preset of vdf.controller_mappings.preset ) {
        const result = {};
        for( const [key, value] of Object.entries(preset.group_source_bindings) ) {
            if( groupIds[key] === undefined ) {
                throw new Error(`Unable to resolve preset id for ${key}`);
            }
            const id = groupIds[key];
            result[id + ""] = value;
        }
        preset.group_source_bindings = result;
    }
}

module.exports = {
    resolvePresets
}