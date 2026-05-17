/**
 * Order the main properties of the VDF
 * @param {Object} controllerMapping - The controller mapping object
 * @returns {Object} The VDF object with the main properties ordered
 */
function _orderControllerMapping(controllerMapping) {
    const leadingKeys = [
        'title',
        'controller_type',
        'version',
        'revision',
        'description',
        'creator',
        'progenitor',
        'export_type',
        'controller_caps',
        'major_revision',
        'minor_revision',
        'Timestamp',
    ];
    const mainKeys = ['actions', 'action_layers', 'localization', 'group', 'preset', 'settings'];

    const orderedKeys = [];
    const seen = new Set();

    function appendKeys(keys) {
        for (const key of keys) {
            if (Object.prototype.hasOwnProperty.call(controllerMapping, key) && !seen.has(key)) {
                orderedKeys.push(key);
                seen.add(key);
            }
        }
    }

    appendKeys(leadingKeys);
    appendKeys(mainKeys);
    for (const key of Object.keys(controllerMapping)) {
        if (!seen.has(key)) {
            orderedKeys.push(key);
            seen.add(key);
        }
    }

    return Object.fromEntries(orderedKeys.map(k => [k, controllerMapping[k]]));
}

function _orderIdFirst(obj) {
    const clonedObj = {
        id: obj.id
    }
    for( const [key, value] of Object.entries(obj) ) {
        if( key !== 'id' ) {
            clonedObj[key] = value;
        }
    }
    return clonedObj;
}

/**
 * Order the group properties
 * @param {*} group - The group object
 * @returns {Object} The group object with the id first
 */
function _orderGroupProperties(group) {
    return _orderIdFirst(group);
}

/**
 * Order the preset properties
 * @param {*} preset - The preset object
 * @returns {Object} The preset object with the id first
 */
function _orderPresetProperties(preset) {
    return _orderIdFirst(preset);
}

function _orderActivatorsProperties(vdf) {
    if( !vdf.controller_mappings.group ) {
        return;
    }
    for( const group of vdf.controller_mappings.group ) {
        if( !group.inputs ) {
            continue;
        }
        for( const [inputKey, inputValue] of Object.entries(group.inputs) ) {
            if( !inputValue.activators ) {
                continue;
            }
            for( const [activatorKey, activatorValue] of Object.entries(inputValue.activators) ) {
                if( activatorValue.settings ) {
                    // Move settings as the last property
                    const settings = activatorValue.settings;
                    delete activatorValue.settings;
                    activatorValue.settings = settings;
                }
            }
        }
    }
}

function _removeGroupFilepath(vdf) {
    if( !vdf.controller_mappings.group ) {
        return;
    }
    const groups = Array.isArray(vdf.controller_mappings.group) ? vdf.controller_mappings.group : [vdf.controller_mappings.group];
    for( const group of groups ) {
        delete group.filepath;
    }
}

function orderControllerProperties(vdf) {
    vdf.controller_mappings = _orderControllerMapping(vdf.controller_mappings);

    if( vdf.controller_mappings.group ) {
        const groups = Array.isArray(vdf.controller_mappings.group) ? vdf.controller_mappings.group : [vdf.controller_mappings.group];
        for( let i=0; i<groups.length; i++ ) {
            groups[i] = _orderGroupProperties(groups[i]);
        }
    }

    if( vdf.controller_mappings.preset ) {
        const presets = Array.isArray(vdf.controller_mappings.preset) ? vdf.controller_mappings.preset : [vdf.controller_mappings.preset];
        for( let i=0; i<presets.length; i++ ) {
            presets[i] = _orderPresetProperties(presets[i]);
        }
    }

    _orderActivatorsProperties(vdf);
    _removeGroupFilepath(vdf);

    return vdf;
}

module.exports = {
    orderControllerProperties
}