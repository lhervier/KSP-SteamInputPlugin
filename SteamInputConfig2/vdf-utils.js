const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');

const ids = {};

function resetIds() {
    ids.group = {};
    ids.group.count = 0;
    ids.group.ids = {};
    ids.preset = {};
    ids.preset.count = 0;
    ids.preset.ids = {};
}

/**
 * Deep clone an object recursively, including all nested objects and arrays
 * @param {*} obj - The object to clone
 * @returns {*} - A deep copy of the object
 */
function deepClone(obj) {
    if (obj === null || typeof obj !== 'object') {
        return obj;
    }
    
    if (obj instanceof Date) {
        return new Date(obj.getTime());
    }
    
    if (Array.isArray(obj)) {
        return obj.map(item => deepClone(item));
    }
    
    if (typeof obj === 'object') {
        const cloned = {};
        for (const key in obj) {
            if (obj.hasOwnProperty(key)) {
                cloned[key] = deepClone(obj[key]);
            }
        }
        return cloned;
    }
    
    return obj;
}

/**
 * Merge two VDF properties.
 * If the source property is an object, it will be merged as an array with the target value.
 * If the source property is an array, it will be added to the target value.
 * @param {*} source 
 * @param {*} target 
 */
function mergeVdfProperties(source, target) {
    if( !source ) {
        return target;
    }
    if( Array.isArray(source) ) {
        if( Array.isArray(target) ) {
            return [...source, ...target];
        } else {
            return [...source, target];
        }
    } else {
        if( Array.isArray(target) ) {
            return [source, ...target];
        } else {
            return [source, target];
        }
    }
}

/**
 * Format and save a VDF object to a file
 * @param {Object} obj - The object to save
 * @param {string} filePath - The path of the output file
 * @throws {Error} If the file cannot be written
 */
function saveVdfFile(obj, filePath) {
    const tab = '\t';
    let result = '';
    
    function writeProperty(key, value, indent) {
        if (Array.isArray(value)) {
            // Case of arrays: write each element with the same key
            value.forEach(item => {
                result += `${tab.repeat(indent)}"${key}"\n${tab.repeat(indent)}{\n`;
                
                // Special case for groups and presets: write the id first
                if ( (key === 'group' || key === 'preset') && item.id !== undefined) {
                    result += `${tab.repeat(indent + 1)}"id"\t\t"${item.id}"\n`;
                    const { id, ...rest } = item;
                    formatVdf(rest, indent + 1);
                } else {
                    formatVdf(item, indent + 1);
                }
                
                result += `${tab.repeat(indent)}}\n`;
            });
        } else if (typeof value === 'object' && value !== null) {
            result += `${tab.repeat(indent)}"${key}"\n${tab.repeat(indent)}{\n`;
            formatVdf(value, indent + 1);
            result += `${tab.repeat(indent)}}\n`;
        } else {
            result += `${tab.repeat(indent)}"${key}"\t\t"${value}"\n`;
        }
    }
    
    function formatVdf(obj, indent = 0) {
        // Write the properties in the specified order
        const orderedProps = ['actions', 'action_layers', 'localization', 'group', 'preset', 'settings'];
        
        // Write first the unordered properties
        for (const [key, value] of Object.entries(obj)) {
            if (!orderedProps.includes(key)) {
                writeProperty(key, value, indent);
            }
        }
        
        // Write then the ordered properties
        orderedProps.forEach(prop => {
            if (obj[prop] !== undefined) {
                writeProperty(prop, obj[prop], indent);
            }
        });
    }
    
    formatVdf(obj);
    fs.writeFileSync(filePath, result);
}

function addRef(refPaths, additionRefPaths) {
    if( typeof additionRefPaths === 'string' ) {
        additionRefPaths = [additionRefPaths];
    }
    for( const additionRefPath of additionRefPaths ) {
        if( typeof additionRefPath !== 'string' ) {
            throw new Error(`#ref array must contain only strings, got ${typeof additionRefPath}`);
        }
    }
    return mergeVdfProperties(refPaths, additionRefPaths);
}

