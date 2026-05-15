const fs = require('fs');
const path = require('path');
const { saveVdfFile, loadVdfFile } = require('./src/vdf-utils');
const { translateVdf } = require('./src/translate-utils');

const IN_GAME_ACTIONS_KEY = 'In Game Actions';

const [gameActionsVdfPath] = process.argv.slice(2);
if (!gameActionsVdfPath) {
    throw new Error(
        'Usage: node merge-game-actions.js <path/to/game_actions_220200.vdf>'
    );
}

const gameActionsPath = path.resolve(gameActionsVdfPath);
const configRoot = path.dirname(gameActionsPath);
const relativeGameActionsPath = '/' + path.relative(configRoot, gameActionsPath).replace(/\\/g, '/');
const merged = loadVdfFile(configRoot, relativeGameActionsPath);
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
    const outName = `game_actions_220200_${lang}.vdf`;
    saveVdfFile(translatedVdf, path.join(buildDir, outName));
}

console.log('game_actions VDF file(s) merged and saved successfully.');
