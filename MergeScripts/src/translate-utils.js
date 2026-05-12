const { deepClone } = require('./utils');

/**
 * Clone the VDF and replace #key references in string values using localizationMap.
 *
 * - Strings that are only a translation token after trim (e.g. "#Set_Flight" or "  #Common_zoomIn  ")
 *   must have that key in localizationMap or an error is thrown.
 * - Other strings (e.g. Steam binding lines like "key_press #C, #Flight_cameraMode, , ") still
 *   replace only keys that exist in the map; unknown #tokens are left unchanged so game action
 *   names are not treated as localization keys.
 * @param {*} vdf Root object to deep-clone and mutate.
 * @param {Object<string, string>} localizationMap Translation keys to resolved strings.
 */
function translateVdf(vdf, localizationMap) {
    if (!localizationMap || typeof localizationMap !== 'object') {
        throw new Error('translateVdf: localizationMap must be a non-null object');
    }

    const KEY_PATTERN = /#([^,\s#]+)/g;
    /** Whole string is exactly one #key (translation reference), ignoring outer whitespace */
    const SOLE_TRANSLATION_REF = /^#([^,\s#]+)$/;

    function translateString(str) {
        if (typeof str !== 'string' || str.indexOf('#') === -1) {
            return str;
        }
        const trimmed = str.trim();
        const sole = SOLE_TRANSLATION_REF.exec(trimmed);
        if (sole) {
            const key = sole[1];
            if (localizationMap[key] === undefined) {
                throw new Error(
                    `translateVdf: missing localization key "${key}" (sole reference): ${JSON.stringify(str)}`
                );
            }
            return localizationMap[key];
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