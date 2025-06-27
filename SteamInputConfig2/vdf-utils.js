const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');

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

/**
 * Process #ref properties in an object by loading referenced files and merging their properties
 * @param {Object} obj - The object to process
 * @param {string} rootDir - Root directory for resolving absolute paths (starting with /)
 * @param {string} currentDir - Current directory for resolving relative paths
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function processRefs(obj, rootDir, currentDir) {
    if (obj === null) {
        return obj;
    }

    const result = {};
    
    for (const [key, value] of Object.entries(obj)) {
        if (key === '#ref') {
            // Handle #ref property - can be a string or array of strings
            let refPaths = [];
            
            if (typeof value === 'string') {
                refPaths = [value];
            } else if (Array.isArray(value)) {
                // Validate that all elements are strings
                for (const item of value) {
                    if (typeof item !== 'string') {
                        throw new Error(`#ref array must contain only strings, got ${typeof item}`);
                    }
                }
                refPaths = value;
            } else {
                throw new Error(`#ref property must be a string or array of strings, got ${typeof value}`);
            }
            
            // Process each referenced file
            for (const refPathStr of refPaths) {
                // Determine the path
                let refPath;
                if (refPathStr.startsWith('/')) {
                    // Absolute path (relative to root)
                    refPath = path.join(rootDir, refPathStr.substring(1));
                } else {
                    // Relative path (relative to current file)
                    refPath = path.join(currentDir, refPathStr);
                }
                
                // Load the referenced file
                let refContent;
                try {
                    refContent = fs.readFileSync(refPath, 'utf8')
                        .split('\n')
                        .filter(line => !line.trim().startsWith('#'))
                        .join('\n');
                } catch (error) {
                    throw new Error(`Impossible de charger le fichier référencé ${refPath}: ${error.message}`);
                }
                
                // Parse the referenced file
                let refObj;
                try {
                    refObj = VDF.parse(refContent);
                } catch (error) {
                    throw new Error(`Erreur lors du parsing du fichier référencé ${refPath}: ${error.message}`);
                }
                
                // Check if the root property is "ref"
                if (!refObj.hasOwnProperty('ref')) {
                    throw new Error(`Le fichier référencé ${refPath} doit avoir "ref" comme propriété racine`);
                }
                
                // Recursively process the referenced object
                const processedRef = processRefs(refObj.ref, rootDir, path.dirname(refPath));
                
                // Merge the properties from the referenced object into the result
                Object.assign(result, processedRef);
            }
            
        } else if (Array.isArray(value)) {
            // Handle arrays
            result[key] = value.map(item => processRefs(item, rootDir, currentDir));
        } else if (typeof value === 'object' && value !== null) {
            // Handle objects recursively
            result[key] = processRefs(value, rootDir, currentDir);
        } else {
            // Handle primitive values
            result[key] = value;
        }
    }
    
    return result;
}

/**
 * Load, clean and parse a VDF file
 * @param {string} baseDir - Base directory
 * @param {string} relativePath - Relative path of the file from the base directory
 * @returns {Object} Parsed object
 * @throws {Error} If the file cannot be loaded or parsed
 */
function loadVdfFile(baseDir, relativePath) {
    const filePath = path.join(baseDir, relativePath);
    let content = fs.readFileSync(filePath, 'utf8')
        .split('\n')
        .filter(line => !line.trim().startsWith('#'))
        .join('\n');
    
    let parsedObj;
    try {
        parsedObj = VDF.parse(content);
    } catch (error) {
        throw new Error(`Erreur lors du parsing de ${filePath}: ${error.message}`);
    }
    
    // Process #ref properties
    const processedObj = processRefs(parsedObj, baseDir, path.dirname(filePath));
    
    return processedObj;
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}