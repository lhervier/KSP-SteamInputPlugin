const fs = require('fs');
const path = require('path');
const Handlebars = require('handlebars');
const VDF = require('vdf-parser');
const { mergeVdfProperties } = require('./utils');
const { compileVdfSource } = require('./handlebars-utils');

// ---------------------------------------------------------
// VDF String encoding/decoding
// ---------------------------------------------------------

/**
 * Encode a string for a VDF file
 * @param {string} s - The string to encode
 * @returns {string} The encoded string
 */
function _encodeVdfString(s) {
    if( typeof s !== 'string' ) {
        return String(s);
    }
    return s.replace(/\\/g, '\\\\').replace(/"/g, '\\"');
}

/** 
 * Decode Valve KeyValues escape sequences in a quoted string value.
 * @param {string} s - The string to decode
 * @returns {string} The decoded string
 */
function _decodeVdfString(s) {
    let result = '';
    for (let i = 0; i < s.length; i++) {
        if (s[i] === '\\' && i + 1 < s.length) {
            const next = s[++i];
            switch (next) {
                case '\\': result += '\\'; break;
                case '"': result += '"'; break;
                case 'n': result += '\n'; break;
                case 't': result += '\t'; break;
                case 'r': result += '\r'; break;
                default: result += next;
            }
        } else {
            result += s[i];
        }
    }
    return result;
}

/** 
 * Recursively decode escaped string values in a parsed VDF tree.
 * @param {Object} node - The node to decode
 * @returns {Object} The decoded node
 */
function _decodeVdfValues(node) {
    if (Array.isArray(node)) {
        for (let i = 0; i < node.length; i++) {
            node[i] = _decodeVdfValues(node[i]);
        }
        return node;
    }
    if (node !== null && typeof node === 'object') {
        for (const key of Object.keys(node)) {
            node[key] = _decodeVdfValues(node[key]);
        }
        return node;
    }
    if (typeof node === 'string') {
        return _decodeVdfString(node);
    }
    return node;
}

/**
 * Parse a VDF file and decode the string values
 * @param {string} content - The content of the VDF file
 * @returns {Object} The parsed object
 * @throws {Error} If the file cannot be parsed
 */
function _parseVdfFile(content) {
    try {
        const parsedObj = VDF.parse(content, { types: false });
        _decodeVdfValues(parsedObj);
        return parsedObj;
    } catch (error) {
        throw new Error(`Error parsing VDF file: ${error.message}`);
    }
}

// ---------------------------------------------------------
// Path utilities
// ---------------------------------------------------------

/**
 * Convert an absolute path, relative to the config root, to an absolute path on the filesystem
 * @param {string} configRoot - The root directory of the VDF files
 * @param {string} vdfAbsolutePath - The absolute path to the VDF file, relative to the config root
 * @returns {string} The absolute path on the filesystem
 */
function _toAbsolutePath(configRoot, vdfAbsolutePath) {
    if( vdfAbsolutePath.startsWith('/') ) {
        return path.join(configRoot, vdfAbsolutePath.substring(1));
    } else {
        return path.join(configRoot, vdfAbsolutePath);
    }
}

/**
 * Convert a path, relative to another VDF file, to an absolute path, relative to the parent VDF file's directory
 * @param {string} absoluteParentVdfPath - The absolute path to the parent VDF file
 * @param {string} vdfPath - The path to the VDF file, relative to the parent VDF file
 * @returns {string} The absolute path, relative to the parent VDF file's directory
 */
function _toAbsoluteConfigPath(absoluteParentVdfPath, vdfPath) {
    if( vdfPath.startsWith('/') ) {
        return vdfPath;
    }
    const folder = path.posix.dirname(absoluteParentVdfPath);
    return path.posix.join(folder, vdfPath);
} 

// ---------------------------------------------------------
// Parse Ref Spec
// ---------------------------------------------------------

/**
 * Parse a ref spec "file.vdf?k1=v1&k2=v2" into { file, params }.
 * Without "?", params is {}. Throws on duplicate keys.
 */
function _parseRefSpec(spec) {
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
 * @param {Object} params - The parameters to canonicalize
 * @returns {string} The canonicalized parameters
 */
function _canonicalizeParams(params) {
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

// ---------------------------------------------------------
// Save VDF File
// ---------------------------------------------------------

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
        if( key === null || key === undefined ) {
            throw new Error(`Property key cannot be null or undefined`);
        }
        if( typeof key !== 'string' ) {
            throw new Error(`Property key must be a string, got ${typeof key}`);
        }
        if( key.includes('\\') ) {
            throw new Error(`Property key cannot contain a backslash, got ${key}`);
        }
        
        if( value === null || value === undefined ) {
            throw new Error(`Property value cannot be null or undefined`);
        }
        
        const indentString = tab.repeat(indent);
        if (Array.isArray(value)) {
            value.forEach(item => {
                writeProperty(key, item, indent);
            });
        } else if (typeof value === 'object' ) {
            result += `${indentString}"${key}"\n${indentString}{\n`;
            formatVdf(value, indent + 1);
            result += `${indentString}}\n`;
        } else if (typeof value == "string") {
            result += `${indentString}"${key}"${tab}${tab}"${_encodeVdfString(value)}"\n`;
        } else {
            throw new Error(`Property ${key} has an invalid type: ${typeof value}`);
        }
    }
    
    function formatVdf(obj, indent = 0) {
        for (const [key, value] of Object.entries(obj)) {
            writeProperty(key, value, indent);
        }
    }
    
    formatVdf(obj);
    fs.writeFileSync(filePath, result);
}

// ---------------------------------------------------------
// VDF File Loading
// ---------------------------------------------------------

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
 * Process #ref properties in an object by loading referenced files and merging their properties
 * @param {Object} obj - The object to process
 * @param {string} vdfPath - Absolute that was used to load the object (relative to configRoot)
 * @param {object} [hbsContext] - Handlebars context
 * @param {string} configRoot - Config root directory to resolve VDF paths
 * @returns {Object} The processed object with #ref properties resolved
 * @throws {Error} If a referenced file cannot be loaded or doesn't have a "ref" root property
 */
function _processRefs(obj, vdfPath, hbsContext, configRoot) {
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
            return _processRefs(node, vdfPath, hbsContext, configRoot);
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
        const { file: refFile, params: refParams } = _parseRefSpec(refSpec);

        let refConfigPath = _toAbsoluteConfigPath(vdfPath, refFile);
        
        const refVdf = _loadVdfFile(configRoot, refConfigPath + _canonicalizeParams(refParams), hbsContext);
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
    const { file: diskPath, params } = _parseRefSpec(vdfPath);
    const raw = fs.readFileSync(_toAbsolutePath(configRoot, diskPath), 'utf8');
    const fileContext = Object.keys(params).length > 0
        ? { ...hbsContext, ...params }
        : hbsContext;
    let content = compileVdfSource(raw, fileContext, vdfPath);
    
    let parsedObj;
    try {
        parsedObj = _parseVdfFile(content);
    } catch (error) {
        throw new Error(`Error parsing ${vdfPath}: ${error.message}`);
    }
    
    // Process #ref properties
    return _processRefs(parsedObj, vdfPath, hbsContext, configRoot);
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}