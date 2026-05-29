const { loadVdfFile } = require('./vdf-utils');

/**
 * Parse a group_source_bindings source key, as authored in the presets, into its optional
 * modeshift id and its group file path.
 *
 * Examples:
 *   "/groups/joystick/mouse.vdf"                  -> { modeshiftId: null,   path: "/groups/joystick/mouse.vdf" }
 *   "main:/groups/joystick/eva-run.vdf?with-mod"  -> { modeshiftId: "main", path: "/groups/joystick/eva-run.vdf?with-mod" }
 *
 * @param {string} key The source key
 * @returns {{ modeshiftId: (string|null), path: string }}
 */
function _parseSourceKey(key) {
    // The id is written before the path ("<id>:/path"). Group paths always start with "/"
    // and ids never contain "/" nor ":", so the first ":" is the separator - a later ":"
    // inside the path's query string can't be mistaken for it.
    if (key.startsWith('/')) {
        return { modeshiftId: null, path: key };
    }
    const idx = key.indexOf(':');
    if (idx === -1) {
        // Not an absolute path and no id: leave it untouched, loadVdfFile will report it.
        return { modeshiftId: null, path: key };
    }
    return { modeshiftId: key.substring(0, idx), path: key.substring(idx + 1) };
}

/**
 * Resolve, for every preset, the group files referenced by its "group_source_bindings":
 * each referenced group is loaded, given a unique numeric id and registered in the global
 * "group" list. Modeshift declarations are validated along the way.
 *
 * @param {*} vdf The VDF object to resolve
 * @param {string} configRoot The root directory of the VDF files
 * @param {object} context The Handlebars context
 */
function resolvePresets(vdf, configRoot, context) {
    if (!vdf.controller_mappings.group) {
        vdf.controller_mappings.group = [];
    }
    let nextGroupId = 0;
    let nextPresetId = 0;
    for (const preset of vdf.controller_mappings.preset) {
        const result = {};
        const seenModeshifts = new Set();
        for (const [key, value] of Object.entries(preset.group_source_bindings)) {
            const { modeshiftId, path } = _parseSourceKey(key);
            const bindings = Array.isArray(value) ? value : [value];
            for (const binding of bindings) {
                _validateDeclaration(preset.name, path, binding, modeshiftId, seenModeshifts);

                const merged = loadVdfFile(configRoot, path, context);
                if( !merged.ref || !merged.ref.group ) {
                    throw new Error(`Group file ${path} must have "ref" > "group" as root content`);
                }
                const group = merged.ref.group;
                group.id = nextGroupId++ + "";
                group.filepath = path;
                vdf.controller_mappings.group.push(group);

                // Rewrite the key to "<groupId>[:<modeshiftId>]". The modeshift id is kept
                // (now after the group id) so phase 3 can tell which group a
                // "mode_shift <zone> <id>" binding targets; phase 4 strips it afterwards.
                const resultKey = modeshiftId ? group.id + ":" + modeshiftId : group.id;
                result[resultKey] = binding;
            }
        }
        preset.id = nextPresetId++ + "";
        preset.group_source_bindings = result;
    }
}

/**
 * Validate a single group_source_bindings declaration, tracking the (zone, modeshiftId)
 * pairs already seen in the current preset to reject duplicates.
 *
 * @param {string} presetName The preset name (for error messages)
 * @param {string} path The group file path (for error messages)
 * @param {string} binding The binding label (e.g. "joystick active modeshift")
 * @param {(string|null)} modeshiftId The modeshift id parsed from the key, or null
 * @param {Set<string>} seenModeshifts The (zone, id) pairs already seen in this preset
 * @throws {Error} On a missing id, a stray id, or a duplicate (zone, id) declaration
 */
function _validateDeclaration(presetName, path, binding, modeshiftId, seenModeshifts) {
    // A declaration is a modeshift when its label carries "modeshift". A modeshift must have
    // an id (so a binding can target it); an active group must not.
    const isModeshift = binding.includes('modeshift');
    const zone = binding.split(' ')[0];

    if (isModeshift && !modeshiftId) {
        throw new Error(`Preset ${presetName}: modeshift declaration for zone "${zone}" (${path}) has no modeshift id (expected "<id>:/path")`);
    }
    if (!isModeshift && modeshiftId) {
        throw new Error(`Preset ${presetName}: active group "${path}" must not carry a modeshift id "${modeshiftId}"`);
    }
    if (isModeshift) {
        const pair = zone + ":" + modeshiftId;
        if (seenModeshifts.has(pair)) {
            throw new Error(`Preset ${presetName}: duplicate modeshift declaration for zone "${zone}" id "${modeshiftId}"`);
        }
        seenModeshifts.add(pair);
    }
}

/**
 * Strip the modeshift id from the intermediate "group_source_bindings" keys, leaving only
 * the numeric group id required by Valve. Must run *after* resolveGroupBindings, which needs
 * the id to disambiguate modeshift targets.
 * @param {*} vdf The VDF object to clean up
 */
function cleanupPresetGroupKeys(vdf) {
    for (const preset of vdf.controller_mappings.preset) {
        const cleaned = {};
        for (const [key, value] of Object.entries(preset.group_source_bindings)) {
            const groupId = key.split(':')[0];
            if (cleaned[groupId] !== undefined) {
                throw new Error(`Preset ${preset.name}: group id collision on "${groupId}" while cleaning up keys`);
            }
            cleaned[groupId] = value;
        }
        preset.group_source_bindings = cleaned;
    }
}

module.exports = {
    resolvePresets,
    cleanupPresetGroupKeys
}
