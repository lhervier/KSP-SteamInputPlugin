const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');

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
 * @param {string} currentDir - Current directory for resolving relative paths
 * @param {string} controllerName - Name of the controller for specialized files (e.g., "controller_steamcontroller_gordon")
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function processRefs(obj, currentDir, controllerName) {
    if (obj === null) {
        return null;
    }

    const result = {};
    let refPaths = [];
    
    for (const [key, value] of Object.entries(obj)) {
        if (key === '#ref') {
            refPaths = addRef(refPaths, value);
            continue;
        }
        
        var processedValues;
        if (Array.isArray(value)) {
            processedValues = [];
            for( const item of value ) {
                const processedValue = processRefs(item, currentDir, controllerName);
                processedValues.push(processedValue);
            }
        } else if( typeof value === 'object' ) {
            processedValues = processRefs(value, currentDir, controllerName);
        } else {
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
        
        const refVdf = loadVdfFile(refAbsolutePath, controllerName);
        if( !refVdf.ref ) {
            throw new Error(`Le fichier référencé ${refAbsolutePath} doit avoir "ref" comme propriété racine`);
        }
        const processedRef = refVdf.ref;
        
        for (const [key, value] of Object.entries(processedRef)) {
            if( key === '#ref' ) {
                refPaths = addRef(refPaths, value);
                continue;
            }
            result[key] = mergeVdfProperties(result[key], value);
        }
    }
    
    return result;
}

/**
 * Load, clean and parse a VDF file
 * @param {string} vdfPath - Path of the VDF file to load
 * @param {string} controllerName - Name of the controller for specialized files (e.g., "controller_steamcontroller_gordon")
 * @returns {Object} Parsed object
 * @throws {Error} If the file cannot be loaded or parsed
 */
function loadVdfFile(vdfPath, controllerName) {
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
    return processRefs(parsedObj, path.dirname(vdfPath), controllerName);
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}