"use strict";
// CLI entry point.
// Run: node tools/appstore-screenshots/dist/index.js
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
const fs_1 = __importDefault(require("fs"));
const path_1 = __importDefault(require("path"));
const layout_1 = require("./layout");
const themes_1 = require("./themes");
const config_1 = require("./config");
const render_1 = require("./render");
const args = process.argv.slice(2);
const watchMode = args.includes("--watch");
const projectRoot = path_1.default.resolve(__dirname, "..", "..", "..");
const configPath = path_1.default.join(projectRoot, "tools", "appstore-screenshots", "appstore.config.json");
const screenshotsDir = path_1.default.join(projectRoot, "artifacts", "ios", "screenshots");
const framesDir = path_1.default.join(projectRoot, "artifacts", "ios", "frames");
const outputDir = path_1.default.join(projectRoot, "artifacts", "ios", "appstore", "6.7");
const previewDir = path_1.default.join(projectRoot, "artifacts", "ios", "appstore", "previews");
const framePath = path_1.default.join(framesDir, "iphone-67.png");
async function runOnce() {
    ensureDirs();
    const config = (0, config_1.loadConfig)(configPath);
    const outputSpec = (0, layout_1.getOutputSpec)(config.outputSize);
    const layout = (0, layout_1.buildLayout)(outputSpec);
    const inputFiles = fs_1.default.existsSync(screenshotsDir) ? fs_1.default.readdirSync(screenshotsDir) : [];
    if (inputFiles.length === 0) {
        console.log("No input screenshots found.");
        console.log(`Drop PNGs/JPGs into: ${screenshotsDir}`);
        console.log("Then re-run: npm run appstore:build");
        return;
    }
    for (const slide of config.slides) {
        const inputPath = (0, config_1.resolveInputPath)(projectRoot, slide.inputFile);
        if (!fs_1.default.existsSync(inputPath)) {
            console.log(`Skipping: ${slide.inputFile} (file not found)`);
            continue;
        }
        const outputPath = path_1.default.join(outputDir, slide.outputName);
        const previewPath = path_1.default.join(previewDir, slide.outputName);
        const theme = (0, themes_1.getTheme)(slide.theme);
        await (0, render_1.renderSlide)({
            inputPath,
            outputPath,
            previewPath,
            headline: slide.headline,
            subheadline: slide.subheadline,
            theme,
            cropMode: (0, config_1.getDefaultCropMode)(slide),
            framePath: fs_1.default.existsSync(framePath) ? framePath : undefined,
            layout
        });
        console.log(`Generated: ${outputPath}`);
    }
}
function ensureDirs() {
    fs_1.default.mkdirSync(screenshotsDir, { recursive: true });
    fs_1.default.mkdirSync(framesDir, { recursive: true });
    fs_1.default.mkdirSync(outputDir, { recursive: true });
    fs_1.default.mkdirSync(previewDir, { recursive: true });
}
function startWatch() {
    console.log("Watch mode enabled. Waiting for changes...");
    let timer;
    const schedule = () => {
        if (timer) {
            clearTimeout(timer);
        }
        timer = setTimeout(() => {
            runOnce().catch(err => console.error(err));
        }, 300);
    };
    fs_1.default.watch(configPath, schedule);
    if (fs_1.default.existsSync(screenshotsDir)) {
        fs_1.default.watch(screenshotsDir, schedule);
    }
    schedule();
}
runOnce()
    .then(() => {
    if (watchMode) {
        startWatch();
    }
})
    .catch(err => {
    console.error(err);
    process.exit(1);
});
