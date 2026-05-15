const fs = require('fs');
const path = require('path');
const Handlebars = require('handlebars');
const VDF = require('vdf-parser');

/** Truthy OR for subexpressions, e.g. {{#if (or steamcontroller hori xboxelite)}} */
Handlebars.registerHelper('or', function (...args) {
    const values = args.slice(0, -1);
    return values.some(Boolean);
});

/**
 * Strict boolean context flag for #if / #unless: {{#if (true haptic)}}
 * Throws unless the resolved value is strictly true or false (e.g. missing property → undefined).
 */
Handlebars.registerHelper('true', function (value) {
    if (value !== true && value !== false) {
        const hint =
            value === undefined
                ? 'undefined (missing context property?)'
                : typeof value;
        throw new Error(`true helper: expected a boolean context flag, got ${hint}`);
    }
    return value;
});

/**
 * Strict string equality for #if / #unless: {{#if (equals mouseZone "right_trackpad")}}
 * Throws unless both operands are strings (catches missing context properties or typos).
 */
Handlebars.registerHelper('equals', function (a, b) {
    if (typeof a !== 'string' || typeof b !== 'string') {
        const hintA = a === undefined ? 'undefined (missing context property?)' : typeof a;
        const hintB = b === undefined ? 'undefined (missing context property?)' : typeof b;
        throw new Error(`equals helper: expected two strings, got ${hintA} and ${hintB}`);
    }
    return a === b;
});

/**
 * Check if variable exists in context #if / #unless: {{#if (defined mod)}}
 */
Handlebars.registerHelper('defined', function (variable) {
    return variable !== undefined;
});

/**
 * Compile VDF source as a Handlebars template (no HTML escaping, strict lookups).
 * @param {string} source - Raw file contents
 * @param {object} [hbsContext] - Passed through to the template (initialized by merge-*.js)
 * @param {string} vdfPath - For error messages only
 * @returns {string}
 */
function compileVdfSource(source, hbsContext, vdfPath) {
    try {
        const template = Handlebars.compile(source, { noEscape: true, strict: true });
        return template(hbsContext || {});
    } catch (error) {
        throw new Error(`${vdfPath}: Handlebars error: ${error.message}`);
    }
}

/**
 * Merge two VDF properties.
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
                if (typeof item === 'string' || typeof item === 'number' || typeof item === 'boolean') {
                    // Primitive values: write as duplicate key-value pairs (valid VDF)
                    result += `${tab.repeat(indent)}"${key}"\t\t"${item}"\n`;
                } else {
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
                }
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

function toAbsolutePath(configRoot, vdfPath) {
    if( vdfPath.startsWith('/') ) {
        return path.join(configRoot, vdfPath.substring(1));
    } else {
        return path.join(configRoot, vdfPath);
    }
}

function toAbsoluteConfigPath(absoluteParentVdfPath, vdfPath) {
    if( vdfPath.startsWith('/') ) {
        return vdfPath;
    }
    const folder = path.posix.dirname(absoluteParentVdfPath);
    return path.posix.join(folder, vdfPath);
} 

/**
 * Parse a ref spec "file.vdf?k1=v1&k2=v2" into { file, params }.
 * Without "?", params is {}. Throws on duplicate keys.
 */
function parseRefSpec(spec) {
    const idx = spec.indexOf('?');
    if (idx === -1) return { file: spec, params: {} };
    const file = spec.substring(0, idx);
    const usp = new URLSearchParams(spec.substring(idx + 1));
    const params = {};
    for (const [k, v] of usp) {
        if (Object.prototype.hasOwnProperty.call(params, k)) {
            throw new Error(`Duplicate param "${k}" in ref spec: ${spec}`);
        }
        params[k] = v;
    }
    return { file, params };
}

/**
 * Canonicalize params into "?k1=v1&k2=v2" with sorted keys and full URI encoding.
 * Empty params → "". A key with an empty string value becomes "?key" (no "=").
 * Uses encodeURIComponent (not URLSearchParams.toString) so "/" is encoded
 * and downstream path.dirname / path.relative aren't fooled by raw slashes inside values.
 */
