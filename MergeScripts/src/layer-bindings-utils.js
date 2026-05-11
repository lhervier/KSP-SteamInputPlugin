/**
 * Resolve the layer_index references in the "binding" properties of activators in the VDF object
 * @param {*} vdf The VDF object to resolve
 */
function resolveLayerBindings(vdf) {
    for( const group of vdf.controller_mappings.group ) {
        if( !group.inputs ) {
            continue;
        }
        for( const [inputKey, inputValue] of Object.entries(group.inputs) ) {
            if( !inputValue.activators ) {
                continue;
            }
            for( let [activatorKey, activatorValues] of Object.entries(inputValue.activators) ) {
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
                    // binding may contain references to an id using the %id% syntax
                    // we need to replace these references with the actual id
                    const match = binding.match(/%([^%]+):([^%]+)%/);
                    if( !match ) {
                        continue;
                    }
                    
                    const type = match[1];
                    const id = match[2];
                    
                    if( type === 'layer_index' ) {
                        // Groups are unique for a given preset (they were duplicated if needed)
                        const groupId = group.id;

                        // Find the preset that corresponds to the group
                        const preset = vdf.controller_mappings.preset.find(preset => preset.group_source_bindings[groupId] !== undefined);
                        if( !preset ) {
                            throw new Error(`Preset for group ${groupId} not found`);
                        }
                        const presetName = preset.name;

                        // Find the preset name of the action layer where the parent set name is the preset name
                        let layerPresetName;
                        for( const [layerKey, layerValue] of Object.entries(vdf.controller_mappings.action_layers) ) {
                            if( layerValue.title !== id) continue;
                            if( layerValue.parent_set_name !== presetName ) continue;
                            layerPresetName = layerKey;
                            break;
                        }
                        if( !layerPresetName ) {
                            throw new Error(`Action layer for preset ${presetName} not found`);
                        }
                        
                        let presetIndex;
                        let found = false;
                        for( let i = 0; i < vdf.controller_mappings.preset.length; i++ ) {
                            const p = vdf.controller_mappings.preset[i];
                            if( p.name !== layerPresetName ) continue;
                            found = true;
                            presetIndex = i + 1;
                            break;
                        }
                        if( !found ) {
                            throw new Error(`Preset ${layerPresetName} not found`);
                        }

                        const resolvedBinding = binding.replace(`%layer_index:${id}%`, presetIndex);
                        activatorValue.bindings.binding = resolvedBinding;
                    }
                }
            }
        }
    }
}

module.exports = {
    resolveLayerBindings
}