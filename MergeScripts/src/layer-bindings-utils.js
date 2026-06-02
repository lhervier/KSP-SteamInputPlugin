/**
 * Build a map from action-layer (preset) name to its 1-based position in the
 * preset list, as expected by the controller_action hold_layer binding.
 * Must be called after the presets are loaded but before the groups are inserted.
 * @param {*} vdf The merged VDF object
 * @returns {Object<string, number>} Map of preset name to 1-based position
 */
function buildLayerPosMap(vdf) {
    const map = {};
    const presets = vdf.controller_mappings.preset;
    for (let i = 0; i < presets.length; i++) {
        const name = presets[i].name;
        if (map[name] !== undefined) {
            throw new Error(`Duplicate preset name "${name}" while building the layer position map`);
        }
        // hold_layer expects a 1-based preset position (layer 0 is the base set)
        map[name] = i + 1;
    }
    return map;
}

module.exports = {
    buildLayerPosMap
}
