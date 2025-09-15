const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');

const ids = {
    "group": {
        "count": 0,
        "ids": {}
    },
    "preset": {
        "count": 0,
        "ids": {}
    }
};

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
 * @param {string} controllerName - Name of the controller for specialized files (e.g., "controller_steamcontroller_gordon")
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
                    let id = match[2];
                    if( !id.startsWith('/')) {
                        id = '/' + path.join(path.dirname(vdfPath), id).replace(/\\/g, '/');
                    }
                    value = value.replace(match[2], id);
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
        const specializedPath = path.join(dir, `${name}.${controllerName}${ext}`);
        
        // Check if specialized file exists
        if (fs.existsSync(specializedPath)) {
            refPaths.push('/' + specializedPath.replace(/\\/g, '/'));
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

function loadVdfFile(vdfPath, controllerName) {
    const vdf = _loadVdfFile(vdfPath, controllerName);
    resolvePresets(vdf);
    resolveGroupBindings(vdf);
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
                    const match = binding.match(/%group_id:([^%]+)%/);
                    if( match ) {
                        const id = match[1];

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

module.exports = {
    saveVdfFile,
    loadVdfFile
}