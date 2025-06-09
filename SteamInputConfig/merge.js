const fs = require('fs');
const path = require('path');
const VDF = require('vdf-parser');

let groupIdCounter = 0;
let presetIdCounter = 0;

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

    // If it's an _action.vdf file, deduce the preset name from the parent directory
    if (relativePath.endsWith('_action.vdf')) {
        const presetName = path.basename(path.dirname(relativePath)).split('-')[1];
        content = `"${presetName}"\n${content}`;
    }
    // If it's a _group.vdf file, add the "group" header
    else if (relativePath.endsWith('_group.vdf')) {
        content = '"group"\n' + content;
    }
    // If it's a file without header, deduce it from the file name
    else if (content.trim().startsWith('{') ) {
        const fileName = path.basename(relativePath);
        const inputName = fileName.includes(' - ') ? fileName.split(' - ')[0] : fileName.replace('.vdf', '');
        content = `"${inputName}"\n${content}`;
    }

    try {
        return VDF.parse(content);
    } catch (error) {
        throw new Error(`Erreur lors du parsing de ${filePath}: ${error.message}`);
    }
}

/**
 * Process a language directory
 * @param {string} languageDir - Directory containing the localization VDF files
 * @returns {Object} Localization data present in the directory
 * @throws {Error} If the directory cannot be processed
 */
function processLanguageDirectory(languageDir) {
    const localizationData = {};
    fs.readdirSync(languageDir)
        .filter(file => file.endsWith('.vdf'))
        .forEach(languageFile => {
            const parsedContent = loadVdfFile(languageDir, languageFile);
            Object.assign(localizationData, parsedContent[languageFile.replace('.vdf', '')]);
        });
    return localizationData;
}

/**
 * Load the base template
 * @param {string} baseDir - Base directory
 * @returns {Object} Template data
 * @throws {Error} If the template cannot be loaded
 */
function loadTemplate(baseDir) {
    const vdf = loadVdfFile(baseDir, 'controller_mappings.vdf');
    vdf.controller_mappings.version = "3";
    vdf.controller_mappings.revision = "1";
    vdf.controller_mappings.progenitor = "";
    vdf.controller_mappings.export_type = "personal_local";
    vdf.controller_mappings.major_revision = "0";
    vdf.controller_mappings.minor_revision = "0";
    vdf.controller_mappings.actions = {};
    vdf.controller_mappings.action_layers = {};
    vdf.controller_mappings.localization = {};
    vdf.controller_mappings.settings = {};
    return vdf;
}

/**
 * Process all action files of a controller
 * @param {string} controllerDir - Controller directory
 * @returns {Object} Merged action data
 * @throws {Error} If the actions cannot be processed
 */
function processActions(controllerDir) {
    const actionsData = {};
    fs.readdirSync(controllerDir)
        .filter(file => fs.statSync(path.join(controllerDir, file)).isDirectory())
        .sort() // Trie naturellement les dossiers par numéro
        .forEach(presetDir => {
            const relativePath = path.join(presetDir, '_action.vdf');
            const actionData = loadVdfFile(controllerDir, relativePath);
            Object.assign(actionsData, actionData);
        });
    return actionsData;
}

/**
 * Process all inputs of a group
 * @param {string} baseDir - Base directory
 * @param {string} groupDir - Group directory
 * @returns {Object} Object containing the inputs as sub-properties
 * @throws {Error} If the inputs cannot be processed
 */
function processInputs(baseDir, groupDir) {
    const inputs = {};
    
    // Read all VDF files in the directory
    const inputFiles = fs.readdirSync(groupDir)
        .filter(file => file.endsWith('.vdf') && file !== '_group.vdf')
        .sort();

    inputFiles.forEach(inputFile => {
        // Load and parse the file
        const inputPath = path.join(groupDir, inputFile);
        const inputData = loadVdfFile(baseDir, path.relative(baseDir, inputPath));
        
        // Get the first (and unique) input of the object
        const inputName = Object.keys(inputData)[0];
        inputs[inputName] = inputData[inputName];
    });

    return inputs;
}

/**
 * Process all groups of a preset
 * @param {string} baseDir - Base directory
 * @param {string} presetDir - Preset directory
 * @returns {Object} Object containing the groups and their bindings
 * @throws {Error} If the groups cannot be processed
 */
function processGroups(baseDir, presetDir) {
    const groups = [];
    const groupBindings = {};

    // Read all subdirectories of groups
    const groupDirs = fs.readdirSync(presetDir)
        .filter(file => fs.statSync(path.join(presetDir, file)).isDirectory())
        .sort();

    groupDirs.forEach(groupType => {
        const groupPath = path.join(presetDir, groupType, '_group.vdf');
        if (!fs.existsSync(groupPath)) {
            throw new Error(`_group file not found: ${groupPath}`);
        }
        
        const groupData = loadVdfFile(baseDir, path.relative(baseDir, groupPath));
        
        // Add the ID to the group
        const groupId = groupIdCounter.toString();
        groupData.group.id = groupId;
        
        // Process the inputs of the group
        const inputs = processInputs(baseDir, path.join(presetDir, groupType));
        groupData.group.inputs = inputs;
        
        // Add the group to the list
        groups.push(groupData.group);
        
        // Add the binding in the preset
        groupBindings[groupId] = groupType;
        
        groupIdCounter++;
    });

    return {
        groups,
        groupBindings
    };
}

