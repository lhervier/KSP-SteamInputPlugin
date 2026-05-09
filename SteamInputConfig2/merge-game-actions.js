const fs = require('fs');
const path = require('path');
const { getVersion } = require('./version-utils');
const { resetIds, saveVdfFile, loadVdfFile, translateVdf } = require('./vdf-utils');

const IN_GAME_ACTIONS_KEY = 'In Game Actions';

const buildVersion = getVersion();

const rootPath = path.join(__dirname, 'game_actions_220200.vdf');
resetIds();
const merged = loadVdfFile(rootPath, '');
if (!merged[IN_GAME_ACTIONS_KEY] || !merged.localization) {
    throw new Error('game_actions_220200.vdf: missing "In Game Actions" or "localization" after resolving #ref');
}

const buildDir = path.join(__dirname, 'build');
if (!fs.existsSync(buildDir)) {
    fs.mkdirSync(buildDir, { recursive: true });
}

const languages = Object.keys(merged.localization);

for (const lang of languages) {
    const langDict = merged.localization[lang];
    const translatedVdf = translateVdf(merged, langDict);
    const outName = `game_actions_220200_${lang}_${buildVersion}.vdf`;
    saveVdfFile(translatedVdf, path.join(buildDir, outName));
}

console.log('game_actions VDF file(s) merged and saved successfully.');
