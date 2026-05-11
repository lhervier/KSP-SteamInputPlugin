const { deepClone } = require('./utils');

/**
 * Make sure that each group is linked to only one preset
 * @param {*} vdf The vdf object 
 * @param {number} nextGroupId The next group id to use
 * @returns {number} The next group id to use
 */
function duplicateGroups(vdf, nextGroupId) {
    const existingGroups = [];
    vdf.controller_mappings.preset.forEach(
        preset => {
            for( const [key, value] of Object.entries(preset.group_source_bindings) ) {
                if( !existingGroups.includes(key) ) {
                    existingGroups.push(key);
                } else {
                    // Find the group
                    const group = vdf.controller_mappings.group.find(group => group.id === key);
                    if( !group ) {
                        throw new Error(`Group ${key} not found`);
                    }
                    
                    // Duplicate the group (recursively)
                    const duplicateGroup = deepClone(group);
                    duplicateGroup.id = nextGroupId++ + "";
                    vdf.controller_mappings.group.push(duplicateGroup);

                    // Update the preset
                    delete preset.group_source_bindings[key];
                    preset.group_source_bindings[duplicateGroup.id] = value;
                }
            }
        }
    );
    return nextGroupId;
}

module.exports = {
    duplicateGroups
}