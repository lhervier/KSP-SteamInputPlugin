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
    try {
        return VDF.parse(content);
    } catch (error) {
        throw new Error(`Erreur lors du parsing de ${filePath}: ${error.message}`);
    }
}

module.exports = {
    saveVdfFile,
    loadVdfFile
}