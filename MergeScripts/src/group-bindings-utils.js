/**
 * Find, within the preset that owns the binding, the numeric id of the group targeted by a
 * "mode_shift <zone> <modeshiftId>" binding.
 *
 * @param {*} vdf The VDF object
 * @param {string} groupId The numeric id of the group holding the binding
 * @param {string} groupFilePath The source path of that group (for error messages)
 * @param {string} zone The physical zone targeted by the mode_shift
 * @param {string} modeshiftId The modeshift id targeted by the mode_shift
 * @returns {string} The numeric id of the targeted group
 * @throws {Error} If the owner preset, or a single matching target, cannot be found
 */
function resolveModeshift(vdf, groupId, groupFilePath, zone, modeshiftId) {
    for( const preset of vdf.controller_mappings.preset ) {
        // The binding's group lives in exactly one preset; after phase 2 its key is
        // "<groupId>[:<modeshiftId>]", so compare on the group id part only.
        const ownsBinding = Object.keys(preset.group_source_bindings)
            .some(key => key.split(':')[0] === groupId);
        if( !ownsBinding ) {
            continue;
        }

        // A target is a modeshift entry of the same preset matching BOTH the zone (from the
        // label) and the id (from the key). The pair is needed because one zone may declare
        // several modeshifts, and one id (e.g. "main") is shared across zones.
        const matches = [];
        for( const [key, label] of Object.entries(preset.group_source_bindings) ) {
            const [targetGroupId, targetModeshiftId] = key.split(':');
            if( targetModeshiftId === undefined ) {
                continue;
            }
            if( !label.includes("modeshift") ) {
                continue;
            }
            if( label.split(' ')[0] === zone && targetModeshiftId === modeshiftId ) {
                matches.push(targetGroupId);
            }
        }

        if( matches.length === 0 ) {
            throw new Error(`Modeshift target not found: zone "${zone}" id "${modeshiftId}" for group ${groupFilePath} in preset ${preset.name}`);
        }
        if( matches.length > 1 ) {
            throw new Error(`Ambiguous modeshift: zone "${zone}" id "${modeshiftId}" matches ${matches.length} groups in preset ${preset.name}`);
        }
        return matches[0];
    }
    throw new Error(`Group ${groupFilePath}: Modeshifting to zone "${zone}" id "${modeshiftId}", but the group was not found in any preset`);
}

/**
 * Resolve the group id references in the "binding" properties of activators in the VDF object.
 * A "mode_shift <zone> <id>" binding is rewritten to "mode_shift <zone> <numericGroupId>".
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
                        const parts = binding.split(' ');
                        const zone = parts[1];
                        const modeshiftId = parts[2];
                        if( !zone || !modeshiftId ) {
                            throw new Error(`Group ${group.filepath}: malformed binding "${binding}" (expected "mode_shift <zone> <id>")`);
                        }
                        const modeShiftGroup = resolveModeshift(vdf, group.id, group.filepath, zone, modeshiftId);
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