function canonicalizeParams(params) {
    const keys = Object.keys(params).sort();
    if (keys.length === 0) return '';
    return '?' + keys
        .map(k => {
            const v = params[k];
            if (v === '') {
                return encodeURIComponent(k);
            }
            return encodeURIComponent(k) + '=' + encodeURIComponent(v);
        })
        .join('&');
}

/**
 * Process #ref properties in an object by loading referenced files and merging their properties
 * @param {Object} obj - The object to process
 * @param {string} vdfPath - Absolute that was used to load the object (relative to configRoot)
 * @param {object} [hbsContext] - Handlebars context
 * @param {string} configRoot - Config root directory to resolve VDF paths
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function processRefs(obj, vdfPath, hbsContext, configRoot) {
    if (obj === null) {
        return null;
    }
    
    const result = {};
    let refPaths = [];

    function processRefValue(node) {
        if (Array.isArray(node)) {
            return node.map(item => processRefValue(item));
        }
        if (node !== null && typeof node === 'object') {
            return processRefs(node, vdfPath, hbsContext, configRoot);
        }
        return node;
    }

    for (let [key, value] of Object.entries(obj)) {
        if (key === '#ref') {
            let refValues = value;
            if( typeof refValues === 'string' ) {
                refValues = [refValues];
            }
            for( const refValue of refValues ) {
                if( typeof refValue !== 'string' ) {
                    throw new Error(`#ref array must contain only strings, got ${typeof refValue}`);
                }
            }
            refPaths = mergeVdfProperties(refPaths, refValues);
            continue;
        }

        var processedValues = processRefValue(value);
        result[key] = mergeVdfProperties(result[key], processedValues);
    }

    while( refPaths.length > 0 ) {
        const refSpec = refPaths.shift();
        const { file: refFile, params: refParams } = parseRefSpec(refSpec);

        let refConfigPath = toAbsoluteConfigPath(vdfPath, refFile);
        
        const refVdf = _loadVdfFile(configRoot, refConfigPath + canonicalizeParams(refParams), hbsContext);
        if( !refVdf.ref ) {
            throw new Error(`Referenced file ${refConfigPath} must have "ref" as the root property`);
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
 * @param {string} configRoot - Directory of the entry VDF (root for "/" paths in the VDF)
 * @param {string} vdfPath - Path to the VDF to load (relative to the configRoot)
 * @param {object} [hbsContext] - Handlebars context for all loaded .vdf
 */
function loadVdfFile(configRoot, vdfPath, hbsContext = {}) {
    return _loadVdfFile(configRoot, vdfPath, hbsContext);
}

/**
 * Load, clean and parse a VDF file
 * @param {string} configRoot - Directory of the entry VDF (root for "/" paths in the VDF)
 * @param {string} vdfPath - Absolute path to the VDF file to load (relative to configRoot)
 * @param {object} [hbsContext] - Handlebars context (same for entry and all #ref targets)
 * @returns {Object} Parsed object
 * @throws {Error} If the file cannot be loaded or parsed
 */
function _loadVdfFile(configRoot, vdfPath, hbsContext) {
    console.log('Loading VDF file:', vdfPath);
    if( !vdfPath.startsWith('/') ) {
        throw new Error(`VDF path must start with "/", got ${vdfPath}`);
    }
    const { file: diskPath, params } = parseRefSpec(vdfPath);
    const raw = fs.readFileSync(toAbsolutePath(configRoot, diskPath), 'utf8');
    const fileContext = Object.keys(params).length > 0
        ? { ...hbsContext, ...params }
        : hbsContext;
    let content = compileVdfSource(raw, fileContext, vdfPath);
    
    let parsedObj;
    try {
        parsedObj = VDF.parse(content);
    } catch (error) {
        throw new Error(`Error parsing ${vdfPath}: ${error.message}`);
    }
    
    // Process #ref properties
    return processRefs(parsedObj, vdfPath, hbsContext, configRoot);
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}