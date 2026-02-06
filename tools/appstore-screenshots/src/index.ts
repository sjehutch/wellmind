// CLI entry point.
// Run: node tools/appstore-screenshots/dist/index.js

import fs from "fs";
import path from "path";
import { buildLayout, getOutputSpec } from "./layout";
import { getTheme } from "./themes";
import { getDefaultCropMode, loadConfig, resolveInputPath } from "./config";
import { renderSlide } from "./render";

const args = process.argv.slice(2);
const watchMode = args.includes("--watch");

const projectRoot = path.resolve(__dirname, "..", "..", "..");
const configPath = path.join(projectRoot, "tools", "appstore-screenshots", "appstore.config.json");
const screenshotsDir = path.join(projectRoot, "artifacts", "ios", "screenshots");
const framesDir = path.join(projectRoot, "artifacts", "ios", "frames");
const outputDir = path.join(projectRoot, "artifacts", "ios", "appstore", "6.7");
const previewDir = path.join(projectRoot, "artifacts", "ios", "appstore", "previews");
const framePath = path.join(framesDir, "iphone-67.png");

async function runOnce(): Promise<void> {
  ensureDirs();

  const config = loadConfig(configPath);
  const outputSpec = getOutputSpec(config.outputSize);
  const layout = buildLayout(outputSpec);

  const inputFiles = fs.existsSync(screenshotsDir) ? fs.readdirSync(screenshotsDir) : [];
  if (inputFiles.length === 0) {
    console.log("No input screenshots found.");
    console.log(`Drop PNGs/JPGs into: ${screenshotsDir}`);
    console.log("Then re-run: npm run appstore:build");
    return;
  }

  for (const slide of config.slides) {
    const inputPath = resolveInputPath(projectRoot, slide.inputFile);
    if (!fs.existsSync(inputPath)) {
      console.log(`Skipping: ${slide.inputFile} (file not found)`);
      continue;
    }

    const outputPath = path.join(outputDir, slide.outputName);
    const previewPath = path.join(previewDir, slide.outputName);
    const theme = getTheme(slide.theme);

    await renderSlide({
      inputPath,
      outputPath,
      previewPath,
      headline: slide.headline,
      subheadline: slide.subheadline,
      theme,
      cropMode: getDefaultCropMode(slide),
      framePath: fs.existsSync(framePath) ? framePath : undefined,
      layout
    });

    console.log(`Generated: ${outputPath}`);
  }
}

function ensureDirs(): void {
  fs.mkdirSync(screenshotsDir, { recursive: true });
  fs.mkdirSync(framesDir, { recursive: true });
  fs.mkdirSync(outputDir, { recursive: true });
  fs.mkdirSync(previewDir, { recursive: true });
}

function startWatch(): void {
  console.log("Watch mode enabled. Waiting for changes...");

  let timer: NodeJS.Timeout | undefined;

  const schedule = () => {
    if (timer) {
      clearTimeout(timer);
    }
    timer = setTimeout(() => {
      runOnce().catch(err => console.error(err));
    }, 300);
  };

  fs.watch(configPath, schedule);

  if (fs.existsSync(screenshotsDir)) {
    fs.watch(screenshotsDir, schedule);
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