/**
 * Process all presets
 * @param {string} controllerDir - Controller directory
 * @returns {Object[]} List of presets with their groups
 * @throws {Error} If the presets cannot be processed
 */
function processPresets(controllerDir) {
    const presets = [];
    const allGroups = [];
    
    // Read all preset directories
    fs.readdirSync(controllerDir)
        .filter(file => fs.statSync(path.join(controllerDir, file)).isDirectory())
        .sort() // Sort the directories naturally by number
        .forEach(presetDir => {
            const presetName = path.basename(presetDir).split('-')[1];
            const presetData = {
                name: presetName,
                id: presetIdCounter.toString()
            };
            
            // Process the groups of the preset
            const presetPath = path.join(controllerDir, presetDir);
            const { groups, groupBindings } = processGroups(controllerDir, presetPath);
            
            // Add the groups to the global list
            allGroups.push(...groups);
            
            // Add the bindings to the preset
            const groupSourceBindings = {};
            presetData.group_source_bindings = groupBindings;
            
            presets.push(presetData);
            presetIdCounter++;
        });

    return {
        presets,
        groups: allGroups
    };
}

/**
 * Process all bindings
 * Replace the %ID% references in the bindings by the corresponding group IDs
 * @param {Object[]} groups - List of groups
 * @param {Object[]} presets - List of presets
 * @throws {Error} If a referenced group is not found in the preset
 */
function processBindings(groups, presets) {
    // Create an index of presets by group ID
	// and an index of group IDs by type for each preset
    const presetByGroupId = {};
    const groupIdByTypeByPreset = {};
    presets.forEach(p => {
        groupIdByTypeByPreset[p.name] = {};
        Object.entries(p.group_source_bindings).forEach(([groupId, bindingValue]) => {
            presetByGroupId[groupId] = p;
            if( !bindingValue.endsWith('modeshift')) {
				return;
			}
			const groupType = bindingValue.split(' ')[0];
			if( groupIdByTypeByPreset[p.name][groupType] ) {
				throw new Error(`Groupe de type ${groupType} en mode_shift déjà défini dans le preset ${p.name}. On ne supporte pas cette configuration...`);
			}
            groupIdByTypeByPreset[p.name][groupType] = groupId;
        });
    });

    // For each group
    groups.forEach(g  => {
        // For each input of the group
        if (!g.inputs) return;
		
		Object.values(g.inputs).forEach(input => {
			if (!input.activators) return;

			// For each activator of the input (the same activator can appear several times)
			Object.values(input.activators).forEach(activator => {
				const activators = Array.isArray(activator) ? activator : [activator];
				activators.forEach(a => {
					if (!a.bindings) return;

					// For each binding (several bindings can be defined for the same activator)
					Object.entries(a.bindings).forEach(([key, binding]) => {
						const bindings = Array.isArray(binding) ? binding : [binding];
						bindings.forEach((value, index) => {
							
							if (value.startsWith('mode_shift ') && value.includes('%ID%')) {
							
								// We search the preset that contains this group
								const groupPreset = presetByGroupId[g.id];
								if (!groupPreset) return; // Ignore groups that are not in any preset

								// We deduce the group type for the mode change
								const groupType = value.split(' ')[1];
								
								// We search the ID of the group of type groupType in this preset
								const targetGroupId = groupIdByTypeByPreset[groupPreset.name][groupType];
								if (!targetGroupId) {
									throw new Error(`Group of type ${groupType} not found in preset ${groupPreset.name}`);
								}
								value = value.replace(/%ID%/, targetGroupId);
							}
							
							// Replace %ID% by the target group ID
							if (Array.isArray(binding)) {
								binding[index] = value;
							} else {
								a.bindings.binding = value;
							}
						});
					});
				});
			});
		});
    });
}

/**
 * Process a controller directory
 * @param {string} controllerDir - Path of the directory containing the controller definition
 * @param {Object} localizationData - Localization data
 * @returns {Object} Controller data
 * @throws {Error} If the directory cannot be processed
 */
function processControllerDirectory(controllerDir, localizationData) {
    try {
        // Load the template
        const templateData = loadTemplate(controllerDir);
        
        // Add the localization keys
        templateData.controller_mappings.localization = localizationData;
        
        // Process the actions
        templateData.controller_mappings.actions = processActions(controllerDir);
        
        // Process the presets and the groups
        const { presets, groups } = processPresets(controllerDir);
        templateData.controller_mappings.preset = presets;
        templateData.controller_mappings.group = groups;
        
        // Process the bindings
        processBindings(
			groups, 
			presets
		);

		templateData.controller_mappings.Timestamp = "" + Date.now();
        
        return templateData;
    } catch (error) {
        console.error(`Erreur lors du traitement du dossier ${controllerDir}: ${error.message}`);
        process.exit(1);
    }
}

