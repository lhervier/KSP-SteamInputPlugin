function resolveModeshift(vdf, groupId, groupFilePath, zone) {
    for( const preset of vdf.controller_mappings.preset ) {
        for( const [key, _] of Object.entries(preset.group_source_bindings) ) {
            if( key !== groupId ) {
                continue;
            }
            for( const [searchedGroupId, bind] of Object.entries(preset.group_source_bindings) ) {
                if( !bind.includes("modeshift")) continue;
                const searchedZone = bind.split(' ')[0];
                if( searchedZone === zone ) {
                    return searchedGroupId;
                }
            }
            throw new Error(`Modeshift zone ${zone} not found for group ${groupFilePath} in preset ${preset.name}`);
        }
    }
    throw new Error(`Group ${groupFilePath}: Modeshifting to zone ${zone}, but not found in presets...`);
}

/**
 * Resolve the group id references in the "binding" properties of activators in the VDF object
 * @param {*} vdf The VDF object to resolve
 */
function resolveGroupBindings(vdf) {
    for( const group of vdf.controller_mappings.group ) {
        if( !group.inputs ) {
            continue;
        }
        for( const [_, inputValue] of Object.entries(group.inputs) ) {
            if( !inputValue.activators ) {
                continue;
            }
            for( let [_, activatorValues] of Object.entries(inputValue.activators) ) {
                if( !Array.isArray(activatorValues) ) {
                    activatorValues = [activatorValues];
                }
                for( const activatorValue of activatorValues ) {
                    if( !activatorValue.bindings ) {
                        continue;
                    }
                    if( !activatorValue.bindings.binding ) {
                        continue;
                    }
                    const binding = activatorValue.bindings.binding;
                    if( binding.startsWith('mode_shift ')) {
                        const zone = binding.split(' ')[1];
                        const modeShiftGroup = resolveModeshift(vdf, group.id, group.filepath, zone);
                        activatorValue.bindings.binding = "mode_shift " + zone + " " + modeShiftGroup;
                    }
                }
            }
        }
    }
}

module.exports = {
    resolveGroupBindings
}