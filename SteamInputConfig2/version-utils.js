const fs = require('fs');
const path = require('path');

const VERSION_FILE = path.join(__dirname, '..', 'VERSION.txt');

/**
 * Read VERSION.txt at the repo root and return the build version string.
 * If the file contains %TIMESTAMP% (possibly among other text), uses a filesystem-safe timestamp.
 */
function getVersion() {
    if (!fs.existsSync(VERSION_FILE)) {
        throw new Error(`VERSION.txt file not found: ${VERSION_FILE}`);
    }
    const rawVersion = fs.readFileSync(VERSION_FILE, 'utf8').trim();
    if (!rawVersion) {
        throw new Error(`VERSION.txt is empty: ${VERSION_FILE}`);
    }
    if (rawVersion.includes('%TIMESTAMP%')) {
        const buildDate = new Date().toISOString().replace('T', ' ').replace('Z', '');
        return buildDate.replace(/:/g, '-');
    }
    return rawVersion;
}

module.exports = {
    getVersion
};