function localizeVdf(vdf, languageData) {
    // The VDF object has only one property, but its name is not always the same
    // depending on that we are localizing a controller or an game_actions file
    // So, just find the value of the first property
    const root = Object.values(vdf)[0];
    root.localization = {};
    
    // Localize the actions titles
    if( root.actions ) {
        Object.entries(root.actions)
            .forEach(([mappingKey, mappingValue]) => {
                if( !mappingValue.title.startsWith('#') ) return;
                const key = mappingValue.title.slice(1);
                const translation = languageData[key];
                if( translation ) {
                    mappingValue.title = translation;
                } else {
                    console.log(`WARN : Key ${key} not found`);
                }
            });
    }
    
    // Localize the bindings
    if( root.group ) {
        root.group
            .forEach(group => {
                if( !group.inputs ) return;
                Object.entries(group.inputs)
                    .forEach(([inputKey, inputValue]) => {
                        if( !inputValue.activators ) return;
                        Object.entries(inputValue.activators)
                            .forEach(([activatorKey, activatorValue]) => {
                                if( !activatorValue.bindings ) return;
                                if( !activatorValue.bindings.binding) return;
                                if( activatorValue.bindings.binding.indexOf('#') === -1 ) return;

                                const binding = activatorValue.bindings.binding;
                                let key = binding.split('#')[1];
                                if( key.indexOf(',') !== -1 ) {
                                    key = key.split(',')[0];
                                }
                                const translation = languageData[key];
                                if( translation ) {
                                    activatorValue.bindings.binding = binding.replace(`#${key}`, translation);
                                } else {
                                    console.log(`WARN : Key ${key} not found`);
                                }
                            });
                    });
            });
    }
    return vdf;
}

// ==========================================================================
// Entry point of the script
// ==========================================================================

// Remove all existing vdf files
fs.readdirSync(".")
    .filter(file => file.endsWith(".vdf"))
    .forEach(file => {
        fs.unlinkSync(file);
    });

// Load the localizations
const localizationData = {};
fs.readdirSync("localization")
    .filter(file => fs.statSync(path.join("localization", file)).isDirectory())
    .forEach(language => {
        const languageDir = path.join("localization", language);
        localizationData[language] = processLanguageDirectory(languageDir);
    });
console.log("Localizations loaded");

// Generate the controllers files
fs.readdirSync("controllers")
    .filter(file => fs.statSync(path.join("controllers", file)).isDirectory())
    .forEach(controllerName => {
        const controllerVdf = processControllerDirectory(
            path.join("controllers", controllerName), 
            localizationData
        );
        
        const outputPath = path.join(`controller_${controllerName}.vdf`);
        saveVdfFile(controllerVdf, outputPath);
        
        console.log(`File created successfully: ${outputPath}`);
    });

// Generate the game_actions file
fs.readdirSync("game_actions")
    .filter(file => file.endsWith(".vdf"))
    .filter(file => file.startsWith("game_actions_"))
    .forEach(gameActionsFile => {
        const gameActionsVdf = loadVdfFile("game_actions", gameActionsFile);
        gameActionsVdf.localization = localizationData;
        saveVdfFile(gameActionsVdf, gameActionsFile);
        console.log(`File created successfully: ${gameActionsFile}`);
    });

// Generate the individually localized controller files
fs.readdirSync(".")
    .filter(file => file.endsWith(".vdf"))
    .filter(file => file.startsWith("controller_"))
    .forEach(controllerFile => {
        Object.entries(localizationData)
            .forEach(([languageName, languageData]) => {
                const controllerVdf = loadVdfFile(".", controllerFile);
                const localizedControllerVdf = localizeVdf(controllerVdf, languageData);
                const outputPath = path.join(`${controllerFile.replace('.vdf', '')}_${languageName}.vdf`);
                saveVdfFile(localizedControllerVdf, outputPath);
                console.log(`File created successfully: ${outputPath}`);
            });
    });

// Generate the individual game_actions files
fs.readdirSync(".")
    .filter(file => file.endsWith(".vdf"))
    .filter(file => file.startsWith("game_actions_"))
    .forEach(gameActionsFile => {
        Object.entries(localizationData)
            .forEach(([languageName, languageData]) => {
                const gameActionsVdf = loadVdfFile(".", gameActionsFile);
                const localizedGameActionsVdf = localizeVdf(gameActionsVdf, languageData);
                const outputPath = path.join(`${gameActionsFile.replace('.vdf', '')}_${languageName}.vdf`);
                saveVdfFile(localizedGameActionsVdf, outputPath);
                console.log(`File created successfully: ${outputPath}`);
            });
    });
