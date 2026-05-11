const { deepClone } = require('./utils');

/**
 * Clone the VDF and replace #key references in every string value using localizationMap.
 * Each key is matched as # followed by one or more characters that are not comma, whitespace,
 * or # (so a key ends before the next comma, or before whitespace, or before another #, or at EOS).
 * @param {*} vdf Root object to deep-clone and mutate.
 * @param {Object<string, string>} localizationMap Translation keys to resolved strings.
 */
function translateVdf(vdf, localizationMap) {
    if (!localizationMap || typeof localizationMap !== 'object') {
        throw new Error('translateVdf: localizationMap must be a non-null object');
    }

    const KEY_PATTERN = /#([^,\s#]+)/g;

    function translateString(str) {
        if (typeof str !== 'string' || str.indexOf('#') === -1) {
            return str;
        }
        return str.replace(KEY_PATTERN, (match, key) => {
            if (localizationMap[key] === undefined) {
                return match;
            }
            return localizationMap[key];
        });
    }

    function walk(obj) {
        if (obj === null || typeof obj !== 'object') {
            return;
        }
        if (Array.isArray(obj)) {
            for (const item of obj) {
                walk(item);
            }
            return;
        }
        for (const [k, value] of Object.entries(obj)) {
            if (typeof value === 'string') {
                obj[k] = translateString(value);
            } else {
                walk(value);
            }
        }
    }

    const clonedVdf = deepClone(vdf);
    walk(clonedVdf);
    return clonedVdf;
}

module.exports = {
    translateVdf
}