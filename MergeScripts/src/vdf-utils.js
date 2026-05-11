const fs = require('fs');
const path = require('path');
const Handlebars = require('handlebars');
const VDF = require('vdf-parser');

/**
 * @param {object} [hbsContext] - Handlebars root context (from merge-*.js)
 * @returns {string} controllerName for specialized name.controllerName.vdf refs
 */
function controllerNameFromContext(hbsContext) {
    const n = hbsContext && hbsContext.controllerName;
    return typeof n === 'string' ? n : '';
}

/**
 * Compile VDF source as a Handlebars template (no HTML escaping).
 * @param {string} source - Raw file contents
 * @param {object} [hbsContext] - Passed through to the template (initialized by merge-*.js)
 * @param {string} vdfPath - For error messages only
 * @returns {string}
 */
function compileVdfSource(source, hbsContext, vdfPath) {
    try {
        const template = Handlebars.compile(source, { noEscape: true });
        return template(hbsContext || {});
    } catch (error) {
        throw new Error(`${vdfPath}: Handlebars error: ${error.message}`);
    }
}

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
 * @param {object} [hbsContext] - Handlebars context (same object for all #ref loads); controllerName drives specialized .vdf files
 * @param {string} configRoot - Root directory for absolute VDF paths (leading "/"): directory of the entry VDF file
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function processRefs(obj, parentName, vdfPath, hbsContext, configRoot) {
    if (obj === null) {
        return null;
    }
    const currentDir = path.dirname(vdfPath);

    function toVdfRootPath(absolutePath) {
        const resolved = path.resolve(absolutePath);
        const rel = path.relative(configRoot, resolved).replace(/\\/g, '/');
        if (rel && !rel.startsWith('..') && !path.isAbsolute(rel)) {
            return '/' + rel;
        }
        return '/' + resolved.replace(/\\/g, '/');
    }

    const result = {};
    let refPaths = [];
    const controllerName = controllerNameFromContext(hbsContext);

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
                value = toVdfRootPath(vdfPath);
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
                const processedValue = processRefs(item, key, vdfPath, hbsContext, configRoot);
                processedValues.push(processedValue);
            }
        } else if( typeof value === 'object' ) {
            processedValues = processRefs(value, key, vdfPath, hbsContext, configRoot);
        } else {
            // If there is an id in the value, we must make sure that it is an absolute path
            if( typeof value === 'string' ) {
                const match = value.match(/%([^%]+):([^%]+)%/);
                if( match ) {
                    if( match[1] === 'group_id' ) {
                        let id = match[2];
                        if( !id.startsWith('/')) {
                            id = toVdfRootPath(path.join(path.dirname(vdfPath), id));
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
            // Leading "/" in VDF is relative to the entry VDF directory (configRoot)
            refAbsolutePath = path.join(configRoot, refPath.substring(1));
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
                refPaths.push(toVdfRootPath(specializedPath));
            }
        }
        
        const refVdf = _loadVdfFile(configRoot, refAbsolutePath, hbsContext);
        if( !refVdf.ref ) {
            throw new Error(`Referenced file ${refAbsolutePath} must have "ref" as the root property`);
        }
        const processedRef = refVdf.ref;
        
        for (const [key, value] of Object.entries(processedRef)) {
            result[key] = mergeVdfProperties(result[key], value);
        }
    }
    
    return result;
}

/**
 * Load a VDF file. Paths starting with "/" in the file are relative to the directory of the given root VDF path.
 * @param {string} vdfPath - Path to the root VDF to load (relative to the current working directory or absolute)
 * @param {object} [hbsContext] - Handlebars context for all loaded .vdf (from merge-*.js)
 */
function loadVdfFile(vdfPath, hbsContext = {}) {
    const absoluteVdfPath = path.resolve(vdfPath);
    const configRoot = path.dirname(absoluteVdfPath);
    resetIds();
    return {
        merged: _loadVdfFile(configRoot, absoluteVdfPath, hbsContext),
        ids: getIds()
    };
}

/**
 * Load, clean and parse a VDF file
 * @param {string} configRoot - Directory of the entry VDF (root for "/" paths in the VDF)
 * @param {string} vdfPath - Absolute path to the VDF file to load
 * @param {object} [hbsContext] - Handlebars context (same for entry and all #ref targets)
 * @returns {Object} Parsed object
 * @throws {Error} If the file cannot be loaded or parsed
 */
function _loadVdfFile(configRoot, vdfPath, hbsContext) {
    const raw = fs.readFileSync(vdfPath, 'utf8');
    let content = compileVdfSource(raw, hbsContext, vdfPath);
    content = content
        .split('\n')
        .filter(line => !line.trim().startsWith('#'))
        .filter(line => line.length > 0)
        .join('\n');
    
    let parsedObj;
    try {
        parsedObj = VDF.parse(content);
    } catch (error) {
        throw new Error(`Error parsing ${vdfPath}: ${error.message}`);
    }
    
    // Process #ref properties
    return processRefs(parsedObj, null, vdfPath, hbsContext, configRoot);
}

function getIds() {
    return ids;
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}