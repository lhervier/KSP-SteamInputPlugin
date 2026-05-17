const Handlebars = require('handlebars');

// ---------------------------------------------------------
// Handlebars helpers
// ---------------------------------------------------------

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

module.exports = {
    compileVdfSource
}