/**
 * Process #ref properties in an object by loading referenced files and merging their properties
 * @param {Object} obj - The object to process
 * @param {string} parentName - Name of the parent tag (e.g. "group" or "preset")
 * @param {string} vdfPath - Current file path that was used to load the object
 * @param {string} controllerName - Name of the controller for specialized files (e.g., "steamcontroller")
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function processRefs(obj, parentName, vdfPath, controllerName) {
    if (obj === null) {
        return null;
    }
    const currentDir = path.dirname(vdfPath);

    const result = {};
    let refPaths = [];
    
    for (let [key, value] of Object.entries(obj)) {
        if (key === '#ref') {
            refPaths = addRef(refPaths, value);
            continue;
        }

        if( key === 'id' ) {
            if( parentName == null ) {
                throw new Error(`Unable to set an id to the root objet`);
            }
            if( !ids[parentName] ) {
                throw new Error(`Unable to set an id on a ${parentName} object`);
            }

            if( value === "#fileName" ) {
                value = '/' + vdfPath.replace(/\\/g, '/');
            }
            if( ids[parentName].ids[value] !== undefined ) {
                throw new Error(`Id already set on file ${value}`);
            }
            ids[parentName].ids[value] = ids[parentName].count;
            value = "" + ids[parentName].count;
            ids[parentName].count++;
        }
        
        var processedValues;
        if (Array.isArray(value)) {
            processedValues = [];
            for( const item of value ) {
                const processedValue = processRefs(item, key,vdfPath, controllerName);
                processedValues.push(processedValue);
            }
        } else if( typeof value === 'object' ) {
            processedValues = processRefs(value, key, vdfPath, controllerName);
        } else {
            // If there is an id in the value, we must make sure that it is an absolute path
            if( typeof value === 'string' ) {
                const match = value.match(/%([^%]+):([^%]+)%/);
                if( match ) {
                    if( match[1] === 'group_id' ) {
                        let id = match[2];
                        if( !id.startsWith('/')) {
                            id = '/' + path.join(path.dirname(vdfPath), id).replace(/\\/g, '/');
                        }
                        value = value.replace(match[2], id);
                    }
                }
            }
            processedValues = value;
        }
        result[key] = mergeVdfProperties(result[key], processedValues);
    }

    while( refPaths.length > 0 ) {
        const refPath = refPaths.shift();
        
        // Determine the ref absolute path
        let refAbsolutePath;
        if (refPath.startsWith('/')) {
            // Absolute path (relative to root)
            refAbsolutePath = path.join('.', refPath.substring(1));
        } else {
            // Relative path (relative to current file)
            refAbsolutePath = path.join(currentDir, refPath);
        }
            
        // Generate specialized path
        const dir = path.dirname(refAbsolutePath);
        const ext = path.extname(refAbsolutePath);
        const name = path.basename(refAbsolutePath, ext);
        
        // Check if specialized file exists
        if (controllerName) {
            const specializedPath = path.join(dir, `${name}.${controllerName}${ext}`);
            if (fs.existsSync(specializedPath)) {
                refPaths.push('/' + specializedPath.replace(/\\/g, '/'));
            }
        }
        
        const refVdf = _loadVdfFile(refAbsolutePath, controllerName);
        if( !refVdf.ref ) {
            throw new Error(`Le fichier référencé ${refAbsolutePath} doit avoir "ref" comme propriété racine`);
        }
        const processedRef = refVdf.ref;
        
        for (const [key, value] of Object.entries(processedRef)) {
            result[key] = mergeVdfProperties(result[key], value);
        }
    }
    
    return result;
}

function translateVdf(vdf, lang)  {
    // Search recursively for all properties named "binding" of type string
    function translateBinding(binding, lang) {
        // Bindings may contain references to translation keys via the #key syntax
        const matches = [...binding.matchAll(/#([^,]+)/g)];
        if( !matches || matches.length === 0 ) {
            return binding;
        }
        for( const match of matches ) {
            const key = match[1];
            if( vdf.controller_mappings.localization[lang][key] === undefined ) {
                throw new Error(`Translation key ${key} not found for language ${lang} in binding ${binding}`);
            }
            const translation = vdf.controller_mappings.localization[lang][key];
            binding = binding.replace(match[0], translation);
        }
        return binding;
    }
    function searchBindings(obj, lang) {
        for( const [key, value] of Object.entries(obj) ) {
            if( key === 'binding' && typeof value === 'string' ) {
                obj[key] = translateBinding(value, lang);
            } else if( Array.isArray(value) ) {
                for( const item of value ) {
                    searchBindings(item, lang);
                }
            } else if( typeof value === 'object' ) {
                searchBindings(value, lang);
            }
        }
    }

    const clonedVdf = deepClone(vdf);
    searchBindings(
        clonedVdf, 
        lang
    );
    return clonedVdf;
}

function loadVdfFile(vdfPath, controllerName) {
    resetIds();
    const vdf = _loadVdfFile(vdfPath, controllerName);
    resolvePresets(vdf);
    resolveGroupBindings(vdf);
    duplicateGroups(vdf);
    resolveLayerBindings(vdf);
    return vdf;
}

/**
 * Load, clean and parse a VDF file
 * @param {string} vdfPath - Path of the VDF file to load
 * @param {string} controllerName - Name of the controller for specialized files (e.g., "controller_steamcontroller_gordon")
 * @returns {Object} Parsed object
 * @throws {Error} If the file cannot be loaded or parsed
 */
function _loadVdfFile(vdfPath, controllerName) {
    let content = fs.readFileSync(vdfPath, 'utf8')
        .split('\n')
        .filter(line => !line.trim().startsWith('#'))
        .filter(line => line.length > 0)
        .join('\n');
    
    let parsedObj;
    try {
        parsedObj = VDF.parse(content);
    } catch (error) {
        throw new Error(`Erreur lors du parsing de ${vdfPath}: ${error.message}`);
    }
    
    // Process #ref properties
    return processRefs(parsedObj, null, vdfPath, controllerName);
}

function resolvePresets(vdf) {
    for( const preset of vdf.controller_mappings.preset ) {
        const result = {};
        for( const [key, value] of Object.entries(preset.group_source_bindings) ) {
            if( ids.group.ids[key] === undefined ) {
                throw new Error(`Unable to resolve preset id for ${key}`);
            }
            const id = ids.group.ids[key];
            result[id + ""] = value;
        }
        preset.group_source_bindings = result;
    }
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
                        if( !ids.group.ids[id] ) {
                            throw new Error(`Unable to resolve group id for ${id}`);
                        }
                        const resolvedBinding = binding.replace(`%group_id:${id}%`, ids.group.ids[id]);
                        activatorValue.bindings.binding = resolvedBinding;
                    }
                }
            }
        }
    }
}

/**
 * Make sure that each group is linked to only one preset
 * @param {*} vdf The vdf object 
 */
function duplicateGroups(vdf) {
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
                    duplicateGroup.id = ids.group.count++ + "";
                    vdf.controller_mappings.group.push(duplicateGroup);

                    // Update the preset
                    delete preset.group_source_bindings[key];
                    preset.group_source_bindings[duplicateGroup.id] = value;
                }
            }
        }
    );
}

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
    saveVdfFile,
    loadVdfFile,
    translateVdf
}