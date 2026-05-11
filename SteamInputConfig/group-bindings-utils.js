/**
 * Resolve the group id references in the "binding" properties of activators in the VDF object
 * @param {*} vdf The VDF object to resolve
 */
function resolveGroupBindings(vdf, groupIds) {
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
                    
                    if( type === 'group_id' ) {
                        if( groupIds[id] === undefined ) {
                            throw new Error(`Unable to resolve group id for ${id}`);
                        }
                        const resolvedBinding = binding.replace(`%group_id:${id}%`, groupIds[id]);
                        activatorValue.bindings.binding = resolvedBinding;
                    }
                }
            }
        }
    }
}

module.exports = {
    resolveGroupBindings
}