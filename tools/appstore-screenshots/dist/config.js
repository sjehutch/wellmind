"use strict";
// Loads and validates the appstore.config.json file.
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.loadConfig = loadConfig;
exports.getDefaultCropMode = getDefaultCropMode;
exports.resolveInputPath = resolveInputPath;
const fs_1 = __importDefault(require("fs"));
const path_1 = __importDefault(require("path"));
function loadConfig(configPath) {
    if (!fs_1.default.existsSync(configPath)) {
        throw new Error(`Config file not found: ${configPath}`);
    }
    const raw = fs_1.default.readFileSync(configPath, "utf8");
    const parsed = JSON.parse(raw);
    validateConfig(parsed);
    return parsed;
}
function validateConfig(config) {
    if (!config.outputSize) {
        throw new Error("config.outputSize is required");
    }
    if (!Array.isArray(config.slides) || config.slides.length === 0) {
        throw new Error("config.slides must be a non-empty array");
    }
    config.slides.forEach((slide, index) => {
        if (!slide.inputFile) {
            throw new Error(`slides[${index}].inputFile is required`);
        }
        if (!slide.headline) {
            throw new Error(`slides[${index}].headline is required`);
        }
        if (!slide.theme) {
            throw new Error(`slides[${index}].theme is required`);
        }
        if (!slide.outputName) {
            throw new Error(`slides[${index}].outputName is required`);
        }
        if (slide.cropMode && slide.cropMode !== "contain" && slide.cropMode !== "cover") {
            throw new Error(`slides[${index}].cropMode must be "contain" or "cover"`);
        }
    });
}
function getDefaultCropMode(slide) {
    return slide.cropMode ?? "contain";
}
function resolveInputPath(projectRoot, inputFile) {
    return path_1.default.join(projectRoot, "artifacts", "ios", "screenshots", inputFile);
}
